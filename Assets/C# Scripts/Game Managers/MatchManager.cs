using Fire_Pixel.Utility;
using UnityEngine;
using Unity.Netcode;
using TMPro;


namespace Fire_Pixel.Networking
{
    public class MatchManager : SmartNetworkBehaviour
    {
        public static MatchManager Instance { get; private set; }


#pragma warning disable UDR0001
        public static OneTimeAction PostMatchStarted_OnServer = new OneTimeAction();
        public static OneTimeAction PostMatchStarted = new OneTimeAction();
#pragma warning restore UDR0001


        [SerializeField] private GameObject gameOverScreenObj;
        [SerializeField] private TextMeshProUGUI gameOverText;

        private int playerReadyCount;



        private void Awake()
        {
            Instance = this;
        }
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            CallbackScheduler.EnableNetworkTickEvents();
        }
        protected override void OnNetworkSystemsSetupPostStart()
        {
            TurnManager.TurnChanged += OnTurnChanged;
            MarkPlayerReady_ServerRPC();
        }

        [ContextMenu("Ready")]
        [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
        public void MarkPlayerReady_ServerRPC()
        {
            playerReadyCount += 1;
            if (playerReadyCount == GlobalGameData.MAX_PLAYERS)
            {
                PostMatchStarted_OnServer?.Invoke();
            }
        }
        private void OnTurnChanged(int clientGameId)
        {
            TurnManager.TurnChanged -= OnTurnChanged;
            PostMatchStarted.Invoke();
        }

        [Rpc(SendTo.ClientsAndHost, Delivery = RpcDelivery.Reliable)]
        public void EndGame_RPC(int winnerClientGameId)
        {
            string winnerClientName = ClientManager.GetPlayerName(winnerClientGameId);

            gameOverScreenObj.SetActive(true);
            gameOverText.text = winnerClientName + " Won!";

            if (IsServer)
            {
                this.Invoke(5, ClientManager.Instance.ShutDownNetwork_ServerRPC);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            TurnManager.TurnChanged -= OnTurnChanged;
            PostMatchStarted_OnServer = new OneTimeAction();
            PostMatchStarted = new OneTimeAction();
        }
    }
}