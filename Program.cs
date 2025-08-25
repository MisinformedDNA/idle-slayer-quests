using IdleSlayerQuests.Models;
using IdleSlayerQuests.Services;

namespace IdleSlayerQuests;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Idle Slayer Quest Strategy Evaluator ===");
        
        // Initialize services
        var questService = new QuestService();
        var strategyService = new QuestStrategyService();
        
        // Load sample quests
        var availableQuests = questService.GetAvailableQuests();
        
        Console.WriteLine($"\nAvailable Quests: {availableQuests.Count}");
        foreach (var quest in availableQuests)
        {
            Console.WriteLine($"  - {quest.Name}: {quest.Description}");
        }
        
        // Demonstrate different strategies
        var player = new PlayerState 
        { 
            Level = 50, 
            Coins = 1000000, 
            SoulsPerSecond = 100,
            AvailableTime = TimeSpan.FromHours(2)
        };
        
        Console.WriteLine($"\nPlayer State: Level {player.Level}, {player.Coins:N0} coins, {player.SoulsPerSecond} souls/sec");
        
        // Test different strategies
        TestStrategy(strategyService, availableQuests, player, QuestStrategy.MaxEfficiency);
        TestStrategy(strategyService, availableQuests, player, QuestStrategy.Balanced);
        TestStrategy(strategyService, availableQuests, player, QuestStrategy.QuickCompletion);
    }
    
    private static void TestStrategy(QuestStrategyService strategyService, List<Quest> quests, PlayerState player, QuestStrategy strategy)
    {
        Console.WriteLine($"\n--- {strategy} Strategy ---");
        var recommendedQuests = strategyService.SelectOptimalQuests(quests, player, strategy);
        
        if (recommendedQuests.Any())
        {
            Console.WriteLine("Recommended quests:");
            foreach (var quest in recommendedQuests)
            {
                var efficiency = strategyService.CalculateQuestEfficiency(quest, player);
                Console.WriteLine($"  - {quest.Name} (Efficiency: {efficiency:F2})");
            }
        }
        else
        {
            Console.WriteLine("No suitable quests found for this strategy.");
        }
    }
}