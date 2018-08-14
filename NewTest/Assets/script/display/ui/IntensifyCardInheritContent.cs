using UnityEngine;
using System.Collections;

public class IntensifyCardInheritContent : MonoBase
{
	public const int PROPSID = 71130;
	/** 继承数据的卡片 */
	public SacrificeShowerCtrl oldRole;
	/** 被清数据的原始卡片 */
	public SacrificeShowerCtrl newRole;
	/** 被清数据的原始卡片选择按钮 */
	public ButtonBase buttonOld;
	/** 继承数据的卡片选择按钮 */
	public ButtonBase buttonNew;
	/** 继承进化等级选择 */
	public UIToggle evoCheckBox;
	/** 交换双方装备星魂选择 */
	public UIToggle equipCheckBox;
	/** 继承进化需要的物品 */
	public GoodsView needProp;
	/** 继承进化需要的游戏币数量 */
	public UILabel needMoneyLabel;
	/** 交换装备星魂描述 */
	public UILabel changeDescLabel;

	/** 传承后卡片 */
	private Card newCard;
	/** 传承前卡片 */
	private Card oldCard;
	/** 继承的进化等级 */
	private int inheritEvoLv;
	/** 继承的经验 */
	private long inheritExp;
	/** 继承的技能经验 */
	private long inheritSkillExp;
	/** 继承的附加经验(生命，攻击，防御，魔法，敏捷) */
	private int[] inheritAddonExp;
	/** 继承的装备 */
	private string[] equips;
	/** 传承需要的道具数量 */
	private int needPropNum;
	/** 传承需要的游戏币数量 */
	private long needMoney;

	/** 预警描述 */
	private string isMore;
	private IntensifyCardWindow win;
	private Prop propTemp;

	public void initInfo (IntensifyCardWindow win)
	{
		this.win = win;
		updateCtrl ();
	}

	public void updateCtrl ()
	{
		isMore = "";
        evoCheckBox.gameObject.SetActive(true);
		changeDescLabel.text = LanguageConfigManager.Instance.getLanguage ("inherit_05");
		if (GuideManager.Instance.isMoreThanStep (GuideGlobal.NEWFUNSHOW30)) {
			changeDescLabel.text += LanguageConfigManager.Instance.getLanguage ("inherit_11");
		}
		evoCheckBox.value = false;
		equipCheckBox.value = false;
		inheritAddonExp = new int[5];
		needPropNum = 0;
		if (propTemp == null) {
			propTemp = new Prop (PROPSID, 0);
			needProp.fatherWindow = win;
			needProp.init (propTemp);
		}
		getPropNumStr (needPropNum, 0);

		if (!IntensifyCardManager.Instance.isHaveMainCard ()) {
			buttonOld.gameObject.SetActive (false);
			oldRole.updateShower (IntensifyCardManager.Instance.getMainCard ());
			oldCard = IntensifyCardManager.Instance.getMainCard ().Clone () as Card;
		} else {
			buttonOld.gameObject.SetActive (true);
			oldRole.cleanData ();
			IntensifyCardManager.Instance.clearFood ();
			buttonNew.gameObject.SetActive (true);
			newRole.cleanAll ();
		}
		
		if (!IntensifyCardManager.Instance.isHaveFood ()) {
			buttonNew.gameObject.SetActive (false);
			newRole.updateShower (IntensifyCardManager.Instance.getFoodCard () [0]);
		} else {
			buttonNew.gameObject.SetActive (true);
			newRole.cleanAll ();
		}
        setEvoCheckBoxState();
		setAddAttr ();
		getPropNum ();
	}

	/** 是否能传承进化等级 */
	private bool isCanEvo ()
	{
		if (oldRole.card == null || newRole.card == null) {
			return false;
		}
		if (newRole.card.getEvoTimes () == 0 || EvolutionManagerment.Instance.isMaxEvoLevel (oldRole.card)) {
			return false;
		}
        //看看能否勾选进化
        if (newRole.card.getQualityId () < oldRole.card.getQualityId ()) {//无法从低继承到高(进化等级)
            if (evoCheckBox.value) {
                TextTipWindow.Show (LanguageConfigManager.Instance.getLanguage ("inherit_08"));
            }
            return false;
        } else {
            return true;
        }
	    return true;
	}

