using UnityEditor;
using UnityEngine;
using Nomlas.UdonPortal;
using TMPro; // AutoSetterが存在する名前空間

[CustomEditor(typeof(AutoSetter))]
public class AutoSetterEditor : Editor
{
    // 対象となるSerializedPropertyを取得
    private SerializedProperty controlPanel;
    private SerializedProperty worldId;
    private SerializedProperty instanceId;
    private SerializedProperty userOrGroup;
    private SerializedProperty instanceType;
    private SerializedProperty userId;
    private SerializedProperty groupType;
    private SerializedProperty groupId;
    private SerializedProperty region;

    private void OnEnable()
    {
        // SerializedObjectから各変数を取得（変数名はAutoSetter内のフィールド名と一致させる）
        controlPanel = serializedObject.FindProperty("controlPanel");
        worldId = serializedObject.FindProperty("worldId");
        instanceId = serializedObject.FindProperty("instanceId");
        userOrGroup = serializedObject.FindProperty("userOrGroup");
        instanceType = serializedObject.FindProperty("instanceType");
        userId = serializedObject.FindProperty("userId");
        groupType = serializedObject.FindProperty("groupType");
        groupId = serializedObject.FindProperty("groupId");
        region = serializedObject.FindProperty("region");
    }

    public override void OnInspectorGUI()
    {
        // 変更があればserializedObjectに反映するため更新
        serializedObject.Update();

        // 各プロパティを表示
        EditorGUILayout.LabelField("AutoSetter Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(controlPanel, new GUIContent("Control Panel"));
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(worldId, new GUIContent("World ID"));
        EditorGUILayout.PropertyField(instanceId, new GUIContent("Instance ID"));
        EditorGUILayout.PropertyField(userOrGroup, new GUIContent("User Or Group"));
        if (userOrGroup.enumValueIndex == (int)UserOrGroup.User)
        {
            EditorGUILayout.PropertyField(instanceType, new GUIContent("Instance Type"));
            if (instanceType.enumValueIndex != (int)InstanceType.Public)
            {
                EditorGUILayout.PropertyField(userId, new GUIContent("User ID"));
            }
        }
        else if (userOrGroup.enumValueIndex == (int)UserOrGroup.Group)
        {
            EditorGUILayout.PropertyField(groupType, new GUIContent("Group Type"));
            EditorGUILayout.PropertyField(groupId, new GUIContent("Group ID"));
        }
        else
        {
            EditorGUILayout.HelpBox("不正です。", MessageType.Error);
        }
        EditorGUILayout.PropertyField(region, new GUIContent("Region"));

        // 編集内容をプロパティへ反映
        serializedObject.ApplyModifiedProperties();

        AutoSetter autoSetter = (AutoSetter)target;
        TextMeshProUGUI tmpText = autoSetter.GetComponentInChildren<TextMeshProUGUI>();

        EditorGUILayout.Space();
        if (tmpText != null)
        {
            EditorGUI.BeginChangeCheck();
            string newText = EditorGUILayout.TextField("Button Text", tmpText.text);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(tmpText, "Change Button Text");
                tmpText.text = newText;
                EditorUtility.SetDirty(tmpText);
            }
        }
    }
}
