using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScoreBoardItem
{
    [SerializeField] public string PlayerName;

    [SerializeField] public int Score;

    public ScoreBoardItem(string playerName, int score)
    {
        PlayerName = playerName;
        Score = score;
    }
}
