using Unity.Netcode;
using UnityEngine;

namespace Backend {
    public struct HitResult {
        public enum Type {
            Miss,
            Hit,
            Destroy
        }

        public Type HitType;
        public Vector3Int Position;

        public HitResult(Type hitType, Vector3Int position) {
            HitType = hitType;
            Position = position;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            serializer.SerializeValue(ref HitType);
            serializer.SerializeValue(ref Position);
        }
    }
}