	/** 需求道具是否足够 */
	private bool isEnoughProp ()
	{
		if (oldRole.card == null || newRole.card == null) {
			return false;
		}
		if (needPropNum == 0) {
			return true;
		}
		Prop prop = StorageManagerment.Instance.getProp (PROPSID);
		if (prop != null && prop.getNum () >= needPropNum) {
			return true;
		} else {
			return false;
		}
	}

	/** 按钮事件 */
	public void doClieckEvent (GameObject gameObj)
	{
		if (gameObj.name == "buttonInherit") {
			if (oldRole.card == null || newRole.card == null) {
				MaskWindow.UnlockUI ();
				return;
			}
			if (evoCheckBox.value && !isEnoughProp ()) {
				TextTipWindow.Show (LanguageConfigManager.Instance.getLanguage ("inherit_09", propTemp.getName ()));
				return;
			}
			if (needMoney > UserManager.Instance.self.getMoney ()) {
				TextTipWindow.Show (LanguageConfigManager.Instance.getLanguage ("guild_851"));
				return;
			}
			setAddAttr ();
			if (isMore != "") {
				isMore += "\n" + LanguageConfigManager.Instance.getLanguage ("inherit_err3");
				MessageWindow.ShowConfirm (isMore, chooseOk);
				return;
			}
			IntensifyCardShowWindow.Show (oldCard, newCard, () => {
				inherit (playEffect);
			});

		}
	}

	private void chooseOk (MessageHandle msg)
	{
		if (msg.buttonID == MessageHandle.BUTTON_RIGHT) {
			IntensifyCardShowWindow.Show (oldCard, newCard, () => {
				inherit (playEffect);
			});
		}
	}

	/** 开始传承 */
	private void inherit (CallBack _callback)
	{
		if (UiManager.Instance.getWindow<CardBookWindow> () != null) {
			UiManager.Instance.getWindow<CardBookWindow> ().destoryWindow ();
		}
//		if (UiManager.Instance.getWindow<TeamEditWindow> () != null) {
//			UiManager.Instance.getWindow<TeamEditWindow> ().destoryWindow ();
//		}
		Card tmpCard = StorageManagerment.Instance.getRole (newRole.card.uid);
		tmpCard.isInherit = true;
		InheritFPort fport = FPortManager.Instance.getFPort ("InheritFPort") as InheritFPort;
		fport.inherit (oldRole.card.uid, newRole.card.uid, equipCheckBox.value ? 1 : 0, evoCheckBox.value ? 1 : 0, _callback);
	}

	/** 播放特效 */
	private void playEffect ()
	{
		isMore = "";

		if (equipCheckBox.value) {
			string[] temp = oldRole.card.getEquips ();
			oldRole.card.setEquips (newRole.card.getEquips ());
			newRole.card.setEquips (temp);

			ArrayList tempStarSoul = oldRole.card.getStarSoulArrayList ();
			oldRole.card.setStarSoul (newRole.card.getStarSoulArrayList ());
			newRole.card.setStarSoul (tempStarSoul);
		}

		newCard = StorageManagerment.Instance.getRole (oldCard.uid);

		UiManager.Instance.openWindow<SkillLevelUpWindow> ((window) => {
			window.Initialize (oldCard, newCard, inheritOver);
		});
	}

	/** 特效播完,结束传承 */
	private void inheritOver ()
	{
		MaskWindow.UnlockUI ();
		clearData ();
		IntensifyCardManager.Instance.intoIntensify (IntensifyCardManager.INTENSIFY_CARD_INHERIT, win.getCallBack ());
	}

