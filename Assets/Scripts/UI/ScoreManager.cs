using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Valve.VR.InteractionSystem;
using TMPro;
using DG.Tweening;
using UniRx;

public class ScoreManager : MonoBehaviour
{
    public IntReactiveProperty _totalScore = new IntReactiveProperty(0);

    [Header("UIComponents")]
    [SerializeField]
    private TextMeshProUGUI scoreUpGUI;
    [SerializeField]
    private Text totalScoreGUI;

    private void Start()
    {
        _totalScore.Subscribe(x => totalScoreGUI.DOTextInt(int.Parse(totalScoreGUI.text), x, 1.0f));
    }

    public void ScoreUp(int score)
    {
        //スコアを加算する
        //スコアアップのUIを表示する
        scoreUpGUI.text = "+" + score.ToString();
        scoreUpGUI.gameObject.SetActive(true);
        DOTween.Sequence().Append(scoreUpGUI.DOFade(1, 1))
        .Join(scoreUpGUI.transform.DOLocalMoveY(2, 2))
       .AppendInterval(.5f)
       .Append(scoreUpGUI.DOFade(0, .5f))
        .Append(scoreUpGUI.transform.DOLocalMoveY(-2, 0))
       .AppendInterval(.5f);

        this._totalScore.Value += score;
    }
}