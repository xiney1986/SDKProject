using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;

public class IntensifyCardWindow : WindowBase
{
	public TapContentBase tapContent;
	public IntensifyCardSacrificeContent sacrificeContent;//献祭容器
	public IntensifyCardLSEContent learnSkillAndEvoContent;//学习技能和进化容器
	public IntensifyCardAddonContent addonContent;//附加容器
	public IntensifyCardInheritContent inheritContent;//继承容器
    public IntensifyCardSuperEvoContent superEvoContent;//超进化容器
	public UILabel costSum;//总花费
	public UILabel allMoney;//玩家所有游戏币
	public ButtonBase selectFoodButton;//选择副卡按钮
	public ButtonBase oneKeyButton;//一键选择按钮
	public ButtonBase intensifyButton;//强化按钮
	public ButtonBase inheritButton;//继承
	public UILabel readmeShowLabel;//文字说明
	public GameObject moneyGround;//金钱需求组
	public ButtonBase getMoney;//去炼金按钮
	private CallBack closeCAllback;
	private CallBack cardCallBack;
	private int evoType = 1;//进化类型，1普通卡片，2万能卡
	private int contentType = 0;
	private bool isClear = true;

    public override void OnAwake()
    {

        base.OnAwake();
        UiManager.Instance.intensifyCardWindow = this;
        if (UserManager.Instance.self.getUserLevel() < 6)
        {
            tapContent.tapButtonList[1].GetComponent<TapButtonBase>().disableButton(true);
        } else
        {
            tapContent.tapButtonList[1].GetComponent<TapButtonBase>().disableButton(false);
        }
        if (GuideManager.Instance.isOverStep(112004000))
        {
            tapContent.tapButtonList[2].GetComponent<TapButtonBase>().disableButton(false);
        } else
        {
            tapContent.tapButtonList[2].GetComponent<TapButtonBase>().disableButton(true);
        }
        if (UserManager.Instance.self.getUserLevel() < 5)
        {
            tapContent.tapButtonList[3].GetComponent<TapButtonBase>().disableButton(true);
        } else
        {
            tapContent.tapButtonList[3].GetComponent<TapButtonBase>().disableButton(false);
        }
    }



	protected override void DoEnable ()
	{
		base.DoEnable ();
		UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
	}
	protected override void begin ()
	{
		base.begin ();
        /** 当有卡进化到+10以上时,显示超进化按钮 */
        if (IntensifyCardManager.Instance.isShowSuperEvoButton())
        {
            tapContent.transform.localPosition = new Vector3(-60f, 85f, 0);
            tapContent.tapButtonList[4].gameObject.SetActive(true);
        } else {
            tapContent.transform.localPosition = new Vector3(0f, 85f, 0);
            tapContent.tapButtonList[4].gameObject.SetActive(false);
        }
		if (isAwakeformHide) {
			updateContentAndButton (contentType);
		}    
		GuideManager.Instance.guideEvent ();
		GuideManager.Instance.doFriendlyGuideEvent ();
		MaskWindow.UnlockUI ();
	}

	public void setCallBack (CallBack _cb)
	{
		closeCAllback = _cb;
	}

	public CallBack getCallBack ()
	{
		 
		return	closeCAllback;
	}

