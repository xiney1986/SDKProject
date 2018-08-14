using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//血脉节点激活补偿奖励
public class BloodEvoWindow : WindowBase
{
//	public UIGrid content;
	public DelegatedynamicContent awardContent;//奖励容器
    public GameObject contentFather;
	/** 关闭按钮 */
	public ButtonBase closeButton;
	/** 确定按钮 */
	public ButtonBase confirmButton;
	public GameObject goodsViewPrefab;
	public GameObject downArrow;
	private PrizeSample[] prizes;
	private CallBack backComfire;
	/** 顶部标题 */
	public UILabel topLabel;
	
	protected override void begin ()
	{
		base.begin ();
		if (!isAwakeformHide) {
			if (prizes != null && prizes.Length > 0) {
				awardContent.reLoad (prizes.Length);
				if (prizes.Length >= 3) {
                    contentFather.transform.localPosition = new Vector3(0, 0, 0);
				} else {
                    contentFather.transform.localPosition = new Vector3(150, 0, 0);
                    awardContent.transform.GetComponent<UIPanel>().clipOffset += new Vector2(-150, 0);
				}
			}
		}
		awardContent.SetUpdateItemCallback (onUpdateItem);
		awardContent.SetinitCallback (initItem);

		GuideManager.Instance.guideEvent ();
		MaskWindow.UnlockUI ();
	}

	GameObject onUpdateItem (GameObject item, int i)
	{
		PrizeSample prizeSample = prizes [i];
		if (item == null) {
			item = NGUITools.AddChild (awardContent.gameObject, goodsViewPrefab);
		}
		GoodsView button = item.GetComponent<GoodsView> ();
		button.fatherWindow = this;
		button.init (prizeSample);
		return item;
	}
	GameObject initItem ( int i)
	{
		PrizeSample prizeSample = prizes [i];

		GameObject		item = NGUITools.AddChild (awardContent.gameObject, goodsViewPrefab);

		GoodsView button = item.GetComponent<GoodsView> ();
		button.fatherWindow = this;
		button.init (prizeSample);
		return item;
	}
	
	public override void DoDisable ()
	{
		base.DoDisable ();
	}

	/* PrizeSample[] */

	public void Initialize (PrizeSample[] _prizes)
	{
		Initialize (_prizes, null, null, null);
	}

	public void Initialize (PrizeSample[] _prizes, CallBack _callback, string topDesc, string downDesc)
	{
		prizes = AllAwardViewManagerment.Instance.contrastToArray (_prizes);
		backComfire = _callback;
        topLabel.text = LanguageConfigManager.Instance.getLanguage("bloodEvoBuChang");
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		
		if (gameObj.name == "close") {
			GuideManager.Instance.doGuide (); 
			GuideManager.Instance.guideEvent ();
			if (fatherWindow is StoreWindow) {
				(fatherWindow as StoreWindow).updateContent ();
			}else if(fatherWindow is LaddersWindow){
				finishWindow ();
				if (backComfire != null) {
					backComfire ();
					backComfire = null;
				}
				return;
			}else if(fatherWindow is TreasureChestWindow){
				EventDelegate.Add (OnHide, () => {
					(fatherWindow as TreasureChestWindow).updateWindow();
				});
			}
			finishWindow ();
            BloodManagement.Instance.ClearPrizes();
		}
	}
}