using UnityEngine;

[CreateAssetMenu(fileName = "CharacterInfo", menuName = "ScriptableObjects/OrderCharacters")]
public class OrderCharacterInfo : ScriptableObject
{
    public string name;
    public Sprite regular;
    public Sprite angry;
    public Sprite fun;
    public Sprite qute;
}
