using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class TimeCanvas : MonoBehaviour
{
    private Animator anim = null;
    [SerializeField] bool Finish;
    [SerializeField] public float timelimit;

    [SerializeField] private Sprite[] numberSprites;

    [Header("--- Parameterの設定 ---")]
    [SerializeField] private CanvasScriptableObject canvasParameter;

    [Header("--- Time用の各桁のImageの設定 ---")]
    [SerializeField] private Image[] TimeDigitImages;


    //時間制限
    public void TimeA(int time)
    {
        if (Finish)
        {
            //時間を０に固定する
            timelimit = 0;
        }
        else
        {
            Debug.Log("時間が減っています");

            timelimit-=Time.deltaTime;

            //TimeOutになったら時間を止める
            if ((int)timelimit >= 1) { return; }
            anim.SetBool("Finish", true);
            Finish = true;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        Finish = false;
        timelimit=canvasParameter.TimeLimit;
    }

    // Update is called once per frame
    void Update()
    {
        TimeA((int)timelimit);

        UpdatePlayerScoreDisplay((int)timelimit, TimeDigitImages);
    }

    private void UpdatePlayerScoreDisplay(int time, Image[] digitImages)
    {
        // 桁数の画像が登録されていない場合はスキップ
        if (digitImages == null || digitImages.Length == 0) return;

        // 3桁（0～999）の範囲に収める
        time = Mathf.Clamp(time, 0, 999);

        // 点数を4桁の文字列に変換（例: 5点 ➔ "005"）
        string scoreText = time.ToString("D3");

        // 各桁の画像を表示された文字に合わせて切り替える
        for (int i = 0; i < digitImages.Length; i++)
        {
            // 安全対策：文字数とImageの数が合わない場合のクラッシュ防止
            if (i >= scoreText.Length) break;

            char digitChar = scoreText[i];
            int spriteIndex = digitChar - '0'; // 文字を数値に変換

            if (digitImages[i] != null)
            {
                digitImages[i].sprite = numberSprites[spriteIndex];
            }
        }
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene("Title");
    }
}
