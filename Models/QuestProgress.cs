using System;
using System.Collections.Generic;

namespace IdleSlayerQuests.Models;

/// <summary>
/// Represents the current progress and status of a quest for a specific player
/// </summary>
public class QuestProgress
{
    public int QuestId { get; set; }
    public QuestStatus Status { get; set; } = QuestStatus.Available;
    public DateTime? StartTime { get; set; }
    public DateTime? CompletionTime { get; set; }
    public Dictionary<string, int> Progress { get; set; } = new(); // e.g., "Wasps" -> current kill count
    public Dictionary<string, int> ProgressAtStart { get; set; } = new(); // Kill counts when quest started
}

/// <summary>
/// Represents the possible states of a quest
/// </summary>
public enum QuestStatus
{
    Available,  // Quest can be started
    Active,     // Quest is currently being tracked
    Completed   // Quest has been finished
}