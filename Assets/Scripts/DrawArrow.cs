using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class DrawArrow : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject startPoint;
    public GameObject endPoint;
    public float width;
    public float arrowWidth;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        LineSegment.Draw(new LineInfo
        {
            startPos = startPoint.transform.position,
            endPos = endPoint.transform.position,
            fillColor = Color.red,
            forward = Vector3.forward,
            width = width,
            endArrow = true,
            arrowWidth = arrowWidth,
            arrowLength = arrowWidth

        });

    }
}
