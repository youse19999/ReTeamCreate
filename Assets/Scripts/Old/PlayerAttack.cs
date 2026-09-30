using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackDistance = 2f;
    [SerializeField] private LayerMask playerLayer;

    private void Update()
    {
        // Xbox‚ÌAƒ{ƒ^ƒ“
        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }
    }

    void Attack()
    {
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        Debug.Log("‰£‚è");
        if (Physics.Raycast(ray, out RaycastHit hit, attackDistance, playerLayer))
        {
            if (hit.collider.gameObject == gameObject)
                return;

            PlayerStun stun = hit.collider.GetComponent<PlayerStun>();

            if (stun != null)
            {
                stun.Stun();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + Vector3.up, transform.forward * attackDistance);
    }
}