using UnityEngine;
using UnityEngine.InputSystem;

public class NewPlayerController : MonoBehaviour
{
    private Rigidbody rigidbody;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float turnSpeed = 90f; // 毎秒の旋回角度（度/秒）
    [SerializeField] private InputActionAsset inputActions;

    private InputAction moveAction;
    private InputAction move2Action;

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

        move2Action = gameplayMap.FindAction("Move2");
        move2Action.Enable();
    }

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void OnDisable()
    {
        moveAction.Disable();
        move2Action.Disable();
    }

    void Update()
    {
        // 左スティック（前後移動）
        Vector2 leftStick = moveAction.ReadValue<Vector2>();
        float forwardInput = leftStick.y;

        // 右スティック（左右旋回）
        Vector2 rightStick = move2Action.ReadValue<Vector2>();
        float turnInput = rightStick.x;

        Debug.Log($"Forward: {forwardInput}, Turn: {turnInput}");

        // 移動（AddForce）
        rigidbody.AddForce((this.transform.forward * forwardInput) * speed);

        // angularVelocityに依存しない回転処理
        float turnAngle = turnInput * turnSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turnAngle, 0f);
        rigidbody.MoveRotation(rigidbody.rotation * turnRotation);

        // --- 前方の地面判定と段差昇降処理（上り・下り両対応） ---
        Vector3 targetXZ = transform.position + transform.forward * forwardDistance;
        Vector3 rayOrigin = new Vector3(targetXZ.x, transform.position.y + rayStartHeight, targetXZ.z);

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, rayDistance, groundLayer))
        {
            float groundY = hit.point.y;
            float diffY = groundY - transform.position.y;

            if (Mathf.Abs(diffY) > 0.001f && Mathf.Abs(diffY) <= stepHeightThreshold)
            {
                Vector3 newPos = transform.position;
                newPos.y = groundY;
                transform.position = newPos;

                rigidbody.linearVelocity = new Vector3(rigidbody.linearVelocity.x, 0, rigidbody.linearVelocity.z);
            }
        }
    }
}