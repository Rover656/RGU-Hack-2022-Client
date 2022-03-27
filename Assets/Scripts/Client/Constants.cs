using System;
using Backend;
using UnityEditor.Search;
using UnityEngine;

namespace Client {
    public static class Constants {
        public const int SquareSize = 32;
        public const int MapUnitSize = 10;
        public const int MapSize = MapUnitSize * SquareSize;
        public const int WaterLevel = 6;

        public const float PlaneToSquareSize = 3f;

        public static bool IsInGrid(GridCoordinate pos) {
            // Debug.Log(pos);
            return pos.x >= 0 && pos.x < MapUnitSize &&
                   pos.y >= 0 && pos.y < MapUnitSize &&
                   pos.z >= 0 && pos.z < MapUnitSize;
        }

        public static Vector3 WorldGridAlign(Vector3 pos) {
            return new Vector3(
                Mathf.Round(pos.x / SquareSize) * SquareSize,
                Mathf.Round(pos.y / SquareSize) * SquareSize,
                Mathf.Round(pos.z / SquareSize) * SquareSize
            );
        }

        public static Vector3 WorldGridToWorld(Vector3Int pos) {
            return new Vector3(
                pos.x * SquareSize,
                pos.y * SquareSize,
                pos.z * SquareSize
            );
        }

        public static GridCoordinate WorldToLogicalGrid(Vector2Int worldPos) {
            return new GridCoordinate(worldPos.x + MapUnitSize / 2, WaterLevel, worldPos.y + MapUnitSize / 2);
        }

        public static Vector3Int LogicalToWorldGrid(GridCoordinate logicalPos) {
            return new Vector3Int(logicalPos.x - MapUnitSize / 2, logicalPos.y, logicalPos.z - MapUnitSize / 2);
        }

        public static GridCoordinate GetShipRearPos(GridCoordinate position, bool up, ShipType type) {
            return new GridCoordinate(
                position.x - (!up ? (type.Length - 1) / 2 : 0),
                position.y,
                position.z - (up ? (type.Length - 1) / 2 : 0)
            );
        }

        public static GridCoordinate GetShipMiddle(Ship ship) {
            return new GridCoordinate(
                ship.Position.x + (!ship.Up ? (ship.Type.Length - 1) / 2 : 0),
                ship.Position.y,
                ship.Position.z + (ship.Up ? (ship.Type.Length - 1) / 2 : 0)
            );
        }
    }
}