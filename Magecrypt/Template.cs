namespace Magecrypt;

public abstract class Template
{
    public int MinLevel { get; set; }
    public int MaxLevel { get; set; }
    public float ChanceWeight { get; set; }

    public Template(int minLevel, int maxLevel, float chanceWeight)
    {
        MinLevel = minLevel;
        MaxLevel = maxLevel;
        ChanceWeight = chanceWeight;
    }
}
