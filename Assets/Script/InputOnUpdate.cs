using UnityEngine;
using System.Collections;

public class InputOnUpdate {
    public Vector3 ClickedPoint;
    public Vector3 MousePoint;
    public Vector3 Offset;
    public Camera MainCamera;
    public Vector3 ClickedCameraPos;
    public Vector3 MovedCameraPos;
    public Vector3 ClickedRotatePos;
    public Vector3 MovedRotatePos;
    public GameObject RotatePosObject;
    public Vector3 RotatePos
    {
        get { return RotatePosObject.transform.position; }
        set { RotatePosObject.transform.position = value; }
    }
    private int touchCount;
    public int TouchCount
    {
        get { return TouchCount; }
        set { TouchCount = value; }
    }

    public bool Clicked = false;
    public bool DoubleTouched = false;
    public float Pinch;

    private float pinchInterval = 0f;
    private float clickInterval = 0f;

    public InputOnUpdate(Camera _mainCamera)
    {
        MainCamera = _mainCamera;
        Pinch = 0f;
    }

    public void Update()
    {
        touchCount = Input.touchCount;
        Vector3 newMousePoint = Input.mousePosition;
        if (touchCount == 2)
        {
            newMousePoint = (Input.touches[0].position + Input.touches[1].position) / 2;
        }
        Offset = MousePoint - newMousePoint;
        MousePoint = newMousePoint;
        if (MousePoint.x < 0 || MousePoint.y < 0 ||
            MousePoint.y > Screen.height || MousePoint.x > Screen.width)
        {
            //マウスが画面外に行ったらクリック状態をOFFに
            Clicked = false;
            return;
        }
        if (Input.GetMouseButtonDown(0) && !Clicked)
        {
            Clicked = true;
            InitMouseClickedPoint();
        }
        if (Input.GetMouseButtonUp(0) && Clicked)
        {
            Clicked = false;
        }

        if ((touchCount > 1) || Input.GetKey(KeyCode.LeftShift))
        {
            DoubleTouched = true;
        }
        else
        {
            if (DoubleTouched)
            {
                //2点タッチが終わったら移動しないように仕向ける
                InitMouseClickedPoint();
            }
            DoubleTouched = false;
        }
        if (touchCount == 2)
        {
            float tmpInterval = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
            if (Input.touches[0].phase == TouchPhase.Began || Input.touches[1].phase == TouchPhase.Began)
            {
                InitMouseClickedPoint();
                pinchInterval = tmpInterval;
            }
            Pinch = (pinchInterval - tmpInterval) * 0.005f;
            pinchInterval = tmpInterval;
        }
        else
        {
            Pinch = Input.GetAxis("Mouse ScrollWheel");
        }

        if (Clicked)
        {
            MoveCamera();
        }
    }

    private void InitMouseClickedPoint()
    {
        ClickedPoint = MousePoint;
        ClickedCameraPos = MainCamera.transform.position;
        ClickedRotatePos = RotatePosObject.transform.position;
        Offset = Vector3.zero;
    }

    private void MoveCamera()
    {
        MousePoint.z = Vector3.Distance(RotatePosObject.transform.position
            , MainCamera.transform.position);
        ClickedPoint.z = MousePoint.z;
        Vector3 movedPos = MainCamera.ScreenToWorldPoint(ClickedPoint)
            - MainCamera.ScreenToWorldPoint(MousePoint);
        MovedCameraPos = ClickedCameraPos + movedPos;
        MovedRotatePos = ClickedRotatePos + movedPos;
    }
    public void ClearClickedPoint()
    {
        this.ClickedPoint = Input.mousePosition;
    }


}
