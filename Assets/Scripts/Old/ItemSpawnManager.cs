using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnManager : MonoBehaviour
{

    [SerializeField] private int maxSpawnAmount = 3;//マップ内に存在できるアイテムの数
    public static int currentSpawnAmount;

    [SerializeField] BoxCollider spawnArea;

    public List<GameObject> ItemList;//ノーマルアイテム
    public List<GameObject> SpecialItemList;//スペシャルアイテム

    [SerializeField] private float spawnInterval = 10.0f;//スポーン感覚
    [SerializeField] private float specialItemTime = 30.0f;//逆転アイテム出現時間
    private float spawnTimer;

    [SerializeField] private CanvasScriptableObject canvasScriptableObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentSpawnAmount = 0;
        SpawnItem();
    }

    // Update is called once per frame
    void Update()
    {
        ItemSpawn();

    }

    //スポーンの条件
    void ItemSpawn()
    { 

        //マップ内のアイテム０ならスポーン
        if (currentSpawnAmount == 0)
        {
            SpawnItem();
        }
        //等間隔でSpawnAreaのランダムな位置にスポーン
        spawnTimer += Time.deltaTime;
        specialItemTime += Time.deltaTime;
        SpawnItem();
        
        //制限時間が一定以下になると10pアイテムのスポーンなど追加予定：未実装
    }

    //スポーン処理
    void SpawnItem()
    {
        if(spawnArea == null) { Debug.LogError("SpawnAreaがありません"); return; }
        //スポーンエリア内のランダムな位置取得
        Bounds bounds = spawnArea.bounds;

        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float y = bounds.center.y;//Ｙの値は固定
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);

        Vector3 randomPosition = new Vector3(randomX, y, randomZ);


        //通常アイテムのスポーン
        if (spawnTimer > spawnInterval)
        {
            if (spawnTimer >= spawnInterval && currentSpawnAmount < maxSpawnAmount)
            {
                if (ItemList == null || ItemList.Count == 0) { Debug.LogError("ItemListに不足があります"); return; }

                int randamItemNumber = Random.Range(0, ItemList.Count);
                Instantiate(ItemList[randamItemNumber], randomPosition, Quaternion.identity);
                //マップ内のアイテム数加算
                currentSpawnAmount++;
                spawnTimer = 0.0f;
            }
        }
        //逆転アイテムのスポーン
        if(canvasScriptableObject.TimeLimit/2 < specialItemTime)
        {
            if (spawnTimer >= spawnInterval && currentSpawnAmount < maxSpawnAmount*1.3f)
            {
                if (SpecialItemList == null || SpecialItemList.Count == 0) { Debug.LogError("SpecialItemListに不足があります"); return; }

                int randamItemNumber = Random.Range(0, SpecialItemList.Count);
                Instantiate(SpecialItemList[randamItemNumber], randomPosition, Quaternion.identity);
                //マップ内のアイテム数加算
                currentSpawnAmount++;
                spawnTimer = 0.0f;
            }
        }

         


    }

}
