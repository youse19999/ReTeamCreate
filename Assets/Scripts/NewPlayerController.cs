using UnityEngine;
using UnityEngine.InputSystem;

public class NewPlayerController : MonoBehaviour
{
    private Rigidbody rigidbody;
    [SerializeField] private float speed;
    [SerializeField] private InputActionAsset inputActions;
    private InputAction moveAction;
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

        rigidbody.AddForce((this.transform.forward*y)*speed);
        rigidbody.AddTorque(new Vector3(0, x, 0)*speed);
    }
}
