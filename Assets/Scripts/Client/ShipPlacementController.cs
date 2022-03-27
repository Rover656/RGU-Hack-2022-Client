using System;
using Backend;
using UnityEngine;

namespace Client {
    public class ShipPlacementController : BaseMouseTool {
        private GameObject previewVessel;

        private int Elevation {
            get {
                if (_selected.Boat) {
                    return Constants.WaterLevel;
                }

                return _elevation;
            }
        }

        // TODO: Ship type selection stuff.
        private ShipType _selected = ShipType.Battleship;
        private int _elevation;
        private bool _upwards = true;

        private void Update() {
            // Enable/Disable systems
            if (GameManager.Singleton.PlacementsEnabled()) {
                if (previewVessel == null) {
                    CreatePreview();
                }
                
                // Move tool on grid.
                MoveToMouseOnGrid();
                
                // Only show the vessel if the mouse is in the grid.
                previewVessel.SetActive(IsInGrid);
                
                // TODO: Elevation controls.
            
                // TODO: Navigate through ship types, recreating the preview.
            
                // Placement actions
                if (MouseLogicalPos.HasValue) {
                    if (PerformAction) {
                        GameManager.Singleton.PlaceShip(GetShipDescriptor());
                    }
                }
            } else {
                if (previewVessel != null) {
                    DestroyPreview();
                }
            }
        }

        private void CreatePreview() {
            previewVessel = Instantiate(GetShipPrefab(), Vector3.zero, Quaternion.identity, transform);
        }

        private void DestroyPreview() {
            Destroy(previewVessel);
        }

        private Ship GetShipDescriptor() {
            if (!MouseLogicalPos.HasValue)
                throw new Exception("Mouse was not present, ship cannot be placed!");

            var coord = Constants.GetShipRearPos(MouseGridPos.Value, _upwards, _selected);
            Debug.Log($"Coord picked: ${coord}");
            return new Ship(coord, _upwards, _selected);
        }

        private GameObject GetShipPrefab() {
            if (_selected.Equals(ShipType.Battleship)) {
                return GameManager.Singleton.battleshipPrefab;
            }

            return null;
        }
    }
}