	/** 计算附加经验 */
	private void addAddon (Card _card, int[] addon)
	{
		if (addon == null) {
			return;
		}
		_card.updateHPExp (_card.getHPExp () + addon [0]);//HP附加经验
		_card.updateATTExp (_card.getATTExp () + addon [1]);//攻击
		_card.updateDEFExp (_card.getDEFExp () + addon [2]);//防御
		_card.updateMAGICExp (_card.getMAGICExp () + addon [3]);//魔法
		_card.updateAGILEExp (_card.getAGILEExp () + addon [4]);//敏捷
	}

	/** 计算消耗 */
	public void getPropNum ()
	{
		if (!isCanEvo ()) {
			evoCheckBox.value = false;
		}
		initCost ();
	}

	/** 计算消耗 */
	private void initCost ()
	{
		if (oldRole.card == null || newRole.card == null) {
			return;
		}
		needMoney = 0;
		needPropNum = 0;

		if (evoCheckBox.value) {
			//被传承方(接受数据oldRole)----传承方(被清数据newRole)
			//如果发生传承进化行为，则道具消耗数量=int(max（（被传承方配置价值-传承方配置价值/3）*传承方进化经验/50，0）+传承方等级/10)
			//如果发生传承进化行为，则游戏币消耗值=被传承方MIN(双方进化经验之和的对应进化等级，54）对应的进化游戏币配置值-被传承方进化阶位对应的进化游戏币配置值+50*传承方等级^2

			//被传承卡片的价值
			int oldValue = EvolutionManagerment.Instance.getEvoValue (oldRole.card);
			//接受传承卡片的价值
			int newValue = EvolutionManagerment.Instance.getEvoValue (newRole.card);

			needPropNum = (int)(Mathf.Max ((oldValue - newValue / 3) * newRole.card.getEvoTimes () / 50, 0) + newRole.card.getLevel () / 10);

			int newLv = EXPSampleManager.Instance.getLevel (EXPSampleManager.SID_EVO_EXP,oldRole.card.getEvoTimes () + newRole.card.getEvoTimes ());
			int a = Mathf.Min (newLv, 54);
			long evoMoney = EvolutionManagerment.Instance.getCostMoney (oldRole.card.getEvolveNextSid (),a);
			long oldEvoMoney = EvolutionManagerment.Instance.getCostMoney (oldRole.card);

			needMoney = evoMoney - oldEvoMoney + 50 * (int)Mathf.Pow (newRole.card.getLevel (), 2);
		} else {
			//如果不发生传承进化行为，则道具消耗数量=int（传承方等级/10)
			//如果不发生传承进化行为，则游戏币消耗值=50*传承方等级^2
			needPropNum = (int)(newRole.card.getLevel () / 10);

			needMoney = 50 * (int)Mathf.Pow (newRole.card.getLevel (), 2);

		}
		getPropNumStr (needPropNum, needMoney);
	}

