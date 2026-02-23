using System;
using System.Linq;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(SkillBaseSO))]
public sealed class BaseSkillSOEditor : Editor
{
    // --- Property path constants ---
    private static readonly string SkillPropName = nameof(SkillBaseSO.Skill);
    private static readonly string OffensiveEffectsPropName = nameof(SkillAttack.effects);
    private static readonly string SupportEffectsPropName = nameof(SkillSupport.effects);

    private Type[] cachedOffensiveEffectTypes;
    private Type[] cachedSupportEffectTypes;
    private Type[] cachedSkillTypes;

    private void OnEnable()
    {
        // Cache all concrete SkillOffsensiveEffectBase types
        cachedOffensiveEffectTypes = TypeCache.GetTypesDerivedFrom<SkillOffsensiveEffectBase>()
            .Where(t => !t.IsAbstract && t.IsClass && !t.IsGenericType)
            .ToArray();

        // Cache all concrete SkillSupportEffectBase types
        cachedSupportEffectTypes = TypeCache.GetTypesDerivedFrom<SkillSupportEffectBase>()
            .Where(t => !t.IsAbstract && t.IsClass && !t.IsGenericType)
            .ToArray();

        // Cache all concrete SkillBase types
        cachedSkillTypes = TypeCache.GetTypesDerivedFrom<SkillBase>()
            .Where(t => !t.IsAbstract && t.IsClass && !t.IsGenericType)
            .ToArray();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SkillBaseSO so = (SkillBaseSO)target;
        SerializedProperty skillProp = serializedObject.FindProperty(SkillPropName);

        // Draw SkillBase fields inline
        if (so.Skill != null)
        {
            if (skillProp != null)
                EditorGUILayout.PropertyField(skillProp, true); // draw all subclass fields

            GUILayout.Space(8);

            // Draw effects based on Skill type
            if (so.Skill is SkillAttack)
                DrawOffensiveEffects();
            else if (so.Skill is SkillSupport)
                DrawSupportEffects();

            GUILayout.Space(8);
        }
        else
        {
            EditorGUILayout.HelpBox("No Skill assigned. Click 'Set SkillType' to create one.", MessageType.Info);
        }

        // Set SkillBase type button
        if (GUILayout.Button("Set SkillType"))
            ShowSetSkillBaseMenu();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawOffensiveEffects()
    {
        if (GUILayout.Button("Add Offensive Effect"))
            ShowAddEffectMenu(isSupport: false);
    }

    private void DrawSupportEffects()
    {
        if (GUILayout.Button("Add Support Effect"))
            ShowAddEffectMenu(isSupport: true);
    }

    // --- Skill Effect menu ---
    private void ShowAddEffectMenu(bool isSupport)
    {
        GenericMenu menu = new GenericMenu();
        Type[] types = isSupport ? cachedSupportEffectTypes : cachedOffensiveEffectTypes;

        foreach (Type type in types)
        {
            Type capturedType = type;
            menu.AddItem(new GUIContent(capturedType.Name), false, () => AddEffect(capturedType, isSupport));
        }

        menu.ShowAsContext();
    }

    private void AddEffect(Type type, bool isSupport)
    {
        SkillBaseSO so = (SkillBaseSO)target;
        if (so.Skill == null)
        {
            Debug.LogWarning("Cannot add SkillEffect: SkillBase is null.");
            return;
        }

        Undo.RecordObject(so, "Add SkillEffect");
        serializedObject.Update();

        SerializedProperty skillProp = serializedObject.FindProperty(SkillPropName);
        string propName = isSupport ? SupportEffectsPropName : OffensiveEffectsPropName;
        SerializedProperty effectsProp = skillProp.FindPropertyRelative(propName);

        if (effectsProp == null)
        {
            Debug.LogError("Effects property is null. Make sure SkillBase has [SerializeReference] effects array.");
            return;
        }

        int index = effectsProp.arraySize;
        effectsProp.InsertArrayElementAtIndex(index);

        SerializedProperty element = effectsProp.GetArrayElementAtIndex(index);
        element.managedReferenceValue = Activator.CreateInstance(type);

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(so);
        Repaint();
    }

    // --- SkillBase menu ---
    private void ShowSetSkillBaseMenu()
    {
        GenericMenu menu = new GenericMenu();

        foreach (Type type in cachedSkillTypes)
        {
            Type capturedType = type;
            menu.AddItem(new GUIContent(capturedType.Name), false, () => SetSkillBase(capturedType));
        }

        menu.ShowAsContext();
    }

    private void SetSkillBase(Type type)
    {
        SkillBaseSO so = (SkillBaseSO)target;

        Undo.RecordObject(so, "Set SkillType");

        // Direct assignment works safely for [SerializeReference]
        so.Skill = CreateSkill(type);

        EditorUtility.SetDirty(so);

        serializedObject.Update();
        Repaint();
    }

    private SkillBase CreateSkill(Type type)
    {
        SkillBase skill = (SkillBase)Activator.CreateInstance(type);

        if (skill is SkillAttack attack && attack.effects == null)
            attack.effects = new SkillOffsensiveEffectBase[0];
        else if (skill is SkillSupport support && support.effects == null)
            support.effects = new SkillSupportEffectBase[0];

        return skill;
    }
}