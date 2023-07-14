//*****  (©) Finward Studios 2021. All rights reserved. *****\\

Suburb Neighborhood House Pack (Modular)

Built-In Render Pipeline Instrcutions:
Recommended Color Space: Linear (Project Settings -> Player -> Other Settings -> Color Space)
Recommended Rendering Path: Deferred
Import Post Processing from Package Manager. Assign Post Processing profiles from BuiltIn_RP folder. 

Prefab Swapper script:
Script is attached to each modular prefab and is found under Transform component. Visible in inspector by selecting modular prefabs from the scene.
Prefab list can be modified from SwapPrefabList.cs in case you want to add your own custom prefabs.

How to use Prefab Swapper?
Select prefab from scene or hierarchy. From inspector find SwapPrefab script which is attached to the prefab.
Select Category and Prefab you want.

Modular prefabs will snap to grid.
Use ProGrid or Ctrl + L to open AutoSnap script.

GameobjectOptimize script:
Will disable gameobject based on distance or height compared to MainCamera tag.

For questions and feedback please email to: support@finwardstudios.com