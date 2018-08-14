using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GodsWarFinalPointItem:MonoBase{

	/// <summary>
	/// 战报点位信息
	/// </summary>
	GodsWarFinalPoint point;
	/// <summary>
	/// 父窗口
	/// </summary>
	public WindowBase win;
	/// <summary>
	/// 左方线条
	/// </summary>
	public UISprite[] leftLine;
	/// <summary>
	/// 右方线条
	/// </summary>
	public UISprite[] rightLine;
	/// <summary>
	/// 查看战报
	/// </summary>
	public ButtonBase replayWarButton;
	/// <summary>
	/// 支持按钮
	/// </summary>
	public ButtonBase suportButton;
	CallBack callback;

	private int big_id;
	private int yu_ming;

    public void init(GodsWarFinalPoint point,int big_id,int yu_ming, WindowBase win,CallBack callback)
    {
		this.point = point;
		this.win = win;
		this.big_id = big_id;
		this.yu_ming = yu_ming;
		this.callback = callback;
		initButtons();
		initDetailInfo();
    }
	/// <summary>
	/// 初始化按钮信息
	/// </summary>
    public void initButtons()
    {
		replayWarButton.gameObject.SetActive(false);
		suportButton.gameObject.SetActive(false);
		replayWarButton.fatherWindow = win;
		suportButton.fatherWindow = win;
		replayWarButton.onClickEvent =doReplayEvent;
		suportButton.onClickEvent = doSuportEvent;
    }
	/// <summary>
	/// 初始化详细信息
	/// </summary>
	public void initDetailInfo()
    {
		//设置线条颜色
		if(point.isHaveWinner!=-1)
		{
			setPointUserIndex();
			if(point.winner.index < point.loser.index)
			for (int i = 0; i < leftLine.Length; i++) {
				leftLine[i].spriteName = "back_1";
				rightLine[i].spriteName = "zisexiantiao";
			}
			else
			for (int i = 0; i < leftLine.Length; i++) {
				rightLine[i].spriteName = "back_1";
				leftLine[i].spriteName = "zisexiantiao";
			}
		}
		else
		{
			for (int i = 0; i < leftLine.Length; i++) {
				leftLine[i].spriteName = "zisexiantiao";
				rightLine[i].spriteName = "zisexiantiao";
			}
		}

		//设置是战报还是支持按钮
		if (point.replayIDs.Length <= 0) {
			if(point.isSuport==-1){
				suportButton.gameObject.SetActive(true);
				replayWarButton.gameObject.SetActive(false);
			}
		}
		else
		{
			replayWarButton.gameObject.SetActive(true);
			suportButton.gameObject.SetActive(false);
		}
		//因为一轮只能支持一个玩家，所以要排除支持的点所在组的其他点位
		List<GodsWarMySuportInfo> mySuport = GodsWarManagerment.Instance.mySuportInfo;
		if(mySuport==null)return;
		int state = GodsWarManagerment.Instance.getTypeByLocalId(point.localID);
		for (int i = 0; i < mySuport.Count; i++) {
			if(GodsWarManagerment.Instance.getTypeByLocalId(mySuport[i].localId)==state)
				suportButton.gameObject.SetActive(false);
		}
	}

	public void setPointUserIndex()
	{
		List<GodsWarFinalUserInfo> list = GodsWarManagerment.Instance.finalInfoList;
		if(list!=null)
		{
			for (int i = 0; i < list.Count; i++) {
				if(list[i].serverName==point.winner.serverName&&list[i].uid==point.winner.uid)
					point.winner.index = list[i].index;
				if(list[i].serverName==point.loser.serverName&&list[i].uid==point.loser.uid)
					point.loser.index = list[i].index;
			}
		}
	}
	/// <summary>
	/// 执行查看战报事件
	/// </summary>
	public void doReplayEvent(GameObject obj)
	{
		UiManager.Instance.openDialogWindow<GodsWarReplayWindow>((win)=>{
			win.initWindow(point,updateUI);
		});
	}
	/// <summary>
	/// 执行点赞事件
	/// </summary>
	public void doSuportEvent(GameObject obj)
	{
		UiManager.Instance.openDialogWindow<GodsWarSuportWindow>((win)=>{
			win.initWindow(big_id,yu_ming,point.localID,updateUI);
		});
	}
	/// <summary>
	/// 更新UI
	/// </summary>
	public void updateUI()
	{
		suportButton.gameObject.SetActive(false);
		replayWarButton.gameObject.SetActive(false);
		if(callback!=null)
			callback();
	}
}
