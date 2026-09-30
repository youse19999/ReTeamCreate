using UnityEngine;

public class PlayerStructure : MonoBehaviour
{
    public bool HasItem
    {
        get { return Item == null; }
    }
    public float stopSpeed;
    public float speed;
    public float deadzone;
    public float rotateSpeed;
    public ItemStructure Item { get; set; } = default;
}