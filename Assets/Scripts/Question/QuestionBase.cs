
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Valve.VR.InteractionSystem;
using KanKikuchi.AudioManager;

//問題文の定義
//問題の正解判定を行う
//問題の得点を計算する
public class QuestionBase
{
    public string DirectiveStatement;
    private int maxTime = 10;

    protected Coil _coil;

    protected Throwable throwableMagnet;
    //正解になるor時間制限が来るまで待機し、スコアを返す

    private void Start()
    {
    }
    public async UniTask<int> WaitAnswerAndGetScore(CancellationToken ct)
    {
        //正解判定
        //得点計算
        throwableMagnet = GameObject.Find("BarMagnetAA").GetComponent<Throwable>();
        throwableMagnet.enabled = true;

        var timerText = GameObject.Find("TimerText").GetComponent<Text>();
        var startTime = Time.time;
        var takenTime = Time.time - startTime;
        var timeLeft = maxTime;
        var delayUICancellationTokenSource = new System.Threading.CancellationTokenSource();
        //ToDo本元のCancellationTokenと合体させる
        var result = await UniTask.WhenAny(
            UniTask.Create(async () =>
            {
                takenTime = Time.time - startTime;
                timeLeft = maxTime;
                try
                {
                    while (timeLeft >= 0 && !delayUICancellationTokenSource.IsCancellationRequested)
                    {
                        //UIに時間を表示する
                        ct.ThrowIfCancellationRequested();
                        timerText.text = timeLeft.ToString();
                        //Todo:音源正しいのに設定しておく
                        SEManager.Instance.Play(SEPath.CLEAR);
                        await UniTask.Delay(1000, cancellationToken: delayUICancellationTokenSource.Token);
                        timeLeft--;
                    }
                    Debug.Log("cancelled");
                }
                //cannel投げ直さなくていいのかな？？？
                finally
                {
                    //UIを消す
                    timerText.text = "";
                    //Debug.Log("finally");
                }
            }).AsAsyncUnitUniTask(),
            WaitCorrectAction(ct)
        /*             UniTask.C
        /*             UniTask.Create(async () =>
                    {
                        takenTime = Time.time - startTime;
                        try
                        {
                            while (takenTime < maxTime && !delayUICancellationTokenSource.IsCancellationRequested)
                            {
                                takenTime = Time.time - startTime;
                                //UIに時間を表示する
                                ct.ThrowIfCancellationRequested();
                                timerText.text = ((int)(maxTime - takenTime)).ToString();
                                await UniTask.Yield();
                            }
                            //Debug.Log("cancelled");
                        }
                        //cannel投げ直さなくていいのかな？？？
                        finally
                        {
                            //UIを消す
                            timerText.text = "";
                            //Debug.Log("finally");
                        }
                    }).AsAsyncUnitUniTask(), */
        );
        delayUICancellationTokenSource.Cancel();
        throwableMagnet.enabled = false;
        Debug.Log("result:" + result);
        if (timeLeft < 0 || ((result.winArgumentIndex == 1) && !result.result2))
        {
            //失敗した場合
            return 0;
        }
        else
        {
            //点数計算処理
            return (int)((maxTime - takenTime) * 10);
        }
    }

    public virtual async UniTask<bool> WaitCorrectAction(CancellationToken ct)
    {
        await UniTask.Delay(1);
        return true;
    }

    private async UniTask<AsyncUnit> DelayWithTimerUI(int ms, System.Threading.CancellationToken ct)
    {
        //時間が終わるまでUIを更新し、時間が来たらUIを消す
        //キャンセルが投げられたらキャンセル処理をしてUIを消す
        //別にUIのタイマーは別でUniRXでやってもいいかも
        await UniTask.WhenAny(
            UniTask.Delay(ms, cancellationToken: ct),
            UniTask.Create(async () =>
            {
                var startTime = Time.time;
                var takenTime = Time.time - startTime;
                try
                {
                    while (takenTime < ms)
                    {
                        takenTime = Time.time - startTime;
                        //UIに時間を表示する
                        ct.ThrowIfCancellationRequested();
                        await UniTask.Yield();
                    }

                }
                finally
                {
                    //UIを消す
                }
            })
        );
        return AsyncUnit.Default;
    }
}
