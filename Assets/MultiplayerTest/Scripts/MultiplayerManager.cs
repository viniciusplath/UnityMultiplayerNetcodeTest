using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace MultiplayerTest
{
    public enum PeerType
    {
        None,
        Server,
        Host,
        Client
    }

    public class MultiplayerManager : NetworkManager
    {
        private static MultiplayerManager instance;
        private List<NetworkClient> clients = new List<NetworkClient>();
        private PeerType peerType = PeerType.None;

        #region Properties
        public static MultiplayerManager Instance { get => instance; }
        #endregion

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void StartButtons()
        {
            if (GUILayout.Button("Server"))
            {
                StartConnection(PeerType.Server);
            }

            if (GUILayout.Button("Host"))
            {
                StartConnection(PeerType.Host);
            }

            if (GUILayout.Button("Client"))
            {
                StartConnection(PeerType.Client);
            }
        }

        private void StartConnection(PeerType p_peerType)
        {
            switch (p_peerType)
            {
                case PeerType.Server:
                    peerType = PeerType.Server;
                    SetupServer();
                    Singleton.StartServer();
                    break;

                case PeerType.Host:
                    peerType = PeerType.Host;
                    SetConnectionData();
                    Singleton.StartHost();
                    break;

                case PeerType.Client:
                    peerType = PeerType.Client;
                    SetConnectionData();
                    Singleton.StartClient();
                    break;
            }
        }

        private void SetConnectionData()
        {
            Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(Application.version); // Just a test.
        }

        private void SetupServer()
        {
            Singleton.ConnectionApprovalCallback += OnConnectionApproval;
            Singleton.OnClientConnectedCallback += OnClientConnected;
            Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            //Singleton.OnServerStarted += OnServerStarted;
        }

        private void OnConnectionApproval(ConnectionApprovalRequest p_request, ConnectionApprovalResponse p_response)
        {
            Debug.Log("Client: " + p_request.ClientNetworkId + " wants to connect.");
            Debug.Log("Version: " + Encoding.ASCII.GetString(p_request.Payload));
            p_response.CreatePlayerObject = false;
            p_response.Approved = true;
        }

        //private void OnServerStarted()
        //{

        //}

        private void OnClientDisconnected(ulong p_playerID)
        {
            if (Singleton.ConnectedClients.TryGetValue(p_playerID, out NetworkClient _client))
            {
                Debug.Log("Client disconected: " + _client.ClientId);
                clients.Remove(clients.Find(x => x.ClientId == p_playerID));
            }
        }

        private void OnClientConnected(ulong p_playerID)
        {
            if (Singleton.ConnectedClients.TryGetValue(p_playerID, out NetworkClient _client))
            {
                Debug.Log("New client connected: " + _client.ClientId);
                clients.Add(_client);
            }
        }

        private void StatusLabels()
        {
            peerType = Singleton.IsHost ? PeerType.Host : Singleton.IsServer ? PeerType.Server : PeerType.Client;

            GUILayout.Label("Transport: " + Singleton.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + peerType);

            if (Singleton.IsServer)
            {
                GUILayout.Label("Players connected: " + clients.Count);
            }
            else if (Singleton.IsClient)
            {
                if (GUILayout.Button("Request spawn player"))
                {
                    SpawnPlayerRequestServerRpc();
                }
            }
        }

        [ServerRpc]
        private void SpawnPlayerRequestServerRpc(ServerRpcParams rpcParams = default)
        {
            Debug.Log("Here."); // Test
        }

        //private void SubmitNewPosition()
        //{
        //    if (GUILayout.Button(NetworkManager.Singleton.IsServer ? "Move" : "Request Position Change"))
        //    {
        //        if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
        //        {
        //            foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
        //            {
        //                NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<Player>().Move();
        //            }
        //        }
        //        else
        //        {
        //            NetworkObject _playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        //            Player _player = _playerObject.GetComponent<Player>();
        //        }
        //    }
        //}

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));

            if (!Singleton.IsClient && !Singleton.IsServer)
            {
                StartButtons();
            }
            else
            {
                StatusLabels();
                //SubmitNewPosition();
            }

            GUILayout.EndArea();
        }
    }
}