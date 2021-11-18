# Virtual ML.NET 2021 Hackathon Submission 

[Virtual on November 12-19, 2021](https://github.com/virtualmlnet/hackathon-2021)

# What we have done:

## Deliverable 1: 
#### A workflow to train PyTorch OpenAI Gym (stable-baselines3 or SB3) model using Godot environment on Windows

The provided GymGodot is being tested ONLY on Linux & Godot 3.3
- [x] We hacked the provided python script solution to make them **work on Windows**

## Deliverable 2: 
#### A working example how to export SB3 ONNX
- [x] We learned that it was not trivial to [simply export PyTorch model to ONNX](https://pytorch.org/tutorials/advanced/super_resolution_with_onnxruntime.html) due to [no broadcast_tensors opperator support discussed here](https://github.com/onnx/onnx/issues/3033) 
- [x] We follow the [instruction](https://stable-baselines3.readthedocs.io/en/master/guide/export.html) and added extra pytorch codes and attached [model_to_onnx.py](https://github.com/JimFFM/ml-hackathon-2021/blob/main/PyTorchTrainingONNXExport/model_to_onnx.py) to export the trained pytorch model to [StarShipLander.ONNX](https://github.com/JimFFM/ml-hackathon-2021/blob/main/GymRogueOne/StarShipLander.onnx)
 
## Deliverable 3: 
#### A .NET WebSocket Server serving RL inference using SB3 ONNX applicable for e.g. Unity3D
- [x] We replace the provided python Gym_server with [godot_net_server](https://github.com/JimFFM/ml-hackathon-2021/tree/main/Godot_NET-Server/godot_net_server)
- [x] This removed dependency on python for RL inference.

## Deliverable 4: 
#### An example how to integrate ML.NET in Godot
- [x] We removed the need of having **two applications**: a UI client and a server serving RL inference by implementing **a single application solution**: [GymRogueOne](https://github.com/JimFFM/ml-hackathon-2021/tree/main/GymRogueOne)

## Deliverable 5:
#### A workflow to make SEVEN OpenAI Gym RL algorithms accessible throgh [ML.NET](https://github.com/dotnet/machinelearning)
- A2C: Asynchronous Advantage Actor Critic.
- DDPG: Deep Deterministic Policy Gradient.
- DQN: Deep Q Network.
- HER: Hindsight Experience Replay.
- [x] PPO: Proximal Policy Optimization. e.g. GymRogueOne
- SAC: Soft Actor Critic.
- TD3: Twin Delayed DDPG.

# How to run the GymRogueOne solution? (Quick Start)

## Step 1 - Git clone and download the missing large size file

```
git clone https://github.com/JimFFM/ml-hackathon-2021
cd ml-hackathon-2021/GymRogueOne/StarShip/
```
Download the missing large size file: **Starship.material** into the StarShip folder, according to the link provided [here](https://github.com/JimFFM/ml-hackathon-2021/blob/main/GymRogueOne/StarShip/ReadmeFirst.md)
```
wget https://drive.google.com/file/d/1pekt-xm665w6h77tOVHtmUjPeRDq5jF4/view?usp=sharing
```

## Step 2 - Download Godot

- [Godot download page](https://godotengine.org/download)
- In our case, we downloaded the [Godot Mono x86-64 bit zip file](https://downloads.tuxfamily.org/godotengine/3.4/mono/Godot_v3.4-stable_mono_win64.zip)
- Unzip the Godot application (~81 MB)
<img src="https://user-images.githubusercontent.com/49812372/142356492-e180285a-2afb-4d73-b5cb-eb9c22bcc32b.png" alt="drawing" width="800"/>

## Step 3 - Select the project.godot file and click run
- A window shows up to allow you to **import** the Godot project file
<img src="https://user-images.githubusercontent.com/49812372/142356675-874202f3-9a08-4197-9062-a02e0d201a7d.png" alt="drawing" width="400"/>

- Navigate to the folder ml-hackathon-2021/GymRogueOne/ and select **project.godot**
- Go to top right corner and click play 

<img src="https://user-images.githubusercontent.com/49812372/142357190-5f001d2e-baa5-4dea-8a51-5121ecd91247.png" alt="drawing" width="400"/>

![image](https://user-images.githubusercontent.com/49812372/142357095-d210ee58-46c8-41cc-8f93-f08b14749422.png)

## Step 4 Play with these parameters to achieve soft landing


<img src="https://user-images.githubusercontent.com/59052120/142358531-a5d2372c-10f3-4552-baa9-1509c70a93be.png" alt="drawing" width="300"/>

<img src="https://user-images.githubusercontent.com/59052120/142358550-6414ab7f-d814-406b-a021-9eb90ba9e11d.png" alt="drawing" width="300"/>


![uzYkLCgQH1](https://user-images.githubusercontent.com/59052120/142358633-33c33c02-7e19-4ce0-b9a8-1148e8c8017d.gif)


