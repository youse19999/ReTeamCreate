using TMPro;
using UnityEngine;

public class PointCanvas : MonoBehaviour
{
    private Animator anim = null;
    public TMP_Text ScoreText1P;
    public TMP_Text ScoreText2P;
    [SerializeField] GoalArea OnePlayer;
    [SerializeField] GoalArea TwoPlayer;

    //1Pと2Pの値を入れ、それをTEXTで表示
    public void Point(int OneP,int TwoP)
    {
        ScoreText1P.text = OneP + "Point";
        ScoreText2P.text = TwoP + "Point";
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        Debug.Log("Start");//デバック用
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(OnePlayer);
        Point(OnePlayer.point, TwoPlayer.point);

        if (OnePlayer.point > TwoPlayer.point) { OnePlayerWin(); }
        else if (OnePlayer.point > TwoPlayer.point) { TwoPlayerWin(); }
        else { Draw(); }
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
