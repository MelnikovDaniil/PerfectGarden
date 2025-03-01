using UnityEngine;

[CreateAssetMenu(fileName = "PotInfo", menuName = "ScriptableObjects/PotInfo")]
public class PotInfo : ScriptableObject
{
    public string name;

    public PlantType plantType;

    public Sprite shopSprite;

    public int price;

    public PotWithPlant potPrefab;
}