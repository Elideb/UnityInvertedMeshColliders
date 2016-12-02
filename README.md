UnityInvertedMeshColliders
==========================
Script to track Unity prefabs with inverted MeshCollider components

Purpose
-------
Somewhere in the development cycle of Unity 5.3 and 5.4, Unity made a change to its MeshCollider component. It was introduced in version 5.4.0p2 (https://issuetracker.unity3d.com/issues/switching-the-graphics-api-creates-155-errors-in-the-multistory-dungeons-asset) and somewhere between 5.3.3f1 and 5.3.6p8.

The change makes MeshCollider refuse to create a collider if:
* The GameObject has negative scale in any axis
* The mesh used is not *Read/write enabled*

If the two conditions are met, Unity will display this message in the log:
> This MeshCollider requires the mesh to be marked as readable in order to be usable with the given transform.

This script should help in identifying affected prefabs and meshes.

Usage
-----
This scripts adds a new menu entry *Custom/Elideb/Log prefabs with inverted mesh colliders* to the menus in Unity. When it is selected, it will look for all prefabs in the project, load them and find all MeshColliders inside. For each MeshCollider without a uniform scale, a line is written to a file placed at the root of the project, with the name *ConflictiveMeshColliders.csv*. It is a tabs separated values file. Here is a small excerpt, displaying its contents:

> Prefab	GameObject	Local scale	Lossy scale	Mesh	Present	Read/Write	Check
> Assets/GameElements/Levels/Factory/Props/Chain/Chain_Box_Opening.prefab	Chain_Box_Opening/Quad	(6,12,2)	(2.999999,6,0.9999998)	Quad	Library/unity default resources	-		
> Assets/GameElements/Levels/Factory/Props/Chain/ChainElement.prefab	ChainElement/Chain_Box_Opening	(0.5,0.5,0.25)	(0.5000001,0.5,0.2500001)	Chain_Box_Opening	Assets/Models and Textures/Scenery/Production_Models/Level_1/Level_1_Basic/MontageChain/Chain_Box_Opening.dae	True	
> Assets/GameElements/Levels/Factory/Props/Chain/ChainElement.prefab	ChainElement/Chain_Box_Opening	(0.5,0.5,0.25)	(0.5000001,0.5,0.2500001)	Chain_Box_Opening	Assets/Models and Textures/Scenery/Production_Models/Level_1/Level_1_Basic/MontageChain/Chain_Box_Opening.dae	True	
> Assets/GameElements/Levels/Factory/Props/Chain/ChainElement_Weapon.prefab	ChainElement_Weapon/Chain_Box_Opening/Quad	(6,12,2)	(3.000001,6,1)	Quad	Library/unity default resources	-		
> Assets/GameElements/Levels/Factory/Props/Chain/ChainElement_Weapon.prefab	ChainElement_Weapon/Chain_Box_Opening/Quad	(6,12,2)	(3.000001,6,1)	Quad	Library/unity default resources	-		
> Assets/GameElements/Levels/Factory/Props/Elevator/Elevator_Main_Center_Piece.prefab	Elevator_Main_Center_Piece/pb-Cube[Collider]-2280086	(-2,2,2)	(2,2,2)	pb_Mesh-2280086	Assets/GameElements/Levels/Factory/Props/Elevator/pb_Mesh-2280086.asset	-		
> Assets/GameElements/Levels/Factory/Props/Elevator/Elevator_Main_Platform.prefab	Elevator_Main_Platform/Elevator_Main_Center_Piece/pb-Cube[Collider]-2280086	(-2,2,2)	(1,1,1)	pb_Mesh-2280086 Assets/GameElements/Levels/Factory/Props/Elevator/pb_Mesh-2280086.asset	-		
> Assets/GameElements/Levels/Factory/Props/Elevator/Elevator_Main_Platform.prefab	Elevator_Main_Platform/Elevator_Main_Center_Piece/pb-Cube[Collider]-2280086	(-2,2,2)	(-1,1,1)	pb_Mesh-2280086	Assets/GameElements/Levels/Factory/Props/Elevator/pb_Mesh-2280086.asset	-		
> Assets/GameElements/Levels/Factory/Rooms/00Dark/1x1x1/OLD/Factory_1x1x1_Alternative_Chain.prefab	Factory_1x1x1_Alternative_Chain/ChainElement/Chain_Box_Opening/Quad	(6,12,2)	(3,6,1)	Quad	Library/unity default resources	-		
> Assets/GameElements/Levels/Factory/Rooms/00Dark/1x1x1/OLD/Factory_1x1x1_Alternative_Chain.prefab	Factory_1x1x1_Alternative_Chain/ChainElement/Chain_Box_Opening/Quad	(6,12,2)	(3,6,1)	Quad	Library/unity default resources	-		
> Assets/GameElements/Levels/Factory/Rooms/00Dark/3x1x1_three doors/Factory_3x1x1_Alternative_Chain.prefab	Factory_3x1x1_Alternative_Chain/GameObject/Ventilation_SideCurve	(1,1,-1)	(0.8,0.8,-0.8)	Ventilation_SideCurve	Assets/Models and Textures/Scenery/Production_Models/Level_1/Level_1_Basic/Ventilation/Ventilation_SideCurve.dae	True		
> Assets/GameElements/Levels/Factory/Rooms/00Dark/3x1x1_three doors/Factory_3x1x1_Alternative_Chain.prefab	Factory_3x1x1_Alternative_Chain/GameObject/Ventilation_UpCurve	(1,-1,1)	(0.8000003,-0.8000004,0.8000003)	Ventilation_UpCurve	Assets/Models and Textures/Scenery/Production_Models/Level_1/Level_1_Basic/Ventilation/Ventilation_UpCurve.dae	True		
> Assets/GameElements/Levels/Factory/Rooms/00Dark/3x1x1_three doors/Factory_3x1x1_Alternative_Chain.prefab	Factory_3x1x1_Alternative_Chain/GameObject/Ventilation_UpCurve	(1,-1,1)	(0.8000003,-0.8,0.8000003)	Ventilation_UpCurve	Assets/Models and Textures/Scenery/Production_Models/Level_1/Level_1_Basic/Ventilation/Ventilation_UpCurve.dae	True		
> Assets/GameElements/Levels/Factory/Rooms/00Dark/3x1x1_three doors/Factory_3x1x1_Alternative_Chain.prefab	Factory_3x1x1_Alternative_Chain/GameObject/Ventilation_UpCurve	(1,-1,1)	(0.8000003,-0.8,0.8000003)	Ventilation_UpCurve	Assets/Models and Textures/Scenery/Production_Models/Level_1/Level_1_Basic/Ventilation/Ventilation_UpCurve.dae	True		
> Assets/GameElements/Levels/Factory/Rooms/00Dark/3x1x1_three doors/Factory_3x1x1_Alternative_Chain.prefab	Factory_3x1x1_Alternative_Chain/GameObject/Ventilation_UpCurve	(1,-1,1)	(0.8000003,-0.8000004,0.8000003)	Ventilation_UpCurve	Assets/Models and Textures/Scenery/Production_Models/Level_1/Level_1_Basic/Ventilation/Ventilation_UpCurve.dae	True		

Each line shows: the affected prefab, the hierarchy inside the prefab of the affected GameObject, the local and lossy scale of the GameObject, the path inside the project of the used Mesh and whether the *Read/write enabled* flag is set or not. Only those GameObjects with negative scale values are affected by the issue at this moment.

If no mesh is listed, it is either a default asset. The *Read/write enabled* value is displayed only on proper mesh files, not default or binary assets.

With this information, each affected prefab can be verified and corrected, either by enabling the *Read/write enabled* flag in the mesh importer settings, or by correcting the scale of the affected GameObjects.

Caveats
-------
This script only looks for potential issues in prefabs. If a project has affected meshes in scenes, the script can be easily altered to load them and check their contents.

This script does not execute any automatic fixing. The solution to apply depends on each project. If automated fixing is desired, it is possible to implement one of several options:
* Create a new mesh asset with inverted vertices, save it, set it and apply the change to the prefab.
* Replace the mesh asset with some corrected version, if available.
* Set the *Read/write enabled* flag in the mesh importer to true. This will consume some extra memory when running the game, depending on the platform.
