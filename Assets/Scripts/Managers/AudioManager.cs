
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Valve.VR.InteractionSystem;
using TMPro;
using DG.Tweening;
using UniRx;

public class AudioManager : MonoBehaviour
{
    public IntReactiveProperty _totalScore = new IntReactiveProperty(0);

    [Header("UIComponents")]

    [SerializeField]
    private Image correctEffectImage;
    [SerializeField]
    private Image incorrectEffectImage;
    [SerializeField]
    private TextMeshProUGUI scoreUpGUI;
    [SerializeField]
    private TextMeshProUGUI totalScoreGUI;

    private void Start()
    {
        _totalScore.Subscribe(x => totalScoreGUI.text = x.ToString());
    }

    public void ScoreUp(int score)
    {
        //スコアを加算する
        //スコアアップのUIを表示する
        scoreUpGUI.text = "+" + score.ToString();
        scoreUpGUI.gameObject.SetActive(true);
        scoreUpGUI.DOFade(0, 1.0f).OnComplete(() => scoreUpGUI.gameObject.SetActive(false));

        this._totalScore.Value += score;
    }

    public void ScoreReset()
    {
        this._totalScore.Value = 0;
    }
}