using System.Collections.Generic;
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

    public class MultiplayerManager : MonoBehaviour
    {
        private List<Player> players = new();
        private PeerType peerType = PeerType.None;

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));

            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
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
                    NetworkManager.Singleton.StartServer();
                    break;

                case PeerType.Host:
                    peerType = PeerType.Host;
                    NetworkManager.Singleton.StartHost();
                    break;

                case PeerType.Client:
                    peerType = PeerType.Client;
                    NetworkManager.Singleton.StartClient();
                    break;
            }
        }

        private void SetupServer()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        }

        private void OnServerStarted()
        {

        }

        private void OnClientDisconnected(ulong obj)
        {
    
        }

        private void OnClientConnected(ulong obj)
        {

        }

        private void StatusLabels()
        {
            peerType = NetworkManager.Singleton.IsHost ? PeerType.Host : NetworkManager.Singleton.IsServer ? PeerType.Server : PeerType.Client;

            GUILayout.Label("Transport: " + NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + peerType);
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
    }
}