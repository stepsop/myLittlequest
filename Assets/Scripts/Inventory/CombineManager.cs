using UnityEngine;

// Отвечает только за логику комбинирования.
// UI и инвентарь вызывает через их Instance — сам ничего не знает про слоты.
public class CombineManager : MonoBehaviour
{
    public static CombineManager Instance { get; private set; }

    [Header("База фраз когда комбинация не найдена")]
    [SerializeField] private FailPhraseDatabase failPhraseDatabase;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void TryCombine(ItemData itemA, ItemData itemB)
    {
        // Ищем рецепт в itemA
        CombineEntry entry = FindEntry(itemA, itemB);

        if (entry != null)
        {
            // Успех — убираем оба предмета, добавляем результат
            InventoryManager.Instance.RemoveItem(itemA);
            InventoryManager.Instance.RemoveItem(itemB);
            InventoryManager.Instance.AddItem(entry.result);

            // Показываем фразу успеха над персонажем
            SpeechBubble.Instance.Show(entry.successPhrase, entry.successAudio);
            return;
        }

        // Неудача — случайная фраза из базы
        FailPhrase fail = failPhraseDatabase != null
            ? failPhraseDatabase.GetRandom()
            : null;

        if (fail != null)
            SpeechBubble.Instance.Show(fail.text, fail.audio);
    }

    private CombineEntry FindEntry(ItemData a, ItemData b)
    {
        // Проверяем оба направления — A→B и B→A
        CombineEntry entry = SearchIn(a, b);
        if (entry == null) entry = SearchIn(b, a);
        return entry;
    }

    private CombineEntry SearchIn(ItemData source, ItemData target)
    {
        // source может быть null (например пустой слот) — тогда рецептов нет
        if (source == null || source.combineWith == null) return null;

        foreach (var entry in source.combineWith)
        {
            if (entry.otherItem == target)
                return entry;
        }
        return null;
    }
}