namespace IdleSlayerQuests.Models;

/// <summary>
/// Represents a quest that can be completed for rewards
/// </summary>
public class Quest
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public QuestType Type { get; set; }
    public QuestDifficulty Difficulty { get; set; }
    public TimeSpan EstimatedDuration { get; set; }
    public QuestRewards Rewards { get; set; } = new();
    public List<QuestRequirement> Requirements { get; set; } = new();
    public int Priority { get; set; } = 0;
    
    // New properties based on Idle Slayer wiki
    public string Bundle { get; set; } = string.Empty; // Training, Rookie, Ancient, etc.
    public QuestCategory Category { get; set; } = QuestCategory.Active; // A/P/W/UA
    public string RecommendedDimension { get; set; } = string.Empty; // Hills, Modern City, etc.
    public QuestObjective Objective { get; set; } = new();
    public int UltraAscensionRequirement { get; set; } = 0; // 0 = no UA required
}

public enum QuestType
{
    Kill,
    Collect,
    Upgrade,
    Prestige,
    TimeBasedGathering,
    Special,
    Unlock
}

public enum QuestDifficulty
{
    Easy,
    Medium,
    Hard,
    Extreme
}

public enum QuestCategory
{
    Active,    // A: Requires active play
    Passive,   // P: Can be completed while idle
    Wherever,  // W: Can be completed in any dimension
    UltraAscension // UA: Requires Ultra Ascension
}

public class QuestObjective
{
    public string Type { get; set; } = string.Empty; // Kill, Collect, etc.
    public int Count { get; set; } = 0;
    public string Target { get; set; } = string.Empty; // Enemy type, item, etc.
    public string Description { get; set; } = string.Empty;
}

public class QuestRewards
{
    public long Coins { get; set; }
    public long Souls { get; set; }
    public long CelestialPoints { get; set; }
    public int Divinities { get; set; }
    public List<string> Items { get; set; } = new();
    public string Description { get; set; } = string.Empty; // For non-numeric rewards like unlocks
    public double CpsBonus { get; set; } = 0; // Bonus coins per second percentage
    public double SoulsBonus { get; set; } = 0; // Bonus souls percentage
}

public class QuestRequirement
{
    public string Type { get; set; } = string.Empty;
    public long Value { get; set; }
    public string Description { get; set; } = string.Empty;
}