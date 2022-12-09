

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.UI;
//問題文の定義
//問題の正解判定を行う
//問題の得点を計算する
public class SelectQuestion : QuestionBase
{

    private string _correctAnswer;
    private string _wrongAnswer;

    [SerializeField] private Button _correctAnswerButton;
    [SerializeField] private Button _wrongAnswerButton;

    public SelectQuestion(string problemStatement, string correctAnswer, string wrongAnswer)
    {
        DirectiveStatement = problemStatement;
        //半分の確率でボタンの位置をスワップすることで正解のボタンの位置をランダムにする
        if (Random.Range(0, 2) == 0)
        {
            //_correctAnswerButtonの位置をタプルでスワップ
            (_correctAnswerButton.transform.position, _wrongAnswerButton.transform.position) = (_wrongAnswerButton.transform.position, _correctAnswerButton.transform.position);
        }
        _correctAnswerButton.GetComponentInChildren<Text>().text = correctAnswer;
        _wrongAnswerButton.GetComponentInChildren<Text>().text = wrongAnswer;

    }
    public override async UniTask<bool> WaitCorrectAction(CancellationToken ct)
    {

        var result = await UniTask.WhenAny(_correctAnswerButton.OnClickAsync(), _wrongAnswerButton.OnClickAsync());
        return result == 0;

    }
}