	public void initWindow (int contentType) {   
        /** 当有卡进化到+10以上时,显示超进化按钮 */
        if (IntensifyCardManager.Instance.isShowSuperEvoButton()) {
            tapContent.transform.localPosition = new Vector3(-60f, 85f, 0);
            tapContent.tapButtonList[4].gameObject.SetActive(true);
        } else {
            tapContent.transform.localPosition = new Vector3(0f, 85f, 0);
            tapContent.tapButtonList[4].gameObject.SetActive(false);
        }

		if (contentType == IntensifyCardManager.INTENSIFY_CARD_SACRIFICE) {
			tapContent.changeTapPage (tapContent.tapButtonList [0], false);
		} else if (contentType == IntensifyCardManager.INTENSIFY_CARD_EVOLUTION) {
			tapContent.changeTapPage (tapContent.tapButtonList [1], false);
		} else if (contentType == IntensifyCardManager.INTENSIFY_CARD_ADDON) {
			tapContent.changeTapPage (tapContent.tapButtonList [2], false);
		} else if (contentType == IntensifyCardManager.INTENSIFY_CARD_INHERIT) {
			tapContent.changeTapPage (tapContent.tapButtonList [3], false);
        } else if (contentType == IntensifyCardManager.INTENSIFY_CARD_SUPRE_EVO) {
            tapContent.changeTapPage(tapContent.tapButtonList[4], false);
        }
		updateContentAndButton (contentType);
		MaskWindow.UnlockUI();
	}
	/// <summary>
	/// 界面隐藏出来做的事儿
	/// </summary>
	/// <param name="type">Type.</param>
	public void updateEvolution(int type){
		if (contentType == IntensifyCardManager.INTENSIFY_CARD_SACRIFICE) {
			tapContent.changeTapPage (tapContent.tapButtonList [0], false);
		} else if (contentType == IntensifyCardManager.INTENSIFY_CARD_EVOLUTION) {
			tapContent.changeTapPage (tapContent.tapButtonList [1], false);
		} else if (contentType == IntensifyCardManager.INTENSIFY_CARD_ADDON) {
			tapContent.changeTapPage (tapContent.tapButtonList [2], false);
		} else if (contentType == IntensifyCardManager.INTENSIFY_CARD_INHERIT) {
			tapContent.changeTapPage (tapContent.tapButtonList [3], false);
        } else if (contentType == IntensifyCardManager.INTENSIFY_CARD_SUPRE_EVO) {
            tapContent.changeTapPage(tapContent.tapButtonList[4], false);
        }
		this.contentType=type;
	}

