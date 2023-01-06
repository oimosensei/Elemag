using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderBetweenTwoPoints : MonoBehaviour
{
    [SerializeField]
    private Transform cylinderPrefab;

    private GameObject _leftSphere;
    private GameObject _rightSphere;
    private GameObject _cylinder;

    private Vector3 _startPoint;

    private void Start()
    {
        _leftSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _rightSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _leftSphere.transform.position = new Vector3(-1, 0, 0);
        _rightSphere.transform.position = new Vector3(1, 0, 0);
        _startPoint = transform.parent.position;

        InstantiateCylinder(cylinderPrefab, _leftSphere.transform.position, _rightSphere.transform.position);
    }

    private void Update()
    {
    }

    private void InstantiateCylinder(Transform cylinderPrefab, Vector3 beginPoint, Vector3 endPoint)
    {
        _cylinder = Instantiate<GameObject>(cylinderPrefab.gameObject, Vector3.zero, Quaternion.identity);
        UpdateCylinderPosition(beginPoint, endPoint);
    }

    public void UpdateCylinderPosition(Vector3 beginPoint, Vector3 endPoint)
    {
        Vector3 offset = endPoint - beginPoint;
        Vector3 position = beginPoint + (offset / 2.0f);

        _cylinder.transform.position = position;
        _cylinder.transform.LookAt(beginPoint);
        Vector3 localScale = _cylinder.transform.localScale;
        localScale.z = (endPoint - beginPoint).magnitude;
        _cylinder.transform.localScale = localScale;
    }
}
