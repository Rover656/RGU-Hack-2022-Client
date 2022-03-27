using System;
using Backend;
using UnityEngine;

namespace Client {
    public class ShipPlacementController : BaseMouseTool {
        private GameObject previewVessel;

        private ShipType _lastType;

        private int Elevation {
            get {
                if (!GameManager.Singleton.GetPlacingShipType().HasValue) return Constants.WaterLevel;
                if (ShipTypes.IsBoat(GameManager.Singleton.GetPlacingShipType().Value)) {
                    return Constants.WaterLevel;
                }

                return _elevation;
            }
        }

        // TODO: Ship type selection stuff.
        private int _elevation = Constants.WaterLevel;
        private bool _upwards = true;

        private void Update() {
            // Enable/Disable systems
            if (GameManager.Singleton == null) return;
            if (GameManager.Singleton.OnCooldown()) return;
            if (GameManager.Singleton.GetPlacingShipType() == null) return;
            
            if (GameManager.Singleton.PlacementsEnabled()) {
                if (_lastType != GameManager.Singleton.GetPlacingShipType().Value) {
                    DestroyPreview();
                    CreatePreview();
                } else if (previewVessel == null) {
                    CreatePreview();
                }

                // Move tool on grid.
                MoveToMouseOnGrid();
                
                // Update preview
                UpdatePreview();
                
                // Only show the vessel if the mouse is in the grid.
                previewVessel.SetActive(IsInGrid);
                
                // Elevation controls.
                if (Input.GetKeyDown(KeyCode.S)) {
                    _elevation--;
                    if (_elevation < 1) _elevation = 1;
                }

                if (Input.GetKeyDown(KeyCode.W)) {
                    _elevation++;
                    if (_elevation > Constants.WaterLevel) _elevation = Constants.WaterLevel;
                }
            
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
            previewVessel = Instantiate(GameManager.Singleton.GetShipPrefab(GameManager.Singleton.GetPlacingShipType().Value), Vector3.zero, Quaternion.identity, transform);
        }

        private void DestroyPreview() {
            Destroy(previewVessel);
        }

        private void UpdatePreview() {
            var newPos = Vector3.zero;
            newPos.y = (Elevation - Constants.WaterLevel) * Constants.SquareSize;
            previewVessel.transform.localPosition = newPos;
        }

        private Ship GetShipDescriptor() {
            if (!MouseLogicalPos.HasValue)
                throw new Exception("Mouse was not present, ship cannot be placed!");

            var coord = Constants.GetShipRearPos(MouseGridPos.Value, _upwards, GameManager.Singleton.GetPlacingShipType().Value);
            coord.y = Elevation;
            Debug.Log($"Coord picked: ${coord}");
            return new Ship(coord, _upwards, GameManager.Singleton.GetPlacingShipType().Value);
        }
    }
}