using UnityEngine;
using System.Collections;

/**
 * 出售窗口（英雄卡片和装备）
 * @author 汤琦
 * */
public class SellWindow : WindowBase
{
	public UISprite indicate;//指示
	public SellCardContent cardContent;
	public SellEquipContent equipContent;
	public GameObject cardButtonPrefab;
	public GameObject equipButtonPrefab;
	public UILabel costMoney;//花费金额文本
	public TapContentBase tapBase;//分页按钮
	public ButtonBase buttonSell;
	private Color NROMAL = new Color (1f, 0.98f, 0.7f, 1);
	private Color LINE = new Color (0.3f, 0.25f, 0.14f, 1);
	private ArrayList allList ;
	private ArrayList showList ;
	private ArrayList selectList = new ArrayList ();
	private bool isShowMessageType = false;
	private const int currentEquipContentMNum = 25;//装备页面一页中的最多元素个数
	private const int currentCardContentMNum = 9;//卡片页面一页中的最多元素个数
	private int tapIndex = 0;//分页按钮索引
	private ArrayList cardofHaveStarSoul;

	public int qualityType = QualityType.EXCELLENT;
	
	public bool isSelect (object obj)
	{
		for (int i = 0; i < selectList.Count; i++) {
			if (selectList [i] == obj) {
				return true;
			}
		}
		return false;
	}

	protected override void begin ()
	{
		base.begin ();
		if (isAwakeformHide == false) {
			tapBase.changeTapPage (tapBase.tapButtonList [0]);
		}
		GuideManager.Instance.guideEvent ();
		MaskWindow.UnlockUI ();
	}
	
	//初始化信息
	public void Initialize ()
	{
		costMoney.text = UserManager.Instance.self.getMoney () + Colors.GREEN + "+0";
		buttonSell.disableButton (true);
	}
	//初始化标记显示
	private void indicateEquipShow (ArrayList list)
	{
		if (list.Count > 25) {
			indicate.gameObject.SetActive (true);
		} else {
			indicate.gameObject.SetActive (false);
		}
	}

	private void indicateCardShow (ArrayList list)
	{
		if (list.Count > 9) {
			indicate.gameObject.SetActive (true);
		} else {
			indicate.gameObject.SetActive (false);
		}
	}
	//计算所有卡片
	public void recalculateAllCard (ArrayList list)
	{
		showList = new ArrayList ();
		
		//生成新的引用list

		foreach (Card each in list) {
			showList.Add (each);
		}
		Card tempCard;
		//剔除保护中、装备中、上阵中的卡片
		for (int i = 0; i < showList.Count; i++) {

			tempCard = showList [i] as Card;


            if ((tempCard.state & CardStateType.STATE_USING) == 1 || (tempCard.state & CardStateType.STATE_MINING) == 4 || 
                ((tempCard.state & (CardStateType.STATE_MINING|CardStateType.STATE_USING)) == 5)) {
				showList.Remove (showList [i]);
				i--;
				continue;
			} else if (tempCard.uid == UserManager.Instance.self.mainCardUid) {
				showList.Remove (showList [i]);
				i--;
				continue;
			} else if (tempCard.getSellPrice () == 0) {
				showList.Remove (showList [i]);
				i--;
				continue;
			}
		}
	}

