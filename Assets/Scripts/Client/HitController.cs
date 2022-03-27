using Backend;
using Client;
using UnityEngine;

public class HitController : BaseMouseTool {
    [SerializeField] private MeshRenderer meshRenderer;

    public Material materialNeverTouched;
    public Material materialWasHit;
    public Material materialWasMiss;

    private void Start() {
        // Hide on start to avoid weirdness.
        meshRenderer.enabled = false;
    }
    
    private void Update() {
        // Move on grid.
        MoveToMouseOnGrid();
        
        // Hide if we're out-of-bounds
        meshRenderer.enabled = IsInGrid;
        
        // Change colour based on the last action in the cell.
        if (MouseLogicalPos.HasValue) {
            // Get the last state
            var lastState = GameManager.Singleton.GetCellLastState(MouseLogicalPos.Value);
            
            // Apply the material
            if (lastState == HitResult.Type.Miss) {
                meshRenderer.material = materialWasMiss;
            } else if (lastState == HitResult.Type.None) {
                meshRenderer.material = materialNeverTouched;
            } else {
                meshRenderer.material = materialWasHit;
            }
            
            // Click action (only if the last attempt wasn't a miss).
            if (PerformAction && lastState != HitResult.Type.Miss) {
                if (GameManager.Singleton.IsMyTurn()) {
                    // Attempt a hit at this cell
                    GameManager.Singleton.TryHit(MouseGridPos.Value);
                }
            }
        }
    }
}