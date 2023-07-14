using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ToolRotationChangedSoundsEventHandler : MonoBehaviour
{
#pragma warning disable 649
    [SerializeField] private AudioSource AudioSource;
    [SerializeField] private AudioClip RotatingInNonAccumulatingDirectionSound;
    [SerializeField] private AudioClip BreakingPointSound;
    [SerializeField] private float MinimumAngleChangeToPlaySound = 3f;
    [SerializeField] private float PlaySoundForMinimumNSeconds = 0.2f;
#pragma warning restore 649
    
    private float _rotationChangeCalledAtTimeSinceStart;
    private bool _isPlayingOneShot;
    
    void Start()
    {
        if (AudioSource == null) AudioSource = GetComponent<AudioSource>();
    }

    void LateUpdate()
    {
        if (!_isPlayingOneShot && AudioSource.isPlaying && (Time.fixedTime - _rotationChangeCalledAtTimeSinceStart) > PlaySoundForMinimumNSeconds)
        {
            AudioSource.Stop();
        }
    }

    public void HandleRotationChanged(RotationChanged rotationChangedArgs)
    {
        if(_isPlayingOneShot) return;

        if (Mathf.Abs(rotationChangedArgs.MovedAngle) >= MinimumAngleChangeToPlaySound)
        {
            _rotationChangeCalledAtTimeSinceStart = Time.fixedTime;
        } 

        if (!AudioSource.isPlaying)
        {
            if (rotationChangedArgs.AddRotationProgressStatus == AddRotationProgressStatus.None)
            {
                AudioSource.clip = RotatingInNonAccumulatingDirectionSound;
                AudioSource.Play();
            }
        }

        if (rotationChangedArgs.AddRotationProgressStatus == AddRotationProgressStatus.OverRotatingBreakingPoint)
        {
            PlayOnce(BreakingPointSound);
        }
    }

    public void PlayOnce(AudioClip audioClip)
    {
        PlayOnce(audioClip, 1f);
    }

    private void PlayOnce(AudioClip audioClip, float volumeScale)
    {
        _isPlayingOneShot = true;
        AudioSource.PlayOneShot(audioClip, volumeScale);
        StartCoroutine(ResetPlayOneShot(audioClip.length));
    }

    private IEnumerator ResetPlayOneShot(float resetAfter)
    {
        yield return new WaitForSeconds(resetAfter);
        _isPlayingOneShot = false;
    }
}
