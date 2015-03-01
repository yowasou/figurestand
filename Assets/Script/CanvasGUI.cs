using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class CanvasGUI : MonoBehaviour 
{
    /// <summary>
    /// メインカメラ
    /// </summary>
    [SerializeField]
    GameObject MainCamera = null;
    /// <summary>
    /// フィギュア選択画面のスクロールボックス
    /// </summary>
    public GameObject ContentFigure;
    /// <summary>
    /// フィギュア・ポーズ選択用のボタン
    /// </summary>
    [SerializeField]
    RectTransform ButtonFigure = null;
    /// <summary>
    /// ポーズ選択画面のスクロールボックス
    /// </summary>
    public GameObject ContentPose;
    /// <summary>
    /// プリセット選択画面のスクロールボックス
    /// </summary>
    public GameObject ContentPreset;
    /// <summary>
    /// プリセット名の入力フィールド
    /// </summary>
    [SerializeField]
    GameObject InputFieldPreset;
    /// <summary>
    /// 実行中のポーズ名
    /// </summary>
    public string PoseName = string.Empty;
    public float PoseChangeWaitFrame = 0f;

    public string Title
    {
        get
        {
            return FindText("Title").text;
        }
        set
        {
            FindText("Title").text = value;
        }
    }
    protected virtual GameObject MainFigure
    {
        get
        {
            return GameObject.FindGameObjectWithTag("MainFigure");
        }
    }
    
    
    public Text FindText(string name)
    {
        foreach (Transform child in transform)
        {
            if (child.name == name)
            {
                return child.gameObject.GetComponent<Text>();
            }
        }
        return null;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void CloseMenu()
    {
        CanvasGUIEnable = false;
    }
    public void ExitApplication()
    {
        Application.Quit();
        return;
    }
    private bool _canvasGUIEnable;
    public bool CanvasGUIEnable
    {
        get { return _canvasGUIEnable; }
        set
        {
            _canvasGUIEnable = value;
            GetComponent<Canvas>().enabled = value;
            SelectPanel("PanelMain");
        }
    }
    public void LoadPoses()
    {
        SelectPanel("PanelPose");
        //メインフィギュア
        //リスト内容クリア
        GameObject[] oldButton = GameObject.FindGameObjectsWithTag("ButtonFigure");
        foreach (GameObject go in oldButton)
        {
            Destroy(go);
        }
        foreach (AnimationState anim in MainFigure.animation)
        {
            RectTransform item = GameObject.Instantiate(ButtonFigure) as RectTransform;
            item.SetParent(ContentPose.transform, false);
            Text text = item.GetComponentInChildren<Text>();
            text.text = anim.name;

            Button button = item.GetComponentInChildren<Button>();
            string name = anim.name;
            button.onClick.AddListener(() =>
            {
                SelectPose(name);
            });
        }
    }
    public void LoadResources()
    {
        SelectPanel("PanelFigure");
        GameObject[] objs = Resources.LoadAll<UnityEngine.GameObject>("Model");
        //リスト内容クリア
        GameObject[] oldButton = GameObject.FindGameObjectsWithTag("ButtonFigure");
        foreach (GameObject go in oldButton)
        {
            Destroy(go);
        }

		foreach(GameObject go in objs)
		{
            RectTransform item = GameObject.Instantiate(ButtonFigure) as RectTransform;
			item.SetParent(ContentFigure.transform, false);

            Text text = item.GetComponentInChildren<Text>();
            text.text = go.name;

            Button button = item.GetComponentInChildren<Button>();
            string name = go.name;
            button.onClick.AddListener(() =>
            {
                SelectFigure(name);
            });
		}
    }
    public void SelectPose(string poseName)
    {
        MainFigure.animation.Play(poseName);
        PoseName = poseName;
    }
    public void SelectFigure(string figureName)
    {
        Destroy(MainFigure);

        GameObject[] objs = Resources.LoadAll<UnityEngine.GameObject>("Model");
        foreach (GameObject go in objs)
        {
            if (go.name == figureName)
            {
                GameObject newFigure = (GameObject)GameObject.Instantiate(go
                    , new Vector3(0, 0, 0), Quaternion.identity);
                newFigure.tag = "MainFigure";
            }
        }
    }

    public void SelectPanel(string panelName)
    {
        foreach (Transform child in transform)
        {
            Image pnl = child.GetComponent<Image>();
            RectTransform rt = child.GetComponent<RectTransform>();
            if (pnl != null && pnl.enabled)
            {
                pnl.enabled = false;
                rt.localPosition = new Vector3(0, Screen.height, 0);
            }
            if (child.name == panelName)
            {
                pnl.enabled = true;
                rt.localPosition = new Vector3(0, 0, 0);
            }
        }
    }

    /// <summary>
    /// プリセット保存
    /// </summary>
    public void SavePreset()
    {
        PresetSave ps = new PresetSave(MainFigure, MainCamera, PoseName);
        ps.Save(InputFieldPreset.GetComponent<InputField>().text);
    }
    /// <summary>
    /// プリセットパネルに切り替え
    /// </summary>
    public void LoadPresetPanel()
    {
        SelectPanel("PanePresetLoad");
        //リスト内容クリア
        GameObject[] oldButton = GameObject.FindGameObjectsWithTag("ButtonFigure");
        foreach (GameObject go in oldButton)
        {
            Destroy(go);
        }

        PresetSave ps = new PresetSave(MainFigure, MainCamera, string.Empty);
        foreach(string s in ps.PresetList)
        {
            RectTransform item = GameObject.Instantiate(ButtonFigure) as RectTransform;
            item.SetParent(ContentPreset.transform, false);

            Text text = item.GetComponentInChildren<Text>();
            text.text = s;
            Button button = item.GetComponentInChildren<Button>();
            string name = s;
            button.onClick.AddListener(() =>
            {
                LoadPreset(name);
            });
            
        }
    }
    public void LoadPreset(string presetName)
    {
        PresetSave ps = new PresetSave(MainFigure, MainCamera, string.Empty);
        PresetSave.Preset p = ps.Load(presetName);
        SelectFigure(p.MainFigureName);
        CameraMove cm = MainCamera.GetComponent<CameraMove>();
        cm.RotateSpeed = p.RotateSpeed;
        cm.RotatePosObject.transform.position = ps.StrToVector3(p.RotatePosObjectPosition);
        MainCamera.transform.position = ps.StrToVector3(p.MainCameraPosition);
        MainCamera.transform.eulerAngles = ps.StrToVector3(p.MainCameraEulerAngles);
        MainFigure.animation.enabled = true;
        SelectPose(p.PoseName);
        PoseChangeWaitFrame = 10f;
    }

}
