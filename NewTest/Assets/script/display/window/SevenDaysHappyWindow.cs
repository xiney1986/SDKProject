using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SevenDaysHappyWindow : WindowBase
{

	public UISprite left, right; //箭头
	public Transform showPos;//具体活动显示位置
	public GameObject content;//活动按钮显示容器
	public GameObject topButton;//活动按钮预设
	public GameObject topButtons;//滚动顶级
	[HideInInspector]
	public GameObject show;
	private DayTopButton lastSelect;//最后选择的按钮
	int firstBootIndex = -1;
	[HideInInspector]
	public int defaultSelectSid = 1;
	private Hashtable tabButtons;
	private bool flag;
	private int indexxx=6;
	public Texture dayTex;
	public Texture dayTex_1;// 第1天icon//
	public Texture dayTex_2;// 第2天icon//
	public Texture dayTex_3;// 第3天icon//
	public Texture dayTex_4;// 第4天icon//
	public Texture dayTex_5;// 第5天icon//
	public Texture dayTex_6;// 第6天icon//
	public Texture dayTex_7;// 第7天icon//
	public SevenDaysHappyContent sevenDaysHappyContent;// 活动类容详情//
	public ButtonBase helpBtn;

	public override void OnAwake ()
	{
		base.OnAwake ();
		helpBtn.onClickEvent = clickHelpBtn;
	}

	void clickHelpBtn(GameObject obj)
	{
		Card[] cards = SevenDaysHappyManagement.Instance.getCards();
		if(cards != null && cards[0] != null && cards[1] != null && cards[2] != null)
		{
			UiManager.Instance.openDialogWindow<SevenDaysHappyHelpWindow>((win)=>{
				win.initWin(cards);
			});
		}
		else
		{
			MaskWindow.UnlockUI();
		}
	}
	protected override void begin ()
	{
		base.begin ();
		if(SevenDaysHappyManagement.Instance.getIsPrizesSelectWin())
		{
			SevenDaysHappyManagement.Instance.setIsPrizesSelectWin(false);
			sevenDaysHappyContent.missonContent.destroyMissons();
			sevenDaysHappyContent.selectedDetailBtn.showMisson();
		}
		MaskWindow.UnlockUI ();
	}
	//断线重连
	public override void OnNetResume ()
	{
		base.OnNetResume (); 
		if (lastSelect == null) {
			return;
		}
		showDetail (lastSelect);
	}

    protected override void DoEnable()
    {
        base.DoEnable();
    }

    protected override void DoUpdate()
    {
        base.DoUpdate();
        if (Time.frameCount % 50 == 0)
            updateTime();
		if(flag){
			if(topButtons.transform.localPosition.x<-120)left.gameObject.SetActive(true);
			else left.gameObject.SetActive(false);
			if(topButtons.transform.localPosition.x<-(indexxx-6)*110)right.gameObject.SetActive(false);
			else right.gameObject.SetActive(true);
		}

		// 在任务时间期间,跨天处理//
		if(SevenDaysHappyManagement.Instance.getActiveMissonEndTime() - ServerTimeKit.getSecondTime() > 0)
		{
			if(SevenDaysHappyManagement.Instance.loginTime == 0)
			{
				SevenDaysHappyManagement.Instance.loginTime = ServerTimeKit.getLoginTime();
			}
			if(ServerTimeKit.getMillisTime() >= SevenDaysHappyManagement.Instance.getSecondDayTime(SevenDaysHappyManagement.Instance.loginTime))// 跨天//
			{
				SevenDaysHappyManagement.Instance.loginTime = ServerTimeKit.getMillisTime();
				StartCoroutine(updateUIForSecondDay());
			}
		}

    }
	// 跨天刷新//
	IEnumerator updateUIForSecondDay ()
	{
		yield return new WaitForSeconds(1);
		
		// 跨天刷新//
		int dayIndex = SevenDaysHappyManagement.Instance.getDayIndex();
		DayTopButton selectBtn = tabButtons [dayIndex] as DayTopButton;
		if(selectBtn != null)
		{
			sevenDaysHappyContent.destroyDetailButtons();
			showDetail(selectBtn);
		}
	}

	private void updateTime ()
	{
		if (content == null || content.transform.childCount == 0)
			return;
		Transform[] childs = content.GetComponentsInChildren<Transform> ();
		DayTopButton dayTopButton;
		foreach (Transform item in childs) {
			dayTopButton = item.gameObject.GetComponent<DayTopButton> ();
			if (dayTopButton != null) {
				dayTopButton.updateTime ();
			}
		}
	}

	public void updateSelectButton (int _type)
	{
		defaultSelectSid = _type;
	}

	public void updateDefaulSelect(int type)
	{
		defaultSelectSid = type;
		DayTopButton selectBtn = tabButtons [defaultSelectSid] as DayTopButton;
		showDetail (selectBtn);
	}
	public void initTopButton ()
	{
		UIScrollView scrollView = topButtons.transform.GetComponent<UIScrollView> ();
		tabButtons = new Hashtable ();
		UIGrid grid = content.GetComponent<UIGrid> ();
		indexxx=0;
		DayTopButton itemButton;
		SevenDaysHappySample sample;
		GameObject obj;
		int dayID;
		foreach (KeyValuePair<int,SevenDaysHappySample> item in SevenDaysHappyManagement.Instance.getSevenDaysHappySampleDic())
		{
			dayID = item.Key;
			sample = item.Value;
			obj = Instantiate (topButton) as GameObject;
			obj.transform.parent = content.transform;
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localScale = Vector3.one;
			obj.GetComponent<UIDragScrollView> ().scrollView = scrollView;
			obj.GetComponent<ButtonBase> ().fatherWindow = this;
			obj.name = "day_" + sample.dayId;
			itemButton = obj.GetComponent<DayTopButton> ();
			tabButtons.Add (dayID, itemButton);
			itemButton.textLabel.text = string.Format(LanguageConfigManager.Instance.getLanguage("sevenDaysHappy_name"),dayID.ToString());
			itemButton.setSevenDaysHappySample(sample);
			itemButton.icon.mainTexture = setIcon (dayID);
			itemButton.local_x = grid.cellWidth * (dayID);
			indexxx++;
		}
		grid.onReposition = updateTabButtonView;
		grid.repositionNow = true;
	}

	private void updateTabButtonView ()
	{
		DayTopButton selectBtn = tabButtons [defaultSelectSid] as DayTopButton;
		if(selectBtn == null)
		{
			selectBtn = tabButtons [1] as DayTopButton;
		}
		UIGrid grid = content.GetComponent<UIGrid> ();
		UIScrollView scrollView = topButtons.transform.GetComponent<UIScrollView> ();
		float toX = selectBtn.local_x;
		
		Vector4 clip = scrollView.panel.finalClipRegion;
		if(toX > clip.z)
		{
			toX = toX - clip.z + scrollView.panel.clipSoftness.x;
			scrollView.MoveRelative (new Vector3 (-toX, 0, 0));
			topButtons.GetComponent<UIPanel>().clipOffset = new Vector2(toX,0);
		}
		showDetail (selectBtn);
	}

	private Texture setIcon (int dayID)
	{
		if(dayID == 1)
		{
			return dayTex_1;
		}
		else if(dayID == 2)
		{
			return dayTex_2;
		}
		else if(dayID == 3)
		{
			return dayTex_3;
		}
		else if(dayID == 4)
		{
			return dayTex_4;
		}
		else if(dayID == 5)
		{
			return dayTex_5;
		}
		else if(dayID == 6)
		{
			return dayTex_6;
		}
		else if(dayID == 7)
		{
			return dayTex_7;
		}
		return dayTex;
	}
	
	public override void DoDisable ()
	{
		base.DoDisable ();
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj); 
		if(gameObj.name == "close")
		{
			finishWindow();
		}
		else if(gameObj.name.StartsWith ("day_"))
		{
			DayTopButton button = gameObj.GetComponent<DayTopButton> ();
			int dayIndex = SevenDaysHappyManagement.Instance.getDayIndex();
			if(button.getSevenDaysHappySample().dayId <= dayIndex)
			{
				if (lastSelect.getSevenDaysHappySample().dayId != button.getSevenDaysHappySample().dayId)
				{
					if(lastSelect.detail.type == SevenDaysHappyDetailType.banjiaqianggou)
					{
						sevenDaysHappyContent.banjiaPanel.SetActive(false);
					}
					sevenDaysHappyContent.destroyDetailButtons();
					showDetail (button);
				}
			}
			else
			{
				// 飘字提示活动未开启//
				UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
					win.Initialize (LanguageConfigManager.Instance.getLanguage ("s0171"));
				});
			}
			
			MaskWindow.UnlockUI ();
		}
	}

	private void showDetail (DayTopButton button)
	{
		SevenDaysHappySample sample = button.getSevenDaysHappySample();
		sevenDaysHappyContent.initContent(sample,button);

		if (lastSelect != null)
			lastSelect.selelct.gameObject.SetActive (false);
		button.selelct.gameObject.SetActive (true);
		
		lastSelect = button;
	}
}
