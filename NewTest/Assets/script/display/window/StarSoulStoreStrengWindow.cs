using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

/// <summary>
/// 星魂强化
/// </summary>
public class StarSoulStoreStrengWindow : WindowBase {

	/**field**/
	/**星魂图标预制件 */
	public GameObject starSoulViewsPerfab;
	/**强化特效预制件 */
	public GameObject strengEffectPerfab;
	/**储备的魂经验界面 */
	public UILabel reserveExp ;
	/**星魂图标点  */
	public GameObject starSoulIconPoint;
	/**强化特效点  */
	public GameObject strengEffectPoint;
	/**提升一级所需要的经验 */
	public UILabel oneLvExp;
	public UILabel allLVExp;
	/**一键充入按钮 */
	public ButtonBase allUpButton;
	/**提升一级按钮 */
	public ButtonBase oneUpButton;
	/** 星魂经验条 */
	public ExpbarCtrl expBar;
	/** 经验文本 */
	public UILabel expLabel;
	/**当前星魂的等级和名称 */
	public UILabel starNameLV;
	/**当前星魂的属性 */
	public UILabel starInfo;
    public UILabel starInfo1;
	/** 当前的强化的星魂 */
	private StarSoul starSoul;
	/** 老的强化星魂 */
	private StarSoul oldStarSoul;
	/** 等级经验升级信息 */
	private LevelupInfo levelupInfo;
	public UILabel[] addLabels;
	public  UILabel[] incLabels;
	private int[] incNumm=new int[3];

	public string indetify_uid;
	public long exp; 

	/* methods */
	/** 激活window */
	protected override void begin () {
		base.begin ();
		MaskWindow.UnlockUI ();
	}

