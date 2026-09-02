using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DialogueOption))]
public class DialogueOptionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
    {
        EditorGUI.BeginProperty(pos, label, prop);

        var y = pos.y;
        float h = EditorGUIUtility.singleLineHeight;
        float sp = EditorGUIUtility.standardVerticalSpacing;

        void Draw(string field)
        {
            var p = prop.FindPropertyRelative(field);
            float ph = EditorGUI.GetPropertyHeight(p, true);
            EditorGUI.PropertyField(new Rect(pos.x, y, pos.width, ph), p, true);
            y += ph + sp;
        }

        Draw("text");
        Draw("nextDialogue");
        Draw("useCondition");

        if (prop.FindPropertyRelative("useCondition").boolValue)
        {
            EditorGUI.indentLevel++;
            Draw("conditionLogic");
            Draw("requiredItem");
            Draw("requiredLoyalState");
            EditorGUI.indentLevel--;
        }

        Draw("actions");

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
    {
        float h = 0;
        float sp = EditorGUIUtility.standardVerticalSpacing;

        h += EditorGUI.GetPropertyHeight(prop.FindPropertyRelative("text"), true) + sp;
        h += EditorGUI.GetPropertyHeight(prop.FindPropertyRelative("nextDialogue"), true) + sp;
        h += EditorGUI.GetPropertyHeight(prop.FindPropertyRelative("useCondition"), true) + sp;

        if (prop.FindPropertyRelative("useCondition").boolValue)
        {
            h += EditorGUI.GetPropertyHeight(prop.FindPropertyRelative("conditionLogic"), true) + sp;
            h += EditorGUI.GetPropertyHeight(prop.FindPropertyRelative("requiredItem"), true) + sp;
            h += EditorGUI.GetPropertyHeight(prop.FindPropertyRelative("requiredLoyalState"), true) + sp;
        }

        h += EditorGUI.GetPropertyHeight(prop.FindPropertyRelative("actions"), true) + sp;

        return h;
    }
}