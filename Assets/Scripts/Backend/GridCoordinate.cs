using System;
using Unity.Netcode;
using UnityEngine;

namespace Backend {
    public struct GridCoordinate : INetworkSerializable {
        public int x;
        public int y;
        public int z;

        public GridCoordinate(int x, int y, int z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            serializer.SerializeValue(ref x);
            serializer.SerializeValue(ref y);
            serializer.SerializeValue(ref z);
        }

        public override string ToString() {
            return String.Format("GridCoordinate({0}, {1}, {2})", x, y, z);
        }
    }
}