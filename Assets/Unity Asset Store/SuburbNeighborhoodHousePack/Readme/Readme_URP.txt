URP Instructions:
1. Import asset to an existing URP project.
2. Import SuburbNeighborhood_URP.unitypackage file from Assets -> Import Package -> Custom Package.
3. Replace all files.
4. Shaders and scene should update to URP.

Issues when baking lighting:
Ground doesn't get proper lightmapping. Reason is custom foliage shaders causes Unity not to recognize alpha while baking.
Change Climberplant, Plant_BushGrass, Plant_Flowers, Plant_Grass, Plant_Hedgebush, Plant_Treebush and Tree materials
to URP/Lit. Make sure corresponding textures are applied and materials use alpha.
Change it back to FinwardStudios/Foliage after the bake. 