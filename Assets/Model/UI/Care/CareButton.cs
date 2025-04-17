using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[RequireComponent(typeof(Button))]
public class CareButton : MonoBehaviour
{
    public CareEvent eventName;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(async () => await CareManager.Instance.StartCareAsync(eventName));
    }
}
