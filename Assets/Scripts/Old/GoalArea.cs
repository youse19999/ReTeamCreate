using UnityEngine;

public class GoalArea : MonoBehaviour
{
    [SerializeField] public GameObject targetPlayer;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip goalSE;
    [SerializeField] private ParticleSystem goalEffect;
    //プレイヤーの得点
    [SerializeField] public int point = 0;
    private string playerName;
    //ここにアイテムtagの名前を書く
    private string pointTargetTag;
    bool seted = false;
    ItemSpawnManager spawnManager;
    GamePlayer gamePlayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerName = targetPlayer.name;
    }
    public bool Seted(GameObject obj)
    {
        if (!seted)
        {
            this.targetPlayer = obj;
            seted = true;
        }
        else
        {
            return true;
        }
        return false;
    }
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject != targetPlayer){return; }
        GamePlayer gamePlayer = targetPlayer.GetComponent<GamePlayer>();

        // プレイヤーの所持アイテムを取得して手放す
        GameObject item = gamePlayer.DropItem();

        if (item == null) { return; }

        ItemScript itemScript = item.GetComponent<ItemScript>();
        point += itemScript.point;

        audioSource.PlayOneShot(goalSE);
        goalEffect.Play();

        Destroy(item);

        Debug.Log($"{playerName}のポイント:{point}");
    }
}
