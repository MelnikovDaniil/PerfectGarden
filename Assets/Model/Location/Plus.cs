using UnityEngine;

public class PlusScript : MonoBehaviour
{
    public Transform plusTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        plusTransform.LookAt(Camera.main.transform);
        plusTransform.localEulerAngles += new Vector3(-90, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