	public int getIntensifyType ()
	{
		return contentType;
	}
	//初始化容器
	private void updateContentAndButton (int contentType)
	{
		this.contentType = contentType;
		intensifyButton.gameObject.SetActive(true);
		if(contentType==IntensifyCardManager.INTENSIFY_CARD_EVOLUTION)intensifyButton.gameObject.SetActive(false);
		sacrificeContent.gameObject.SetActive (false);
		learnSkillAndEvoContent.gameObject.SetActive (false);
		addonContent.gameObject.SetActive (false);
		inheritContent.gameObject.SetActive (false);
        superEvoContent.gameObject.SetActive(false);
		inheritButton.gameObject.SetActive (false);
		initButton (false);
		if (contentType == IntensifyCardManager.INTENSIFY_CARD_SACRIFICE) {
			//readmeShowLabel.text = LanguageConfigManager.Instance.getLanguage ("Intensify_temp1", "\n");
			setTitle (LanguageConfigManager.Instance.getLanguage ("Intensify1"));
			sacrificeContent.initInfo (this);
			sacrificeContent.updateCtrl ();
			sacrificeContent.gameObject.SetActive (true);
			sacrificeContent.sacrificeRotCtrl.flashingBugFix ();
			sacrificeContent.updateExp ();
			sacrificeContent.sacrificeRotCtrl.changeDepth (true);
			addonContent.sacrificeRotCtrl.cleanCastShower ();
			intensifyButton.textLabel.text=LanguageConfigManager.Instance.getLanguage("Intensify1");
			intensifyButton.transform.localPosition = new Vector3 (158f, intensifyButton.transform.localPosition.y, intensifyButton.transform.localPosition.z);
			//设置金币的显示位置
			moneyGround.transform.FindChild("allMoney").gameObject.transform.localPosition = new Vector3(-167,-40f,0f);
			moneyGround.transform.FindChild("costMoney").gameObject.transform.localPosition = new Vector3(-167f,-80f,0f);
			initButton (true);

		} else if (contentType == IntensifyCardManager.INTENSIFY_CARD_LEARNSKILL || contentType == IntensifyCardManager.INTENSIFY_CARD_EVOLUTION) {
			//readmeShowLabel.text = LanguageConfigManager.Instance.getLanguage ("Intensify_temp2", "\n");
			if (contentType == IntensifyCardManager.INTENSIFY_CARD_LEARNSKILL) {
				intensifyButton.transform.localPosition = new Vector3 (11.8f, intensifyButton.transform.localPosition.y, intensifyButton.transform.localPosition.z);
				setTitle (LanguageConfigManager.Instance.getLanguage ("Intensify2"));
			} else if (contentType == IntensifyCardManager.INTENSIFY_CARD_EVOLUTION) {
				setTitle (LanguageConfigManager.Instance.getLanguage ("Intensify3"));
				sacrificeContent.sacrificeRotCtrl.cleanCastShower ();
				addonContent.sacrificeRotCtrl.cleanCastShower ();
				intensifyButton.textLabel.text=LanguageConfigManager.Instance.getLanguage("Intensify3");
				intensifyButton.transform.localPosition = new Vector3 (0, intensifyButton.transform.localPosition.y, intensifyButton.transform.localPosition.z);
				//设置金币的显示位置
				moneyGround.transform.FindChild("allMoney").gameObject.transform.localPosition = new Vector3(-167,-40f,0f);
				moneyGround.transform.FindChild("costMoney").gameObject.transform.localPosition = new Vector3(-167f,-80f,0f);
			}
			learnSkillAndEvoContent.updateCtrl (this,true);
			if(contentType==IntensifyCardManager.INTENSIFY_CARD_EVOLUTION){
				intensifyButton.gameObject.SetActive (true);
			}
			learnSkillAndEvoContent.gameObject.SetActive (true);
		} else if (contentType == IntensifyCardManager.INTENSIFY_CARD_ADDON) {
			//readmeShowLabel.text = LanguageConfigManager.Instance.getLanguage ("Intensify_temp3");
			setTitle (LanguageConfigManager.Instance.getLanguage ("Intensify4"));
			addonContent.initInfo (this);
			addonContent.updateCtrl ();
			addonContent.gameObject.SetActive (true);
			addonContent.sacrificeRotCtrl.flashingBugFix ();
			addonContent.rushCombat ();
			addonContent.recalculateAddonNumber ();
			addonContent.sacrificeRotCtrl.changeDepth (true);
			sacrificeContent.sacrificeRotCtrl.cleanCastShower ();
			intensifyButton.textLabel.text=LanguageConfigManager.Instance.getLanguage("s0141");
			intensifyButton.transform.localPosition = new Vector3 (0, intensifyButton.transform.localPosition.y, intensifyButton.transform.localPosition.z);
			//设置金币的显示位置
			moneyGround.transform.FindChild("allMoney").gameObject.transform.localPosition = new Vector3(-174f,-80f,0f);
			moneyGround.transform.FindChild("costMoney").gameObject.transform.localPosition = new Vector3(95f,-80f,0f);
			initButton (true);
		} else if (contentType == IntensifyCardManager.INTENSIFY_CARD_INHERIT) {
			//readmeShowLabel.text = LanguageConfigManager.Instance.getLanguage ("inherit_06", "\n", "\n");
			setTitle (LanguageConfigManager.Instance.getLanguage ("inherit_00"));
			sacrificeContent.sacrificeRotCtrl.cleanCastShower ();
			addonContent.sacrificeRotCtrl.cleanCastShower ();
			inheritContent.initInfo (this);
			inheritContent.gameObject.SetActive (true);
			intensifyButton.gameObject.SetActive (false);
			inheritButton.gameObject.SetActive (true);

		}
        else if (contentType == IntensifyCardManager.INTENSIFY_CARD_SUPRE_EVO) {
            setTitle(LanguageConfigManager.Instance.getLanguage("Intensify26"));
            intensifyButton.transform.localPosition = new Vector3(0, intensifyButton.transform.localPosition.y, intensifyButton.transform.localPosition.z);
            intensifyButton.gameObject.SetActive(true);
            intensifyButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("Intensify26");
            superEvoContent.gameObject.SetActive(true);
            //设置金币的显示位置
            moneyGround.transform.FindChild("allMoney").gameObject.transform.localPosition = new Vector3(-167, -40f, 0f);
            moneyGround.transform.FindChild("costMoney").gameObject.transform.localPosition = new Vector3(-167f, -80f, 0f);
            superEvoContent.updateCtrl(this, true);
        }
		getMoney.gameObject.SetActive(false);
		updateLabel ();
		updateButton ();
	}

	public void updateInfo ()
	{
		if (contentType == IntensifyCardManager.INTENSIFY_CARD_SACRIFICE) {
			sacrificeContent.clickUpdateCtrl ();
			sacrificeContent.updateExp ();
		} else if (contentType == IntensifyCardManager.INTENSIFY_CARD_ADDON) {
			addonContent.clickUpdateCtrl ();
			addonContent.rushCombat ();
			addonContent.recalculateAddonNumber ();
			addonContent.updateButtonSprites();
		} else if (contentType == IntensifyCardManager.INTENSIFY_CARD_LEARNSKILL || contentType == IntensifyCardManager.INTENSIFY_CARD_EVOLUTION) {
			learnSkillAndEvoContent.updateCtrl (this,false);
		} else if (contentType == IntensifyCardManager.INTENSIFY_CARD_INHERIT) {
			inheritContent.initInfo (this);
        } else if (contentType == IntensifyCardManager.INTENSIFY_CARD_SUPRE_EVO) {
            superEvoContent.updateCtrl(this,false);
        }
		getMoney.gameObject.SetActive(false);
		updateLabel ();
		updateButton ();
	}
	//初始化按钮
	private void initButton (bool isShow)
	{
		if(contentType==IntensifyCardManager.INTENSIFY_CARD_ADDON)
			isShow = false;
		selectFoodButton.gameObject.SetActive (isShow);
		if (GuideManager.Instance.isLessThanStep (50001000)) {
			oneKeyButton.gameObject.SetActive (false);
		} else {
			oneKeyButton.gameObject.SetActive (isShow);
		}
	}

