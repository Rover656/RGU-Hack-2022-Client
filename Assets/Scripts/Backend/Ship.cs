using Backend;
using Unity.Netcode;
using UnityEngine;

public struct Ship : INetworkSerializable {
    public Vector3Int Position;

    /// <summary>
    /// Unit vector for direction.
    /// </summary>
    public Vector3Int Direction; // (0, -1, 0) for example.
    public ShipType Type;
    public int Length;
    public bool[] Hits;
    public ulong Owner;

    public Ship(Vector3Int position, Vector3Int direction, ShipType type, int length, ulong owner = default) {
        Position = position;
        Direction = direction;
        Type = type;
        Length = length;
        Hits = new bool[length];
        Owner = owner;
    }

    /// <summary>
    /// Attempt to hit the ship.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public HitResult Hit(Vector3Int pos) {
        if (IsHit(pos)) {
            // Translate the position into an index along the length of the ship.
            var shipRelPos = pos - Position;

            // Get index along the direction of length
            var idx = shipRelPos.x * Direction.x + shipRelPos.y * Direction.y + shipRelPos.z * Direction.z;
            
            // If we already hit, this is a "miss"
            if (Hits[idx]) {
                return HitResult.Miss;
            }

            // Mark the hit.
            Hits[idx] = true;
            return IsDestroyed() ? HitResult.Destroy : HitResult.Hit;
        }

        // Dumbass.
        return HitResult.Miss;
    }

    public bool IsHit(Vector3Int pos) {
        // Check for a collision at this position.
        return pos.x >= Position.x && pos.x <= Position.x + Direction.x * (Length - 1) &&
               pos.y >= Position.y && pos.y <= Position.y + Direction.y * (Length - 1) &&
               pos.z >= Position.z && pos.z <= Position.z + Direction.z * (Length - 1);
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
        serializer.SerializeValue(ref Direction);
        serializer.SerializeValue(ref Type);

        var length = 0;
        if (!serializer.IsReader) {
            length = Length;
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