using UnityEngine;
using UnityEngine.UI;

public class FinishPlanting : MonoBehaviour
{
    public Canvas canvas;
    public Button finishPlantingButton;

    public void ShowMenu()
    {
        canvas.gameObject.SetActive(true);
    }

    public void HideMenu()
    {
        canvas.gameObject.SetActive(false);
    }
}
