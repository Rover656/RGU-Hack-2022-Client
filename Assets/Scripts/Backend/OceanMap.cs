using System.Collections.Generic;
using UnityEngine;

namespace Backend {
    public class OceanMap {
        public int WaterLevel = 16;

        private List<Ship> _ships = new List<Ship>();
        
        public void AddShip(Ship ship) {
            // Store
            _ships.Add(ship);
        }

        public HitResult BombAt(Vector2Int pos) {
            // X is X and Y is Z ;P
            for (int y = WaterLevel + 1; y >= 0; y++) {
                var shipPos = new Vector3Int(pos.x, y, pos.y);
                
                // TODO: Some way of organising this data so I dont have to iterate so many damn times
                foreach (var ship in _ships) {
                    if (ship.IsHit(shipPos)) {
                        return ship.Hit(shipPos);
                    }
                }
            }

            return HitResult.Miss;
        }
    }
}