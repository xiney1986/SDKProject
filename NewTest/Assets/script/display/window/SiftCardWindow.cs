using UnityEngine;
using System.Collections.Generic;

/**
 * 筛选窗口组件
 * @author 汤琦
 * */
public class SiftCardWindow : WindowBase
{
	
	public UIToggle[] job_box;
	public UIToggle[] quality_box;
	public UIToggle[] sort_box;
	public Card chooseCard;
	public const int  INTOTEAM = 1;//上阵模式
	public const int  CARDCHANGE = 2;//卡牌替换模式
	//进入方式
	public const int SACRIFICEWINDOW = 1;//献祭窗口进入.
	public const int LEARNINGSKILLWINDOWONE = 2;//学习技能窗口进入.
	public const int LEARNINGSKILLWINDOWTWO = 9;
	public const int EVOLUTIONWINDOW = 3;//进化窗口进入.
	public const int ROLECHOOSEWINDOW = 4;//卡片选择窗口进入. 
	public const int CHATSHOW = 5;//卡片选择窗口进入. 
	public const int ROLEVIEW = 6;//卡片浏览窗口进入.	 
	public const int ADDONWINDOW = 8;//附加窗口进入
	public const int SACRIFICEBEASTWINDOW = 10;//召唤兽强化窗口进入
	public const int INTENSIFYCARD = 11;//强化卡片进入
	public const int PICTURE = 12;//图鉴进入
	public const int CARDSTORE = 13;//卡片仓库进入
    public const int CARDTRAINING = 14; //卡牌训练
	SortCondition sc;
	int comeFrom;
	public int chatChannelType;//聊天频道

	
	public void Initialize (Card chooseItem, int from, int siftType)
	{
		clearBox ();
		sc = SortConditionManagerment.Instance.getConditionsByKey (siftType);
		this.comeFrom = from;
		initChecked ();
		initCheckLabel ();
		chooseCard = chooseItem;
	}

	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}
	
	private void initCheckLabel ()
	{
		job_box [0].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0234");
		job_box [1].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0235");
		job_box [2].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0236");
		job_box [3].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0237");
		job_box [4].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0238");
		job_box [5].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0239");
		quality_box [0].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0244");
		quality_box [1].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0243");
		quality_box [2].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0242");
		quality_box [3].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0241");
		quality_box [4].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0240");
		sort_box [0].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0245");
		sort_box [1].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0246");
		sort_box [2].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0247");
		sort_box [3].GetComponent<SiftItemModule> ().text.text = LanguageConfigManager.Instance.getLanguage ("s0248");
	}
	
	//设置按钮状态
	private void setCheckState (UIToggle toggle)
	{
		toggle.startsActive = false;
		SiftItemModule sim = toggle.GetComponent<SiftItemModule> ();
		sim.checkedBg.gameObject.SetActive (false);
		sim.text.color = Color.gray;
		//sim.bg.color = Color.gray;
	}
	
	//根据进入的类型初始化按钮的状态
	private void initChecked ()
	{
		if (comeFrom == LEARNINGSKILLWINDOWONE) {
			for (int i = 0; i < quality_box.Length; i++) {
				if (toInt (quality_box [i].name) < 3) {
					setCheckState (quality_box [i]);
				}
			}
		} else if (comeFrom == LEARNINGSKILLWINDOWTWO) {
			for (int i = 0; i < quality_box.Length; i++) {
				if (toInt (quality_box [i].name) < 4) {
					setCheckState (quality_box [i]);
				}
			}
		} else if (comeFrom == CARDTRAINING) {
            for (int i = 0; i < quality_box.Length; i++)
            {
                if (toInt(quality_box[i].name) <= 2)
                    setCheckState(quality_box[i]); 
            }
        } else {
			Condition condition;
			//还原上次的筛选条件
			if (sc != null && sc.siftConditionArr != null && sc.siftConditionArr.Length > 0) {
				for (int i=0; i < sc.siftConditionArr.Length; i++) {
					condition = sc.siftConditionArr [i];
					switch (condition.getType ()) {
					case SortType.JOB:
						changeBox (job_box, condition.getConditions ().ToArray (), "job");
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
		//		for (int i = 0; i < quality_box.Length; i++) {
		//				quality_box [i].startsActive = false;
//			}
//		for (int i = 0; i < job_box.Length; i++) {
//			job_box [i].startsActive = false;
//		}
		
	}
	 
	private void changeBox (UIToggle[] box, int[] value, string type)
	{
		for (int i=0; i<box.Length; i++)
			box [i].value = false;
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
		for (int i=0; i<job_box.Length; i++)
			job_box [i].value = false;
		for (int i=0; i<quality_box.Length; i++)
			quality_box [i].value = false;
		for (int i=0; i<sort_box.Length; i++)
			sort_box [i].value = false;
	}
	
	private string getName (string type, int value)
	{
		return type + "_" + value;
	}
	
	public void needSift ()
	{
		switch (comeFrom) {
		case INTENSIFYCARD:
			SortManagerment.Instance.isIntensifyCardChooseModifyed = true;
			break;
 
		case ROLECHOOSEWINDOW:
			SortManagerment.Instance.isCardChooseModifyed = true;
			break;
		case LEARNINGSKILLWINDOWONE:
			break;
		case CHATSHOW:
			SortManagerment.Instance.isCardChooseModifyed = true;
			break;
		case PICTURE:
			(fatherWindow as PictureWindow).needReload = true;
			break;
		case CARDSTORE:
			SortManagerment.Instance.isCardStoreModifyed = true;
			break;
        case CARDTRAINING :
            SortManagerment.Instance.isCardChooseModifyed = true;
            break;

		}

	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "confirm") { 
			if (sortChange () || siftChange ()) {
				sc.siftConditionArr = getSiftCondition ();
				sc.sortCondition = getSortCondition ();
				needSift ();
			}
			finishWindow ();
		}
		if (gameObj.name == "close") {
			finishWindow ();
		}
	}


	//获得所有被选中的项
	private Condition getSortCondition ()
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
	private  Condition[] getSiftCondition ()
	{
		Condition[] cons = new Condition[2];
		cons [0] = new Condition (SortType.JOB);
		cons [1] = new Condition (SortType.QUALITY);
		for (int i = 0; i < quality_box.Length; i++) {
			if (quality_box [i].value) {
				cons [1].addCondition (toInt (quality_box [i].name));
			}
		}
		for (int i = 0; i < job_box.Length; i++) {
			if (job_box [i].value) {
				cons [0].addCondition (toInt (job_box [i].name));
			}
		}
		return cons;
	}

	//获得指定被选中的项的名字
	private  Condition getAllChecked (int type)
	{
		Condition con = null;
		switch (type) {
		case SortType.JOB:
			con = new Condition (SortType.JOB);
			for (int i = 0; i < job_box.Length; i++) {
				if (job_box [i].value) {
					con.addCondition (toInt (job_box [i].name));
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
		} else if (getSiftCondition ().Length > 0) {
			return true;
		}
		return false;
	}
	
	private bool sortChange ()
	{
		Condition con1 = sc.sortCondition;
		Condition con2 = getSortCondition ();
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
