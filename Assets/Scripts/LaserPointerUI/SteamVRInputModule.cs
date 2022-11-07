using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;

public class SteamVRInputModule : BaseInputModule
{
    private List<RaycastResult> _raycastResultsCache;
    private Camera _uiCamera;

    [SerializeField]
    private InputSource InputSourceLeft;

    [SerializeField]
    private InputSource InputSourceRight;

    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    [SerializeField]
    private SteamVR_Action_Boolean InteractUI = SteamVR_Input.GetBooleanAction("InteractUI");
    public SteamVR_Action_Boolean teleportAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Teleport");

    private List<InputSource> Poses => new List<InputSource> { InputSourceLeft, InputSourceRight };

    protected override void Start()
    {
        base.Start();

        if (InteractUI == null)
            Debug.LogError("No UI interaction action has been set on this component", this);
        InputSourceLeft.Initialize(eventSystem);
        InputSourceRight.Initialize(eventSystem);

        // Ray 照射用のカメラを作成する、実際に内容が描画されることはない
        _uiCamera = new GameObject("UI Camera").AddComponent<Camera>();
        _uiCamera.clearFlags = CameraClearFlags.Nothing;
        _uiCamera.cullingMask = 0;
        _uiCamera.enabled = false;
        _uiCamera.fieldOfView = 1;
        _uiCamera.nearClipPlane = 0.01f;

        // シーン上の全ての Canvas を引っ張ってきて、 UI Camera を判定に使用する
        foreach (var canvas in Resources.FindObjectsOfTypeAll<Canvas>())
            canvas.worldCamera = _uiCamera;
    }

    protected override void Awake()
    {
        base.Awake();

        _raycastResultsCache = new List<RaycastResult>();
    }

    public override void Process()
    {
        if (!InputSourceLeft.Validate() || !InputSourceRight.Validate())
            return;

        Poses.ForEach(ProcessEvents);
    }

    private void ProcessEvents(InputSource source)
    {
        // Controller がある位置にカメラを移動させる
        UpdateCameraPositionTo(source.Pose.transform);

        source.EventData.Reset();
        source.EventData.position = new Vector2(_uiCamera.pixelWidth * 0.5f, _uiCamera.pixelHeight * 0.5f);

        // Ray を照射して、一番手前にあるものを引っ張ってくる
        eventSystem.RaycastAll(source.EventData, _raycastResultsCache);
        source.EventData.pointerCurrentRaycast = FindFirstRaycast(_raycastResultsCache);
        // Receiver が設定されていたら、 Receiver に Raycast 情報を渡す
        source.Receiver?.OnUpdate(source.EventData.pointerCurrentRaycast);
        _raycastResultsCache.Clear();

        HandlePointerExitAndEnter(source.EventData, source.EventData.pointerCurrentRaycast.gameObject);

        // トリガーボタンが押されていれば
        if (InteractUI.GetState(source.Pose.inputSource))
        {
            // 前の値がなければ新規で押された場合なので、
            if (source.PreviousContactObject == null)
            {
                // 新規オブジェクトに Click 等を送信
                HandlePress(source);
            }
            // 別のオブジェクトへと判定が移動した場合は、
            else if (source.PreviousContactObject != source.EventData.pointerCurrentRaycast.gameObject)
            {
                // 古いオブジェクトはリリースし、新しいオブジェクトに Click 等を送信
                HandleRelease(source);
                HandlePress(source);
            }
            // 前の値と同じであれば、
            else
            {
                // ドラッグイベントを発行してあげる
                source.EventData.pointerPressRaycast = source.EventData.pointerCurrentRaycast;
                ExecuteEvents.Execute(source.EventData.pointerDrag, source.EventData, ExecuteEvents.dragHandler);
                ExecuteEvents.Execute(source.PreviousContactObject, source.EventData, ExecuteEvents.dragHandler);
            }

            return;
        }

        // ボタンが放されたらリリース
        if (source.PreviousContactObject)
            HandleRelease(source);
    }

    // ReSharper disable once ParameterHidesMember
    private void UpdateCameraPositionTo(Transform transform)
    {
        _uiCamera.transform.position = transform.position;
        _uiCamera.transform.rotation = transform.rotation;
    }

    private void HandlePress(InputSource source)
    {
        // press
        source.PreviousContactObject = source.EventData.pointerCurrentRaycast.gameObject;
        source.EventData.pointerPressRaycast = source.EventData.pointerCurrentRaycast;

        var pressed = ExecuteEvents.ExecuteHierarchy(source.PreviousContactObject, source.EventData, ExecuteEvents.pointerDownHandler);
        if (pressed == null)
        {
            // Button などの場合、重なっている Label などが取得されてしまうので、 Button 本体を引っ張ってくる
            pressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(source.PreviousContactObject);
            ExecuteEvents.Execute(source.PreviousContactObject, source.EventData, ExecuteEvents.pointerClickHandler);
            ExecuteEvents.Execute(source.PreviousContactObject, source.EventData, ExecuteEvents.beginDragHandler);
        }
        else
        {
            // 直接　Button など動きがあるものが取れていたならそれにそのままイベントを投げる
            ExecuteEvents.Execute(pressed, source.EventData, ExecuteEvents.pointerClickHandler);
            ExecuteEvents.Execute(pressed, source.EventData, ExecuteEvents.beginDragHandler);
            ExecuteEvents.Execute(source.PreviousContactObject, source.EventData, ExecuteEvents.pointerClickHandler);
            ExecuteEvents.Execute(source.PreviousContactObject, source.EventData, ExecuteEvents.beginDragHandler);
        }

        if (pressed != null)
        {
            source.EventData.pressPosition = pressed.transform.position;
            eventSystem.SetSelectedGameObject(pressed);
        }

        source.EventData.pointerPress = pressed;
        source.EventData.pointerDrag = pressed;
        source.EventData.rawPointerPress = source.PreviousContactObject;
    }

    private void HandleRelease(InputSource source)
    {
        // release
        ExecuteEvents.Execute(source.EventData.pointerPress, source.EventData, ExecuteEvents.pointerUpHandler);
        ExecuteEvents.Execute(source.EventData.pointerDrag, source.EventData, ExecuteEvents.endDragHandler);

        eventSystem.SetSelectedGameObject(null);

        source.EventData.pressPosition = Vector2.zero;
        source.EventData.pointerPress = null;
        source.EventData.pointerDrag = null;
        source.EventData.rawPointerPress = null;
        source.PreviousContactObject = null;
    }

    [System.Serializable]
    public class InputSource
    {
        public PointerEventData EventData { get; private set; }
        public GameObject PreviousContactObject { get; set; }

        public void Initialize(EventSystem eventSystem)
        {
            if (Pose == null)
                Debug.LogError("No SteamVR_Behaviour_Pose component found on this component");

            EventData = new PointerEventData(eventSystem);
            PreviousContactObject = null;
        }

        public bool Validate()
        {
            return Pose != null;
        }

        #region Pose

        [SerializeField]
        private SteamVR_Behaviour_Pose _pose;

        public SteamVR_Behaviour_Pose Pose
        {
            get => _pose;
            set => _pose = value;
        }

        #endregion

        #region Receiver

        [SerializeField]
        private LaserPointerRaycastReceiver _receiver;

        public LaserPointerRaycastReceiver Receiver
        {
            get => _receiver;
            set => _receiver = value;
        }

        #endregion
    }

}

