using Unity.Netcode;

namespace Backend {
    public struct ShipType : INetworkSerializable {
        // TODO: WE DEPEND UPON ODD NUMBERS!
        public static readonly ShipType Battleship = new ShipType(0, 5, true);
        public static readonly ShipType Frigate = new ShipType(1, 3, true);
        public static readonly ShipType Submarine = new ShipType(10, 3, false);

        private int _id;
        public int Length;
        public bool Boat;
        
        public ShipType(int id, int length, bool boat) {
            _id = id;
            Length = length;
            Boat = boat;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            serializer.SerializeValue(ref _id);
            serializer.SerializeValue(ref Length);
            serializer.SerializeValue(ref Boat);
        }

        public override bool Equals(object obj) {
            if (obj is ShipType) {
                if (_id == ((ShipType) obj)._id) {
                    return true;
                }
            }

            return false;
        }
    }
}