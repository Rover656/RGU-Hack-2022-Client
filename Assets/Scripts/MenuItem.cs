using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuItem : MonoBehaviour
{
    // Start is called before the first frame update
    public void StartHosting() {
        NetworkManager.Singleton.StartHost();
        SceneManager.LoadScene(1);
    }

    public void StartClient() {
        SceneManager.LoadScene(1); // loading screen
        NetworkManager.Singleton.StartClient();
    }
    
    
}
