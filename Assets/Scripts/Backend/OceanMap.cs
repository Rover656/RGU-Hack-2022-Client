using System.Collections.Generic;
using Client;
using UnityEngine;

namespace Backend {
    public class OceanMap {
        private List<Ship> _ships = new List<Ship>();
        
        public bool AddShip(Ship ship) {
            for (var i = 0; i < ShipTypes.GetLength(ship.Type); i++) {
                var pos = ship.Position;
                if (ship.Up) pos.z += i;
                else pos.x += i;
                
                if (CollidesWith(pos, ship.Owner)) {
                    return false;
                }

                // Stop leaving the play area!
                if (!Constants.IsInGrid(pos)) {
                    return false;
                }
            }
            
            // Store
            _ships.Add(ship);
            return true;
        }

        public bool CollidesWith(GridCoordinate pos, ulong owner) {
            foreach (var ship in _ships) {
                if (ship.Owner == owner && ship.IsHit(pos)) {
                    return true;
                }
            }

            return false;
        }
        
        public HitResult BombAt(GridCoordinate pos, ulong owner) {
            // X is X and Y is Z ;P
            for (int y = Constants.WaterLevel + 1; y >= 0; y--) {
                var shipPos = new GridCoordinate(pos.x, y, pos.z);
                
                // TODO: Some way of organising this data so I dont have to iterate so many damn times
                foreach (var ship in _ships) {
                    if (ship.Owner == owner && ship.IsHit(shipPos)) {
                        return ship.Hit(shipPos);
                    }
                }
            }

            return new HitResult(HitResult.Type.Miss, new GridCoordinate(pos.x, Constants.WaterLevel, pos.y));
        }
        
        public Ship? GetAt(GridCoordinate pos) {
            foreach (var ship in _ships) {
                if (ship.IsHit(pos)) {
                    return ship;
                }
            }

            return null;
        }
    }
}