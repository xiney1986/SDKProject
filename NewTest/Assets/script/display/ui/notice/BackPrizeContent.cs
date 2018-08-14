using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackPrizeContent : dynamicContent
{
	List<BackPrize> prizeList = new List<BackPrize>();
	public GameObject backPrizeItemPrefab;
	WindowBase fatherWin;
	NoticeTopButton button;

	public void reLoad(WindowBase win,NoticeTopButton button)
	{
		this.button = button;
		fatherWin = win;
		prizeList = BackPrizeLoginInfo.Instance.prizeList;
		base.reLoad (prizeList.Count);
	}


	public override void initButton (int  i)
	{
		if (nodeList [i] == null){
			nodeList [i] = NGUITools.AddChild (gameObject, backPrizeItemPrefab);
		}
	}

	public override void updateItem (GameObject item, int index)
	{
		BackPrizeItem bpi = item.GetComponent<BackPrizeItem>();
		bpi.updateItem(prizeList[index],fatherWin,this.button);
	}

	void Update()
	{
		if(BackPrizeLoginInfo.Instance.loginTime == 0)
		{
			BackPrizeLoginInfo.Instance.loginTime = ServerTimeKit.getLoginTime();
		}
		if(ServerTimeKit.getMillisTime() >= BackPrizeLoginInfo.Instance.getSecondDayTime(BackPrizeLoginInfo.Instance.loginTime))
		{
			BackPrizeLoginInfo.Instance.loginTime = ServerTimeKit.getMillisTime();
			BackPrizeInfoFPort bpif = FPortManager.Instance.getFPort ("BackPrizeInfoFPort") as BackPrizeInfoFPort;
			bpif.BackPrizeLoginInfoAccess(updateState);
		}
	}

	public void updateState()
	{
		// 刷新可领状态//
		GameObject itemObj;
		BackPrizeItem bpi;
		for(int i=0;i<BackPrizeLoginInfo.Instance.prizeList.Count;i++)
		{
			if(BackPrizeLoginInfo.Instance.prizeList[i].isRecevied == BackPrizeRecevieType.RECEVIE)// 可领取//
			{
				itemObj = gameObject.transform.FindChild("00" + (i+1)).gameObject;
				if(itemObj != null)
				{
					bpi = itemObj.GetComponent<BackPrizeItem>();
					bpi.updateItem(prizeList[i],fatherWin,this.button);
				}
			}
		}
	}

}
