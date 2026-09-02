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

        void Draw(string field)
        {
            var p = prop.FindPropertyRelative(field);

            if (p == null)
                return;

            float ph = EditorGUI.GetPropertyHeight(p, true);

            EditorGUI.PropertyField(
                new Rect(pos.x, y, pos.width, ph),
                p,
                true
            );

            y += ph + sp;
        }

        Draw("type");

        var typeProp = prop.FindPropertyRelative("type");

        if (typeProp == null)
        {
            EditorGUI.EndProperty();
            return;
        }

        var type = (DialogueActionType)typeProp.enumValueIndex;

        switch (type)
        {
            case DialogueActionType.GiveItem:
                Draw("item");
                Draw("giverNpc");
                break;

            case DialogueActionType.SetFlag:
                Draw("targetNpc");

                Draw("applyLoyal");

                var applyLoyal = prop.FindPropertyRelative("applyLoyal");
                if (applyLoyal != null && applyLoyal.boolValue)
                    Draw("loyalValue");

                Draw("applyLocked");

                var applyLocked = prop.FindPropertyRelative("applyLocked");
                if (applyLocked != null && applyLocked.boolValue)
                    Draw("lockedValue");

                break;

            case DialogueActionType.DestroyObject:
                Draw("targetObject");
                break;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(
        SerializedProperty prop,
        GUIContent label)
    {
        float sp = EditorGUIUtility.standardVerticalSpacing;

        var typeProp = prop.FindPropertyRelative("type");

        if (typeProp == null)
            return EditorGUIUtility.singleLineHeight;

        float h = EditorGUI.GetPropertyHeight(typeProp, true) + sp;

        var type = (DialogueActionType)typeProp.enumValueIndex;

        switch (type)
        {
            case DialogueActionType.GiveItem:
                h += GetHeight(prop, "item", sp);
                h += GetHeight(prop, "giverNpc", sp);
                break;

            case DialogueActionType.SetFlag:
                h += GetHeight(prop, "targetNpc", sp);
                h += GetHeight(prop, "applyLoyal", sp);

                var applyLoyal = prop.FindPropertyRelative("applyLoyal");

                if (applyLoyal != null && applyLoyal.boolValue)
                    h += GetHeight(prop, "loyalValue", sp);

                h += GetHeight(prop, "applyLocked", sp);

                var applyLocked = prop.FindPropertyRelative("applyLocked");

                if (applyLocked != null && applyLocked.boolValue)
                    h += GetHeight(prop, "lockedValue", sp);

                break;

            case DialogueActionType.DestroyObject:
                h += GetHeight(prop, "targetObject", sp);
                break;
        }

        return h;
    }

    private float GetHeight(
        SerializedProperty prop,
        string field,
        float spacing)
    {
        var p = prop.FindPropertyRelative(field);

        if (p == null)
            return 0f;

        return EditorGUI.GetPropertyHeight(p, true) + spacing;
    }
}