using System.Collections.Generic;
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// ScriptableObject — это файл-данных, который ты создаёшь прямо в Unity
// через ПКМ → Create → Dialogue → Dialogue
[CreateAssetMenu(menuName = "Dialogue/Dialogue")]
public class DialogueData : ScriptableObject
{
    [Header("Игрок — левая сторона")]
    public string playerName;           // Имя игрока, будет над его портретом
    public Sprite playerPortrait;    // Картинка игрока

    [Header("НПС — правая сторона")]
    public string npcName;            // Имя нпс
    public Sprite npcPortrait;       // Картинка нпс

    [Header("Текст диалога")]
    [TextArea(2, 5)]                 // В инспекторе будет удобное большое поле
    public string text;

    public List<DialogueOption> options; // Варианты ответа
    [HideInInspector]
    public string rewardTrackingNamespace;

    public string GetRewardTrackingId(DialogueOption option)
    {
        if (option == null) return string.Empty;
        if (string.IsNullOrEmpty(rewardTrackingNamespace))
            rewardTrackingNamespace = Guid.NewGuid().ToString("N");
        if (string.IsNullOrEmpty(option.rewardTrackingId))
            option.rewardTrackingId = Guid.NewGuid().ToString("N");
        return $"{rewardTrackingNamespace}:{option.rewardTrackingId}";
    }

    private void OnValidate()
    {
        bool changed = false;

#if UNITY_EDITOR
        string assetPath = AssetDatabase.GetAssetPath(this);
        string assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
        if (!string.IsNullOrEmpty(assetGuid) && rewardTrackingNamespace != assetGuid)
        {
            rewardTrackingNamespace = assetGuid;
            changed = true;
        }
#endif

        if (string.IsNullOrEmpty(rewardTrackingNamespace))
        {
            rewardTrackingNamespace = Guid.NewGuid().ToString("N");
            changed = true;
        }

        if (options != null)
        {
            var ids = new HashSet<string>();
            foreach (var option in options)
            {
                if (option == null) continue;
                if (string.IsNullOrEmpty(option.rewardTrackingId) || !ids.Add(option.rewardTrackingId))
                {
                    option.rewardTrackingId = Guid.NewGuid().ToString("N");
                    ids.Add(option.rewardTrackingId);
                    changed = true;
                }
            }
        }

#if UNITY_EDITOR
        if (changed) EditorUtility.SetDirty(this);
#endif
    }
}