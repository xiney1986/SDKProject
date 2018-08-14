using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.IO.IsolatedStorage;

/**配置文件管理器
  *负责配置文件的读取 
  *当前为直接读取配置文件 后期可修改为读取二进制文件
  *配置文件统一用行描述 一行一描述 
  *#为行注释
  *@author longlingquan
  **/
public class ConfigManager
{
    private const string NOTE = "#";
    private const string SUFFIX = ".txt";

    public ConfigManager()
    {

    }

    //读取配置文件
    public void readConfig(string name)
    {
        if (Log.useUnityLog)
        {
            //如果存在版本号，就从基础配置名字里面获取对应的版本配置名
            if (GameManager.Instance.isHaveVersion())
            {
                name = name + "_" + GameManager.CONFIG_VERSION;
            }

            ResourcesData rd = ResourcesManager.Instance.getResource("Config");
            if (rd != null && !ResourcesManager.Instance.allowLoadFromRes)
            {
                if (GameManager.Instance.isHaveVersion())
                {
                    //读取版本配置
                    resolveConfigTextAsset(rd, name);
                }
                //读取基础配置
                string defaultName2 = name.Replace("_" + GameManager.CONFIG_VERSION, "");
                resolveConfigTextAsset(rd, defaultName2);
                //读取运营版配置
                string defaultName3 = defaultName2 + "_v";
                resolveConfigTextAsset(rd, defaultName3);

            }
            else
            {
                string path = ConfigGlobal.CONFIG_FOLDER + "/" + name;
                readConfigFromResources(path);
            }
        }
        else
        {
            string path = "./../../../unity/Assets/Resources/" + ConfigGlobal.CONFIG_FOLDER + "/" + name;
            string oldPath = path.Replace("_" + GameManager.CONFIG_VERSION, "");
            readTextAsset1(oldPath);
            readTextAsset1(oldPath + "_v");
        }
    }

    private void readTextAsset1(string defaultPath)
    {
        if (!File.Exists(defaultPath + ".txt"))
            return;
        string temp = File.ReadAllText(defaultPath + ".txt");
        string[] strArr = temp.Replace("\r", "").Split('\n');
        for (int i = 0; i < strArr.Length; i++)
        {
            string str = strArr[i];
            if (!(str.IndexOf(NOTE) == 0) && str != "")
            {
                //				if (defaultPath == "/Config/notice_v" || defaultPath == "/Config/notice" || defaultPath == "/Config/notice_2")
                //					Debug.LogError (str);
                parseConfig(str);
            }
        }
    }

    public void resolveConfigTextAsset(ResourcesData rd, string name)
    {
        TextAsset ta = rd.ResourcesBundle.Load(name) as TextAsset;
        if (ta == null)
            return;
        resolveConfig(ta.text);
    }

    /// <summary>
    /// 解析配置
    /// </summary>
    /// <param name="content">配置内容</param>
    public void resolveConfig(string content)
    {
        if (content == null)
            return;
        string text = content.Replace("\r\n", "\n");
        string[] lines = text.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < lines.Length; i++)
        {
            string str = lines[i];
            if (!(str.IndexOf(NOTE) == 0) && str != "")
            {
                parseConfig(str);
            }
        }
    }

    //获得配置文件完整路径
    private string getConfigHolePath(string path)
    {
        return PathKit.GetOSDataPath(path) + SUFFIX;
    }

    //获得配置目录文件夹路径
    private string getConfigFolder(string path)
    {
        return PathKit.GetOSDataPath(path);
    }

    private void readConfigFromResources(string path)
    {
        //如果存在版本号，就从基础配置名字里面获取对应的版本配置名
        if (GameManager.Instance.isHaveVersion())
        {
            readTextAsset(path);
        }
        string oldPath = path.Replace("_" + GameManager.CONFIG_VERSION, "");
        readTextAsset(oldPath);
        readTextAsset(oldPath + "_v");
    }

    /// <summary>
    /// 读取对应配置
    /// </summary>
    private void readTextAsset(string defaultPath)
    {
        //		if(!MiniConnectManager.IsRobot && Debug.isDebugBuild==true)
        //			Debug.LogWarning ("ReadTextAsset >>>>>>>>>" + defaultPath);
        TextAsset ta = (TextAsset)Resources.Load(changeResourcesPath(defaultPath), typeof(TextAsset));
        if (ta == null)
        {
            //			if(!MiniConnectManager.IsRobot && Debug.isDebugBuild==true)
            //				Debug.LogWarning ("ReadTextAsset,But >>>>>>>>>" + defaultPath + "<<<<<<<<< is Null");
            return;
        }
        string[] strArr = ta.text.Replace("\r", "").Split('\n');
        for (int i = 0; i < strArr.Length; i++)
        {
            string str = strArr[i];
            if (!(str.IndexOf(NOTE) == 0) && str != "")
            {
                //				if (defaultPath == "/Config/notice_v" || defaultPath == "/Config/notice" || defaultPath == "/Config/notice_2")
                //					Debug.LogError (str);
                parseConfig(str);
            }
        }
        createFile(defaultPath, ta.text);
    }

    //转化成Resources路径 去掉开头的'/'
    private string changeResourcesPath(string path)
    {
        return path.Substring(1);
    }

    //创建文件并写入数据
    private void createFile(string path, string str)
    {
        string holePath = getConfigHolePath(path);
        FileStream fs = new FileStream(holePath, FileMode.OpenOrCreate, FileAccess.Write);
        StreamWriter sw = new StreamWriter(fs);
        sw.Flush();
        sw.BaseStream.Seek(0, SeekOrigin.Begin);
        sw.Write(str);
        sw.Close();
    }

    //解析配置文件 需要子类覆盖
    public virtual void parseConfig(string str)
    {

    }

}

