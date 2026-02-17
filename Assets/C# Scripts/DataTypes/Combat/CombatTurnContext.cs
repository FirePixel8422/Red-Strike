using Fire_Pixel.Networking;


/// <summary>
/// Container class uswd for handling attacking and defending player tracking.
/// </summary>
[System.Serializable]
public static class CombatTurnContext
{
    public static PlayerStats[] Players { get; private set; }

    public static int AttackerGameId => TurnManager.ClientOnTurnId;
    public static PlayerStats Attacker => Players[AttackerGameId];
    public static PlayerStats Defender => Players[AttackerGameId == 0 ? 1 : 0];


    public static void Init(PlayerStats[] players)
    {
        Players = players;
    }
}