	//更新界面文本信息
	private void updateLabel ()
	{
        if (contentType != IntensifyCardManager.INTENSIFY_CARD_EVOLUTION && contentType != IntensifyCardManager.INTENSIFY_CARD_SUPRE_EVO)
        {
			//readmeShowLabel.text = "";
			if (contentType == IntensifyCardManager.INTENSIFY_CARD_ADDON) {
				if (StringKit.toInt (IntensifyCardManager.Instance.getAddonCost ()) > UserManager.Instance.self.getMoney ()) {
					costSum.text = "[FF0000]" + IntensifyCardManager.Instance.getAddonCost ();
				} else {
					costSum.text = IntensifyCardManager.Instance.getAddonCost ();
				}
			} else {
				if (StringKit.toInt (IntensifyCardManager.Instance.getCost ()) > UserManager.Instance.self.getMoney ()) {
					costSum.text = "[FF0000]" + IntensifyCardManager.Instance.getCost ();
					getMoney.gameObject.SetActive(true);
					if(contentType == IntensifyCardManager.INTENSIFY_CARD_SACRIFICE)
					{
						getMoney.transform.localPosition = new Vector3(getMoney.transform.localPosition.x, -280, 0);
					}
                    if (selectFoodButton.gameObject.activeInHierarchy) selectFoodButton.gameObject.SetActive(false);
					//readmeShowLabel.text=LanguageConfigManager.Instance.getLanguage("intensifyCardWindow_buttonSelect3");
				} else {
                    if (contentType == IntensifyCardManager.INTENSIFY_CARD_SACRIFICE) {
                        selectFoodButton.gameObject.SetActive(true);
                    }
					costSum.text = IntensifyCardManager.Instance.getCost ();
				}
			}
		}
        else if (contentType == IntensifyCardManager.INTENSIFY_CARD_SUPRE_EVO) {
            Card mainCard = IntensifyCardManager.Instance.getMainCard();
            if (mainCard != null)
            {
                long qian = EvolutionManagerment.Instance.getCostToNextEvoLevel(mainCard);
                costSum.text = qian > UserManager.Instance.self.getMoney() ? ("[FF0000]" + qian) : qian.ToString();
                if (qian > UserManager.Instance.self.getMoney())
                {
                    getMoney.gameObject.SetActive(true);
                    getMoney.transform.localPosition = new Vector3(getMoney.transform.localPosition.x, -280, 0);
                }
            }
            else {
                costSum.text = "0";
            }
        }
        else if (contentType == IntensifyCardManager.INTENSIFY_CARD_EVOLUTION)
        {
            Card mainCard = IntensifyCardManager.Instance.getMainCard();
            List<Card> foodCards = IntensifyCardManager.Instance.getFoodCard();

            if (mainCard != null && foodCards != null && foodCards.Count > 0)
            {
                long qian = EvolutionManagerment.Instance.getNeedMoneyByLevel(mainCard, foodCards[0].getEvoTimes() + 1, foodCards[0].getEvoLevel());
                costSum.text = qian > UserManager.Instance.self.getMoney() ? ("[FF0000]" + qian) : qian.ToString();
                if (qian > UserManager.Instance.self.getMoney())
                {
                    getMoney.gameObject.SetActive(true);
                    getMoney.transform.localPosition = new Vector3(getMoney.transform.localPosition.x, -280, 0);
                }
            }
            else if (mainCard != null)
            {
                long qian = EvolutionManagerment.Instance.getNeedMoney(mainCard);
                costSum.text = qian > UserManager.Instance.self.getMoney() ? ("[FF0000]" + qian) : qian.ToString();
                if (qian > UserManager.Instance.self.getMoney())
                {
                    getMoney.gameObject.SetActive(true);
                    getMoney.transform.localPosition = new Vector3(getMoney.transform.localPosition.x, -280, 0);
                }
            }
            else
            {
                costSum.text = "0";
            }
        }
		allMoney.text = UserManager.Instance.self.getMoney ().ToString ();
	}

