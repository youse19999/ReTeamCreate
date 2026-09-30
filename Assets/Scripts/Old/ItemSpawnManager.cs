using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ItemSpawnManager : MonoBehaviour
{
    //マップ内に存在できる最大アイテム数
    [SerializeField] private int maxSpawnAmount = 3;
    // スポーン範囲
    [SerializeField] private BoxCollider spawnArea;
    [SerializeField] private float spawnInterval = 10.0f;
    [SerializeField] private CanvasScriptableObject canvasScriptableObject;
    [SerializeField] private float minSpawnDistance = 2.0f;
    [SerializeField] private LayerMask itemLayerMask;
    [SerializeField] private float navMeshSearchDistance = 2.0f;
    [SerializeField] private int maxSpawnAttempts = 20;
    public static int currentSpawnAmount;
    public List<GameObject> ItemList;
    // スペシャルアイテム
    public List<GameObject> SpecialItemList;
    // ゲーム開始からの経過時間
    private float specialItemTime = 0.0f;
    private float spawnTimer = 0.0f;

    void Start()
    {
        currentSpawnAmount = 0;
        // 最初のアイテムをすぐ出せるようにする
        spawnTimer = spawnInterval;

        SpawnItem();
    }


    void Update()
    {
        ItemSpawn();
    }

    void ItemSpawn()
    {
        spawnTimer += Time.deltaTime;
        specialItemTime += Time.deltaTime;

        // アイテムが0個ならすぐスポーン可能にする
        if (currentSpawnAmount == 0)
        {
            spawnTimer = spawnInterval;
        }

        SpawnItem();
    }

    void SpawnItem()
    {
        if (spawnArea == null)
        {
            Debug.LogError("SpawnAreaが設定されていません");
            return;
        }

        // スポーン間隔になっていない
        if (spawnTimer < spawnInterval)
        {
            return;
        }

        // NavMesh上かつ他アイテムと重ならない場所を探す
        if (!TryGetSpawnPosition(out Vector3 spawnPosition))
        {
            Debug.LogWarning("スポーン可能な位置が見つかりませんでした");
            return;
        }

        if (currentSpawnAmount < maxSpawnAmount)
        {
            if (ItemList == null || ItemList.Count == 0){Debug.LogError("ItemListが設定されていません");return;}

            int randomItemNumber = Random.Range(0, ItemList.Count);

            SpawnItemObject(ItemList[randomItemNumber], spawnPosition);


            currentSpawnAmount++;

            spawnTimer = 0.0f;

            return;
        }



        // 制限時間の半分を経過したら出現可能
        if (specialItemTime >= canvasScriptableObject.TimeLimit / 2.0f)
        {
            // 通常最大数より少し多く出現可能
            int specialMaxAmount = Mathf.CeilToInt(maxSpawnAmount * 1.3f);

            if (currentSpawnAmount < specialMaxAmount)
            {
                if (SpecialItemList == null || SpecialItemList.Count == 0){Debug.LogError("SpecialItemListが設定されていません");return;}

                int randomItemNumber = Random.Range(0, SpecialItemList.Count);

                SpawnItemObject(SpecialItemList[randomItemNumber], spawnPosition);

                currentSpawnAmount++;

                spawnTimer = 0.0f;
            }
        }
    }

    // アイテムを生成し、Colliderの底面を床の高さに合わせる
    void SpawnItemObject(GameObject prefab, Vector3 spawnPosition)
    {
        GameObject spawnedItem = Instantiate(prefab,spawnPosition,Quaternion.identity);

        Collider itemCollider = spawnedItem.GetComponentInChildren<Collider>();

        if (itemCollider == null){return;}

        // Colliderの底面がNavMesh上に来るように補正
        float offset = spawnPosition.y - itemCollider.bounds.min.y;

        spawnedItem.transform.position += Vector3.up * (offset + 0.02f);
    }

    bool TryGetSpawnPosition(out Vector3 spawnPosition)
    {
        Bounds bounds = spawnArea.bounds;

        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            // SpawnArea内でランダム位置を作成
            float randomX = Random.Range(bounds.min.x, bounds.max.x);

            float randomZ = Random.Range(bounds.min.z, bounds.max.z);

            Vector3 randomPosition = new Vector3(randomX,bounds.center.y,randomZ);

            if (!NavMesh.SamplePosition(randomPosition,out NavMeshHit navHit,navMeshSearchDistance,NavMesh.AllAreas))
            {
                continue;
            }

            bool itemExists = Physics.CheckSphere(navHit.position,minSpawnDistance,itemLayerMask,QueryTriggerInteraction.Collide);

            if (itemExists)
            {
                continue;
            }
            spawnPosition = navHit.position;

            return true;
        }
        spawnPosition = Vector3.zero;

        return false;
    }

    void OnDrawGizmosSelected()
    {
        if (spawnArea == null){return;}

        Gizmos.DrawWireCube(spawnArea.bounds.center,spawnArea.bounds.size);
    }
}