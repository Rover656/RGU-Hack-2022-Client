using Backend;
using Client;
using Unity.Netcode;
using UnityEngine;

public struct Ship : INetworkSerializable {
    public GridCoordinate Position;

    /// <summary>
    /// Unit vector for direction.
    /// </summary>
    public bool Up;
    public ShipType Type;
    public bool[] Hits;
    public ulong Owner;
    
    public Ship(GridCoordinate position, bool up, ShipType type, ulong owner = default) {
        Position = position;
        Up = up;
        Type = type;
        Hits = new bool[type.Length];
        Owner = owner;
    }

    /// <summary>
    /// Attempt to hit the ship.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public HitResult Hit(GridCoordinate pos) {
        if (IsHit(pos)) {
            // Translate the position into an index along the length of the ship.
            var shipRelPos = new GridCoordinate(pos.x - Position.x, pos.y - Position.y, pos.z - Position.z);

            // Get index along the direction of length
            var idx = Up ? shipRelPos.z : shipRelPos.x;
            
            // If we already hit, this is a "miss"
            if (Hits[idx]) {
                return new HitResult(HitResult.Type.Miss, new GridCoordinate(pos.x, Constants.WaterLevel, pos.z));
            }

            // Mark the hit.
            Hits[idx] = true;
            return IsDestroyed() ? new HitResult(HitResult.Type.Destroy, pos) : new HitResult(HitResult.Type.Hit, pos);
        }

        // Dumbass.
        return new HitResult(HitResult.Type.Miss, new GridCoordinate(pos.x, Constants.WaterLevel, pos.z));
    }

    public bool IsHit(GridCoordinate pos) {
        // Check for a collision at this position.
        if (Up) {
            return pos.x == Position.x &&
                   pos.y == Position.y &&
                   pos.z >= Position.z &&
                   pos.z < Position.z + Type.Length;
        } else {
            return pos.x >= Position.x &&
                   pos.x < Position.x + Type.Length &&
                   pos.y == Position.y &&
                   pos.z == Position.z;
        }
    }

    public bool IsDestroyed() {
        foreach (var hit in Hits) {
            if (!hit) {
                return false;
            }
        }
        return true;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref Position);
        serializer.SerializeValue(ref Up);
        serializer.SerializeNetworkSerializable(ref Type);

        var length = 0;
        if (!serializer.IsReader) {
            length = Type.Length;
        }
        serializer.SerializeValue(ref length);

        if (serializer.IsReader) {
            Hits = new bool[length];
        }

        for (int i = 0; i < length; i++) {
            serializer.SerializeValue(ref Hits[i]);
        }

        serializer.SerializeValue(ref Owner);
    }
}