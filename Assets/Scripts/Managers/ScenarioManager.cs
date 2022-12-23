using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Valve.VR.InteractionSystem;
using TMPro;
using DG.Tweening;
using KanKikuchi.AudioManager;

public class ScenarioManager : MonoBehaviour
{
    public TextMeshProUGUI FollowHUDText;
    public Throwable throwable;

    public Coil coil;

    public Image circleGuage;

    public QuestionManager questionManager;
    // Start is called before the first frame update
    void Start()
    {
        FirstTutorial();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public async void FirstTutorial()
    {
        await SetFollowHUDText("このゲームは、電流の流れを観察するゲームです。");
        await SetFollowHUDText("目の前の磁石を持ってみてください。");

        /*         await throwable.onPickUp.OnInvokeAsync(default);
                await SetFollowHUDText("磁石を動かしてみてください。");
                await SetFollowHUDText("磁石の動かす方向により電流の向きが変わります。");
                await SetFollowHUDText("コイルを流れる電流の向きは右ねじの法則により決定されます");
                await SetFollowHUDText("電流の向きを観察してください。");
                await SetFollowHUDText("電流の向きが変わるのは、磁石の動かす方向が変わった時です。");
                await SetFollowHUDText("より早く磁石を動かすと生まれる電流はより大きくなります。");
                SetFollowHUDText("電流の大きさは、磁石を早く動かすほど大きくなります。\n早く動かしてみましょう。").Forget();
                await WaitUntilCurrentFlows(10.0f);
                await SetFollowHUDText("次に、簡単なクイズを行います。"); */
        await SetFollowHUDText("磁石を動かして指示に従い、回答を行ってください。");
        SetFollowHUDText("").Forget();
        await questionManager.QuestionLoop();
    }

    public async UniTask SetFollowHUDText(string text)
    {
        int feedTime = 70;
        //一文字ずつ表示する
        FollowHUDText.text = text;
        for (int i = 0; i < text.Length + 1; i++)
        {
            FollowHUDText.maxVisibleCharacters = i;
            await UniTask.Delay(feedTime);
        }
        await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        //音を鳴らす
        SEManager.Instance.Play(SEPath.CLEAR);
        //テキストのアニメーションをつけて、アニメーションが終わるまで待つようにする
    }

    public async UniTask WaitUntilCurrentFlows(float current)
    {
        float currentSum = 0;
        circleGuage.DOFade(1.0f, 0);
        while (currentSum < current)
        {
            currentSum += Mathf.Abs(coil.deltaAllJisokuRP.Value);
            circleGuage.fillAmount = currentSum / current;
            Debug.Log(currentSum);
            await UniTask.Yield();
        }
        circleGuage.DOFade(0.0f, 0.5f);
    }

    public void GetPoint(int point)
    {

    }
}
