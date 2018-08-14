using UnityEngine;
using System.Collections;

/**
 * 卡片仓库
 * */
public class CardChooseWindow : WindowBase
{
	public UILabel numbers;
	public ContentCardChoose content;
	public Card  instandCard;
	public UILabel title;
	int showType;
	 SortCondition sc ;
	//开打模式
	public const int ROLECHOOSE = 1;//队伍卡片选择模式 
	public const int CARDCHANGE = 2;//卡牌替换模式 按钮显示替换
    public const int CHATSHOW = 5;//卡片聊天展示模式
    public const int CARDTRAINING = 8; // 卡牌训练选择模式
    public const int MINING = 11;//采矿选择模式
	public int chatChannelType;//聊天频道
	ArrayList list = null;
	int storageVersion=-1;


	//2014.7.5 added
	protected override void DoEnable ()
	{
		base.DoEnable ();
		UiManager.Instance.backGround.switchBackGround ("ChouJiang_BeiJing");
	}


	protected override void begin ()
	{ 
		base.begin ();
        sc = SortConditionManagerment.Instance.getConditionsByKey(SiftWindowType.SIFT_CARDCHOOSE_WINDOW);
		if (content.nodeList == null || !isAwakeformHide || StorageManagerment.Instance.RoleStorageVersion!=storageVersion) {
//			SortConditionManagerment.Instance.initDefaultSort(SiftWindowType.SIFT_CARDCHOOSE_WINDOW);
            sc.siftConditionArr = null;
			updateContent();
	
		}

		if (SortManagerment.Instance.isCardChooseModifyed) {
			SortManagerment.Instance.isCardChooseModifyed=false;
			updateContent ();
		}
	    if (!GuideManager.Instance.isEqualStep(135009000))
	    {
            MaskWindow.UnlockUI();
	    }
	    
	}
	public void updateContent()
	{
		storageVersion=StorageManagerment.Instance.RoleStorageVersion;
		if (showType == ROLECHOOSE || showType == CARDCHANGE) {
			list = SortManagerment.Instance.cardSort (StorageManagerment.Instance.getAllRoleByNotToEat (), sc, CardStateType.STATE_USING);
			list = ignoreTeamPlayer (list);
		} else if (showType == CARDTRAINING) {
			list = SortManagerment.Instance.cardSort(StorageManagerment.Instance.getAllRoleByTraining(), sc, CardStateType.STATE_USING);
            list = ignoreTeamPlayer(list);
        } else if (showType == MINING) {
            list = SortManagerment.Instance.cardSort(StorageManagerment.Instance.getAllRoleByNotToEatAndLevel(20), sc, CardStateType.STATE_USING);
            list = ignoreTeamPlayer(list);
        } else {
            list = SortManagerment.Instance.cardSort(StorageManagerment.Instance.getAllRole(), sc, CardStateType.STATE_USING);
            list = ignoreTeamPlayer(list);
        }

        if (GuideManager.Instance.isEqualStep(135009000)) {
            ArrayList all = StorageManagerment.Instance.getRoleBySid(11218);
            if (all == null || all.Count < 1)
            {
                GuideManager.Instance.doGuide();
            }
            else
            {
                Card tempCard = StorageManagerment.Instance.getRole((all[0] as Card).uid);
                ArrayList tempListt = new ArrayList();
                tempListt.Add(tempCard);
                for (int i = 0; i < list.Count; i++) {
                    Card cc = list[i] as Card;
                    if (cc.uid != tempCard.uid) tempListt.Add(cc);
                }
                list = tempListt;

            }
            
        }
		content.reLoad (list);
		numbers.text = list.Count + "/" + StorageManagerment.Instance.getRoleStorageMaxSpace ();
	    if (GuideManager.Instance.isEqualStep(135009000))
	    {
            StartCoroutine(Utils.DelayRun(() => {
                GuideManager.Instance.guideEvent();
                MaskWindow.UnlockUI();
            }, 1f));
	        
	    }
	}


