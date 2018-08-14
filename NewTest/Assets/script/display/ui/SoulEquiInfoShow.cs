using UnityEngine;
using System.Collections;

public class SoulEquiInfoShow : MonoBase {

	public StarSoulEquipContent starSoulContent;
	//public StarSoulWindow fatherwin;
	public WindowBase fatherwin;
	public ButtonBase[] grooveButton;
	public UILabel teamLabel;
	public GameObject[] starSoulPoints;
	public GameObject starSoulShowPrefab;
	public UILabel combat;
	public ButtonBase teamButton;
	public Card currectCard;
	int oldCombat = 0;//初始战斗力
	int newCombat = 0;//最新战斗力
	int step;//步进跳跃值
	private bool isRefreshCombat = false;//刷新战斗力开关
	private int flagg;
	
	//public 
	void Start()
	{
	}
	public void initInfo(Card card,WindowBase win,int index,int flag)
	{
		currectCard=card;
		//fatherwin=win as StarSoulWindow;
		if(win is StarSoulWindow)
			fatherwin = win as StarSoulWindow;
		if(win is SoulHuntWindow)
			fatherwin = win as SoulHuntWindow;
		
		teamButton.fatherWindow=win;
		this.flagg=flag;
		if(index==1)
		{
			teamLabel.text=LanguageConfigManager.Instance.getLanguage ("s0066");
		}else if(index==10)
		{
			teamLabel.text=LanguageConfigManager.Instance.getLanguage ("StarSoulArmySelectWindow_store");
		}
		else
		{
			teamLabel.text=LanguageConfigManager.Instance.getLanguage ("s0068");
		}
		rushCombat (flag);
		updataIcon(card);
	}
	private void updataIcon(Card card)
	{
		GameObject obj;
		for(int j=0;j<starSoulPoints.Length;j++){
			if(starSoulPoints[j].transform.childCount>0){
				obj = starSoulPoints[j].transform.GetChild(0).gameObject;
			}else{
				obj = NGUITools.AddChild(starSoulPoints[j],starSoulShowPrefab);
			}
			SoulInfoButtonItem soulInfoButton=obj.GetComponent<SoulInfoButtonItem>();
			soulInfoButton.init(StringKit.toInt(starSoulPoints[j].transform.name),card,fatherwin,flagg);
			
		}
	}
	/// <summary>
	/// 点击team按键
	/// </summary>
	public void teamClick()
	{
		UiManager.Instance.openDialogWindow<StarSoulArmySelectWindow> ((win) => {
			win.initWindowI (updateTeamData);
		});
	}
	/// <summary>
	/// 重新更新队伍显示
	/// </summary>
	private void updateTeamData(int i) {
		starSoulContent.updateTData(i);
		//MaskWindow.UnlockUI ();
	}
	/// <summary>
	/// 刷新战斗力
	/// </summary>
	public void rushCombat (int flag)
	{
		if(flag==5)newCombat=currectCard.CardCombat;
		else newCombat = CombatManager.Instance.getCardCombat (currectCard);
		isRefreshCombat = true;
		if (newCombat >= oldCombat)
			step = (int)((float)(newCombat - oldCombat) / 20);
		else
			step = (int)((float)(oldCombat - newCombat) / 20);
		if (step < 1)
			step = 1;
	}
	/// <summary>
	/// 战斗力数值跳动方法
	/// </summary>
	private void refreshCombat ()
	{
		if (oldCombat != newCombat) {
			if (oldCombat < newCombat) {
				oldCombat += step;
				if (oldCombat >= newCombat)
					oldCombat = newCombat;
			} else if (oldCombat > newCombat) {
				oldCombat -= step;
				if (oldCombat <= newCombat)
					oldCombat = newCombat;
			}
			combat.text = oldCombat + "";
		} else {
			isRefreshCombat = false;
			combat.text = newCombat + "";
			oldCombat = newCombat;
		}
	}
	/// <summary>
	/// 时刻更新
	/// </summary>
	void Update ()
	{		
		if (isRefreshCombat) 
		{
			refreshCombat ();
		}
	}
}
