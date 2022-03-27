using Unity.Netcode;
using UnityEngine;

public class GameManagerCreator : MonoBehaviour {
    public GameObject gameManager;

    // Start is called before the first frame update
    void Start()
    {
        if (NetworkManager.Singleton.IsHost) {
            var go = Instantiate(gameManager);
            go.GetComponent<NetworkObject>().Spawn();
        }
    }
}
