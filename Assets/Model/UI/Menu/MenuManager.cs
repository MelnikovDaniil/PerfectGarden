using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject GardenCanvas;

    public GameObject OrderCanvas;

    public void OpenGardenLocation()
    {
        GardernManager.Instance.Open();
        GardenCanvas.SetActive(true);

        OrderManager.Instance.Close();
        OrderCanvas.SetActive(false);
    }


    public void OpenOrderLocation()
    {
        GardernManager.Instance.Close();
        GardenCanvas.SetActive(false);

        OrderManager.Instance.Open();
        OrderCanvas.SetActive(true);
    }
}
