using System;
using System.Collections.Generic;
using Backend;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : NetworkBehaviour {

    public GameObject waitingForConnectUI;
    public GameObject placementUI;
    public GameObject attackUI;
    
    #region Behaviour Actions

    // Setup
    private void Start() {
        if (NetworkManager.IsHost) {
            waitingForConnectUI.SetActive(true);
        }

        placementUI.SetActive(false);
        attackUI.SetActive(false);
        
        Debug.Log("This bit runs.");
        if (NetworkManager.IsServer) {
            Debug.Log("Server init.");
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.ConnectionApprovalCallback += OnConnectionApprovalCallback;

            if (NetworkManager.IsHost) {
                _clients.Add(NetworkManager.Singleton.LocalClientId);
            }
        }
    }

    #endregion
    
    #region Client Frontend

    public bool PlacementsEnabled() {
        return _isPlacing;
    }
    
    public bool IsMyTurn() {
        return _isTurn;
    }

    public void PlaceShip(Ship ship) {
        if (NetworkManager.Singleton.IsClient) {
            PlaceShipServerRpc(ship);
        } else throw new Exception("Why is the server placing ships??");
    }

    public void TryHit(Vector2Int position) {
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

    // Turn tracker for players.
    private int _currentTurn = -1;

    private void OnClientConnected(ulong client) {
        // Track the client, used for clearing up who's player 1 and 2.
        _clients.Add(client);

        if (_clients.Count >= 2) {
            if (IsHost) {
                waitingForConnectUI.gameObject.SetActive(false);
            }
            
            BeginPlacePhaseClientRpc();
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

    [ServerRpc]
    private void PlaceShipServerRpc(Ship ship, ServerRpcParams rpcParams = default) {
        // Set the owner
        ship.Owner = rpcParams.Receive.SenderClientId;
        
        // Place ship
        // TODO: Validation..
        _map.AddShip(ship);

        // Return the placed ship
        ShipPlacedClientRpc(ship, new ClientRpcParams() {
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

    [ServerRpc]
    private void TryHitServerRpc(Vector2Int pos, ServerRpcParams rpcParams = default) {
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

    #region Map Events

    /// <summary>
    /// Callback for attempting to place a ship.
    /// Determines the final ship location etc.
    /// </summary>
    /// <param name="ship">The ship that was placed.</param>
    /// <param name="receiveParams"></param>
    [ClientRpc]
    private void ShipPlacedClientRpc(Ship ship, ClientRpcParams receiveParams = default) {
        // TODO: Render ship for meeeee
        
        Debug.Log("A SHIP HATH BEEN PLACE!");
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
        _isPlacing = true;
        placementUI.SetActive(true);
    }

    [ClientRpc]
    private void EndPlacePhaseClientRpc() {
        // TODO: Disable build controls
        _isPlacing = false;
        placementUI.SetActive(false);
    }

    [ClientRpc]
    private void TurnStartClientRpc(ClientRpcParams rpcParams = default) {
        // TODO: Enable client controls.
        _isTurn = true;
        attackUI.SetActive(true);
    }

    [ClientRpc]
    private void TurnEndClientRpc(ClientRpcParams rpcParams = default) {
        // TODO: Disable client controls
        _isTurn = false;
        attackUI.SetActive(false);
    }

    #endregion

    #endregion
        
}