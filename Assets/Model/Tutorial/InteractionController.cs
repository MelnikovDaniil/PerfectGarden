using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionController
{
    public GameObject activeObject;
    private Dictionary<Button, bool> changedButtons = new Dictionary<Button, bool>();

    public void BlockAllExcept(GameObject obj)
    {
        if (obj == null)
        {
            UnlockObjects();
            return;
        }

        activeObject = obj;

        SetInteractableStateForAllObjects();
    }

    private void SetInteractableStateForAllObjects()
    {
        var colliders = GameObject.FindObjectsByType<Collider>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var collider in colliders)
        {
            if (collider == null) continue;
            var isActive = IsChildOrSame(collider.gameObject, activeObject);
            collider.enabled = isActive;
        }

        var buttons = GameObject.FindObjectsByType<Button>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var button in buttons)
        {
            if (button == null) continue;
            changedButtons.Add(button, button.interactable);
            var isActive = IsChildOrSame(button.gameObject, activeObject);
            button.interactable = isActive;
        }
    }

    private bool IsChildOrSame(GameObject potentialChild, GameObject parent)
    {
        if (potentialChild == parent) return true;
        if (parent == null) return false;

        var current = potentialChild.transform;
        while (current != null)
        {
            if (current.gameObject == parent) return true;
            current = current.parent;
        }

        return false;
    }

    public void UnlockObjects()
    {
        activeObject = null;

        var colliders = GameObject.FindObjectsByType<Collider>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var collider in colliders)
        {
            if (collider == null) continue;
            collider.enabled = true;
        }

        foreach (var button in changedButtons)
        {
            button.Key.interactable = button.Value;
        }

        changedButtons.Clear();
    }
}