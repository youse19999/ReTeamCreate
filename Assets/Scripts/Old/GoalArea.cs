using UnityEngine;

public class GoalArea : MonoBehaviour
{
    bool seted = false;
    [SerializeField] public GameObject targetPlayer;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip goalSE;
    [SerializeField] private ParticleSystem goalEffect;
    [SerializeField] public int point = 0;//プレイヤーの得点
    private string playerName;
    private string pointTargetTag;//ここにアイテムtagの名前を書く
    ItemSpawnManager spawnManager;
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
    private void OnTriggerEnter(Collider collider)
    {
        //エリアに侵入したアイテムの固有ポイントを取得、その分ポイント加算
     
        if (collider.gameObject == targetPlayer)
        {
            //ターゲットプレイヤの所持しているアイテムを取得
            Transform ItemPosition = targetPlayer.transform.Find("ItemPosition");

            //アイテムのポイント加算と手持ちのアイテムDestory
            foreach(Transform child in ItemPosition)
            {
                //ItemPositionの子オブジェクト、スクリプトを取得、このPointに加算する
                GameObject Item = child.gameObject;
                ItemScript itemScript = Item.GetComponent<ItemScript>();
                this.point += itemScript.point;
                // SE再生
                audioSource.PlayOneShot(goalSE);
                // エフェクト再生
                goalEffect.Play();

                //加算を終えたらDestory
                Destroy(Item);

                //マップ内に存在するアイテム数を減少
                ItemSpawnManager.currentSpawnAmount--;

            }
            //デバッグ
            Debug.Log($"{playerName}のポイント:{point}");
        }
    }
}
