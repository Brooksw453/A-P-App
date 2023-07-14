using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.ExceptionServices;
using TMPro;

namespace PrefabSwapperSuburb
{

    //*****  (©) Finward Studios 2021. All rights reserved. *****\\

    [AddComponentMenu("PrefabSwapper/SwapPrefabSuburb")]
    [ExecuteInEditMode]
    public class SwapPrefab : MonoBehaviour
    {
        public static List<string> categoryList;
        public static List<string> prefabList;

        public bool placed = false;
        public string category;
        public string prefab;
        public string[] additionals;
        public string[] additionalsBtns;
        public Vector3[] additionalsPos;
        public Vector3[] additionalsRot;

        string exCategory;
        string exPrefab;

        public void CreateCategoryList()
        {
            categoryList = new List<string>();

            foreach (SwapPrefabList.SwapPrefabProperties _prefab in SwapPrefabList.swapPrefabProperties)
            {
                if (!categoryList.Contains(_prefab.category))
                    categoryList.Add(_prefab.category);
            }
        }

        public void CreatePrefabList()
        {
            prefabList = new List<string>();

            foreach (SwapPrefabList.SwapPrefabProperties _prefab in SwapPrefabList.swapPrefabProperties)
            {
                if (_prefab.category == category)
                    if (!prefabList.Contains(_prefab.prefabName))
                        prefabList.Add(_prefab.prefabName);
            }
        }


        public void FindPrefabProperties()
        {
            //Debug.Log("looking for prefab properties");
            foreach (SwapPrefabList.SwapPrefabProperties _prefab in SwapPrefabList.swapPrefabProperties)
            {
                if (_prefab.prefabName == transform.name)
                {
                    GetPrefabProperties(_prefab);
                    return;
                }
            }
        }

        public void CleanPrefabProperties()
        {
            category = "";
            prefab = "";

            additionals = new string[0];
            additionalsBtns = new string[0];
            additionalsPos = new Vector3[0];
            additionalsRot = new Vector3[0];

            exCategory = "";
            exPrefab = "";
        }

        public void GetPrefabProperties(SwapPrefabList.SwapPrefabProperties _prefab)
        {
            category = _prefab.category;
            prefab = _prefab.prefabName;

            additionals = _prefab.addAdditional;
            additionalsBtns = _prefab.addAdditionalButtonName;
            additionalsPos = _prefab.addAdditionalPos;
            additionalsRot = _prefab.addAdditionalRot;

            exCategory = category;
            exPrefab = prefab;
        }

        public void Initialize()
        {
            FindPrefabProperties();

            CreateCategoryList();
            CreatePrefabList();
        }


#if UNITY_EDITOR
        public void Awake()
        {
            Initialize();
        }

        private void OnValidate()
        {

            Event e = Event.current;

            if (e != null)
            {
                if (e.type == EventType.ExecuteCommand && e.commandName == "Duplicate")
                {
                    placed = true;
                }
            }

            CreateCategoryList();
            CreatePrefabList();
        }
#endif
    }
}

//*****  (©) Finward Studios 2021. All rights reserved. *****\\
//*****     SwapPrefab written by: Jan Procházka  *****\\