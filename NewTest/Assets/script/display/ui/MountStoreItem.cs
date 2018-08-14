using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// 坐骑兑换信息条目
/// </summary>
public class MountStoreItem : MonoBase {

	/* gameobj filed */
	/*const */
	public const int IS_CAN_STOP=1,
	IS_CAN_RIDE=0,
	IS_CAN_ACTIVE=2,
	IS_CAN_UNACTIVE=3;
	/**条件label 5个 */
	public UILabel[] conditions;
	/** 条件label 挂接点*/
	public GameObject conditionPoint;
	/** 坐骑名字*/
	public UILabel name;
	/**战斗力 */
	public UILabel fight;
	/**坐骑头像预制体 就用着显示*/
	public RoleView role;
	/**坐骑头像 */
	public UITexture icon;
	/**坐骑头像背景 */
	public UISprite iconBg;
	/**其他条件显示 */
	public UILabel otherCondition;
	/**物品显示 */
	public GoodsView goodsView;
	/**被动技能 */
	public ButtonSkill[] passiveSkill;
	/**技能显示节点 */
	public GameObject skillPoint;
	/** 功能按钮  */
	public ButtonMountStoreResult storeButton;
	/** 条目兑换模板 */
	private ExchangeSample temp;
	/**如果有兑换物品 这里保存兑换物品的Sid */
	private int propSid;
	/**坐骑状态标示 0可骑乘 1可休息 2可激活 3 不可激活 */
	private int stateType = 3;
	/**是否显示坐骑的剩余时间 */
	public UILabel timeLabel;
	/**是否显示坐骑的剩余时间挂节点 */
	public GameObject timeObject;
	private Timer timer;
	private Mounts selsetMounts;

