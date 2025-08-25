using IdleSlayerQuests.Models;
using IdleSlayerQuests.Services;

namespace IdleSlayerQuests;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Idle Slayer Quest Strategy Evaluator ===");
        Console.WriteLine("Using real quest data from Idle Slayer wiki");
        
        // Initialize services
        var questService = new QuestService();
        var strategyService = new QuestStrategyService();
        
        // Load real Idle Slayer quests
        var availableQuests = questService.GetAvailableQuests();
        
        Console.WriteLine($"\nTotal Available Quests: {availableQuests.Count}");
        Console.WriteLine("\nQuest Bundles Available:");
        var bundles = availableQuests.GroupBy(q => q.Bundle).Select(g => new { Bundle = g.Key, Count = g.Count() });
        foreach (var bundle in bundles.OrderBy(b => b.Bundle))
        {
            Console.WriteLine($"  - {bundle.Bundle}: {bundle.Count} quests");
        }
        
        // Demonstrate with different player progression levels
        Console.WriteLine("\n" + new string('=', 60));
        TestPlayerProgression("Early Game Player", CreateEarlyGamePlayer(), questService, strategyService);
        
        Console.WriteLine("\n" + new string('=', 60));
        TestPlayerProgression("Mid Game Player", CreateMidGamePlayer(), questService, strategyService);
        
        Console.WriteLine("\n" + new string('=', 60));
        TestPlayerProgression("Late Game Player", CreateLateGamePlayer(), questService, strategyService);
    }
    
    private static PlayerState CreateEarlyGamePlayer()
    {
        return new PlayerState 
        { 
            Level = 15, 
            Coins = 50000, 
            Souls = 500,
            SoulsPerSecond = 10,
            AvailableTime = TimeSpan.FromHours(1),
            CurrentDimension = "Hills",
            UnlockedDimensions = new() { "Hills" },
            UltraAscensions = 0,
            UnlockedFeatures = new() { "Basic Quests" }
        };
    }
    
    private static PlayerState CreateMidGamePlayer()
    {
        return new PlayerState 
        { 
            Level = 45, 
            Coins = 2000000, 
            Souls = 15000,
            CelestialPoints = 50,
            SoulsPerSecond = 150,
            AvailableTime = TimeSpan.FromHours(2),
            CurrentDimension = "Modern City",
            UnlockedDimensions = new() { "Hills", "Modern City", "Mystic Valley" },
            UltraAscensions = 0,
            UnlockedFeatures = new() { "Basic Quests", "Modern City unlocked", "Mystic Valley unlocked" }
        };
    }
    
    private static PlayerState CreateLateGamePlayer()
    {
        return new PlayerState 
        { 
            Level = 80, 
            Coins = 50000000, 
            Souls = 100000,
            CelestialPoints = 500,
            Divinities = 10,
            SoulsPerSecond = 500,
            AvailableTime = TimeSpan.FromHours(3),
            CurrentDimension = "Mystic Valley",
            UnlockedDimensions = new() { "Hills", "Modern City", "Mystic Valley", "Hot Desert", "Jungle" },
            UltraAscensions = 2,
            UnlockedFeatures = new() { "Basic Quests", "Modern City unlocked", "Mystic Valley unlocked", "Advanced Features" }
        };
    }
    
    private static void TestPlayerProgression(string playerType, PlayerState player, QuestService questService, QuestStrategyService strategyService)
    {
        Console.WriteLine($"=== {playerType} ===");
        Console.WriteLine($"Level: {player.Level}, Coins: {player.Coins:N0}, Souls: {player.Souls:N0}");
        Console.WriteLine($"Ultra Ascensions: {player.UltraAscensions}, Current Dimension: {player.CurrentDimension}");
        
        // Get eligible quests for this player
        var eligibleQuests = questService.GetQuestsForPlayer(player);
        Console.WriteLine($"Eligible Quests: {eligibleQuests.Count}");
        
        if (eligibleQuests.Any())
        {
            // Test wiki-recommended strategy
            TestStrategy(strategyService, eligibleQuests, player, QuestStrategy.MaxEfficiency, "Wiki-Based Efficiency");
            
            // Show quest details for the top recommendations
            var recommended = strategyService.SelectWikiRecommendedQuests(eligibleQuests, player);
            if (recommended.Any())
            {
                Console.WriteLine($"\nTop Wiki-Recommended Quests:");
                foreach (var quest in recommended.Take(3))
                {
                    var efficiency = strategyService.CalculateQuestEfficiency(quest, player);
                    Console.WriteLine($"  - {quest.Name}");
                    Console.WriteLine($"    Bundle: {quest.Bundle}, Category: {quest.Category}");
                    Console.WriteLine($"    Objective: {quest.Objective.Description}");
                    Console.WriteLine($"    Reward: {quest.Rewards.Description}");
                    Console.WriteLine($"    Dimension: {quest.RecommendedDimension}, Efficiency: {efficiency:F2}");
                }
            }
        }
        else
        {
            Console.WriteLine("No eligible quests found for this player level.");
        }
    }
    
    private static void TestStrategy(QuestStrategyService strategyService, List<Quest> quests, PlayerState player, QuestStrategy strategy, string displayName = null)
    {
        var strategyName = displayName ?? strategy.ToString();
        Console.WriteLine($"\n--- {strategyName} Strategy ---");
        var recommendedQuests = strategyService.SelectOptimalQuests(quests, player, strategy);
        
        if (recommendedQuests.Any())
        {
            Console.WriteLine("Top recommended quests:");
            foreach (var quest in recommendedQuests.Take(3))
            {
                var efficiency = strategyService.CalculateQuestEfficiency(quest, player);
                Console.WriteLine($"  - {quest.Name} (Efficiency: {efficiency:F2}, Bundle: {quest.Bundle})");
            }
        }
        else
        {
            Console.WriteLine("No suitable quests found for this strategy.");
        }
    }
}