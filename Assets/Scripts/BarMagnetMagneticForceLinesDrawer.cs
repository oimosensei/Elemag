using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;

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

    BarMagnetModel barMagnetModel;


    /// <summary>
    /// 磁力線を描画中か管理するフラグ(public)　オフになったら自動的にLnePrefabを消す
    /// </summary>
    public BoolReactiveProperty IsDrawing = new BoolReactiveProperty(true);

    List<LineRenderer> magneticForceLines = null;
    List<float> listStartY;
    List<float> listStartZ;


    Transform[] southPolesTransform;
    Transform[] northPolesTransform;

    int numOfCalclation = 0;

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

    public void GenerateLines()
    {
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
                lineGameObject.tag = "CloneLine";
                lineGameObject.transform.parent = transform;
                var line = lineGameObject.GetComponent<LineRenderer>();
                InitializeLineRenderer(line);
                magneticForceLines.Add(line);

                lineGameObject = Instantiate(magneticForceLinePrefab, transform.position, Quaternion.identity);
                // 作成したオブジェクトを子として登録
                lineGameObject.tag = "CloneLine";
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

    public void DrawLoop(bool lineIsFromNorthPole, Vector3 polePosInWorld)
    {
        Vector3 barMagnetDirection = transform.rotation.eulerAngles;

        //S極だったらcountを半分から始める
        int count = (lineIsFromNorthPole ? 0 : (int)magneticForceLines.Count / 2);

        var rotation = gameObject.transform.rotation;

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

                tasks[count] = DrawOneLine(
                    magneticForceLines[count],
                    lineIsFromNorthPole,
                    polePosInWorld + rotation * shiftPositionFromMyPole);

                count++;
            }
        }

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
    public async UniTask DrawOneLine(LineRenderer magnetForceLine, bool lineIsFromNorthPole, Vector3 startPosition)
    {
        // --- LineRendererの始点を初期位置にセットする ---
        magnetForceLine.SetPosition(0, startPosition);  // 引数の(x, y, z)を始点として磁力線を描く
        var positions = new Vector3[numLines];
        await UniTask.SwitchToThreadPool();

        // --- lineの長さ ---
        float lengthOfLine = baseLengthOfLine * scaleToFitLocalPosition;

        // === 変数の初期化 ===
        Vector3 positionCurrentPoint = startPosition;

        // 線分を描画し続ける
        for (int i = 1; i < magnetForceLine.positionCount; i++)
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

            numOfCalclation++;
            positions[i] = positionCurrentPoint;
        }

        await UniTask.SwitchToMainThread();
        for (int i = 1; i < magnetForceLine.positionCount; i++)
        {
            magnetForceLine.SetPosition(i, positions[i]);
        }

    }
}