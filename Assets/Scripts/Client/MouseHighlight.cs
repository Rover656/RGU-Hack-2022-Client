using Client;
using UnityEngine;

public class MouseHighlight : MonoBehaviour {
    [SerializeField] private Camera mainCamera;

    private void Update() {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var raycastHit)) {
            var target = raycastHit.point;

            // Attach to the grid squares.
            target.x = Mathf.Round(target.x / Constants.SquareSize) * Constants.SquareSize;
            target.y = Mathf.Round(target.y / Constants.SquareSize) * Constants.SquareSize;
            target.z = Mathf.Round(target.z / Constants.SquareSize) * Constants.SquareSize;

            transform.position = target;
        }
    }
}