using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 讨伐奖励窗口
/// </summary>
public class WarAwardWindow : WindowBase {

	/* gameojb fields */
	/** 奖励容器 */
	public DelegatedynamicContent awardContent;
	/** goods预制体 */
	public GameObject goodsViewPrefab;
	/** 展示的奖品 */
	private PrizeSample[] prizes;
	/** */
	private int[] missionList;

	/* methods */
	/// <summary>
	/// 初始化
	/// </summary>
	/// <param name="prizes">奖品列表</param>
	public void init(Award[] aw) {
		this.prizes=AllAwardViewManagerment.Instance.exchangeAwards(aw);
		this.prizes = AllAwardViewManagerment.Instance.contrastToArray (this.prizes);
		UpdateUI();
	}
	/** begin */
	protected override void begin () {
		base.begin ();
		GuideManager.Instance.guideEvent ();
		MaskWindow.UnlockUI ();
	}
	/// <summary>
	/// 断线重连
	/// </summary>
	public override void OnNetResume () {
		base.OnNetResume ();
		UpdateUI();
	}
	/** 更新UI */
	public void UpdateUI() {
		if (!isAwakeformHide) {
			if (prizes != null && prizes.Length > 0) {
				awardContent.reLoad (prizes.Length);
				awardContent.SetUpdateItemCallback (onUpdateItem);
				awardContent.SetinitCallback (initItem);
			}
		}
	}
	/** 更新条目 */
	GameObject onUpdateItem (GameObject item, int i) {
		PrizeSample prizeSample = prizes [i];
		if (item == null)  {
			item = NGUITools.AddChild (awardContent.gameObject, goodsViewPrefab);
		}
		GoodsView button = item.GetComponent<GoodsView> ();
		button.fatherWindow = this;
		button.init (prizeSample);
		return item;
	}
	/** 初始化条目 */
	GameObject initItem (int i) {
		PrizeSample prizeSample = prizes [i];
		GameObject item = NGUITools.AddChild (awardContent.gameObject, goodsViewPrefab);
		item.transform.localScale = new Vector3(0.9f,0.9f,1f);
		GoodsView button = item.GetComponent<GoodsView> ();
		button.fatherWindow = this;
		button.init (prizeSample);
		return item;
	}
	/** 点击事件  */
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			if (FuBenManagerment.Instance.tmpStorageVersion != StorageManagerment.Instance.tmpStorageVersion) {
				MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage("s0122"));
			}
            if (FuBenManagerment.Instance.isWarActiveFuben)
            {
                FuBenManagerment.Instance.isWarActiveFuben = false;
                FuBenManagerment.Instance.ActiveWinAward = null;
                finishWindow();
            }
            else if (FuBenManagerment.Instance.warWinAward!=null)
            {
                isNewWarchapter(); 
            }
			
			//finishWindow ();
		}
	}

	///<summary>
	/// 判断是否有新的讨伐关卡开启
	/// </summary>
	private void isNewWarchapter()
	{
		bool openOrNot = false;
		FuBenManagerment.Instance.warWinAward = null;
		missionList = FuBenManagerment.Instance.getAllShowMissions (FuBenManagerment.Instance.getWarChapter ().sid);
		for (int i=0; i<missionList.Length; i++) {
			if(MissionInfoManager.Instance .getMissionBySid (missionList[i]).getMissionType() == MissionShowType.NEW){
				openOrNot = true;
				break;
			}
		}

		if (openOrNot) {
			finishWindow();
			if (UiManager.Instance.getWindow<WarChooseWindow> () == null)
//				UiManager.Instance.openMainWindow ();
				UiManager.Instance.openWindow<WarChooseWindow>();
			else
				UiManager.Instance.BackToWindow<WarChooseWindow> ();
		}
		else {
			finishWindow();
		}
	}
}