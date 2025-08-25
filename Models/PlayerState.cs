namespace IdleSlayerQuests.Models;

/// <summary>
/// Represents the current state and capabilities of the player
/// </summary>
public class PlayerState
{
    public int Level { get; set; }
    public long Coins { get; set; }
    public long Souls { get; set; }
    public long CelestialPoints { get; set; }
    public int Divinities { get; set; }
    public double SoulsPerSecond { get; set; }
    public double CoinsPerSecond { get; set; }
    public TimeSpan AvailableTime { get; set; }
    public List<string> UnlockedFeatures { get; set; } = new();
    public Dictionary<string, int> Statistics { get; set; } = new();
    public PlayerPreferences Preferences { get; set; } = new();
    
    // Idle Slayer specific properties
    public int UltraAscensions { get; set; } = 0;
    public string CurrentDimension { get; set; } = "Hills";
    public List<string> UnlockedDimensions { get; set; } = new() { "Hills" };
    public Dictionary<string, int> EnemyKills { get; set; } = new(); // Track total enemy kill counts
    public Dictionary<int, QuestProgress> QuestProgresses { get; set; } = new(); // Track quest progress by quest ID
    public List<int> ActiveQuestIds { get; set; } = new(); // Currently active quest IDs
}

public class PlayerPreferences
{
    public QuestStrategy PreferredStrategy { get; set; } = QuestStrategy.Balanced;
    public bool PrioritizeShortQuests { get; set; } = false;
    public bool AutoSelectQuests { get; set; } = true;
    public List<QuestType> PreferredQuestTypes { get; set; } = new();
    public List<QuestType> AvoidedQuestTypes { get; set; } = new();
}

public enum QuestStrategy
{
    MaxEfficiency,      // Prioritize quests with highest reward/time ratio
    Balanced,          // Balance between efficiency and completion time
    QuickCompletion,   // Prioritize quests that can be completed fastest
    HighReward,        // Prioritize quests with highest absolute rewards
    Progressive        // Prioritize quests that help with long-term progression
}