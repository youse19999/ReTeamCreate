using UnityEngine;
using UnityEngine.UI;

public class ItemScript : MonoBehaviour
{
    public int point; //1,2,3,10
    NewPlayerController gamePlayer;

    [SerializeField] float amplitude = 0.1f;
    [SerializeField] float speed = 2f;

    public bool having = false;

    Vector3 startPos;

    [Header("ポイント数字")]
    [SerializeField] Image digit1;
    [SerializeField] Image digit2;

    [SerializeField] Sprite sprite0;
    [SerializeField] Sprite sprite1;
    [SerializeField] Sprite sprite2;
    [SerializeField] Sprite sprite3;

    [Header("ポイント表示")]
    [SerializeField] Transform pointCanvas;
    [SerializeField, Range(0.1f, 1.0f)] float pointSizeRatio = 0.8f;
    [SerializeField] float surfaceOffset = 0.02f;

    [SerializeField] private AudioClip getSE;

    Camera mainCamera;
    Bounds itemBounds;
    private void OnTriggerEnter(Collider col)
    {
        if (having) { return; }
        if (!col.CompareTag("Player")) { return; }

        NewPlayerController gamePlayer = col.GetComponent<NewPlayerController>();

        if (gamePlayer == null) { return; }
        if (gamePlayer.HasItem()) { return; }

        having = true;

        gamePlayer.ShowItem(gameObject);

        AudioSource.PlayClipAtPoint(getSE, transform.position);

        Destroy(gameObject);

        ItemSpawnManager.currentSpawnAmount--;
    }

    void Start()
    {
        startPos = transform.position;
        mainCamera = Camera.main;

        UpdatePointImage();
        UpdateItemBounds();
        UpdatePointScale();
    }

    void UpdatePointImage()
    {
        digit2.gameObject.SetActive(false);

        RectTransform digit1Rect = digit1.rectTransform;
        RectTransform digit2Rect = digit2.rectTransform;

        digit1Rect.anchoredPosition = Vector2.zero;

        switch (point)
        {
            case 1:
                digit1.sprite = sprite1;
                break;

            case 2:
                digit1.sprite = sprite2;
                break;

            case 3:
                digit1.sprite = sprite3;
                break;

            case 10:
                digit1.sprite = sprite1;
                digit2.sprite = sprite0;

                digit1Rect.anchoredPosition = new Vector2(-50.0f, 0.0f);
                digit2Rect.anchoredPosition = new Vector2(50.0f, 0.0f);

                digit2.gameObject.SetActive(true);
                break;
        }
    }

    void UpdateItemBounds()
    {
        Renderer[] itemRenderers = GetComponentsInChildren<Renderer>();

        if (itemRenderers.Length == 0) { return; }

        itemBounds = itemRenderers[0].bounds;

        for (int i = 1; i < itemRenderers.Length; ++i)
        {
            itemBounds.Encapsulate(itemRenderers[i].bounds);
        }
    }

    void UpdatePointScale()
    {
        if (pointCanvas == null) { return; }

        RectTransform pointRect = pointCanvas.GetComponent<RectTransform>();
        if (pointRect == null) { return; }

        float canvasWidth = pointRect.rect.width;
        float canvasHeight = pointRect.rect.height;
        float canvasSize = Mathf.Max(canvasWidth, canvasHeight);

        if (canvasSize <= 0.0f) { return; }

        float itemSize = Mathf.Min(itemBounds.size.x, itemBounds.size.y);
        float targetSize = itemSize * pointSizeRatio;

        float parentScale = 1.0f;

        if (pointCanvas.parent != null)
        {
            Vector3 parentLossyScale = pointCanvas.parent.lossyScale;
            parentScale = Mathf.Max(Mathf.Abs(parentLossyScale.x), Mathf.Abs(parentLossyScale.y), Mathf.Abs(parentLossyScale.z));
        }

        if (parentScale <= 0.0f) { return; }

        float pointScale = targetSize / canvasSize / parentScale;
        pointCanvas.localScale = Vector3.one * pointScale;
    }

    void UpdatePointPosition()
    {
        if (mainCamera == null) { return; }
        if (pointCanvas == null) { return; }

        UpdateItemBounds();

        Vector3 screenPosition = mainCamera.WorldToScreenPoint(itemBounds.center);

        float itemDepth = Mathf.Max(itemBounds.size.x, itemBounds.size.y, itemBounds.size.z);
        screenPosition.z -= itemDepth * 0.5f + surfaceOffset;

        pointCanvas.position = mainCamera.ScreenToWorldPoint(screenPosition);
    }

    private void Update()
    {
        if (!having)
        {
            transform.localPosition = startPos + Vector3.up * Mathf.Sin(Time.time * speed) * amplitude;
        }
        else
        {
            transform.localPosition = Vector3.zero;
        }
    }

    void LateUpdate()
    {
        if (mainCamera == null) { return; }
        if (pointCanvas == null) { return; }

        UpdatePointPosition();

        pointCanvas.rotation = mainCamera.transform.rotation;
    }
}