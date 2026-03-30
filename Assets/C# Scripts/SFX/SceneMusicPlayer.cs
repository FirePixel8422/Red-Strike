using UnityEngine;


public class SceneMusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip[] clips;

    [SerializeField] private NativeSampledAnimationCurve blendCurve = NativeSampledAnimationCurve.Default;
    [SerializeField] private float blendTime = 1;

    private int cClipId;


    private void Start()
    {
        blendCurve.Bake();
        StartNextClip();
    }

    private void StartNextClip()
    {
        cClipId = Random.Range(0, clips.Length);

        AudioClip clip = clips[cClipId];
        float clipLength = clip.length - blendTime;

        MusicManager.TransitionToClip(clip, blendCurve, blendTime);

        this.Invoke(clipLength, StartNextClip);
    }

    private void OnDestroy()
    {
        blendCurve.Dispose();
        this.CancelAllInvokes();
    }
}