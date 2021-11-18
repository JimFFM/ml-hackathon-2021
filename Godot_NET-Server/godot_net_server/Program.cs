using System;
using System.Linq;
using Microsoft.ML;

namespace godot_net_server
{
    class Program
    {
        static void Main(string[] args)
        {
            GodotServer server = new GodotServer(port: 8001);
            while (true)
            {
                try
                {
                    //var start_godot = @"/H cd H:/00-Test/2021-Hackathon-Submit/GymGodot//gym-godot && H:/00-Test/Godot_v3.4-stable_mono_win64//Godot_v3.4-stable_mono_win64.exe ./examples/mars_lander//Root.tscn --fixed-fps 60 --disable-render-loop --serverIP=127.0.0.1 --serverPort=8001 --renderPath=H:/00-Test/2021-Hackathon-Submit/GymGodot//render_frames/";
                    server.Start().Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

    }
}
