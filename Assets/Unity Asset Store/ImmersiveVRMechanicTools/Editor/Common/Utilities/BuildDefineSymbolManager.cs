using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace ReliableSolutions.Unity.Common.Editor.Utilities
{
    public static class BuildDefineSymbolManager
    {
        public static void SetBuildDefineSymbolState(string buildSymbol, bool isEnabled) =>
            SetBuildDefineSymbolState(buildSymbol, isEnabled, EditorUserBuildSettings.selectedBuildTargetGroup);
        public static void SetBuildDefineSymbolState(string buildSymbol, bool isEnabled, BuildTargetGroup buildTargetGroup)
        {
            EditorUtility.DisplayProgressBar("Please wait", "Modifying build symbols", 0.1f);

            var allBuildSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup)
                .Split(';').ToList();
            var isBuildSymbolDefined = allBuildSymbols.Any(s => s == buildSymbol);

            if (isEnabled && !isBuildSymbolDefined)
            {
                allBuildSymbols.Add(buildSymbol);
                SetBuildSymbols(allBuildSymbols, buildTargetGroup);
                UnityEngine.Debug.Log($"Build Symbol Added: {buildSymbol}");
            }

            if (!isEnabled && isBuildSymbolDefined)
            {
                allBuildSymbols.Remove(buildSymbol);
                SetBuildSymbols(allBuildSymbols, buildTargetGroup);
                UnityEngine.Debug.Log($"Build Symbol Removed: {buildSymbol}");
            }

            EditorUtility.ClearProgressBar();
        }

        private static void SetBuildSymbols(List<string> allBuildSymbols, BuildTargetGroup buildTargetGroup)
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                buildTargetGroup,
                string.Join(";", allBuildSymbols.ToArray())
            );
        }
    }

}
