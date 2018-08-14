using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 恶魔挑战奖励窗口
/// </summary>
public class BossAwardWindow : WindowBase {

	/* gameojb fields */
	/** 奖励容器 */
	public DelegatedynamicContent awardContent;
	/** goods预制体 */
	public GameObject goodsViewPrefab;
	/** 展示的奖品 */
	private PrizeSample[] prizes;
	/** */
	private int[] missionList;
    private CallBack callBack;
    public UILabel damageValue;

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

    public void init(Award[] aw,CallBack callback) {
        this.prizes = AllAwardViewManagerment.Instance.exchangeAwards(aw);
        this.prizes = AllAwardViewManagerment.Instance.contrastToArray(this.prizes);
        this.callBack = callback;
        GetBossAttackFPort fport = FPortManager.Instance.getFPort("GetBossAttackFPort") as GetBossAttackFPort;
        fport.access(CommandConfigManager.Instance.getBossFightSid(), UpdateUI);
        //UpdateUI();
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
        GetBossAttackFPort fport = FPortManager.Instance.getFPort("GetBossAttackFPort") as GetBossAttackFPort;
        fport.access(CommandConfigManager.Instance.getBossFightSid(), UpdateUI);
        //UpdateUI();
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
        //damageValue.text = StringKit.toLong(AttackBossOneOnOneManager.Instance.getTotalDamage()) - AttackBossOneOnOneManager.Instance.damageTemp + "";
		damageValue.text = AttackBossOneOnOneManager.Instance.damageValue.ToString();
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
            //if (FuBenManagerment.Instance.tmpStorageVersion != StorageManagerment.Instance.tmpStorageVersion) {
            //    MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage("s0122"));
            //}
            if (UiManager.Instance.getWindow<OneOnOneBossWindow>() != null) {
                if (callBack != null) {
                    callBack();
                    callBack = null;
                }
                finishWindow();
            }
			
			//finishWindow ();
		}
	}
}