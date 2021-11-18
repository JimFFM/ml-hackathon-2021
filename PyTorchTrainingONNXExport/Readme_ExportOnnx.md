The following code converts a trained PPO model from Stable_Baseline3 PPO model to a torch model
```
onnxable_model = OnnxablePolicy(model.policy.mlp_extractor, model.policy.action_net, model.policy.value_net)
```

then exports it into onnx format. 
```
onnxable_model = OnnxablePolicy(model.policy.mlp_extractor, model.policy.action_net, model.policy.value_net)
```

and finally tests it to see if it was exported correctly

```
##### Load and test with onnx
import onnx
import onnxruntime as ort
import numpy as np
onnx_path = "my_ppo_1_model.onnx"
onnx_model = onnx.load(onnx_path)
onnx.checker.check_model(onnx_model)

observation = np.zeros((10)).astype(np.float32)
ort_sess = ort.InferenceSession(onnx_path)
action, value = ort_sess.run(None, {'input.1': observation})
print(action)

```
