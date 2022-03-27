using UnityEngine;

public class UIManager : MonoBehaviour {
    public GameObject waitingUI;
    public GameObject placingUI;
    public GameObject attackUI;

    public Transform ownShips;
    public Transform enemyShips;

    public static UIManager Singleton;

    private void Start() {
        Singleton = this;
    }
}