using Fire_Pixel.Utility;
using UnityEngine;
using Unity.Netcode;


namespace Fire_Pixel.Networking
{
    public class MatchManager : SmartNetworkBehaviour
    {
#pragma warning disable UDR0001
        public static OneTimeAction StartMatch_OnServer = new OneTimeAction();
#pragma warning restore UDR0001

        [SerializeField] private int playerReadyCount;



        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            UpdateScheduler.EnableNetworkTickEvents();
        }
        protected override void OnNetworkSystemsSetupPostStart()
        {
            MarkPlayerReady_ServerRPC();
        }

        [ContextMenu("Ready")]
        [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
        public void MarkPlayerReady_ServerRPC()
        {
            playerReadyCount += 1;
            if (playerReadyCount == GlobalGameData.MAX_PLAYERS)
            {
                StartMatch_OnServer?.Invoke(); 
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            StartMatch_OnServer = new OneTimeAction();
        }
    }
}