using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;





#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

[System.Serializable]
public class ItemData
{
    [JsonProperty("Timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonProperty("Column 1")]
    public DateTime Column1 { get; set; }

    [JsonProperty("Column 2")]
    public DateTime Column2 { get; set; }

    [JsonProperty("名前")]
    public string Name { get; set; }

    [JsonProperty("リソース")]
    public string ResourceUrl { get; set; }
}
public class InputDialogWindow : EditorWindow
{
    private string inputText = "";
    private System.Action<string> onComplete;

    public static void ShowDialog(string title, System.Action<string> onClickOK)
    {
        InputDialogWindow window = CreateInstance<InputDialogWindow>();
        window.titleContent = new GUIContent(title);
        window.onComplete = onClickOK;
        window.ShowUtility();
    }

    void OnGUI()
    {
        GUILayout.Label("APIキーの入力をしてください。", EditorStyles.boldLabel);
        inputText = GUILayout.TextField(inputText);

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("OK"))
        {
            onComplete?.Invoke(inputText);
            Close();
        }
        if (GUILayout.Button("キャンセル"))
        {
            Close();
        }
        GUILayout.EndHorizontal();
    }
}
public class ResourcesUpdater:MonoBehaviour
{
    async void Awake()
    {
            using (UnityWebRequest webRequestApi = UnityWebRequest.Get("https://script.google.com/macros/s/AKfycbzAr0dCGDKkGGyC_JVEzA90k-DBlF44Gq7fPcu1dFRY86xvfkcTxrwz8p4c075aCBxymA/exec"))
            {
                await webRequestApi.SendWebRequest();
                 string jsonString = UTF8Encoding.UTF8.GetString(webRequestApi.downloadHandler.data);

                List<ItemData> items = JsonConvert.DeserializeObject<List<ItemData>>(jsonString);
                List<ItemData> descendingOrder = items.OrderByDescending(x => x.Timestamp).ToList();
                // データの確認用ログList<ItemData> ascendingOrder = items.OrderBy(x => x.Timestamp).ToList();
                if (!File.Exists("version.tmp") || descendingOrder.First().Timestamp.ToString() != File.ReadAllText("version.tmp").ToString())
                {
                    InputDialogWindow.ShowDialog("APIキーの入力", async (result) =>
                    {
                    var id = descendingOrder.First().ResourceUrl.Split("https://drive.google.com/open?id=")[1];
                    Debug.Log(id);
                    string url = $"https://www.googleapis.com/drive/v3/files/{id}?alt=media&key={result}";

                    Debug.Log(url);
                    using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
                    {
                        // リクエスト送信
                        await webRequest.SendWebRequest();

                        if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                            webRequest.result == UnityWebRequest.Result.ProtocolError)
                        {
                            Debug.LogError($"エラーが発生しました: {webRequest.error}");
                            Debug.LogError($"レスポンスコード: {webRequest.responseCode}");
                        }
                        else
                        {
                            Debug.Log("ダウンロード成功！");

                            byte[] rawData = webRequest.downloadHandler.data;

                            File.WriteAllText("version.tmp", descendingOrder.First().Timestamp.ToString());

                            File.WriteAllBytes("lastest.zip", rawData);

                            string temp_folder = Path.Combine(UnityEngine.Application.dataPath, "temp");
                            string temp_folder_resource = Path.Combine(UnityEngine.Application.dataPath, "temp/Resources");
                            string resource_folder = Path.Combine(UnityEngine.Application.dataPath, "Resources");
                            Debug.Log(temp_folder);
                            string bak_folder = Path.Combine(UnityEngine.Application.dataPath, "bak");

                            if (!Directory.Exists(bak_folder))
                            {
                                Directory.CreateDirectory(bak_folder);
                            }
                            if (!Directory.Exists(temp_folder))
                            {
                                Directory.CreateDirectory(temp_folder);
                            }

                                if (!Directory.Exists(resource_folder))
                                {
                                    Directory.CreateDirectory(resource_folder);
                                }
                                ZipFile.ExtractToDirectory("lastest.zip", temp_folder,true);
                            Debug.Log(Directory.GetFiles(temp_folder).Length);
                            foreach (string path in Directory.GetFiles(temp_folder_resource))
                            {
                                if (File.Exists(path))
                                {
                                    SHA256 sha256 = SHA256.Create();
                                    if (File.Exists(Path.Combine(resource_folder, Path.GetFileName(path))) == false || sha256.ComputeHash(File.ReadAllBytes(path)).SequenceEqual(sha256.ComputeHash(File.ReadAllBytes(Path.Combine(resource_folder, Path.GetFileName(path))))) == false)
                                    {
                                        try
                                        {
                                            Debug.Log(path + "が更新されました");
                                            File.Copy(path, Path.Combine(resource_folder, Path.GetFileName(path)), true);
                                        }
                                        catch (Exception ex)
                                        {
                                        }
                                    }
                                }
                            }
                            Directory.Delete(temp_folder, true);
                            Debug.Log("");
                        }
                    }
                });
            }
        }
    }
}  
public static class GameInitializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        GameObject autoManager = new GameObject("Automated_Manager");

        autoManager.AddComponent<ResourcesUpdater>();
    }
}
#endif