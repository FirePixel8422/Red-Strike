using Fire_Pixel.Utility;
using UnityEngine;
using Unity.Netcode;


namespace Fire_Pixel.Networking
{
    public class MatchManager : SmartNetworkBehaviour
    {
        public static MatchManager Instance { get; private set; }
        private void Awake() => Instance = this;


#pragma warning disable UDR0001
        public static OneTimeAction StartMatch_OnServer = new OneTimeAction();
#pragma warning restore UDR0001
        [SerializeField] private int playerReadyCount;



        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            UpdateScheduler.EnableNetworkTickEvents();
        }
        protected override void OnNetworkSystemsSetup()
        {
            MarkPlayerReady_ServerRPC();
        }

        [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
        private void MarkPlayerReady_ServerRPC()
        {
            playerReadyCount += 1;
            if (playerReadyCount == GlobalGameData.MAX_PLAYERS)
            {
                StartMatch_OnServer?.Invoke();
            }
        }
    }
}