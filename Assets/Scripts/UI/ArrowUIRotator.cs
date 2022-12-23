using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowUIRotator : MonoBehaviour
{
    // Start is called before the first frame update
    private Coil _coil;

    private GameObject _clockwiseArrow;
    private GameObject _counterclockwiseArrow;
    void Start()
    {
        _coil = GameObject.Find("Coil").GetComponent<Coil>();
        _clockwiseArrow = transform.Find("ClockwiseArrow").gameObject;
        _counterclockwiseArrow = transform.Find("CounterclockwiseArrow").gameObject;

    }

    // Update is called once per frame
    void Update()
    {
        var speed = _coil.deltaAllJisokuRP.Value * 1000000;
        if (speed > 0)
        {
            _clockwiseArrow.SetActive(true);
            _counterclockwiseArrow.SetActive(false);
        }
        else
        {
            _clockwiseArrow.SetActive(false);
            _counterclockwiseArrow.SetActive(true);
        }
        //スピードに応じてY軸方向に回転
        transform.Rotate(0, 0, speed * Time.deltaTime);
        //ToDo; スピードの上限と下限を設定する
    }
}
