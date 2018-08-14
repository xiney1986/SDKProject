using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// vip奖励容器
/// </summary>
public class VipAwardContent  : dynamicContent
{

	/* gameobj fields */
	/** 网格格式视图 */
	public GameObject gridItemProfab;
	/** 向下箭头 */
	public GameObject arrowBottom;
	/** 向上箭头 */
	public GameObject arrowUp;

	/* fields */
	/** vip奖励列表 */
	Vip[] vipAwards;

	/** load */
	public void reLoad (Vip[] vips) {

		if (vips == null)
			return;
		vipAwards = vips;
//		int level = Math.Max(UserManager.Instance.self.getVipLevel (),1);
//		List<Vip> lvs = new List<Vip>();
//		for (int i = 0; i < level; i++) {
//			if (vipAwards[i] != null && !VipManagerment.Instance.alreadyGetAward (vipAwards[i].vipAwardSid)) {
//				lvs.Add(vipAwards[i]);
//			}
//		}
//		if (lvs.Count > 0) {
//			base.reLoad (vipAwards.Length,lvs[0].vipLevel);
//		} else {
//			base.reLoad (vipAwards.Length,level);
//		}
		int index=getLastGetIndex();
		if(index<=1)index=0;
		base.reLoad (vipAwards.Length,index);
	}
	public int getLastGetIndex(){
		for(int i=0;i<vipAwards.Length;i++){
			if(!VipManagerment.Instance.alreadyGetAward (vipAwards[i].vipAwardSid))return i-1;
		}
		return 0;
	}
	/** 更新条目信息 */
	public override void updateItem (GameObject item, int index) {
		PrizesGridItem gridItem = item.GetComponent<PrizesGridItem> ();
		if (gridItem == null)
			return;
        if (index >= vipAwards.Length)
            index = vipAwards.Length - 1;
		Vip vip = vipAwards [index];
		string titleText = LanguageConfigManager.Instance.getLanguage("Vip01") + LanguageConfigManager.Instance.getLanguage ("s0506",Convert.ToString(vip.vipLevel));
		gridItem.init(vip,vip.prizes,fatherWindow,receiveAward,titleText);
	}
	/** 初始化条目信息 */
	public override void initButton (int  i) {
		if (nodeList [i] == null){
			nodeList [i] = NGUITools.AddChild (gameObject, gridItemProfab);
		}
	}
	/** 领取奖励 */
	public void receiveAward(PrizesGridItem gridItem,object awardObj) {
		if (!(awardObj is Vip))
			return;
		Vip activeVip = awardObj as Vip;
		if (UserManager.Instance.self.vipLevel >= activeVip.vipLevel) {
			/** 检查仓库容量是否可以接受奖励*/
			string checkResult = "";
			if(	StorageManagerment.Instance.checkStoreFull(gridItem.getPrizes(),out checkResult)){
				UiManager.Instance.createMessageLintWindow (checkResult + LanguageConfigManager.Instance.getLanguage("storeFull_msg_01"));
				return;
			}
			VipFPort port = FPortManager.Instance.getFPort ("VipFPort") as VipFPort;
			port.get_gift (() => {
                    PrizeSample[] ps = VipManagerment.Instance.getVipbyLevel (activeVip.vipLevel).prizes;
                    UiManager.Instance.createPrizeMessageLintWindow(ps);
                    bool isOpenHeroRoad = HeroRoadManagerment.Instance.isOpenHeroRoad(ps);
                    if (isOpenHeroRoad) { 
                        UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
						    win.Initialize (LanguageConfigManager.Instance.getLanguage ("HeroRoad_open"));
				        });
                    }
				if(fatherWindow is VipAwardViewWindow) {
					VipAwardViewWindow vipwin=fatherWindow as VipAwardViewWindow;
					vipwin.updateUI();
				}
			}, VipManagerment.Instance.getVipbyLevel (activeVip.vipLevel).vipAwardSid);
		} else {
			UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("s0316"));
		}
	}
	/** 动态容器到达顶部,底部情况 */
	public override void OnBorder(int state) {
		base.OnBorder (state);
		if (state==-1) {
			arrowUp.SetActive(false);	
			arrowBottom.SetActive(true);	
		} else if(state==0) {
			arrowUp.SetActive(true);
			arrowBottom.SetActive(true);	
		} else if(state==1) {
			arrowUp.SetActive(true);
			arrowBottom.SetActive(false);	
		}
	}
}