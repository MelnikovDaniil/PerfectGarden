using Assets.Scripts.Common;
using UnityEngine;

public static class GuideMapper
{
    private const string MapperName = "Guide";

    public static bool IsGuideComplete(GuideStep guideStep)
    {
        return PlayerPrefs.GetInt(MapperName + guideStep.ToString(), 0) == 1;
    }
    public static void Complete(GuideStep guideStep)
    {
        PlayerPrefs.SetInt(MapperName + guideStep.ToString(), 1);
    }
}
