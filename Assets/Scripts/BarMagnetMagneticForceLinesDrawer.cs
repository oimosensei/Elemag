using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

// Todo: 後でクラス名をRenameする
// Todo: 後でStartとUpdateからメソッドを外出しする
/// <summary>
/// /// 磁力線の描画を行う
/// </summary>
public class BarMagnetMagneticForceLinesDrawer : MonoBehaviour
{
    //磁力線描画のPrefab
    private GameObject magneticForceLinePrefab;
    //ログ出力用
    private bool hasLogged;

    //矢印を描画する間隔を決める変数
    private int _lineArrowInterval = 10;

    BarMagnetModel barMagnetModel;
    [SerializeField] GameObject lineArrowPrefab;

    /// <summary>
    /// 磁力線を描画中か管理するフラグ(public)　オフになったら自動的にLnePrefabを消す
    /// </summary>
    public BoolReactiveProperty IsDrawing = new BoolReactiveProperty(true);

    List<LineRenderer> magneticForceLines = null;
    List<float> listStartY;
    List<float> listStartZ;

    private bool processing = false;

    Transform[] southPolesTransform;
    Transform[] northPolesTransform;


    private void Start()
    {
        barMagnetModel = GetComponent<BarMagnetModel>();
        magneticForceLinePrefab = barMagnetModel.MagneticForceLinePrefab;

        magneticForceLines = new List<LineRenderer>();

        // listStartY = new List<float> { -0.02f, -0.002f, 0, 0.002f, 0.02f };
        listStartY = new List<float> { -0.002f, 0, 0.002f };

        listStartZ = new List<float> { -0.002f, 0, 0.002f };
        //        listStartZ = new List<float> { 0 };
        //違いがわからん
        IsDrawing.Subscribe(x =>
        {
            if (!x)
            {
                DeleteLines();
            }
        });

    }
    [SerializeField]
    private int drawLineInterval = 1;
    private int drawLineCurrentFrame = 0;
    public void Update()
    {
        DrawLines();
    }


    private void DrawLines()
    {
        // Debug.Log("update");
        //drawLineIntervalフレームに一回実行
        if (drawLineCurrentFrame < drawLineInterval)
        {
            drawLineCurrentFrame++;
            return;
        }
        drawLineCurrentFrame = 0;
        if (IsDrawing.Value)
        {
            //必要なら磁力線の初期化
            if (magneticForceLines.Count == 0)
                GenerateLines();


            //N極磁力線の描画
            DrawLoop(true, barMagnetModel.NorthPoleReference.transform.position).Forget();

            //S極磁力線の描画
            DrawLoop(false, barMagnetModel.SouthPoleReference.transform.position).Forget();


        }
    }

    private void InitializeLineRenderer(LineRenderer magnetForceLine)
    {
        // === LineRendererを設定する ===
        // --- LineRendererを初期化する ---
        magnetForceLine.useWorldSpace = true;
        magnetForceLine.positionCount = _positionCountOfLine;

        // --- lineの太さ ---
        magnetForceLine.startWidth = _widthOfLines;
        magnetForceLine.endWidth = _widthOfLines;
    }

