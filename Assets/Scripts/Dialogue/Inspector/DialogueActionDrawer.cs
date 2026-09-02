using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DialogueAction))]
public class DialogueActionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
    {
        EditorGUI.BeginProperty(pos, label, prop);
        float y = pos.y;
        float sp = EditorGUIUtility.standardVerticalSpacing;

        var typeProp = prop.FindPropertyRelative("type");

        void Draw(string field)
        {
            var p = prop.FindPropertyRelative(field);
            float ph = EditorGUI.GetPropertyHeight(p, true);
            EditorGUI.PropertyField(new Rect(pos.x, y, pos.width, ph), p, true);
            y += ph + sp;
        }

        Draw("type");

        var type = (DialogueActionType)typeProp.enumValueIndex;
        switch (type)
        {
            case DialogueActionType.GiveItem:
                Draw("item");
                break;
            case DialogueActionType.SetFlag:
                Draw("targetState");
                Draw("applyLoyal");
                if (prop.FindPropertyRelative("applyLoyal").boolValue) Draw("loyalValue");
                Draw("applyLocked");
                if (prop.FindPropertyRelative("applyLocked").boolValue) Draw("lockedValue");
                break;
            case DialogueActionType.DestroyObject:
                Draw("targetObject");
                break;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
    {
        float sp = EditorGUIUtility.standardVerticalSpacing;
        float h = EditorGUI.GetPropertyHeight(prop.FindPropertyRelative("type"), true) + sp;

        var type = (DialogueActionType)prop.FindPropertyRelative("type").enumValueIndex;
        switch (type)
        {
            case DialogueActionType.GiveItem:
                h += EditorGUI.GetPropertyHeight(prop.FindPropertyRelative("item"), true) + sp;
                break;
            case DialogueActionType.SetFlag:
                h += EditorGUI.GetPropertyHeight(prop.FindPropertyRelative("targetState"), true) + sp;
                h += EditorGUI.GetPropertyHeight(prop.FindPropertyRelative("applyLoyal"), true) + sp;
                if (prop.FindPropertyRelative("applyLoyal").boolValue)
                    h += EditorGUI.GetPropertyHeight(prop.FindPropertyRelative("loyalValue"), true) + sp;
                h += EditorGUI.GetPropertyHeight(prop.FindPropertyRelative("applyLocked"), true) + sp;
                if (prop.FindPropertyRelative("applyLocked").boolValue)
                    h += EditorGUI.GetPropertyHeight(prop.FindPropertyRelative("lockedValue"), true) + sp;
                break;
            case DialogueActionType.DestroyObject:
                h += EditorGUI.GetPropertyHeight(prop.FindPropertyRelative("targetObject"), true) + sp;
                break;
        }
        return h;
    }
}