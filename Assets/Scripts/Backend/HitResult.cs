using Unity.Netcode;
using UnityEngine;

namespace Backend {
    public struct HitResult {
        public enum Type {
            None,
            Miss,
            Hit,
            Destroy
        }

        public Type HitType;
        public GridCoordinate Position;

        public HitResult(Type hitType, GridCoordinate position) {
            HitType = hitType;
            Position = position;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            serializer.SerializeValue(ref HitType);
            serializer.SerializeValue(ref Position);
        }
    }
}