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
        _quests = GenerateRealIdleSlayerQuests();
    }
    
    public List<Quest> GetAvailableQuests()
    {
        return _quests.ToList();
    }
    
    public List<Quest> GetQuestsForPlayer(PlayerState player)
    {
        return _quests.Where(quest => CanPlayerCompleteQuest(player, quest))
                     .Where(quest => !IsQuestCompleted(player, quest))
                     .ToList();
    }
    
    public bool IsQuestCompleted(PlayerState player, Quest quest)
    {
        return player.QuestProgresses.ContainsKey(quest.Id) && 
               player.QuestProgresses[quest.Id].Status == QuestStatus.Completed;
    }
    
    public bool IsQuestActive(PlayerState player, Quest quest)
    {
        return player.ActiveQuestIds.Contains(quest.Id);
    }
    
    public void StartQuest(PlayerState player, Quest quest)
    {
        if (!CanPlayerCompleteQuest(player, quest) || IsQuestCompleted(player, quest))
            return;
            
        // Initialize quest progress
        var progress = new QuestProgress
        {
            QuestId = quest.Id,
            Status = QuestStatus.Active,
            StartTime = DateTime.Now,
            ProgressAtStart = new Dictionary<string, int>(player.EnemyKills) // Snapshot current kill counts
        };
        
        player.QuestProgresses[quest.Id] = progress;
        if (!player.ActiveQuestIds.Contains(quest.Id))
        {
            player.ActiveQuestIds.Add(quest.Id);
        }
    }
    
    public void UpdateQuestProgress(PlayerState player, Quest quest)
    {
        if (!IsQuestActive(player, quest))
            return;
            
        var progress = player.QuestProgresses[quest.Id];
        
        // Calculate kills since quest started
        if (quest.Objective.Type.ToLower() == "kill")
        {
            var targetType = quest.Objective.Target.ToLower();
            var currentKills = player.EnemyKills.ContainsKey(targetType) ? player.EnemyKills[targetType] : 0;
            var killsAtStart = progress.ProgressAtStart.ContainsKey(targetType) ? progress.ProgressAtStart[targetType] : 0;
            var killsSinceStart = Math.Max(0, currentKills - killsAtStart);
            
            progress.Progress[targetType] = killsSinceStart;
            
            // Check if quest is completed
            if (killsSinceStart >= quest.Objective.Count)
            {
                CompleteQuest(player, quest);
            }
        }
    }
    
    public void CompleteQuest(PlayerState player, Quest quest)
    {
        if (player.QuestProgresses.ContainsKey(quest.Id))
        {
            player.QuestProgresses[quest.Id].Status = QuestStatus.Completed;
            player.QuestProgresses[quest.Id].CompletionTime = DateTime.Now;
            player.ActiveQuestIds.Remove(quest.Id);
        }
    }
    
    public int GetQuestProgress(PlayerState player, Quest quest)
    {
        if (!player.QuestProgresses.ContainsKey(quest.Id))
            return 0;
            
        var progress = player.QuestProgresses[quest.Id];
        if (quest.Objective.Type.ToLower() == "kill")
        {
            var targetType = quest.Objective.Target.ToLower();
            return progress.Progress.ContainsKey(targetType) ? progress.Progress[targetType] : 0;
        }
        
        return 0;
    }
    
    public bool CanPlayerCompleteQuest(PlayerState player, Quest quest)
    {
        // Check Ultra Ascension requirement first
        if (quest.UltraAscensionRequirement > 0 && player.UltraAscensions < quest.UltraAscensionRequirement)
        {
            return false;
        }
        
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
                case "ultraascension":
                    if (player.UltraAscensions < requirement.Value) return false;
                    break;
                case "feature":
                    if (!player.UnlockedFeatures.Contains(requirement.Description)) return false;
                    break;
            }
        }
        
        // Check if player has access to the recommended dimension
        if (!string.IsNullOrEmpty(quest.RecommendedDimension) && 
            !player.UnlockedDimensions.Contains(quest.RecommendedDimension))
        {
            return false;
        }
        
        return true;
    }
    
    private List<Quest> GenerateRealIdleSlayerQuests()
    {
        return new List<Quest>
        {
            // Training Quests (Early game)
            new Quest
            {
                Id = 1,
                Name = "Test Your Might",
                Description = "Kill 8 flying enemies",
                Type = QuestType.Kill,
                Difficulty = QuestDifficulty.Easy,
                Bundle = "Training",
                Category = QuestCategory.Active,
                RecommendedDimension = "Hills",
                EstimatedDuration = TimeSpan.FromMinutes(5),
                Objective = new QuestObjective { Type = "Kill", Count = 8, Target = "flying enemies", Description = "Kill 8 flying enemies." },
                Rewards = new QuestRewards { Coins = 7000, Description = "Training reward" },
                Requirements = new List<QuestRequirement>()
            },
            
            new Quest
            {
                Id = 2,
                Name = "Beekeeper",
                Description = "Kill 50 Wasps",
                Type = QuestType.Kill,
                Difficulty = QuestDifficulty.Easy,
                Bundle = "Training",
                Category = QuestCategory.Active,
                RecommendedDimension = "Hills",
                EstimatedDuration = TimeSpan.FromMinutes(10),
                Objective = new QuestObjective { Type = "Kill", Count = 50, Target = "Wasps", Description = "Kill 50 Wasps." },
                Rewards = new QuestRewards { Description = "Honey: Gain 3% bonus CpS", CpsBonus = 3 },
                Requirements = new List<QuestRequirement>()
            },
            
            new Quest
            {
                Id = 3,
                Name = "Gaius Confrontation", 
                Description = "Kill 2 Giants",
                Type = QuestType.Kill,
                Difficulty = QuestDifficulty.Medium,
                Bundle = "Training",
                Category = QuestCategory.Active,
                RecommendedDimension = "Hills",
                EstimatedDuration = TimeSpan.FromMinutes(15),
                Objective = new QuestObjective { Type = "Kill", Count = 2, Target = "Giants", Description = "Kill 2 Giants." },
                Rewards = new QuestRewards { Description = "Souls Flask: Unlocks the Souls Flask to keep the enemy souls safe" },
                Requirements = new List<QuestRequirement> 
                { 
                    new() { Type = "Level", Value = 15, Description = "Must be level 15+" }
                }
            },
            
            // Rookie Quests
            new Quest
            {
                Id = 4,
                Name = "The Winner Takes All",
                Description = "Kill 125 Wasps",
                Type = QuestType.Kill,
                Difficulty = QuestDifficulty.Easy,
                Bundle = "Rookie",
                Category = QuestCategory.Active,
                RecommendedDimension = "Hills",
                EstimatedDuration = TimeSpan.FromMinutes(20),
                Objective = new QuestObjective { Type = "Kill", Count = 125, Target = "Wasps", Description = "Kill 125 Wasps." },
                Rewards = new QuestRewards { Description = "Novice Quests: Unlock new Quests" },
                Requirements = new List<QuestRequirement> 
                { 
                    new() { Type = "Level", Value = 20, Description = "Must be level 20+" }
                }
            },
            
            new Quest
            {
                Id = 5,
                Name = "Worm Nest",
                Description = "Kill 80 Worms",
                Type = QuestType.Kill,
                Difficulty = QuestDifficulty.Easy,
                Bundle = "Rookie", 
                Category = QuestCategory.Active,
                RecommendedDimension = "Hills",
                EstimatedDuration = TimeSpan.FromMinutes(15),
                Objective = new QuestObjective { Type = "Kill", Count = 80, Target = "Worms", Description = "Kill 80 Worms." },
                Rewards = new QuestRewards { Description = "Worm Jelly Potion: Gain 3% bonus CpS", CpsBonus = 3 },
                Requirements = new List<QuestRequirement>()
            },
            
            new Quest
            {
                Id = 6,
                Name = "Clearing The Skies",
                Description = "Kill 500 flying enemies",
                Type = QuestType.Kill,
                Difficulty = QuestDifficulty.Medium,
                Bundle = "Rookie",
                Category = QuestCategory.Active,
                RecommendedDimension = "Modern City",
                EstimatedDuration = TimeSpan.FromMinutes(30),
                Objective = new QuestObjective { Type = "Kill", Count = 500, Target = "flying enemies", Description = "Kill 500 flying enemies." },
                Rewards = new QuestRewards { Description = "Blessing of Hades: Gain 10% Souls bonus", SoulsBonus = 10 },
                Requirements = new List<QuestRequirement> 
                { 
                    new() { Type = "Level", Value = 25, Description = "Must be level 25+" },
                    new() { Type = "feature", Value = 0, Description = "Modern City unlocked" }
                }
            },
            
            // Novice Quests
            new Quest
            {
                Id = 7,
                Name = "The Thrill Of One More Kill",
                Description = "Kill 350 Jellies",
                Type = QuestType.Kill,
                Difficulty = QuestDifficulty.Medium,
                Bundle = "Novice",
                Category = QuestCategory.Active,
                RecommendedDimension = "Modern City",
                EstimatedDuration = TimeSpan.FromMinutes(25),
                Objective = new QuestObjective { Type = "Kill", Count = 350, Target = "Jellies", Description = "Kill 350 Jellies." },
                Rewards = new QuestRewards { Description = "Beginner Quests: Unlock new Quests" },
                Requirements = new List<QuestRequirement> 
                { 
                    new() { Type = "Level", Value = 30, Description = "Must be level 30+" }
                }
            },
            
            new Quest
            {
                Id = 8,
                Name = "Wasps Domination",
                Description = "Kill 350 Wasps",
                Type = QuestType.Kill,
                Difficulty = QuestDifficulty.Medium,
                Bundle = "Novice",
                Category = QuestCategory.Active,
                RecommendedDimension = "Modern City",
                EstimatedDuration = TimeSpan.FromMinutes(25),
                Objective = new QuestObjective { Type = "Kill", Count = 350, Target = "Wasps", Description = "Kill 350 Wasps." },
                Rewards = new QuestRewards { Description = "Guardian Angel: Increase CpS while the game is closed by 10%", CpsBonus = 10 },
                Requirements = new List<QuestRequirement> 
                { 
                    new() { Type = "Level", Value = 30, Description = "Must be level 30+" }
                }
            },
            
            new Quest
            {
                Id = 9,
                Name = "Worm Domination",
                Description = "Kill 500 Worms",
                Type = QuestType.Kill,
                Difficulty = QuestDifficulty.Medium,
                Bundle = "Novice",
                Category = QuestCategory.Active,
                RecommendedDimension = "Modern City",
                EstimatedDuration = TimeSpan.FromMinutes(30),
                Objective = new QuestObjective { Type = "Kill", Count = 500, Target = "Worms", Description = "Kill 500 Worms." },
                Rewards = new QuestRewards { Description = "Soul Splitter: Increase the amount of Souls received for slaying an enemy ingame by 40%", SoulsBonus = 40 },
                Requirements = new List<QuestRequirement> 
                { 
                    new() { Type = "Level", Value = 35, Description = "Must be level 35+" }
                }
            },
            
            // Higher level quests
            new Quest
            {
                Id = 10,
                Name = "Worm Slaughter",
                Description = "Kill 2,000 Worms",
                Type = QuestType.Kill,
                Difficulty = QuestDifficulty.Hard,
                Bundle = "Advanced",
                Category = QuestCategory.Active,
                RecommendedDimension = "Modern City",
                EstimatedDuration = TimeSpan.FromHours(1),
                Objective = new QuestObjective { Type = "Kill", Count = 2000, Target = "Worms", Description = "Kill 2,000 Worms." },
                Rewards = new QuestRewards { Description = "Need For Kill: Increase the chance of spawning enemies by 30%" },
                Requirements = new List<QuestRequirement> 
                { 
                    new() { Type = "Level", Value = 50, Description = "Must be level 50+" }
                }
            },
            
            new Quest
            {
                Id = 11,
                Name = "Demon Eye",
                Description = "Kill 250 Demons", 
                Type = QuestType.Kill,
                Difficulty = QuestDifficulty.Hard,
                Bundle = "Advanced",
                Category = QuestCategory.Active,
                RecommendedDimension = "Mystic Valley",
                EstimatedDuration = TimeSpan.FromMinutes(45),
                Objective = new QuestObjective { Type = "Kill", Count = 250, Target = "Demons", Description = "Kill 250 Demons." },
                Rewards = new QuestRewards { Description = "Black Secret Potion: Gain 10% bonus CpS", CpsBonus = 10 },
                Requirements = new List<QuestRequirement> 
                { 
                    new() { Type = "Level", Value = 60, Description = "Must be level 60+" },
                    new() { Type = "feature", Value = 0, Description = "Mystic Valley unlocked" }
                }
            },
            
            // Ultra Ascension Quests
            new Quest
            {
                Id = 12,
                Name = "Ancient Power",
                Description = "Complete an Ancient Quest bundle",
                Type = QuestType.Special,
                Difficulty = QuestDifficulty.Extreme,
                Bundle = "Ancient",
                Category = QuestCategory.UltraAscension,
                RecommendedDimension = "Hills",
                EstimatedDuration = TimeSpan.FromHours(2),
                UltraAscensionRequirement = 1,
                Objective = new QuestObjective { Type = "Special", Count = 1, Target = "Quest Bundle", Description = "Complete Ancient Quest requirements." },
                Rewards = new QuestRewards { CelestialPoints = 100, Description = "Unlock Ancient powers" },
                Requirements = new List<QuestRequirement> 
                { 
                    new() { Type = "UltraAscension", Value = 1, Description = "Must have completed 1 Ultra Ascension" }
                }
            }
        };
    }
}