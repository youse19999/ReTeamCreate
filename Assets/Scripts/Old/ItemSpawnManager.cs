using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ItemSpawnManager : MonoBehaviour
{
    //マップ内に存在できる最大アイテム数
    [SerializeField] private int maxSpawnAmount = 3;

    //スポーン範囲
    [SerializeField] private BoxCollider spawnArea;
    [SerializeField] private float spawnInterval = 10.0f;
    [SerializeField] private float minSpawnDistance = 2.0f;
    [SerializeField] private LayerMask itemLayerMask;
    [SerializeField] private float navMeshSearchDistance = 2.0f;
    [SerializeField] private int maxSpawnAttempts = 20;

    //ゲーム時間
    [SerializeField] private TimeCanvas timeCanvas;

    //スポーン演出
    [SerializeField] private GameObject spawnEffectPrefab;
    [SerializeField] private float spawnEffectDelay = 1.0f;

    //SE
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip spawnSE;

    public static int currentSpawnAmount;

    public List<GameObject> ItemList;

    //スペシャルアイテム
    public List<GameObject> SpecialItemList;

    private float spawnTimer = 0.0f;
    private float initialTimeLimit = 0.0f;

    private bool isSpawning = false;
    private bool isSpecialItemSpawned = false;

    void Start()
    {
        currentSpawnAmount = 0;
        spawnTimer = spawnInterval;

        if (timeCanvas != null)
        {
            initialTimeLimit = timeCanvas.timelimit;
        }

        SpawnItem();
    }

    void Update()
    {
        ItemSpawn();
    }

    void ItemSpawn()
    {
        spawnTimer += Time.deltaTime;

        if (currentSpawnAmount == 0)
        {
            spawnTimer = spawnInterval;
        }

        SpawnSpecialItem();
        SpawnItem();
    }

    void SpawnItem()
    {
        if (spawnArea == null) { Debug.LogError("SpawnAreaが設定されていません"); return; }
        if (isSpawning) { return; }
        if (spawnTimer < spawnInterval) { return; }
        if (currentSpawnAmount >= maxSpawnAmount) { return; }
        if (ItemList == null || ItemList.Count == 0) { Debug.LogError("ItemListが設定されていません"); return; }

        if (!TryGetSpawnPosition(out Vector3 spawnPosition)) { Debug.LogWarning("通常アイテムのスポーン位置が見つかりませんでした"); return; }

        int randomItemNumber = Random.Range(0, ItemList.Count);

        StartCoroutine(SpawnItemWithEffect(ItemList[randomItemNumber], spawnPosition, true));

        spawnTimer = 0.0f;
    }

    void SpawnSpecialItem()
    {
        if (isSpecialItemSpawned) { return; }
        if (timeCanvas == null) { Debug.LogError("TimeCanvasが設定されていません"); return; }
        if (initialTimeLimit <= 0.0f) { return; }
        if (timeCanvas.timelimit > initialTimeLimit / 3.0f) { return; }
        if (spawnArea == null) { Debug.LogError("SpawnAreaが設定されていません"); return; }
        if (isSpawning) { return; }
        if (SpecialItemList == null || SpecialItemList.Count == 0) { Debug.LogError("SpecialItemListが設定されていません"); return; }

        if (!TryGetSpawnPosition(out Vector3 spawnPosition)) { Debug.LogWarning("スペシャルアイテムのスポーン位置が見つかりませんでした"); return; }

        int randomItemNumber = Random.Range(0, SpecialItemList.Count);

        StartCoroutine(SpawnItemWithEffect(SpecialItemList[randomItemNumber], spawnPosition, false));

        isSpecialItemSpawned = true;
    }

    //スポーン演出後にアイテムを生成
    IEnumerator SpawnItemWithEffect(GameObject prefab, Vector3 spawnPosition, bool countAsNormalItem)
    {
        isSpawning = true;

        if (spawnEffectPrefab != null)
        {
            GameObject effect = Instantiate(spawnEffectPrefab, spawnPosition + Vector3.up * 0.05f, Quaternion.identity);

            ParticleSystem particle = effect.GetComponentInChildren<ParticleSystem>();

            if (particle != null)
            {
                particle.Play();
            }
            else
            {
                Debug.LogWarning("SpawnEffectにParticleSystemがありません");
            }

            Destroy(effect, spawnEffectDelay + 2.0f);
        }
        else
        {
            Debug.LogWarning("SpawnEffectPrefabが設定されていません");
        }

        yield return new WaitForSeconds(spawnEffectDelay);

        SpawnItemObject(prefab, spawnPosition);

        if (countAsNormalItem)
        {
            currentSpawnAmount++;
        }

        isSpawning = false;
    }

    //アイテムを生成し、Colliderの底面を地面の高さに合わせる
    void SpawnItemObject(GameObject prefab, Vector3 spawnPosition)
    {
        GameObject spawnedItem = Instantiate(prefab, spawnPosition, Quaternion.identity);

        Renderer[] renderers = spawnedItem.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0) { return; }

        Bounds itemBounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; ++i)
        {
            itemBounds.Encapsulate(renderers[i].bounds);
        }

        float offset = spawnPosition.y - itemBounds.min.y;

        spawnedItem.transform.position += Vector3.up * (offset + 0.02f);

        audioSource.PlayOneShot(spawnSE);
    }

    bool TryGetSpawnPosition(out Vector3 spawnPosition)
    {
        Bounds bounds = spawnArea.bounds;

        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomZ = Random.Range(bounds.min.z, bounds.max.z);

            Vector3 randomPosition = new Vector3(randomX, bounds.center.y, randomZ);

            if (!NavMesh.SamplePosition(randomPosition, out NavMeshHit navHit, navMeshSearchDistance, NavMesh.AllAreas))
            {
                continue;
            }

            bool itemExists = Physics.CheckSphere(navHit.position, minSpawnDistance, itemLayerMask, QueryTriggerInteraction.Collide);

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
        if (spawnArea == null) { return; }

        Gizmos.DrawWireCube(spawnArea.bounds.center, spawnArea.bounds.size);
    }
}