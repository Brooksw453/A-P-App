using System;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.ImmersiveVRInventory.Common.Utilities
{
    public static class ScriptableObjectUtility
    {
        public static T CreateAsset<T>(string path, string name) where T : ScriptableObject
        {
#if UNITY_EDITOR
            T asset = ScriptableObject.CreateInstance<T>();

            if (Path.GetExtension(path) != "")
            {
                throw new Exception("Please user path without extension");
            }

            var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath($"{path}/{name}.asset");
            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            return asset;
#else
            return null;
#endif
        }
    }
}
