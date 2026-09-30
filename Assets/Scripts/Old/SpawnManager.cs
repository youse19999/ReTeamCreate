using UnityEngine;

[CreateAssetMenu(fileName = "PlayerParameter", menuName = "ScriptableObjects/PlayerParameter", order = 1)]
public class SpawnManagerScriptableObject : ScriptableObject
{
    [SerializeField]
    private float speed;
}