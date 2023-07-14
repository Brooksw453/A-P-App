using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrefabSwapperSuburb
{

    //*****  (©) Finward Studios 2021. All rights reserved. *****\\

    [AddComponentMenu("PrefabSwapper/SwapPrefabListVisualizerSuburb")]
    public class SwapPrefabListVisualizer : MonoBehaviour {

        [Header("Use Rightclick > Refresh to refresh list")]
        public SwapPrefabList.SwapPrefabProperties[] prefabList;

        [ContextMenu("Refresh")]
        public void Refresh()
        {
            prefabList = SwapPrefabList.swapPrefabProperties;
            Debug.Log("PrefabSwapper ::: Refreshing visualization prefab list");
        }
    }
}

//*****  (©) Finward Studios 2021. All rights reserved. *****\\
//*****     SwapPrefab written by: Jan Procházka  *****\\