using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 诸神战淘汰赛玩家信息条目
/// </summary>
public class GodsWarFinalUserInfoItem : MonoBase
{

	public GodsWarFinalUserInfo user;

	public UILabel lblName;
	public UILabel lblSeverName;
	public GameObject suport;
	public ButtonBase viewButton;

	private int big_id;
	private int yu_ming;

	/// <summary>
	/// 初始化UI
	/// </summary>
	public void initUI(int big_id,int yu_ming,GodsWarFinalUserInfo info,WindowBase win)
	{
		this.user = info;
		this.big_id =big_id;
		this.yu_ming = yu_ming;

		lblName.text = info.name;
		lblSeverName.text = info.serverName;
		viewButton.fatherWindow = win;
		viewButton.onClickEvent = doViewEvent;
		setSuportInfo();
	}

	public void doViewEvent(GameObject obj)
	{
		UiManager.Instance.openDialogWindow<GodsWarUserInfoWindow>((win)=>{
			win.initWindow(user.serverName,user.name,user.uid,big_id,yu_ming,updateUI);
		});
	}
	public void updateUI()
	{

	}
	/// <summary>
	/// 设置是否支持
	/// </summary>
	public void setSuportInfo()
	{
		List<GodsWarMySuportInfo> mySuport = GodsWarManagerment.Instance.mySuportInfo;
		if (mySuport==null) {
			return;
		}
		for (int i = 0; i < mySuport.Count; i++) {
		    if (user.serverName == mySuport[i].serverName && user.uid == mySuport[i].uid && mySuport[i].isWin == -1)
		    {
		        suport.gameObject.SetActive(true);
		    }
		    else
		    {
		        suport.gameObject.SetActive(false);
		    }
		}
	}
}