    private List<List<GameObject>> _lineArrowGameObjectsList = new List<List<GameObject>>();
    public void GenerateLines()
    {
        processing = false;
        if (magneticForceLines.Count > 0)
        {
            DeleteLines();
        }

        foreach (float startY in listStartY)
        {
            foreach (float startZ in listStartZ)
            {
                //N用とS用とLoop毎に２つ生成する
                //line用のオブジェクト作ってもいいかも、、同じ処理2回書くの冗長すぎ

                var lineGameObject = Instantiate(magneticForceLinePrefab, transform.position, Quaternion.identity);
                // 作成したオブジェクトを子として登録
                //lineGameObject.tag = "CloneLine";
                lineGameObject.transform.parent = transform;
                var line = lineGameObject.GetComponent<LineRenderer>();
                InitializeLineRenderer(line);
                magneticForceLines.Add(line);

                lineGameObject = Instantiate(magneticForceLinePrefab, transform.position, Quaternion.identity);
                // 作成したオブジェクトを子として登録
                // lineGameObject.tag = "CloneLine";
                lineGameObject.transform.parent = transform;
                line = lineGameObject.GetComponent<LineRenderer>();
                InitializeLineRenderer(line);
                magneticForceLines.Add(line);


                int lineArrowCount = _positionCountOfLine / _lineArrowInterval;
                lineArrowCount = lineArrowCount % 2 == 0 ? lineArrowCount : lineArrowCount + 1;

                List<GameObject> lineArrowGameObjectsForN = new List<GameObject>();
                List<GameObject> lineArrowGameObjectsForS = new List<GameObject>();
                for (int i = 0; i < lineArrowCount; i++)
                {
                    var lineArrowGameObjectForN = Instantiate(lineArrowPrefab, transform.position, Quaternion.identity);
                    var lineArrowGameObjectForS = Instantiate(lineArrowPrefab, transform.position, Quaternion.identity);
                    // 作成したオブジェクトを子として登録
                    // lineArrowGameObject.transform.parent = transform;
                    lineArrowGameObjectsForN.Add(lineArrowGameObjectForN);
                    lineArrowGameObjectsForS.Add(lineArrowGameObjectForS);
                }
                _lineArrowGameObjectsList.Add(lineArrowGameObjectsForN);
                _lineArrowGameObjectsList.Add(lineArrowGameObjectsForS);
            }
        }

        // すべてのN極、S極を取得する
        northPolesTransform = GameObject.FindGameObjectsWithTag("North Pole").
            Select(go => go.transform).
            ToArray();
        southPolesTransform = GameObject.FindGameObjectsWithTag("South Pole").
            Select(go => go.transform).
            ToArray();
    }

    public void DeleteLines()
    {
        foreach (var line in magneticForceLines)
        {
            Destroy(line.gameObject);
        }
        magneticForceLines.Clear();
        foreach (var lineArrowGameObjects in _lineArrowGameObjectsList)
        {
            foreach (var lineArrowGameObject in lineArrowGameObjects)
            {
                Destroy(lineArrowGameObject);
            }
        }
        _lineArrowGameObjectsList.Clear();
    }

    public async UniTask DrawLoop(bool lineIsFromNorthPole, Vector3 polePosInWorld)
    {
        // if (processing) return;
        // if (lineIsFromNorthPole)
        // Debug.Log("DrawLoop");
        processing = true;
        Vector3 barMagnetDirection = transform.rotation.eulerAngles;

        //S極だったらcountを半分から始める
        int initialCount = (lineIsFromNorthPole ? 0 : (int)magneticForceLines.Count / 2);
        int count = initialCount;

        var rotation = gameObject.transform.rotation;
        MagneticPoleManager.Instance.UpdatePolesPositions();
        UniTask[] tasks = new UniTask[listStartY.Count * listStartZ.Count];
        foreach (float startY in listStartY)
        {
            foreach (float startZ in listStartZ)
            {
                //var  shiftPositionFromMyPole2 = gameObject.transform.rotation * shiftPositionFromMyPole;
                //Vector3 startPosition = polePosInWorld + shiftPositionFromMyPole2;


                //なんでｙｘｚの順番でないといけないのかわからん 回転がかかってるからか？
                Vector3 shiftPositionFromMyPole = new Vector3(
                    0.001f * (!lineIsFromNorthPole ? 1 : -1),  // 極からx方向にどれくらい離すか
                    startY,
                    startZ
                    );

                tasks[count - initialCount] = DrawOneLine(
                // DrawOneLineSync(
                magneticForceLines[count],
                _lineArrowGameObjectsList[count],
                lineIsFromNorthPole,
                polePosInWorld + rotation * shiftPositionFromMyPole);

                count++;
            }
        }
        //stopwatchで実行時間を計測する

        // await UniTask.SwitchToThreadPool();
        // UniTask.WhenAll(tasks).AsTask().Wait();
        // await UniTask.SwitchToMainThread();

        await UniTask.WhenAll(tasks);


        //計測を終了する
        // sw.Stop();
        //結果を表示する
        // Debug.Log(sw.Elapsed);
        processing = false;
        // if (lineIsFromNorthPole)
        // Debug.Log("DrawLoop end");

    }

