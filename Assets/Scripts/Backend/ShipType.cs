using Unity.Netcode;

namespace Backend {
    public class ShipType : INetworkSerializable {
        public ShipType BATTLESHIP = new ShipType(5);

        public int Length;

        public ShipType() {
            Length = 0;
        }

        public ShipType(int length) {
            Length = length;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            serializer.SerializeValue(ref Length);
        }
    }
}