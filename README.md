# unity-ml-racing-track

Sample project using Unity ML-Agents in an autopilot racing car.

## Environment setup

1. Install Unity ML-Agents and dependencies (steps [here](https://github.com/Unity-Technologies/ml-agents/blob/release_12_docs/docs/Installation.md)).

## ML-Agents related

2. Train the model

- Activate the virtual environment in the repo root folder
- Run `mlagents-learn config/autopilot_config.yaml --run-id=Autopilot --force`. After the script starts it will ask you to run the project in Unity.
- Press `Play` in Unity

3. Review training statistics

- Activate the virtual environment in the repo root folder
- Run `tensorboard --logdir results --port 6006`
- Open a browser window and navigate to `localhost:6006`

## Steps into development

- [ML-Agents getting started](https://github.com/Unity-Technologies/ml-agents/blob/release_12_docs/docs/Getting-Started.md)
- [Create a learning environment](https://github.com/Unity-Technologies/ml-agents/blob/release_12_docs/docs/Learning-Environment-Create-New.md)
- [ML Agents walkthrough](https://towardsdatascience.com/ultimate-walkthrough-for-ml-agents-in-unity3d-5603f76f68b)

## Lectures

### ML-Agents

- [How to read TensorBoard?](https://github.com/Unity-Technologies/ml-agents/blob/master/docs/Using-Tensorboard.md)
- [How to read TensorBoard? II](https://medium.com/aureliantactics/understanding-ppo-plots-in-tensorboard-cbc3199b9ba2)
- [Configuration file in ML-Agents](https://cursos.uadla.com/curso/curso-intermedio-de-ml-agents/lecciones/la-importancia-del-fichero-yaml/)

### Unity

- [Car movement in Unity with Wheel Colliders](https://www.youtube.com/watch?v=j6_SMdWeGFI)
- [ML Agents in car movement](https://unitylist.com/p/xha/Unity-ML-Agent-Car-prototype)
- [Autonomous car parking](https://medium.com/xrpractices/autonomous-car-parking-using-ml-agents-d780a366fe46)
- [Ignore collitions while training?](https://gamedev.stackexchange.com/questions/75782/how-to-ignore-collision-between-two-objects)

### Geometry

- [Dot vector applications in games](https://hackernoon.com/applications-of-the-vector-dot-product-for-game-programming-12443ac91f16)

### ML in gaming

- [NN usage in Colin McRae's Rally 2](http://www.ai-junkie.com/misc/hannan/hannan.html)
