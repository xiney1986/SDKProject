

using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;

/**
 * 服务器管理器
 * @author 汤琦
 * */
public class ServerManagerment
{
	/** 最近登录的服务器 */
	public Json_ServerInfo lastServer;
	private List<Json_ServerInfo> servers = new List<Json_ServerInfo> ();
	public string userName;//登录帐号名,或者ID
	public string passWord;//登录帐号名,或者ID
	private const string SUFFIX = ".txt";
	private static ServerManagerment servermanager;
	//暂时  登录状态
	public bool isLogin = false;

	//登录成功
	public void loginServer(){
		isLogin=true;
		lastServer.isRunning=true;
	}

	/// <summary>
	/// 添加服务器信息
	/// </summary>
	/// <param name="json">Json.</param>
	public void addServer(Json_ServerInfo json) {
		if(servers.Contains(json)) return;
		servers.Add(json);
	}

	public static ServerManagerment Instance {
		get {
			if (servermanager == null) {
				servermanager = new ServerManagerment ();
				return servermanager;
			}
			return servermanager;
		}
		//get{return SingleManager.Instance.getObj("ServerManagerment") as ServerManagerment;}
	}

	public void createServer ()
	{
		 
	}

	public string getServerIp ()
	{
		return this.lastServer.domainName;
	}

	public int getServerPort ()
	{
		return this.lastServer.port;
	}

