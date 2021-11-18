import gym
from gym import spaces
from gym.wrappers import TimeLimit
from stable_baselines3.common.monitor import Monitor
from stable_baselines3 import PPO
from stable_baselines3.common.vec_env import SubprocVecEnv
import numpy as np
import torch as th
import os
import gym_server


from torch.autograd import Variable

import torch.onnx
import torchvision
import torch
import json

import onnx
import onnxruntime as ort

# Env parameters

# Server
serverIP = '127.0.0.1'

# # Executable
# projectPath = os.getcwd()[:-20]  # project.godot folder
# godotPath = 'flatpak run org.godotengine.Godot'  # godot editor executable
# scenePath = './examples/mars_lander/Root.tscn'  # env Godot scene
# exeCmd = 'cd {} && {} {}'.format(projectPath, godotPath, scenePath)


projectPath = 'C:/Users/Shehroze/source/repos/GymGodot/gym-godot'  # project.godot folder
godotPath = 'E:/godot/Godot/Godot_v3.4-stable_mono_win64.exe'  # godot editor executable
scenePath = './examples/mars_lander/Root.tscn'  # env Godot scene
exeCmd = 'cd {} && {} {}'.format(projectPath, godotPath, scenePath)


# Action Space : Jet activation : Main, AuxX, AuxXn, AuxZ, AuxZn, None
action_space = spaces.Discrete(6)

# Observation Space : X, Y, Z, X_dt, Y_dt, Z_dt, AuxX_vertical, AuxZ_vertical,
# AuxX_vertical_dt, AuxZ_vertical_dt
high = np.array([8, 14, 8, 8, 7, 8, 1, 1, 1, 1], dtype=np.float32)
low = -high
observation_space = spaces.Box(low=low, high=high, dtype=np.float32)

# Create folder to store renders
renderPath = os.getcwd() + '/render_frames/'
if not os.path.exists(renderPath):
    os.makedirs(renderPath)
else:
    # clean folder if not empty
    for file in os.scandir(renderPath):
        os.remove(file.path)

# Max episode length
max_episode_steps = 450

# Create vectorized env
def make_env_fn(i, max_episode_steps=max_episode_steps):
    def make_env():
        # Create env with those parameters
        env = gym.make('server-v0', serverIP=serverIP, serverPort=str(8000+i),
                       exeCmd=exeCmd, action_space=action_space,
                       observation_space=observation_space, proc_mode='thread',
                       window_render=False, renderPath=renderPath)
        # Add a time limit + a tensorboard logger
        env = Monitor(TimeLimit(env, max_episode_steps=max_episode_steps))
        return env
    return make_env


nb_envs = 4
if __name__ == '__main__':
    # vec_envs = SubprocVecEnv([make_env_fn(i) for i in range(nb_envs)],
    #                      start_method='spawn')
    
    # # Custom actor (pi) and value function (vf) networks
    # policy_kwargs = dict(activation_fn=th.nn.ReLU,
    #                      net_arch=[dict(pi=[16, 16], vf=[16, 16])])
    
    # # Train
    # model = PPO('MlpPolicy', vec_envs, verbose=0,  learning_rate=0.002,
    #             tensorboard_log='./tensorboard_logs/', policy_kwargs=policy_kwargs,
    #             device='cuda', seed=0)
    # model.learn(total_timesteps=5e5)
    # vec_envs.close()
    
    # # Save model
    # model.save('mars_lander_model')
    #model = PPO.load('mars_lander_model.zip', device='cuda')
    
    onnx_path = "my_ppo_1_model.onnx"
    onnx_model = onnx.load(onnx_path)
    onnx.checker.check_model(onnx_model)

    observation = np.zeros((10)).astype(np.float32)
    ort_sess = ort.InferenceSession(onnx_path)
    
    # dummy_input = vec_envs
    # state_dict = torch.load('mars_lander_model')
    # model.load_state_dict(state_dict)
    # torch.onnx.export(model, dummy_input, "mars_lander_model.onnx")
    
    def predict_mlnet(obs):
        obs_file =  open('obs.json','wt')
        obs_file.write(json.dumps(obs))
        obs_file.close()
        os.system('dotnet my_ml_runner.dll')
    
    # Record one episode
    env = (make_env_fn(1))()
    obs = env.reset()
    for i in range(max_episode_steps):
        #action1, _states = model.predict(obs)
        action, _states = ort_sess.run(None, {'input.1': obs})
        
        action_to_take = 0
        for i_ in range(6):
            if action[i_] ==  action.max():
                action_to_take = i_
                break;    
        #if( i%2 == 0):
        obs, rewards, done, info = env.step(action_to_take)
        #else:
        #    obs, rewards, done, info = env.step(action1)
        #print((action1,action_to_take))
        env.render()
        if done:
            break
    env.close()

    # Create video from frames
    #os.system('cd {} && ffmpeg -hide_banner -loglevel error -framerate 30 -y -i %01d.png -vcodec libvpx -b 2000k video.webm'.format(renderPath))
    
    # Remove frames
    #for item in os.listdir(renderPath):
    #    if item.endswith('.png'):
    #        os.remove(os.path.join(renderPath, item))
