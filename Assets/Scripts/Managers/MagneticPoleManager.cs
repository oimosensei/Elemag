using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticPoleManager : MonoBehaviour
{
    // Start is called before the first frame update
    private static MagneticPoleManager instance;

    public static MagneticPoleManager Instance
    {
        get
        {
            //nullチェック
            if (null == instance)
            {
                instance = (MagneticPoleManager)FindObjectOfType(typeof(MagneticPoleManager));
                if (null == instance)
                {
                    Debug.Log(" MagneticPoleManager Instance Error ");
                }
            }
            return instance;
        }
    }
    [HideInInspector] public List<MagneticPole> MagneticPoles = new List<MagneticPole>();

    // private List<Vector3> MagneticPolesPosition = new List<Vector3>();
    public void Add(MagneticPole mp)
    {
        MagneticPoles.Add(mp);

    }
    public void Remove(MagneticPole mp)
    {
        MagneticPoles.Remove(mp);

    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdatePolesPositions();
    }
    /// <summary>
    /// ある位置でのクーロン力を計算する     
    /// </summary>
    /// <param name="posision"></param>
    /// <returns></returns>
    public Vector3 GetMagneticPower(Vector3 posision)
    {
        Vector3 power = Vector3.zero;
        foreach (var magneticPole in MagneticPoles)
        {
            // N極からの現在の頂点への変位ベクトル(ベクトルn)
            var displacement = posision - magneticPole.positionCache;

            // ベクトルnの長さの2乗（これで単位ベクトルを割る）
            var lengthSquare =
            displacement.sqrMagnitude;

            power += (magneticPole.isN ? 1 : -1) * magneticPole.power / lengthSquare * displacement.normalized;
        }
        return power;
    }

    public float GetMinDistanceFromPoles(Vector3 posision)
    {
        float minDistance = float.MaxValue;
        foreach (var magneticPole in MagneticPoles)
        {
            float distance = Vector3.Distance(posision, magneticPole.positionCache);
            if (distance < minDistance)
            {
                minDistance = distance;
            }
        }
        return minDistance;
    }

    public void UpdatePolesPositions()
    {
        // MagneticPolesPosition.Clear();
        foreach (var magneticPole in MagneticPoles)
        {
            // MagneticPolesPosition.Add(magneticPole.transform.position);
            magneticPole.UpdatePositionCache();
        }

    }

}
