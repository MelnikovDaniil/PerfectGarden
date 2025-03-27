using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class LevelEstimationText : MonoBehaviour
{
    public TMP_Text text;
    public Animator animator;

    public async Task ShowText(GlassBottleLevel level)
    {
        if (level == GlassBottleLevel.Great)
        {
            text.text = "Great!";
        }
        else if (level == GlassBottleLevel.Good)
        {
            text.text = "Good!";
        }
        else
        {
            text.text = "Try Again!";
        }

        await AnimatorHelper.PlayAnimationForTheEndAsync(animator, "GlassLevelText_Show");
    }
}
