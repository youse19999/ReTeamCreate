using UnityEngine;

public class ItemScript : MonoBehaviour
{
    public int point;//1,2,3,10
    GamePlayer gamePlayer;

    //疑似的なアニメーション
    [SerializeField] float amplitude = 0.3f; // 上下幅
    [SerializeField] float speed = 2f;        // 揺れる速さ

    public bool having = false;

    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            if(col.gameObject.GetComponent<GamePlayer>().HasItem())
            {
                return;
            }
            col.gameObject.GetComponent<GamePlayer>().ShowItem(this.gameObject);
            having = true;
            ItemSpawnManager.currentSpawnAmount--;
        }
    }

    private void Update()
    {
        if (!having)
        {
            //ふわふわ動く浮遊感
            transform.localPosition = startPos + Vector3.up * Mathf.Sin(Time.time * speed) * amplitude;
        }
        else
        {
            transform.localPosition = Vector3.zero;
        }
    }
}
