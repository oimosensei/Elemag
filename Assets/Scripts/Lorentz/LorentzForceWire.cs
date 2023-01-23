using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

[RequireComponent(typeof(CylinderBetweenTwoPoints))]
public class LorentzForceWire : MonoBehaviour
{
    public FloatReactiveProperty CurrentReactiveProperty = new FloatReactiveProperty(1.0f);
    //磁束密度
    public FloatReactiveProperty MagneticFluxDensityReactiveProperty = new FloatReactiveProperty(1.0f);

    private Rigidbody _rb;

    private CylinderBetweenTwoPoints _cylinderBetweenTwoPoints;

    private Vector3 initialPosition;

    private float radius = .3f; //ローレンツ力を計算する
    public Vector3 CalculateLorentzForce()
    {
        //電流の向き
        Vector3 currentDirection = transform.right;
        //磁場の向き
        Vector3 magneticFieldDirection = transform.up;
        //電流の大きさ
        float currentMagnitude = CurrentReactiveProperty.Value;
        //磁場の大きさ
        float magneticFieldMagnitude = MagneticFluxDensityReactiveProperty.Value;
        //ローレンツ力の計算
        Vector3 lorentzForce = currentMagnitude * magneticFieldMagnitude * Vector3.Cross(currentDirection, magneticFieldDirection);
        return lorentzForce;
    }
    public void OnButtonClickedTest()
    {
        Debug.Log("OnButtonClickedTest");
    }
    //係数
    public float coefficient = 1.0f;
    void FixedUpdate()
    {
        //ローレンツ力を計算する
        Vector3 lorentzForce = coefficient * CalculateLorentzForce();
        //ToDo 方向を導線が浮く方向にする
        //ローレンツ力を加える
        _rb.AddForce(lorentzForce);
        Vector3 beginPoint = initialPosition - Vector3.down * radius;
        Vector3 endPoint = transform.position;
        _cylinderBetweenTwoPoints.UpdateCylinderPosition(beginPoint, endPoint);

    }
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        initialPosition = transform.position;
        _cylinderBetweenTwoPoints = GetComponent<CylinderBetweenTwoPoints>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
