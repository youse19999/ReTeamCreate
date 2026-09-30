using UnityEngine;

public class GameITem : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        GamePlayer gamePlayer = null;
        if (other.TryGetComponent<GamePlayer>(out gamePlayer))
        {
            gamePlayer.ShowItem(this.gameObject);
            Debug.LogWarning(this.gameObject);
        }
    }
}