	public override void tapButtonEventBase (GameObject gameObj, bool enable)
	{
		base.tapButtonEventBase (gameObj, enable);
		if (gameObj.name == "sacrificeButton" && enable) {
			if (isClear) {
				IntensifyCardManager.Instance.TapChange (IntensifyCardManager.INTENSIFY_CARD_SACRIFICE);
			}
			isClear = true;
			IntensifyCardManager.Instance.intoIntensify (IntensifyCardManager.INTENSIFY_CARD_SACRIFICE, closeCAllback);

		}
		else if (gameObj.name == "evolutionButton" && enable) {
			GuideManager.Instance.doGuide (); 
			GuideManager.Instance.guideEvent ();
			if (isClear) {
				IntensifyCardManager.Instance.TapChange (IntensifyCardManager.INTENSIFY_CARD_EVOLUTION);
			}
			isClear = true;
			IntensifyCardManager.Instance.intoIntensify (IntensifyCardManager.INTENSIFY_CARD_EVOLUTION, closeCAllback);
            GuideManager.Instance.doFriendlyGuideEvent();
			
		}
		else if (gameObj.name == "addonButton" && enable) {
			GuideManager.Instance.doGuide ();
			GuideManager.Instance.guideEvent ();
			if (isClear) {
				IntensifyCardManager.Instance.TapChange (IntensifyCardManager.INTENSIFY_CARD_ADDON);
			}
			isClear = true;
			IntensifyCardManager.Instance.intoIntensify (IntensifyCardManager.INTENSIFY_CARD_ADDON, closeCAllback);
		}
		else if (gameObj.name == "inheritTapButton" && enable) {
			if (isClear) {
				IntensifyCardManager.Instance.TapChange (IntensifyCardManager.INTENSIFY_CARD_INHERIT);
			}
			isClear = true;
			IntensifyCardManager.Instance.intoIntensify (IntensifyCardManager.INTENSIFY_CARD_INHERIT, closeCAllback);
		}
        else if (gameObj.name == "superEvoButton" && enable)
        {
            if (isClear) {
                IntensifyCardManager.Instance.TapChange(IntensifyCardManager.INTENSIFY_CARD_SUPRE_EVO);
            }
            isClear = true;
            IntensifyCardManager.Instance.intoIntensify(IntensifyCardManager.INTENSIFY_CARD_SUPRE_EVO, closeCAllback);
            
        }
		MaskWindow.UnlockUI ();
	}

