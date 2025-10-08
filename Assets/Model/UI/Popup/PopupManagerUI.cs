using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PopupManagerUI : MonoBehaviour
{
    public static PopupManagerUI Instance;
    public Canvas canvas;
    public PopupDialogueWindow popupDialogueWindow;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        canvas.gameObject.SetActive(false);
    }

    // Option 0
    // Brainstorm with AI
    // Option 1
    // Get money (buy in-game currency). Offer type: Small, Large,
    // Option 2
    // Get a bonus for viewing an ad
    // Option 3
    // Purchase at a discount, limited time

    public void ShowAddMoneyPopup(Action<bool> onComplete = null)
    {
        canvas.gameObject.SetActive(true);
        popupDialogueWindow.acceptButton.onClick.RemoveAllListeners();
        popupDialogueWindow.acceptButton.onClick.AddListener(() =>
        {
            onComplete?.Invoke(true);
            canvas.gameObject.SetActive(false);
        });
        popupDialogueWindow.rejectButton.onClick.RemoveAllListeners();
        popupDialogueWindow.rejectButton.onClick.AddListener(() =>
        {
            onComplete?.Invoke(false);
            canvas.gameObject.SetActive(false);
        });

    }
}
