using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

// Todo: 後でクラス名をRenameする
// Todo: 後でStartとUpdateからメソッドを外出しする
/// <summary>
/// 磁力線の描画を行う
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

        //listStartY = new List<float> { -0.02f, -0.002f, 0, 0.002f, 0.02f };
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

    public void Update()
    {
        // Debug.Log("Update");
        if (IsDrawing.Value)
        {
            //必要なら磁力線の初期化
            if (magneticForceLines.Count == 0)
                GenerateLines();

            //N極磁力線の描画
            DrawLoop(true, barMagnetModel.NorthPoleReference.transform.position);

            //S極磁力線の描画
            DrawLoop(false, barMagnetModel.SouthPoleReference.transform.position);


        }
    }

    private void InitializeLineRenderer(LineRenderer magnetForceLine)
    {
        // === LineRendererを設定する ===
        // --- LineRendererを初期化する ---
        magnetForceLine.useWorldSpace = true;
        magnetForceLine.positionCount = numLines;

        // --- lineの太さ ---
        magnetForceLine.startWidth = widthLines;
        magnetForceLine.endWidth = widthLines;
    }

    private List<List<GameObject>> lineArrowGameObjectsList = new List<List<GameObject>>();
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

                var lineGameObject = Instantiate(magneticForceLinePrefab, transform.position, Quaternion.identity);
                // 作成したオブジェクトを子として登録
                //lineGameObject.tag = "CloneLine";
                lineGameObject.transform.parent = transform;
                var line = lineGameObject.GetComponent<LineRenderer>();
                InitializeLineRenderer(line);
                magneticForceLines.Add(line);
                int lineArrowCount = numLines / _lineArrowInterval;
                //これいるかわからん
                lineArrowCount = lineArrowCount % 2 == 0 ? lineArrowCount : lineArrowCount + 1;
                List<GameObject> lineArrowGameObjects = new List<GameObject>();
                for (int i = 0; i < lineArrowCount; i++)
                {
                    var lineArrowGameObject = Instantiate(lineArrowPrefab, transform.position, Quaternion.identity);
                    // 作成したオブジェクトを子として登録
                    lineArrowGameObject.transform.parent = transform;
                    lineArrowGameObjects.Add(lineArrowGameObject);
                }
                lineArrowGameObjectsList.Add(lineArrowGameObjects);



                lineGameObject = Instantiate(magneticForceLinePrefab, transform.position, Quaternion.identity);
                // 作成したオブジェクトを子として登録
                // lineGameObject.tag = "CloneLine";
                lineGameObject.transform.parent = transform;
                line = lineGameObject.GetComponent<LineRenderer>();
                InitializeLineRenderer(line);
                magneticForceLines.Add(line);
            }
        }
        Debug.Log(MagneticPoleManager.Instance.MagneticPoles.Count());
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
    }

    public async void DrawLoop(bool lineIsFromNorthPole, Vector3 polePosInWorld)
    {
        // if (processing) return;
        // Debug.Log("DrawLoop");
        processing = true;
        Vector3 barMagnetDirection = transform.rotation.eulerAngles;

        //S極だったらcountを半分から始める
        int initialCount = (lineIsFromNorthPole ? 0 : (int)magneticForceLines.Count / 2);
        int count = initialCount;

        var rotation = gameObject.transform.rotation;
        MagneticPoleManager.Instance.UpdatePolesPositions();
        UniTask[] tasks = new UniTask[listStartY.Count * listStartZ.Count];
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
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
                lineArrowGameObjectsList[count - initialCount],
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
        // Debug.Log("DrawLoop end");

    }

    float scaleToFitLocalPosition = 0.15f;

    /// <summary>
    /// 線分の長さ
    /// 調節してN極から出た磁力線とS極から出た磁力線が一致するようにする
    /// </summary>
    [SerializeField] float baseLengthOfLine = 0.01f;

    /// <summary>
    /// 描く線分の数
    /// </summary>
    [SerializeField] int numLines = 55;

    /// <summary>
    /// 描く線分の太さ
    /// 調節してN極から出た磁力線とS極から出た磁力線が一致するようにする
    /// </summary>
    [SerializeField] float widthLines = 0.005f;


    /// <summary>
    /// 引数の(x, y, z)を始点として磁力線を描く
    /// </summary>
    public async UniTask DrawOneLine(LineRenderer magnetForceLine, List<GameObject> lineArrowList, bool lineIsFromNorthPole, Vector3 startPosition)
    {
        //時間の計測開始
        // var sw = new System.Diagnostics.Stopwatch();
        // sw.Start();
        // --- LineRendererの始点を初期位置にセットする ---
        magnetForceLine.SetPosition(0, startPosition);  // 引数の(x, y, z)を始点として磁力線を描く
        var positions = new Vector3[numLines];
        var directions = new Vector3[numLines];
        var posisionCount = magnetForceLine.positionCount;

        await UniTask.SwitchToThreadPool();


        //現在のスレッドのIDをLOGに出力
        // Debug.Log("Thread ID:" + Thread.CurrentThread.ManagedThreadId);

        // --- lineの長さ ---
        float lengthOfLine = baseLengthOfLine * scaleToFitLocalPosition;

        // === 変数の初期化 ===
        Vector3 positionCurrentPoint = startPosition;

        // 線分を描画し続ける
        for (int i = 1; i < posisionCount; i++)
        {

            Vector3 forceResultant = MagneticPoleManager.Instance.GetMagneticPower(positionCurrentPoint);

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
        }

        await UniTask.SwitchToMainThread();
        for (int i = 1; i < magnetForceLine.positionCount; i++)
        {
            magnetForceLine.SetPosition(i, positions[i]);
        }
        //計測を終了する
        // sw.Stop();
        // //結果を表示する
        // Debug.Log(sw.Elapsed);

    }
    public async UniTask DrawOneLineSync(LineRenderer magnetForceLine, List<GameObject> lineArrowList, bool lineIsFromNorthPole, Vector3 startPosition)
    {
        // --- LineRendererの始点を初期位置にセットする ---
        magnetForceLine.SetPosition(0, startPosition);  // 引数の(x, y, z)を始点として磁力線を描く
        var positions = new Vector3[numLines];
        var posisionCount = magnetForceLine.positionCount;
        //現在のスレッドのIDをLOGに出力
        // Debug.Log("Thread ID:" + Thread.CurrentThread.ManagedThreadId);

        // --- lineの長さ ---
        float lengthOfLine = baseLengthOfLine * scaleToFitLocalPosition;

        // === 変数の初期化 ===
        Vector3 positionCurrentPoint = startPosition;

        // 線分を描画し続ける
        for (int i = 1; i < posisionCount; i++)
        {

            Vector3 forceResultant = MagneticPoleManager.Instance.GetMagneticPower(positionCurrentPoint);

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
        }

        for (int i = 1; i < magnetForceLine.positionCount; i++)
        {
            magnetForceLine.SetPosition(i, positions[i]);
            // if (i % _lineArrowInterval == 0)
            // {
            //     var arrow = lineArrowList[i];
            //     arrow.transform.rotation = Quaternion.LookRotation(forceResultant);
            //     arrow.transform.position = positionCurrentPoint;
            // }
        }
    }
}