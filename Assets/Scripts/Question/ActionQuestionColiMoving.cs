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
    public ActionQuestionCoilMoving(MagnetDirectionEnum magnetDirection, CurrentDirectonEnum currentDirection, string directiveStatement = "")
    {
        _magnetDirection = magnetDirection;
        if (magnetDirection != MagnetDirectionEnum.None)
        {
            var magnetDirectionStr = magnetDirection == MagnetDirectionEnum.NUp ? "上" : "下";
            DirectiveStatement = $"磁石のN極を{magnetDirectionStr}に向けて、\n";

        }
        _currentDirection = currentDirection;
        var currentDirectionStr = currentDirection == CurrentDirectonEnum.Clockwise ? "時計回り" : "反時計回り";
        DirectiveStatement += currentDirectionStr + "に電流を流してください";
        if (directiveStatement != "")
        {
            DirectiveStatement = directiveStatement;
        }
    }

    //引数を指定しない場合、ランダムに問題を生成する
    public ActionQuestionCoilMoving()
    {
        //SystemのRamdomを用いて乱数を生成し、enumの値をランダムに取得する
        System.Random random = new System.Random();
        var magnetDirection = (MagnetDirectionEnum)random.Next(0, 3);
        var currentDirection = (CurrentDirectonEnum)random.Next(0, 2);
        _magnetDirection = magnetDirection;
        if (magnetDirection != MagnetDirectionEnum.None)
        {
            var magnetDirectionStr = magnetDirection == MagnetDirectionEnum.NUp ? "上" : "下";
            DirectiveStatement = $"磁石のN極を{magnetDirectionStr}に向けて、\n";

        }
        _currentDirection = currentDirection;
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

        //コイルをSceneから取得し、問題文と行動が合致しているか判定する
        var transform = throwableMagnet.transform;
        var isMagnetDirectionCorrect = (_magnetDirection != MagnetDirectionEnum.NDown) ^ (transform.rotation.x > -90 && transform.rotation.x < 90);
        Debug.Log("isMagnetDirectionCorrect" + isMagnetDirectionCorrect);
        isMagnetDirectionCorrect |= _magnetDirection == MagnetDirectionEnum.None;
        _coil = GameObject.Find("Coil").GetComponent<Coil>();
        var isCurrentDirectionCorrect = _coil.deltaAllJisokuRP.Value > 0 ^ _currentDirection == CurrentDirectonEnum.Clockwise;
        Debug.Log("isCurrentDirectionCorrect" + isCurrentDirectionCorrect);
        Debug.Log("kaiotu" + (isMagnetDirectionCorrect &&
            isCurrentDirectionCorrect));
        return isMagnetDirectionCorrect &&
            isCurrentDirectionCorrect;

    }
}
