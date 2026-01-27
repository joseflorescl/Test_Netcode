using Unity.Netcode;
using UnityEngine;

namespace HelloWorld
{
    public class HelloWorldPlayer : NetworkBehaviour
    {
        [SerializeField] Transform spawnedObjectPrefab;

        NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();

        string testServerRpc;

        public override void OnNetworkSpawn()
        {
            Position.OnValueChanged += OnStateChanged;

            if (IsOwner)
            {
                Move();
            }

            if (IsServer)
            {
                testServerRpc = "Is Server";
            }
            else
            {
                testServerRpc = "Is NOT Server";
            }

            print($"OnNetworkSpawn: {testServerRpc}");
        }

        public override void OnNetworkDespawn()
        {
            Position.OnValueChanged -= OnStateChanged;
        }

        public void OnStateChanged(Vector3 previous, Vector3 current)
        {
            // note: `Position.Value` will be equal to `current` here
            if (Position.Value != previous)
            {
                transform.position = Position.Value;
            }
        }

        public void Move()
        {
            SubmitPositionRequestServerRpc();
        }

        [Rpc(SendTo.Server)]
        void SubmitPositionRequestServerRpc(RpcParams rpcParams = default)
        {
            var randomPosition = GetRandomPositionOnPlane();
            transform.position = randomPosition; //TODO: test comentando esta línea.
            Position.Value = randomPosition;
        }

        static Vector3 GetRandomPositionOnPlane()
        {
            return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
        }

        private void Update()
        {
            if (!IsOwner)
                return;


            if (Input.GetKeyDown(KeyCode.Space))
            {
                //TestClientRpc();
                //TestServerRpc(testServerRpc);
                var spawned = Instantiate(spawnedObjectPrefab);
                spawned.GetComponent<NetworkObject>().Spawn(true);


            }
        }

        [Rpc(SendTo.Server)]
        //[ServerRpc]
        void TestServerRpc(string message)
        {
            //print($"TestServerRpc. {testServerRpc}. OwnerClientId: {OwnerClientId}.");
            print($"TestServerRpc. {testServerRpc}. message: {message}");
        }

        [Rpc(SendTo.ClientsAndHost)]
        void TestClientRpc()
        {
            print($"TestClientRpc. {testServerRpc}. OwnerClientId: {OwnerClientId}.");
        }

    }
}