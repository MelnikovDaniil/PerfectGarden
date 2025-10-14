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

    public static float GetGuideProgress(GuideStep guideStep)
    {
        return PlayerPrefs.GetFloat(MapperName + "Progress" + guideStep.ToString(), 0);
    }

    public static void SetGuideProgress(GuideStep guideStep, float progress)
    {
        PlayerPrefs.SetFloat(MapperName + "Progress" + guideStep.ToString(), progress);
    }
}
