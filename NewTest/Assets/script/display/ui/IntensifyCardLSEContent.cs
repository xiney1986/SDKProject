using UnityEngine;
using System.Collections;

public class IntensifyCardLSEContent : MonoBase
{
	/** 主卡展示组件 */
	public SacrificeShowerCtrl mainRole;
	/** 副卡展示组件 */
	public SacrificeShowerCtrl food;
	/** 选择主卡按钮 */
	public ButtonBase buttonMain;
	/** 选择副卡按钮 */
	public ButtonBase buttonFood;
	/** 临时进化后展示卡片 */
	public RoleView tempEvoCard;
	/** 进化前最大等级 */
	public UILabel oldMaxLevelLabel;
	/** 进化后最大等级 */
	public UILabel newMaxLevelLabel;
	/** 进化前进化等级 */
	public UILabel oldEvoLevelLabel;
	/** 进化后进化等级*/
	public UILabel newEvoLevelLabel;
	/** 进化前战斗力 */
	public UILabel oldCombatLabel;
	/** 进化后战斗力 */
	public UILabel newCombatLabel;
	/** 天赋觉醒说明 */
	public UILabel talentLabel;
	public GameObject evoTitel;
	public GameObject lvTitel;

	private IntensifyCardWindow win;
	private int intoType = 0;
	private string cost;
	private string evolutionCardId;
	private int evoType;
	private Card newCard;//进化后卡片
	private Card oldCard;//进化前卡片
	/** 万能卡道具 */
	private Prop foodProp;
	/** 预警描述 */
	private string isMore;
	/** 继承的进化等级 */
	private int inheritEvoLv;
	/** 继承的经验 */
	private long inheritExp;
	/** 继承的技能经验 */
	private long inheritSkillExp;
	/** 继承的附加经验(生命，攻击，防御，魔法，敏捷) */
	private int[] inheritAddonExp;
	/** 临时存放的进化前卡片 */
	private Card tempCard;