	public void Initialize (int type )
	{ 
		showType = type;
        list = null;
	}
	//按照品质高低，品质相同再按照等级
	private ArrayList softCard(ArrayList tempList)
	{
		if(tempList == null)
			return null;
		Card temp1 = null;
		Card temp2 = null;
		Card temp = null;
 		for (int i = 0; i < tempList.Count - 1; i++) {
			MonoBase.print((tempList[i] as Card).getQualityId());
			for (int j = 0; j < tempList.Count - 1 - i; j++) {
				temp1 = tempList[j] as Card;
				temp2 = tempList[j+1] as Card;
				if(temp1.getQualityId() < temp2.getQualityId() || (temp1.getQualityId() == temp2.getQualityId() && temp1.getLevel() < temp2.getLevel()))
				{
					temp = temp1;
					tempList[j] = temp2;
					tempList[j + 1] = temp;
				}
			}
		}
		return tempList;
	}
 
	public int  getShowType ()
	{
		return showType;
	}


	//忽略已经选了的召唤兽
	void ignoreTeamBeast (ArrayList list)
	{	
		
		for (int i=0; i<list.Count; i++) {
			if ((list [i] as Card).uid == ArmyManager.Instance.ActiveEditArmy.beastid) {
				list.Remove (list [i]);
				break;
			}
		} 
	}
	
	//列表中不显示已经选中的队员, 增加,排除已经在卡牌训练上的
	private ArrayList ignoreTeamPlayer (ArrayList _allList)
	{
        CardTrainingManagerment tm = CardTrainingManagerment.Instance;
		ArrayList newList = new ArrayList();
		Card card;
		for (int i = 0; i < _allList.Count; i++) {
			card=_allList [i] as Card;
			bool has = ArmyManager.Instance.isExistByActiveEditArmy(card.uid) || (tm.CardsUid[0] == card.uid || tm.CardsUid[1] == card.uid || tm.CardsUid[2] == card.uid);
			if(has == false)
				newList.Add(_allList [i]);
		}
		return newList;
	}

	public override void DoDisable ()
	{
		base.DoDisable ();
	}
	

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
//			sc=SortConditionManagerment.Instance.defaultCardChooseSort();
			if (ArmyManager.Instance.activeInstandCard == null && showType != CHATSHOW) {
				//空栏位的情况
				finishWindow();
				return;
			} else {

				if (showType == ROLECHOOSE) {
					finishWindow();
//					showTeamEditWindow();
				}
//				else if (showType == CARDCHANGE) {
//					UiManager.Instance.openWindow<CardAttrWindow>((win)=>{
//						win.Initialize (ArmyManager.Instance.activeInstandCard, CardAttrWindow.CARDCHANGE, showTeamEditWindow);
//					});
//				}

				else if (showType == CHATSHOW) {
					finishWindow();
					/*
					UiManager.Instance.openDialogWindow<NewChatWindow> ((win) => {
						win.initChatWindow (ChatManagerment.Instance.sendType - 1);
					});
					*/
				}
                else if (showType == CARDTRAINING) {
                    finishWindow();
                } else if (showType == MINING) {
                    finishWindow();
                
                }
			}

//			if (fatherWindow != null) {
//				fatherWindow.destoryWindow ();
//			}
//			
//			destoryWindow ();
		}
		
		if (gameObj.name == "buttonfilter") {

			UiManager.Instance.openWindow<SiftCardWindow>((win)=>{
				if (showType == ROLECHOOSE || showType == MINING) {
					win.Initialize (null, SiftCardWindow.ROLECHOOSEWINDOW,SiftWindowType.SIFT_CARDCHOOSE_WINDOW);
				}  else if (showType == CHATSHOW) {
					win.setChatType(getChatType());
					win.Initialize (null, SiftCardWindow.CHATSHOW,SiftWindowType.SIFT_CARDCHOOSE_WINDOW);
                } else if (showType == CARDTRAINING) {
                    win.setChatType(getChatType());
                    win.Initialize(null, SiftCardWindow.CARDTRAINING, SiftWindowType.SIFT_CARDCHOOSE_WINDOW);
                }
			});

		} 
	}
	
	private void showTeamEditWindow ()
	{
		UiManager.Instance.openWindow<TeamEditWindow> ();
	//	win.updateChooseButton (oldCard);
	}
	
	//设置聊天频道
	public void setChatType (int _type)
	{
		chatChannelType = _type;
	}
	
	public int getChatType ()
	{
		return chatChannelType;
	}
}
