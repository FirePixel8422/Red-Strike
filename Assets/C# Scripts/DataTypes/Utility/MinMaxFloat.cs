


/// <summary>
/// A lightweight struct that holds a float min and float max.
/// </summary>
[System.Serializable]
public struct MinMaxFloat
{
    public float min;
    public float max;

    public MinMaxFloat(float min, float max)
    {
        this.min = min;
        this.max = max;
    }

    /// <returns>A random value between <see cref="min"/> and <see cref="max"/></returns>
    public readonly float RandomValue => EzRandom.Range(min, max);
}