	/// <summary>
	/// 更新进化信息
	/// </summary>
	/// <param name="win">父窗口.</param>
	/// <param name="isChooseFood">如果设置 <c>true</c> 需要选择祭品卡.</param>
	public void updateCtrl (IntensifyCardWindow _win,bool isChooseFood)
	{
		this.win = _win;
		clearLabelMsg ();
		inheritAddonExp = new int[5];
		//主卡不为空
		if (!IntensifyCardManager.Instance.isHaveMainCard ()) {
			Card tmpFoodCard = null;
			if (!IntensifyCardManager.Instance.isHaveFood ()) {
				tmpFoodCard = IntensifyCardManager.Instance.getFoodCard () [0];
			}

			buttonMain.gameObject.SetActive (false);
			mainRole.updateShower (IntensifyCardManager.Instance.getMainCard ());
            /** 如果主卡在超进化阶段 */
            if (mainRole.getCard().isInSuperEvo()) {
                clearRoleAndFood();
                UiManager.Instance.createMessageLintWindow(Language("Intensify28"));
                win.costSum.text = "0";
                return;
            }
            /** 进化到最大等级 */
            else if (mainRole.getCard().isMaxEvoLevel()) {
                clearRoleAndFood();
                UiManager.Instance.createMessageLintWindow(Language("Intensify30"));
                win.costSum.text = "0";
                return;
            }
			//主卡不为空就自动上副卡(优先卡片再万能卡)
			if(isChooseFood) {
				//先检测有没有副卡，有副卡的时候匹配主卡进化唯一标识，不符合就清理掉自动选
				Card tmpMain = IntensifyCardManager.Instance.getMainCard ();
				if (tmpFoodCard != null && tmpFoodCard.getEvolveNextSid () == tmpMain.getEvolveNextSid ()) {
					food.updateShower (tmpFoodCard);
				} else {
					IntensifyCardManager.Instance.clearFood ();
					Card foodEvoCard = EvolutionManagerment.Instance.getFoodCardForEvo (IntensifyCardManager.Instance.getMainCard ());
					Prop foodEvoProp = EvolutionManagerment.Instance.getCardByQuilty (IntensifyCardManager.Instance.getMainCard ());
                    if (tmpMain.getQualityId() == QualityType.MYTH) { //主卡是神话级卡片
                        if (tmpMain.getEvoLevel() < 4) {//副卡选择时，万能卡优先(进化等级<= 4)
                            if ((foodEvoCard != null && foodEvoProp != null) || (foodEvoCard == null && foodEvoProp != null)) {
                                IntensifyCardManager.Instance.setFoodProp(foodEvoProp);
                                initEvoChooseByProp(IntensifyCardManager.Instance.getFoodProp());
                            } else if ((foodEvoCard != null && foodEvoProp == null)) {
                                IntensifyCardManager.Instance.setFoodCard(foodEvoCard);
                                tmpFoodCard = foodEvoCard;
                                food.updateShower(IntensifyCardManager.Instance.getFoodCard()[0]);
                            }
                        } else { //副卡选择时，卡片优先(进化等级> 4)
                            if ((foodEvoCard != null && foodEvoProp != null) || (foodEvoCard != null && foodEvoProp == null)) {
                                IntensifyCardManager.Instance.setFoodCard(foodEvoCard);
                                tmpFoodCard = foodEvoCard;
                                food.updateShower(IntensifyCardManager.Instance.getFoodCard()[0]);
                            } else if (foodEvoCard == null && foodEvoProp != null) {
                                IntensifyCardManager.Instance.setFoodProp(foodEvoProp);
                                initEvoChooseByProp(IntensifyCardManager.Instance.getFoodProp());
                            }
                        }
                    }
					else{
						if((foodEvoCard != null && foodEvoProp != null) || (foodEvoCard != null && foodEvoProp == null)) {
							IntensifyCardManager.Instance.setFoodCard (foodEvoCard);
							tmpFoodCard=foodEvoCard;
							food.updateShower (IntensifyCardManager.Instance.getFoodCard () [0]);
						}
						else if (foodEvoCard == null && foodEvoProp != null) {
							IntensifyCardManager.Instance.setFoodProp (foodEvoProp);
							initEvoChooseByProp (IntensifyCardManager.Instance.getFoodProp ());
						}  
					}                 
				}
			}

			//只要有主卡,就无论有没副卡都要显示进化后效果

			Card tmpCard = getTempEvoCard (IntensifyCardManager.Instance.getMainCard (),tmpFoodCard);
			if (tmpCard != null) {
				tempEvoCard.gameObject.SetActive (true);
				tempEvoCard.init (tmpCard,this.win,(role)=>{
					CardBookWindow.Show (tmpCard,CardBookWindow.OTHER,null);
				});
				setLabelMsg (IntensifyCardManager.Instance.getMainCard (), tmpCard);
			} else {
				tempEvoCard.gameObject.SetActive (false);
			}
		} else {
            clearRoleAndFood();
		}

		if (!IntensifyCardManager.Instance.isHaveFood ()) {
			buttonFood.gameObject.SetActive (false);
			food.updateShower (IntensifyCardManager.Instance.getFoodCard () [0]);
			_win.setEvoType(1);
		} else if (IntensifyCardManager.Instance.getFoodProp () != null) {
			initEvoChooseByProp (IntensifyCardManager.Instance.getFoodProp ());
			_win.setEvoType(2);
		} else {
			buttonFood.gameObject.SetActive (true);
			food.cleanAll ();
		}
	}

	/// <summary>
	/// 显示万能卡
	/// </summary>
	public void initEvoChooseByProp (Prop prop)
	{
        IntensifyCardManager.Instance.clearFood();
        IntensifyCardManager.Instance.setFoodProp(prop);
		buttonFood.gameObject.SetActive (false);
		food.updateShowerByProp (prop);
	}

	/// <summary>
	/// 强化入口
	/// </summary>
	/// <param name="intoType">窗口类型.</param>
	/// <param name="cost">消耗.</param>
	/// <param name="evoType">进化类型1普通卡,2万能卡.</param>
	public void intensify (int intoType, string cost, int evoType)
	{
		this.intoType = intoType;
		this.evoType = evoType;
		if (intoType == IntensifyCardManager.INTENSIFY_CARD_LEARNSKILL) {
			UiManager.Instance.openWindow<SkillChooseWindow>((window)=>{
				window.Initialize (IntensifyCardManager.Instance.getMainCard (), IntensifyCardManager.Instance.getFoodCard () [0], cost, SkillChooseWindow.TYPE_SKILLLEARN);
			});
			clearData ();
		} else if (intoType == IntensifyCardManager.INTENSIFY_CARD_EVOLUTION) {
			sendEvoData ();
		}
	}

	/// <summary>
	/// 进化端口
	/// </summary>
	private void sendEvoData ()
	{
		tempCard = mainRole.getCard ().Clone () as Card;
		EvolutionFPort fport = FPortManager.Instance.getFPort ("EvolutionFPort") as EvolutionFPort;
		
		if (evoType == 2) {
			foodProp = PropManagerment.Instance.createProp(EvolutionManagerment.Instance.getCardByQuilty (mainRole.getCard ()).sid);
			fport.evolutionCard (mainRole.getCard(), playEffect);
		}
		else
		{
			fport.evolutionCard (mainRole.getCard (), food.getCard(), playEffect);
		}
	}

