using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

/**
 * 选服窗口
 * @author 汤琦
 * */
public class SelectServerWindow : WindowBase {

	/* gameobj fields */
	/** 服务器按钮预制体 */
	public GameObject serverButtonPrefab;
	public UIScrollView ScrollContent;
	public GameObject downArrow;
	/** 选择效果背景 */
	public GameObject selectBg;
	/** 最近登录的服务器按钮 */
	public ServerButton[] latelyServers;

	/* methods */
	protected override void DoEnable () {
		base.DoEnable ();

		UiManager.Instance.backGround.switchBackGround ("loginBackGround");
		initServerButtons (ServerManagerment.Instance.lastServer);
		List<Json_ServerInfo> serverList = ServerManagerment.Instance.getAllServer ();
		foreach (Json_ServerInfo each in serverList) {
			GameObject obj = NGUITools.AddChild (ScrollContent.gameObject, serverButtonPrefab);
			ServerButton button = obj.GetComponent<ServerButton> ();
			button.initButton (each);
			button.fatherWindow = this;
			obj.AddComponent<UIDragScrollView> ();
		}
		UIGrid grid = ScrollContent.GetComponent<UIGrid> ();
		grid.enabled = true;
		grid.Reposition ();
		if (GuideManager.Instance.guideUI != null) {
			GuideManager.Instance.hideGuideUI ();
		}
		if (serverList.Count > 8)
			downArrow.SetActive (true);
		MaskWindow.UnlockUI ();
	}

	void OnGetServerListLogin (List<Json_ServerInfo> serverList)
	{
		if (serverList != null)
			ServerManagerment.Instance.setAllServer (serverList);
		updateUI ();
	}

	public override void DoDisable ()
	{
		base.DoDisable();
	}

	public void updateUI()
	{
		initServerButtons (ServerManagerment.Instance.lastServer);
	

		for (int i = 0; i<ScrollContent.transform.childCount; i++) {
			GameObject go = ScrollContent.transform.GetChild (i).gameObject;
			DestroyImmediate (go);
		}

		List<Json_ServerInfo> serverList = ServerManagerment.Instance.getAllServer ();
		foreach (Json_ServerInfo each in serverList) {
			GameObject obj = NGUITools.AddChild (ScrollContent.gameObject, serverButtonPrefab);
			ServerButton button = obj.GetComponent<ServerButton> ();
			button.initButton (each);
			button.fatherWindow = this;
			obj.AddComponent<UIDragScrollView> ();
		}
		UIGrid grid = ScrollContent.GetComponent<UIGrid> ();
		grid.enabled = true;
		grid.Reposition ();
		if (GuideManager.Instance.guideUI != null) {
			GuideManager.Instance.hideGuideUI ();
		}
		if (serverList.Count > 8)
			downArrow.SetActive (true);
	}


	/***/
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow();
		}
	}
	/** 重置服务器按钮状态 */
	private void resetLatelyServersActive () {
		for (int i=0; i<latelyServers.Length; i++) {
			latelyServers [i].gameObject.SetActive (false);
		}
		selectBg.SetActive(false);
	}
	/** 初始化服务器按钮 */
	private void initServerButtons (Json_ServerInfo selectServer) {
		if(selectServer==null)
			return;
		string[] serverInfos = ServerManagerment.Instance.getLatelyLoginServer ();
		resetLatelyServersActive ();
		ServerButton latelyServer;
		if (serverInfos == null) {
			latelyServer = latelyServers [0];
			latelyServer.server = selectServer;
			latelyServer.platform.text = selectServer.name;
			latelyServer.gameObject.SetActive (true);
		}
		else {
			Json_ServerInfo server;
			string[] servers;
			int len = serverInfos.Length > latelyServers.Length ? latelyServers.Length : serverInfos.Length;
			for (int i=0,j=len-1; i<len; i++,j--) {
				string serverInfo = serverInfos [i];
				server = ServerManagerment.Instance.createJsonServerInfo (serverInfo);
				if(server==null) continue;
				latelyServer = latelyServers [j];
				if (selectServer.name == server.name) {
					latelyServer.server = selectServer;
					latelyServer.platform.text = selectServer.name;
					UpdateSelectBgPosition(latelyServer);
				}
				else {
					latelyServer.server = server;
					latelyServer.platform.text = server.name;
				}
				latelyServer.gameObject.SetActive (true);
			}
		}
	}
	/** 更新选择背景位置 */
	public void UpdateSelectBgPosition(ServerButton latelyServer) {
		selectBg.SetActive(true);
		selectBg.transform.localPosition=new Vector3(latelyServer.transform.localPosition.x,latelyServer.transform.localPosition.y,latelyServer.transform.localPosition.z);
	}
	//获得配置文件完整路径
	private string getConfigHolePath (string path) {
		return PathKit.GetOSDataPath (path) + PathKit.SUFFIX;
	}
	//创建文件
	private void createFile (string path) { 
		string holePath = getConfigHolePath (path); 
		FileStream fs = new FileStream (holePath, FileMode.OpenOrCreate, FileAccess.Write);
		StreamWriter sw = new StreamWriter (fs);
		sw.Flush ();
		sw.BaseStream.Seek (0, SeekOrigin.Begin);
		sw.Close (); 
	}
	//转化成Resources路径 去掉开头的'/'
	private string changeResourcesPath (string path) { 
		return  path.Substring (1); 
	} 
}