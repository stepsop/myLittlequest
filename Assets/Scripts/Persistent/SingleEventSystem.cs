// Assets/Scripts/Persistent/SingleEventSystem.cs
using UnityEngine;
using UnityEngine.EventSystems;

public class SingleEventSystem : MonoBehaviour
{
    private void Awake()
    {
        var all = FindObjectsByType<EventSystem>(FindObjectsInactive.Include); ;

        // Если нас больше одного — уничтожаем себя
        if (all.Length > 1)
            Destroy(gameObject);
    }
}