	public string getErrorString ()
	{
		bool isHave = false;
		bool isAddAttr = false;
		string errorStr = "";
		Card tempCard;
		for (int i = 0; i < selectList.Count; i++) {
			tempCard = selectList [i] as Card;
			if (tempCard.getQualityId () >= 4) {
				errorStr += "\n" + LanguageConfigManager.Instance.getLanguage ("sellErr02");
				isHave = true;
				break;
			}
		}
		for (int i = 0; i < selectList.Count; i++) {
			tempCard = selectList [i] as Card;
			if (tempCard.getLevel () > 1) {
				errorStr += "\n" + LanguageConfigManager.Instance.getLanguage ("sellErr03");
				isHave = true;
				isAddAttr = true;
				break;
			}
		}
		for (int i = 0; i < selectList.Count; i++) {
			tempCard = selectList [i] as Card;
            if (tempCard.getCardType() == 4) {//精灵卡不要提示
                continue;
            }
			if (tempCard.getHPExp () > 0 || tempCard.getATTExp () > 0 || tempCard.getDEFExp () > 0 || tempCard.getMAGICExp () > 0 || tempCard.getAGILEExp () > 0) {
				errorStr += "\n" + LanguageConfigManager.Instance.getLanguage ("sellErr04");
				isHave = true;
				isAddAttr = true;
				break;
			}
		}
		for (int i = 0; i < selectList.Count; i++) {
			tempCard = selectList [i] as Card;
			if (tempCard.getSkillsExp () > 0) {
				errorStr += "\n" + LanguageConfigManager.Instance.getLanguage ("sellErr06");
				isHave = true;
				isAddAttr = true;
				break;
			}
		}
		for (int i = 0; i < selectList.Count; i++) {
			tempCard = selectList [i] as Card;
			if (tempCard.getEvoLevel () > 0) {
				errorStr += "\n" + LanguageConfigManager.Instance.getLanguage ("sellErr05");
				isHave = true;
				isAddAttr = true;
				break;
			}
		}
        for (int i = 0; i < selectList.Count; i++) {
            tempCard = selectList[i] as Card;
            if (CardSampleManager.Instance.checkBlood(tempCard.sid,tempCard.uid) && tempCard.cardBloodLevel >0) {
                errorStr += "\n" + LanguageConfigManager.Instance.getLanguage("sellErr09");
                isHave = true;
                isAddAttr = true;
                break;
            }
        }
		errorStr += "\n" + (isAddAttr ? LanguageConfigManager.Instance.getLanguage ("sellErr07") : LanguageConfigManager.Instance.getLanguage ("sellErr08"));

		return isHave ? errorStr : "";
	}
	
	public void recalculateAllEquip (ArrayList list)
	{
		showList = new ArrayList ();
		
		//生成新的引用list

		foreach (Equip each in list) {
			showList.Add (each);
		}
		//剔除保护中、装备中、上阵中的卡片
		for (int i = 0; i < showList.Count; i++) {
			if (((showList [i] as Equip).state & CardStateType.STATE_USING) == 1) {
				showList.Remove (showList [i]);
				i--;
				continue;
			} else if (((showList [i] as Equip).state & CardStateType.STATE_LOCK) == 2) {
				showList.Remove (showList [i]);
				i--;
				continue;
			}
		}
	}
	//向选择到的装备集中添加装备
	public void onSelectEquip (Equip equip)
	{
		selectList.Add (equip);
		changeButton ();
	}
	//向选择到的装备集中移除装备
	public void offSelectEquip (Equip equip)
	{
		selectList.Remove (equip);
		changeButton ();
	}
	//向选择到的卡片集中添加卡片
	public void onSelectCard (Card card)
	{
		selectList.Add (card);
		changeButton ();
	}
	//向选择到的卡片集中移除卡片
	public void offSelectCard (Card card)
	{
		selectList.Remove (card);
		changeButton ();
	}
	
	public void changeButton ()
	{
		int cast = 0;
		
		foreach (var item in selectList) {
			if (item is Card) {
				cast += (item as Card).getSellPrice ();
			} else if (item is Equip) {
				cast += (item as Equip).getSellPrice ();
			}
		}
		
		if (cast == 0) {
			buttonSell.disableButton (true);
		} else {
			buttonSell.disableButton (false);
		}
		
		costMoney.text = UserManager.Instance.self.getMoney () + Colors.GREEN + "+" + cast.ToString ();
	}
	
	public override void tapButtonEventBase (GameObject gameObj, bool enable)
	{
		base.buttonEventBase (gameObj); 
		if (gameObj.name == "buttonCard" && enable == true) { 
//			selectList.Clear(); //支持同时扣除卡片和装备 切换标签不清理
			changeButton ();
			tapIndex = 0;
			//SortCondition sc = SortConditionManagerment.Instance.getConditionsByKey (SiftWindowType.SIFTCARDWINDOW);
			Condition con = new Condition (SortType.SORT);
			con.addCondition (SortType.SORT_QUALITYDOWN);			
//			con.addCondition (SortType.SORT_LEVELUP);	
			SortCondition sc = new SortCondition ();
			sc.sortCondition = con;
			sc.siftConditionArr = new Condition[0];

			allList = SortManagerment.Instance.cardSort (StorageManagerment.Instance.getAllRole (), sc);
			recalculateAllCard (allList);
			indicateCardShow (showList);
			cardContent.reLoad (showList);
		} else if (gameObj.name == "buttonCard" && enable == false) { 
			cardContent.cleanAll ();
		} else if (gameObj.name == "buttonEquip" && enable == true) {
//			selectList.Clear(); //支持同时扣除卡片和装备 切换标签不清理
			changeButton ();
			tapIndex = 1;
			//SortCondition sc = SortConditionManagerment.Instance.getConditionsByKey (SiftWindowType.SIFTCEQUIPWINDOW);
			Condition con = new Condition (SortType.SORT);
			con.addCondition (SortType.SORT_QUALITYDOWN);		
//			con.addCondition (SortType.SORT_LEVELUP);		
			SortCondition sc = new SortCondition ();
			sc.sortCondition = con;
			sc.siftConditionArr = new Condition[0];

			allList = SortManagerment.Instance.equipSort (StorageManagerment.Instance.getAllEquip (), sc);
			recalculateAllEquip (allList);
			indicateEquipShow (showList);
			equipContent.reLoad (showList);
		} else if (gameObj.name == "buttonEquip" && enable == false) { 
			equipContent.cleanAll ();
		}

        selectList.Clear();

	}

