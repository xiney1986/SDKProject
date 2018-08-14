using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SevenDaysHappyDetailBtn : ButtonBase 
{
	public UILabel noselect_text;// 没选中状态按钮文字//
	public UILabel select_text;// 选中状态按钮文字//
	public GameObject tip;// 小角标提示//
	public UILabel labelTip;// 小角标提示个数//
	public UISprite btnBg;// 按钮背景//
	[HideInInspector]
	public SevenDaysHappyDetail detail;
	[HideInInspector]
	public SevenDaysHappyContent content;
	const string btnSpriteName_select = "tap_normal";
	const string btnSpriteName_NOselect = "tap_clickOn";
	[HideInInspector]
	public int canReceivedCount;// 改内容下可领取个数//
	[HideInInspector]
	public DayTopButton topButton;

	public override void DoClickEvent ()
	{
		//base.DoClickEvent ();

		if(content.selectedDetailBtn != this)
		{
			content.missonContent.destroyMissons();
			if(detail.type == SevenDaysHappyDetailType.banjiaqianggou)// 当点击的是半价购买时//
			{
				content.initBanjiaPanel(detail.missonList[0]);
				content.banjiaPanel.SetActive(true);
			}
			else
			{
				if(content.selectedDetailBtn.detail.type == SevenDaysHappyDetailType.banjiaqianggou)// 如果上一次是半价购买//
				{
					content.banjiaPanel.SetActive(false);
				}
				showMisson();
			}
			setSelectState(true);
			content.selectedDetailBtn.setSelectState(false);
			content.selectedDetailBtn = this;
			topButton.detail = detail;
		}
	}

	public void initDetailBtn(SevenDaysHappyDetail detail,SevenDaysHappyContent content,DayTopButton topBtn)
	{
		this.topButton = topBtn;
		this.detail = detail;
		this.content = content;

		noselect_text.text = detail.detailName;
		select_text.text = detail.detailName;

		if(content.selectedDetailBtn == this)
		{
			setSelectState(true);
		}
		else
		{
			setSelectState(false);
		}

		canReceivedCount = 0;
		for(int i=0;i<detail.missonList.Count;i++)
		{
			if(detail.missonList[i].missonState == SevenDaysHappyMissonState.Completed)
			{
				canReceivedCount++;
			}
		}
	}

	public void setSelectState(bool b)
	{
		if(b)// 被选中//
		{
			noselect_text.gameObject.SetActive(false);
			select_text.gameObject.SetActive(true);
			btnBg.spriteName = btnSpriteName_select;
		}
		else
		{
			noselect_text.gameObject.SetActive(true);
			select_text.gameObject.SetActive(false);
			btnBg.spriteName = btnSpriteName_NOselect;
		}
	}

	public void showMisson()
	{
		content.missonContent.reLoad(detail,this.fatherWindow,content,this);
	}

	void Update()
	{
		if(canReceivedCount > 0)
		{
			tip.SetActive(true);
			labelTip.text = canReceivedCount.ToString();
		}
		else
		{
			tip.SetActive(false);
		}
	}
}
