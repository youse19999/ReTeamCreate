using UnityEngine;

public class ItemScript : MonoBehaviour
{
    public int point;//1,2,3,10
    GamePlayer gamePlayer;

    //疑似的なアニメーション
    [SerializeField] float amplitude = 0.1f; 
    [SerializeField] float speed = 2f;       

    public bool having = false;

    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (having) {return;}
        if (!col.CompareTag("Player")){ return;}

        GamePlayer gamePlayer = col.GetComponent<GamePlayer>();

        if (gamePlayer == null) { return; }
        if (gamePlayer.HasItem()) { return; }

        having = true;

        gamePlayer.ShowItem(gameObject);

        Destroy(gameObject);

        ItemSpawnManager.currentSpawnAmount--;
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
