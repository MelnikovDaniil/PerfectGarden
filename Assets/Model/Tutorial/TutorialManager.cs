using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    public Canvas tutorialCanvas;
    public List<TutorialScenario> tutorialScenarios;
    public TutorialPointer pointerPrefab;

    private List<TutorialPointer> pointersPool = new List<TutorialPointer>();
    private InteractionController interactionController;
    private HighlightManager highlightManager;

    private Vector3? startPosition;
    private float holdTime;

    private int tapCount;
    private float tapTimeWindow = 0.5f;

    private void Awake()
    {
        Instance = this;
        interactionController = new InteractionController();
        highlightManager = GetComponent<HighlightManager>();
    }

    #region Setters

    public async Task SetTap(GameObject obj, bool isBlocking = false, CancellationToken token = default)
    {
        Func<bool> clickCondition = null;
        var conditionDone = false;
        if (obj.TryGetComponent(out Button button))
        {
            UnityAction updateConditionState = null;
            updateConditionState = () =>
            {
                conditionDone = true;
                button.onClick.RemoveListener(updateConditionState);
            };
            button.onClick.AddListener(updateConditionState);
            clickCondition = () => conditionDone;
        }
        else
        {
            clickCondition = () => CheckTap(obj);
        }

        await HandleInteraction(new TutorialObject
        {
            InteractionType = InteractionType.Tap,
            IsBlocking = isBlocking,
            TargetObject = obj
        }, clickCondition, token);
    }

    public async Task SetFastTaps(GameObject obj, int tapThreshold, float timeWindow, bool isBlocking = false, CancellationToken token = default)
    {
        tapTimeWindow = timeWindow;
        tapCount = 0;
        await HandleInteraction(new TutorialObject
        {
            InteractionType = InteractionType.FastTaps,
            IsBlocking = isBlocking,
            TargetObject = obj
        }, () => CheckFastTaps(obj, tapThreshold, timeWindow),
        token);
    }

    public async Task SetShake(GameObject obj, bool isBlocking = false, CancellationToken token = default)
    {
        await HandleInteraction(new TutorialObject
        {
            InteractionType = InteractionType.Shake,
            IsBlocking = isBlocking,
            TargetObject = obj
        }, () => CheckTap(obj),
        token);
    }

    public async Task SetSwipeAsync(GameObject obj, Vector2 direction, float lenth, bool isBlocking = false, CancellationToken token = default)
    {
        await HandleInteraction(new TutorialObject
        {
            InteractionType = InteractionType.Swipe,
            IsBlocking = isBlocking,
            TargetObject = obj,
            swipeDirection = direction.normalized * lenth,
        }, () => CheckSwipe(direction, lenth),
        token);
    }

    public async Task SetHoldAsync(GameObject obj, float seconds, bool isBlocking = false, CancellationToken token = default)
    {
        await HandleInteraction(new TutorialObject
        {
            InteractionType = InteractionType.Hold,
            IsBlocking = isBlocking,
            TargetObject = obj
        }, () => CheckHold(obj, seconds),
        token);
    }

    #endregion

    private async Task HandleInteraction(TutorialObject tutorialObject, Func<bool> condition, CancellationToken token = default)
    {
        if (tutorialObject.IsBlocking)
        {
            interactionController.BlockAllExcept(tutorialObject.TargetObject);

            highlightManager.HighlightObject(tutorialObject.TargetObject);
        }

        var pointer = PlayHandAnimation(tutorialObject.InteractionType, tutorialObject.TargetObject);
        pointer.fingerAnimator.SetFloat("swipeX", tutorialObject.swipeDirection.x);
        pointer.fingerAnimator.SetFloat("swipeY", tutorialObject.swipeDirection.y);

        while (!condition.Invoke()
            && tutorialObject.TargetObject != null
            && tutorialObject.TargetObject.gameObject.activeSelf
            && !token.IsCancellationRequested)
        {
            if (CheckHold(tutorialObject.TargetObject))
            {
                if (pointer.gameObject.activeSelf)
                {
                    pointer.Hide();
                }
            }
            else
            {
                if (!pointer.isActiveAndEnabled)
                {
                    pointer.Show(tutorialObject.TargetObject.transform, tutorialObject.InteractionType);
                    pointer.fingerAnimator.SetFloat("swipeX", tutorialObject.swipeDirection.x);
                    pointer.fingerAnimator.SetFloat("swipeY", tutorialObject.swipeDirection.y);
                }
            }
            await Task.Yield();
        }


        pointer.Hide();

        if (tutorialObject.IsBlocking)
        {
            highlightManager.StopHighlight();
            interactionController.UnlockObjects();
        }
    }

    private TutorialPointer PlayHandAnimation(InteractionType interactionType, GameObject targetObject)
    {
        var pointer = pointersPool.FirstOrDefault(x => !x.isActiveAndEnabled);
        if (pointer == null)
        {
            pointer = Instantiate(pointerPrefab, tutorialCanvas.transform);
            pointersPool.Add(pointer);
        }

        pointer.Show(targetObject.transform, interactionType);
        Debug.Log($"Playing hand animation for {interactionType} on {targetObject.name}");

        return pointer;
    }

    #region Checks

    private bool CheckTap(GameObject obj)
    {
        if (Input.GetMouseButtonDown(0))
        {
            return IsOverGameObject(obj);
        }
        return false;
    }

    private bool CheckHold(GameObject obj)
    {
        if (Input.GetMouseButton(0) && IsOverGameObject(obj))
        {
            return true;
        }
        return false;
    }

    private bool CheckHold(GameObject obj, float seconds)
    {
        if (Input.GetMouseButtonDown(0) && IsOverGameObject(obj))
        {
            holdTime = seconds;
        }

        if (Input.GetMouseButton(0) && IsOverGameObject(obj))
        {
            holdTime -= Time.deltaTime;
            if (holdTime <= 0)
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckFastTaps(GameObject obj, int tapThreshold, float timeWindow)
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsOverGameObject(obj))
            {
                tapCount++;
                if (tapCount >= tapThreshold)
                {
                    return true;
                }
            }
        }

        if (tapTimeWindow > 0)
        {
            tapTimeWindow -= Time.deltaTime;
        }
        else
        {
            tapTimeWindow = timeWindow;
            tapCount = 0;
        }

        return false;
    }

    private bool CheckSwipe(Vector2 direction, float minSwipeDistance)
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
        }
        else if (startPosition.HasValue && Input.GetMouseButtonUp(0))
        {
            var endPosition = Input.mousePosition;
            var swipe = endPosition - startPosition.Value;
            if (swipe.magnitude >= minSwipeDistance)
            {
                var swipeDirection = swipe.normalized;
                if (Vector2.Dot(swipeDirection, direction.normalized) > 0.7f)
                {
                    return true;
                }
            }
            startPosition = null;
        }
        return false;
    }

    private bool IsOverGameObject(GameObject target)
    {
        if (target.GetComponent<RectTransform>() != null)
        {
            var pointerEventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            foreach (var result in raycastResults)
            {
                if (result.gameObject == target)
                {
                    return true;
                }
            }
            return false;
        }

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            return IsTargetOrParent(hit.collider.gameObject, target);
        }
        return false;
    }

    private bool IsTargetOrParent(GameObject current, GameObject target)
    {
        if (current == target)
        {
            return true;
        }

        if (current.transform.parent != null)
        {
            return IsTargetOrParent(current.transform.parent.gameObject, target);
        }

        return false;
    }
    #endregion
}