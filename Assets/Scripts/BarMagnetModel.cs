using UnityEngine;

public class BarMagnetModel : MonoBehaviour
{
    //磁力線が出る磁石の端っこの位置の目安となるGameObject
    public GameObject NorthPoleReference;
    public GameObject SouthPoleReference;
    public GameObject MagneticForceLinePrefab;
    //public bool IsDrawing;

    // Use this for initialization
    void Start()
    {
        /*         gameObject.GetComponent<BarMagnetModel>().NorthPoleReference =
                    transform.Find("North Body/North Pole").gameObject;
                gameObject.GetComponent<BarMagnetModel>().SouthPoleReference =
                    transform.Find("South Body/South Pole").gameObject; */


    }

}