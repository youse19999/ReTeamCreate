using UnityEngine;
using UnityEngine.InputSystem;

public class NewPlayerController : MonoBehaviour
{
    private Rigidbody rigidbody;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float turnSpeed = 90f;

    [SerializeField] private float forwardDistance = 1.0f;
    [SerializeField] private float stepHeightThreshold = 0.5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float rayStartHeight = 2.0f;
    [SerializeField] private float rayDistance = 5.0f;

    private Vector2 leftStickInput;
    private Vector2 rightStickInput;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // PlayerInput (Send Messages) から呼び出されるメソッド
    public void OnMove(InputValue value)
    {
        leftStickInput = value.Get<Vector2>();
    }

    public void OnMove2(InputValue value)
    {
        rightStickInput = value.Get<Vector2>();
    }

    void Update()
    {
        float forwardInput = leftStickInput.y;
        float turnInput = rightStickInput.x;

        // 移動（AddForce）
        rigidbody.AddForce((this.transform.forward * forwardInput) * speed);

        // 回転処理
        float turnAngle = turnInput * turnSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turnAngle, 0f);
        rigidbody.MoveRotation(rigidbody.rotation * turnRotation);

        // --- 前方の地面判定と段差昇降処理 ---
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