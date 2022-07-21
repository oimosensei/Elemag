using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticPole : MonoBehaviour
{
    // Start is called before the first frame update
    public float power = 1;

    public bool isN = true;

    private Vector3 initialPosition;
    void Start()
    {
        MagneticPoleManager.Instance.Add(this);
        initialPosition = transform.position;
    }

    private void OnDestroy()
    {
        MagneticPoleManager.Instance.Remove(this);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
