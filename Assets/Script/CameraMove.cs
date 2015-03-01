using UnityEngine;
using System.Collections;

public class CameraMove : CameraConst
{
    //プロパティ
    private float _rotateSpeed = 10f;
    public float RotateSpeed
    {
        get { return _rotateSpeed; }
        set { 
            _rotateSpeed = value;
            KaitenSlider.GetComponent<UnityEngine.UI.Slider>().value = _rotateSpeed;
        }
    }
    public float RotateManualSpeed = 10f;
    public GameObject RotatePosObject;
    public GameObject CanvasGUIObject;
    private CanvasGUI canvas;
    private bool _rotatePosEnable;
    public bool RotatePosEnable
    {
        get { return _rotatePosEnable; }
        set { 
            _rotatePosEnable = value;
            GlobalFunctions.SetVisible(RotatePosObject, value);
        }
    }
    /// <summary>
    /// 回転速度スライダ
    /// </summary>
    [SerializeField]
    GameObject KaitenSlider = null;

    //ローカル変数
    private InputOnUpdate iou = null;
    private float menuWaitInterval = 0f;

	// Use this for initialization
	void Start () {
        iou = new InputOnUpdate(Camera.main);
        iou.RotatePosObject = RotatePosObject;
        //最初は回転中心非表示に
        RotatePosEnable = false;
        //最初はGUI非表示に
        canvas = CanvasGUIObject.GetComponent<CanvasGUI>();
        canvas.CanvasGUIEnable = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (canvas.PoseChangeWaitFrame > 0f)
        {
            canvas.PoseChangeWaitFrame = canvas.PoseChangeWaitFrame - Time.fixedDeltaTime;
            canvas.SelectPose(canvas.PoseName);
        }
        if (!canvas.CanvasGUIEnable)
        {
            iou.Update();
        }
        else
        {
            ///クリックしていないことにする（こうしないとメニュー表示時に回転しない
            iou.Clicked = false;
        }
        if (iou.Pinch != 0f)
        {
            float distance = Vector3.Distance(iou.RotatePos, transform.position);
            if (iou.Pinch > 0 && distance < 0.5f)
            {
                return;
            }
            transform.position = transform.position + ((transform.position - iou.RotatePos) * iou.Pinch * -1);
        }
        if (!iou.Clicked && (iou.Pinch == 0f) && !RotatePosEnable)
        {
            //回転も拡大も視点変更もしていない時は回転
            transform.RotateAround(iou.RotatePos, Vector3.up, RotateSpeed * Time.deltaTime);
            menuWaitInterval = 0f;
        }
        else
        {
            if (iou.Offset.x != 0 || iou.Offset.y != 0)
            {
                if (iou.DoubleTouched)
                {
                    transform.RotateAround(iou.RotatePos, Vector3.up, RotateManualSpeed * iou.Offset.x * Time.deltaTime);
                    transform.RotateAround(iou.RotatePos, Vector3.right, RotateManualSpeed * iou.Offset.y * Time.deltaTime);   
                }
                else if (iou.Pinch == 0f)
                {
                    if (RotatePosEnable)
                    {
                        RotatePosObject.transform.position = iou.MovedRotatePos;
                    }
                    else
                    {
                        transform.position = iou.MovedCameraPos;
                    }
                }
                menuWaitInterval = 0f;
            }
            else
            {
                //クリックしているが動いていなかった場合、メニューを出すためのwait値を加算する
                menuWaitInterval += Time.deltaTime;
                if (menuWaitInterval > MenuWait)
                {
                    canvas.CanvasGUIEnable = true;
                    menuWaitInterval = 0f;
                }
            }
        }
	}
    public void RotatePosEnableChange()
    {
        RotatePosEnable = !RotatePosEnable;
    }
}

