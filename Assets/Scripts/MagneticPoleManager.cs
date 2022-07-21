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
    List<MagneticPole> MagneticPoles = new List<MagneticPole>();

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

    }
    /// <summary>
    /// ある位置でのクーロン力を計算する     
    /// </summary>
    /// <param name="posision"></param>
    /// <returns></returns>
    public Vector3 getMagneticPower(Vector3 posision)
    {
        Vector3 power = Vector3.zero;
        foreach (var magneticPole in MagneticPoles)
        {
            float distance = Vector3.Distance(posision, magneticPole.transform.position);
            power += magneticPole.power / Mathf.Pow(distance, 2) * (posision - magneticPole.transform.position);
        }
        return power;
    }

}
