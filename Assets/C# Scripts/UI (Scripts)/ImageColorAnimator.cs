using UnityEngine;
using UnityEngine.UI;


public class ImageColorAnimator : UpdateMonoBehaviour
{
    [SerializeField] private Color a, b;
    [SerializeField] private float lerpTime;

    private Image targetImage;


    private void Awake()
    {
        targetImage = GetComponent<Image>();
    }
    protected override void OnUpdate()
    {
        float t = Mathf.PingPong(Time.time, lerpTime) / lerpTime;
        targetImage.color = Color.Lerp(a, b, t);
    }
}
