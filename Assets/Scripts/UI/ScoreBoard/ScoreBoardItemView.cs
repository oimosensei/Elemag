using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ScoreBoardItemView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerNameText;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private Button _addButtonForDebug;

    public Button AddButtonForDebug => _addButtonForDebug;

    public void Initialize(ScoreBoardItem item)
    {
        _playerNameText.text = item.PlayerName;
        _scoreText.text = item.Score.ToString();
    }
}