	private void getPropNumStr (int _needPropNum, long _needMoney)
	{
		needProp.rightBottomText.gameObject.SetActive (true);
		int havePropNum = StorageManagerment.Instance.getProp (PROPSID) == null ? 0 : StorageManagerment.Instance.getProp (PROPSID).getNum ();
		needProp.rightBottomText.text = (_needPropNum > havePropNum ? Colors.RED : Colors.WHITE) + havePropNum + "/" + _needPropNum;

		int haveMoney = UserManager.Instance.self.getMoney ();
		needMoneyLabel.text = (_needMoney > haveMoney ? Colors.RED : Colors.WHITE) + haveMoney + "/" + _needMoney;

		if (isEnoughProp () && haveMoney >= _needMoney) {
			win.inheritButton.disableButton (false);
		} else {
			win.inheritButton.disableButton (true);
		}
	}
    /// <summary>
    /// //红卡不可以传承进化次数
    /// </summary>
    private void setEvoCheckBoxState() {
        if (newRole.card == null && oldRole.card == null)
            return;
        if (newRole.card != null && newRole.card.getQualityId() == QualityType.MYTH) {
            evoCheckBox.gameObject.SetActive(false);
        }
        if (oldRole.card != null && oldRole.card.getQualityId() == QualityType.MYTH) {
            evoCheckBox.gameObject.SetActive(false);
        }
    }
	/** 计算传承数据,得到新的卡片 */
	private void setAddAttr ()
	{
		if (oldRole.card == null || newRole.card == null) {
			return;
		}
		isMore = "";
		inheritEvoLv = newRole.card.getEvoTimes ();
		inheritExp = newRole.card.getEXP ();
		inheritSkillExp = newRole.card.getSkillsExp ();
		inheritAddonExp [0] = newRole.card.getHPExp ();
		inheritAddonExp [1] = newRole.card.getATTExp ();
		inheritAddonExp [2] = newRole.card.getDEFExp ();
		inheritAddonExp [3] = newRole.card.getMAGICExp ();
		inheritAddonExp [4] = newRole.card.getAGILEExp ();
		equips = newRole.card.getEquips ();

		//进化等级-卡片经验-技能经验-附加经验
		newCard = oldCard.Clone () as Card;
		if (evoCheckBox.value && oldRole.card.getQualityId () <= newRole.card.getQualityId ()) {
            if ((newCard.getEvoTimes() + inheritEvoLv) > EvolutionManagerment.Instance.getMaxLevel(newCard)) {
				isMore += LanguageConfigManager.Instance.getLanguage ("inherit_err2");
			}
			newCard.updateEvoLevel (newCard.getEvoTimes () + inheritEvoLv);
		}
		Card mainCard = StorageManagerment.Instance.getRole (UserManager.Instance.self.mainCardUid);
		long tmpExp = newCard.checkExp (inheritExp);
		if (tmpExp != -1) {
			if (isMore != "") {
				if(newCard.getEXP () + inheritExp >= mainCard.getEXP()) //检查是否受主卡影响
					isMore += "\n" + LanguageConfigManager.Instance.getLanguage ("inherit_err0");
				else
					isMore += "\n" + LanguageConfigManager.Instance.getLanguage ("inherit_err4");
			} else {
				if(newCard.getEXP () + inheritExp >= mainCard.getEXP()) //检查是否受主卡影响
					isMore += LanguageConfigManager.Instance.getLanguage ("inherit_err0");
				else
					isMore += LanguageConfigManager.Instance.getLanguage ("inherit_err4");
			}
			newCard.updateExp (tmpExp);
		} else
			newCard.updateExp (newCard.getEXP () + inheritExp);

		if (newCard.isSkillExpUpFull ((int)inheritSkillExp)) {
			if (isMore != "") {
				isMore += "\n" + LanguageConfigManager.Instance.getLanguage ("inherit_err1");
			} else {
				isMore += LanguageConfigManager.Instance.getLanguage ("inherit_err1");
			}
		}

		if (newCard.getSkills () != null) {
			newCard.getSkills () [0].updateExp (newCard.getSkills () [0].getEXP () + inheritSkillExp);
		}
		if (newCard.getBuffSkills () != null) {
			newCard.getBuffSkills () [0].updateExp (newCard.getBuffSkills () [0].getEXP () + inheritSkillExp);
		}
		if (newCard.getAttrSkills () != null) {
			newCard.getAttrSkills () [0].updateExp (newCard.getAttrSkills () [0].getEXP () + inheritSkillExp);
		}

		addAddon (newCard, inheritAddonExp);
		if (equipCheckBox.value) {
			newCard.setEquips (equips);
		}
		newCard.uid = "-1";
	}

	private void clearData ()
	{
		IntensifyCardManager.Instance.clearData ();
		oldRole.cleanAll ();
		newRole.cleanAll ();
		newCard = null;
		oldCard = null;
		inheritEvoLv = 0;
		inheritExp = 0;
		inheritSkillExp = 0;
		inheritAddonExp = null;
		equips = null;
		isMore = "";
	}
}
