using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PointCanvas : MonoBehaviour
{
    private Animator anim = null;
    [SerializeField] private Sprite[] numberSprites;


    [Header("--- 1Player（OnePlayer）の設定 ---")]
    [SerializeField] private GoalArea OnePlayerGoalArea;
    [SerializeField] private Image[] onePlayerDigitImages; // 1P用の各桁のImage

    [Header("--- 2Player（TwoPlayer）の設定 ---")]
    [SerializeField] private GoalArea TwoPlayerGoalArea;
    [SerializeField] private Image[] twoPlayerDigitImages; // 2P用の各桁のImage

    private int lastOnePlayerScore = -1;
    private int lastTwoPlayerScore = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        Debug.Log("Start");//デバック用
    }

    void Update()
    {
        // --- 1Playerの更新処理 ---
        if (OnePlayerGoalArea != null)
        {
            int current1PScore = OnePlayerGoalArea.point;
            if (current1PScore != lastOnePlayerScore)
            {
                lastOnePlayerScore = current1PScore;
                UpdatePlayerScoreDisplay(current1PScore, onePlayerDigitImages);
            }
        }

        // --- 2Playerの更新処理 ---
        if (TwoPlayerGoalArea != null)
        {
            int current2PScore = TwoPlayerGoalArea.point;
            if (current2PScore != lastTwoPlayerScore)
            {
                lastTwoPlayerScore = current2PScore;
                UpdatePlayerScoreDisplay(current2PScore, twoPlayerDigitImages);
            }
        }

        Debug.Log(OnePlayerGoalArea);

        if (OnePlayerGoalArea.point > TwoPlayerGoalArea.point) { OnePlayerWin(); }
        else if (OnePlayerGoalArea.point > TwoPlayerGoalArea.point) { TwoPlayerWin(); }
        else { Draw(); }
    }

    // 指定されたプレイヤーのスコア画像を更新する共通メソッド
    private void UpdatePlayerScoreDisplay(int score, Image[] digitImages)
    {
        // 桁数の画像が登録されていない場合はスキップ
        if (digitImages == null || digitImages.Length == 0) return;

        // 3桁（0～999）の範囲に収める
        score = Mathf.Clamp(score, 0, 999);

        // 点数を4桁の文字列に変換（例: 5点 ➔ "005"）
        string scoreText = score.ToString("D3");

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

    void OnePlayerWin()
    {
        anim.SetBool("OnePWin", true);
        anim.SetBool("TwoPWin", false);
    }

    void TwoPlayerWin()
    {
        anim.SetBool("OnePWin", false);
        anim.SetBool("TwoPWin", true);
    }

    void Draw()
    {
        anim.SetBool("OnePWin", false);
        anim.SetBool("TwoPWin", false);
    }

}
