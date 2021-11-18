# GodotServerDotnet
Having a server that can provide predictions over websockets or network allows us to integrate the ML model where we cant run mlnet. Before consuming out model directly, we used such server to test out model in mlnet with the godot engine.

[Godot Engine](https://godotengine.org/) as an [OpenAI Gym](https://github.com/openai/gym) environment for Reinforcement Learning.

## Overview

![overview](https://github.com/HugoTini/GymGodot/blob/main/overview.svg)

### `gym-godot` : Godot Side (client)
- The '_Environment node_', created by the user, implements the required methods (`execute_action()`, `get_observation()`, `get_reward()`, `reset()` and `is_done()`).
- The `GymGodot` node (`GymGodot.tscn`) bridges the '_Environment node_' node and the Python side server.

### `GodotServer` : Dotnet Side (server)
- `GodotServer` communicates with its Godot client and exposes it as a Gym environment.

Communications between the server and client are done with WebSocket JSON messages ([protocol.md](protocol.md))

## Installation & Usage

- Download or clone this repo.
- Add `GymGodot.tscn`, `GymGodot.gd` and `WebSocketClient.gd` from `/gym-godot` to your Godot project folder. Then add the `GymGodot.tscn` node into your scene.
- Create a node (the '_Environment node_') that implements the required functions.
- In the inspector, set GymGodot Node's '_Environment Node_' property to your '_Environment node_'.
- Replace the paths to your paths and add the follwing code before server.Start() in Program.cs
```
var start_godot = @"/C cd C:/Users/Shehroze/source/repos/GymGodot/gym-godot && E:/godot/Godot/Godot_v3.4-stable_mono_win64.exe ./examples/mars_lander/Root.tscn --fixed-fps 60 --disable-render-loop --serverIP=127.0.0.1 --serverPort=8001 --renderPath=C:\Users\Shehroze\source\repos\GymGodot/render_frames/";
System.Diagnostics.Process.Start("CMD.exe", start_godot);
```

## Example environments

### Mars Lander

`gym-godot/examples/mars_lander/`

![mars_lander](./gym-godot/examples/mars_lander/output.gif)

Description : [mars_lander.md](gym-godot/examples/mars_lander/mars_lander.md)

## Notes

- Make sure to open the project (`gym-godot/project.godot`) in the Godot Editor at least once before using the example environments (so that the resources are imported).

- The code follows the Gym API so it might work with other Gym-compatible frameworks but has only been tested with [Stable-Baselines 3](https://github.com/DLR-RM/stable-baselines3).

