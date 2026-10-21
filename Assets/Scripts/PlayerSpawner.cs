using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField]
    Transform p1s;
    [SerializeField]
    Transform p2s;
    [SerializeField]
    PlayerInputManager playerInputManager;
    void Start()
    {
        var p1 = playerInputManager.JoinPlayer();
        var p2 = playerInputManager.JoinPlayer();
        p1.GetComponentsInChildren<Animator>()[0].gameObject.SetActive(false);
        p2.GetComponentsInChildren<Animator>()[1].gameObject.SetActive(false);
        p1.GetComponent<NewPlayerController>().animator = p1.GetComponentInChildren<Animator>();
        p2.GetComponent<NewPlayerController>().animator = p2.GetComponentInChildren<Animator>();
        p1.gameObject.transform.position = p1s.position;
        p2.gameObject.transform.position = p2s.position;
        p1.GetComponent<PlayerTag>().ChangeName("P1");
        p2.GetComponent<PlayerTag>().ChangeName("P2");
    }
    void Update()
    {
        
    }
}
