using Backend;
using UnityEngine;

namespace Client {
    public abstract class BaseMouseTool : MonoBehaviour {
        protected Vector2Int? MouseLogicalPos => MouseUtil.GetMouseGridPos();

        protected GridCoordinate? MouseGridPos {
            get {
                if (MouseLogicalPos.HasValue) {
                    return Constants.WorldToLogicalGrid(MouseLogicalPos.Value);
                }

                return null;
            }
        }

        protected Vector3? MouseWorldPos => MouseUtil.GetMouseWorldPos();

        protected Vector3? MouseWorldPosGridAlign {
            get {
                var worldPos = MouseWorldPos;
                if (worldPos.HasValue) {
                    return Constants.WorldGridAlign(worldPos.Value);
                }
                return null;
            }
        }

        protected bool IsInGrid {
            get {
                var pos = MouseGridPos;
                return pos.HasValue && Constants.IsInGrid(pos.Value);
            }
        }
        
        protected bool PerformAction => Input.GetMouseButtonDown(0);

        protected void MoveToMouseOnGrid() {
            if (MouseWorldPosGridAlign.HasValue) {
                transform.position = MouseWorldPosGridAlign.Value;
            }
        }
    }
}