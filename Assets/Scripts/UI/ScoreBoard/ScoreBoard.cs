using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using ObservableCollections;

public class ScoreBoard : MonoBehaviour
{


    private ObservableList<ScoreBoardItem> _scoreBoardItemsList = new ObservableList<ScoreBoardItem>();
    public IObservableCollection<ScoreBoardItem> ScoreBoardItems => _scoreBoardItemsList;

    void Start()
    {
        ResetData();
        LoadScoreBoard();
        AddItem(new ScoreBoardItem("test1", 100));
        AddItem(new ScoreBoardItem("test2", 300));
        AddItem(new ScoreBoardItem("test3", 520));
        SaveScoreBoard();
    }
    public void AddItem(ScoreBoardItem item)
    {
        _scoreBoardItemsList.Add(item);
    }

    public void RemoveItem(ScoreBoardItem item)
    {
        _scoreBoardItemsList.Remove(item);
    }

    public void LoadScoreBoard()
    {
        //null checkいるかも
        string json = PlayerPrefs.GetString("ScoreBoard", "[]");
        Debug.Log(json);
        var items = JsonHelper.FromJson<ScoreBoardItem>(json);
        _scoreBoardItemsList.AddRange(items);
    }

    public void SaveScoreBoard()
    {
        //null checkいるかも
        string json = JsonHelper.ToJson(_scoreBoardItemsList);
        PlayerPrefs.SetString("ScoreBoard", json);
    }

    public void ResetData()
    {
        PlayerPrefs.DeleteKey("ScoreBoard");
    }
}
