using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading;

public class Coil : MonoBehaviour
{
    float width = 1;
    float height = 1;

    float space = 0.1f;
    // Start is called before the first frame update

    float pervAllJisoku = 0;

    float deltaAllJisoku = 0;

    public ReadOnlyReactiveProperty<float> deltaAllJisokuRP;

    public Slider slider;

    public Light light;

    public float coefficient = 0.01f;

    public MeshRenderer mr;
    Material mat;

    public Text text;
    void Start()
    {
        mat = mr.material;
        mat.EnableKeyword("_EMISSION");
        Observable.IntervalFrame(3, FrameCountType.FixedUpdate)
        .Subscribe(_ =>
         {
             float newAllJisoku = calcAllJisoku();
             if (newAllJisoku * pervAllJisoku < 0) pervAllJisoku = 0;
             deltaAllJisoku = newAllJisoku - pervAllJisoku;
             pervAllJisoku = newAllJisoku;
             if (slider != null)
                 slider.value = deltaAllJisoku;
             light.intensity = coefficient * Mathf.Abs(deltaAllJisoku);
             float factor = Mathf.Pow(2, Mathf.Min(coefficient * 12 * Mathf.Abs(deltaAllJisoku) - 3, 7.5f));
             mat.SetColor("_EmissionColor", new Color(0.97254906f * factor, 0.3803922f * factor, 0.05098039f * factor));

             if (text != null)
                 text.text = pervAllJisoku.ToString();
         })
         .AddTo(this);

        deltaAllJisokuRP = this.ObserveEveryValueChanged(x => x.deltaAllJisoku).ToReadOnlyReactiveProperty();

        Observable.Interval(System.TimeSpan.FromSeconds(1))
        .Subscribe(_ => Debug.Log(deltaAllJisoku))
        .AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public float calcAllJisoku()
    {
        float sumJisoku = 0;
        width = 10 * transform.localScale.x;
        height = 10 * transform.localScale.y;

        Vector3 leftup = transform.rotation * (transform.position - new Vector3(width / 2, 0, height / 2));
        Vector3 rightDirVec = transform.rotation * Vector3.right;
        Vector3 forwardDirVec = transform.rotation * Vector3.forward;
        for (float x = -width / 2; x < width / 2; x += space)
        {
            for (float z = -height / 2; z < height / 2; z += space)
            {
                Vector3 position = leftup + x * rightDirVec + z * forwardDirVec;
                sumJisoku += getJisoku(position);
            }
        }
        return sumJisoku;
    }
    public float getJisoku(Vector3 position)
    {
        var a = transform.rotation * Vector3.down;
        float b = Vector3.Dot(MagneticPoleManager.Instance.getMagneticPower(position), a);
        return b;
    }
}
