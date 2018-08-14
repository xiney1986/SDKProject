using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LearningSkillWindow : WindowBase
{
	
	public ContentSkillLearn content;
	public SacrificeShowerCtrl mainRole;
	public SacrificeShowerCtrl food;
	public ButtonBase buttonLearn;
	public ButtonBase buttonSift;
	public ButtonBase buttonCancelSift;
	public UILabel userMoney;
	public UILabel castMoney;
	public UILabel promptLabel;
	Color NROMAL = new Color (1f, 0.98f, 0.7f, 1);
	Color LINE = new Color (0.3f, 0.25f, 0.14f, 1);
	private ArrayList AllList;
	private ArrayList hideList;
	private ArrayList showList;
	private SiftCardWindow siftWindow;
	bool needReload=true;
	CallBack callback;
	private List<int> spiritCardSids = new List<int>(){11423,11424,11425,11426,11427};//精灵卡sid

	public void hideAll ()
	{
		mainRole.cleanData ();
		food.cleanAll ();
	}

	public void reloadAfterLearning ()
	{

		//更新学习卡
		mainRole.card = StorageManagerment.Instance.getRole (mainRole.card.uid);
		
		food.cleanData ();
//		cleanSortCondition ();
		Initialize (true);
	}
	
	public void recalculateAllCard (ArrayList list)
	{
		SortCondition sc = SortConditionManagerment.Instance.getConditionsByKey (SiftWindowType.SIFT_CARDCHOOSE_WINDOW);
		gruopCard ( AllList);
		showList = SortManagerment.Instance.cardSort (showList, createNextSortCondition ());
		showList = SortManagerment.Instance.cardSort (showList, sc);
	}
	
		public void gruopCard (ArrayList list)
	{
		showList = new ArrayList ();
		hideList = new ArrayList ();
		
		//生成新的引用list

		foreach (Card each in list) {
			showList.Add (each);
		}
		
		//把3方案中的人丢进隐藏卡组
		for (int i = 0; i < showList.Count; i++) {
			
			ArrayList teamUids = ArmyManager.Instance.getTeamCardUidList ();
			for (int n = 0; n<teamUids.Count; n++) {
				
				if ((showList [i] as Card).uid == (teamUids [n] as string)) {
					hideList.Add (showList [i]);
					showList.Remove (showList [i]);
					i--;
					break;
				}
				
			}
		}

	}

	//移除精灵卡
	private ArrayList removeSpiritCard(ArrayList list)
	{
		ArrayList lists = new ArrayList();
		for (int i = 0; i < list.Count; i++) {
			if(!spiritCardSids.Contains((list[i] as Card).sid))
			{
				lists.Add(list[i]);
			}
		}
		return lists;
	}
	
	public bool isCasterEmpty ()
	{
		if (food.card != null)
			return false;
		else
			return true;
		
	}

	protected override void begin ()
	{
		base.begin ();
		if (needReload == false) {
			MaskWindow.UnlockUI();
			return;
		}
		
		userMoney.text = UserManager.Instance.self.getMoney () + "";
		if (mainRole. card == null) {
			
			mainRole.cleanData ();
			//如果祭品没人，清理台面
			if (isCasterEmpty ()) {
				food.cleanAll ();
			}
			AllList = removeSpiritCard (AllList);
			content.reLoad (AllList);
			showPrompt(AllList.Count,LanguageConfigManager.Instance.getLanguage("s0283"));
		} else {
			//从技能学习回来		
			//移除主卡，
			ArrayList tmpList = RemoveMainCard (showList);
			//学习者放第一个
			topLearnCard (tmpList);
			tmpList = removeSpiritCard (tmpList);
			content.reLoad (tmpList);
			showPrompt(tmpList.Count,LanguageConfigManager.Instance.getLanguage("s0284"));
		}
		 
		changeButton ();
		MaskWindow.UnlockUI();
	}
	
	//显示提示
	private void showPrompt(int num,string str)
	{
		if(num <= 1)
		{
			promptLabel.text = str;
			promptLabel.gameObject.SetActive(true);
		}
		else
		{
			promptLabel.gameObject.SetActive(false);
		}
	}

	public void	changeButton ()
	{
		int cast = 0;
		if (food.card != null) {
			cast = food.card.getCardSkillLearnCast ();	
		}
		
		castMoney.text = "" + cast;
		
		
		
		if (mainRole.card != null && food.card != null && UserManager.Instance.self.getMoney () >= cast) {
			enableButton ();
		} else {
			disableButton ();
		}
			
	}
 
	void disableButton ()
	{
		buttonLearn.GetComponent<UIImageButton> ().isEnabled = false;
		buttonLearn.textLabel.color = Color.gray;
		buttonLearn.textLabel.effectColor = Color.black;
	}
	
	void enableButton ()
	{
		buttonLearn.GetComponent<UIImageButton> ().isEnabled = true;
		buttonLearn.textLabel.color = NROMAL;
		buttonLearn.textLabel.effectColor = LINE;
	}
	
	//构造筛选条件(配置极品以上)
	private SortCondition createSortCondition ()
	{ 
		SortCondition sc = new SortCondition ();
		sc.siftConditionArr = new Condition[1];
		sc.siftConditionArr [0] = new Condition (SortType.QUALITY);
		sc.siftConditionArr [0].addCondition (3);
		sc.siftConditionArr [0].addCondition (4);
		sc.siftConditionArr [0].addCondition (5);
		sc.sortCondition = new Condition (SortType.SORT);
		sc.sortCondition.addCondition (1);
		return sc;
	}
	
	//构造筛选条件(满级 配置史诗以上)
	private SortCondition createNextSortCondition ()
	{
		SortCondition sc = new SortCondition ();
		sc.siftConditionArr = new Condition[2];
		sc.siftConditionArr [0] = new Condition (SortType.QUALITY);
		sc.siftConditionArr [0].addCondition (4);
		sc.siftConditionArr [0].addCondition (5);
		sc.siftConditionArr [1] = new Condition (SortType.CARD_LEVEL_MAX);
		sc.siftConditionArr [1].addCondition (1);
		sc.sortCondition = new Condition (SortType.SORT);
		sc.sortCondition.addCondition (1);
		return sc;
	}
 
	public void Initialize (bool reload)
	{
		needReload=reload;
		if(needReload==false)
			return;
		
		
		SortCondition sc = SortConditionManagerment.Instance.getConditionsByKey (SiftWindowType.SIFT_CARDCHOOSE_WINDOW);
		AllList = SortManagerment.Instance.cardSort (StorageManagerment.Instance.getAllRole (), createSortCondition ());
		recalculateAllCard (AllList);	
		


	}
	public void Initialize (Card  importCard,CallBack back)
	{
		callback=back;
		//更新学习卡
		mainRole.card = StorageManagerment.Instance.getRole (importCard.uid);
		Initialize(true);
	}

	public ArrayList RemoveMainCard (ArrayList list)
	{
		ArrayList newlist = new ArrayList ();
		//如果选中的进化卡不是主卡，那么在准备显示的list中移除主卡;
		if (mainRole .card != null && mainRole.card.uid != UserManager.Instance.self.mainCardUid) {
			foreach (Card each in list) {	
				if (each.uid != UserManager.Instance.self.mainCardUid) {
					newlist.Add (each);
				}		
			}	


		}else{
			foreach (Card each in list) {	

					newlist.Add (each);
 	
			}	

		}

		return newlist;		
	}
	
 
	//进化卡放第一
	void topLearnCard (ArrayList list)
	{
		if (list == null)
			return;
		
		if (siftMainOrFood (list, mainRole.card)) {
			Card temp = list [0] as Card;
			list [0] = list [list.Count - 1] as Card;
			list [list.Count - 1] = temp;
		}
		
	}

	public void reLoadMatchingCard ()
	{
		ArrayList list = RemoveMainCard (showList);
		topLearnCard (list);
		content.reLoad (list);
		if(list.Count<=1)
			print ("show descript");
		if(mainRole.card.uid == UserManager.Instance.self.mainCardUid)
		{
			showPrompt(list.Count,LanguageConfigManager.Instance.getLanguage("s0284"));
		}
		else
		{
			showPrompt(list.Count,LanguageConfigManager.Instance.getLanguage("s0285"));
		}
	}
	
	public void showALLCard ()
	{
		//如果还在筛选中，过筛选
		SortCondition sc = SortConditionManagerment.Instance.getConditionsByKey (SiftWindowType.SIFT_CARDCHOOSE_WINDOW);
		ArrayList list = SortManagerment.Instance.cardSort (StorageManagerment.Instance.getAllRole (), createSortCondition ());
		list = SortManagerment.Instance.cardSort (list, sc);
		
		//	list =RemoveSacrificeCard(list);
		content.reLoad (list);
		showPrompt(list.Count,LanguageConfigManager.Instance.getLanguage("s0283"));
	}
	
	private bool siftMainOrFood (ArrayList list, Card card)
	{
		if (card != null) {
			for (int i = 0; i < list.Count; i++) {
				if ((list [i] as Card).uid == card.uid) {
					list.Remove (list [i]);
					break;
				}
			}
			list.Add (card);
			return true;
		}
		return false;
	}

	public bool hasMainRole ()
	{
		if (mainRole.card != null)
			return true;
		else
			return false;
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "buttonLearn") {
		
			fatherWindow.finishWindow();
			UiManager.Instance.openWindow<SkillChooseWindow>((win)=>{
				win.Initialize (mainRole.card, food.card, castMoney.text, SkillChooseWindow.TYPE_SKILLLEARN);
			});
			//这里不提前关闭的话，进化回来按钮显示有bug
			buttonCancelSift.gameObject.SetActive (false);
		}
		if (gameObj.name == "buttonSift") {
			UiManager.Instance.openWindow<SiftCardWindow>((scw)=>{
				siftWindow = scw;
				if (mainRole. card == null) {
					scw.Initialize (null, SiftCardWindow.LEARNINGSKILLWINDOWONE,SiftWindowType.SIFT_CARDCHOOSE_WINDOW);
				} else {
					scw.Initialize (null, SiftCardWindow.LEARNINGSKILLWINDOWTWO,SiftWindowType.SIFT_CARDCHOOSE_WINDOW);
				}
				fatherWindow.finishWindow();
			});
		}	
		if (gameObj.name == "close") {
			destoryWindow ();
			if (siftWindow != null) {
				siftWindow.destoryWindow ();
			}

			if(callback!=null){
				callback();
			}else{
				UiManager.Instance.openMainWindow();
			}
		}	
		if (gameObj.name == "cancelSift") {
			buttonSift.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0016");
			buttonSift.name = "buttonSift";
//			cleanSortCondition ();
			SortCondition sc = SortConditionManagerment.Instance.getConditionsByKey (SiftWindowType.SIFT_CARDCHOOSE_WINDOW);
			ArrayList list = SortManagerment.Instance.cardSort (StorageManagerment.Instance.getAllRole (), sc);
			content.reLoad (list);
		}
		
		if (gameObj.name == "buttonCancelSift") {
//			cleanSortCondition ();
			SortCondition sc = SortConditionManagerment.Instance.getConditionsByKey (SiftWindowType.SIFT_CARDCHOOSE_WINDOW);
			Initialize (true);
			//最后把按钮隐藏了
			gameObj.SetActive (false);
		}
		
	}

	public void cleanSortCondition ()
	{
		SortCondition sc = SortConditionManagerment.Instance.getConditionsByKey (SiftWindowType.SIFT_CARDCHOOSE_WINDOW);
		sc.clearSortCondition ();
	}
}
