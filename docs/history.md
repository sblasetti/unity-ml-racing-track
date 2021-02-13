# History

## Iteration 0

Created the initial project structure with a sample track and a sample vehicle.

![Initial track](./images/000-track.png)

## Iteration 1

Added checkpoints as strategy for track mapping. Tried training a single agent.

![Checkpoints](./images/001-checkpoints.png)

![Single agent](./images/001-single-agent.gif)

## Iteration 2

Updated project to train 20 agents with a default PPO algorythm configuration.
The agents reached a third of the track.

![20 agents I](./images/002-learning1.gif)

![20 agents II](./images/002-learning2.gif)

## Iteration 3

To let the agents know more about the track, tried adding a separate mesh for the track limits.
The agents look lost, will try updating the algorithm onfiguration.

![Track limits](./images/003-track-limits.png)

![Agents with track limit](./images/003-limits.gif)
