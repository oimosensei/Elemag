using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.EventSystems;

public class AvatarLaserPointer : LaserPointerRaycastReceiver
{
    private GameObject _laser;
    private GameObject _pointer;

    [SerializeField]
    public SteamVR_Action_Boolean teleportAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Teleport");
    [SerializeField]
    private Material LaserMaterial;

    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    [SerializeField]
    private float LaserThickness = 0.001f;

    [SerializeField]
    private Material PointerMaterial;

    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    [SerializeField]
    private float PointerRadius = 0.05f;

    private LineRenderer _lineRenderer;
    public override void OnUpdate(RaycastResult raycast)
    {
        if (raycast.gameObject)
        {
            // Ray が Hit したところまで描画してあげる
            if (_laser != null)
            {
                _laser.transform.localScale = new Vector3(LaserThickness * 4f, LaserThickness * 4f, raycast.distance);
                _laser.transform.localPosition = new Vector3(0f, 0f, raycast.distance / 2f);
                // _lineRenderer.SetPosition(0, transform.position);
                // _lineRenderer.SetPosition(1, raycast.worldPosition);
            }
            if (_pointer != null)
            {
                _pointer.transform.position = raycast.worldPosition;
                _pointer.SetActive(true);
            }
        }
        else
        {
            if (_laser != null)
            {
                _laser.transform.localScale = new Vector3(LaserThickness, LaserThickness, 0f);
                _laser.transform.localPosition = new Vector3(0f, 0f, 0f);
                // _lineRenderer.SetPosition(0, transform.position);
                // _lineRenderer.SetPosition(1, transform.position);
            }

            if (_pointer != null)
                _pointer.SetActive(false);
        }
    }

    private void Start()
    {
        if (LaserMaterial == null)
            Debug.LogWarning("No Laser Material found on this component", this);
        if (PointerMaterial == null)
            Debug.LogWarning("No Pointer Material found on this component", this);

        _laser = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _laser.transform.parent = transform;
        _laser.transform.localScale = new Vector3(LaserThickness, LaserThickness, 100f);
        _laser.transform.localPosition = new Vector3(0f, 0f, 50f);
        _laser.transform.localRotation = Quaternion.identity;
        _laser.GetComponent<MeshRenderer>().material = LaserMaterial;

        // _lineRenderer = gameObject.AddComponent<LineRenderer>();
        // _lineRenderer.material = LaserMaterial;
        // _lineRenderer.startWidth = LaserThickness;
        // _lineRenderer.endWidth = LaserThickness;
        // _lineRenderer.positionCount = 2;
        // _lineRenderer.SetPosition(0, transform.position);
        // _lineRenderer.SetPosition(1, transform.position);

        _pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _pointer.transform.parent = transform;
        _pointer.transform.localScale = new Vector3(PointerRadius, PointerRadius, PointerRadius);
        _pointer.transform.localPosition = new Vector3(0f, 0f, 0f);
        _pointer.GetComponent<MeshRenderer>().material = PointerMaterial;
        _pointer.SetActive(false);
    }
}
