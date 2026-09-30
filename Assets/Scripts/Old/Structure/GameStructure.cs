using UnityEngine;

public class GameStructure : MonoBehaviour
{
    public PlayerStructure playerStructure;

    private static GameStructure structure;
    public static GameStructure GetInstance()
    {
        return structure;
    }
    public void Awake()
    {
        if (structure == null)
        {
            structure = this;
        }
    }
}