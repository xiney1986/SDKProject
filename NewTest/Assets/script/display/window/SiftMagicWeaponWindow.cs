using UnityEngine;
using System.Collections.Generic;

// 筛选窗口组件
// @author 李程

public class SiftMagicWeaponWindow : WindowBase
{
	public UIToggle[] equipjob_box;
	public UIToggle[] quality_box;
	public UIToggle[] sort_box;
	public const int EQUIPJOB_NUM = 6;//职业数量是6
	public const int QUALITY_NUM = 5;//品质数量是5
	public const int PART_NUM = 5;//部位数量是5
	SortCondition sc;
	public const int EQUIPCHOOSEWINDOW = 1;
	public const int STOREWINDOW = 2;
	public const int EVOLUTIONWINDOW = 3;//进化窗口进入
	public const int CHATSHOW = 5;//聊天展示窗口
	int chatType;
	int currentType = 0;
	int currentPart = 0;
	bool refresh;//筛选后是否要刷新窗口
	
	public void Initialize (int type, int siftType)
	{
		clearBox ();
		currentType = type;
		sc = SortConditionManagerment.Instance.getConditionsByKey (siftType);

		initChecked ();
		initCheckLabel ();
	}
	
	private void initCheckLabel ()
	{
		equipjob_box [0].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0234");
		equipjob_box [1].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0235");
		equipjob_box [2].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0236");
		equipjob_box [3].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0237");
		equipjob_box [4].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0238");
		equipjob_box [5].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0239");
		quality_box [0].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0244");
		quality_box [1].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0243");
		quality_box [2].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0242");
		quality_box [3].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0241");
		quality_box [4].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0240");
        sort_box[0].GetComponent<SiftItemModule>().text.text = LanguageConfigManager.Instance.getLanguage("s0245l0");
        sort_box[1].GetComponent<SiftItemModule>().text.text = LanguageConfigManager.Instance.getLanguage("s0245l1");
		sort_box [2].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0247");
		sort_box [3].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0248");
		sort_box [4].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0254");
		sort_box [5].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0255");
        sort_box[6].GetComponent<SiftItemModule>().text.text = LanguageConfigManager.Instance.getLanguage("s0245l2");
        sort_box[7].GetComponent<SiftItemModule>().text.text = LanguageConfigManager.Instance.getLanguage("s0245l3");
	}
	
	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}
	//关闭按钮
	private void disableCheckState (UIToggle toggle)
	{
		toggle.value = false;
		SiftItemModule sim = toggle.GetComponent<SiftItemModule> ();
		if (sim.checkedBg != null)
			sim.checkedBg.gameObject.SetActive (false);
//		if (sim.text != null)
//			sim.text.color = Color.gray;
		if (sim.bg != null)
			sim.bg.color = Color.gray;
		toggle.GetComponent<BoxCollider> ().enabled = false;
	}
	//打开按钮状态
	private void enableCheckState (UIToggle toggle)
	{
		SiftItemModule sim = toggle.GetComponent<SiftItemModule> ();
		if (sim.checkedBg != null)
			sim.checkedBg.gameObject.SetActive (true);
//		if (sim.text != null)
//			sim.text.color = Colors.BUTTON_TEXT_NROMAL;
		if (sim.bg != null)
			sim.bg.color = Color.white;
		toggle.GetComponent<BoxCollider> ().enabled = true;
	}
	private void initChecked ()
	{
		Condition condition;
		//还原上次的筛选条件
		if (sc != null && sc.siftConditionArr != null && sc.siftConditionArr.Length > 0) {
			for (int i=0; i < sc.siftConditionArr.Length; i++) {
				condition = sc.siftConditionArr [i];
				switch (condition.getType ()) {
				case SortType.EQUIP_JOB:
					changeBox (equipjob_box, condition.getConditions ().ToArray (), "equipJob");
					break;
				case SortType.QUALITY:
					changeBox (quality_box, condition.getConditions ().ToArray (), "quality");
					break;
				default :
					break;
				}
			}
		}
		condition = sc.sortCondition;
		if (sc != null && condition != null && condition.type == SortType.SORT) 
			changeBox (sort_box, condition.getConditions ().ToArray (), "sort");
	}

	private void changeBox (UIToggle[] box, int[] value, string type)
	{
		string name;
		for (int i=0; i<value.Length; i++) {
			name = getName (type, value [i]);
			for (int j=0; j<box.Length; j++) {
				if (box [j].name == name) {
					box [j].value = true;
					break;
				}
			}
		}
	}

	private void clearBox ()
	{
		for (int i=0; i<equipjob_box.Length; i++) {
			equipjob_box [i].value = false;
			enableCheckState (equipjob_box [i]);
		}
		for (int i=0; i<quality_box.Length; i++) {
			quality_box [i].value = false;
			enableCheckState (quality_box [i]);
		}
		for (int i=0; i<sort_box.Length; i++) {
			sort_box [i].value = false;
			enableCheckState (sort_box [i]);
		}
	}

	private string getName (string type, int value)
	{
		return type + "_" + value;
	}
	 
	public void needSift ()
	{
		switch (currentType) {
		case EQUIPCHOOSEWINDOW:
			SortManagerment.Instance.isEquipChooseModifyed=true;
			break;
		case STOREWINDOW:
			SortManagerment.Instance.isStoreModifyed=true;
			break;
		case EVOLUTIONWINDOW:
			SortManagerment.Instance.isIntensifyEquipModifyed=true;
			break;
		case CHATSHOW:
			SortManagerment.Instance.isEquipChooseModifyed=true;
			break;
		}
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "confirm") {
			if (sortChange () || siftChange ()) {
				sc.siftConditionArr = getAllChecked ();
				sc.sortCondition = getSortChecked ();
				needSift();
			}
			finishWindow();
		}
		if (gameObj.name == "close") {
			finishWindow();
		}
	}

	private bool siftChange ()
	{
		Condition con1, con2;
		int[] arr1, arr2;
		if (sc != null && sc.siftConditionArr != null && sc.siftConditionArr.Length > 0) {
			for (int i=0; i<sc.siftConditionArr.Length; i++) {
				con1 = sc.siftConditionArr [i];
				con2 = getAllChecked (con1.type);
				if (con2 == null)
					return true;
				arr1 = con1.conditions.ToArray ();
				arr2 = con2.conditions.ToArray ();
				if (arr1.Length != arr2.Length)
					return true;
				for (int j=0; j<arr2.Length; j++) {
					for (int k=0; k<arr1.Length; k++) {
						if (arr2 [j] == arr1 [k])
							break;
						if (k >= arr1.Length - 1)
							return true;
					}
				}
			}
		} else if (getAllChecked ().Length > 0) {
			return true;
		}
		return false;
	}

	private bool sortChange ()
	{
		Condition con1 = sc.sortCondition;
		Condition con2 = getSortChecked ();
		int[] arr1 = con1.conditions.ToArray ();
		int[] arr2 = con2.conditions.ToArray ();
		if (arr1.Length != arr2.Length)
			return true;
		for (int j=0; j<arr2.Length; j++) {
			for (int k=0; k<arr1.Length; k++) {
				if (arr2 [j] == arr1 [k])
					break;
				if (k >= arr1.Length - 1)
					return true;
			}
		}
		return false;
	}
	
	//获得所有被选中的项
	private Condition getSortChecked ()
	{
		Condition con = new Condition (SortType.SORT);
		
		for (int i = 0; i < sort_box.Length; i++) {
			if (sort_box [i].value) {
				con.addCondition (toInt (sort_box [i].name));
			}
		}
		
		return con;
	}
	
	//获得所有被选中的项的名字
	private  Condition[] getAllChecked ()
	{
		Condition[] cons = new Condition[3];
		cons [0] = new Condition (SortType.EQUIP_JOB);
		cons [1] = new Condition (SortType.QUALITY);
		cons [2] = new Condition (SortType.EQUIP_PART);
		for (int i = 0; i < equipjob_box.Length; i++) {
			if (equipjob_box [i].value) {
				cons [0].addCondition (toInt (equipjob_box [i].name));
			}
		}
		for (int i = 0; i < quality_box.Length; i++) {
			if (quality_box [i].value) {
                if (toInt(quality_box[i].name) == 5) {
                    cons[1].addCondition(6);
                }
				cons [1].addCondition (toInt (quality_box [i].name));
			}
		}
		return cons;
	}

	//获得指定被选中的项的名字
	private  Condition getAllChecked (int type)
	{
		Condition con = null;
		switch (type) {
		case SortType.EQUIP_JOB:
			con = new Condition (SortType.EQUIP_JOB);
			for (int i = 0; i < equipjob_box.Length; i++) {
				if (equipjob_box [i].value) {
					con.addCondition (toInt (equipjob_box [i].name));
				}
			}
			break;
		case SortType.QUALITY:
			con = new Condition (SortType.QUALITY);
			for (int i = 0; i < quality_box.Length; i++) {
				if (quality_box [i].value) {
                    if (toInt(quality_box[i].name) == 5) {
                        con.addCondition(6);
                    }
					con.addCondition (toInt (quality_box [i].name));
				}
			}
			break;
		}
		return con;
	}
	
	private string cutString (string str)
	{
		int index = str.IndexOf ('_');
		return str.Substring (0, index);
	}

	private int toInt (string str)
	{
        return int.Parse(str.Split('_')[1]);
		//return int.Parse (str.Substring (str.Length - 1, 1));
	}
	
	//设置聊天发言频道
	public void setChatType (int _type)
	{
		chatType = _type;
	}
	
}
