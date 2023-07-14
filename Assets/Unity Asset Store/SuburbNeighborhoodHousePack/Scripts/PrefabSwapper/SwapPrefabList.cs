using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrefabSwapperSuburb
{

    //*****  (©) Finward Studios 2021. All rights reserved. *****\\

    public class SwapPrefabList : ScriptableObject
    {
        // Add folders to look the prefabs from: { "Assets/Folder_A", "Assets/Folder_B" },
        public static string[] prefabsFolders = { "Assets/SuburbNeighborhoodHousePack/Prefabs/Building", "Assets/SuburbNeighborhoodHousePack/Prefabs/Ground", "Assets/SuburbNeighborhoodHousePack/Prefabs/Doors" };

        [System.Serializable]
        public class SwapPrefabProperties
        {

            public string prefabName;
            public string category;
            public string[] addAdditional;
            public string[] addAdditionalButtonName;
            public Vector3[] addAdditionalPos;
            public Vector3[] addAdditionalRot;

            public SwapPrefabProperties(
                string _PrefabName,
                string _Category,
                string[] _AddAdditional,
                string[] _AddAdditionalButtonName,
                Vector3[] _AddAdditionalPos,
                Vector3[] _AddAdditionalRot
                )

            {
                prefabName = _PrefabName;
                category = _Category;
                addAdditional = _AddAdditional;
                addAdditionalButtonName = _AddAdditionalButtonName;
                addAdditionalPos = _AddAdditionalPos;
                addAdditionalRot = _AddAdditionalRot;
            }
        }

        public static SwapPrefabProperties[] swapPrefabProperties = new SwapPrefabProperties[]
        {
            new SwapPrefabProperties(
                //prefabName
                "Wall_1st_½x",
                //category
                "Walls 1st",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_1st_1x",
                //category
                "Walls 1st",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_1st_1x_door",
                //category
                "Walls 1st",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_1st_2x",
                //category
                "Walls 1st",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_1st_3x",
                //category
                "Walls 1st",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_1st_3x_door_A",
                //category
                "Walls 1st",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_1st_3x_door_B",
                //category
                "Walls 1st",
                //addAdditional
                new string[] { "Window_blocker_door_B" },
                //addAdditionalButtonName
                new string[] { "Window_blocker" },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_1st_3x_door_C",
                //category
                "Walls 1st",
                //addAdditional
                new string[] { "Window_blocker_door_C" },
                //addAdditionalButtonName
                new string[] { "Window_blocker" },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_1st_3x_window_bay_A",
                //category
                "Walls 1st",
                //addAdditional
                new string[] { "Window_blocker_curtains", "Window_blocker_blinds" },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(-1.5f, 1.7f, 0.35f) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_1st_3x_window_bay_B",
                //category
                "Walls 1st",
                //addAdditional
                new string[] { "Window_blocker_window_bay_B" },
                //addAdditionalButtonName
                new string[] { "Window_blocker" },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_1st_3x_window_double_A",
                //category
                "Walls 1st",
                //addAdditional
                new string[] { "Window_blocker_curtains", "Window_blocker_blinds" },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(-1.5f, 1.7f, -0.15f) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_1st_3x_window_double_B",
                //category
                "Walls 1st",
                //addAdditional
                new string[] { "Window_blocker_curtains", "Window_blocker_blinds" },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(-1.5f, 1.7f, -0.15f) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_1st_3x_window_openable",
                //category
                "Walls 1st",
                //addAdditional
                new string[] { "Window_blocker_curtains", "Window_blocker_blinds" },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(-1.5f, 1.7f, -0.15f) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_1st_3x_window_single_A",
                //category
                "Walls 1st",
                //addAdditional
                new string[] { "Window_blocker_curtains", "Window_blocker_blinds" },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(-1.5f, 1.7f, -0.15f) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_1st_3x_window_single_B",
                //category
                "Walls 1st",
                //addAdditional
                new string[] { "Window_blocker_curtains", "Window_blocker_blinds" },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(-1.5f, 1.7f, -0.15f) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_1st_3x_window_single_C",
                //category
                "Walls 1st",
                //addAdditional
                new string[] { "Window_blocker_curtains", "Window_blocker_blinds" },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(-1.5f, 1.7f, -0.15f) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_1st_3x_window_single_D",
                //category
                "Walls 1st",
                //addAdditional
                new string[] { "Window_blocker_curtains", "Window_blocker_blinds" },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(-1.5f, 1.8f, -0.15f) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_1st_4x_door_garage",
                //category
                "Walls 1st",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_1st_4x_window_double",
                //category
                "Walls 1st",
                //addAdditional
                new string[] { "Window_blocker_curtains", "Window_blocker_blinds" },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(-2.0f, 1.8f, -0.15f) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_1st_5x_door_garage",
                //category
                "Walls 1st",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_1st_corner_in",
                //category
                "Walls 1st",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_1st_corner_in_patch",
                //category
                "Walls 1st",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_1st_corner_out",
                //category
                "Walls 1st",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_1st_corner_out_patch",
                //category
                "Walls 1st",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_2nd_½x",
                //category
                "Walls 2nd",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_2nd_1x",
                //category
                "Walls 2nd",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_2nd_2x",
                //category
                "Walls 2nd",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_2nd_3x",
                //category
                "Walls 2nd",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_2nd_3x_window_double_A",
                //category
                "Walls 2nd",
                //addAdditional
                new string[] { "Window_blocker_curtains", "Window_blocker_blinds" },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(-1.5f, 1.8f, -0.15f) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                                ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_2nd_3x_window_double_B",
                //category
                "Walls 2nd",
                //addAdditional
                new string[] { "Window_blocker_curtains", "Window_blocker_blinds" },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(-1.5f, 1.8f, -0.15f) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                                ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_2nd_3x_window_openable",
                //category
                "Walls 2nd",
                //addAdditional
                new string[] { "Window_blocker_curtains", "Window_blocker_blinds" },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(-1.5f, 1.8f, -0.15f) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_2nd_3x_window_single_A",
                //category
                "Walls 2nd",
                //addAdditional
                new string[] { "Window_blocker_curtains", "Window_blocker_blinds" },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(-1.5f, 1.8f, -0.15f) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_2nd_3x_window_single_B",
                //category
                "Walls 2nd",
                //addAdditional
                new string[] { "Window_blocker_curtains", "Window_blocker_blinds" },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(-1.5f, 1.8f, -0.15f) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_2nd_3x_window_single_C",
                //category
                "Walls 2nd",
                //addAdditional
                new string[] { "Window_blocker_curtains", "Window_blocker_blinds" },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(-1.5f, 1.8f, -0.15f) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_2nd_3x_window_single_D",
                //category
                "Walls 2nd",
                //addAdditional
                new string[] { "Window_blocker_curtains", "Window_blocker_blinds" },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(-1.5f, 1.8f, -0.15f) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_2nd_4x_window_double",
                //category
                "Walls 2nd",
                //addAdditional
                new string[] { "Window_blocker_curtains", "Window_blocker_blinds" },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(-2.0f, 1.8f, -0.15f) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_2nd_corner_in",
                //category
                "Walls 2nd",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_2nd_corner_in_patch",
                //category
                "Walls 2nd",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_2nd_corner_out",
                //category
                "Walls 2nd",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_2nd_corner_out_patch",
                //category
                "Walls 2nd",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_Interior_½x",
                //category
                "Walls Interior",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_Interior_½x_end",
                //category
                "Walls Interior",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_Interior_1x",
                //category
                "Walls Interior",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_Interior_1x_door",
                //category
                "Walls Interior",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_Interior_1x_door_clothcloset",
                //category
                "Walls Interior",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_Interior_1x_end",
                //category
                "Walls Interior",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_Interior_2x",
                //category
                "Walls Interior",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_Interior_2x_door_double",
                //category
                "Walls Interior",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_Interior_3x",
                //category
                "Walls Interior",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_Interior_3x_door",
                //category
                "Walls Interior",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_Interior_3x_end",
                //category
                "Walls Interior",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_Interior_3x_end",
                //category
                "Walls Interior",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_Interior_5x_door",
                //category
                "Walls Interior",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_Interior_6x_fireplace",
                //category
                "Walls Interior",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_Interior_8x_door",
                //category
                "Walls Interior",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_Interior_column_half",
                //category
                "Walls Interior",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_Interior_corner",
                //category
                "Walls Interior",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_Interior_corner_out_patch",
                //category
                "Walls Interior",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_Interior_corner_T",
                //category
                "Walls Interior",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_Interior_halfheight_1x",
                //category
                "Walls Interior",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_Interior_halfheight_2x",
                //category
                "Walls Interior",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_Interior_halfheight_3x",
                //category
                "Walls Interior",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Wall_Interior_halfheight_pilar",
                //category
                "Walls Interior",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Stairs_main_L",
                //category
                "Stairs",
                //addAdditional
                new string[] { "Floor_stairs_5x_3x", "Floor_stairs_5x_4x" },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Stairs_main_R",
                //category
                "Stairs",
                //addAdditional
                new string[] { "Floor_stairs_5x_3x", "Floor_stairs_5x_4x" },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(-5, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Stairs_basement",
                //category
                "Stairs",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Floor_½x_5x",
                //category
                "Floors",
                //addAdditional
                new string[] { "Ceiling_½x_5x" },
                //addAdditionalButtonName
                new string[] { "Ceiling" },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 3, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Floor_½x_7x",
                //category
                "Floors",
                //addAdditional
                new string[] { "Ceiling_½x_7x" },
                //addAdditionalButtonName
                new string[] { "Ceiling" },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 3, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Floor_½x_8x",
                //category
                "Floors",
                //addAdditional
                new string[] { "Ceiling_½x_8x" },
                //addAdditionalButtonName
                new string[] { "Ceiling" },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 3, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Floor_1x_3x",
                //category
                "Floors",
                //addAdditional
                new string[] { "Ceiling_1x_3x" },
                //addAdditionalButtonName
                new string[] { "Ceiling" },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 3, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                                ,
            new SwapPrefabProperties(
                //prefabName
                "Floor_1x_5x",
                //category
                "Floors",
                //addAdditional
                new string[] { "Ceiling_1x_5x" },
                //addAdditionalButtonName
                new string[] { "Ceiling" },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 3, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                                ,
            new SwapPrefabProperties(
                //prefabName
                "Floor_1x_7x",
                //category
                "Floors",
                //addAdditional
                new string[] { "Ceiling_1x_7x" },
                //addAdditionalButtonName
                new string[] { "Ceiling" },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 3, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                                ,
            new SwapPrefabProperties(
                //prefabName
                "Floor_1x_8x",
                //category
                "Floors",
                //addAdditional
                new string[] { "Ceiling_1x_8x" },
                //addAdditionalButtonName
                new string[] { "Ceiling" },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 3, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Floor_2x_2x",
                //category
                "Floors",
                //addAdditional
                new string[] { "Ceiling_2x_2x" },
                //addAdditionalButtonName
                new string[] { "Ceiling" },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 3, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Floor_2½x_2½x",
                //category
                "Floors",
                //addAdditional
                new string[] { "Ceiling_2½x_2½x" },
                //addAdditionalButtonName
                new string[] { "Ceiling" },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 3, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Floor_3x_½x",
                //category
                "Floors",
                //addAdditional
                new string[] { "Ceiling_3x_½x" },
                //addAdditionalButtonName
                new string[] { "Ceiling" },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 3, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Floor_3x_1x",
                //category
                "Floors",
                //addAdditional
                new string[] { "Ceiling_3x_1x" },
                //addAdditionalButtonName
                new string[] { "Ceiling" },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 3, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Floor_3x_2x",
                //category
                "Floors",
                //addAdditional
                new string[] { "Ceiling_3x_2x" },
                //addAdditionalButtonName
                new string[] { "Ceiling" },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 3, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Floor_3x_3x",
                //category
                "Floors",
                //addAdditional
                new string[] { "Ceiling_3x_3x" },
                //addAdditionalButtonName
                new string[] { "Ceiling" },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 3, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                                ,
            new SwapPrefabProperties(
                //prefabName
                "Floor_3x_4x",
                //category
                "Floors",
                //addAdditional
                new string[] { "Ceiling_3x_4x" },
                //addAdditionalButtonName
                new string[] { "Ceiling" },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 3, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Floor_3x_5x",
                //category
                "Floors",
                //addAdditional
                new string[] { "Ceiling_3x_5x" },
                //addAdditionalButtonName
                new string[] { "Ceiling" },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 3, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Floor_3x_7x",
                //category
                "Floors",
                //addAdditional
                new string[] { "Ceiling_3x_7x" },
                //addAdditionalButtonName
                new string[] { "Ceiling" },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 3, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Floor_3x_8x",
                //category
                "Floors",
                //addAdditional
                new string[] { "Ceiling_3x_8x" },
                //addAdditionalButtonName
                new string[] { "Ceiling" },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 3, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                                            ,

            new SwapPrefabProperties(
                //prefabName
                "Floor_4x_3x",
                //category
                "Floors",
                //addAdditional
                new string[] { "Ceiling_4x_3x" },
                //addAdditionalButtonName
                new string[] { "Ceiling" },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 3, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                                ,

            new SwapPrefabProperties(
                //prefabName
                "Floor_5x_3x",
                //category
                "Floors",
                //addAdditional
                new string[] { "Ceiling_5x_3x" },
                //addAdditionalButtonName
                new string[] { "Ceiling" },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 3, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                                            ,
            new SwapPrefabProperties(
                //prefabName
                "Floor_5x_5x",
                //category
                "Floors",
                //addAdditional
                new string[] { "Ceiling_5x_5x" },
                //addAdditionalButtonName
                new string[] { "Ceiling" },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 3, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                ,
            new SwapPrefabProperties(
                //prefabName
                "Floor_12x_8x",
                //category
                "Floors",
                //addAdditional
                new string[] { "Ceiling_12x_8x" },
                //addAdditionalButtonName
                new string[] { "Ceiling" },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 3, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Ceiling_½x_5x",
                //category
                "Ceilings",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Ceiling_½x_7x",
                //category
                "Ceilings",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Ceiling_½x_8x",
                //category
                "Ceilings",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Ceiling_1x_3x",
                //category
                "Ceilings",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Ceiling_1x_5x",
                //category
                "Ceilings",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Ceiling_1x_7x",
                //category
                "Ceilings",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Ceiling_1x_8x",
                //category
                "Ceilings",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Ceiling_2x_2x",
                //category
                "Ceilings",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                                ,
            new SwapPrefabProperties(
                //prefabName
                "Ceiling_2½x_2½x",
                //category
                "Ceilings",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Ceiling_3x_½x",
                //category
                "Ceilings",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Ceiling_3x_1x",
                //category
                "Ceilings",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Ceiling_3x_2x",
                //category
                "Ceilings",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Ceiling_3x_3x",
                //category
                "Ceilings",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Ceiling_3x_4x",
                //category
                "Ceilings",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Ceiling_3x_5x",
                //category
                "Ceilings",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Ceiling_3x_7x",
                //category
                "Ceilings",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Ceiling_3x_8x",
                //category
                "Ceilings",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Ceiling_4x_3x",
                //category
                "Ceilings",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Ceiling_5x_3x",
                //category
                "Ceilings",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Ceiling_5x_5x",
                //category
                "Ceilings",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Ceiling_12x_8x",
                //category
                "Ceilings",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Roof_5x",
                //category
                "Roofs",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Roof_5x_end",
                //category
                "Roofs",
                //addAdditional
                new string[] { "Drainpipe_1story", "Drainpipe_1story", "Drainpipe_2story", "Drainpipe_2story" },
                //addAdditionalButtonName
                new string[] { "Drainpipe 1story L", "Drainpipe 1story R", "Drainpipe 2story L", "Drainpipe 2story R" },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, -0.5f), new Vector3(-5, 0, 0), new Vector3(0, 0, -0.5f), new Vector3(-5, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 90.0f, 0), new Vector3(0, -90.0f, 0), new Vector3(0, 90.0f, 0), new Vector3(0, -90.0f, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Roof_5x_end_window",
                //category
                "Roofs",
                //addAdditional
                new string[] { "Drainpipe_1story", "Drainpipe_1story", "Drainpipe_2story", "Drainpipe_2story" },
                //addAdditionalButtonName
                new string[] { "Drainpipe 1story L", "Drainpipe 1story R", "Drainpipe 2story L", "Drainpipe 2story R" },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, -0.5f), new Vector3(-5, 0, 0), new Vector3(0, 0, -0.5f), new Vector3(-5, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 90.0f, 0), new Vector3(0, -90.0f, 0), new Vector3(0, 90.0f, 0), new Vector3(0, -90.0f, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Roof_5x_patch",
                //category
                "Roofs",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Roof_7x",
                //category
                "Roofs",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                                ,
            new SwapPrefabProperties(
                //prefabName
                "Roof_7x_end",
                //category
                "Roofs",
                //addAdditional
                new string[] { "Drainpipe_1story", "Drainpipe_1story", "Drainpipe_2story", "Drainpipe_2story" },
                //addAdditionalButtonName
                new string[] { "Drainpipe 1story L", "Drainpipe 1story R", "Drainpipe 2story L", "Drainpipe 2story R" },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, -0.5f), new Vector3(-7, 0, 0), new Vector3(0, 0, -0.5f), new Vector3(-7, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 90.0f, 0), new Vector3(0, -90.0f, 0), new Vector3(0, 90.0f, 0), new Vector3(0, -90.0f, 0) }
                )
                                                ,
            new SwapPrefabProperties(
                //prefabName
                "Roof_7x_end_window",
                //category
                "Roofs",
                //addAdditional
                new string[] { "Drainpipe_1story", "Drainpipe_1story", "Drainpipe_2story", "Drainpipe_2story" },
                //addAdditionalButtonName
                new string[] { "Drainpipe 1story L", "Drainpipe 1story R", "Drainpipe 2story L", "Drainpipe 2story R" },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, -0.5f), new Vector3(-7, 0, 0), new Vector3(0, 0, -0.5f), new Vector3(-7, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 90.0f, 0), new Vector3(0, -90.0f, 0), new Vector3(0, 90.0f, 0), new Vector3(0, -90.0f, 0) }
                )
                                                ,
            new SwapPrefabProperties(
                //prefabName
                "Roof_7x_patch",
                //category
                "Roofs",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                                ,
            new SwapPrefabProperties(
                //prefabName
                "Roof_8x",
                //category
                "Roofs",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                                ,
            new SwapPrefabProperties(
                //prefabName
                "Roof_8x_corner",
                //category
                "Roofs",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                                ,
            new SwapPrefabProperties(
                //prefabName
                "Roof_8x_end",
                //category
                "Roofs",
                //addAdditional
                new string[] { "Drainpipe_1story", "Drainpipe_1story", "Drainpipe_2story", "Drainpipe_2story" },
                //addAdditionalButtonName
                new string[] { "Drainpipe 1story L", "Drainpipe 1story R", "Drainpipe 2story L", "Drainpipe 2story R" },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, -0.5f), new Vector3(-8, 0, 0), new Vector3(0, 0, -0.5f), new Vector3(-8, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 90.0f, 0), new Vector3(0, -90.0f, 0), new Vector3(0, 90.0f, 0), new Vector3(0, -90.0f, 0) }
                )
                                                            ,
            new SwapPrefabProperties(
                //prefabName
                "Roof_8x_end_window",
                //category
                "Roofs",
                //addAdditional
                new string[] { "Drainpipe_1story", "Drainpipe_1story", "Drainpipe_2story", "Drainpipe_2story" },
                //addAdditionalButtonName
                new string[] { "Drainpipe 1story L", "Drainpipe 1story R", "Drainpipe 2story L", "Drainpipe 2story R" },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, -0.5f), new Vector3(-8, 0, 0), new Vector3(0, 0, -0.5f), new Vector3(-8, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 90.0f, 0), new Vector3(0, -90.0f, 0), new Vector3(0, 90.0f, 0), new Vector3(0, -90.0f, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Roof_8x_patch",
                //category
                "Roofs",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "RoofLow_5x",
                //category
                "Roofs",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "RoofLow_5x_end",
                //category
                "Roofs",
                //addAdditional
                new string[] { "Drainpipe_1story", "Drainpipe_1story", "Drainpipe_2story", "Drainpipe_2story" },
                //addAdditionalButtonName
                new string[] { "Drainpipe 1story L", "Drainpipe 1story R", "Drainpipe 2story L", "Drainpipe 2story R" },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, -0.5f), new Vector3(-5, 0, 0), new Vector3(0, 0, -0.5f), new Vector3(-5, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 90.0f, 0), new Vector3(0, -90.0f, 0), new Vector3(0, 90.0f, 0), new Vector3(0, -90.0f, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "RoofLow_5x_patch",
                //category
                "Roofs",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "RoofLow_7x",
                //category
                "Roofs",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "RoofLow_7x_end",
                //category
                "Roofs",
                //addAdditional
                new string[] { "Drainpipe_1story", "Drainpipe_1story", "Drainpipe_2story", "Drainpipe_2story" },
                //addAdditionalButtonName
                new string[] { "Drainpipe 1story L", "Drainpipe 1story R", "Drainpipe 2story L", "Drainpipe 2story R" },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, -0.5f), new Vector3(-7, 0, 0), new Vector3(0, 0, -0.5f), new Vector3(-7, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 90.0f, 0), new Vector3(0, -90.0f, 0), new Vector3(0, 90.0f, 0), new Vector3(0, -90.0f, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "RoofLow_7x_patch",
                //category
                "Roofs",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "RoofLow_8x",
                //category
                "Roofs",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "RoofLow_8x_end",
                //category
                "Roofs",
                //addAdditional
                new string[] { "Drainpipe_1story", "Drainpipe_1story", "Drainpipe_2story", "Drainpipe_2story" },
                //addAdditionalButtonName
                new string[] { "Drainpipe 1story L", "Drainpipe 1story R", "Drainpipe 2story L", "Drainpipe 2story R" },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, -0.5f), new Vector3(-8, 0, 0), new Vector3(0, 0, -0.5f), new Vector3(-8, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 90.0f, 0), new Vector3(0, -90.0f, 0), new Vector3(0, 90.0f, 0), new Vector3(0, -90.0f, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "RoofLow_8x_patch",
                //category
                "Roofs",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                                ,
            new SwapPrefabProperties(
                //prefabName
                "Roof_extra_window_double",
                //category
                "Roofs",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Roof_extra_window_single",
                //category
                "Roofs",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Porch_2x",
                //category
                "Porch",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Porch_2x_end_left",
                //category
                "Porch",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Porch_2x_end_right",
                //category
                "Porch",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Porch_4x",
                //category
                "Porch",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Porch_4x_end_left",
                //category
                "Porch",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Porch_4x_end_right",
                //category
                "Porch",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                                ,
            new SwapPrefabProperties(
                //prefabName
                "Porch_floor_1x_2x",
                //category
                "Porch",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                                            ,
            new SwapPrefabProperties(
                //prefabName
                "Porch_floor_3x_2x",
                //category
                "Porch",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Porch_pilar",
                //category
                "Porch",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Porch_rail_1x",
                //category
                "Porch",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Porch_rail_1½x",
                //category
                "Porch",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Porch_rail_2x",
                //category
                "Porch",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Road_1x",
                //category
                "Ground",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Road_4x",
                //category
                "Ground",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Road_4x_walkway",
                //category
                "Ground",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Road_4x_walkway_double",
                //category
                "Ground",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Road_8x",
                //category
                "Ground",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Road_8x_driveway",
                //category
                "Ground",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Road_circle",
                //category
                "Ground",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Road_corner_in",
                //category
                "Ground",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Road_corner_out",
                //category
                "Ground",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Road_end",
                //category
                "Ground",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Road_parking_lot",
                //category
                "Ground",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                                ,
            new SwapPrefabProperties(
                //prefabName
                "Road_garageramp_3x",
                //category
                "Ground",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                                            ,
            new SwapPrefabProperties(
                //prefabName
                "Road_garageramp_5x",
                //category
                "Ground",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                                            ,
            new SwapPrefabProperties(
                //prefabName
                "Road_driveway_piece",
                //category
                "Ground",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Road_walkway_piece",
                //category
                "Ground",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Ground_1x4x",
                //category
                "Ground",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Ground_2x4x",
                //category
                "Ground",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Ground_2x8x",
                //category
                "Ground",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Ground_4x8x",
                //category
                "Ground",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Ground_8x8x",
                //category
                "Ground",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Ground_20x20x",
                //category
                "Ground",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Ground_45x45x",
                //category
                "Ground",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Ground_pool_4x8x_A",
                //category
                "Ground",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                                ,
            new SwapPrefabProperties(
                //prefabName
                "Ground_pool_4x8x_B",
                //category
                "Ground",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Door_exterior_paneled",
                //category
                "Doors",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Door_exterior_white",
                //category
                "Doors",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Door_exterior_windowed",
                //category
                "Doors",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                                ,
            new SwapPrefabProperties(
                //prefabName
                "Door_exterior_windowed_brown",
                //category
                "Doors",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                                            ,
            new SwapPrefabProperties(
                //prefabName
                "Door_interior",
                //category
                "Doors",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
            ,
            new SwapPrefabProperties(
                //prefabName
                "Door_garage_3x",
                //category
                "Doors",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                                    ,
            new SwapPrefabProperties(
                //prefabName
                "Door_garage_brown_3x",
                //category
                "Doors",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Door_garage_5x",
                //category
                "Doors",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
                        ,
            new SwapPrefabProperties(
                //prefabName
                "Door_garage_brown_5x",
                //category
                "Doors",
                //addAdditional
                new string[] { },
                //addAdditionalButtonName
                new string[] { },
                //addAdditionalPos
                new Vector3[] { new Vector3(0, 0, 0) },
                //addAdditionalRot
                new Vector3[] { new Vector3(0, 0, 0) }
                )
        };
    }
}

//*****  (©) Finward Studios 2021. All rights reserved. *****\\
//*****     SwapPrefab written by: Jan Procházka  *****\\