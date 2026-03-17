using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[RequireComponent(typeof(Button))]
public class CareButton : MonoBehaviour
{
    public CareEvent eventName;
    [NonSerialized]
    public RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        GetComponent<Button>().onClick.AddListener(async () => await CareManager.Instance.StartCareAsync(eventName));
    }
}
