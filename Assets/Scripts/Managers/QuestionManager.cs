using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Valve.VR.InteractionSystem;
using TMPro;
using DG.Tweening;
using KanKikuchi.AudioManager;
public class QuestionManager : MonoBehaviour
{

    //questionbaseのリスト
    private List<QuestionBase> questionList = new List<QuestionBase>() {
    /*new ActionQuestionCoilMoving(ActionQuestionCoilMoving.MagnetDirectionEnum.NDown,ActionQuestionCoilMoving.CurrentDirectonEnum.Clockwise) ,
    new ActionQuestionCoilMoving(ActionQuestionCoilMoving.MagnetDirectionEnum.NUp,ActionQuestionCoilMoving.CurrentDirectonEnum.CounterClockwise) ,
    new ActionQuestionCoilMoving(),
     new ActionQuestionCoilMoving(ActionQuestionCoilMoving.MagnetDirectionEnum.NDown,ActionQuestionCoilMoving.CurrentDirectonEnum.Clockwise) ,
    new ActionQuestionCoilMoving(ActionQuestionCoilMoving.MagnetDirectionEnum.NDown,ActionQuestionCoilMoving.CurrentDirectonEnum.Clockwise) ,
    new ActionQuestionCoilMoving(ActionQuestionCoilMoving.MagnetDirectionEnum.NDown,ActionQuestionCoilMoving.CurrentDirectonEnum.Clockwise) ,
    new ActionQuestionCoilMoving(ActionQuestionCoilMoving.MagnetDirectionEnum.NDown,ActionQuestionCoilMoving.CurrentDirectonEnum.Clockwise) ,
    new ActionQuestionCoilMoving(ActionQuestionCoilMoving.MagnetDirectionEnum.NDown,ActionQuestionCoilMoving.CurrentDirectonEnum.Clockwise) , */
    new ActionQuestionCoilMoving(ActionQuestionCoilMoving.MagnetDirectionEnum.NDown,ActionQuestionCoilMoving.CurrentDirectonEnum.Clockwise) ,
    };
    private QuestionBase questionBase;

    [Header("UIComponents")]

    [SerializeField] private Image _correctEffectImage;
    [SerializeField] private Image _incorrectEffectImage;
    [SerializeField] private TextMeshProUGUI _questionTextGUI;
    [SerializeField] private ScoreBoard _scoreBoard;
    [SerializeField] private ScoreManager _scoreManager;
    void Start()
    {
        //correctImageの不透明度を0にする
        _correctEffectImage.DOFade(0, 0);
        //incorrectImageの不透明度を0にする
        _incorrectEffectImage.DOFade(0, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }


    public async UniTask QuestionLoop()
    {
        while (questionList.Count != 0)
        {
            //ランダムに問題を選びリストから削除する
            var index = Random.Range(0, questionList.Count);
            questionBase = questionList[index];
            questionList.RemoveAt(index);
            var throwableMagnet = GameObject.Find("BarMagnetAA").GetComponent<Throwable>();
            throwableMagnet.GetComponent<Interactable>().enabled = false;
            //問題を表示する
            var questionString = questionBase.DirectiveStatement;
            await DrawQuestionString(questionString);
            //回答を始めるUIを表示、少し時間をおく　
            await UniTask.Delay(1000);
            //回答が終わるor時間が切れるまで待つ
            var score = await questionBase.WaitAnswerAndGetScore(this.GetCancellationTokenOnDestroy());

            _scoreManager.ScoreUp(score);
            //UI得点処理
            //スコアが0だったら失敗UI
            await PlayCorrectOrUncorrectAnimation(score != 0);
            _questionTextGUI.text = "";
            await UniTask.Delay(1000);
        }
        //問題とかのUIを非表示してScoreBoardを表示する
        _questionTextGUI.gameObject.SetActive(false);
        _correctEffectImage.gameObject.SetActive(false);
        _incorrectEffectImage.gameObject.SetActive(false);

        _scoreBoard.AddItem(new ScoreBoardItem("Player", _scoreManager._totalScore.Value));
        _scoreBoard.gameObject.SetActive(true);
    }


    async UniTask PlayCorrectOrUncorrectAnimation(bool isCorrect)
    {
        var image = isCorrect ? _correctEffectImage : _incorrectEffectImage;
        var sePath = SEPath.CLEAR;//isCorrect ? SEPath.CORRECT : SEPath.INCORRECT;        
        SEManager.Instance.Play(sePath);
        //dotweenで画像を３秒間点滅するアニメーション
        await DOTween.Sequence().Append(image.DOFade(1, 0))
        .AppendInterval(.5f)
        .Append(image.DOFade(0, 0))
        .AppendInterval(.5f)
        .SetLoops(3, LoopType.Restart).AsyncWaitForCompletion();
    }
    //問題文を表示する
    //あとでUIようのクラスに移す予定
    async UniTask DrawQuestionString(string questionString)
    {
        //UIに問題文を表示する
        int feedTime = 70;
        _questionTextGUI.text = questionString;
        //一文字ずつ表示する
        for (int i = 0; i < questionString.Length + 1; i++)
        {
            _questionTextGUI.maxVisibleCharacters = i;
            await UniTask.Delay(feedTime);
        }
    }
}
