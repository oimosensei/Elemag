using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using ObservableCollections;

public class ScoreBoard : MonoBehaviour
{
    private GameObject _content;
    private GameObject _scoreBoardItemPrefab;

    private ReactiveCollection<ScoreBoardItem> _scoreBoardItems = new ReactiveCollection<ScoreBoardItem>();
    public IObservableCollection<ScoreBoardItem> ScoreBoardItems => _scoreBoardItemsList;
    private ObservableList<ScoreBoardItem> _scoreBoardItemsList = new ObservableList<ScoreBoardItem>();

    public void AddItem(ScoreBoardItem item)
    {
        _scoreBoardItemsList.Add(item);
    }

    public void RemoveItem(ScoreBoardItem item)
    {
        _scoreBoardItemsList.Remove(item);
    }
}
