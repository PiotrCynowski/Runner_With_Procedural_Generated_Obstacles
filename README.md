## This repository contains a simple game project where the player controls a character, collecting both points and special obstacles. These unique obstacles elevate our character's position, enabling them to navigate through obstacles that could otherwise deplete our collected items.
Unity Engine Project

*level setup example*
![alt text](https://github.com/PiotrCynowski/Runner_With_Procedural_Generated_Obstacles/blob/master/pic/LevelSetup.png?raw=true)

#### - Controllers:
Move left and right with mouse position and hold LMB
Escape button to open the pause menu

#### - Level Generate:
levels are generated based on data asset we reference to in LevelManager.
*level setup example*
![alt text](https://github.com/PiotrCynowski/Runner_With_Procedural_Generated_Obstacles/blob/master/pic/LevelSetup.png?raw=true)

#### - Levels
Obstacles and obstacles to collect in the player's stand are procedurally generated based on the number of stages from the level's data resource.
If resources are generated, they will be reused by the object pool, if the next level requires higher obstacles, they will be additionally generated for the object pool.
When the player completes all available levels listed in LevelManager, the game will reload the levels from the beginning.

the main scene is in Assets/Scenes/Main
#### - In main scene there are prefabs for:
-CanvasMain
-CameraMain
-Player
-LevelManager