	private void updateButton ()
	{
		//非传承才需要显示金钱
		if (contentType == IntensifyCardManager.INTENSIFY_CARD_INHERIT) {
			moneyGround.SetActive (false);
            if (inheritContent.newRole.card != null) {//传承显示炼金按钮判断
                int needMoney = 50 * (int)Mathf.Pow(inheritContent.newRole.card.getLevel(), 2);
                if (needMoney >= UserManager.Instance.self.getMoney())
				{
					getMoney.gameObject.SetActive(true);
					getMoney.transform.localPosition = new Vector3(getMoney.transform.localPosition.x,-290,getMoney.transform.localPosition.z);
				}
                else
                    getMoney.gameObject.SetActive(false);
            }
		} else {
			moneyGround.SetActive (true);
		}

		if (contentType == IntensifyCardManager.INTENSIFY_CARD_SACRIFICE) {
			selectFoodButton.disableButton (false);
			oneKeyButton.disableButton (false);
			if (!IntensifyCardManager.Instance.isShowIntesifyBtnBySkillLevelUp ()) {
				intensifyButton.disableButton (true);
			} else {
				intensifyButton.disableButton (false);
			}
		} else if (contentType == IntensifyCardManager.INTENSIFY_CARD_ADDON) {
			selectFoodButton.disableButton (false);
			oneKeyButton.disableButton (false);
			if (!IntensifyCardManager.Instance.isShowIntesifyBtnByAddon ()) {
				intensifyButton.disableButton (true);
			} else {
				intensifyButton.disableButton (false);
			}

		} else if (contentType == IntensifyCardManager.INTENSIFY_CARD_EVOLUTION) {
			if (!IntensifyCardManager.Instance.isShowIntesifyBtnByEvo ()) {
				intensifyButton.disableButton (true);
			} else {
				intensifyButton.disableButton (false);
			}
		}
        else if (contentType == IntensifyCardManager.INTENSIFY_CARD_SUPRE_EVO) {
            if (!IntensifyCardManager.Instance.isShowIntesifyBtnSuperEvo())
            {
                intensifyButton.disableButton(true);
            }
            else
            {
                intensifyButton.disableButton(false);
            }
        }

		if (IntensifyCardManager.Instance.isFoodFull ()) {
			oneKeyButton.disableButton (true);
		} else {
			oneKeyButton.disableButton (false);
		}
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "buttonSelect" || gameObj.name.Contains("buttonFood")) {
			GuideManager.Instance.doGuide ();
			//是进化就走特殊渠道选择副卡
            if (contentType == IntensifyCardManager.INTENSIFY_CARD_EVOLUTION)
            {
				if (IntensifyCardManager.Instance.getMainCard () == null) {
					UiManager.Instance.createMessageWindowByOneButton (LanguageConfigManager.Instance.getLanguage ("Evo05"), null);
					return;
				}
				UiManager.Instance.openDialogWindow<EvolutionChooseWindow> ((win) => {
					win.initWin (openChooswWinCallBack, setEvoType);
				});
			}
            else  if (contentType == IntensifyCardManager.INTENSIFY_CARD_SUPRE_EVO) {
                IntensifyCardManager.Instance.resetSuperExoChoosedNum();
                if (IntensifyCardManager.Instance.getMainCard() == null)
                {
                    UiManager.Instance.createMessageWindowByOneButton(LanguageConfigManager.Instance.getLanguage("Evo05"), null);
                    return;
                }
                UiManager.Instance.openDialogWindow<EvolutionChooseWindow>((win) =>
                {
                    win.initWin(openChooswWinCallBack, setEvoType);
                });
            } else
            {
                UiManager.Instance.openWindow<IntensifyCardChooseWindow>((win) =>
                {
                    win.initWindow(contentType, IntensifyCardManager.FOODCARDSELECT);
                });
            }
		} else if (gameObj.name == "buttonMain") {
			GuideManager.Instance.doGuide ();
            IntensifyCardManager.Instance.setMainCard(null);
			UiManager.Instance.openWindow<IntensifyCardChooseWindow> ((win) => {
				win.initWindow (contentType, IntensifyCardManager.MAINCARDSELECT);
			});
		} else if (gameObj.name == "buttonOneKey") {
			if (contentType == IntensifyCardManager.INTENSIFY_CARD_SACRIFICE) {
				sacrificeContent.oneKey ();
			} else if (contentType == IntensifyCardManager.INTENSIFY_CARD_ADDON) {
				addonContent.oneKey ();
			}
			MaskWindow.UnlockUI ();
		} else if (gameObj.name == "buttonIntensify") {
			if (IntensifyCardManager.Instance.getMainCard () == null) {
				UiManager.Instance.createMessageWindowByOneButton (LanguageConfigManager.Instance.getLanguage ("Evo05"), null);
				MaskWindow.UnlockUI ();
				return;
			}
			//献祭开始
			if (contentType == IntensifyCardManager.INTENSIFY_CARD_SACRIFICE) {
				Card mainCard = IntensifyCardManager.Instance.getMainCard ();
				if (mainCard != null) {
					IntensifyCardSacrificeContent icsc = sacrificeContent.GetComponent<IntensifyCardSacrificeContent> ();
					if (!mainCard.isCanSacrific ()) {
						doSacrifice (mainCard);
					} else {
						UiManager.Instance.openDialogWindow<MessageLineWindow> ((win) => {
							win.Initialize (LanguageConfigManager.Instance.getLanguage ("Intensify_t04l1"));
						});
						MaskWindow.UnlockUI ();
						return;
					}
				}
            } else if (contentType == IntensifyCardManager.INTENSIFY_CARD_SUPRE_EVO) {
                Card mc = IntensifyCardManager.Instance.getMainCard();//主卡

                if (!EvolutionManagerment.Instance.isCanEvoByString(mc))
                {
                    MaskWindow.UnlockUI();
                    return;
                }
                if (!string.IsNullOrEmpty(superEvoContent.getErrorMsg()))
                {
                    UiManager.Instance.createMessageWindowByTwoButton(superEvoContent.getErrorMsg(), (MessageHandle msg) =>
                    {
                        if (msg.buttonID == MessageHandle.BUTTON_LEFT)
                        {
                            MaskWindow.UnlockUI();
                            return;
                        }
                        intensifyButton.gameObject.SetActive(false);
                        superEvoContent.intensify();
                    });
                } else
                {
                    intensifyButton.gameObject.SetActive(false);
                    superEvoContent.intensify();
                }
                if (UiManager.Instance.cardBookWindow != null)
                {
                    UiManager.Instance.cardBookWindow.destoryWindow();
                }
            }
                //进化
            else if (contentType == IntensifyCardManager.INTENSIFY_CARD_EVOLUTION)
            {
                Card mc = IntensifyCardManager.Instance.getMainCard();//主卡

                if (!EvolutionManagerment.Instance.isCanEvoByString(mc))
                {
                    MaskWindow.UnlockUI();
                    return;
                }
                if (!string.IsNullOrEmpty(learnSkillAndEvoContent.getErrorMsg()))
                {
                    UiManager.Instance.createMessageWindowByTwoButton(learnSkillAndEvoContent.getErrorMsg(), (MessageHandle msg) =>
                    {
                        if (msg.buttonID == MessageHandle.BUTTON_LEFT)
                        {
                            MaskWindow.UnlockUI();
                            return;
                        }
                        intensifyButton.gameObject.SetActive(false);
                        learnSkillAndEvoContent.intensify(contentType, costSum.text, evoType);
                    });
                } else
                {
                    intensifyButton.gameObject.SetActive(false);
                    learnSkillAndEvoContent.intensify(contentType, costSum.text, evoType);
                }
                if (UiManager.Instance.cardBookWindow != null)
                {
                    UiManager.Instance.cardBookWindow.destoryWindow();
                }
            } else if (contentType == IntensifyCardManager.INTENSIFY_CARD_ADDON)
            {
                addonContent.intensify();
            }
		}else if(gameObj.name=="goGetMoney"){
			UiManager.Instance.openWindow<NoticeWindow> ((win) => {
				win.entranceId = NoticeSampleManager.Instance.getNoticeSampleBySid(NoticeType.ALCHEMY_SID).entranceId;
				win.updateSelectButton (NoticeType.ALCHEMY_SID);
			});
		}else if(gameObj.name=="buttonHelp"){
			UiManager.Instance.openDialogWindow<GeneralDesWindow> ((win) => {
				//Intensify1|献祭
				//Intensify3|进化
				//Intensify4|附加
				//inherit_00|传承
                //Intensify26|超进化
				if(getTitle() == LanguageConfigManager.Instance.getLanguage("Intensify1")){
					win.initialize(LanguageConfigManager.Instance.getLanguage("Intensify_des1"),LanguageConfigManager.Instance.getLanguage("Intensify25"),LanguageConfigManager.Instance.getLanguage("Intensify_t01"));
				}
				else if(getTitle() == LanguageConfigManager.Instance.getLanguage("inherit_00")){
					win.initialize(LanguageConfigManager.Instance.getLanguage("Intensify_des4"),LanguageConfigManager.Instance.getLanguage("Intensify25"),LanguageConfigManager.Instance.getLanguage("Intensify_t04"));
				}
				else if(getTitle() == LanguageConfigManager.Instance.getLanguage("Intensify3")){
					win.initialize(LanguageConfigManager.Instance.getLanguage("Intensify_des2"),LanguageConfigManager.Instance.getLanguage("Intensify25"),LanguageConfigManager.Instance.getLanguage("Intensify_t02"));
				}
				else if(getTitle() == LanguageConfigManager.Instance.getLanguage("Intensify4")){
					win.initialize(LanguageConfigManager.Instance.getLanguage("Intensify_des3"),LanguageConfigManager.Instance.getLanguage("Intensify25"),LanguageConfigManager.Instance.getLanguage("Intensify_t03"));
				}
                else if (getTitle() == LanguageConfigManager.Instance.getLanguage("Intensify26")) {
                    win.initialize(LanguageConfigManager.Instance.getLanguage("Intensify_des3"), LanguageConfigManager.Instance.getLanguage("Intensify25"), LanguageConfigManager.Instance.getLanguage("Intensify_t03"));
                }
			});
		}
		else if (gameObj.name == "close") {
			IntensifyCardManager.Instance.clearData ();
			if (!GuideManager.Instance.isGuideComplete ()) { 
				UiManager.Instance.openMainWindow ();
				GuideManager.Instance.doGuide ();
				IntensifyCardManager.Instance.isFromIncrease = false;
				MaskWindow.UnlockUI ();
				return;
			}
			if (IntensifyCardManager.Instance.isFromIncrease) {
				UiManager.Instance.openMainWindow ();
				IntensifyCardManager.Instance.isFromIncrease = false;
			} else {
				WindowBase win;
				win = UiManager.Instance.getWindow<CardStoreWindow>();
				if(win!=null){
					UiManager.Instance.BackToWindow<CardStoreWindow>();
				}
				else {
					win = UiManager.Instance.getWindow<TeamEditWindow>();
					if(win!=null){
						UiManager.Instance.BackToWindow<TeamEditWindow>();
						this.dialogCloseUnlockUI=false;
					}
					else{
						finishWindow ();
					}
				}
			}
		}
		//传承
		if (contentType == IntensifyCardManager.INTENSIFY_CARD_INHERIT) {
			inheritContent.doClieckEvent (gameObj);
		}

	}

	/** 执行卡牌强化 */
	private void doSacrifice (Card mainCard)
	{
		GuideManager.Instance.saveTimes (GuideManager.TypeSacrifice);
		ArrayList messageList = new ArrayList ();
		IntensifyCardSacrificeContent icsc = sacrificeContent.GetComponent<IntensifyCardSacrificeContent> ();

		if (icsc.sacrificeRotCtrl.isSeniorQualityByFoods ()) {
			messageList.Add (LanguageConfigManager.Instance.getLanguage ("Intensify23"));
		}
		if (mainCard.isSkillExpUpFull (icsc.sacrificeRotCtrl.recalculateSkillEXP ())) {
			messageList.Add (LanguageConfigManager.Instance.getLanguage ("Intensify22"));
		}
		if(!EvolutionManagerment.Instance.isMaxEvoLevel(mainCard) && mainCard.getQualityId() > QualityType.EXCELLENT && icsc.sacrificeRotCtrl.isNamesake(mainCard.sid)){
			messageList.Add (LanguageConfigManager.Instance.getLanguage ("Intensify24"));
		}
        if (icsc.sacrificeRotCtrl.isHasBloodLvByFoods()) {
            messageList.Add(LanguageConfigManager.Instance.getLanguage("Intensify31"));
        }
		string messageInfo = MessageWindow.createMessageInfo (messageList);
		if (!string.IsNullOrEmpty (messageInfo)) {
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.content.pivot = UIWidget.Pivot.Left;
				//win.content.transform.localPosition = new Vector3 (-190, 136, 0);
				//win.content.transform.localScale = new Vector3 (1.1f, 1.1f, 1);
				win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0093"), messageInfo, (msg) => {
					if (msg.buttonID == MessageHandle.BUTTON_LEFT) {
						MaskWindow.UnlockUI ();
						return;
					} else {
						sacrificeContent.intensify ();
					}
				});
			});
		} else {
			sacrificeContent.intensify ();
		}
	}


	//打开进化副卡选择窗口
	private void openChooswWinCallBack ()
	{
		UiManager.Instance.openWindow<IntensifyCardChooseWindow> ((win) => {
			win.initWindow (contentType, IntensifyCardManager.FOODCARDSELECT);
		});
	}

	public void setEvoType (int _type)
	{
		evoType = _type;
        updateButton();
	}

	public override void OnNetResume ()
	{
		base.OnNetResume ();
		IntensifyCardManager.Instance.clearFoodCard ();
		if (contentType == IntensifyCardManager.INTENSIFY_CARD_SACRIFICE) {
			sacrificeContent.sacrificeRotCtrl.cleanCastShower ();
            Card card = StorageManagerment.Instance.getRole(IntensifyCardManager.Instance.getMainCard().uid);
            IntensifyCardManager.Instance.setMainCard(card);
		} else if (contentType == IntensifyCardManager.INTENSIFY_CARD_LEARNSKILL || contentType == IntensifyCardManager.INTENSIFY_CARD_EVOLUTION) {
			if (learnSkillAndEvoContent.food != null)
				learnSkillAndEvoContent.food.cleanAll ();
		} else if (contentType == IntensifyCardManager.INTENSIFY_CARD_ADDON) {
			addonContent.sacrificeRotCtrl.cleanCastShower ();
		}
		initWindow (contentType);
	}
}
