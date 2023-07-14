using System.Linq;
using UnityEngine;

public class MissingAssetsNotifier : MonoBehaviour
{
    private const string IgnoreMissingAssetsEditorPrefKey = "IgnoreMissingAssetsEditor";
    [SerializeField] private string PopupTitle = "Missing Assets";
    [SerializeField] [Multiline] private string PopupMessage = "Some assets are missing. You'll find more information in the console.";

    [ContextMenu("Check For Missing Assets")]
    public void CheckFormMissingAssets()
    {
#if UNITY_EDITOR
        var ignoreMissingAssets = UnityEditor.EditorPrefs.GetBool(IgnoreMissingAssetsEditorPrefKey);
        if(ignoreMissingAssets) return;
        
        var activeMeshFilters = ((MeshFilter[])Resources.FindObjectsOfTypeAll(typeof(MeshFilter)))
            .Where(mf => mf.gameObject.activeInHierarchy)
            .ToList();
        var isAnyMeshMissing = false;
        foreach (var mf in activeMeshFilters)
        {
            if (mf.sharedMesh == null || mf.sharedMesh.vertexCount == 0)
            {
                Debug.LogWarning("Missing Mesh", mf.gameObject);
                isAnyMeshMissing = true;
            }
        }

        if (isAnyMeshMissing)
        {
            var shouldNotShowAnymore = !UnityEditor.EditorUtility.DisplayDialog(
                PopupTitle, PopupMessage, "Ok", "Ok, don't show this warning again."
            );
            if (shouldNotShowAnymore)
            {
                UnityEditor.EditorPrefs.SetBool(IgnoreMissingAssetsEditorPrefKey, true);
            }
        }
#endif
    }
}