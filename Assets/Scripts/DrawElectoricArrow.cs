using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using UniRx;
using Cysharp.Threading.Tasks;

public class DrawElectoricArrow : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject startPoint;
    public GameObject endPoint;
    public Coil coil;

    public float width;
    public float arrowWidth;
    bool reverse = false;
    void Start()
    {
        UniTask.Create(async () =>
        {
            await UniTask.NextFrame();
            coil.deltaAllJisokuRP.Subscribe(x =>
            {
                reverse = x < 0;
            });
        });

    }

    // Update is called once per frame
    void Update()
    {
        LineSegment.Draw(new LineInfo
        {
            startPos = startPoint.transform.position,
            endPos = endPoint.transform.position,
            fillColor = Color.yellow,
            forward = Vector3.forward,
            width = width,
            endArrow = !reverse,
            startArrow = reverse,
            arrowWidth = arrowWidth,
            arrowLength = arrowWidth

        });

    }
}
