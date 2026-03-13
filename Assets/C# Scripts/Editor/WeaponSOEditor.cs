using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(WeaponSkillEntry))]
public class WeaponSkillEntryDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect rect = position;
        rect.height = EditorGUIUtility.singleLineHeight;

        SerializedProperty skillSO = property.FindPropertyRelative(nameof(WeaponSkillEntry.SkillSO));

        SerializedProperty anim = property.FindPropertyRelative(nameof(WeaponSkillEntry.AnimationName));

        SerializedProperty startup = property.FindPropertyRelative(nameof(WeaponSkillEntry.AttackDuration));

        EditorGUI.PropertyField(rect, skillSO);
        rect.y += EditorGUIUtility.singleLineHeight + 2;

        EditorGUI.PropertyField(rect, anim);
        rect.y += EditorGUIUtility.singleLineHeight + 2;

        SkillBaseSO so = skillSO.objectReferenceValue as SkillBaseSO;

        if (so != null && so.Skill is SkillAttack)
        {
            EditorGUI.PropertyField(rect, startup);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty skillSO =
            property.FindPropertyRelative(nameof(WeaponSkillEntry.SkillSO));

        SkillBaseSO so = skillSO.objectReferenceValue as SkillBaseSO;

        int lines = 2;

        if (so != null && so.Skill is SkillAttack)
        {
            lines = 3;
        }

        return lines * (EditorGUIUtility.singleLineHeight + 2);
    }
}