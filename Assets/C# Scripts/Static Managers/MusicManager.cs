using Fire_Pixel.Utility;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// Static class that allows to start and transition to audio (music) clips. Blends 2 audio sources, "current" "new" for a smooth transition.
/// </summary>
public static class MusicManager
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        MusicManagerInstance musicManager = new GameObject(">>MusicManager<<").AddComponent<MusicManagerInstance>();
        GameObject.DontDestroyOnLoad(musicManager.gameObject);
    }

    public static void TransitionToClip(AudioClip clip, NativeSampledAnimationCurve blendCurve, float blendTime)
    {
        MusicManagerInstance.Instance.TransitionToClip(clip, blendCurve, blendTime);
    }

    private class MusicManagerInstance : MonoBehaviour
    {
        public static MusicManagerInstance Instance { get; private set; }


        private AudioSource[] sources;
        [SerializeField] private int cSourceId;

        private float fadeIn;

        private NativeSampledAnimationCurve blendCurve;
        private float blendTime = 1;


        private void Awake()
        {
            Instance = this;

            //Setup 2 audio sources for blending
            sources = new AudioSource[2];
            sources[0] = transform.AddComponent<AudioSource>();
            sources[0].playOnAwake = false;

            sources[1] = transform.AddComponent<AudioSource>();
            sources[1].playOnAwake = false;
        }

        public void TransitionToClip(AudioClip clip, NativeSampledAnimationCurve blendCurve, float blendTime)
        {
            this.blendTime = blendTime;
            this.blendCurve = blendCurve;

            // Flip audio source id
            cSourceId.Flip();
            fadeIn = 0;

            // Start new inactive audio source
            sources[cSourceId].clip = clip;
            sources[cSourceId].Play();

            CallbackScheduler.RegisterUpdate(TransitionClips);
        }

        /// <summary>
        /// Fade active aduio source out while fading inactive audio source in.
        /// </summary>
        private void TransitionClips()
        {
            fadeIn = Mathf.Clamp(fadeIn + Time.deltaTime / blendTime, 0, 1);
            float fadeOut = 1 - fadeIn;

            sources[cSourceId].volume = blendCurve.Evaluate(fadeIn);
            sources[cSourceId.AsFlipped()].volume = blendCurve.Evaluate(fadeOut);

            if (fadeIn == 1)
            {
                sources[cSourceId.AsFlipped()].Stop();
                CallbackScheduler.UnRegisterUpdate(TransitionClips);
            }
        }

        private void OnDestroy()
        {
            this.CancelAllInvokes();
        }
    }
}
