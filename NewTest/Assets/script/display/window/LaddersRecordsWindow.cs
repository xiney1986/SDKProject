using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 天梯战斗记录窗口
/// </summary>
public class LaddersRecordsWindow : WindowBase
{
	public UITextList textList_records;
	public GameObject filpArrow;//下翻页箭头指示
	public  UIScrollBar bar;
	public LaddersRecordsContent content;
	/// <summary>
	/// begin中更新记录信息
	/// </summary>
	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI();

		List<LaddersRecordInfo> records=LaddersManagement.Instance.Records.M_getRecords();
		content.reLoad(records.ToArray());
		if(records.Count >=4)
		{
			filpArrow.SetActive(true);
		}
		else
		{
			filpArrow.SetActive(false);		
		}
		bar.value=0;


	}
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		string btnName=gameObj.name;
		switch(btnName)
		{
			case "btn_close":
				finishWindow();
				break;
		}
	}
	/// <summary>
	/// 当点击战斗记录中的超链
	/// </summary>
	/// <param name="url">URL.</param>
	public void M_onClickUrl(string url)
	{
		finishWindow();
		MaskWindow.instance.setServerReportWait(true);
		GameManager.Instance.battleReportCallback=GameManager.Instance.intoBattleNoSwitchWindow;

			LaddersBattleReplayFPort fport=new LaddersBattleReplayFPort();
					fport.apply(StringKit.toInt(url),null); 



	}
}
