using UnityEngine;
using System.Collections;

/// <summary>
/// 星魂穿戴装备按钮
/// </summary>
public class SoulInfoButtonItem : ButtonBase {

	/* fields */
	/** 魂显示条目预制件 */
	public GameObject viewPerfabe;
	public UISprite lvClose;
	public UISprite lvOpen;
	public GameObject viewPoint;
	/** 等级开放标签 */
	public UILabel unOpenLabel;
	/** 当前激活的星魂 */
	private StarSoul starSoul;
	/** 当前激活的卡片 */
	private Card currentcard;
	/**当前凹槽的位置(1开始) */
	private int currentStarBroeIndex;
	private int flagChange;//是不是可以点按钮 只有自己的可以点

	/* methods */
	/// <summary>
	/// 初始化
	/// </summary>
	public void init(int currentStarBroeIndex,Card card,WindowBase win,int flag) {
		currentcard=card;
		fatherWindow=win;
		flagChange=flag;
		this.currentStarBroeIndex=currentStarBroeIndex;
		//得到指定位置的星魂（包括位置）
		StarSoulBore starSoulBore=currentcard.getStarSoulBoreByIndex(currentStarBroeIndex);
		if (starSoulBore != null) {
            if(starSoulBore.getSid()!=0){
                starSoul = new StarSoul("0", starSoulBore.getSid(), starSoulBore.getExp(), 1);
            } else {
                starSoul = StorageManagerment.Instance.getStarSoul(starSoulBore.getUid());// 自己的
            }
			
		} else {
			starSoul = null;
		}
		this.onClickEvent=grooveButtonClickHander;
		UpdateUI ();
	}
	/** 更新UI */
	public void UpdateUI() {
		updateStarSoulBoreButton();
		updateStarSoulView();
	}
	/// <summary>
	/// 更新星魂槽按钮
	/// </summary>
	private void updateStarSoulBoreButton() {
		lvOpen.gameObject.SetActive(false);
		lvClose.gameObject.SetActive(false);
		unOpenLabel.gameObject.SetActive(false);
        if (flagChange != CardBookWindow.CLICKCHATSHOW && !StarSoulManager.Instance.checkBroeOpenLev(currentcard, currentStarBroeIndex)) {
			lvClose.gameObject.SetActive(true);
			unOpenLabel.gameObject.SetActive(true);
			unOpenLabel.text=LanguageConfigManager.Instance.getLanguage("warchoose04",StarSoulConfigManager.Instance.getGrooveOpen()[currentStarBroeIndex-1].ToString());
			this.GetComponent<BoxCollider>().enabled=false;
		} else {
			this.GetComponent<BoxCollider>().enabled=true;
			if(starSoul!=null){
				lvOpen.gameObject.SetActive(false);
			}else{
				lvOpen.gameObject.SetActive(true);
			}
		}
	}
	/// <summary>
	/// 更新星魂视图
	/// </summary>
	private void updateStarSoulView() {
		if (starSoul == null) {
			if (viewPoint.transform.childCount > 0)
				Utils.RemoveAllChild (viewPoint.transform);		
		} else {
			GameObject obj;
			if (viewPoint.transform.childCount > 0)
				obj = viewPoint.transform.GetChild(0).gameObject;
			else
				obj = NGUITools.AddChild(viewPoint,viewPerfabe);
			GoodsView gv = obj.GetComponent<GoodsView>();
			gv.setFatherWindow (fatherWindow);
			gv.onClickCallback=grooveButtonClickHanderr;
			gv.init(starSoul,GoodsView.BOTTOM_TEXT_NAME_LV);
		}
	}
	/// <summary>
	/// 凹槽按钮的点击事件
	/// </summary>
	private void grooveButtonClickHander(GameObject obj) {
		if(starSoul!=null) {
			UiManager.Instance.openDialogWindow<StarSoulAttrWindow>((win)=>{
                if (flagChange != CardBookWindow.CLICKCHATSHOW) {
                    win.Initialize(starSoul, StarSoulAttrWindow.AttrWindowType.Power_Replace_Unsnatch, currentcard, currentStarBroeIndex, flagChange);
                } else {
                    win.Initialize(starSoul, StarSoulAttrWindow.AttrWindowType.None, currentcard, currentStarBroeIndex, flagChange);
                }
				
			});
        } else if (starSoul == null&& flagChange != CardBookWindow.CLICKCHATSHOW) { //没有星魂用于装配
			UiManager.Instance.openWindow<StarSoulStoreAloneWindow>((win)=>{
				StarSoulManager manager=StarSoulManager.Instance;
				manager.setActiveInfo(currentcard,currentStarBroeIndex);
				win.init (currentcard,starSoul,ButtonStoreStarSoul.ButtonStateType.PutOn);
			});
		}else{
			MaskWindow.UnlockUI();
		}
		
	}
	private void grooveButtonClickHanderr(){
		if(starSoul!=null) {
			UiManager.Instance.openDialogWindow<StarSoulAttrWindow>((win)=>{
				win.Initialize (starSoul, StarSoulAttrWindow.AttrWindowType.Power_Replace_Unsnatch,currentcard,currentStarBroeIndex,flagChange);
			});
		}
	}
}
