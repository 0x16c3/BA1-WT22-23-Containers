using System.Collections.Generic;

[System.Serializable]
public class ScoreTable
{
    public ScoreValues scoreValues;
}

[System.Serializable]
public class ScoreValues
{
    public BeastScoreData Beasts;
    public ContainerScoreData[] Containers;
}

[System.Serializable]
public class BeastScoreData
{
    public int VolcanoCat;
    public int GardeningGoat;
    public int ScorpionShredder;

    public int GetScore(ContainerType type)
    {
        switch (type)
        {
            case ContainerType.VolcanoCat:
                return VolcanoCat;
            case ContainerType.GardeningGoat:
                return GardeningGoat;
            case ContainerType.ScorpionShredder:
                return ScorpionShredder;
            default:
                return 0;
        }
    }
}


[System.Serializable]
public class ContainerScoreData
{
    public string type;
    public int Common;
    public int Rare;
    public int Epic;
    public int Legendary;

    public ContainerType ContainerType => (ContainerType)System.Enum.Parse(typeof(ContainerType), type);

    public int GetScore(ContainerRarity rarity)
    {
        switch (rarity)
        {
            case ContainerRarity.Common:
                return Common;
            case ContainerRarity.Rare:
                return Rare;
            case ContainerRarity.Epic:
                return Epic;
            case ContainerRarity.Legendary:
                return Legendary;
            default:
                return 0;
        }
    }
}
