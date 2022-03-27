using System;
using Backend;
using Client;
using UnityEngine;

public class MouseUtil {
    public static Vector3? GetMouseWorldPos() {
        if (Camera.main != null) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var raycastHit, int.MaxValue, LayerMask.GetMask("MapLayer"))) { // Ignore normal colliders
                return raycastHit.point;
            }
        }

        return null;
    }

    public static Vector2Int? GetMouseGridPos() {
        var worldPosNullable = GetMouseWorldPos();

        if (worldPosNullable.HasValue) {
            var worldPos = worldPosNullable.Value;

            // Debug.Log(worldPos);
                
            worldPos.x = Mathf.Round(worldPos.x / Constants.SquareSize);
            worldPos.y = Mathf.Round(worldPos.y / Constants.SquareSize);
            worldPos.z = Mathf.Round(worldPos.z / Constants.SquareSize);
            
            return new Vector2Int((int) worldPos.x, (int) worldPos.z);
        }

        return null;
    }
}