	//断线重连
	public override void OnNetResume ()
	{
		base.OnNetResume ();
		messageBack (null);
	}

	//卖出回调
	private void sellBack (int num)
	{
		if (isShowMessageType) {
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, LanguageConfigManager.Instance.getLanguage ("s0100", num.ToString ()), messageBack);
			});
		} else {
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, LanguageConfigManager.Instance.getLanguage ("s0097", num.ToString ()), messageBack);
			});
		}
		
	}
	//刷新信息回调
	private void messageBack (MessageHandle msg)
	{
		isShowMessageType = false;
		costMoney.text = UserManager.Instance.self.getMoney () + Colors.GREEN + "+0";
		buttonSell.disableButton (true);
		selectList.Clear ();
		if (tapIndex == 0) {
			tapBase.resetTap ();
			tapBase.changeTapPage (tapBase.tapButtonList [0]);
		} else if (tapIndex == 1) {
			tapBase.resetTap ();
			tapBase.changeTapPage (tapBase.tapButtonList [1]);
		}
	}
	//卖出通信
	private void sellFPort (MessageHandle msg)
	{
		//取消就取消..
		if (msg.buttonID == MessageHandle.BUTTON_LEFT)
			return;
		
		
		SellGoodsFPort port = FPortManager.Instance.getFPort ("SellGoodsFPort") as SellGoodsFPort;
		port.sellGoods (change (selectList), cardofHaveStarSoul,sellBack);
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "confirm") {

			if (tapIndex == 0 && getErrorString () != "") {
				UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
					win.content.pivot = UIWidget.Pivot.Left;
					win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0093"), getErrorString (), sellFPort);
				});
				return;
			}
            int tipType = 0;  //出售装备提示信息
		    string dec = "";
		    bool flag0=false, flag1=false, flag2=false, flag3=false;
			for (int i = 0; i < selectList.Count; i++) {
                if (selectList[i] is Card)
                {
                    if ((selectList[i] as Card).getEquips() != null)
                    {
                        isShowMessageType = true;
                    }
                    if ((selectList[i] as Card).getEvoLevel() >= 1)
                    {
                        UiManager.Instance.openDialogWindow<MessageWindow>((win) =>
                        {
                            win.initWindow(2, LanguageConfigManager.Instance.getLanguage("s0094"), LanguageConfigManager.Instance.getLanguage("s0093"),
                                            LanguageConfigManager.Instance.getLanguage("sell05", (selectList[i] as Card).getName()), sellFPort);
                        });
                        return;
                    }
                    else if ((selectList[i] as Card).getQualityId() >= 4)
                    {
                        UiManager.Instance.openDialogWindow<MessageWindow>((win) =>
                        {
                            win.initWindow(2, LanguageConfigManager.Instance.getLanguage("s0094"), LanguageConfigManager.Instance.getLanguage("s0093"), LanguageConfigManager.Instance.getLanguage("s0095"), sellFPort);
                        });
                        return;
                    }
                }
                else if (selectList [i] is Equip) 
                {
                    Equip tmp = selectList[i] as Equip;
                        if (!flag0 && tmp.getQualityId() >= 4)
                        {
                            dec += LanguageConfigManager.Instance.getLanguage("equipSelele3");
                            tipType +=1;
                            flag0 = true;
                        }

                    if (!flag1 && tmp.equpStarState > 0)
                    {
                        dec += LanguageConfigManager.Instance.getLanguage("equipSelele");
                        flag1 = true;
                        tipType += 2;
                    }
                    if (!flag2 && tmp.getEXP() > 0)
                    {
                        dec += LanguageConfigManager.Instance.getLanguage("equipSelele1");
                        flag2 = true;
                        tipType += 4;
                    }
                    if (!flag3 && tmp.getrefineEXP() > 0)
                    {
                        dec += LanguageConfigManager.Instance.getLanguage("equipSelele2");
                        flag3 = true;
                        tipType += 8;
                    }
                }
			}
            if (flag1||flag2||flag3)
		    {
                dec += LanguageConfigManager.Instance.getLanguage("equipSelele4");
		    }
		    if (dec != "")
		    {
                UiManager.Instance.openDialogWindow<MessageWindow>((win) => {
                    win.initWindow(2, LanguageConfigManager.Instance.getLanguage("s0094"), LanguageConfigManager.Instance.getLanguage("s0093"), dec, sellFPort);
                                                                                
                });
                return;
		    }
			//品质低直接卖不解释
			SellGoodsFPort port = FPortManager.Instance.getFPort ("SellGoodsFPort") as SellGoodsFPort;
			port.sellGoods (change (selectList), cardofHaveStarSoul,sellBack);
			
		} else if (gameObj.name == "confirmOnTap") {
			selectList.Clear ();
			//showList已经排除特殊卡
			if (tapIndex == 0) {
//				Card c;
//				foreach (object obj in showList) {
//					c = obj as Card;
//					int quality = c.getQualityId ();
//                    
//					if (quality != QualityType.COMMON && quality != QualityType.EXCELLENT)
//						continue;
//					if(c.getEXP() > 1)
//						continue;
//					if (c.getHPExp() > 0 || c.getATTExp() > 0 || c.getDEFExp() > 0 || c.getMAGICExp() > 0 || c.getAGILEExp() > 0)
//						continue;
//					if (c.getSkills()[0].getEXP() > 0)
//						continue;
//					selectList.Add (c);
//				}
//				cardContent.updateVisibleItem ();
//				TextTipWindow.Show (LanguageConfigManager.Instance.getLanguage (selectList.Count == 0 ? "s0394" : "s0395"));
				UiManager.Instance.openDialogWindow<OneKeyWindow> ((win) => {
					win.neverChoose.gameObject.SetActive(false);
					win.spiritCardBox.SetActive(true);
				});
			} else {
//				foreach (object obj in showList) {
//					Equip e = obj as Equip;
//					int quality = e.getQualityId ();
//					if (quality != QualityType.COMMON && quality != QualityType.EXCELLENT)
//						continue;
////					if(e.getLevel() > 1)
////						continue;
//					selectList.Add (e);
//				}
//				equipContent.updateVisibleItem ();
//				TextTipWindow.Show (LanguageConfigManager.Instance.getLanguage (selectList.Count == 0 ? "s0400" : "s0399"));
				UiManager.Instance.openDialogWindow<OneKeyWindow> ((win) => {
					win.neverChoose.gameObject.SetActive(false);
					win.sacrificeEquipBox.SetActive(true);
				});
			}
			//changeButton ();
		} else if (gameObj.name == "close") {
			finishWindow ();
		}
	}

	public void oneKeyChoose()
	{
		selectList.Clear();
		if (tapIndex == 0) 
		{
			Card c;
			foreach (object obj in showList) {
				c = obj as Card;
				int quality = c.getQualityId ();
				if(qualityType == QualityType.SPIRITCARD)
				{
					if(IntensifyCardManager.Instance.isSpriteCard(c))
						selectList.Add (c);
				}
				else
				{
					if(quality <= qualityType && !IntensifyCardManager.Instance.isSpriteCard(c))
						selectList.Add (c);
				}
			}
			cardContent.updateVisibleItem ();
		}
		else 
		{
			foreach (object obj in showList) {
				Equip e = obj as Equip;
				int quality = e.getQualityId ();
				if(qualityType == QualityType.SACRIFICE)
				{
					if(IntensifyEquipManager.Instance.isEat(e))
						selectList.Add (e);
				}
				else
				{
					if(quality <= qualityType && !IntensifyEquipManager.Instance.isEat(e))
						selectList.Add (e);
				}
			}
			equipContent.updateVisibleItem ();
		}
		changeButton ();
	}
	

	//发送购买通信格式转换
	private string change (ArrayList list)
	{
		string strCard = "card";
		string strEquip = "equipment";
		cardofHaveStarSoul = new ArrayList();
		for (int i = 0; i < list.Count; i++) {
			if (list [i] is Card) {
				if((list[i] as Card).getStarSoulByAll()!=null&&((list[i] as Card).getStarSoulByAll()).Length>0)cardofHaveStarSoul.Add(list[i] as Card);
				strCard += "," + (list [i] as Card).uid;
			} else if (list [i] is Equip) {
				strEquip += "," + (list [i] as Equip).uid;
			}
		}
		return strCard + "|" + strEquip;
	}



}