	public void saveLastServer (Json_ServerInfo _server)
	{
		this.lastServer = _server;
    }
	/// <summary>
	/// 获取指定服务器名字的JsonServer
	/// </summary>
	/// <param name="server">Server.</param>
	public Json_ServerInfo getJsonServerInfoByName (Json_ServerInfo server)
	{
		if (server == null)
			return null;
		servers = getAllServer ();
		if (servers == null)
			return null;
		Json_ServerInfo jsi;
		for (int i = 0; i < servers.Count; i++) {
			jsi=servers [i];
			if (jsi.name.Equals (server.name)) {
				return jsi;
			}
		}
		return null;
	}
	public bool Contains (Json_ServerInfo server)
	{
		if (server == null)
			return false;
		servers = getAllServer ();
		bool istrue = false;

		if (servers == null)
			return false;

		for (int i = 0; i < servers.Count; i++) {
			if (servers [i].name.Equals (server.name)) {
				istrue = true;
				return istrue;
			}
		}
		return istrue;
	}
	/** 构建服务器信息 */
	public string getServerInfo (Json_ServerInfo jsonServerInfo) {
		return  userName + "#" + jsonServerInfo.sid + "#" + jsonServerInfo.domainName + "#" + jsonServerInfo.port + "#" + jsonServerInfo.name+ "#" + jsonServerInfo.payUrl + "#" + jsonServerInfo.isTestServer;
	}
	/** 设置最近3次登陆的服务器 */
	public void setLatelyLoginServer (string currentServer) {
		// 这里用Json_ServerInfo方便以后统一修改
		Json_ServerInfo currentJsonServerInfo = createJsonServerInfo (currentServer);
		string oldServerInfo = PlayerPrefs.GetString (PlayerPrefsComm.LOGIN_LATELY_SERVER);
		string[] temps = null;
		if (!string.IsNullOrEmpty (oldServerInfo)) {
			string[] oldServerInfoData = StringKit.stringToStringList (oldServerInfo, new char[]{StringKit.USD_SIGN});
			Json_ServerInfo tempJsonServerInfos;
			int jsonIndex = -1;
			for (int i=0; i<oldServerInfoData.Length; i++) {
				tempJsonServerInfos = createJsonServerInfo (oldServerInfoData [i]);
				if (tempJsonServerInfos.name == currentJsonServerInfo.name) { // 将最新的数据赋予老数据
					oldServerInfoData [i] = getServerInfo (currentJsonServerInfo);
					jsonIndex = i;
				}
			}
			if (jsonIndex==-1) { //  没有更新数据表示之前不存在此信息,添加之
				int saveNumber = 3;
				if (oldServerInfoData.Length >= saveNumber) { // >=3条顶掉第一条
					temps = new string[saveNumber];
					System.Array.Copy (oldServerInfoData, 1, temps, 0, saveNumber - 1);
					temps [temps.Length - 1] = currentServer;
				}
				else { // <3条添加
					temps = new string[oldServerInfoData.Length + 1];
					System.Array.Copy (oldServerInfoData, 0, temps, 0, oldServerInfoData.Length);
					temps [temps.Length - 1] = currentServer;
				}
			}
			else {
				temps = new string[oldServerInfoData.Length];
				if(jsonIndex>0)
					System.Array.Copy (oldServerInfoData, 0, temps, 0, jsonIndex);
				if(jsonIndex<oldServerInfoData.Length-1)
					System.Array.Copy (oldServerInfoData, jsonIndex+1, temps, jsonIndex, oldServerInfoData.Length-(jsonIndex+1));
				temps [temps.Length - 1] = currentServer;
			}
		}
		else {
			temps = new string[1];
			temps [0] = currentServer;
		}
		string newServerInfo = StringKit.stringListTostring (temps, StringKit.USD_SIGN);
		PlayerPrefs.SetString (PlayerPrefsComm.LOGIN_LATELY_SERVER, newServerInfo);
	}
	/** 移除指定名字的保存服务器信息 */
	public void removeLoginServer(Json_ServerInfo removeJson) {
		if(removeJson==null)
			return;
		string oldServer = PlayerPrefs.GetString (PlayerPrefsComm.LOGIN_LATELY_SERVER);
		if (string.IsNullOrEmpty(oldServer))
			return;
		string[] oldServerInfoData = StringKit.stringToStringList (oldServer, new char[]{StringKit.USD_SIGN});
		Json_ServerInfo tempJsonServerInfos;
		int jsonIndex = -1;
		for (int i=0; i<oldServerInfoData.Length; i++) {
			tempJsonServerInfos = createJsonServerInfo (oldServerInfoData [i]);
			if (tempJsonServerInfos.name == removeJson.name) { // 将最新的数据赋予老数据
				jsonIndex = i;
			}
		}
		string[] temps = new string[oldServerInfoData.Length-1];
		if(jsonIndex>0)
			System.Array.Copy (oldServerInfoData, 0, temps, 0, jsonIndex);
		if(jsonIndex<oldServerInfoData.Length-1)
			System.Array.Copy (oldServerInfoData, jsonIndex+1, temps, jsonIndex, oldServerInfoData.Length-(jsonIndex+1));
		string newServerInfo = StringKit.stringListTostring (temps, StringKit.USD_SIGN);
		PlayerPrefs.SetString (PlayerPrefsComm.LOGIN_LATELY_SERVER, newServerInfo);
	}
	/** 获取最后一次登陆的服务器 */
	public string getLastLoginServer () {
		string oldServer = PlayerPrefs.GetString (PlayerPrefsComm.LOGIN_LATELY_SERVER);
		if (string.IsNullOrEmpty(oldServer))
			return null;
		string[] servers = StringKit.stringToStringList (oldServer, new char[]{StringKit.USD_SIGN});
		return servers [servers.Length - 1];
	}
	/** 获取最近3次登陆的服务器 */
	public string[] getLatelyLoginServer () {
		string oldServer = PlayerPrefs.GetString (PlayerPrefsComm.LOGIN_LATELY_SERVER);
		if (oldServer == null)
			return null;
		string[] servers = StringKit.stringToStringList (oldServer, new char[]{StringKit.USD_SIGN});
		return servers;
	}
	/// <summary>
	/// 获得本地记录的服务器*千万不要在游戏内调用,请调lastServer*
	/// </summary>
	/// <returns>The local save server.</returns>
	public Json_ServerInfo getLocalSaveServer () {
		Json_ServerInfo server = null;
		string layelyServerInfo = getLastLoginServer ();
		//有记录,并且记录的服务器存在
		if (!string.IsNullOrEmpty (layelyServerInfo))
			setLatelyServer (layelyServerInfo);
		Json_ServerInfo jsi = getJsonServerInfoByName (lastServer);
		if (jsi == null) { //记录的服务器不存在
			removeLoginServer (lastServer);
			lastServer = null;
		}
		else { // 更新存在服务器的信息,防止ip,端口等数据修改后导致问题
			string serverInfo=getServerInfo(jsi);
			setLatelyServer (serverInfo);
			setLatelyLoginServer(serverInfo);
		}
		// 设置最后一次登录的服务器
		if (lastServer != null && Contains (lastServer)) {
			server = lastServer;
		}
		else if (getAllServer () != null && getAllServer ().Count > 0) {
			lastServer = getAllServer () [0];
            server = lastServer;
		}
		return server;
	}
	public void setLatelyServer (string str)
	{
		string[] serverInfoData = StringKit.stringToStringList (str, new char[]{StringKit.POUND_SIGN});
		lastServer = createJsonServerInfo(serverInfoData);
        if (ServerManagerment.Instance.userName == null 
			|| ServerManagerment.Instance.userName == "") {
			ServerManagerment.Instance.userName = serverInfoData [0];
		}
	}
	/** 创建Json服务器信息 */
	public Json_ServerInfo createJsonServerInfo(string serverInfo) {
		if(serverInfo==null) return null;
		string[] serverInfoData = StringKit.stringToStringList (serverInfo, new char[]{StringKit.POUND_SIGN});
		return createJsonServerInfo(serverInfoData);
	}
	/** 创建Json服务器信息 */
	public Json_ServerInfo createJsonServerInfo(string[] serverInfoData) {
		if(serverInfoData==null||serverInfoData.Length<5)
			return null;
		Json_ServerInfo jsonServerInfo = new Json_ServerInfo (serverInfoData [1], serverInfoData [2], StringKit.toInt (serverInfoData [3]), serverInfoData [4]);
        jsonServerInfo.payUrl = serverInfoData[5];
        if (serverInfoData.Length>=7)
			if(serverInfoData[6]=="True")
				jsonServerInfo.isTestServer=true;
		return jsonServerInfo;
	}
	
	public List<Json_ServerInfo> getAllServer ()
	{
		return servers;
	}

    public void InitTestServers()
    {
        if (GameManager.Instance.isShowTestServer)
        {
            ServerManagerment manager = ServerManagerment.Instance;

            Json_ServerInfo xzns_int = new Json_ServerInfo("1", "192.168.2.18", 7610, "xzns_int");
            xzns_int.isTestServer = true;
            manager.addServer(xzns_int);
            Json_ServerInfo xzns_patch = new Json_ServerInfo("2", "server.natappfree.cc", 35814, "xzns_patch");
            xzns_patch.isTestServer = true;
            manager.addServer(xzns_patch);
        }
    }

	public void setAllServer (List<Json_ServerInfo> serverList)
	{
		servers = serverList;
	}
	
	

 
 
	
}
