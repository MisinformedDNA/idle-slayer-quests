# Idle Slayer Quest Strategy Implementation

## Overview
This project implements a quest strategy determination system for the game Idle Slayer, providing multiple algorithmic approaches for optimal quest selection.

## Features
- **Multiple Strategy Types**: MaxEfficiency, Balanced, QuickCompletion, HighReward, and Progressive
- **Dynamic Quest Evaluation**: Real-time efficiency calculations based on player state
- **Extensible Architecture**: Easy to add new strategies and quest types
- **Comprehensive Modeling**: Detailed quest and player state representations

## Running the Application
```bash
dotnet run
```

## Architecture

### Models
- `Quest`: Represents individual quests with rewards, requirements, and metadata
- `PlayerState`: Tracks player progress, resources, and capabilities
- `QuestRewards`: Defines various reward types (coins, souls, celestial points, divinities)

### Services
- `QuestService`: Manages quest data and eligibility checking
- `QuestStrategyService`: Core strategy determination and optimization logic

### Strategy Types
1. **MaxEfficiency**: Prioritizes quests with highest reward-to-time ratio
2. **Balanced**: Combines efficiency with time management constraints
3. **QuickCompletion**: Focuses on fastest completable quests
4. **HighReward**: Prioritizes absolute reward values
5. **Progressive**: Emphasizes long-term character development

## Evaluation Results
See [QUEST_STRATEGY_EVALUATION.md](QUEST_STRATEGY_EVALUATION.md) for detailed analysis of the implementation against wiki recommendations.

## Example Output
```
=== Idle Slayer Quest Strategy Evaluator ===

Available Quests: 5
  - Slay 100 Goblins: Kill 100 goblins in the forest
  - Collect 1M Coins: Gather 1 million coins through any means
  - Upgrade Weapon 5 Times: Perform 5 weapon upgrades
  - Idle for 2 Hours: Let the game idle for 2 hours continuously
  - Defeat Boss: Defeat the current area boss

Player State: Level 50, 1,000,000 coins, 100 souls/sec

--- MaxEfficiency Strategy ---
Recommended quests:
  - Defeat Boss (Efficiency: 7133.33)
  - Slay 100 Goblins (Efficiency: 4080.00)
  - Upgrade Weapon 5 Times (Efficiency: 3450.00)
```