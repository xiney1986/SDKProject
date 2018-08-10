using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

public class Main : MonoBehaviour
{

    void Awake()
    {
        Button loginButton = transform.Find("LoginButton").GetComponent<Button>();
        loginButton.onClick.AddListener(ClickLogin);
        Button logoutButton = transform.Find("LogoutButton").GetComponent<Button>();
        logoutButton.onClick.AddListener(ClickLogout);
        Button loadButton = transform.Find("LoadButton").GetComponent<Button>();
        loadButton.onClick.AddListener(ClickLoad);
    }

    private void ClickLogin()
    {
        SdkManager.INSTANCE.Login();
        //Debug.Log(Application.persistentDataPath);
        //Debug.Log(Application.streamingAssetsPath);
        //Directory.CreateDirectory(Application.persistentDataPath);
        //StartCoroutine(LoadUrl(GetPath(Application.streamingAssetsPath + "/RawImage.u3d"), (result) =>
        //{
        //    File.WriteAllBytes(Application.persistentDataPath + "/RawImage.u3d", result.www.bytes);
        //}));
    }

    private void ClickLogout()
    {
        //SdkManager.INSTANCE.Logout();
    }

    private int index = 0;
    private void ClickLoad()
    {
        StartCoroutine(LoadUrl("http://192.168.2.141:8080/update.json", (result) =>
        {
            Debug.Log(result.state+","+ result.www.error+","+ result.www.url);
            if (result.state == LoadStatus.OK)
            {
                Dictionary<string, object> dic =
                    MiniJSON.Json.Deserialize(result.www.text) as Dictionary<string, object>;
                List<AssetItem> list = new List<AssetItem>(dic.Count);
                foreach (var VARIABLE in dic)
                {
                    string path = VARIABLE.Key;
                    ulong crc = ulong.Parse((string) VARIABLE.Value);
                    string newPath = Application.persistentDataPath + "/" + path;
                    if (!File.Exists(newPath))
                    {
                        Debug.Log(newPath + ",no Exist");
                        list.Add(new AssetItem(path, crc));
                    }
                    else
                    {
                        ulong tempCrc = Crc32Utils.GetCRC32Str(File.ReadAllBytes(newPath));
                        if (tempCrc != crc)
                        {
                            Debug.Log(newPath + ",crc is not same");
                            list.Add(new AssetItem(path, crc));
                        }
                    }
                }
                if (list.Count > 0)
                {
                    StartCoroutine(LoadUrl("http://192.168.2.141:8080/" + list[index].p, (loadResult) =>
                    {
                        if (loadResult.state == LoadStatus.OK)
                        {
                            ulong tempCrc = Crc32Utils.GetCRC32Str(loadResult.www.bytes);
                            if (tempCrc == list[index].c)
                            {
                                string newPath = Application.persistentDataPath + "/" + list[index].p;
                                Debug.Log(newPath);
                                File.WriteAllBytes(newPath, loadResult.www.bytes);
                            }
                            else
                            {
                                //TODO 要处理失败
                            }
                        }
                        //TODO 要处理失败
                    }, 0));
                }
            }
            else
            {
                //TODO 要处理失败
            }
        }));

        //Dictionary<string,string> dic=new Dictionary<string, string>();

        //dic.Add("path1", "1");
        //dic.Add("path2", "2");

        //Debug.Log(MiniJSON.Json.Serialize(dic));

        //byte[] bytes = File.ReadAllBytes(Application.dataPath + "/StreamingAssets/RawImage.u3d");
        //Debug.Log(Crc32Utils.GetCRC32Str(bytes));
    }

    private IEnumerator LoadUrl(string url, Action<LoadResult> action, int timeOut = 10)
    {
        WWW www = new WWW(url);
        float time = Time.realtimeSinceStartup;
        while (true)
        {
            if (www.isDone)
            {
                string error = www.error;
                if (string.IsNullOrEmpty(error))
                {
                    LoadResult result = new LoadResult(LoadStatus.OK, www);
                    action(result);
                }
                else
                {
                    LoadResult result = new LoadResult(LoadStatus.ERROR, www);
                    action(result);
                }
            }
            else if (Time.realtimeSinceStartup - time >= timeOut && timeOut != 0)
            {
                LoadResult result = new LoadResult(LoadStatus.TIMEOUT, www);
                action(result);
                yield break;
            }
            yield return new WaitForSeconds(1);
        }
    }

    public static string GetPath(string path)
    {
#if UNITY_IPHONE||UNITY_EDITOR
        return "file://" + path;
#endif
        return path;
    }

    struct LoadResult
    {
        public int state;
        public WWW www;

        public LoadResult(int state, WWW www)
        {
            this.state = state;
            this.www = www;
        }
    }

    class LoadStatus
    {
        public static int OK = 0;
        public static int ERROR = 1;
        public static int TIMEOUT = 2;
    }

    struct AssetItem
    {
        public string p;
        public ulong c;

        public AssetItem(string p, ulong c)
        {
            this.p = p;
            this.c = c;
        }

        //public Dictionary<string, string> toDic()
        //{
        //    Dictionary<string, string> dic = new Dictionary<string, string>(2);
        //    dic.Add("p", p);
        //    dic.Add("c", c.ToString());
        //    return dic;
        //}
    }
}
