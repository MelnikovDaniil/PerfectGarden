using UnityEngine;

public class TutorialPointer : MonoBehaviour
{
    public Animator fingerAnimator;

    private Transform targetObject;
    private RectTransform targetRectTransform;
    private bool isUIElement;

    private void Update()
    {
        if (targetObject != null)
        {
            if (isUIElement && targetRectTransform != null)
            {
                transform.position = targetRectTransform.position;
            }
            else
            {
                transform.position = Camera.main.WorldToScreenPoint(targetObject.position);
            }
        }
    }

    public void Show(Transform targetObject, InteractionType interactionType)
    {
        this.targetObject = targetObject;
        this.targetRectTransform = targetObject.GetComponent<RectTransform>();
        this.isUIElement = targetRectTransform != null;

        gameObject.SetActive(true);
        fingerAnimator.Play($"Pointer_{interactionType}", 0, 0);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        targetObject = null;
        targetRectTransform = null;
    }
}