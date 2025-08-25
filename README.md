# Idle Slayer Quest Strategy Implementation

## Overview
This project implements a quest strategy determination system for the game Idle Slayer using **real quest data from the Idle Slayer wiki**. It provides multiple algorithmic approaches for optimal quest selection that align with community-recommended strategies.

## Features
- **Real Wiki Data**: Uses authentic quest data from the Idle Slayer wiki including actual quest names, objectives, rewards, and progression bundles
- **Bundle-Based Progression**: Implements the wiki's quest bundle system (Training → Rookie → Novice → Beginner → Advanced → etc.)
- **Multiple Strategy Types**: MaxEfficiency, Balanced, QuickCompletion, HighReward, Progressive, and Wiki-Recommended strategies
- **Dimension Awareness**: Considers recommended dimensions for optimal quest completion
- **Ultra Ascension Support**: Handles Ultra Ascension requirements and unlocks
- **Dynamic Quest Evaluation**: Real-time efficiency calculations based on player state and wiki recommendations
- **Comprehensive Modeling**: Detailed quest categorization (Active/Passive/Wherever/Ultra Ascension)

## Data Source
All quest data is sourced from the official [Idle Slayer Wiki](https://idleslayer.fandom.com/wiki/Quests) and [Quest Strategy Guide](https://idleslayer.fandom.com/wiki/Quests_Strategy), ensuring authentic gameplay experience and alignment with community best practices.

## Running the Application
```bash
dotnet run
```

## Architecture

### Models
- `Quest`: Represents individual quests with real Idle Slayer quest data including bundles, categories, and wiki-based requirements
- `PlayerState`: Tracks player progress including Ultra Ascensions, current dimension, and unlocked features  
- `QuestRewards`: Defines various reward types including percentage bonuses and unlock descriptions
- `QuestObjective`: Structured representation of quest objectives (Kill counts, targets, etc.)

### Services
- `QuestService`: Manages authentic Idle Slayer quest data and eligibility checking
- `QuestStrategyService`: Core strategy determination following wiki recommendations

### Real Quest Bundles Implemented
1. **Training Quests**: Early game introduction (Test Your Might, Beekeeper, etc.)
2. **Rookie Quests**: Basic progression (The Winner Takes All, Clearing The Skies)  
3. **Novice Quests**: Intermediate challenges (Worm Domination, Soul Splitter unlocks)
4. **Beginner → Expert**: Progressive difficulty scaling
5. **Ancient/Ultra Ascension**: Post-ascension content

### Strategy Types
1. **MaxEfficiency**: Prioritizes quests with highest reward-to-time ratio
2. **Balanced**: Combines efficiency with time management constraints  
3. **QuickCompletion**: Focuses on fastest completable quests
4. **HighReward**: Prioritizes absolute reward values
5. **Progressive**: Emphasizes long-term character development
6. **Wiki-Recommended**: Follows official wiki progression and bundle order (default)

## Evaluation Results
See [QUEST_STRATEGY_EVALUATION.md](QUEST_STRATEGY_EVALUATION.md) for detailed analysis of the implementation against wiki recommendations.

## Example Output
```
=== Idle Slayer Quest Strategy Evaluator ===
Using real quest data from Idle Slayer wiki

Total Available Quests: 12

Quest Bundles Available:
  - Training: 3 quests
  - Rookie: 3 quests  
  - Novice: 3 quests
  - Advanced: 2 quests
  - Ancient: 1 quests

=== Early Game Player ===
Level: 15, Coins: 50,000, Souls: 500
Ultra Ascensions: 0, Current Dimension: Hills
Eligible Quests: 4

Top Wiki-Recommended Quests:
  - Test Your Might
    Bundle: Training, Category: Active
    Objective: Kill 8 flying enemies.
    Reward: Training reward
    Dimension: Hills, Efficiency: 1680.00
    
  - Beekeeper  
    Bundle: Training, Category: Active
    Objective: Kill 50 Wasps.
    Reward: Honey: Gain 3% bonus CpS
    Dimension: Hills, Efficiency: 360.00
```
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