	/// <summary>
	/// 初始化星魂强化界面
	/// </summary>
	public void init(StarSoul starSoul) {
		this.starSoul=starSoul;
		updateUI ();
	}
	/** 更新UI */
	public void updateUI() {
		updateLabel();
		updateStarSoulView ();
		reserveExp.text = StarSoulManager.Instance.getStarSoulExp ().ToString ();
		long needExpforOne = starSoul.getEXPUp () - starSoul.getEXP ();
		long needExpforAll = starSoul.getMaxExp () - starSoul.getEXP ();
		oneLvExp.text = LanguageConfigManager.Instance.getLanguage ("StarSoulStrengWindow_DecThree", needExpforOne.ToString ());
		allLVExp.text=LanguageConfigManager.Instance.getLanguage ("StarSoulStrengWindow_DecFour", needExpforAll.ToString ());
		expLabel.text= EXPSampleManager.Instance.getExpBarShow (starSoul.getEXPSid (), starSoul.getEXP ());
		starNameLV.text =starSoul.getName () + " Lv." + starSoul.getLevel ();
        string[] str = starSoul.getDescribe().Split('#');
        if (str.Length > 1) {
            starInfo1.text = str[0].Split('+')[0] + "[3A9663] +" + str[0].Split('+')[1];
            starInfo.text = str[1].Split('+')[0] + "[3A9663] +" + str[1].Split('+')[1];
        } else {
            starInfo1.text = str[0].Split('+')[0] + "[3A9663] +" + str[0].Split('+')[1];
        }
		//starInfo.text = starSoul.getDescribe ().Split('+')[0]+"[3A9663] +"+starSoul.getDescribe().Split('+')[1];
		long storeExp=StarSoulManager.Instance.getStarSoulExp ();
		if (storeExp == 0||starSoul.isMaxLevel()) {
			allUpButton.disableButton(true);
			oneUpButton.disableButton(true);
		} else {
			// 升一级需要的经验
			long needExp=starSoul.getEXPUp()-starSoul.getEXP();
			if(storeExp<needExp){
				oneUpButton.disableButton(true);
			}
			else {
				oneUpButton.disableButton(false);
			}
			allUpButton.disableButton(false);
		}
	}
	/// <summary>
	/// 经验条飞
	/// </summary>
	private void updateExpBar(){
		LevelupInfo levelupInfo;
		if (oldStarSoul == null) {
			levelupInfo=createLevelupInfo(starSoul,starSoul);
		} else{
			levelupInfo=createLevelupInfo(oldStarSoul,starSoul);
		}
		expLabel.text= EXPSampleManager.Instance.getExpBarShow (starSoul.getEXPSid (), starSoul.getEXP ());
		expBar.init (levelupInfo);
	}
	/// <summary>
	/// 更新星魂形象
	/// </summary>
	private void updateStarSoulView() {
		GameObject obj;
		if (starSoulIconPoint.transform.childCount > 0) {
			obj = starSoulIconPoint.transform.GetChild(0).gameObject;
		}
		else {
			obj = NGUITools.AddChild(starSoulIconPoint,starSoulViewsPerfab);
			obj.transform.localScale=new Vector3(0.85f,0.85f,1);
		}
		GoodsView gv = obj.GetComponent<GoodsView>();
		gv.setFatherWindow (fatherWindow);
		gv.init(starSoul);
	}
	/** 初始化新经验条 */
	private LevelupInfo createLevelupInfo (StarSoul oldStarSoul, StarSoul newStarSoul) {
		LevelupInfo levelupInfo = new LevelupInfo ();
		levelupInfo.newExp = newStarSoul.getEXP();
		levelupInfo.newExpDown = EXPSampleManager.Instance.getEXPDown (newStarSoul.getEXPSid(), newStarSoul.getLevel());
		levelupInfo.newExpUp = EXPSampleManager.Instance.getEXPUp (newStarSoul.getEXPSid(), newStarSoul.getLevel());
		levelupInfo.newLevel = newStarSoul.getLevel();
		levelupInfo.oldExp = oldStarSoul.getEXP();
		levelupInfo.oldExpDown = EXPSampleManager.Instance.getEXPDown (oldStarSoul.getEXPSid(), oldStarSoul.getLevel());
		levelupInfo.oldExpUp = EXPSampleManager.Instance.getEXPUp (oldStarSoul.getEXPSid(), oldStarSoul.getLevel());
		levelupInfo.oldLevel = oldStarSoul.getLevel();
		return levelupInfo;
	}
	/** 点击事件 */
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
		} else if(gameObj.name=="putInButton") { //一键充入
			long storeExp=StarSoulManager.Instance.getStarSoulExp ();
			if(storeExp==0) {
				MaskWindow.UnlockUI();
				return;
			}
			// 升满级需要的经验
			long needExp=starSoul.getMaxExp()-starSoul.getEXP();
			if(storeExp<needExp) needExp=storeExp;
			oldStarSoul=(StarSoul)starSoul.Clone();
			setStarSoulIndefity(starSoul.uid,needExp);
			(FPortManager.Instance.getFPort ("StarSoulEquipFPort") as StarSoulEquipFPort).doStrengStarSoulAccess (starSoul.uid,needExp,doStrengStarSoulFinshed);
		} else if(gameObj.name=="oneUpButton") { //升一级
			long storeExp=StarSoulManager.Instance.getStarSoulExp ();
			if(storeExp==0) {
				MaskWindow.UnlockUI();
				return;
			}
			// 升一级需要的经验
			long needExp=starSoul.getEXPUp()-starSoul.getEXP();
			if(storeExp<needExp) needExp=storeExp;
			oldStarSoul=(StarSoul)starSoul.Clone();
			setStarSoulIndefity(starSoul.uid,needExp);
			(FPortManager.Instance.getFPort ("StarSoulEquipFPort") as StarSoulEquipFPort).doStrengStarSoulAccess (starSoul.uid,needExp,doStrengStarSoulFinshed);
		}
	}
	/// <summary>
	/// 强化完成回调
	/// </summary>
	private void doStrengStarSoulFinshed() {
		if(strengEffectPoint.transform.childCount>0)
			Utils.RemoveAllChild (strengEffectPoint.transform);	
		NGUITools.AddChild(strengEffectPoint,strengEffectPerfab);
		updateExpBar();
		updateIncNum();
		StartCoroutine(Utils.DelayRun(()=>{
			updateUI();
			MaskWindow.UnlockUI();
			oldStarSoul=null;
		},3f));
        StartCoroutine(Utils.DelayRun(() => {
            if (strengEffectPoint.transform.childCount > 0) Utils.RemoveAllChild(strengEffectPoint.transform);
        }, 3f));

	}
	/// <summary>
	/// 飘红字
	/// </summary>
	private void updateIncNum(){
		AttrChangeSample[] oldattrs=oldStarSoul.getAttrChangesByAll();
		AttrChangeSample[] newattrs=starSoul.getAttrChangesByAll();
		string[] oldNum=DescribeManagerment.getDescribeParam(oldStarSoul.getLevel(),oldattrs);
		string[] newNum=DescribeManagerment.getDescribeParam(starSoul.getLevel(),newattrs);
		TweenLabelNumber[] tlns=new TweenLabelNumber[newNum.Length]; 
		for(int i=0;i<newNum.Length;i++){
			incNumm[i]+=StringKit.toInt(newNum[i])-StringKit.toInt(oldNum[i]);
			addLabels[i].gameObject.SetActive(true);
			addLabels[i].transform.localPosition=starInfo1.transform.localPosition+new Vector3(Math.Max(starInfo1.width,starInfo.width)+10f, i*(-20f)+10,0f);
			incLabels[i].gameObject.SetActive(true);
            incLabels[i].transform.localPosition = addLabels[i].transform.localPosition + new Vector3(addLabels[i].width + 10f,(- 3f), 0f);
			if(incNumm[i]>0) {
				tlns[i] = TweenLabelNumber.Begin (incLabels[i].gameObject, 0.5f, incNumm[i]);
				EventDelegate.Add (tlns[i].onFinished, () => {
					incNumm=new int[3];
					addLabels[i].gameObject.SetActive(false);
					incLabels[i].gameObject.SetActive(false);
					incLabels[i].text="0";
				}, true);
			}
		}
	}
	/** 更新数值标签 */
	private void updateLabel(){
		for(int i=0;i<addLabels.Length;i++){
			addLabels[i].gameObject.SetActive(false);
			incLabels[i].gameObject.SetActive(false);
			incLabels[i].text="0";
		}
	}
	public override void OnNetResume ()
	{
		base.OnNetResume ();
		starSoulIndefity();
		doStrengStarSoulFinshed();
	}

	public void setStarSoulIndefity(string uid,long exp)
	{
		this.indetify_uid = uid;
		this.exp = exp;
	}
	
	public void starSoulIndefity()
	{
		StorageManagerment smanager=StorageManagerment.Instance;
		StarSoulManager manager=StarSoulManager.Instance;
		StarSoul starSoul=smanager.getStarSoul(indetify_uid);
		if(starSoul!=null) {
			manager.delStarSoulExp(exp);
			starSoul.updateExp(starSoul.getEXP());
			starSoul.isNew=false;
		}
		StorageManagerment.Instance.starSoulStorageVersion++;
		init (starSoul);
	}

}
