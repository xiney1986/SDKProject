using UnityEngine;
using System.Collections;

public class ButtonIntensifyEquip : ButtonBase
{
	
	public UITexture equipImage;
	public Equip equip;//关联的card对象
	public UISprite qualityBack;//品质图标
	public UILabel level;//等级不解释
	public UISprite selectPic;//选中标志
	public ButtonIntensifyEquip shower;//选中展示的按钮
	public bool isChoose;//是否选中;
	public	IntensifyEquipWindow win;
	public const string MAINSELECT = "img_9";
	public const string CASTSELECT = "gou_3";
	public const string OCCUPY = "s0041";
	public const string MAXLEVEL = "s0070";
	public const string EAT = "JustToEat00";
	public UILabel  occupyLabel ;
	public bool isShower;
	public UILabel starLvel;
	private const string FIRSTOBJECT = "001";//加载的第一个对象
	private const string SECONDOBJECT = "002";//加载的第二个对象
	private const string THREEOBJECT = "003";//加载的第三个对象
	
	public override void DoLongPass ()
	{
		if (equip == null)
			return;
		if (!GuideManager.Instance.isGuideComplete ())
			return;
		UiManager.Instance.openWindow<EquipAttrWindow> ((win) => {
			win.Initialize (equip, EquipAttrWindow.OTHER,null);
		});
	}
	
	public  bool isOccupy ()
	{
		if (equip == null)
			return false;
 	
		if (equip.getState () == EquipStateType.OCCUPY) {
			return true;
		} else {
			return false;
		}
	}
	
	public  void changeBlack ()
	{
		equipImage.color = Color.gray;
		//	selectPic.color=Color.gray;
		//	sign.color=Color.gray;
		
	}

	public  bool isFullLevel ()
	{
		if (equip == null)
			return false;
		
		if (equip.getLevel () >= equip.getMaxLevel ())
			return true;
		
		return false;
		
	}

	//是吃的么
	public  bool isEat ()
	{
		if (equip == null)
			return false;
		
		if (ChooseTypeSampleManager.Instance.isToEat (equip, ChooseTypeSampleManager.TYPE_EQUIP_EXP))
			return true;

		
		return false;
		
	}

	public  void showBase ()
	{
		qualityBack.gameObject.SetActive (true);
		qualityBack.spriteName = QualityManagerment.qualityIDToIconSpriteName (1);
	}

	public  void cleanData ()
	{
		equip = null;
		equipImage.gameObject.SetActive (false);
		level.text = "";
		if(starLvel != null)
			starLvel.gameObject.SetActive (false);
		qualityBack.gameObject.SetActive (true);
		qualityBack.spriteName = QualityManagerment.qualityIDToIconSpriteName (1);
		
	}

	public  void 	cleanAll ()
	{
		equipImage.gameObject.SetActive (false);
		level.text = "";
		qualityBack.gameObject.SetActive (false);
	
	}

	public  void changeLight ()
	{
		equipImage.color = Color.white;
		//	selectPic.color=Color.gray;
		//	sign.color=Color.gray;
		
	}
	
