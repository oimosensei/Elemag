using System.Collections;
using UniRx;
using ObservableCollections;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoardItemPresenter : MonoBehaviour
{
    [SerializeField] private GameObject _content;
    [SerializeField] private GameObject _scoreBoardItemPrefab;
    [SerializeField] private ScoreBoard _scoreBoard;

    private ISynchronizedView<ScoreBoardItem, ScoreBoardItemView> _synchronizedView;

    private void Start()
    {
        _synchronizedView = _scoreBoard.ScoreBoardItems.CreateView(i =>
        {
            ScoreBoardItemView view = Instantiate(_scoreBoardItemPrefab, _content.transform).GetComponent<ScoreBoardItemView>();
            view.Initialize(i);
            return view;
        });
        Observable.EveryUpdate()
      .Where(_ => Input.GetKeyDown(KeyCode.A))
      .Subscribe(_ =>
      {
          _scoreBoard.AddItem(new ScoreBoardItem("test", 100));
      });
    }
}