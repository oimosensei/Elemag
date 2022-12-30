using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        Observable.IntervalFrame(10)
            .Subscribe(_ =>
            {
                Debug.Log("test");
            });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
