using UnityEngine;
using System.Collections;

/// <summary>
/// 星魂图标弹出的属性窗口
/// </summary>
public class StarSoulAttrWindow : WindowBase {

	/* enum */
	/** 属性窗口类型 */
	public enum AttrWindowType {
		None, //都不显示
		Power, //显示强化按钮
		Replace, //显示替换按钮
		PutOff, //显示卸下按钮
		Lock, //显示锁按钮
		Power_Replace, //显示强化,替换按钮
		Power_Unsnatch , //显示强化,卸下按钮
		Replace_Unsnatch, //显示替换,卸下按钮
		Power_Replace_Unsnatch, //显示强化,替换,卸下按钮
	}

	/* fields */
	/** 强化按钮 */
	public ButtonBase buttonPower;
	/** 替换按钮 */
	public ButtonBase buttonReplace;
	/** 卸下按钮 */
	public ButtonBase buttonUnsnatch;
	/** 锁按钮 */
	public ButtonBase buttonLock;
	/** 关闭标签 */
	public UILabel closeLabel;
	/** 星魂图标点 */
	public GameObject starSoulViewPoint;
	/** 星魂锁图标 */
	public UISprite lockSprite;
	/** 星魂名字 */
	public UILabel starSoulNameLabel;
	/** 属性描述 */
	public UILabel attrDescLabel;
    public UILabel attrDescLabel1;
	/** 星魂等级 */
	public UILabel levelValueLabel;
	/** 刻印描述 */
	public UILabel suitDescLabel;
	/** 星魂经验条 */
	public barCtrl expBar;
	/** 经验文本 */
	public UILabel expLabel;
	/** 当前选择的星魂 */
	private StarSoul starSoul;
	/** 当前激活的卡片 */
	Card currentCard;
	/** 当前激活的卡片星魂槽位置 */
	int starBroeIndex;
	/** 显示类型 */
	AttrWindowType showType;

