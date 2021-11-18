using System;
using System.Collections.Generic;
using System.Linq;

using Godot;
using Microsoft.ML;
using Newtonsoft.Json;
using Array = Godot.Collections.Array;

namespace GodotGym
{

    public class GymONNX : Node
    {

        [Export]
        NodePath environmentNode = null;

        [Export]
        NodePath landerNode = null;

        [Export]
        int stepLength = 2;

        [Export]
        float hoverStrength = 2.5f;

        [Export]
        float hoverMax = 8;

        [Export]
        float hoverMin = 4;

        private Node environment;
        private Node lander;

        private OnnxModelScorer modelScorer;


        private int frameCounter = 0;
        private float yPosition = 10;

        private Array currentAction;
        private int action_command;

        private Array observation;
        private float reward = 0f;
        private bool isDone = false;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {

            currentAction = new Array();
            observation = new Array();
            environment = GetNode(environmentNode); //GetNode("Env");
            lander = GetNode(landerNode); //GetNode("Env");

            //This node will never be paused
            this.PauseMode = PauseModeEnum.Process;

            //Initialy, the environment is paused
            GetTree().Paused = true;


            MLContext mlContext = new MLContext();
            modelScorer = new OnnxModelScorer("StarShipLander.onnx", mlContext);

            environment.Call("reset");

        }

        //Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta)
        {

            if (yPosition > 1.74f && !isDone)
            {

                //Simulate stepLength frames with the current action.
                //Then pause the game and return the observation/reward/isDone to the server
                if (frameCounter >= stepLength)
                {

                    if(yPosition < hoverMax && yPosition >= hoverMin) { lander.Call("mainengine", hoverStrength);  }
                    else{ lander.Call("mainengine", 1.0); }


                    //GetTree().Paused = true;
                    frameCounter = 0;
                    communicateWithEnvironment();
                }
                else
                {

                    if (frameCounter < stepLength -1) { GetTree().Paused = true; }

                    if (GetTree().Paused == true)
                    {

                        frameCounter += 1;

                    }
                }
            }
            else
            {
                if (!isDone)
                {

                        currentAction = new Array();
                        currentAction.Add(5);
                        environment.Call("apply_action", currentAction);
                        environment.Call("has_landed");
                        isDone = true;
                    
                }
            }
        }

        /// <summary>
        /// Return current observation/reward/isDone to the GymONNX
        /// </summary>
        private void communicateWithEnvironment()
        {
            //Receive get_observation from Environment 
            observation = (Array)(environment.Call("get_observation"));
            yPosition = (float)(observation[1]);
            Console.WriteLine("Y Position: " + yPosition);
            //ML.NET inference

            var temp = modelScorer.Score(ArrayToFloatList(observation));
            currentAction = FloatArrayToArray(temp);

            //environment.apply_action
            step(currentAction);

            reward = (float)(environment.Call("get_reward"));

            Console.WriteLine("Reward: " + reward);

            isDone = (bool)(environment.Call("is_done"));



        }

        #region message from Environment.gd => communicateWithEnvironment
        public void reset()
        {
            environment.Call("reset");
            observation = (Array)(environment.Call("get_observation"));


        }
        public void step(Array action)
        {
            action_command = ArrayToFloatList(action).IndexOf(ArrayToFloatList(action).Max()); ;
            GetTree().Paused = false;
            Array applyAction = new Array();
            PrintCurrentAction(action_command);
            applyAction.Add(action_command);
            environment.Call("apply_action", applyAction);

        }

        private void PrintCurrentAction(int action_command)
        {
            switch (action_command)
            {
                case 0:
                    Console.WriteLine("Main engine fire ...");
                    break;
                case 1:
                    Console.WriteLine("AuxX engine fire ...");
                    break;
                case 2:
                    Console.WriteLine("AuxXn engine fire ..");
                    break;
                case 3:
                    Console.WriteLine("AuxZ engine fire ...");
                    break;
                case 4:
                    Console.WriteLine("AuxZn engine fire ...");
                    break;
                default:
                    break;
            }
        }

        public void close()
        {
            GetTree().Quit();

        }
        #endregion

        private Array FloatArrayToArray(IEnumerable<float> action)
        {
            Array tmpArray = new Array();
            foreach (var item in action)
            {
                tmpArray.Add(item);
            }
            return tmpArray;
        }
        private List<float> ArrayToFloatList(Array inObs)
        {
            List<float> floatArray = new List<float>();
            foreach (var item in inObs)
            {
                floatArray.Add((float)item);
            }

            return floatArray;
        }

        public class GodotMessage
        {
            [JsonProperty("init_observation")]
            public List<float> InitObservation { get; set; }
            [JsonProperty("observation")]
            public List<float> Observation { get; set; }
            [JsonProperty("reward")]
            public float Reward { get; set; }
        }
    }
}