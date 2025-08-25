using IdleSlayerQuests.Models;

namespace IdleSlayerQuests.Services;

/// <summary>
/// Service for managing and providing quest data
/// </summary>
public class QuestService
{
    private readonly List<Quest> _quests;
    
    public QuestService()
    {
        _quests = GenerateSampleQuests();
    }
    
    public List<Quest> GetAvailableQuests()
    {
        return _quests.ToList();
    }
    
    public List<Quest> GetQuestsForPlayer(PlayerState player)
    {
        return _quests.Where(quest => CanPlayerCompleteQuest(player, quest)).ToList();
    }
    
    public bool CanPlayerCompleteQuest(PlayerState player, Quest quest)
    {
        foreach (var requirement in quest.Requirements)
        {
            switch (requirement.Type.ToLower())
            {
                case "level":
                    if (player.Level < requirement.Value) return false;
                    break;
                case "coins":
                    if (player.Coins < requirement.Value) return false;
                    break;
                case "souls":
                    if (player.Souls < requirement.Value) return false;
                    break;
                case "celestialpoints":
                    if (player.CelestialPoints < requirement.Value) return false;
                    break;
                case "feature":
                    if (!player.UnlockedFeatures.Contains(requirement.Description)) return false;
                    break;
            }
        }
        return true;
    }
    
    private List<Quest> GenerateSampleQuests()
    {
        return new List<Quest>
        {
            new Quest
            {
                Id = 1,
                Name = "Slay 100 Goblins",
                Description = "Kill 100 goblins in the forest",
                Type = QuestType.Kill,
                Difficulty = QuestDifficulty.Easy,
                EstimatedDuration = TimeSpan.FromMinutes(15),
                Rewards = new QuestRewards { Coins = 50000, Souls = 100 },
                Requirements = new List<QuestRequirement> 
                { 
                    new() { Type = "Level", Value = 10, Description = "Must be level 10+" }
                }
            },
            new Quest
            {
                Id = 2,
                Name = "Collect 1M Coins",
                Description = "Gather 1 million coins through any means",
                Type = QuestType.Collect,
                Difficulty = QuestDifficulty.Medium,
                EstimatedDuration = TimeSpan.FromMinutes(45),
                Rewards = new QuestRewards { Souls = 500, CelestialPoints = 10 },
                Requirements = new List<QuestRequirement> 
                { 
                    new() { Type = "Level", Value = 25, Description = "Must be level 25+" }
                }
            },
            new Quest
            {
                Id = 3,
                Name = "Upgrade Weapon 5 Times",
                Description = "Perform 5 weapon upgrades",
                Type = QuestType.Upgrade,
                Difficulty = QuestDifficulty.Medium,
                EstimatedDuration = TimeSpan.FromMinutes(30),
                Rewards = new QuestRewards { Coins = 100000, Souls = 300, CelestialPoints = 5 },
                Requirements = new List<QuestRequirement> 
                { 
                    new() { Type = "Coins", Value = 500000, Description = "Need 500k coins for upgrades" }
                }
            },
            new Quest
            {
                Id = 4,
                Name = "Idle for 2 Hours",
                Description = "Let the game idle for 2 hours continuously",
                Type = QuestType.TimeBasedGathering,
                Difficulty = QuestDifficulty.Easy,
                EstimatedDuration = TimeSpan.FromHours(2),
                Rewards = new QuestRewards { Souls = 1000, CelestialPoints = 20 },
                Requirements = new List<QuestRequirement>(),
                Priority = -1 // Lower priority due to time commitment
            },
            new Quest
            {
                Id = 5,
                Name = "Defeat Boss",
                Description = "Defeat the current area boss",
                Type = QuestType.Kill,
                Difficulty = QuestDifficulty.Hard,
                EstimatedDuration = TimeSpan.FromMinutes(60),
                Rewards = new QuestRewards { Coins = 500000, Souls = 2000, CelestialPoints = 50, Divinities = 1 },
                Requirements = new List<QuestRequirement> 
                { 
                    new() { Type = "Level", Value = 40, Description = "Must be level 40+" }
                }
            }
        };
    }
}