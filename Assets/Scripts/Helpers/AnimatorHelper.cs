using System.Threading.Tasks;
using UnityEngine;

public static class AnimatorHelper
{
    public static async Task PlayAnimationForTheEndAsync(Animator animator, string animationName)
    {
        animator.Play(animationName, 0, 0);
        await Task.Yield();
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(animationName) &&
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            await Task.Yield();
        }
    }
}
