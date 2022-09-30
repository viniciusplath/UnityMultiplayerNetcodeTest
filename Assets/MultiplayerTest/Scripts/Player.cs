using Unity.Netcode;
using UnityEngine;

namespace MultiplayerTest
{
    public class Player : NetworkBehaviour
    {
        [Header("Configurations")]
        [SerializeField] private float moveSpeed = 1;

        private NetworkVariable<Vector3> moveInput = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        //public override void OnNetworkSpawn()
        //{
        //    if (IsOwner)
        //    {
        //        Move();
        //    }
        //}

        //public void Move()
        //{
        //    if (NetworkManager.Singleton.IsServer)
        //    {
        //        var randomPosition = GetRandomPositionOnPlane();
        //        transform.position = randomPosition;
        //        Position.Value = randomPosition;
        //    }
        //    else
        //    {
        //        SubmitPositionRequestServerRpc();
        //    }
        //}

        //[ServerRpc]
        //void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
        //{
        //    Position.Value = GetRandomPositionOnPlane();
        //}

        //static Vector3 GetRandomPositionOnPlane()
        //{
        //    return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
        //}

        private void Update()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                transform.position += moveSpeed * Time.deltaTime * moveInput.Value;
            }

            if (IsOwner)
            {
                ReadInputs();
            }
        }

        private void ReadInputs()
        {
            moveInput.Value = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        }
    }
}