	/***/
	protected override void begin () {
		base.begin ();
		MaskWindow.UnlockUI ();
	}
	/* methods */
	/// <summary>
	/// Initialize
	/// </summary>
	/// <param name="starSoul">星魂</param>
	/// <param name="showType">显示类型</param>
	public void Initialize(StarSoul starSoul,AttrWindowType showType) {
		Initialize (starSoul,showType,null,0,0);
	}
	/// <summary>
	/// Initialize
	/// </summary>
	/// <param name="starSoul">星魂</param>
	/// <param name="showType">显示类型</param>
	/// <param name="card">激活的卡片</param>
	public void Initialize(StarSoul starSoul,AttrWindowType showType,Card card,int starBroeIndex,int flaggg) {
		this.showType = showType;
		this.starSoul = starSoul;
		this.currentCard = card;
		this.starBroeIndex = starBroeIndex;
		InitializeUI (flaggg);
	}
	/// <summary>
	/// Initializes UI
	/// </summary>
	public void InitializeUI(int flaggg) {
		resetButton ();
		updateButton ();
		updateStarSoul ();
        if (flaggg ==CardBookWindow.CLICKCHATSHOW) {
			resetButton ();
			closeLabel.gameObject.SetActive (true);
		}
	}
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if(gameObj.name=="close") {
			this.dialogCloseUnlockUI=true;
			finishWindow ();
		} else if(gameObj.name=="buttonPower") {
			finishWindow ();
			EventDelegate.Add(OnHide,()=>{
				UiManager.Instance.openWindow<StarSoulStoreStrengWindow>((win)=>{
					win.init (starSoul);
				});
			});
		} else if(gameObj.name=="buttonUnsnatch") {
			if(currentCard==null||starBroeIndex==-1) {
				MaskWindow.UnlockUI();
				return;
			}
			finishWindow();
			EventDelegate.Add(OnHide,()=>{
				StarSoulManager.Instance.setInfo(currentCard.uid,starBroeIndex);
				StarSoulManager.Instance.soul = starSoul;
				(FPortManager.Instance.getFPort ("StarSoulEquipFPort") as StarSoulEquipFPort).putOffEquipStarSoulAccess (currentCard.uid,starBroeIndex,()=>{
					putOffFinished();
				});
			});
		} else if(gameObj.name=="buttonReplace") {
			if(currentCard==null||starBroeIndex==-1) {
				MaskWindow.UnlockUI();
				return;
			}
			StarSoulManager manager=StarSoulManager.Instance;
			manager.setActiveInfo(currentCard,starBroeIndex);
			finishWindow();
			EventDelegate.Add(OnHide,()=>{
				UiManager.Instance.openWindow<StarSoulStoreAloneWindow>((win)=>{
					win.init (currentCard,starSoul,ButtonStoreStarSoul.ButtonStateType.Replace);
				});
			});
		}
	}
	/// <summary>
	/// 卸下星魂完成回调
	/// </summary>
	public void putOffFinished() {
//		if (fatherWindow is StarSoulWindow) {
//			StarSoulWindow win=fatherWindow as StarSoulWindow;
//            string str= "";
//            if (StarSoulManager.Instance.getStarSoulDese(starSoul).Split('#').Length > 1) {
//                str = StarSoulManager.Instance.getStarSoulDese(starSoul).Split('#')[0] + StarSoulManager.Instance.getStarSoulDese(starSoul).Split('#')[1];
//            } else {
//                str = StarSoulManager.Instance.getStarSoulDese(starSoul).Split('#')[0];
//            }
//			UiManager.Instance.openDialogWindow<MessageLineWindow> ((winn) => {
//				winn.Initialize (LanguageConfigManager.Instance.getLanguage
//                                ("StarSoulStrengWindow_LOSE", str));
//				winn.dialogCloseUnlockUI=false;
//			});
//			win.UpdateContent();
//		}
		if(fatherWindow is SoulHuntWindow || fatherWindow is StarSoulWindow)
		{
			string str= "";
			if (StarSoulManager.Instance.getStarSoulDese(starSoul).Split('#').Length > 1) {
				str = StarSoulManager.Instance.getStarSoulDese(starSoul).Split('#')[0] + StarSoulManager.Instance.getStarSoulDese(starSoul).Split('#')[1];
			} else {
				str = StarSoulManager.Instance.getStarSoulDese(starSoul).Split('#')[0];
			}
			UiManager.Instance.openDialogWindow<MessageLineWindow> ((winn) => {
				winn.Initialize (LanguageConfigManager.Instance.getLanguage
				                 ("StarSoulStrengWindow_LOSE", str));
				winn.dialogCloseUnlockUI=false;
			});
			if(fatherWindow is SoulHuntWindow)
			{
				SoulHuntWindow win = fatherWindow as SoulHuntWindow;
				win.UpdateContent();
			}
			else if(fatherWindow is StarSoulWindow)
			{
				StarSoulWindow win = fatherWindow as StarSoulWindow;
				win.UpdateContent();
			}
		}
	}
	/// <summary>
	/// 更新星魂信息
	/// </summary>
	public void updateStarSoul () {
		levelValueLabel.text = starSoul.getLevel () + "/" + starSoul.getMaxLevel ();
		starSoulNameLabel.text = QualityManagerment.getQualityColor(starSoul.getQualityId()) + starSoul.getName ();
		long currentLvExp = EXPSampleManager.Instance.getNowEXPShow (starSoul.getEXPSid (), starSoul.getEXP ());
		long currentLvMaxEXP=EXPSampleManager.Instance.getMaxEXPShow (starSoul.getEXPSid (), starSoul.getEXP ());
		if (starSoul.isMaxLevel ()) {
			expBar.updateValue (currentLvMaxEXP,currentLvMaxEXP);
		} else {
			expBar.updateValue (currentLvExp, currentLvMaxEXP);
		}
		expLabel.text = EXPSampleManager.Instance.getExpBarShow (starSoul.getEXPSid (), starSoul.getEXP ());
		if (starSoul.checkState (EquipStateType.LOCKED)) {
			lockSprite.gameObject.SetActive(true);
		} else{
			lockSprite.gameObject.SetActive(false);
		}
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.STARSOUL_ICONPREFAB_PATH+starSoul.getIconId (),starSoulViewPoint.transform,(obj)=>{
			GameObject gameObj=obj as GameObject;
			if(gameObj!=null) {
				Transform childTrans=gameObj.transform;
				if(childTrans!=null){
					StarSoulEffectCtrl effectCtrl=childTrans.gameObject.GetComponent<StarSoulEffectCtrl>();
					effectCtrl.setColor(starSoul.getQualityId());
				}
			}
		});
        string[] str = starSoul.getDescribe().Split('#');
        if (str.Length > 1) {
            attrDescLabel.text = "[A65644]" + str[0].Replace("+", "[3A9663]+");
            attrDescLabel1.text = "[A65644]" + str[1].Replace("+", "[3A9663]+");
        } else {
            attrDescLabel1.text = "[A65644]" + str[0].Replace("+", "[3A9663]+");
        }
		int partNum = 0;
		if (currentCard != null) {
			partNum=currentCard.getStarSoulsPartNum (starSoul.partId);
		}
		// 刻印暂时没有开放
