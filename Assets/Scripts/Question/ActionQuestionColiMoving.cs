using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

//問題文の定義
//問題の正解判定を行う
//問題の得点を計算する
public class ActionQuestionCoilMoving : QuestionBase
{

    public enum MagnetDirectionEnum
    {
        NUp,
        NDown,
        None
    }

    public enum CurrentDirectonEnum
    {
        Clockwise,
        CounterClockwise,
    }

    private MagnetDirectionEnum _magnetDirection;

    private CurrentDirectonEnum _currentDirection;
    private float _moveThreshold = 0.1f;
    public ActionQuestionCoilMoving(MagnetDirectionEnum magnetDirection, CurrentDirectonEnum currentDirection)
    {
        if (magnetDirection != MagnetDirectionEnum.None)
        {
            var magnetDirectionStr = magnetDirection == MagnetDirectionEnum.NUp ? "上" : "下";
            DirectiveStatement = $"磁石のN極を{magnetDirectionStr}に向けて、\n";

        }
        var currentDirectionStr = currentDirection == CurrentDirectonEnum.Clockwise ? "時計回り" : "反時計回り";
        DirectiveStatement += currentDirectionStr + "に電流を流してください";
    }
    public override async UniTask<bool> WaitCorrectAction(CancellationToken ct)
    {
        //ToDo: 流れる電流ベースで判定をして、電流が一定量流れたら最後に磁石の向きなどを確認する流れのほうが良さそう
        //それか磁石の向きを整えてる間は位置がずれる可能性があるので判定を行わない(あとで向きの判定も行わなければいけないのでめんどくさい)
        await throwableMagnet.onPickUp.OnInvokeAsync(default);
        //この時点での磁石の位置を保存する
        var startPosition = throwableMagnet.transform.position;
        Debug.Log("startPosition" + startPosition);
        //磁石が最初の位置からz軸方向に５動くまでまつ
        await UniTask.WaitUntil(() => Mathf.Abs(throwableMagnet.transform.position.y - startPosition.y) > _moveThreshold, cancellationToken: ct);
        Debug.Log("磁石が動いた");

        var transform = throwableMagnet.transform;
        var isMagnetDirectionCorrect = _magnetDirection == MagnetDirectionEnum.NDown ^ (transform.rotation.x > -90 && transform.rotation.x < 90);
        isMagnetDirectionCorrect |= _magnetDirection == MagnetDirectionEnum.None;
        return isMagnetDirectionCorrect &&
                transform.position.y - startPosition.y > 0;


    }
}
