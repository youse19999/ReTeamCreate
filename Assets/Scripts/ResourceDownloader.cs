using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

#if UNITY_EDITOR
using UnityEditor;

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

public class UpdateProgressWindow : EditorWindow
{
    private string apiKey = "";
    private string statusMessage = "準備中...";
    private float progress = 0f;
    private bool isDownloading = false;
    private bool isComplete = false;
    private bool isError = false;
    private Action<string> onComplete;

    // アニメーション用
    private double lastTime;
    private float spinnerAngle = 0f;

    public static void ShowWindow(Action<string> onCompleteCallback)
    {
        var window = GetWindow<UpdateProgressWindow>("リソースアップデート");
        window.minSize = new Vector2(400, 200);
        window.maxSize = new Vector2(400, 200);
        window.onComplete = onCompleteCallback;
        window.isDownloading = false;
        window.isComplete = false;
        window.isError = false;
        window.progress = 0f;
        window.statusMessage = "APIキーを入力してください";
        window.ShowUtility();
    }

    private void OnEnable()
    {
        lastTime = EditorApplication.timeSinceStartup;
        EditorApplication.update += Repaint;
    }

    private void OnDisable()
    {
        EditorApplication.update -= Repaint;
    }

    private void OnGUI()
    {
        double currentTime = EditorApplication.timeSinceStartup;
        float deltaTime = (float)(currentTime - lastTime);
        lastTime = currentTime;

        spinnerAngle -= deltaTime * 300f;
        if (spinnerAngle < 0f) spinnerAngle += 360f;

        // カラーパレット定義 (ダーク・モダン)
        Color bgDark = new Color(0.18f, 0.19f, 0.22f);
        Color cardBg = new Color(0.24f, 0.26f, 0.30f);
        Color accentColor = new Color(0.25f, 0.55f, 0.95f);
        Color textColor = new Color(0.9f, 0.92f, 0.95f);
        Color dimText = new Color(0.6f, 0.65f, 0.7f);

        // 背景描画
        EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), bgDark);

        // スタイル設定
        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 16,
            normal = { textColor = textColor },
            alignment = TextAnchor.MiddleCenter
        };

        GUIStyle subStyle = new GUIStyle(EditorStyles.label)
        {
            fontSize = 11,
            normal = { textColor = dimText },
            alignment = TextAnchor.MiddleCenter
        };

        GUIStyle boxStyle = new GUIStyle();
        boxStyle.normal.background = MakeTex(2, 2, cardBg);
        boxStyle.padding = new RectOffset(15, 15, 15, 15);

        GUILayout.BeginVertical();
        GUILayout.Space(20);

        // ヘッダータイトル
        GUILayout.Label("Resources Updater", titleStyle);
        GUILayout.Space(5);
        GUILayout.Label("Google Driveから最新のリソースを取得します", subStyle);
        GUILayout.Space(15);

        // コンテンツカード領域
        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
        GUILayout.BeginVertical(boxStyle, GUILayout.Width(360), GUILayout.Height(90));

        if (!isDownloading && !isComplete && !isError)
        {
            // APIキー入力画面
            GUILayout.Label("APIキーの入力", EditorStyles.boldLabel);
            GUILayout.Space(5);
            apiKey = EditorGUILayout.PasswordField("API Key", apiKey);
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("キャンセル", GUILayout.Width(80), GUILayout.Height(25)))
            {
                Close();
            }
            if (GUILayout.Button("実行する", GUILayout.Width(100), GUILayout.Height(25)))
            {
                if (!string.IsNullOrEmpty(apiKey))
                {
                    isDownloading = true;
                    onComplete?.Invoke(apiKey);
                }
            }
            GUILayout.EndHorizontal();
        }
        else
        {
            // プログレス＆アニメーション画面
            GUILayout.Space(5);
            GUILayout.Label(statusMessage, new GUIStyle(EditorStyles.label) { normal = { textColor = textColor }, alignment = TextAnchor.MiddleCenter });
            GUILayout.Space(15);

            // カスタムプログレスバー＆ローディング
            Rect rect = GUILayoutUtility.GetRect(330, 16);
            EditorGUI.DrawRect(rect, new Color(0.15f, 0.16f, 0.18f));

            if (progress > 0)
            {
                Rect fillRect = new Rect(rect.x, rect.y, rect.width * Mathf.Clamp01(progress), rect.height);
                EditorGUI.DrawRect(fillRect, accentColor);
            }
            else
            {
                // インフィニティ風アニメーションバー
                float barWidth = 80f;
                float xPos = rect.x + (Mathf.Sin((float)EditorApplication.timeSinceStartup * 4f) * 0.5f + 0.5f) * (rect.width - barWidth);
                EditorGUI.DrawRect(new Rect(xPos, rect.y, barWidth, rect.height), accentColor);
            }

            GUILayout.Space(10);
            if (isComplete)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("閉じる", GUILayout.Width(100), GUILayout.Height(25)))
                {
                    Close();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }

        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    public void SetProgress(float val, string msg)
    {
        progress = val;
        statusMessage = msg;
        Repaint();
    }

    public void SetComplete(string msg)
    {
        isDownloading = false;
        isComplete = true;
        progress = 1f;
        statusMessage = msg;
        Repaint();
    }

    public void SetError(string msg)
    {
        isDownloading = false;
        isError = true;
        statusMessage = msg;
        Repaint();
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}

