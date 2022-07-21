using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetMover : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 initialPosition;

    public bool ismove = true;
    void Start()
    {
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }
    void FixedUpdate()
    {
        if (ismove)
            transform.position = initialPosition + new Vector3(0, 2 * Mathf.Sin(4 * Time.time), 0);
    }
}
