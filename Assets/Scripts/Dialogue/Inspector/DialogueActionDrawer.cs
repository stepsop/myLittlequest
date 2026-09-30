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
                Draw("itemPrefab");
                Draw("hideOptionAfterGive");
                break;

            case DialogueActionType.SetFlag:
                Draw("targetNpcId");
                Draw("applyLoyal");
                if (prop.FindPropertyRelative("applyLoyal").boolValue)
                    Draw("loyalValue");
                Draw("applyLocked");
                if (prop.FindPropertyRelative("applyLocked").boolValue)
                    Draw("lockedValue");
                break;

            case DialogueActionType.DestroyObject:
                Draw("targetNpcId");
                Draw("targetItemUniqueId");
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
                h += GetHeight(prop, "itemPrefab", sp);
                h += GetHeight(prop, "hideOptionAfterGive", sp);
                break;

            case DialogueActionType.SetFlag:
                h += GetHeight(prop, "targetNpcId", sp);
                h += GetHeight(prop, "applyLoyal", sp);
                if (prop.FindPropertyRelative("applyLoyal").boolValue)
                    h += GetHeight(prop, "loyalValue", sp);
                h += GetHeight(prop, "applyLocked", sp);
                if (prop.FindPropertyRelative("applyLocked").boolValue)
                    h += GetHeight(prop, "lockedValue", sp);
                break;

            case DialogueActionType.DestroyObject:
                h += GetHeight(prop, "targetNpcId", sp);
                h += GetHeight(prop, "targetItemUniqueId", sp);
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