using System;
using System.Collections.Generic;
using Backend;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour {

    #region Behaviour Actions

    // Setup
    private void Setup() {
        if (NetworkManager.Singleton.IsServer) {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        } else {
                
        }
    }

    #endregion
    
    #region Client Frontend

    public void PlaceShip(Ship ship) {
        if (NetworkManager.Singleton.IsClient) {
            PlaceShipServerRpc(ship);
        } else throw new Exception("Why is the server placing ships??");
    }
    
    #endregion

    public void StartGame() {
        if (NetworkManager.IsHost) {
            // TODO: Perform necessary actions.
        }
    }

    #region Serverside Backend

    private OceanMap _map = new OceanMap(); // TODO: Move back to ServerSetupGame()
    private Dictionary<ulong, ushort> _clientToTeam;

    private const ushort TeamCap = 2; // In case we use AI
    private const ushort HumanTeamCap = 2;
    private ushort _teams;

    private void ServerSetupGame() {
        // Create map
        _map = new OceanMap();
            
        // TODO: Create ships for all present teams.
            
        // Just a test
        _map.AddShip(new Ship(new Vector3Int(2, 4, 6), new Vector3Int(1, 0, 0),
            ShipType.Boat, 6, 1));
    }

    private void OnClientConnected(ulong client) {
        if (_teams < HumanTeamCap && _teams < TeamCap) {
            _clientToTeam.Add(client, _teams++);
        } else {
            Debug.LogError("Client tried to connect to a full game!");
            NetworkManager.Singleton.DisconnectClient(client);
        }
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
    }

    [ServerRpc]
    private void BombServerRpc(Vector2Int pos) {
        var result = _map.BombAt(pos);
    }

    #endregion

    #region Client Backend

    // TODO: Track ships etc. so we can render effects

    /// <summary>
    /// Callback for attempting to place a ship.
    /// Determines the final ship location etc.
    /// </summary>
    /// <param name="ship">The ship that was placed.</param>
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
    private void ShipHitClientRpc(Vector3Int hit, HitResult hitResult) {
        // Render effects etc.
    }

    #endregion
        
}