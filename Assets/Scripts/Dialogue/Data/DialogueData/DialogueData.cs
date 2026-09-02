using System.Collections.Generic;
using UnityEngine;

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
}