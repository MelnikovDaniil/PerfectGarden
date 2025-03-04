using UnityEngine;
using UnityEngine.UI;

public class InteractionController
{
    public GameObject activeObject;

    // Метод для установки активного объекта
    public void BlockAllExcept(GameObject obj)
    {
        if (obj == null)
        {
            UnlockObjects();
            return;
        }

        activeObject = obj;

        // Устанавливаем взаимодействие для всех объектов на сцене
        SetInteractableStateForAllObjects();
    }

    // Метод для установки взаимодействия только с активным объектом
    private void SetInteractableStateForAllObjects()
    {
        // Перебираем все коллайдеры и блокируем или разблокируем их в зависимости от объекта
        Collider[] colliders = GameObject.FindObjectsByType<Collider>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var collider in colliders)
        {
            if (collider == null) continue;
            // Проверяем, является ли текущий объект или его родитель активным
            bool isActive = IsChildOrSame(collider.gameObject, activeObject);
            collider.enabled = isActive;
        }

        // Перебираем все кнопки и блокируем или разблокируем их
        Button[] buttons = GameObject.FindObjectsByType<Button>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var button in buttons)
        {
            if (button == null) continue;
            // Проверяем, является ли текущий объект или его родитель активным
            bool isActive = IsChildOrSame(button.gameObject, activeObject);
            button.interactable = isActive;
        }
    }

    // Вспомогательная функция для проверки, является ли один объект дочерним или тем же самым
    private bool IsChildOrSame(GameObject potentialChild, GameObject parent)
    {
        if (potentialChild == parent) return true;
        if (parent == null) return false;

        Transform current = potentialChild.transform;
        while (current != null)
        {
            if (current.gameObject == parent) return true;
            current = current.parent;
        }

        return false;
    }

    // Метод для разблокировки всех объектов
    public void UnlockObjects()
    {
        activeObject = null;

        // Разблокируем все объекты
        Collider[] colliders = GameObject.FindObjectsByType<Collider>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var collider in colliders)
        {
            if (collider == null) continue;
            collider.enabled = true;
        }

        Button[] buttons = GameObject.FindObjectsByType<Button>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var button in buttons)
        {
            if (button == null) continue;
            button.interactable = true;
        }
    }
}