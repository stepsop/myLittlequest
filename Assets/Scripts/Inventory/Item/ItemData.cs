using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("Основное")]
    public string itemName => name;
    public Sprite icon;
    public bool consumable = true;

    [Header("Осмотр")]
    [TextArea] public string inspectPhrase;
    public AudioClip inspectAudio;
    public ItemData inspectTransformTo;

    [Header("Комбинирование")]
    public CombineEntry[] combineWith;
}

[System.Serializable]
public class CombineEntry
{
    public ItemData otherItem;
    public ItemData result;
    [TextArea] public string successPhrase;
    public AudioClip successAudio;
}