using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GodsWarReplayWindow : WindowBase 
{
	/// <summary>
	/// 条目点位
	/// </summary>
	public GameObject[] points;
	/// <summary>
	/// 对战玩家名称
	/// </summary>
	public UILabel[] lblPlayersName;
	public UILabel lblVsValue;
	/// <summary>
	/// 标题
	/// </summary>
	public UILabel lbltitle;
	public GameObject itemPrefab;
	public GameObject mask;
	
	List<GodsWarFinalUserInfo> players;
	GodsWarFinalPoint point;

	CallBack callback;

	protected override void begin ()
	{
		base.begin ();
	    UiManager.Instance.godsWarReplayWindow = this;
		mask.gameObject.SetActive(true);
		MaskWindow.UnlockUI();
		initUI();
	}

	public void initWindow(GodsWarFinalPoint point, CallBack callback)
	{
		this.point = point;
		this.callback = callback;
	}
	/// <summary>
	/// 初始化数据
	/// </summary>
	public void initTitle()
	{
		setPlayerName();
		for (int i = 0; i < lblPlayersName.Length; i++) {
			lblPlayersName[i].text = point.players[i].name+"."+point.players[i].serverName;
		}
		lblVsValue.text = getVsValueByNames(point.players[0].serverName,point.players[0].uid)+":"+getVsValueByNames(point.players[1].serverName,point.players[1].uid);
	}

	public void setPlayerName()
	{
		//决赛单独处理
		if(point.localID == 16)
		{
			List<GodsWarFinalUserInfo> shenmoUser = GodsWarManagerment.Instance.shenMoUserlist;
			for (int i = 0; i < shenmoUser.Count; i++) {
				for (int j = 0; j < point.players.Length; j++) {
					if(shenmoUser[i].serverName==point.players[j].serverName && shenmoUser[i].uid==point.players[j].uid)
						point.players[j].name = shenmoUser[i].name;
				}
				for (int j = 0; j < point.winnerSingle.Length; j++) {
					if(shenmoUser[i].serverName==point.winnerSingle[j].serverName && shenmoUser[i].uid==point.winnerSingle[j].uid)
						point.winnerSingle[j].name = shenmoUser[i].name;
				}
			}
			return;
		}
		List<GodsWarFinalUserInfo> list = GodsWarManagerment.Instance.finalInfoList;
		if(list!=null)
		{
			for (int i = 0; i < list.Count; i++) {
				for (int j = 0; j < point.players.Length; j++) {
				if(list[i].serverName==point.players[j].serverName&&list[i].uid==point.players[j].uid)
					point.players[j].name = list[i].name;
				}
				for (int j = 0; j < point.winnerSingle.Length; j++) {
					if(list[i].serverName==point.winnerSingle[j].serverName && list[i].uid==point.winnerSingle[j].uid)
						point.winnerSingle[j].name = list[i].name;
				}
			}
		}
	}

	/// <summary>
	/// 初始化UI
	/// </summary>
	public void initUI()
	{
		initTitle();
		for (int i = 0; i < points.Length; i++) {
			if(point.replayIDs.Length-1<i)continue;
			GameObject go = NGUITools.AddChild(points[i],itemPrefab);
			GodsWarRepalyItem item = go.GetComponent<GodsWarRepalyItem>();
			item.initItem(point.winnerSingle[i],i,point.replayIDs[i],this,updateUI);
		}
	}

	/// <summary>
	/// 获取比分
	/// </summary>
	public int getVsValueByNames(string serverName,string uid)
	{
		int value = 0;
		for (int i = 0; i < point.winnerSingle.Length; i++) {
			if(point.winnerSingle[i].serverName==serverName && point.winnerSingle[i].uid==uid)
				value++;
		} 
		return value;
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if(gameObj.name=="button_close")
		{
			mask.gameObject.SetActive(false);
			finishWindow();
		}
	}

	public void updateUI()
	{
		finishWindow();
	}
    void OnDestroy() {
        UiManager.Instance.godsWarReplayWindow = null;
    }
}
