using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 通用奖励展示主窗口
 * @authro 陈世惟  
 * */
public class AllAwardViewWindow : WindowBase
{
	
//	public UIGrid content;
	public DelegatedynamicContent awardContent;//奖励容器
	/** 分享内容 */
	public UILabel shareLabel;
	/** 关闭按钮 */
	public ButtonBase closeButton;
	/** 确定按钮 */
	public ButtonBase confirmButton;
	public GameObject goodsViewPrefab;
	public GameObject downArrow;
	private PrizeSample[] prizes;
	private CallBack backComfire;
	public ButtonBase shareicon;
	/** 顶部标题 */
	public UILabel topLabel;
	/** 底部贴士 */
	public UILabel downLabel;
	
	protected override void begin ()
	{
		base.begin ();
		if (!isAwakeformHide) {
			if (prizes != null && prizes.Length > 0) {
				awardContent.reLoad (prizes.Length);
				if (prizes.Length > 8) {
					downArrow.SetActive (true);
				} else {
					downArrow.SetActive (false);
				}
			}
		}
		awardContent.SetUpdateItemCallback (onUpdateItem);
		awardContent.SetinitCallback (initItem);

		shareLabel.text=LanguageConfigManager.Instance.getLanguage("shareContent02");

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

	/* List<PrizeSample> */

	public void Initialize (List<PrizeSample> _prizes)
	{
		Initialize (_prizes.ToArray (), null, null, null);
	}

	public void Initialize (List<PrizeSample> _prizes, string topDesc)
	{
		Initialize (_prizes.ToArray (), null, topDesc, null);
	}

	public void Initialize (List<PrizeSample> _prizes, CallBack _callback, string topDesc)
	{
		Initialize (_prizes.ToArray (), _callback, topDesc, null);
	}

	public void Initialize (List<PrizeSample> _prizes, CallBack _callback, string topDesc, string downDesc)
	{
		Initialize (_prizes.ToArray (), _callback, topDesc, downDesc);
	}

	/* Award[] */

	public void Initialize (Award[] awards)
	{
		Initialize (AllAwardViewManagerment.Instance.exchangeAwardsToPrize (awards).ToArray (), null, null, null);
	}

	public void Initialize (Award[] awards, string topDesc)
	{
		Initialize (AllAwardViewManagerment.Instance.exchangeAwardsToPrize (awards).ToArray (), null, topDesc, null);
	}

	public void Initialize (Award[] awards, CallBack _callback, string topDesc)
	{
		Initialize (AllAwardViewManagerment.Instance.exchangeAwardsToPrize (awards).ToArray (), _callback, topDesc, null);
	}

	public void Initialize (Award[] awards, CallBack _callback, string topDesc, string downDesc)
	{
		Initialize (AllAwardViewManagerment.Instance.exchangeAwardsToPrize (awards).ToArray (), _callback, topDesc, downDesc);
	}

	/* Award */

	public void Initialize (Award award)
	{
		Initialize (AllAwardViewManagerment.Instance.exchangeAwardToPrize (award).ToArray (), null, null, null);
	}

	public void Initialize (Award award, string topDesc)
	{
		Initialize (AllAwardViewManagerment.Instance.exchangeAwardToPrize (award).ToArray (), null, topDesc, null);
	}

	public void Initialize (Award award, CallBack _callback, string topDesc)
	{
		Initialize (AllAwardViewManagerment.Instance.exchangeAwardToPrize (award).ToArray (), _callback, topDesc, null);
	}

	public void Initialize (Award award, CallBack _callback, string topDesc, string downDesc)
	{
		Initialize (AllAwardViewManagerment.Instance.exchangeAwardToPrize (award).ToArray (), _callback, topDesc, downDesc);
	}

	/* PrizeSample[] */

	public void Initialize (PrizeSample[] _prizes)
	{
		Initialize (_prizes, null, null, null);
	}

	public void Initialize (PrizeSample[] _prizes, string topDesc)
	{
		Initialize (_prizes, null, topDesc, null);
	}

	public void Initialize (PrizeSample[] _prizes, string topDesc, string downDesc)
	{
		Initialize (_prizes, null, topDesc, downDesc);
	}

	public void Initialize (PrizeSample[] _prizes, CallBack _callback, string topDesc, string downDesc)
	{
		prizes = AllAwardViewManagerment.Instance.contrastToArray (_prizes);
		backComfire = _callback;
		topLabel.text = string.IsNullOrEmpty (topDesc) ? LanguageConfigManager.Instance.getLanguage ("Guild_95") : topDesc;
		downLabel.text = string.IsNullOrEmpty (downDesc) ? "" : downDesc;
	}


	//bl 是否可点击 str 按钮描述
	public void showComfireButton (bool showConfirmButton, string str)
	{
		if (str != null)
			confirmButton.textLabel.text = str;
		confirmButton.gameObject.SetActive (showConfirmButton);
		closeButton.gameObject.SetActive (!showConfirmButton);
	}
	
	public void refereshWin ()
	{
		if (fatherWindow is StoreWindow) {
			StoreWindow win = fatherWindow as StoreWindow;
			win.GetComponent<StoreWindow> ().updateContent ();
		}
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
		} else if (gameObj.name == "confirm") {
			finishWindow ();
			if (backComfire != null) {
				backComfire ();
				backComfire = null;
			}
		}
		else if(gameObj.name=="shareicon"){
			UiManager.Instance.openDialogWindow<OneKeyShareWindow>((win) => { win.initWin(); });
			if (fatherWindow is StoreWindow)
				(fatherWindow as StoreWindow).updateContent ();
            if (backComfire != null) {
                backComfire();
                backComfire = null;
            }
			finishWindow();
		}
	}
}