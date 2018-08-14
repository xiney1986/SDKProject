using UnityEngine;
using System.Collections;

public class SwapPropsWindow : WindowBase {

	/** 继承进化等级选择 */
	public UIToggle starCheckBox;
	/** 交换双方装备星魂选择 */
	public UIToggle equipCheckBox;
	/**title Label */
	public UILabel titleLabel;

	private Card oldCard;
	private Card newCard;
	private CallBack callBack;

	public static void Show (Card oldCard,Card newCard,CallBack callBack)
	{
		UiManager.Instance.openDialogWindow<SwapPropsWindow> ((win) => {
			win.initWindow (oldCard,newCard,callBack);
		});
	}

	public void initWindow (Card oldCard,Card newCard,CallBack callBack)
	{
		this.oldCard = oldCard;
		this.newCard = newCard;
		this.callBack = callBack;
	}

	protected override void begin ()
	{
		base.begin ();

		if (oldCard.getEquips () != null && oldCard.getEquips ().Length > 0) {
			equipCheckBox.value = true;
			equipCheckBox.enabled = true;
		} else {
			equipCheckBox.value = false;
			equipCheckBox.enabled = false;
		}

		if (oldCard.getStarSoulByAll () != null) {
			if(starCheckBox.transform.parent!=null) {
				starCheckBox.transform.parent.gameObject.SetActive(true);
				titleLabel.text=LanguageConfigManager.Instance.getLanguage("swapProps00");
			}
			starCheckBox.value = true;
			starCheckBox.enabled = true;
		} else {
			if(starCheckBox.transform.parent!=null) {
				starCheckBox.transform.parent.gameObject.SetActive(false);
				titleLabel.text=LanguageConfigManager.Instance.getLanguage("swapProps05");
			}
			starCheckBox.value = false;
			starCheckBox.enabled = false;
		}
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);

		if (gameObj.name == "buttonCancel") {
			finishWindow ();
			callBack ();
		}
		else if (gameObj.name == "buttonOk") {
			string checkType = "";
			if (equipCheckBox.value) {
				checkType = "" + EquipSwapFPort.TYPE_EQUIP;
			}
			if (starCheckBox.value) {
				if (string.IsNullOrEmpty (checkType)) {
					checkType = "" + EquipSwapFPort.TYPE_STARSOUL;
				} else {
					checkType += "," + EquipSwapFPort.TYPE_STARSOUL;
				}
			}

			if (string.IsNullOrEmpty (checkType)) {
				finishWindow ();
				callBack ();
			} else {
				EquipSwapFPort fport = FPortManager.Instance.getFPort ("EquipSwapFPort") as EquipSwapFPort;
				fport.access (oldCard.uid, newCard.uid,checkType, (result) => {
					swapOver (result);
				});
			}
		}
	}

	void swapOver (bool isSwap)
	{
		if (equipCheckBox.value) {
			string[] temp = oldCard.getEquips ();
			oldCard.setEquips (newCard.getEquips ());
			newCard.setEquips (temp);
		}

		if (starCheckBox.value) {
			ArrayList temp = oldCard.getStarSoulArrayList ();
			oldCard.setStarSoul (newCard.getStarSoulArrayList ());
			newCard.setStarSoul (temp);
		}

		finishWindow ();
		callBack ();
	}
}
