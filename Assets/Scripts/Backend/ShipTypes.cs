using System.Collections.Generic;

namespace Backend {
    public class ShipTypes {
        private static Dictionary<ShipType, int> _lengths = new Dictionary<ShipType, int>();
        private static Dictionary<ShipType, bool> _boats = new Dictionary<ShipType, bool>();

        private static bool _init = false;
        
        public static int GetLength(ShipType type) {
            if (!_init) {
                Init();
                _init = true;
            }
            return _lengths[type];
        }

        public static bool IsBoat(ShipType type) {
            if (!_init) {
                Init();
                _init = true;
            }
            
            return _boats[type];
        }

        private static void Init() {
            _lengths.Add(ShipType.Battleship, 5);
            _boats.Add(ShipType.Battleship, true);
            
            _lengths.Add(ShipType.Frigate, 3);
            _boats.Add(ShipType.Frigate, true);
            
            _lengths.Add(ShipType.Destroyer, 6);
            _boats.Add(ShipType.Destroyer, true);
            
            _lengths.Add(ShipType.Cruiser, 3);
            _boats.Add(ShipType.Cruiser, true);
            
            _lengths.Add(ShipType.Corvette, 1);
            _boats.Add(ShipType.Corvette, true);
            
            _lengths.Add(ShipType.Submarine, 3);
            _boats.Add(ShipType.Submarine, false);
        }
    }
}