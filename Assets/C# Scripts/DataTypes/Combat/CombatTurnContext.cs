using Fire_Pixel.Networking;


/// <summary>
/// Container class uswd for handling attacking and defending player tracking.
/// </summary>
[System.Serializable]
public class CombatTurnContext
{
    public PlayerStats[] Players { get; private set; }

    public int AttackerGameId => TurnManager.Instance.ClientOnTurnId;
    public PlayerStats Attacker => Players[AttackerGameId];
    public PlayerStats Defender => Players[AttackerGameId == 0 ? 1 : 0];


    public CombatTurnContext(PlayerStats[] players)
    {
        Players = players;
    }
}