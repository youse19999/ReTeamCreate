using UnityEngine;

public class KobayashiPlayer : MonoBehaviour,IPlayer
{
    [SerializeField] private PlayerScriptableObject playerParameter;
    bool onGround;

    private void PlayerMove()
    {
        float horizontalInput = Input.GetAxis("Horizontal");//‰¡ˆÚ“®‚Ìˆ—
        float varticalInput = Input.GetAxis("Vertical");//cˆÚ“®‚Ìˆ—

        if (Input.GetKey(KeyCode.Space)) { }
        float jumpInput = Input.GetAxis("Jump");


        transform.Translate(new Vector3(horizontalInput, jumpInput, varticalInput)
                * playerParameter.speed * Time.deltaTime);
    }

    public void CreateMove()
    {
        PlayerMove();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();
    }

    void OnTriggerStay(Collider other)
    {
            
    }
}
