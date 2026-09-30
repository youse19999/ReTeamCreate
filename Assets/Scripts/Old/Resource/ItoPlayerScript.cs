using System.Data;
using UnityEngine;

public class ItoPlayerScript : MonoBehaviour,IPlayer
{
    [SerializeField] private float playerSpeed = 10.0f;
    [SerializeField] private float jumpPower = 10.0f;
    private float horizontal;
    private float vertical;
    private bool canJump = true;

    private Rigidbody rigidBody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //キー入力
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        CreateMove();
    }

    public void CreateMove()
    {
        //移動処理
        Vector3 moveDirectionY = (transform.forward * vertical) * (playerSpeed / 100);
        Vector3 moveDirectionX = (transform.right * horizontal) * (playerSpeed / 100);
        Vector3 moveDirection = moveDirectionX + moveDirectionY;
        transform.Translate(moveDirection);

        //ジャンプ処理
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!canJump) { return; }
            canJump = false;
            rigidBody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
    }

    //他オブジェクトに触れた時の処理
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ground") { canJump = true; }

    }
}
