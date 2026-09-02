using UnityEngine;

// Общий хелпер для UI-компонентов со статическим Instance (DialogueUI,

public static class UIActivator
{
    // Ищет компонент типа T (включая выключенные объекты) и активирует
   
    public static T FindAndActivate<T>() where T : Component
    {
        T component = Object.FindAnyObjectByType<T>(FindObjectsInactive.Include);
        if (component == null)
            return null;

        ActivateHierarchy(component.transform);
        return component;
    }

    // Включает GameObject и всех его родителей по цепочке —
    
    public static void ActivateHierarchy(Transform target)
    {
        Transform current = target;
        while (current != null)
        {
            if (!current.gameObject.activeSelf)
                current.gameObject.SetActive(true);

            current = current.parent;
        }
    }
}