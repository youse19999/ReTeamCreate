using UnityEngine;

[CreateAssetMenu(fileName = "PlayerParamwter", menuName = "ScriptableObjects/PlayerParameter", order = 1)]
public class PlayerScriptableObject : ScriptableObject
{
    [SerializeField]
    public float speed;
}