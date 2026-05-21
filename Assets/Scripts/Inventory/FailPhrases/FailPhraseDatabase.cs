 using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Fail Phrase Database")]
public class FailPhraseDatabase : ScriptableObject
{
    public FailPhrase[] phrases;

    public FailPhrase GetRandom()
    {
        if (phrases == null || phrases.Length == 0) return null;

        var available = System.Array.FindAll(phrases, p => !p.used);
        if (available.Length == 0)
        {
            foreach (var p in phrases) p.used = false;
            available = phrases;
        }

        int index = Random.Range(0, available.Length);
        available[index].used = true;
        return available[index];
    }
}

[System.Serializable]
public class FailPhrase
{
    [TextArea] public string text;
    public AudioClip audio;
    [HideInInspector] public bool used;
}