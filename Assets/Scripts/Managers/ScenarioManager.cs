using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Valve.VR.InteractionSystem;
using Valve.VR;
using TMPro;
using DG.Tweening;
using KanKikuchi.AudioManager;

public class ScenarioManager : MonoBehaviour
{
    public SteamVR_Action_Boolean TextSkipAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI");
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
        // await SetFollowHUDText("このテキストはコントローラーのAボタンを押すことで次に進むことができます。");
        // await SetFollowHUDText("まず、このゲームでの操作方法を説明します。");
        // await SetFollowHUDText("右スティックを倒すことで左、右に45度回転することができます。");
        // await SetFollowHUDText("一度右にスティックを倒してみてください。");
        // await SetFollowHUDText("OKです。");
        // await SetFollowHUDText("次に、左にスティックを倒してみてください。");
        // await SetFollowHUDText("OKです。");
        // await SetFollowHUDText("次に、このゲームでの移動方法を説明します。");
        // await SetFollowHUDText("このゲームでの移動はテレポート方式を採用しています。");
        // await SetFollowHUDText("コントローラーの左スティックを倒すことでテレポートモードに移行し、\nテレポート地点で離すとテレポートします。");
        // await SetFollowHUDText("左前方のテレポート地点へテレポートしてみてください。");
        // await SetFollowHUDText("OKです。");
        // await SetFollowHUDText("次は、元いた場所に戻ってみてください。");
        // await SetFollowHUDText("OKです。");
        // await SetFollowHUDText("このゲームは、電流の流れを観察するゲームです。");
        // await SetFollowHUDText("コントローラーのトリガーを引いて、目の前の磁石を持ってみてください。");

        // await throwable.onPickUp.OnInvokeAsync(default);
        // await SetFollowHUDText("磁石を動かしてみてください。");
        // await SetFollowHUDText("磁石の動かす方向により電流の向きが変わります。");
        // await SetFollowHUDText("コイルを流れる電流の向きは右ねじの法則により決定されます");
        // await SetFollowHUDText("電流の向きを観察してください。");
        // await SetFollowHUDText("電流の向きが変わるのは、磁石の動かす方向が変わった時です。");
        await SetFollowHUDText("より早く磁石を動かすと生まれる電流はより大きくなります。");
        SetFollowHUDText("電流の大きさは、磁石を早く動かすほど大きくなります。\n早く動かしてみましょう。").Forget();
        await WaitUntilCurrentFlows(0.05f);
        await SetFollowHUDText("電流の向きや大きさは、ファラデーの電磁誘導の法則により決定されます。");
        await SetFollowHUDText("次に、簡単なクイズを行います。");
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
            if (Input.GetKeyDown(KeyCode.Space) || TextSkipAction.GetStateDown(SteamVR_Input_Sources.Any))
            {
                //文字送り中にスキップボタンが押されたら文字送りをスキップし文字全体をすぐに表示する
                FollowHUDText.maxVisibleCharacters = text.Length;
                break;
            }
        }
        //スキップボタンが押されるまで待つ
        await UniTask.WhenAny(
            UniTask.WaitUntil(() => TextSkipAction.GetStateDown(SteamVR_Input_Sources.Any)),//スキップボタンが押されたら
            UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space))
        );
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
