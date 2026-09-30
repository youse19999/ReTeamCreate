using UnityEngine;
using UnityEngine.InputSystem;

public class NewPlayerController : MonoBehaviour
{
    private Rigidbody rigidbody;
    [SerializeField] private float speed;
    [SerializeField] private InputActionAsset inputActions;
    private InputAction moveAction;

    [SerializeField] private float forwardDistance = 1.0f; // 前方の検知距離
    [SerializeField] private float stepHeightThreshold = 0.5f; // 乗れる段差（上下）の閾値
    [SerializeField] private LayerMask groundLayer; // 地面のレイヤー
    [SerializeField] private float rayStartHeight = 2.0f; // レイを撃ち下ろす高さ
    [SerializeField] private float rayDistance = 5.0f; // レイの長さ

    void OnEnable()
    {
        var gameplayMap = inputActions.FindActionMap("Gameplay");
        moveAction = gameplayMap.FindAction("Move");
        moveAction.Enable();
    }

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void OnDisable()
    {
        moveAction.Disable();
    }

    void Update()
    {
        Vector2 axisValue = moveAction.ReadValue<Vector2>();

        float x = axisValue.x;
        float y = axisValue.y;

        Debug.Log($"Axis X: {x}, Y: {y}");

        rigidbody.AddForce((this.transform.forward * y) * speed);
        rigidbody.AddTorque(new Vector3(0, x, 0) * speed);

        // --- 前方の地面判定と段差昇降処理（上り・下り両対応） ---
        Vector3 targetXZ = transform.position + transform.forward * forwardDistance;
        Vector3 rayOrigin = new Vector3(targetXZ.x, transform.position.y + rayStartHeight, targetXZ.z);

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, rayDistance, groundLayer))
        {
            float groundY = hit.point.y;
            float diffY = groundY - transform.position.y;

            // 高低差の絶対値が0より大きく、かつ閾値以下の場合（上り・下り共通）
            if (Mathf.Abs(diffY) > 0.001f && Mathf.Abs(diffY) <= stepHeightThreshold)
            {
                Vector3 newPos = transform.position;
                newPos.y = groundY;
                transform.position = newPos;

                // 垂直方向の慣性をリセット
                rigidbody.linearVelocity = new Vector3(rigidbody.linearVelocity.x, 0, rigidbody.linearVelocity.z);
            }
        }
    }
}