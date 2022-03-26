using Client;
using UnityEngine;

public class ShipScaleRenderer : MonoBehaviour {
    public int length;
    public Vector3 direction;
    
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, Vector3.one * Constants.SquareSize + direction * length * Constants.SquareSize * 0.5f);
    }
}
