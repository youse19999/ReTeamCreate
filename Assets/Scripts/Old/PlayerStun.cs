using System.Collections;
using UnityEngine;

public class PlayerStun : MonoBehaviour
{
    [SerializeField] private float stunTime = 2f;

    private NewPlayerController player;

    private void Awake()
    {
        player = GetComponent<NewPlayerController>();
    }

    public void Stun()
    {
        StartCoroutine(StunCoroutine());
    }

    private IEnumerator StunCoroutine()
    {
        player.enabled = false;
        Debug.Log("ÉXÉ^ÉìÅI");

        player.DropItem();

        yield return new WaitForSeconds(stunTime);

        player.enabled = true;
    }
}