using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;
using System.Xml;

public class PresetSave
{
    public class Preset
    {
        public string MainFigureName;
        public string PoseName;
        public float RotateSpeed;
        public string MainCameraPosition;
        public string MainCameraEulerAngles;
        public string RotatePosObjectPosition;
    }
    public System.Collections.Generic.List<string> presets = new System.Collections.Generic.List<string>();
    GameObject MainFigure;
    GameObject MainCamera;
    string _poseName;
    public string PoseName
    {
        get { return _poseName; }
        set
        {
            _poseName = value;
            if (_poseName == string.Empty)
            {
                _poseName = MainFigure.animation.clip.name;
            }
        }
    }
    string MainFigureName
    {
        get
        {
            if (MainFigure.name.EndsWith("(Clone)",StringComparison.Ordinal))
            {
                return MainFigure.name.Substring(0, MainFigure.name.Length - 7);
            }
            else
            {
                return MainFigure.name;
            }
        }
    }
    public PresetSave(GameObject _mainFigure,GameObject _mainCamera, string _poseName)
    {
        MainFigure = _mainFigure;
        MainCamera = _mainCamera;
        PoseName = _poseName;
    }
    public string[] PresetList
    {
        get
        {
            if (!Directory.Exists(Application.persistentDataPath))
            {
                Directory.CreateDirectory(Application.persistentDataPath);
            }
            string[] fileList = Directory.GetFiles(Application.persistentDataPath);
            for (int i = 0; i < fileList.Length;i++)
            {
                fileList[i] = Path.GetFileNameWithoutExtension(fileList[i]);
            }
            return fileList;
        }
    }
    public Preset Load(string presetName)
    {
        string path = GetPresetPath(presetName);
        System.Xml.Serialization.XmlSerializer serializer =
            new System.Xml.Serialization.XmlSerializer(typeof(Preset));
        System.IO.StreamReader sr = new System.IO.StreamReader(
            path, new System.Text.UTF8Encoding(false));
        try
        {
            return (Preset)serializer.Deserialize(sr);
        }
        finally
        {
            sr.Close();
        }
    }

    protected virtual string GetPresetPath(string presetName)
    {
        if (string.IsNullOrEmpty(presetName))
        {
            presetName = "nanashi";
        }
        string path = Application.persistentDataPath + "/" + presetName + ".dat";
        return path;
    }
    public void Save(string presetName)
    {
        CameraMove cm = MainCamera.GetComponent<CameraMove>();
        //メインフィギュア名
        //ポーズ名
        //回転速度
        //カメラ位置・角度
        //回転中心位置
        Preset p = new Preset();
        p.MainFigureName = MainFigureName;
        p.PoseName = PoseName;
        p.RotateSpeed = cm.RotateSpeed;
        p.MainCameraPosition = Vector3ToStr(MainCamera.transform.position);
        p.MainCameraEulerAngles = Vector3ToStr(MainCamera.transform.eulerAngles);
        p.RotatePosObjectPosition = Vector3ToStr(cm.RotatePosObject.transform.position);

        if (!Directory.Exists(Application.persistentDataPath))
        {
            Directory.CreateDirectory(Application.persistentDataPath);
        }
        if (string.IsNullOrEmpty(presetName))
        {
            presetName = "nanashi";
        }
        string path = GetPresetPath(presetName);
        System.Xml.Serialization.XmlSerializer serializer =
            new System.Xml.Serialization.XmlSerializer(typeof(Preset));
        System.IO.StreamWriter sw = new System.IO.StreamWriter(
            path, false, new System.Text.UTF8Encoding(false));
        serializer.Serialize(sw, p);
        sw.Close();
       
    }


    public string Vector3ToStr(Vector3 v3)
    {
        return Convert.ToString(v3.x) + "," + Convert.ToString(v3.y) + "," + Convert.ToString(v3.z);
    }
    public Vector3 StrToVector3(string s)
    {
        string[] lst = s.Split(new char[] { ',' });
        return new Vector3(float.Parse(lst[0]), float.Parse(lst[1]), float.Parse(lst[2]));
    }
}
