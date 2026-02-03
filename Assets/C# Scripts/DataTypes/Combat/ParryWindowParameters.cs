


[System.Serializable]
public struct ParryWindowParameters
{
    public float ParryWindow;
    public float PerfectParryWindow;

    public ParryWindowParameters(float parryWindow, float perfectParryWindow)
    {
        ParryWindow = parryWindow;
        PerfectParryWindow = perfectParryWindow;
    }
}