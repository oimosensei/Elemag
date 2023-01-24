using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticPole : MonoBehaviour
{
    // Start is called before the first frame update
    public float power = 1;

    public bool isN = true;


    public Vector3 positionCache;

    private void OnEnable()
    {
        MagneticPoleManager.Instance.Add(this);
    }
    private void OnDisable()
    {
        MagneticPoleManager.Instance.Remove(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdatePositionCache()
    {
        positionCache = transform.position;
    }
}
