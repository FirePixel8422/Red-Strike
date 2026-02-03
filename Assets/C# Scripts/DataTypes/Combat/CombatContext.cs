using FirePixel.Networking;


/// <summary>
/// Container class uswd for handling attacking and defending player tracking.
/// </summary>
[System.Serializable]
public class CombatContext
{
    private readonly PlayerStats[] playerStats;

    public int AttackerGameId => TurnManager.ClientOnTurnId;
    public PlayerStats Attacker => playerStats[AttackerGameId];
    public PlayerStats Defender => playerStats[AttackerGameId == 0 ? 1 : 0];


    public CombatContext(PlayerStats[] playerStats)
    {
        this.playerStats = playerStats;
    }
}