//		StarSoulSuitSample starSoulSuitSample=starSoul.getStarSoulSuitSample ();
//		int needNum = starSoulSuitSample.parts.Length;
//		if (partNum>=needNum) {
//			// 策划文档上还有个龙之刻印:2分是什么东西？？？
//			suitDescLabel.text = "[FFFFFF]"+starSoulSuitSample.name+": "+starSoulSuitSample.desc;
//		} else {
//			suitDescLabel.text = "[999999]"+starSoulSuitSample.name+"("+LanguageConfigManager.Instance.getLanguage("StarSoulWindow_Suit_NotAcitve")+"): "+starSoulSuitSample.desc+"("+
//									LanguageConfigManager.Instance.getLanguage("StarSoulWindow_Suit_Acitve_Desc",needNum.ToString())+")";
//		}
	}
	/***/
	void Update () {
		//UpdateCloseLable ();
	}
	private void UpdateCloseLable() {
		if(closeLabel.gameObject.activeSelf)
			closeLabel.alpha = sin ();
	}
	/// <summary>
	/// 重置button
	/// </summary>
	private void resetButton() {
		buttonPower.gameObject.SetActive (false);
		buttonReplace.gameObject.SetActive (false);
		buttonUnsnatch.gameObject.SetActive (false);
		buttonLock.gameObject.SetActive (false);
		closeLabel.gameObject.SetActive (false);
	}
	/** 根据类型更新button */
	private void updateButton() {
		switch (showType) {
			case AttrWindowType.None:
				closeLabel.gameObject.SetActive (true);
				break;
			case AttrWindowType.Power:
				setButtonPower();
				break;
			case AttrWindowType.Replace:
				buttonReplace.gameObject.SetActive (true);
				break;
			case AttrWindowType.PutOff:
				buttonUnsnatch.gameObject.SetActive (true);
				break;
			case AttrWindowType.Lock:
				closeLabel.gameObject.SetActive (true);
				buttonLock.gameObject.SetActive (true);
				break;
			case AttrWindowType.Power_Replace:
				setButtonPower();
				buttonReplace.gameObject.SetActive (true);
				break;
			case AttrWindowType.Power_Unsnatch:
				setButtonPower();
				buttonUnsnatch.gameObject.SetActive (true);
				break;
			case AttrWindowType.Replace_Unsnatch:
				buttonReplace.gameObject.SetActive (true);
				buttonUnsnatch.gameObject.SetActive (true);
				break;
			case AttrWindowType.Power_Replace_Unsnatch:
				setButtonPower();
				buttonReplace.gameObject.SetActive (true);
				buttonUnsnatch.gameObject.SetActive (true);
				break;
			default:				
				break;
		}
	}
	public void setButtonPower(){
		if (starSoul.isMaxLevel ()||starSoul.getStarSoulType()==0)
			buttonPower.gameObject.SetActive (false);
		else
			buttonPower.gameObject.SetActive (true);
	}
}
