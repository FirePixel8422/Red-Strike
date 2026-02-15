using Fire_Pixel.Utility;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;



[System.Serializable]
public class ResourceBarUI
{
    [SerializeField] private Image bar;
    [SerializeField] private ImageColorAnimator anim;
    [SerializeField] private float lerpSpeed;

    private float targetValue;
    private bool isLerping;


    public void UpdateBar(float value01)
    {
        targetValue = value01;
        if (anim != null)
        {
            anim.enabled = math.distance(targetValue, 1) < 0.001f;
        }

        if (isLerping == false)
        {
            UpdateScheduler.RegisterUpdate(LerpBar);
            isLerping = true;
        }
    }

    /// <summary>
    /// Updated through <see cref="UpdateScheduler.Update"/> when the bar value is changed
    /// </summary>
    private void LerpBar()
    {
        float cValue = bar.fillAmount;
        cValue = Mathf.Lerp(cValue, targetValue, lerpSpeed * Time.deltaTime);

        if (math.distance(cValue, targetValue) < 0.001f)
        {
            bar.fillAmount = targetValue;

            UpdateScheduler.UnRegisterUpdate(LerpBar);
            isLerping = false;
        }
        else
        {
            bar.fillAmount = cValue;
        }
    }
}