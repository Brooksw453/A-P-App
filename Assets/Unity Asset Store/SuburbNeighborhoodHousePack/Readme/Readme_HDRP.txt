HDRP Instructions:
1. Import asset to an existing HDRP project.
2. Import SuburbNeighborhood_HDRP.unitypackage file from Assets -> Import Package -> Custom Package.
3. Replace all files.
4. Shaders and scene should update to HDRP.

Issues when baking lighting:
Ground doesn't get proper lightmapping. Reason is custom foliage shaders causes Unity not to recognize alpha while baking.
Change Climberplant, Plant_BushGrass, Plant_Flowers, Plant_Grass, Plant_Hedgebush, Plant_Treebush and Tree materials
to HDRP/Lit. Make sure corresponding textures are applied and materials use alpha.
Change it back to FinwardStudios/Foliage after the bake.

If you feel the framerate is too low for you, change some of the lights from realtime to baked and rebake the lightning.