    private float distanceThresholdFromPole = 0.01f;

    float scaleToFitLocalPosition = 0.15f;

    /// <summary>
    /// 線分の長さ
    /// 調節してN極から出た磁力線とS極から出た磁力線が一致するようにする
    /// </summary>
    [SerializeField] float _baseLengthOfLine = 0.01f;

    /// <summary>
    /// 描く線分の数
    /// </summary>
    [SerializeField] int _positionCountOfLine = 55;

    /// <summary>
    /// 描く線分の太さ
    /// 調節してN極から出た磁力線とS極から出た磁力線が一致するようにする
    /// </summary>
    [SerializeField] float _widthOfLines = 0.005f;


    /// <summary>
    /// 引数の(x, y, z)を始点として磁力線を描く
    /// </summary>
    public async UniTask DrawOneLine(LineRenderer magnetForceLine, List<GameObject> lineArrowList, bool lineIsFromNorthPole, Vector3 startPosition)
    {
        // --- LineRendererの始点を初期位置にセットする ---
        magnetForceLine.SetPosition(0, startPosition);  // 引数の(x, y, z)を始点として磁力線を描く
        var positions = new Vector3[_positionCountOfLine];
        var directions = new Vector3[_positionCountOfLine];
        //trueで初期化する
        var isSkipDrawing = Enumerable.Repeat<bool>(true, _positionCountOfLine).ToArray();
        var posisionCount = magnetForceLine.positionCount;
        //0000052
        await UniTask.SwitchToThreadPool();



        // --- lineの長さ ---
        float lengthOfLine = _baseLengthOfLine * scaleToFitLocalPosition;

        // === 変数の初期化 ===
        Vector3 positionCurrentPoint = startPosition;

        // 線分を描画し続ける
        for (int i = 1; i < posisionCount; i++)
        {

            Vector3 forceResultant = MagneticPoleManager.Instance.GetMagneticPower(positionCurrentPoint);
            //distane2回計算してるので、あとで直す
            float minDistanceFromPole = MagneticPoleManager.Instance.GetMinDistanceFromPoles(positionCurrentPoint);
            if (minDistanceFromPole < distanceThresholdFromPole && i > 3)
            {
                break;
            }
            // --- 描画 ---
            if (lineIsFromNorthPole)
            {
                positionCurrentPoint += forceResultant.normalized * lengthOfLine;
            }
            else
            {
                positionCurrentPoint += -forceResultant.normalized * lengthOfLine;
            }

            positions[i] = positionCurrentPoint;
            directions[i] = forceResultant.normalized;
            isSkipDrawing[i] = false;
        }

        //処理取りやめる場合
        //0001400 or 0000045    

        //0002011
        await UniTask.SwitchToMainThread();

        for (int i = 1; i < posisionCount; i++)
        {
            if (positions[i] == Vector3.zero)
            {
                positions[i] = positions[i - 1];
            }
            magnetForceLine.SetPosition(i, positions[i]);
            if (i % _lineArrowInterval == 0)
            {
                int index = i / _lineArrowInterval;//+ (lineIsFromNorthPole ? 0 : lineArrowList.Count / 2);
                var arrow = lineArrowList[index];
                // if (distances[i] == 0)
                if (isSkipDrawing[i])
                {
                    arrow.SetActive(false);
                }
                else
                {
                    arrow.SetActive(true);
                    // arrow.transform.rotation = Quaternion.LookRotation(directions[index]);
                    arrow.transform.position = positions[i];
                    arrow.transform.LookAt(positions[i + 1]);
                    arrow.transform.Rotate(90, 0, lineIsFromNorthPole ? 0 : 180);
                }

            }
        }

        //0000301
    }
}