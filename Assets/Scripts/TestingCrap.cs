using Backend;
using Unity.Netcode;
using UnityEngine;

public class TestingCrap : MonoBehaviour {
    public GameManager gameManager;
    
    // Start is called before the first frame update
    void Start() {
        var ship = new Ship(new Vector3Int(0, 0, 3), new Vector3Int(0, 1, 0), ShipType.Boat, 5, 0);

        NetworkManager.Singleton.StartHost();
        gameManager.PlaceShip(ship);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
