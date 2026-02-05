


[System.Serializable]
public struct DefenseWindowParameters
{
    public float Block;
    public float Parry;
    public float PerfectParry;

    public DefenseWindowParameters(float blockWindow, float parryWindow, float perfectParryWindow)
    {
        Block = blockWindow;
        Parry = parryWindow;
        PerfectParry = perfectParryWindow;
    }
}