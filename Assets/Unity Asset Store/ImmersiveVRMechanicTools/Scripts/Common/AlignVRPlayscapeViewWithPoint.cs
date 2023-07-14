using System.Collections;
using System.Linq;
using ReliableSolutions.Unity.Common.Extensions;
using ReliableSolutions.Unity.Common.PropertyDrawer;
using UnityEngine;

public class AlignVRPlayscapeViewWithPoint: MonoBehaviour
{
    [SerializeField] private Transform _playscapeToAlign;
    [SerializeField] private Transform _offset;
    [SerializeField] private Vector3 _customAdjustment;
    [SerializeField] private Transform _alightToReference;

    [SerializeField] private bool _autoStartAfterPlayEnabled;
    [SerializeField] [ShowIf(nameof(_autoStartAfterPlayEnabled))] private float _autoStartNSecondsAfterPlay = 1f;

    [SerializeField] private bool _updateEveryFrame;

    private Vector3 _playscapeCalculatedPosition;

    void Start()
    {
        var enabledAlignVrPlayscapeViewScripts = Object.FindObjectsOfType<AlignVRPlayscapeViewWithPoint>().Where(p => p.enabled).ToList();
        if (enabledAlignVrPlayscapeViewScripts.Count > 1)
        {
            foreach (var enabledAlignVrPlayscapeViewScript in enabledAlignVrPlayscapeViewScripts)
            {
                Debug.LogError($"There are multiple active {nameof(AlignVRPlayscapeViewWithPoint)} - please make sure only 1 is enabled at scene start, current: " +
                               $"{enabledAlignVrPlayscapeViewScript.name}", enabledAlignVrPlayscapeViewScript);
            }
        }

        if (_autoStartAfterPlayEnabled) 
            StartCoroutine(Align(_autoStartNSecondsAfterPlay));
    }

    void Update()
    {
        if (_updateEveryFrame)
            _playscapeToAlign.SetPosition(_playscapeCalculatedPosition + _customAdjustment);
    }

    private IEnumerator Align(float secondsDelay)
    {
        yield return new WaitForSeconds(secondsDelay);
        Align();
    }

    [ContextMenu("Align")]
    private void Align()
    {
        //var sceneViewCameraRotation = sceneViewCameraTransform.rotation;

        _playscapeToAlign.SetPosition(Vector3.zero);
        _playscapeToAlign.SetPosition(_alightToReference.position + (_offset.position * -1));
        _playscapeCalculatedPosition = _playscapeToAlign.position;

        //TODO: rotation is not yet finished, re-add when needed
        Debug.Log("Playscape postion realigned, rotation still todo");
    }

    [ContextMenu("Create Playscape reference from current SceneView")]
    private void CapturePlayscapeReference()
    {
#if UNITY_EDITOR
        var go = new GameObject("Playspace Reference");
        var sceneViewCameraTransform = UnityEditor.SceneView.lastActiveSceneView.camera.gameObject.transform;
        go.transform.SetPosition(sceneViewCameraTransform.position);
        go.transform.SetRotation(sceneViewCameraTransform.rotation);
        go.transform.SetParentTracked(gameObject.transform);

        _alightToReference = go.transform;
#endif
    }
}
