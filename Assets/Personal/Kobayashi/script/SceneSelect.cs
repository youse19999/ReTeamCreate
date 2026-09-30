using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;

[CustomPropertyDrawer(typeof(SceneFolderAttribute))]
public class SceneSelect : PropertyDrawer
{
    public override void OnGUI(
        Rect position,
        SerializedProperty property,
        GUIContent label)
    {
        EditorGUI.PropertyField(
            position,
            property,
            label
        );

        //フォルダが指定されてなければ終了
        if(property.objectReferenceValue==null)
        {
            return;
        }

        // 指定されたフォルダのパスを取得
        string folderPath = AssetDatabase.GetAssetPath(
            property.objectReferenceValue
        );

        // Sceneを検索
        string[] guids = AssetDatabase.FindAssets("Assets/Scenes", new[] { folderPath });

        // Sceneを1つずつ表示
        foreach (string guid in guids)
        {
            string scenePath =
                AssetDatabase.GUIDToAssetPath(guid);

            string sceneName =
                Path.GetFileNameWithoutExtension(scenePath);

            // Sceneボタンを表示
            if (GUILayout.Button(sceneName))
            {
                OpenScene(scenePath);
            }
        }
    }
    private void OpenScene(string scenePath)
    {
        EditorSceneManager.OpenScene(scenePath);
    }
}