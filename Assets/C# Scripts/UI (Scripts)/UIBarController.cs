using UnityEngine;
using UnityEngine.UI;


public class UIBarController : MonoBehaviour
{
    [SerializeField] private Image bar;



    public void UpdateResource(float value, float percentage)
    {
        bar.fillAmount = percentage;
    }
}