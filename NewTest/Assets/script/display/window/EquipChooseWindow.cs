using UnityEngine;
using System.Collections;

/**
 * 装备仓库窗口(仅仅包含装备)
 * @author longlingquan
 * */
public class EquipChooseWindow : WindowBase
{
	public ContentEquipChoose contentEquip;
	public UILabel storeSpceNum;
	private Equip equip;
	public SortCondition sc ;//筛选条件
	ArrayList equipList;//装备集
	public int comeFrom = 0;
	public const int FROM_CHAT = 1;
	public const int FROM_CARDATTR = 2;
	public const int FROM_OTHER = 3;
    public const int FROM_CHAT_FRIEND = 4;
	public const int FROM_TO_UPSTAR = 5;

	protected override void begin ()
	{
		base.begin ();
		if (isAwakeformHide == false) {
			//初始化筛选
			sc = SortConditionManagerment.Instance.initDefaultSort(SiftWindowType.SIFT_EQUIPCHOOSE_WINDOW);
		

			if (comeFrom == FROM_CARDATTR) {
				//从卡片属性过来就筛选装备部件
				sc.siftConditionArr = getSoftConditions ();
			}

			updateContent ();

		}
		//筛选改变,装备仓库,道具仓库改变,刷新
		if (SortManagerment.Instance.isEquipChooseModifyed) {
			SortManagerment.Instance.isEquipChooseModifyed=false;
			updateContent ();
		}
		GuideManager.Instance.guideEvent ();
		GuideManager.Instance.withoutFriendlyGuide ();
		MaskWindow.UnlockUI ();
	}

	public void updateContent ()
	{
		sc = SortConditionManagerment.Instance.getConditionsByKey (SiftWindowType.SIFT_EQUIPCHOOSE_WINDOW);
		if (comeFrom == FROM_CARDATTR)
			equipList = SortManagerment.Instance.equipSort (StorageManagerment.Instance.getAllEquipByNotToEat (), sc);
		else if(comeFrom == FROM_TO_UPSTAR)
			equipList = SortManagerment.Instance.equipSort(StorageManagerment.Instance.getAllEquipByLegend(),sc);
		else
			equipList = SortManagerment.Instance.equipSort (StorageManagerment.Instance.getAllEquip (), sc);
		
		//剔除已经穿戴身上的装备
		if (comeFrom != FROM_CHAT && comeFrom != FROM_CHAT_FRIEND&&comeFrom!=FROM_TO_UPSTAR) {
			for (int i = 0; i < equipList.Count; i++) {
				if (((equipList [i] as Equip).state & EquipStateType.OCCUPY) > 0) {
					equipList.Remove (equipList [i]);
					i--;
				} else if (EquipManagerment.Instance.activeEquipMan != null && !EquipManagerment.Instance.canUse (EquipManagerment.Instance.activeEquipMan.sid, (equipList [i] as Equip).sid)) {
					equipList.Remove (equipList [i]);
					i--;
				}
			}
		}
		
		equipList = equipSort (equipList);
		if (comeFrom != FROM_CHAT && comeFrom != FROM_CHAT_FRIEND && comeFrom!=FROM_TO_UPSTAR) {
			contentEquip.reLoad (equipList, ContentEquipChoose.PUT_ON);
			
		} else if(comeFrom == FROM_TO_UPSTAR)
		{
			//装备升星指引载入
			contentEquip.reLoad (equipList, ContentEquipChoose.FROM_TO_UPSTAR);
		}
		else
		{
			contentEquip.reLoad (equipList, ContentEquipChoose.CHATSHOW);
		}
		
		storeSpceNum.text = equipList.Count + "/" + StorageManagerment.Instance.getEquipStorageMaxSpace ();
	}

	//装备排序
	private ArrayList equipSort (ArrayList list)
	{
		if (!GuideManager.Instance.isGuideComplete ()) {//指引的特殊排序
			for (int i = 0; i < list.Count; i++) {
				if ((list [i] as Equip).sid == GuideGlobal.SPECIALEQUIPSID) {
					Equip temp = list [i] as Equip;
					list.Remove (list [i]);
					list.Insert (0, temp);
					break;
				}
			}
			return list;
		}

		ArrayList equipList = new ArrayList ();
		ArrayList listequip = new ArrayList ();
		for (int i = 0; i < list.Count; i++) {
			if (!(list [i] as Equip).isPutOn (StringKit.toInt (UserManager.Instance.self.mainCardUid))) {
				equipList.Add (list [i]);
			} else {
				listequip.Add (list [i]);
			}
		}
		ListKit.AddRange (equipList,listequip);
		return equipList;
	}


	//排序条件
	private Condition getSoftCondition ()
	{
		Condition con = new Condition (SortType.SORT);
		con.addCondition (1);
		return con;
	}
	//筛选条件
	private  Condition[] getSoftConditions ()
	{
		Condition[] cons = new Condition[3];
		cons [0] = new Condition (SortType.EQUIP_JOB);
		cons [1] = new Condition (SortType.QUALITY);
		cons [2] = new Condition (SortType.EQUIP_PART);

		if (EquipManagerment.Instance.activePartID > 0)
			cons [2].addCondition (EquipManagerment.Instance.activePartID);
		return cons;
	}

	public void Initialize (int comeFrom)
	{
		this.comeFrom = comeFrom;
	}
	//初始化装备
	public void Initialize (Equip equip)
	{
		this.equip = equip;
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
			if (comeFrom == FROM_CHAT || comeFrom == FROM_CHAT_FRIEND) {
				finishWindow();
				/*这里开始是可滑动聊天窗口展示的关闭后处理，暂时不删
				UiManager.Instance.openDialogWindow<NewChatWindow> ((win) => {
					win.initChatWindow (ChatManagerment.Instance.sendType - 1);
				});
				*/
			} else
				EquipManagerment.Instance.finishEquipChange ();
		} else if (gameObj.name == "buttonSift") {

			UiManager.Instance.openWindow<SiftEquipWindow> ((win) => {
                if (comeFrom == FROM_CHAT || comeFrom == FROM_CHAT_FRIEND) {
					win.Initialize (SiftEquipWindow.CHATSHOW, SiftWindowType.SIFT_EQUIPCHOOSE_WINDOW);
					win.initPartCondition (0);
				} else {
					win.Initialize (SiftEquipWindow.EQUIPCHOOSEWINDOW, SiftWindowType.SIFT_EQUIPCHOOSE_WINDOW);
					win.initPartCondition (EquipManagerment.Instance.activePartID);
				}
			});
		} else if (gameObj.name == "intensifyButton") {
			finishWindow();
		}
	} 

	public override void OnNetResume ()
	{
		base.OnNetResume ();
		updateContent ();
	}
}