	// 等你初始化诗句
	public  void Initialize (Equip _equip)
	{
		win = fatherWindow as IntensifyEquipWindow;
		updateButton (_equip);
	}
	
	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		GuideManager.Instance.doGuide ();
		GuideManager.Instance.guideEvent (); 
		if (equip == null)
			return;
		if (starLvel != null)
			starLvel.gameObject.SetActive (true);
		//是主卡
		if (this.name == win.main.name) {
			win.content.updateButton (this);
			return;
		}
		//是食物卡
		for (int i = 0; i < win.foods.Length; i++) {
			if (this.name == win.foods [i].name) {
				win.content.updateButton (this);
				return;
			}
		}
		if (isChoose == false) {
			selectOn ();
		} else {
			selectOff ();
		}
		if (win.equip == null&&isChoose) {
			win.setChooseEquip (equip);
		}
		win.recalculateEXP ();
        win.recalcuateRefineEXP();
		win.changeButton ();
	}

	public void putOff(){
		isChoose = false;
		if (equip != null && win.main.equip != null && win.main.equip.uid == equip .uid) {
			
			win.removeMainEquip ();
			//解禁装备中的按钮
			//win.undisableOccupyEquip ();
			win.disableMaxLevelEquip ();
			IntensifyEquipManager.Instance.removeMainEquip ();
		} else {
			//下面是祭品
//		if (shower != null) {
			win.removeFoodEquip (this);
			IntensifyEquipManager.Instance.removeFoodEquip (equip);
			//		}	
		}
			win.recalculateEXP ();
            win.recalcuateRefineEXP();
			win.changeButton ();

	}
 
	void selectOn ()
	{


		if (win.hasMainEquip () == false) {
			
			//满级的装备不能再选成进化者
			if (isFullLevel () == true) {
				return;
			}
			
			//进化者为空就选为进化者
			setShower (win.main, MAINSELECT);
			IntensifyEquipManager.Instance.setMainEquip (equip);
			win.disableOccupyEquip ();
			//	win.foods.showBase ();
			isChoose = true;
//			//选中主角后，祭品有人，就能点献祭
//			if (win.isCasterEmpty () == false)
//				win.enableButton ();

			win.updateContent ();
			return;
		} 
		
//		//主卡不能成为祭品啊~~~
//		if (UserManager.Instance.self.mainCardUid == equip.uid) {
//			return;
//		}
		


		//装备中的物品不能作为祭品
		if (win.hasMainEquip () == true && isOccupy () == true)
			return;
		
		//进化者有了,并且祭品没满人，就选为祭品
		if (win.hasMainEquip () == true && win.isCasterFull () == false) {
			setShower (win.selectOneEmptyCastShower (), CASTSELECT);
			//	win.enableButton ();
			IntensifyEquipManager.Instance.setFoodEquip (equip);
			isChoose = true;	
		}
 
	}
	void Update ()
	{
		if (starLvel != null && starLvel.gameObject != null && starLvel.gameObject.activeSelf)
			starLvel.alpha = sin ();
	}	
	void selectOff ()
	{
		isChoose = false;
		if (equip != null && win.main.equip != null && win.main.equip.uid == equip .uid) {
	 
			win.removeMainEquip ();
			//解禁装备中的按钮
			//win.undisableOccupyEquip ();
			win.disableMaxLevelEquip ();
			cleanShower ();
			win.updateContent ();
			IntensifyEquipManager.Instance.removeMainEquip ();
			return;
		}	
		//下面是祭品
		
		if (shower != null) {
			win.removeFoodEquip (shower);
			IntensifyEquipManager.Instance.removeFoodEquip (equip);
		}	
		cleanShower ();	
	}

	void cleanShower ()
	{
		shower = null;
		selectPic.gameObject.SetActive (false);
	}
	
	void setShower (ButtonIntensifyEquip _shower, string spName)
	{

		shower = _shower;
		if (_shower != null)
			_shower.updateButton (equip);
		if (spName == "") {
			selectPic.gameObject.SetActive (false);
		} else {
			selectPic.gameObject.SetActive (true);
			selectPic.spriteName = spName;
			if (spName == MAINSELECT) {
				selectPic.height = 90;
				selectPic.width = 90;
			} else {
				selectPic.height = 70;
				selectPic.width = 64;
			}
		}
	}

	void cleanBtton ()
	{
		equipImage.gameObject.SetActive (false);
		equipImage.mainTexture = null;
		level.text = "";
		qualityBack .gameObject.SetActive (false);
		equip = null;
		shower = null;
		selectPic.gameObject.SetActive (false);
		
	}

	//已装备
	public void lockButtonByOccupy ()
	{
		occupyLabel.text = LanguageConfigManager.Instance.getLanguage (OCCUPY);
		occupyLabel.gameObject.SetActive (true); 
	}

	//满级
	public void lockButtonByFullLevel ()
	{
		occupyLabel.text = LanguageConfigManager.Instance.getLanguage (MAXLEVEL);
		occupyLabel.gameObject.SetActive (true); 
	}

	//祭品
	public void lockButtonByEat ()
	{
		occupyLabel.text = LanguageConfigManager.Instance.getLanguage (EAT);
		occupyLabel.gameObject.SetActive (true); 
	}
	
	public void unlockButton ()
	{
		if (occupyLabel != null)
			occupyLabel.gameObject.SetActive (false);
	}
	
	void resetData ()
	{
		equipImage.gameObject.SetActive (true);
		unlockButton ();
		isChoose = false;
		if (selectPic != null)
			selectPic.gameObject.SetActive (false);
		
		level.text = "0";
	}

	public void updateButton (Equip _equip)
	{
		 
		if (_equip == null) {
			cleanBtton ();
			return;
		} else {
			resetData ();
			equip = _equip;
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + equip.getIconId (), equipImage);
			qualityBack.spriteName = QualityManagerment.qualityIDToIconSpriteName (equip.getQualityId ());
			level.text = "Lv." + equip.getLevel ();
			if(equip.equpStarState > 0 && starLvel != null){
				starLvel.gameObject.SetActive(true);
				starLvel.text = "+" + equip.equpStarState;
			}else{
				starLvel.gameObject.SetActive(false);
			}
			
			if (isShower)
				return;

			if (win.hasMainEquip () == false && isEat ()) {
				lockButtonByEat ();
				return;
			}
			
			if (win.hasMainEquip () == false && isFullLevel () == true) {
				lockButtonByFullLevel ();
				return;
			}
			
			if (win.hasMainEquip () && isOccupy () == true && win.main.equip.uid != equip.uid) {
				lockButtonByOccupy ();
				return;
			}
			//下面是做选中标记
			
			if (equip != null && win.main .equip != null && equip.uid == win.main.equip.uid) {
				setShower (win.main, MAINSELECT);
				isChoose = true;
				return;
			}
			
			ButtonIntensifyEquip button = win.isOneOfTheCaster (equip.uid);
			if (button != null) {
				setShower (button, CASTSELECT);
				isChoose = true;
				return;
			}

		}
	}
	
 
}
