using System;
using System.Collections.Generic;
using Backend;
using Client;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour {

    public static GameManager Singleton;

    public GameObject battleshipPrefab;
    public GameObject frigatePrefab;
    public GameObject submarinePrefab;

    private static readonly ShipType[] PlacedTypes = {
        ShipType.Battleship,
        ShipType.Frigate,
        ShipType.Frigate,
        ShipType.Submarine,
        ShipType.Submarine,
    };
    
    #region Behaviour Actions

    // Setup
    private void Start() {
        Singleton = this;

        if (NetworkManager.IsHost) {
            Debug.Log("Host waiting screen display.");
            UIManager.Singleton.waitingUI.SetActive(true);
        } else UIManager.Singleton.waitingUI.SetActive(false);

        UIManager.Singleton.placingUI.SetActive(false);
        UIManager.Singleton.attackUI.SetActive(false);
        
        Debug.Log("This bit runs.");
        if (NetworkManager.IsServer) {
            Debug.Log("Server init.");
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.ConnectionApprovalCallback += OnConnectionApprovalCallback;

            if (NetworkManager.IsHost) {
                Debug.Log("Adding local client to list.");
                _clients.Add(NetworkManager.Singleton.LocalClientId);
            }
        }
    }

    #endregion
    
    #region Client Frontend

    public GameObject GetShipPrefab(ShipType type) {
        if (type.Equals(ShipType.Battleship)) {
            return battleshipPrefab;
        }

        if (type.Equals(ShipType.Frigate)) {
            return frigatePrefab;
        }

        if (type.Equals(ShipType.Submarine)) {
            return submarinePrefab;
        }

        return null;
    }

    public bool PlacementsEnabled() {
        return _isPlacing;
    }
    
    public bool IsMyTurn() {
        return _isTurn;
    }

    public ShipType? GetPlacingShipType() {
        return _placingType;
    }

    public HitResult.Type GetCellLastState(Vector2Int pos) {
        if (_previousHits.ContainsKey(pos)) {
            return _previousHits[pos];
        }

        return HitResult.Type.None;
    }

    public void PlaceShip(Ship ship) {
        if (NetworkManager.Singleton.IsClient) {
            Debug.Log($"Asking to place at {ship.Position}");
            PlaceShipServerRpc(ship);
        } else throw new Exception("Why is the server placing ships??");
    }

    public void TryHit(GridCoordinate position) {
        if (NetworkManager.Singleton.IsClient) {
            TryHitServerRpc(position);
        }
    }
    
    #endregion

    public void StartGame() {
        if (NetworkManager.IsHost) {
            // TODO: Perform necessary actions.
        }
    }

    #region Serverside Backend

    private OceanMap _map = new OceanMap(); // TODO: Move back to ServerSetupGame()
    private List<ulong> _clients = new List<ulong>();
    private Dictionary<ulong, int> _clientPlaceCounts = new Dictionary<ulong, int>();

    // Turn tracker for players.
    private int _currentTurn = -1;

    private void OnClientConnected(ulong client) {
        // Track the client, used for clearing up who's player 1 and 2.
        _clients.Add(client);

        if (_clients.Count >= 2) {
            if (IsHost) {
                UIManager.Singleton.waitingUI.gameObject.SetActive(false);
            }
            
            Debug.Log("Clients all connected, starting placement phase.");
            
            BeginPlacePhaseClientRpc();
            SetCurrentPlaceTypeClientRpc(PlacedTypes[0]);
            _clientPlaceCounts.Add(_clients[0], 1);
            _clientPlaceCounts.Add(_clients[1], 1);
        }
    }
    
    private void OnConnectionApprovalCallback(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback) {
        // Reject if there's more than 2 people connected.
        if (_clients.Count > 2) {
            callback(false, null, false, null, null);
        }

        // Approve connection
        callback(false, null, true, null, null);
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlaceShipServerRpc(Ship ship, ServerRpcParams rpcParams = default) {
        // Set the owner
        ship.Owner = rpcParams.Receive.SenderClientId;
        
        // Place ship
        if (_map.AddShip(ship)) {
            // Return the placed ship
            OwnShipPlacedClientRpc(ship, new ClientRpcParams() {
                Send = new ClientRpcSendParams() {
                    TargetClientIds = new [] {
                        rpcParams.Receive.SenderClientId
                    }
                }
            });
        
            // TODO: If player has finished their place phase, end building mode
            // TODO: If the last player finished, trigger the first turn.

            // _currentTurn = Random.Range(0, _clients.Count);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void TryHitServerRpc(GridCoordinate pos, ServerRpcParams rpcParams = default) {
        // Get player index
        var idx = _clients.FindIndex(x => x == rpcParams.Receive.SenderClientId);
        if (_currentTurn != idx) {
            return;
        }
        
        // Attempt the strike
        var result = _map.BombAt(pos);
        
        // Broadcast the hit
        BombResultClientRpc(result);
        
        // End the current turn
        TurnEndClientRpc(new ClientRpcParams {
            Send = new ClientRpcSendParams {
                TargetClientIds = new []{rpcParams.Receive.SenderClientId}
            }
        });

        // Start next turn
        _currentTurn = (_currentTurn + 1) % _clients.Count;
        
        TurnStartClientRpc(new ClientRpcParams {
            Send = new ClientRpcSendParams {
                TargetClientIds = new []{_clients[_currentTurn]}
            }
        });
    }

    #endregion

    #region Client Backend

    // TODO: Track ships etc. so we can render effects

    private bool _isPlacing = false;
    private bool _isTurn = false;

    private ShipType? _placingType = null;

    private Dictionary<Vector2Int, HitResult.Type> _previousHits = new Dictionary<Vector2Int, HitResult.Type>();

    #region Map Events

    /// <summary>
    /// Callback for attempting to place a ship.
    /// Determines the final ship location etc.
    /// </summary>
    /// <param name="ship">The ship that was placed.</param>
    /// <param name="receiveParams"></param>
    [ClientRpc]
    private void OwnShipPlacedClientRpc(Ship ship, ClientRpcParams receiveParams = default) {
        Debug.Log("A SHIP HATH BEEN PLACE!");

        // Reorient for client.
        ship.Position = Constants.GetShipMiddle(ship);

        // TODO: Put it into the "My Ships" group.
        Instantiate(GetShipPrefab(ship.Type), Constants.WorldGridToWorld(Constants.LogicalToWorldGrid(ship.Position)),
            Quaternion.identity, null);
    }

    [ClientRpc]
    private void DisplayDestroyedEnemyShipClientRpc(Ship ship, ClientRpcParams rpcParams = default) {
        // TODO: Add the render of the ship to the enemy layer to represent a beaten vessel.
    }

    /// <summary>
    /// Broadcast to all clients telling of a hit or miss at a given position.
    /// </summary>
    /// <param name="hit">3D position of hit. Ignore y if miss, display y if hit.</param>
    /// <param name="hitResult">The result of the hit.</param>
    [ClientRpc]
    private void BombResultClientRpc(HitResult hitResult) {
        // Render effects etc.
    }

    #endregion

    #region Game Phases

    [ClientRpc]
    private void BeginPlacePhaseClientRpc() {
        // TODO: Enable build controls
        Debug.Log("We can build!");
        _isPlacing = true;
        UIManager.Singleton.placingUI.SetActive(true);
    }

    [ClientRpc]
    private void EndPlacePhaseClientRpc() {
        // TODO: Disable build controls
        _isPlacing = false;
        UIManager.Singleton.placingUI.SetActive(false);
    }

    [ClientRpc]
    private void TurnStartClientRpc(ClientRpcParams rpcParams = default) {
        // TODO: Enable client controls.
        _isTurn = true;
        UIManager.Singleton.attackUI.SetActive(true);
    }

    [ClientRpc]
    private void SetCurrentPlaceTypeClientRpc(ShipType type, ClientRpcParams rpcParams = default) {
        _placingType = type;
    }

    [ClientRpc]
    private void TurnEndClientRpc(ClientRpcParams rpcParams = default) {
        // TODO: Disable client controls
        _isTurn = false;
        UIManager.Singleton.attackUI.SetActive(false);
    }

    #endregion

    #endregion
        
}