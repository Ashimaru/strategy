using System;
using UnityEngine;

[Serializable]
public struct GroupRequirements
{
    public int Size;
    public int Power;
}
[Serializable]
public class VillageHeatSettings
{
    public GroupRequirements MinimumGarrisonToSendResources;
    public GroupRequirements ResourceGroupRequirements;
    public GroupRequirements PatrolRequirements;
}

[Serializable]
public class AIHeatSettings
{
    public VillageHeatSettings VillageSettings;
}

[CreateAssetMenu(fileName = "HeatSettings", menuName = "Game/Heat settings")]
public class HeatSettings : ScriptableObject
{
    [SerializeField]
    private AIHeatSettings LowHeatSettings;
    [SerializeField]
    private AIHeatSettings MediumHeatSettings;
    [SerializeField]
    private AIHeatSettings HighHeatSettings;

    public AIHeatSettings GetHeatSettings(HeatLevel currentHeatLevel)
    {
        switch (currentHeatLevel)
        {
            case HeatLevel.Low:
                return LowHeatSettings;
            case HeatLevel.Medium:
                return MediumHeatSettings;
            case HeatLevel.High:
                return HighHeatSettings;
        }

        Debug.Assert(false);
        return null;
    }
}
