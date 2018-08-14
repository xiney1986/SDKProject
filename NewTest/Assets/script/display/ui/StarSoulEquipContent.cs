using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// 星魂装备容器
/// </summary>
public class StarSoulEquipContent : MonoBase {

	/* fields */
	/**顶部滑动模块 */
	public ContentStarSoulEquiTop contentTop;
	/**中间详细卡片滑动模块 */
	public ContentShowItem contentShowItem;
	/** 顶部滑动模块不显示*/
	public GameObject topPoint;
	public GameObject teamButton;
	public int changeflag=0;//可以穿戴

	/* methods */
	public void init(WindowBase win,int index,Card card) {
		//默认得到激活队伍的数据

		contentShowItem.fatherWindow=win;
		List<Card>  teamData=new List<Card>();
		if(card==null)teamData=StarSoulManager.Instance.getTeamCardData(index);
		else teamData=StarSoulManager.Instance.getTeamCardData(index,card);
        changeflag = index;
		//更新顶部滑动栏数据
		updataShowItemContent(teamData,1);
		if(card!=null){
			topPoint.SetActive(false);
			teamButton.SetActive(false);
		}else{
			topPoint.SetActive(true);
			teamButton.SetActive(true);
            contentTop.fatherWindow = win;
			updataShowTopContent(teamData);
		}
	}
	/** 更新UI */
	public void updateUI() {
		contentShowItem.updateUI();
	}
	/// <summary>
	/// 换队重置
	/// </summary>
	public void updateTData(int chooseTeamIndex) {	
		List<Card> teamData=StarSoulManager.Instance.getTeamCardData(chooseTeamIndex);
		if(teamData.Count==0&&chooseTeamIndex==10){
			UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("StarSoulArmySelectWindow_infoo"));
			return;
		} else{
			contentTop.cleanAll ();
			updataShowTopContent(teamData);
			updataShowItemContent(teamData,chooseTeamIndex);
		}
	}
	/// <summary>
	/// 更新顶部滑块数据
	/// </summary>
	private void updataShowTopContent(List<Card> cards) {
		contentTop.init(cards);
	}
	/// <summary>
	/// 更新中间滑动栏数据
	/// </summary>
	private void updataShowItemContent(List<Card> data,int chooseTeamIndex) {	
		contentShowItem.startIndex=0;		
		if(contentShowItem.callbackUpdateEach==null) {
			contentShowItem.init (this,data,chooseTeamIndex,changeflag);
			contentShowItem.callbackUpdateEach = contentShowItem.updateButton;
			contentShowItem.onCenterItem =contentShowItem.updateActive;	
			contentShowItem.init ();
		} else {
			contentShowItem.init (this,data,chooseTeamIndex,changeflag);
			contentShowItem.callbackUpdateEach = contentShowItem.updateButton;
			contentShowItem.onCenterItem =contentShowItem.updateActive;	
			contentShowItem.init ();
		}
	}
	/** button点击事件 */
	public void buttonEventBase (GameObject gameObj) {
		if(gameObj.name=="teamButton"){
			contentShowItem.soulEquiInfo.teamClick();
		}
	}
}
