using System;
using UnityEngine;

[Serializable]
public class TutorialObject
{
    public GameObject TargetObject;
    public InteractionType InteractionType;
    public bool IsBlocking;
    public Vector2 swipeDirection;
}