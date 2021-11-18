using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using Microsoft.ML;

namespace godot_net_server
{
    public class GodotServer
    {
        HttpListener httpListener;
        WebSocket Socket;
        MLContext mlContext = new MLContext();
        OnnxModelScorer modelScorer;
        static readonly CancellationTokenSource s_cts = new CancellationTokenSource();
        public GodotServer(string ip = "127.0.0.1", int port = 8001)
        {
            modelScorer = new OnnxModelScorer("my_ppo_1_model.onnx", mlContext);
            s_cts.CancelAfter(5000);
            httpListener = new HttpListener();
            httpListener.Prefixes.Add($"http://{ip}:{port}/");
            httpListener.Start();

            // Load trained model

        }
        public async Task Start()
        {
            bool landed = false;

            while (Socket == null)
            {
                HttpListenerContext context = await httpListener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);
                    Socket = webSocketContext.WebSocket;
                    var message = await reset();
                    var action = modelScorer.Score(message.InitObservation);
                    while (true)
                    {
                        await render();
                        if (!landed)
                        {
                            var action_command = action.ToList().IndexOf(action.ToList().Max());


                            message = await step(action_command);
                            var yposition = message.Observation[1];
                            action = modelScorer.Score(message.Observation);
                            if (yposition < 1.709)
                            {
                                landed = true;
                                action.Select(c => { c  = 0.0f; return c; }).ToList();
                                message = await step(action_command);
                            }
                            Console.WriteLine("Lander Y position: " + message.Observation[1].ToString());
                        }

                    }
                }
            }
        }
        public async Task<string> SendMessageAndGetReply(string message)
        {
            await Socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)),
                WebSocketMessageType.Text, true, CancellationToken.None);
            var incominingBuffer = new ArraySegment<byte>(new byte[1000]);
            await Socket.ReceiveAsync(incominingBuffer, new CancellationToken()).ContinueWith(x =>
             {
                 Console.WriteLine(x.Exception);
             });
            return Encoding.UTF8.GetString(incominingBuffer);
        }
        string make_command(string command)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(new { cmd = command });
        }
        string make_command(string command, int action)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(new { cmd = command, action = new List<int>() { action } });
        }
        public async Task<GodotMessage> reset()
        {
            var resp = await SendMessageAndGetReply(make_command("reset"));
            return Newtonsoft.Json.JsonConvert.DeserializeObject<GodotMessage>(resp);
        }
        public async Task<GodotMessage> step(int action)
        {
            /*
             * answer = self._sendAndGetAnswer(
            {'cmd': 'step', 'action': action.tolist()})
            observation_np = np.array(answer['observation']).astype(np.float32)
             */
            var resp = await SendMessageAndGetReply(make_command("step", action));

            return Newtonsoft.Json.JsonConvert.DeserializeObject<GodotMessage>(resp); ;
        }
        public void close()
        {

        }
        public async Task render()
        {
            var resp = await SendMessageAndGetReply(make_command("render"));
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
