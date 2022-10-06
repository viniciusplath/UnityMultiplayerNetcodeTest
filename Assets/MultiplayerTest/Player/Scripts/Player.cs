using Unity.Netcode;
using UnityEngine;

namespace MultiplayerTest
{
    public class Player : NetworkBehaviour
    {
        [Header("Configurations")]
        [SerializeField] private float moveSpeed = 1;

        private NetworkVariable<Vector3> moveInput = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

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