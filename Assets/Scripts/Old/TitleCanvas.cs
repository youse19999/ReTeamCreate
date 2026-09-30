using UnityEngine;

public class TitleCanvas : MonoBehaviour
{
    [SerializeField] private CanvasScriptableObject canvasParameter;
    [SerializeField] public static bool StopTitle;//Title‚Ì‘JˆÚ
    
    void Start()
    {
        StopTitle = false;
    }    void Update()
    {
        if (transform.localPosition.y <= 90) { 
            StopTitle = true;//‘JˆÚI—¹
        }

        //ˆÚ“®ˆ—
        if (StopTitle) { return; }
        transform.localPosition += new Vector3(0, -canvasParameter.TitleMoveSpeed * Time.deltaTime, 0);
    }
}
