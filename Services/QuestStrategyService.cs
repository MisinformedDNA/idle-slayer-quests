using IdleSlayerQuests.Models;

namespace IdleSlayerQuests.Services;

/// <summary>
/// Service responsible for quest strategy determination and optimization
/// Based on Idle Slayer wiki recommendations for efficient quest management
/// </summary>
public class QuestStrategyService
{
    /// <summary>
    /// Selects optimal quests based on the chosen strategy and player state
    /// </summary>
    public List<Quest> SelectOptimalQuests(List<Quest> availableQuests, PlayerState player, QuestStrategy strategy)
    {
        var eligibleQuests = availableQuests
            .Where(quest => CanPlayerCompleteInTime(quest, player))
            .ToList();
            
        if (!eligibleQuests.Any())
            return new List<Quest>();
            
        return strategy switch
        {
            QuestStrategy.MaxEfficiency => SelectByMaxEfficiency(eligibleQuests, player),
            QuestStrategy.Balanced => SelectBalancedQuests(eligibleQuests, player),
            QuestStrategy.QuickCompletion => SelectQuickestQuests(eligibleQuests, player),
            QuestStrategy.HighReward => SelectHighestRewardQuests(eligibleQuests, player),
            QuestStrategy.Progressive => SelectProgressiveQuests(eligibleQuests, player),
            _ => SelectBalancedQuests(eligibleQuests, player)
        };
    }
    
    /// <summary>
    /// Calculates the efficiency score for a quest (reward value per time unit)
    /// Higher scores indicate more efficient quests
    /// </summary>
    public double CalculateQuestEfficiency(Quest quest, PlayerState player)
    {
        var rewardValue = CalculateTotalRewardValue(quest.Rewards, player);
        var timeInMinutes = quest.EstimatedDuration.TotalMinutes;
        
        if (timeInMinutes <= 0) return 0;
        
        var baseEfficiency = rewardValue / timeInMinutes;
        
        // Apply difficulty modifier (easier quests are more reliable)
        var difficultyModifier = quest.Difficulty switch
        {
            QuestDifficulty.Easy => 1.2,
            QuestDifficulty.Medium => 1.0,
            QuestDifficulty.Hard => 0.8,
            QuestDifficulty.Extreme => 0.6,
            _ => 1.0
        };
        
        // Apply priority modifier
        var priorityModifier = quest.Priority switch
        {
            > 0 => 1.0 + (quest.Priority * 0.1),
            < 0 => 1.0 + (quest.Priority * 0.1),
            _ => 1.0
        };
        
        return baseEfficiency * difficultyModifier * priorityModifier;
    }
    
    private bool CanPlayerCompleteInTime(Quest quest, PlayerState player)
    {
        return quest.EstimatedDuration <= player.AvailableTime;
    }
    
    private List<Quest> SelectByMaxEfficiency(List<Quest> quests, PlayerState player)
    {
        return quests
            .OrderByDescending(q => CalculateQuestEfficiency(q, player))
            .Take(GetMaxConcurrentQuests(player))
            .ToList();
    }
    
    private List<Quest> SelectBalancedQuests(List<Quest> quests, PlayerState player)
    {
        var efficiency = quests.Select(q => new { Quest = q, Efficiency = CalculateQuestEfficiency(q, player) });
        var avgEfficiency = efficiency.Average(e => e.Efficiency);
        
        // Select quests with above-average efficiency that aren't too long
        return quests
            .Where(q => 
            {
                var eff = CalculateQuestEfficiency(q, player);
                var timeRatio = q.EstimatedDuration.TotalMinutes / player.AvailableTime.TotalMinutes;
                return eff >= avgEfficiency * 0.7 && timeRatio <= 0.6;
            })
            .OrderByDescending(q => CalculateQuestEfficiency(q, player))
            .Take(GetMaxConcurrentQuests(player))
            .ToList();
    }
    
    private List<Quest> SelectQuickestQuests(List<Quest> quests, PlayerState player)
    {
        return quests
            .OrderBy(q => q.EstimatedDuration)
            .Take(GetMaxConcurrentQuests(player))
            .ToList();
    }
    
    private List<Quest> SelectHighestRewardQuests(List<Quest> quests, PlayerState player)
    {
        return quests
            .OrderByDescending(q => CalculateTotalRewardValue(q.Rewards, player))
            .Take(GetMaxConcurrentQuests(player))
            .ToList();
    }
    
    private List<Quest> SelectProgressiveQuests(List<Quest> quests, PlayerState player)
    {
        // Prioritize quests that unlock features or provide long-term benefits
        var progressiveQuests = quests.Where(q => 
            q.Rewards.Divinities > 0 || 
            q.Rewards.CelestialPoints > 100 || 
            q.Type == QuestType.Upgrade ||
            q.Type == QuestType.Prestige
        ).ToList();
        
        if (progressiveQuests.Any())
        {
            return progressiveQuests
                .OrderByDescending(q => CalculateProgressiveValue(q, player))
                .Take(GetMaxConcurrentQuests(player))
                .ToList();
        }
        
        // Fallback to balanced selection if no progressive quests available
        return SelectBalancedQuests(quests, player);
    }
    
    private double CalculateTotalRewardValue(QuestRewards rewards, PlayerState player)
    {
        // Weight different reward types based on player's current needs
        // These weights should be configurable in a real implementation
        var coinValue = rewards.Coins * 1.0;
        var soulValue = rewards.Souls * 10.0; // Souls are typically more valuable
        var celestialValue = rewards.CelestialPoints * 100.0; // Very valuable late game
        var divinityValue = rewards.Divinities * 10000.0; // Extremely valuable
        
        return coinValue + soulValue + celestialValue + divinityValue;
    }
    
    private double CalculateProgressiveValue(Quest quest, PlayerState player)
    {
        var baseValue = CalculateTotalRewardValue(quest.Rewards, player);
        var progressiveBonus = 0.0;
        
        // Bonus for divinities (major progression items)
        progressiveBonus += quest.Rewards.Divinities * 5000;
        
        // Bonus for celestial points (important for late game)
        progressiveBonus += quest.Rewards.CelestialPoints * 50;
        
        // Bonus for upgrade quests (improve efficiency)
        if (quest.Type == QuestType.Upgrade)
            progressiveBonus += 2000;
            
        return baseValue + progressiveBonus;
    }
    
    private int GetMaxConcurrentQuests(PlayerState player)
    {
        // In a real implementation, this would be based on unlocked quest slots
        // For now, assume players can handle multiple quests based on level
        return player.Level switch
        {
            < 20 => 1,
            < 50 => 2,
            < 100 => 3,
            _ => 4
        };
    }
}