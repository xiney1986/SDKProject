using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Net;
using System.Net.Sockets;

public class loginmsgClass
{

    public int status;
    public string msg;
}

public class json_SDKMessage
{
    public string head;
    public string body;
}

public class json_Goods
{
    public string amount;
    public string id;
    public string rate;
    public string desc1;
    public string desc2;
    public string name;
    public string payName;
    public int type;
    public json_Goods() { }
    public json_Goods(Dictionary<string, object> data)
    {

        amount = data["amount"].ToString();
        id = data["id"].ToString();
        rate = data["rate"].ToString();
        desc1 = data["desc1"].ToString();
        desc2 = data["desc2"].ToString();
        name = data["name"].ToString();
        if (data.ContainsKey("type"))
            type = StringKit.toInt(data["type"].ToString());
    }

}

public class Json_AppInfo
{
    public string appId;
    public string appName;
    public string appVersion;
    public string deviceId;
    public string qudao;

    public Json_AppInfo(string msg)
    {
        Dictionary<string, object> jsonData = MiniJSON.Json.Deserialize(msg) as Dictionary<string, object>;
        appName = jsonData["appName"].ToString();
        appId = jsonData["appId"].ToString();
        appVersion = jsonData["version"].ToString();
        deviceId = jsonData["appId"].ToString();
        qudao = jsonData["appId"].ToString();
    }

}

public class Json_ServerInfo
{
    public string is_hot;
    public string is_new;
    public string sid;
    private string namec;
    public string is_open;
    public string ext; // chat server url
    public bool isRunning = true; //服务器是否开启
                                  //前台用
    public string domainName;
    public IPEndPoint ipEndPoint;
    public int port;
    public bool isTestServer = false;
    public string payUrl;

    public Json_ServerInfo(Dictionary<string, object> data)
    {
        is_hot = data["is_hot"].ToString();
        is_new = data["is_new"].ToString();
        is_open = data["is_open"].ToString();
        sid = data["sid"].ToString();
        namec = data["name"].ToString();
        domainName = data["domainName"].ToString();
        port = int.Parse(data["port"].ToString());
        payUrl = data["payUrl"].ToString();
    }

    public string Namec
    {
        get
        {
            return namec;
        }
    }

    public string name
    {
        get
        {
            if (is_open == "0")
            {
                return namec;
            }
            else if (is_open == "1")
            {
                return (namec + LanguageConfigManager.Instance.getLanguage("server_repert"));
            }
            else
            {
                return namec;
            }
        }
    }


    public Json_ServerInfo(string id, string ipAddress, int ipPort, string serverName)
    {
        sid = id;
        domainName = ipAddress;
        port = ipPort;
        namec = serverName;


    }
}
public class samplePlayerInfo
{
    public string nickname;
    public string grade;
    public string rank;
    public string vip_grade;
    public string face;
}

