using UnityEngine;

public class RainAudio : MonoBehaviour
{
    [SerializeField] private MinMaxFloat pitchChangeDelayRange;
    [SerializeField] private MinMaxFloat pitchRange;

    private AudioSource audioSource;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        this.Invoke(EzRandom.Range(pitchChangeDelayRange), ChangePitch);
    }

    private void ChangePitch()
    {
        audioSource.pitch = EzRandom.Range(pitchRange);

        this.Invoke(pitchChangeDelayRange.RandomValue, ChangePitch);
    }

    private void OnDestroy()
    {
        this.CancelAllInvokes();
    }
}
