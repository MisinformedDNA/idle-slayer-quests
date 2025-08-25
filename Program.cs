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
        var player = new PlayerState 
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
        
        // Simulate some kill counts to demonstrate quest progress tracking
        player.EnemyKills = new Dictionary<string, int>
        {
            { "flying enemies", 5 },
            { "wasps", 25 },
            { "worms", 10 }
        };
        
        return player;
    }
    
    private static PlayerState CreateMidGamePlayer()
    {
        var player = new PlayerState 
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
        
        // Simulate more kill counts for mid-game player
        player.EnemyKills = new Dictionary<string, int>
        {
            { "flying enemies", 200 },
            { "wasps", 150 },
            { "worms", 300 },
            { "jellies", 50 }
        };
        
        return player;
    }
    
    private static PlayerState CreateLateGamePlayer()
    {
        var player = new PlayerState 
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
        
        // Simulate extensive kill counts for late-game player
        player.EnemyKills = new Dictionary<string, int>
        {
            { "flying enemies", 1500 },
            { "wasps", 800 },
            { "worms", 2500 },
            { "jellies", 400 },
            { "demons", 100 }
        };
        
        return player;
    }
    
    private static void TestPlayerProgression(string playerType, PlayerState player, QuestService questService, QuestStrategyService strategyService)
    {
        Console.WriteLine($"=== {playerType} ===");
        Console.WriteLine($"Level: {player.Level}, Coins: {player.Coins:N0}, Souls: {player.Souls:N0}");
        Console.WriteLine($"Ultra Ascensions: {player.UltraAscensions}, Current Dimension: {player.CurrentDimension}");
        
        // Show current kill counts
        Console.WriteLine("Current Kill Counts:");
        foreach (var killCount in player.EnemyKills.Take(5))
        {
            Console.WriteLine($"  - {killCount.Key}: {killCount.Value}");
        }
        
        // Get eligible quests for this player
        var eligibleQuests = questService.GetQuestsForPlayer(player);
        Console.WriteLine($"\nEligible Quests: {eligibleQuests.Count}");
        
        if (eligibleQuests.Any())
        {
            // Show recommended quest completion order
            ShowRecommendedQuestOrder(strategyService, eligibleQuests, player);
            
            // Show wiki-based order for comparison
            ShowWikiRecommendedOrder(strategyService, eligibleQuests, player);
            
            // Demonstrate quest progress tracking
            DemonstrateQuestProgressTracking(questService, eligibleQuests.First(), player);
        }
        else
        {
            Console.WriteLine("No eligible quests found for this player level.");
        }
    }
    
    private static void ShowRecommendedQuestOrder(QuestStrategyService strategyService, List<Quest> quests, PlayerState player)
    {
        Console.WriteLine("\n--- RECOMMENDED QUEST COMPLETION ORDER ---");
        var recommendedOrder = strategyService.GetRecommendedQuestOrder(quests, player);
        
        if (recommendedOrder.Any())
        {
            Console.WriteLine("Complete quests in this order for optimal efficiency:");
            for (int i = 0; i < Math.Min(5, recommendedOrder.Count); i++)
            {
                var quest = recommendedOrder[i];
                var efficiency = strategyService.CalculateQuestEfficiency(quest, player);
                Console.WriteLine($"  {i + 1}. {quest.Name}");
                Console.WriteLine($"     Bundle: {quest.Bundle}, Objective: {quest.Objective.Description}");
                Console.WriteLine($"     Reward: {quest.Rewards.Description}");
                Console.WriteLine($"     Efficiency Score: {efficiency:F2}");
            }
        }
    }
    
    private static void ShowWikiRecommendedOrder(QuestStrategyService strategyService, List<Quest> quests, PlayerState player)
    {
        Console.WriteLine("\n--- WIKI-BASED QUEST ORDER (for comparison) ---");
        var wikiOrder = strategyService.GetWikiRecommendedOrder(quests, player);
        
        if (wikiOrder.Any())
        {
            Console.WriteLine("Wiki progression order:");
            for (int i = 0; i < Math.Min(5, wikiOrder.Count); i++)
            {
                var quest = wikiOrder[i];
                Console.WriteLine($"  {i + 1}. {quest.Name} ({quest.Bundle} Bundle)");
            }
            
            // Check for divergences
            var recommendedOrder = strategyService.GetRecommendedQuestOrder(quests, player);
            if (recommendedOrder.Any() && wikiOrder.Any())
            {
                var firstRecommended = recommendedOrder.First().Name;
                var firstWiki = wikiOrder.First().Name;
                if (firstRecommended != firstWiki)
                {
                    Console.WriteLine($"🔄 DIVERGENCE: Recommended starts with '{firstRecommended}', Wiki starts with '{firstWiki}'");
                }
                else
                {
                    Console.WriteLine("✅ Both strategies agree on the first quest");
                }
            }
        }
    }
    
    private static void DemonstrateQuestProgressTracking(QuestService questService, Quest quest, PlayerState player)
    {
        Console.WriteLine($"\n--- QUEST PROGRESS TRACKING DEMO ---");
        Console.WriteLine($"Starting quest: {quest.Name}");
        Console.WriteLine($"Objective: {quest.Objective.Description}");
        
        // Show kill counts before starting quest
        var targetType = quest.Objective.Target.ToLower();
        var totalKills = player.EnemyKills.ContainsKey(targetType) ? player.EnemyKills[targetType] : 0;
        Console.WriteLine($"Total {targetType} killed in game: {totalKills}");
        
        // Start the quest
        questService.StartQuest(player, quest);
        Console.WriteLine($"✅ Quest activated. Progress will ONLY count kills from this point forward.");
        
        // Show initial progress (should be 0 since we just started)
        var initialProgress = questService.GetQuestProgress(player, quest);
        Console.WriteLine($"Quest progress: {initialProgress}/{quest.Objective.Count} {quest.Objective.Target}");
        
        // Simulate some gameplay kills
        if (player.EnemyKills.ContainsKey(targetType))
        {
            var killsToAdd = Math.Min(quest.Objective.Count + 2, 5); // Add a few kills
            player.EnemyKills[targetType] += killsToAdd;
            questService.UpdateQuestProgress(player, quest);
            
            var newTotalKills = player.EnemyKills[targetType];
            var questProgress = questService.GetQuestProgress(player, quest);
            
            Console.WriteLine($"🎯 Killed {killsToAdd} more {quest.Objective.Target} during quest");
            Console.WriteLine($"Total {targetType} killed now: {newTotalKills}");
            Console.WriteLine($"Quest progress: {questProgress}/{quest.Objective.Count} (only counting kills since quest started)");
            
            if (questProgress >= quest.Objective.Count)
            {
                Console.WriteLine("🎉 Quest completed!");
            }
            else
            {
                Console.WriteLine($"Need {quest.Objective.Count - questProgress} more {quest.Objective.Target} to complete");
            }
        }
    }
}