public class ResourcesUpdater : MonoBehaviour
{
    async void Awake()
    {
        try
        {
            using (UnityWebRequest webRequestApi = UnityWebRequest.Get("https://script.google.com/macros/s/AKfycbzAr0dCGDKkGGyC_JVEzA90k-DBlF44Gq7fPcu1dFRY86xvfkcTxrwz8p4c075aCBxymA/exec"))
            {
                var operation = webRequestApi.SendWebRequest();
                while (!operation.isDone) await Task.Yield();

                if (webRequestApi.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"API通信エラー: {webRequestApi.error}");
                    return;
                }

                string jsonString = UTF8Encoding.UTF8.GetString(webRequestApi.downloadHandler.data);
                List<ItemData> items = JsonConvert.DeserializeObject<List<ItemData>>(jsonString);
                List<ItemData> descendingOrder = items.OrderByDescending(x => x.Timestamp).ToList();

                if (!File.Exists("version.tmp") || descendingOrder.First().Timestamp.ToString() != File.ReadAllText("version.tmp").ToString())
                {
                    UpdateProgressWindow.ShowWindow(async (resultKey) =>
                    {
                        var window = EditorWindow.GetWindow<UpdateProgressWindow>();
                        try
                        {
                            window.SetProgress(0.1f, "URLを解析中...");
                            var id = descendingOrder.First().ResourceUrl.Split("https://drive.google.com/open?id=")[1];
                            string url = $"https://www.googleapis.com/drive/v3/files/{id}?alt=media&key={resultKey}";

                            window.SetProgress(0.2f, "リソースをダウンロード中...");
                            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
                            {
                                var downloadOp = webRequest.SendWebRequest();
                                while (!downloadOp.isDone)
                                {
                                    window.SetProgress(0.2f + webRequest.downloadProgress * 0.4f, $"ダウンロード中... {(webRequest.downloadProgress * 100f):F0}%");
                                    await Task.Yield();
                                }

                                if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                                    webRequest.result == UnityWebRequest.Result.ProtocolError)
                                {
                                    window.SetError($"エラー: {webRequest.error}");
                                    Debug.LogError($"エラーが発生しました: {webRequest.error}");
                                }
                                else
                                {
                                    window.SetProgress(0.6f, "ファイルを展開・検証中...");
                                    byte[] rawData = webRequest.downloadHandler.data;

                                    File.WriteAllText("version.tmp", descendingOrder.First().Timestamp.ToString());
                                    File.WriteAllBytes("lastest.zip", rawData);

                                    string temp_folder = Path.Combine(UnityEngine.Application.dataPath, "temp");
                                    string temp_folder_resource = Path.Combine(UnityEngine.Application.dataPath, "temp/Resources");
                                    string resource_folder = Path.Combine(UnityEngine.Application.dataPath, "Resources");
                                    string bak_folder = Path.Combine(UnityEngine.Application.dataPath, "bak");

                                    if (!Directory.Exists(bak_folder)) Directory.CreateDirectory(bak_folder);
                                    if (!Directory.Exists(temp_folder)) Directory.CreateDirectory(temp_folder);
                                    if (!Directory.Exists(resource_folder)) Directory.CreateDirectory(resource_folder);

                                    ZipFile.ExtractToDirectory("lastest.zip", temp_folder, true);

                                    string[] files = Directory.GetFiles(temp_folder_resource);
                                    int totalFiles = files.Length;
                                    int processed = 0;

                                    foreach (string path in files)
                                    {
                                        if (File.Exists(path))
                                        {
                                            using (SHA256 sha256 = SHA256.Create())
                                            {
                                                string targetPath = Path.Combine(resource_folder, Path.GetFileName(path));
                                                if (!File.Exists(targetPath) || !sha256.ComputeHash(File.ReadAllBytes(path)).SequenceEqual(sha256.ComputeHash(File.ReadAllBytes(targetPath))))
                                                {
                                                    try
                                                    {
                                                        File.Copy(path, targetPath, true);
                                                    }
                                                    catch (Exception) { }
                                                }
                                            }
                                        }
                                        processed++;
                                        window.SetProgress(0.6f + (float)processed / totalFiles * 0.3f, $"ファイルを更新中 ({processed}/{totalFiles})");
                                        await Task.Yield();
                                    }

                                    if (Directory.Exists(temp_folder))
                                    {
                                        Directory.Delete(temp_folder, true);
                                    }

                                    AssetDatabase.Refresh();
                                    window.SetComplete("アップデートが完了しました！");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            window.SetError("エラーが発生しました");
                            Debug.LogError(ex);
                        }
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
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