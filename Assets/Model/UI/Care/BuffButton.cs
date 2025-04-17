using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[RequireComponent(typeof(Button))]
public class BuffButton : MonoBehaviour
{
    public BuffType buffType;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(async () => await CareManager.Instance.StartBuffAsync(buffType));
    }
}
