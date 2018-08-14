using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IntensifyEquipWindow : WindowBase
{
	public ContentIntensifyEquip content;
	public ButtonIntensifyEquip main;
	public ButtonIntensifyEquip[] foods;
	public flyEquipCtrl[] flyEquip;
	public ButtonBase buttonCancelSift;
	public ButtonBase buttonEvo;
	public ButtonBase buttonOneKey;
	public UILabel userMoney;
	public UILabel castMoney;
	public UILabel ExpLabel;
	public UILabel promptLabel;
	/**装备属性类型标签 */
	public UILabel[] equipArrLabel;
	/**装备属性增加值 */
	public UILabel[] equipAddArrLabel;
	/**装备等级标签 */
	public UILabel equipLvLabel;
	/**装备等级增加数值 */
	public UILabel equipLvValue;
	 /** 装备经验增加经验条 */
	public IncbarCtrl EquipIncbarCtrl;
	/** 附加属性信息对象 */
	AttrAddInfo attrAddInfo = new AttrAddInfo();
	int  comeFrom = 1;
	public const int NORMAL = 1;
	public const int TEAMEDIT = 2;
	public const int EQUIPSTORE = 3;
	public const int EQUIPCHOOSE = 4;//从装备选择面板过来
	public const int EQUIPVIEW = 5;//卡片信息界面
	public Equip equip;
	LevelupInfo levelInfo;
	ArrayList AllList;
//	private Card card;//从card属性面板带过来的
	SortCondition sc;
	private bool isMaxExp;//经验值是否超过最大值
	private bool isRarityCard;//食物中是否有史诗或者传说卡片
    private bool isMaxRefineEXP;//精炼经验是否超过最大值
	private Equip maxEquip; //满级装备
	private long overExp;//超出经验
	private float  starNum = 0; //返还的结晶
	private List<PrizeSample> starPrizeList = new List<PrizeSample> ();
	private bool starNeedTip = false;
	private float time; //装备属性切换显示的时间
	public const float SWITCHTIME = 3f;
	private string starIndex;
    public UILabel refineEXPLable;//精炼经验
    public UILabel refineLevelNum;//精炼等级
    public GameObject refineLabel;//精炼有关GameObj
    private Equip newEquipClone;//比较精炼等级克隆
    public UILabel maxRefineLevel;//精炼等级满级时显示

	int storageVersion = -1;
	/// <summary>
	/// 断线重连用
	/// </summary>
	public string uid="";

	public void cleanAllData ()
	{
		main.cleanData ();
		cleanAllFoodData ();
	}

	public void	disableOccupyEquip ()
	{

		foreach (ButtonIntensifyEquip each in content.buttonList) {
			if (each.isFullLevel () == true) {
				each.unlockButton ();
			}	
			if (each.isOccupy () == true && each.equip.uid != main.equip.uid) {
				each.lockButtonByOccupy ();
			}
		}
		ArrayList newList = addIntensifyEquip (AllList);
		showPrompt (newList.Count, LanguageConfigManager.Instance.getLanguage ("s0301"));
	}
	
	//显示提示
	private void showPrompt (int num, string str)
	{
		if (num <= 1) {
			promptLabel.text = str;
			promptLabel.gameObject.SetActive (true);
		} else {
			promptLabel.gameObject.SetActive (false);
		}
	}
	
	public void	disableMaxLevelEquip ()
	{
		foreach (ButtonIntensifyEquip each in content.buttonList) {
			if (each.isOccupy () == true) {
				each.unlockButton ();
			}
			if (each.isFullLevel () == true) {
				each.lockButtonByFullLevel ();
			}			
			
		}
		showPrompt (AllList.Count, LanguageConfigManager.Instance.getLanguage ("s0300"));
	}
	
	public void	undisableOccupyEquip ()
	{

		foreach (ButtonIntensifyEquip each in content.buttonList) {
			if (each.isOccupy () == true) {
				each.unlockButton ();
			}
		}
		
	}
	/// <summary>
	/// 增加，减少 装备，包括主装备 都会刷新经验值
	/// </summary>
	public  void recalculateEXP ()
	{
		long exp = 0;
		foreach (ButtonIntensifyEquip each in foods) {
			if (each.equip == null)
				continue;
			exp += each.equip.getEatenExp ();
		}
		ExpLabel.text = "+" + exp;
		updateBarInfo((int)exp);
	    recalcuateRefineEXP();
	}
    /// <summary>
    /// 精炼经验加多少
    /// </summary>
    /// <param name="exp"></param>
    public void recalcuateRefineEXP()
    {
        long refineEXP = 0;
        long equiprefineEXP = 0;
        if (equip == null)
        {
            maxRefineLevel.gameObject.SetActive(false);
            refineLabel.gameObject.SetActive(false);
            refineEXPLable.gameObject.SetActive(false);
            return;
        }
        foreach (ButtonIntensifyEquip each in foods)
        {
            if (each.equip == null)
                continue;
            refineEXP += each.equip.getrefineEXP();
        }
        
        long ollExp = equip.getrefineEXP();
        int expSid = equip.getRefineExpSid();
        if (expSid == 0 || EXPSampleManager.Instance.getEXPSampleBySid(expSid) == null)
        {
            isMaxRefineEXP = false;
        }
        else
        {
            long[] exppp = EXPSampleManager.Instance.getEXPSampleBySid(expSid).getExps();
			isMaxRefineEXP = (exppp[exppp.Length - 1] - ollExp) >= 0 && (exppp[exppp.Length - 1] - ollExp) < refineEXP * 0.6;
        }
        
        if (refineEXP==0||UserManager.Instance.self.getUserLevel()<30)
        {
            maxRefineLevel.gameObject.SetActive(refineEXP > 0);
            refineLabel.gameObject.SetActive(false);
        }
        else if (equip.getrefineLevel() == equip.getRefineMaxLevel() && RefineSampleManager.Instance.getRefineSampleBySid(equip.sid)!=null)
        {
            //Debug.LogError("refineEXP===" + refineEXP);
            refineLabel.gameObject.SetActive(false);
            maxRefineLevel.gameObject.SetActive(refineEXP > 0);
            maxRefineLevel.text = LanguageConfigManager.Instance.getLanguage("intensifyEquip12");
           // isMaxRefineEXP = true;
        }
        else if (RefineSampleManager.Instance.getRefineSampleBySid(equip.sid) != null && refineEXP!=0)
        {
            //isMaxRefineEXP = false;
            maxRefineLevel.gameObject.SetActive(false);
            refineLabel.gameObject.SetActive(true);
            refineEXPLable.gameObject.SetActive(true);
            refineEXPLable.text = "+" + (long)(refineEXP * 0.6);
            updateRefineLevelInfo((long)(refineEXP*0.6));
        }

    }
    /// <summary>
    /// 精炼等级提升了多少
    /// </summary>
    /// <param name="refineEXP"></param>
    private void updateRefineLevelInfo(long refineEXP)
    {
        Equip refineEq = main.equip;
        int refineLevel = refineEq.getrefineLevel();
        newEquipClone = (Equip)refineEq.Clone();
        newEquipClone.updatereExp(refineEXP+refineEq.getrefineEXP());
        int newrefineLevel = newEquipClone.getrefineLevel();
        int lv = newrefineLevel-refineLevel>=0?newrefineLevel-refineLevel:0;
        refineLevelNum.text = "+" + lv;
        newEquipClone = null;
        //if (lv >= 0)
        //{
        //    refineLevelNum.text = "+" + lv;
        //    newEquipClone = null;
        //}
        //else
        //    refineLevelNum.text = refineEq.getRefineMaxLevel().ToString();
    }
	private void updateBarInfo(int exp){
		Equip mainEq=main.equip;
		if(mainEq!=null){
			attrAddInfo.OldEquipGrade=mainEq.getLevel();
			attrAddInfo.EquipGrad = EXPSampleManager.Instance.getLevel (mainEq.getEXPSid(), mainEq.getEXP() + exp, mainEq.getLevel())-attrAddInfo.OldEquipGrade;
			long equipExpMax = EXPSampleManager.Instance.getMaxEXPShow(mainEq.getEXPSid(),mainEq.exp);
			long equipExpNow = EXPSampleManager.Instance.getNowEXPShow(mainEq.getEXPSid(),mainEq.exp);
			AttrChange[] attrsOld = equip.getAttrChanges (attrAddInfo.OldEquipGrade);
			AttrChange[] attrsNew = equip.getAttrChanges (attrAddInfo.OldEquipGrade+attrAddInfo.EquipGrad);
			for(int i=0;i<attrsOld.Length;i++){
				equipArrLabel[i].gameObject.SetActive(true);
				equipAddArrLabel[i].gameObject.SetActive(true);
				equipArrLabel[i].text= attrsOld [i].typeToString () + attrsOld [i].num ;
				equipAddArrLabel[i].text="+ "+(attrsNew [0].num - attrsOld [0].num);
			}
			equipLvLabel.text = "Lv";
			equipLvValue.text = "+" + attrAddInfo.EquipGrad.ToString();
			EquipIncbarCtrl.updateValue (equipExpNow,exp, equipExpMax);
		}else{
			for(int i=0;i<equipArrLabel.Length;i++){
				equipArrLabel[i].gameObject.SetActive(false);
				equipAddArrLabel[i].gameObject.SetActive(false);
			}
			attrAddInfo.OldEquipGrade=0;
			attrAddInfo.EquipGrad=0;
			equipLvLabel.text = "Lv";
			equipLvValue.text = "+" + 0;
			EquipIncbarCtrl.updateValue (0,0, 1);
		}
	}
	void Update ()
	{
		if (main.equip != null) {
			time -= Time.deltaTime;
			if (time <= 0) {
				time = SWITCHTIME;
				updateInfo ();
			}
			float alphaValue= sin();
			updateAttrInfoAlpha (alphaValue);
		}

	}
	public void updateInfo()
	{
		clearShowInfo();
		equipLvLabel.text = "Lv";
		equipLvValue.text = "+" + attrAddInfo.EquipGrad.ToString();
		AttrChange[] attrsOld = equip.getAttrChanges (attrAddInfo.OldEquipGrade);
		AttrChange[] attrsNew = equip.getAttrChanges (attrAddInfo.OldEquipGrade+attrAddInfo.EquipGrad);
		for(int i=0;i<attrsOld.Length;i++){
			equipAddArrLabel[i].gameObject.SetActive(true);
			equipArrLabel[i].gameObject.SetActive(true);
			equipArrLabel[i].text= attrsOld [i].typeToString () + attrsOld [i].num ;
			equipAddArrLabel[i].text= "+ " + (attrsNew [0].num - attrsOld [0].num);
		}
	}
	public void clearShowInfo(){
		equipLvValue.text = "";
		equipLvLabel.text = "";
		for(int i=0;i<equipAddArrLabel.Length;i++){
			equipAddArrLabel[i].text="";
			equipArrLabel[i].text="";
		}
	}
	private void updateAttrInfoAlpha(float alphaValue)
	{
		// hp
		EquipIncbarCtrl.incBar.SliderBar.alpha=alphaValue;
        if (attrAddInfo.EquipGrad > 0)
        {
            equipLvValue.alpha = alphaValue;
            refineLevelNum.alpha = alphaValue;
            refineEXPLable.alpha = alphaValue;
        }
        else
        {
            equipLvValue.alpha = 1;
            refineLevelNum.alpha = 1;
            refineEXPLable.alpha = 1;
        }
		for(int i=0;i<equipAddArrLabel.Length;i++){
			if(!equipAddArrLabel[i].text.StartsWith("[FF0000]0"))equipAddArrLabel[i].alpha=alphaValue;
			else equipAddArrLabel[i].alpha=1;
		}
	}
	public void removeMainEquip ()
	{
		equip=null;
		main.cleanData ();
		
	}
 
	public void removeFoodEquip (ButtonIntensifyEquip button)
	{
		button.cleanData ();
	}
	
	public void cleanAllFoodData ()
	{
		foreach (ButtonIntensifyEquip each in  foods) {
			each.cleanData ();
		}

	}
	
	public void setChooseEquip (Equip _equip)
	{
		equip = _equip;
		if (equip == null)
			main.cleanData ();

	}

	public void reloadAfterIntensify (Equip equip)
	{
		//升级后满级不再自动填入主坑
		if (equip.getLevel () < equip.getMaxLevel ())
			setChooseEquip (equip);
		else {
			maxEquip = equip;
			setChooseEquip (null);
			TextTipWindow.Show(LanguageConfigManager.Instance.getLanguage("intensifyEquip08"));
		}

		//确定了选不选中主卡再刷新容器

		ExpLabel.text = "+0"; 
		updateBarInfo(0);
		cleanAllFoodData ();
		IntensifyEquipManager.Instance.clearFoodEquip ();
		SortConditionManagerment.Instance.initDefaultSort (SiftWindowType.SIFT_EQUIPCHOOSE_WINDOW);

		updateContent ();
	}

	public ArrayList siftOccupyEquip ()
	{
		return null;
	}

	//是否为8个选中的献祭者 中的一个
	public ButtonIntensifyEquip  isOneOfTheCaster (string equipID)
	{
		
		foreach (ButtonIntensifyEquip each in foods) {
			if (each == null)
				continue;
			if (each.equip == null)
				continue;
			if (each.equip.uid == equipID)
				return each;
		}
		
		return null;
	}

	public ButtonIntensifyEquip  selectOneEmptyCastShower ()
	{
		if (foods [0].equip == null) {
			return foods [0];
		} else if (foods [1].equip == null) {
			return foods [1];
		} else if (foods [2].equip == null) {
			return foods [2];
		} else if (foods [3].equip == null) {
			return foods [3];
		} else if (foods [4].equip == null) {
			return foods [4];
		} else if (foods [5].equip == null) {
			return foods [5];
		} else if (foods [6].equip == null) {
			return foods [6];
		} else if (foods [7].equip == null) {
			return foods [7];
		}
		
		return null;
	}
	
	public bool isCasterEmpty ()
	{
		foreach (ButtonIntensifyEquip each in foods) {
			
			if (each.equip != null) {
				return false;
			}  
		}
		return true;
	}
	
	public bool isCasterFull ()
	{
		foreach (ButtonIntensifyEquip each in foods) {
			
			if (each.equip == null) {
				return false;
			}  
		}
		return true;
	}
	
	protected override void begin ()
	{
		base.begin ();

		
		if (!isAwakeformHide) {
			SortConditionManagerment.Instance.initDefaultSort(SiftWindowType.SIFT_EQUIPCHOOSE_WINDOW);
		}

		if (SortManagerment.Instance.isIntensifyEquipModifyed || StorageManagerment.Instance.EquipStorageVersion != storageVersion) {
			SortManagerment.Instance.isIntensifyEquipModifyed = false;
			updateContent ();
		}
		GuideManager.Instance.doFriendlyGuideEvent ();
		MaskWindow.UnlockUI ();
	}

	public 	void updateContent ()
	{
		userMoney.text = UserManager.Instance.self.getMoney () + "";

		storageVersion = StorageManagerment.Instance.EquipStorageVersion;
		sc = SortConditionManagerment.Instance.getConditionsByKey (SiftWindowType.SIFT_EQUIPCHOOSE_WINDOW);
//		Debug.LogError ("=-=========new guidesid=" + GuideManager.Instance.guideSid);
		if (GuideManager.Instance.isEqualStep (GuideGlobal.SPECIALSID70)) {
			AllList = toAddGuideEquip (StorageManagerment.Instance.getAllEquip ());
			AllList = SortManagerment.Instance.equipSort (AllList, sc);
		} else {
			AllList = SortManagerment.Instance.equipSort (StorageManagerment.Instance.getAllEquip (), sc);
		}

		if (AllList == null)
			return;
		//添加准备强化的武器到队列
		if (main.equip != null) {
			ArrayList newList = toRemoveUseInEquip (addIntensifyEquip (AllList));
			newList = equipSort (newList);
			newList = topIntensifyEquip (newList);
			if (newList == null)
				return;
			content.reLoad (newList);
			showPrompt (newList.Count, LanguageConfigManager.Instance.getLanguage ("s0301"));
		} else {
			AllList = equipSort (AllList);
			content.reLoad (AllList);
			showPrompt (AllList.Count, LanguageConfigManager.Instance.getLanguage ("s0300"));
		}
		//	StorageManagerment.Instance.isEquipStorageModifyed = false;
		changeButton ();
	}
	//装备排序
	private ArrayList equipSort (ArrayList list)
	{
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

	public void	changeButton ()
	{
		//没有主卡或者副卡满了就不能一键
		if (main.equip == null || IntensifyEquipManager.Instance.getFoodEquip ().Count >= 8) {
			buttonOneKey.disableButton (true);
		} else {
			buttonOneKey.disableButton (false);
		}



		int cast = 0;
		foreach (ButtonIntensifyEquip each in  foods) {
			if (each.equip == null)
				continue;
			cast += each.equip.getIntensifyCast ();
		}
		
		castMoney.text = "" + cast;

		if (cast == 0) {
			buttonEvo.disableButton (true);
			userMoney.color=Color.white;
			return;
		} else if (UserManager.Instance.self.getMoney () < cast) {
			userMoney.color=Color.red;
			buttonEvo.disableButton (true);
			return;
		}
		
		if (main.equip != null && isCasterEmpty () != true) {
			userMoney.color=Color.white;
			buttonEvo.disableButton (false);
		} else {
			buttonEvo.disableButton (true);
		}
	}

	public void Initialize (Equip _equip, int comefrom)
	{
		comeFrom = comefrom;
		equip = _equip;
		main.updateButton (equip);
		updateBarInfo(0);
		IntensifyEquipManager.Instance.setMainEquip (equip);
	}

	//主装备提前
	ArrayList topIntensifyEquip (ArrayList list)
	{
		if (main.equip == null)
			return list;

		ArrayList newList = new ArrayList();
		
		for (int i=0; i<list.Count; i++) {
			
			if ((list [i] as Equip).uid == main.equip.uid) {	
//				Equip tmp = list [0] as Equip;
//				list [0] = list [i];
//				list [i] = tmp;
				newList.Add (list [i]);
				list.Remove(list [i]);
				break;
			} 
		}
		ListKit.AddRange(newList,list);
		return newList;
	}

	//新手指引，只显示祭品装备
	ArrayList toAddGuideEquip (ArrayList list)
	{
		ArrayList newArrayList = new ArrayList ();
		for (int i=0; i<list.Count; i++) {
			if (ChooseTypeSampleManager.Instance.isToEat((list [i] as Equip),ChooseTypeSampleManager.TYPE_EQUIP_EXP))
				newArrayList.Add (list [i]);
		}
		
		return newArrayList;
	}

	//排除不是主祭品的装备中
	ArrayList toRemoveUseInEquip (ArrayList list)
	{
		ArrayList newArrayList = new ArrayList ();
		for (int i=0; i<list.Count; i++) {
			
			if ((list [i] as Equip).getState () == EquipStateType.OCCUPY && (list [i] as Equip).uid != main.equip.uid)
				continue;
			else
				newArrayList.Add (list [i]);
		}

		return newArrayList;
	}
	
	ArrayList addIntensifyEquip (ArrayList list)
	{
		ArrayList newList = new ArrayList ();
		
		bool has = false;//看循环里是否包含了该装备
		foreach (Equip each in list) {
			newList.Add (each);
			if (each.uid == main.equip.uid) {
				has = true;
			}
		}
		
		if (has == false) {
			newList.Add (main.equip);
		}
		
		return newList;
	}

	public bool hasMainEquip ()
	{
		if (main.equip != null)
			return true;
		else
			return false;
	}
	//获取献祭前主卡技能信息
	public void  getOldSkillInfo ()
	{
		if (hasMainEquip () == false)
			return;
		
		Equip tmpEquip = main.equip;
		levelInfo = new LevelupInfo ();
  
		levelInfo .oldLevel = tmpEquip.getLevel ();
		levelInfo.oldExp = tmpEquip.getEXP ();			
		levelInfo .oldExpUp = tmpEquip.getEXPUp ();	
		levelInfo.oldExpDown = tmpEquip.getEXPDown ();	
	 
	}
	
	//获取献祭后主卡技能信息
	public void  getNewSkillInfo (Equip  tmpEquip)
	{
		
		if (tmpEquip == null)
			return; 

		levelInfo.newLevel = tmpEquip.getLevel ();
		levelInfo.newExp = tmpEquip.getEXP ();			
		levelInfo.newExpUp = tmpEquip.getEXPUp ();	
		levelInfo.newExpDown = tmpEquip.getEXPDown ();	
		//	levelInfo.newskill =tmpEquip;
	}

	public void instensifyOver (string i)
	{
		StartCoroutine (playSummonEffect (i));

	}

	string eatFood ()
	{
		CardsIntensifyData data = new CardsIntensifyData ();
		foreach (ButtonIntensifyEquip each in foods) {
			
			if (each.equip == null)
				continue;
			data.addFood (each.equip.uid);
		}
		
		return data.ToFooding ();
	}
	
	//是否超过经验值上限
	private void isMoreThanMaxExp ()
	{
		long foodExp = 0;
		overExp = 0;
		foreach (ButtonIntensifyEquip each in foods) {
			
			if (each.equip == null)
				continue;
			foodExp += each.equip.getEatenExp();
		}

		if ((main.equip.getEXP() + foodExp) > main.equip.getEXPDown(main.equip.getMaxLevel())) {
			isMaxExp = true;
			overExp = (main.equip.getEXP() + foodExp) - main.equip.getEXPDown(main.equip.getMaxLevel());
		} else {
			isMaxExp = false;
		}
	}
	//是否是升星装备
	private void isEquipStar(){
		starNum = 0;
		starPrizeList.Clear ();
		foreach (ButtonIntensifyEquip each in foods) {
			int[] consumeInfo;
			if (each.equip == null)
				continue;
			bool finding = false;
			//遍历装备结晶
			if(each.equip.equpStarState > 0){
				starNum += Mathf.Pow(2f,each.equip.equpStarState*1.0f) * 15;
				for(int i=0;i<each.equip.equpStarState;i++){
					finding = false;
                    consumeInfo = EquipStarConfigManager.Instance.getCrystalCondition(each.equip.sid,i);
					PrizeSample tmp = new PrizeSample(PrizeType.PRIZE_PROP,consumeInfo[0],consumeInfo[1]);
				if(starPrizeList == null){
						starPrizeList.Add(tmp);
					}else{
						for(int j=0;j<starPrizeList.Count;j++){
							if(starPrizeList[j].pSid == tmp.pSid){
								starPrizeList[j].addNum(StringKit.toInt(tmp.num));
								finding = true;
							}
						}
						if(!finding){
							starPrizeList.Add(tmp);
						}
					}
				}
			}
			if(each.equip.equpStarState > 0){
				for(int i=0;i<each.equip.equpStarState;i++){
					finding = false;
                    consumeInfo = EquipStarConfigManager.Instance.getStoneCondition(each.equip.sid,i);
					PrizeSample tmp = new PrizeSample(PrizeType.PRIZE_PROP,consumeInfo[0],consumeInfo[1]);
					if(starPrizeList == null){
						starPrizeList.Add(tmp);
					}else{
						for(int j=0;j<starPrizeList.Count;j++){
							if(starPrizeList[j].pSid == tmp.pSid){
								starPrizeList[j].addNum(StringKit.toInt(tmp.num));
								finding = true;
							}
						}
						if(!finding){
							starPrizeList.Add(tmp);
						}
					}
				}
			}
		    if (each.equip.equpStarState > 9)
		    {
                PrizeSample[] prizeMoney = EquipStarConfigManager.Instance.getMoneyCostBySid(each.equip.sid);
		        if (prizeMoney == null) 
                    return;
		        for (int i = 9; i < each.equip.equpStarState; i++)
		        {
                    finding = false;
		            PrizeSample tmp = prizeMoney[i];
                    if (starPrizeList == null) {
                        starPrizeList.Add(tmp);
                    } else {
                        for (int j = 0; j < starPrizeList.Count; j++) {
                            if (starPrizeList[j].pSid == tmp.pSid) {
                                starPrizeList[j].addNum(StringKit.toInt(tmp.num));
                                finding = true;
                            }
                        }
                        if (!finding) {
                            starPrizeList.Add(tmp);
                        }
                    }
		        }
		    }
		}
		if (starNum > 0)
			starNeedTip = true;
		else
			starNeedTip = false;
	}
	//是否存在珍贵卡片
	private void isHaveRarityCard ()
	{
		foreach (ButtonIntensifyEquip each in foods) {
			
			if (each.equip == null)
				continue;
			if (each.equip.getQualityId () >= QualityType.EPIC&&!ChooseTypeSampleManager.Instance.isToEat(each.equip,2)) {
				isRarityCard = true;
				return;
			}  
		}
		isRarityCard = false;
	}
	 
	private void doIntensify (MessageHandle msg)
	{
		GuideManager.Instance.saveTimes (GuideManager.TypeEquip);
		if (msg != null && msg.buttonID == MessageHandle.BUTTON_LEFT) {
			buttonEvo.disableButton (false);
			buttonOneKey.disableButton (false);
			MaskWindow.UnlockUI();
			return;
		}
		string strOK = LanguageConfigManager.Instance.getLanguage ("s0093");
		string strCel = LanguageConfigManager.Instance.getLanguage ("s0094");
		if (isMaxExp) {
			isMaxExp = false;
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.dialogCloseUnlockUI=false;
				win.initWindow (2, strCel, strOK, LanguageConfigManager.Instance.getLanguage ("intensifyEquip09",overExp.ToString()), doIntensify);
			});
			return;
		}
		if (isRarityCard) {
			isRarityCard = false;
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.dialogCloseUnlockUI = false;
				win.initWindow (2, strCel, strOK, LanguageConfigManager.Instance.getLanguage ("s0269"), doIntensify);
			});
			return;	
		}
        if(isMaxRefineEXP)
        {
            isMaxRefineEXP = false;
            UiManager.Instance.openDialogWindow<MessageWindow>((win) =>
            {
                win.dialogCloseUnlockUI = false;
                win.initWindow(2, strCel, strOK, LanguageConfigManager.Instance.getLanguage("intensifyEquip13"), doIntensify);
            });
            return;

        }
		if (starNeedTip) {
			starNeedTip = false;
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.dialogCloseUnlockUI = false;
				win.initWindow (2, strCel, strOK, LanguageConfigManager.Instance.getLanguage ("equipStar10"), doIntensify);
			});
			return;	
		}
		EquipIntensifyFPort ces = FPortManager.Instance.getFPort ("EquipIntensifyFPort") as EquipIntensifyFPort;
		uid = main.equip.uid;
		ces.access (main.equip.uid, eatFood (), instensifyOver);//(第一个参数,主卡uid,第二个参数,食物卡uid,第三个参数回调)
	}

	//一键选择
	public void OneKeyChoose ()
	{
		cleanAllFoodData ();
		IntensifyEquipManager.Instance.clearFoodEquip ();

		List<Equip> onekeyListEat = IntensifyEquipManager.Instance.getOneKeySacrificeEat ();
		List<Equip> onekeyList = IntensifyEquipManager.Instance.getOneKeySacrifice ();
		ListKit.AddRange (onekeyListEat,onekeyList);

		if (onekeyListEat == null || onekeyListEat.Count <= 0) {
			UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("Intensify7"));
			return;
		}

		for (int i = 0; i < onekeyListEat.Count; i++) {
			//没空巢就断掉
			ButtonIntensifyEquip ctrl = selectOneEmptyCastShower ();
			if (ctrl == null)
				break;
			if (isOneOfTheCaster (onekeyListEat [i].uid))
				continue;
			
			if (onekeyListEat [i].getQualityId () <= IntensifyEquipManager.Choose) {
//				content.updateButton(onekeyListEat [i]);
				ctrl.updateButton (onekeyListEat [i]);
				IntensifyEquipManager.Instance.setFoodEquip (onekeyListEat [i]);
			}
		}

		onekeyListEat = IntensifyEquipManager.Instance.getFoodEquip ();
		if (onekeyListEat == null || onekeyListEat.Count <= 0) {
			UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("Intensify7"));
		}

		updateContent ();
		recalculateEXP ();
        recalcuateRefineEXP();
		changeButton ();
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "buttonIntensify") {
			buttonEvo.disableButton (true);
			buttonOneKey.disableButton (true);
			getOldSkillInfo ();
			isMoreThanMaxExp ();
			isHaveRarityCard ();
			isEquipStar();
			doIntensify (null);
		} else if (gameObj.name == "buttonOneKey") {
			if (IntensifyEquipManager.IsOpenOneKeyWnd) {
				UiManager.Instance.openDialogWindow<OneKeyWindow> ((win) => {
				});
			} else{
				//不出一键选择框的话就需要解锁
				MaskWindow.UnlockUI();
				OneKeyChoose ();
			}
		} else if (gameObj.name == "buttonSift") {
			UiManager.Instance.openWindow<SiftEquipWindow> ((win) => {
				win.Initialize (SiftEquipWindow.EVOLUTIONWINDOW, SiftWindowType.SIFT_EQUIPCHOOSE_WINDOW);
			});
		} else if (gameObj.name == "close") {
			IntensifyEquipManager.Instance.removeMainEquip ();
			IntensifyEquipManager.Instance.clearFoodEquip ();


//			sc = SortConditionManagerment.Instance.defaultEquipChooseSort ();
			if (!GuideManager.Instance.isGuideComplete ()) {
				GuideManager.Instance.doGuide (); 
				UiManager.Instance.openMainWindow ();
				return;
			}
			finishWindow ();

//			if (comeFrom == NORMAL) {
//				UiManager.Instance.openWindow<MainWindow>((win)=>{ });
//			}
//			if (comeFrom == EQUIPSTORE) {
//				UiManager.Instance.openWindow<StoreWindow>((win)=>{
//					win.Initialize (1);
//					if (StorageManagerment.Instance.isEquipStorageModifyed)
//						win.updateContent ();
//				});
//			}
//			if (comeFrom == EQUIPCHOOSE) {
//				UiManager.Instance.openWindow<EquipChooseWindow>((win)=>{
//				});
//			}
//			if (comeFrom == EQUIPVIEW) {
//				UiManager.Instance.openWindow<EquipAttrWindow>((win)=>{
//					win.Initialize (equip == null ? maxEquip : equip, EquipAttrWindow.CARDVIEW, showCardAttrWindow);
//				});
//			}
			
		} else if (gameObj.name == "buttonCancelSift") {
//			cleanSortCondition ();
			//Initialize (false);
			//最后把按钮隐藏了
			gameObj.SetActive (false);
		}
	}

	private void showEquipAttrWindow ()
	{
		UiManager.Instance.openWindow<EquipAttrWindow> ((win) => {
		});

	}

	private void showCardAttrWindow ()
	{
		//TODO
		//	CardBookWindow.Show ().showItemUpdateAll ();
	}
	private void starEquip ()
	{
		UiManager.Instance.openDialogWindow<EquipIntensifyResultsWIndow> ((win) => {
			Equip _equip = StorageManagerment.Instance.getEquip (starIndex);
			getNewSkillInfo (_equip);
			win.Initialize (StorageManagerment.Instance.getEquip (starIndex), levelInfo);
		});
	}

	public void cleanSortCondition ()
	{
		SortCondition sc = SortConditionManagerment.Instance.getConditionsByKey (SiftWindowType.SIFT_EQUIPCHOOSE_WINDOW);
		sc.clearSortCondition ();
	}
	
	IEnumerator playSummonEffect (string index)
	{
		for (int i=0; i< foods .Length; i++) {
			if (foods [i].equip != null) {
				flyEquip [i].gameObject.transform.position = foods [i].equipImage.transform.position;
				flyEquip [i].gameObject.SetActive (true);
				flyEquip [i].Initialize (foods [i].equipImage.mainTexture, this);
				foods [i].cleanData ();	
			}
		}
		EffectManager.Instance.CreateEffectCtrlByCache(main.transform,"Effect/UiEffect/Reinforced_SyntheticTwo",null);
		yield return new WaitForSeconds (2f);
		content.cleanAll ();
		//成功后显示按钮
		buttonEvo.disableButton (false);
		userMoney.color=Color.white;
		buttonOneKey.disableButton (false);

		if (starNum > 0) {
			starIndex = index;
			UiManager.Instance.openDialogWindow<AllAwardViewWindow>((win)=>{
				//win.Initialize(starPrizeList,starEquip,LanguageConfigManager.Instance.getLanguage("equipStar13"));
				win.Initialize (starPrizeList, starEquip, null);
				win.showComfireButton (true, Language ("s0093"));
			});
		}
		else {
			UiManager.Instance.openDialogWindow<EquipIntensifyResultsWIndow> ((win) => {
				Equip _equip = StorageManagerment.Instance.getEquip (index);
				getNewSkillInfo (_equip);
				win.Initialize (StorageManagerment.Instance.getEquip (index), levelInfo);
			});
		}
	}
	public override void OnNetResume ()
	{
		base.OnNetResume ();
		if(uid!="")
			instensifyOver(uid);
	}
}
