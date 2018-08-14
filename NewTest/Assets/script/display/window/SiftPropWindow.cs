using UnityEngine;
using System.Collections;

public class SiftPropWindow : WindowBase
{
	public UIToggle[] propType_box;
	public UIToggle[] quality_box;
	public UIToggle[] sort_box;
	public const int PROPTYPE_NUM = 4;//职业数量是6
	public const int QUALITY_NUM = 5;//品质数量是5
	SortCondition sc;
	bool refresh;//筛选后是否要刷新窗口
	
	public void Initialize ()
	{
		clearBox();
		sc = SortConditionManagerment.Instance.getConditionsByKey (SiftWindowType.SIFT_PROPSTORE_WINDOW);
		initChecked ();
		initCheckLabel ();
	}
	
	private void initCheckLabel ()
	{
		propType_box [0].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0256");
		propType_box [1].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0258");
		propType_box [2].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0257");
		propType_box [3].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0259");
		quality_box [0].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0244");
		quality_box [1].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0243");
		quality_box [2].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0242");
		quality_box [3].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0241");
		quality_box [4].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0240");
		sort_box [0].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0247");
		sort_box [1].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0248");
	}
	
	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}
	
	private void initChecked ()
	{
		Condition condition;
		//还原上次的筛选条件
		if (sc != null && sc.siftConditionArr != null && sc.siftConditionArr.Length > 0) {
			for (int i=0; i < sc.siftConditionArr.Length; i++) {
				condition = sc.siftConditionArr [i];
				switch (condition.getType ()) {
				case SortType.PROP_TYPE:
					changeBox (propType_box, condition.getConditions ().ToArray (), "propType");
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
//		for (int i = 0; i < propType_box.Length; i++) {
//			propType_box[i].startsActive = false;
//		}
//		for (int i = 0; i < quality_box.Length; i++) {
//			quality_box[i].startsActive = false;
//		}
	}

	private void changeBox (UIToggle[] box, int[] value, string type)
	{
		for(int i=0;i<box.Length;i++)
			box[i].value=false;
		string name;
		for (int i=0; i<value.Length; i++) {
			name = getName (type, value [i]);
			for (int j=0; j<box.Length; j++) {
				if (box [j].name == name)
				{
					box [j].value = true;
					break;
				}
			}
		}
	}

	private void clearBox()
	{
		for(int i=0;i<propType_box.Length;i++)
			propType_box[i].value=false;
		for(int i=0;i<quality_box.Length;i++)
			quality_box[i].value=false;
		for(int i=0;i<sort_box.Length;i++)
			sort_box[i].value=false;
	}
	
	private string getName (string type, int value)
	{
		return type + "_" + value;
	}
	 
	public void needSift ()
	{
		SortManagerment.Instance.isStoreModifyed = true;		
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
		else if (gameObj.name == "close") {
			finishWindow();
		}
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
		Condition[] cons = new Condition[2];
		cons [0] = new Condition (SortType.PROP_TYPE);
		cons [1] = new Condition (SortType.QUALITY);
		for (int i = 0; i < propType_box.Length; i++) {
			if (propType_box [i].value) {
				cons [0].addCondition (toInt (propType_box [i].name));
			}
		}
		for (int i = 0; i < quality_box.Length; i++) {
			if (quality_box [i].value) {
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
		case SortType.PROP_TYPE:
			con = new Condition (SortType.PROP_TYPE);
			for (int i = 0; i < propType_box.Length; i++) {
				if (propType_box [i].value) {
					con.addCondition (toInt (propType_box [i].name));
				}
			}
			break;
		case SortType.QUALITY:
			con = new Condition (SortType.QUALITY);
			for (int i = 0; i < quality_box.Length; i++) {
				if (quality_box [i].value) {
					con.addCondition (toInt (quality_box [i].name));
				}
			}
			break;
		}
		return con;
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
	
	private string cutString (string str)
	{
		int index = str.IndexOf ('_');
		return str.Substring (0, index);
	}
	
	private int toInt (string str)
	{
		return int.Parse (str.Substring (str.Length - 1, 1));
	}
	
}
