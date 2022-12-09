using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct ScoreBoardItem
{
    public string PlayerName { get; }

    public int Score { get; }

    public ScoreBoardItem(string playerName, int score)
    {
        PlayerName = playerName;
        Score = score;
    }
}