	/// <summary>
	/// 进化结束，开始播动画
	/// </summary>
	private void playEffect ()
	{
		newCard = StorageManagerment.Instance.getRole (mainRole.getCardUid ());
		UiManager.Instance.openWindow<EmptyWindow>((win)=> {
			UiManager.Instance.switchWindow<EffectBlackWindow> ((win222) => {
				if (evoType == 1) {
					oldCard = tempCard;
					win222.playEvoEffect(oldCard, food.getCard (), showEvolutionWindow);
				} else {
					oldCard = tempCard;
					win222.playEvoEffect(oldCard, foodProp.getIconId (), showEvolutionWindow);
				}
			});
		});
	}

	/// <summary>
	/// 动画结束，展示窗口
	/// </summary>
	private void showEvolutionWindow ()
	{
		foodProp = null;
		IntensifyCardManager.Instance.clearData ();
		if (EvolutionManagerment.Instance.isCanEvo (newCard) == false){
			//IntensifyCardManager.Instance.intoIntensify (IntensifyCardManager.INTENSIFY_CARD_EVOLUTION, win.getCallBack ());
			IntensifyCardManager.Instance.updateEvolution(IntensifyCardManager.INTENSIFY_CARD_EVOLUTION, win.getCallBack ());
		}
		else {
			IntensifyCardManager.Instance.updateEvolution(IntensifyCardManager.INTENSIFY_CARD_EVOLUTION, newCard, win.getCallBack ());
			//IntensifyCardManager.Instance.intoIntensify (IntensifyCardManager.INTENSIFY_CARD_EVOLUTION, newCard, win.getCallBack ());
		}
	}

	/// <summary>
	/// 清除进化数据
	/// </summary>
	private void clearData () {
		IntensifyCardManager.Instance.clearData ();
		mainRole.cleanAll ();
		food.cleanAll ();
		clearLabelMsg ();
	}

	/// <summary>
	/// 清理文本数据
	/// </summary>
	private void clearLabelMsg () {
		isMore = "";
		oldMaxLevelLabel.text = "";
		newMaxLevelLabel.text = "";
		oldEvoLevelLabel.text = "";
		newEvoLevelLabel.text = "";
		oldCombatLabel.text = "";
		newCombatLabel.text = "";
		talentLabel.text = "";
		talentLabel.gameObject.SetActive (false);
		evoTitel.SetActive(false);
		oldMaxLevelLabel.transform.parent.gameObject.SetActive (true);
		lvTitel.SetActive(true);

	}

    private void clearRoleAndFood() {
        buttonMain.gameObject.SetActive(true);
        mainRole.cleanData();
        buttonFood.gameObject.SetActive(true);
        food.cleanAll();
        tempEvoCard.gameObject.SetActive(false);
    }

	/// <summary>
	/// 设置文本信息
	/// </summary>
	private void setLabelMsg (Card _oldMainCard, Card _newMainCard) {
		if (_oldMainCard != null) {
			oldMaxLevelLabel.text = "" + _oldMainCard.getMaxLevel ();
			oldCombatLabel.text = "" + _oldMainCard.getCardCombat ();
			oldEvoLevelLabel.text = "" + _oldMainCard.getEvoLevel () + " ( " + _oldMainCard.getSuperEvoProgress () + " )";
			if (_newMainCard != null) {

					oldMaxLevelLabel.transform.parent.gameObject.SetActive (true);
					oldEvoLevelLabel.transform.parent.gameObject.SetActive (false);
					evoTitel.SetActive(false);
					lvTitel.SetActive(true);
				newEvoLevelLabel.text = "" + _newMainCard.getEvoLevel () + " ( " + _newMainCard.getSuperEvoProgress () + " )";
				newMaxLevelLabel.text = _oldMainCard.getMaxLevel () + " + [3A9663]" + (_newMainCard.getMaxLevel () - _oldMainCard.getMaxLevel ());
				newCombatLabel.text = _oldMainCard.getCardCombat () + " + [3A9663]" + (_newMainCard.getCardCombat () - _oldMainCard.getCardCombat ());
			}
			string talentStr = EvolutionManagerment.Instance.getOpenTalentString (_newMainCard,_oldMainCard.getEvoLevel());
			//有天赋激活的时候显示
			if (EvolutionManagerment.Instance.isOpenTalentByThisEvoLv (_newMainCard,_oldMainCard.getEvoLevel())) {
				talentLabel.text = LanguageConfigManager.Instance.getLanguage ("Evo17",":\n",talentStr);
				talentLabel.gameObject.SetActive (true);
			} 
//			else if (!string.IsNullOrEmpty (talentStr)) {
//				talentLabel.text = LanguageConfigManager.Instance.getLanguage ("Evo16",instance.getTalentNeedTime (_newMainCard).ToString (),"\n",talentStr);
//				talentLabel.gameObject.SetActive (true);
//			}
		}
	}