	/*method */
	/** 更新条目 */
	public void updateItem(Exchange exchange,WindowBase win) {
		initFather(win);
		role.onClickCallback=rideHander;
		temp = exchange.getExchangeSample ();
		Mounts activeMounts=MountsManagerment.Instance.getMountsBySid(temp.exchangeSid);
		if(activeMounts!=null) {
			storeButton.gameObject.SetActive(true);
			if(activeMounts.isInUse()) {
				storeButton.UpdateResultButton(activeMounts,Bgeneralscrollview.ButtonStateType.stop);
				stateType=IS_CAN_STOP;
			} else {
				storeButton.UpdateResultButton(activeMounts,Bgeneralscrollview.ButtonStateType.ride);
				stateType=IS_CAN_RIDE;
			}
			updateRide(activeMounts);
			storeButton.disableButton(false);
		} else {
			Mounts exchangeMounts=MountsManagerment.Instance.createMounts(temp.exchangeSid);
			ExchangeManagerment exchangeManager=ExchangeManagerment.Instance;
			if(MountsSampleManager.Instance.getMountsSampleBySid(temp.exchangeSid).isShowTime){
				storeButton.gameObject.SetActive(false);
				updateActive(exchangeMounts);
			}else{
				//这里开始检查兑换条件 优先检查VIP
				if(exchangeManager.checkConditionByAll(exchange)){
					storeButton.UpdateResultButton(exchangeMounts,Bgeneralscrollview.ButtonStateType.acitve);
					updateActive(exchangeMounts);
					storeButton.disableButton(false);
					stateType=IS_CAN_ACTIVE;
				} else {
					storeButton.UpdateResultButton(exchangeMounts,Bgeneralscrollview.ButtonStateType.acitve);
					updateActive(exchangeMounts);
					storeButton.disableButton(true);
					stateType=IS_CAN_UNACTIVE;
				}
			}
			updateSkillInfo(null);

		}
	}
	/// <summary>
	/// 给所有有用的button设置父类
	/// </summary>
	public void initFather(WindowBase win){
		storeButton.fatherWindow=win;
		goodsView.fatherWindow=win;
		role.fatherWindow=win;
		for(int i=0;i<passiveSkill.Length;i++){
			passiveSkill[i].fatherWindow=win;
		}
	}
	/** 更新骑状态面板 有技能显示等等 */
	private void updateRide(Mounts mounts) {
		selsetMounts=mounts;
		updateComon(mounts);
		conditionPoint.SetActive(false);
		skillPoint.SetActive(true);
		updateSkillInfo(mounts);
	}
	/** 点击坐骑头像 */
	private void rideHander(RoleView obj) {
		UiManager.Instance.openWindow<MountShowWindow> ((win) => {
			win.init(temp.exchangeSid,stateType);
		});
	}
	/** 更新激活装备面板 有条件显示 */
	private void updateActive (Mounts mounts) {
		updateComon (mounts);
		conditionPoint.SetActive (true);
		cleanAllLabel ();
		writeInLabel ();//描述数据写入label
		updateLabel ();//看看有没有可以变黑的
	}
	/**重置所有label */
	private void cleanAllLabel () {
		for (int i=0; i<conditions.Length; i++) {
			conditions [i].text = "";
		}
	}
	/**把不必要的条件置灰 */
	private void updateLabel() {
		//先拿普通兑换前置的描述
		ExchangeManagerment exchangeManagerment=ExchangeManagerment.Instance;
		string[] strrr=exchangeManagerment.getAllPremises(temp,0);
		string[] str1=exchangeManagerment.getConditionsDesc(temp,0);
		string[] str=new string[strrr.Length];
		for(int ss=0;ss<strrr.Length;ss++){
			if(strrr[ss].StartsWith(Colors.GREEN)){
				str[ss]=Colors.GRENN+strrr[ss].Substring(8);
			}else if(strrr[ss].StartsWith(Colors.RED)){
				str[ss]=Colors.REDD+strrr[ss].Substring(8);
			}else {
				str[ss]=strrr[ss];
			}
			
		}
		int x=0,y=0,xx=0,yy=0;
		xx=str.Length+str1.Length;
		for(int i=0;i<str.Length;i++){
			if(!str[i].EndsWith(LanguageConfigManager.Instance.getLanguage("s0099"))){
				x++;
			}
		}
		for(int i=0;i<str1.Length;i++){
			if(str1[i].EndsWith(LanguageConfigManager.Instance.getLanguage("s0098"))){
				x++;
			}
		}
		if(temp.premises.Length>1){
			string[] str22=exchangeManagerment.getAllPremises(temp,1);
			string[] str2=new string[str22.Length];
			for(int ss=0;ss<str22.Length;ss++){
				if(str22[ss].StartsWith(Colors.GREEN)){
					str2[ss]=Colors.GRENN+str22[ss].Substring(8);
				}else if(strrr[ss].StartsWith(Colors.RED)){
					str2[ss]=Colors.REDD+str22[ss].Substring(8);
				}else {
					str2[ss]=str22[ss];
				}
			}

			for(int i=0;i<str2.Length;i++){
				if(str2[i].StartsWith(Colors.GRENN))
					y++;
			}
			yy+=str2.Length;
		}
		if(temp.conditions.Length>1){
			string[] str3=exchangeManagerment.getConditionsDesc(temp,1);
			yy+=str3.Length;
			for(int i=0;i<str3.Length;i++){
				if(str3[i].StartsWith(Colors.GRENN))y++;
			}
		}
		//优先把非VIP变黑
		if(yy==y&&yy!=0){
			for(int i=0;i<xx;i++){
				string st=conditions[i].text;
				if(st.StartsWith(Colors.GRENN)||st.StartsWith(Colors.REDD))st=st.Split(']')[1];
				if(st.StartsWith("[u]"+Colors.REDD)){
					st=st.Split(new string[]{Colors.REDD},StringSplitOptions.None)[1];
				}
				if(st.StartsWith("[u]"+Colors.GRENN)){
					st=st.Split(new string[]{Colors.GRENN},StringSplitOptions.None)[1];
				}
				st="[s][808080]"+st;
				conditions[i].text=st;
			}
		}
		else if(xx==x){
			for(int i=0;i<yy;i++){
				string st=conditions[i+xx].text;
				if(st.StartsWith(Colors.GRENN)||st.StartsWith(Colors.REDD))st=st.Split(']')[1];
				st="[s][808080]"+st;
				conditions[i+xx].text=st;
			}
		}

	}
	/// <summary>
	/// 描述数据写入label
	/// </summary>
	private void writeInLabel(){
		otherCondition.gameObject.SetActive(false);
		ExchangeManagerment exchangeManagerment=ExchangeManagerment.Instance;
		//先拿普通兑换前置的描述
		string[] strrr=exchangeManagerment.getAllPremises(temp,0);
		string[] str=new string[strrr.Length];
		for(int ss=0;ss<strrr.Length;ss++){
			if(strrr[ss].StartsWith(Colors.GREEN)){
				str[ss]=Colors.GRENN+strrr[ss].Substring(8);
			}else if(strrr[ss].StartsWith(Colors.RED)){
				str[ss]=Colors.REDD+strrr[ss].Substring(8);
			}else {
				str[ss]=strrr[ss];
			}
			
		}
		int k=0;
		for(int i=0;i<str.Length;i++) {
			conditions[k].text=str[i];
			if(conditions[k].text.EndsWith(LanguageConfigManager.Instance.getLanguage("s0099")))conditions[k].text=Colors.REDD+conditions[k].text;
			k++;
		}
		//在拿普通兑换的条件描述
		string[] str1=exchangeManagerment.getConditionsDesc(temp,0);
		for(int i=0;i<str1.Length;i++) {
			if(str1[i]!=""){
				if(str1[i].StartsWith("[u")){
					//str1[i]="[u]"+str1[i].Split('^')[0];
					goodsView.init(temp.conditions[0][i].costType,temp.conditions[0][i].costSid,1);
					BoxCollider boxCollider =goodsView.GetComponent<BoxCollider>();
					boxCollider.center = new Vector3 (conditions[k].transform.localPosition.x-25f,conditions[k].transform.localPosition.y+51f,boxCollider.center.z);
					goodsView.setCountActive(false);
					goodsView.icon.gameObject.SetActive(false);
				}
				conditions[k].text=str1[i];
				k++;
			}
		}
		//拿VIP前置条件
		if(temp.premises.Length>1){
			string[] str22=exchangeManagerment.getAllPremises(temp,1);
			string[] str2=new string[str22.Length];
			for(int ss=0;ss<str22.Length;ss++){
				if(str22[ss].StartsWith(Colors.GREEN)){
					str2[ss]=Colors.GRENN+str22[ss].Substring(8);
				}else if(strrr[ss].StartsWith(Colors.RED)){
					str2[ss]=Colors.REDD+str22[ss].Substring(8);
				}else {
					str2[ss]=str22[ss];
				}
			}
			for(int i=0;i<str2.Length;i++) {
				conditions[k].text=str2[i];
				if(!otherCondition.gameObject.activeSelf){
					otherCondition.gameObject.SetActive(false);
					otherCondition.transform.localPosition=conditions[k].transform.localPosition+new Vector3(-11f,0f,0f);
				}
                if (conditions[k].text.EndsWith(LanguageConfigManager.Instance.getLanguage("s0099")))
                    conditions[k].text = Colors.REDD + conditions[k].text.Substring(0, conditions[k].text.Length - 5);
                k++;
			}
		}
		if(temp.conditions.Length>1){
			string[] str3=exchangeManagerment.getConditionsDesc(temp,1);
			for(int i=0;i<str3.Length;i++) {
				if(str3[i]!=""){
					if(str3[i].StartsWith("[u]")){
						//str3[i]="[u]"+str3[i].Split('^')[1];
						goodsView.init(temp.conditions[1][i].costType,temp.conditions[1][i].costSid,1);
						BoxCollider boxCollider =goodsView.GetComponent<BoxCollider>();
						boxCollider.center = new Vector3 (conditions[k].transform.localPosition.x-25f,conditions[k].transform.localPosition.y+51f,boxCollider.center.z);
						goodsView.setCountActive(false);
						goodsView.icon.gameObject.SetActive(false);
					}
					conditions[k].text=str1[i];
					k++;
				}
			}
		}
	}
	/// <summary>
	/// 打开物品窗口
	/// </summary>
	public void openPropInfoWindow(GameObject obj){
		Prop prop = PropManagerment.Instance.createProp(propSid);
		if(prop!=null){
			UiManager.Instance.openDialogWindow <PropAttrWindow> (
				(winProp) => {
				winProp.Initialize (prop);
			});
		}
		MaskWindow.UnlockUI();
	}
	/** 公共信息的更新 */
	private void updateComon(Mounts mounts) {
		name.text=temp.getExhangeItemName();
		if(mounts!=null) {
			fight.text=mounts.getCombat ().ToString();
		}
		MountsSample mountsSample=MountsSampleManager.Instance.getMountsSampleBySid (temp.exchangeSid);
		Mounts activeMounts=MountsManagerment.Instance.getMountsBySid(temp.exchangeSid);
		if(mountsSample.isShowTime&&activeMounts!=null){
			timeObject.SetActive(true);
		}else {
			timeObject.SetActive(false);
		}
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH +  mountsSample.imageID.ToString (), icon);
		iconBg.spriteName = QualityManagerment.qualityIDToIconSpriteName (mountsSample.qualityId);
	}
	void Update ()
	{
		if(timeObject.activeInHierarchy){
			int time=selsetMounts.getMountsCloseTime()- ServerTimeKit.getSecondTime();
			timeLabel.text=TimeKit.timeTransformDHMS((double)time);
		}
	}
	/** 更新技能信息 */
	private void updateSkillInfo(Mounts mounts) {
		if(mounts==null){
			skillPoint.SetActive(false);
			return;
		}
		Skill[] mainSkills=mounts.getSkills();
		for(int j=0;j<passiveSkill.Length;j++) {
			passiveSkill[j].gameObject.SetActive(false);
		}
		if (mainSkills != null&&mainSkills.Length>0) {	
			for(int i=0;i<mainSkills.Length;i++) {
				passiveSkill[i].gameObject.SetActive(true);
				passiveSkill[i].initSkillData (mainSkills [i], ButtonSkill.STATE_CANLEARN);
				passiveSkill[i].expbar.gameObject.SetActive(false);
			}
		}
	}
}
