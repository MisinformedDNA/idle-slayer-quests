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
}

public enum QuestType
{
    Kill,
    Collect,
    Upgrade,
    Prestige,
    TimeBasedGathering,
    Special
}

public enum QuestDifficulty
{
    Easy,
    Medium,
    Hard,
    Extreme
}

public class QuestRewards
{
    public long Coins { get; set; }
    public long Souls { get; set; }
    public long CelestialPoints { get; set; }
    public int Divinities { get; set; }
    public List<string> Items { get; set; } = new();
}

public class QuestRequirement
{
    public string Type { get; set; } = string.Empty;
    public long Value { get; set; }
    public string Description { get; set; } = string.Empty;
}