	/// <summary>
	/// 获得提示
	/// </summary>
	public string getErrorMsg () {
		return isMore;
	}

	/// <summary>
	/// 计算进化数据,得到新的卡片
	/// </summary>
	private Card getTempEvoCard (Card _oldMainCard, Card _foodCard) {
		//主卡都没有，就肯定是有问题了
		if (_oldMainCard == null) {
			return null;
		}

		//假设是万能卡或者没有副卡的情况下，默认进化等级加1
		Card tempNewCard = _oldMainCard.Clone () as Card;
		tempNewCard.uid = "-1";

		if (_foodCard == null) {
			if ((tempNewCard.getEvoTimes () + 1) > EvolutionManagerment.Instance.getMaxLevel (tempNewCard)) {
				isMore += LanguageConfigManager.Instance.getLanguage ("inherit_err2");
			}
			tempNewCard.updateEvoLevel (tempNewCard.getEvoTimes () + 1);
			return tempNewCard;
		}
        if (_foodCard != null) {
            if (CardSampleManager.Instance.checkBlood(_foodCard.sid, _foodCard.uid) && _foodCard.cardBloodLevel > 0) {
                isMore += "\n" + LanguageConfigManager.Instance.getLanguage("bloodDesc2");
            }
        }

		//有副卡的话就计算副卡的属性

		inheritEvoLv = _foodCard.getEvoTimes () + 1;//因为是进化，吃了对方还会涨1级进化等级
		inheritExp = _foodCard.getEXP ();
		inheritSkillExp = _foodCard.getSkillsExp ();
		inheritAddonExp [0] = _foodCard.getHPExp ();
		inheritAddonExp [1] = _foodCard.getATTExp ();
		inheritAddonExp [2] = _foodCard.getDEFExp ();
		inheritAddonExp [3] = _foodCard.getMAGICExp ();
		inheritAddonExp [4] = _foodCard.getAGILEExp ();
		
        /** 增加副卡经验 */
        tempNewCard.updateHPExp(tempNewCard.getHPExp() + inheritAddonExp[0]);
        tempNewCard.updateATTExp(tempNewCard.getATTExp() + inheritAddonExp[1]);
        tempNewCard.updateDEFExp(tempNewCard.getDEFExp() + inheritAddonExp[2]);
        tempNewCard.updateMAGICExp(tempNewCard.getMAGICExp() + inheritAddonExp[3]);
        tempNewCard.updateAGILEExp(tempNewCard.getAGILEExp() + inheritAddonExp[4]);

		//进化等级-卡片经验-技能经验-附加经验
		if ((tempNewCard.getEvoTimes () + inheritEvoLv) > EvolutionManagerment.Instance.getMaxLevel (tempNewCard)) {
			isMore += LanguageConfigManager.Instance.getLanguage ("inherit_err2");
		}
		tempNewCard.updateEvoLevel (tempNewCard.getEvoTimes () + inheritEvoLv);
		
		long tmpExp = tempNewCard.checkExp (inheritExp);
		if (tmpExp != -1) {
			if (isMore != "") {
				isMore += "\n" + LanguageConfigManager.Instance.getLanguage ("inherit_err0");
			}
			else {
				isMore += LanguageConfigManager.Instance.getLanguage ("inherit_err0");
			}
			tempNewCard.updateExp (tmpExp);
		}
		else
			tempNewCard.updateExp (tempNewCard.getEXP () + inheritExp);
		
		if (tempNewCard.isSkillExpUpFull ((int)inheritSkillExp)) {
			if (isMore != "") {
				isMore += "\n" + LanguageConfigManager.Instance.getLanguage ("inherit_err1");
			}
			else {
				isMore += LanguageConfigManager.Instance.getLanguage ("inherit_err1");
			}
		}
		
		if (tempNewCard.getSkills () != null) {
			tempNewCard.getSkills () [0].updateExp (tempNewCard.getSkills () [0].getEXP () + inheritSkillExp);
		}
		if (tempNewCard.getBuffSkills () != null) {
			tempNewCard.getBuffSkills () [0].updateExp (tempNewCard.getBuffSkills () [0].getEXP () + inheritSkillExp);
		}
		if (tempNewCard.getAttrSkills () != null) {
			tempNewCard.getAttrSkills () [0].updateExp (tempNewCard.getAttrSkills () [0].getEXP () + inheritSkillExp);
		}
		return tempNewCard;
	}
}
