using Fire_Pixel.Utility;
using Unity.Netcode;


namespace Fire_Pixel.Networking
{
    public class MatchManager : SmartNetworkBehaviour
    {
        public static MatchManager Instance { get; private set; }
        private void Awake()
        {
            Instance = this;
        }


#pragma warning disable UDR0001
        public static OneTimeAction StartMatch_OnServer = new OneTimeAction();
        private int playerReadyCount;
#pragma warning restore UDR0001


        protected override void OnNetworkSystemsSetup()
        {
            NetworkManager.SceneManager.OnSynchronizeComplete += ClientLoadedNetworkScene_ServerCallback;
        }
        private void ClientLoadedNetworkScene_ServerCallback(ulong clientId)
        {
            playerReadyCount += 1;
            if (playerReadyCount == GlobalGameData.MaxPlayers)
            {
                StartMatch_OnServer?.Invoke();
                NetworkManager.Singleton.SceneManager.OnSynchronizeComplete -= ClientLoadedNetworkScene_ServerCallback;

                DebugLogger.Log("Game Ready");
            }
        }
    }
}