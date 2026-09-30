using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerJoin : MonoBehaviour
{
    int num;
    [SerializeField]
    PlayerInputManager manager;
    [SerializeField]
    GoalArea area1;
    [SerializeField]
    GoalArea area2;
    public void Awake()
    {
        manager.onPlayerJoined += joined;
    }

    private void joined(PlayerInput input)
    {
        Debug.Log("aaa");
        if (num == 0)
        {
            area1.targetPlayer = input.gameObject;
        }
        if (num == 1)
        {
            area2.targetPlayer = input.gameObject;
        }
        num++;
    }
}
