using UnityEngine;
using System.Collections;

/// <summary>
/// 星魂主窗口
/// </summary>
public class SoulHuntWindow : WindowBase {

	/* const */
	/** 容器下标常量 */
	const int TAP_HUNT_CONTENT=0, // 猎魂
					TAP_EQUIP_CONTENT=1, // 装备
					TAP_STORE_CONTENT=2, // 魂库
					TAP_MAKE_CONTENT=3; // 
		
	/* gameobj fields */
	/** 预制件容器数组-猎魂容器,装备容器,星魂仓库,重铸星魂 */
	public GameObject[] contentPrefabs;
	public static int ALONE_TEAM=5;
	/** tap容器 */
	public TapContentBase tapContent;
	/** 容器数组-猎魂容器,装备容器,星魂仓库,重铸星魂 */
	public GameObject[] contents;

	public int integral=0;
	public int rank;
	//**限时猎魂sid*/
	public int hountSid=0;
	private int selectIndex=0;//默认选择的星魂队伍 如果是单独显示就是5吧
	/* fields */
	/** 当前tap下标--0开始 */
	int currentTapIndex;
	private Card card;

	/* methods */
	public override void OnAwake () {
		base.OnAwake ();
		EventDelegate.Add (onDestroy, StarSoulManager.Instance.cleanDic);
	}
	protected override void DoEnable () {
		//UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
		UiManager.Instance.backGround.switchBackGround("intensifyHallowsBg");
	}
	/// <summary>
	/// 断线重连
	/// </summary>
	public override void OnNetResume () {
		base.OnNetResume ();
		StarSoulManager.Instance.delSoulStarState(OnNet);
		UpdateContent();
	}
	public void OnNet()
	{
		string str= "";
		StarSoul starSoul = StarSoulManager.Instance.soul;
		if(starSoul==null)return;
		if (StarSoulManager.Instance.getStarSoulDese(starSoul).Split('#').Length > 1) {
			str = StarSoulManager.Instance.getStarSoulDese(starSoul).Split('#')[0] + StarSoulManager.Instance.getStarSoulDese(starSoul).Split('#')[1];
		} else {
			str = StarSoulManager.Instance.getStarSoulDese(starSoul).Split('#')[0];
		}
		UiManager.Instance.openDialogWindow<MessageLineWindow> ((winn) => {
			winn.Initialize (LanguageConfigManager.Instance.getLanguage
			                 ("StarSoulStrengWindow_LOSE", str));
			winn.dialogCloseUnlockUI=false;
		});
	}
	/**  */
	public void init(int tapIndex) {
		this.currentTapIndex = tapIndex-1;
	}
	public void init(int index,Card card) {
		this.currentTapIndex = 1;
		this.selectIndex=index;
		this.card=card;
	}
	/** begin */
	protected override void begin () {
		base.begin ();
		if (StarSoulManager.Instance.getStarSoulInfo () == null) {
			// 与服务器通讯
			(FPortManager.Instance.getFPort ("StarSoulFPort") as StarSoulFPort).getStarSoulInfoAccess (doBegin);
		}
		else {
			doBegin();
		}
        if (selectIndex != 0 && selectIndex != CardBookWindow.INTOTEAM && selectIndex != CardBookWindow.CARDCHANGE &&selectIndex != CardBookWindow.VIEW)
        {
            tapContent.tapButtonList[0].disableButton(true);
            tapContent.tapButtonList[2].disableButton(true);
        }
	}
	/** 执行begin */
	private void doBegin() {
		if(!isAwakeformHide) {
			tapContent.changeTapPage(tapContent.tapButtonList[currentTapIndex]);
			StartCoroutine (initContent(currentTapIndex));
		} else{
			UpdateContent();
			GuideManager.Instance.guideEvent();
			MaskWindow.UnlockUI ();
		}
	}
	/** 更新节点容器 */
	public void UpdateContent() {
		GameObject content = getContent (currentTapIndex);
		if (currentTapIndex == TAP_HUNT_CONTENT) {
			//StarSoulHuntContent sshc = content.GetComponent<StarSoulHuntContent> ();
			SoulHuntContent sshc = content.GetComponent<SoulHuntContent> ();
			sshc.nebulaPanel.gameObject.transform.localScale=new Vector3(1f,1f,1f);
			sshc.integral = integral;
			sshc.rank = rank;
			sshc.hountSid = hountSid;
			sshc.UpdateUI();
		} else if (currentTapIndex == TAP_EQUIP_CONTENT) {
			StarSoulEquipContent ssec = content.GetComponent<StarSoulEquipContent> ();
			ssec.updateUI();
		} else if (currentTapIndex == TAP_STORE_CONTENT) {
			StarSoulStoreContent sssc = content.GetComponent<StarSoulStoreContent> ();
			sssc.UpdateUI();
		} else if (currentTapIndex == TAP_MAKE_CONTENT) {
			StarSoulMakeContent ssmc = content.GetComponent<StarSoulMakeContent> ();
		}
	}
	/** 重置容器激活状态 */
	private void resetContentsActive() {
		foreach (GameObject item in contents) {
			item.SetActive(false);
		}
	}
	/** 初始化容器 */
	private IEnumerator initContent(int tapIndex) {
//		GameObject lastPoint = contents [this.currentTapIndex];
//		if(lastPoint.transform.childCount>0)
//			Utils.RemoveAllChild (lastPoint.transform);
		resetContentsActive ();
		GameObject content = getContent (tapIndex);
		switch (tapIndex) {
			case TAP_HUNT_CONTENT:
				//StarSoulHuntContent ssh = content.GetComponent<StarSoulHuntContent> ();
				SoulHuntContent ssh = content.GetComponent<SoulHuntContent> ();
				//ssh.init(this,PlayerPrefs.GetInt (UserManager.Instance.self.uid + PlayerPrefsComm.STARSOUL_HUNT_TAP));
				ssh.integral = integral;
				ssh.rank = rank;
				ssh.init(this,1);
                ssh.updateNebulaEffectUI();//更新星魂特效视图
				break;
			case TAP_EQUIP_CONTENT: 
				StarSoulEquipContent ec = content.GetComponent<StarSoulEquipContent> ();
				ec.init(this,selectIndex,card);
				break;
			case TAP_STORE_CONTENT: 
				StarSoulStoreContent ssc = content.GetComponent<StarSoulStoreContent> ();
				ssc.init(this,ButtonStoreStarSoul.ButtonStateType.Power);
				break;
			case TAP_MAKE_CONTENT: 
				StarSoulMakeContent msc = content.GetComponent<StarSoulMakeContent> ();
				msc.init();
				break;
		}
		GuideManager.Instance.guideEvent();
		MaskWindow.UnlockUI ();
		yield break;
	}
	/// <summary>
	/// 获取指定下标的容器
	/// </summary>
	/// <param name="contentPoint">容器点</param>
	/// <param name="tapIndex">下标</param>
	private GameObject getContent(int tapIndex) {
		GameObject contentPoint = contents [tapIndex];
		contentPoint.SetActive (true);
		GameObject content;
		if (contentPoint.transform.childCount > 0) {
			Transform childContent=contentPoint.transform.GetChild (0);
			content = childContent.gameObject;
		} else {
			content = NGUITools.AddChild (contentPoint, contentPrefabs [tapIndex]);
		}
		return content;
	}
	/** tap点击事件 */
	public override void tapButtonEventBase (GameObject gameObj, bool enable) {
		if (!enable)
			return;
		base.tapButtonEventBase (gameObj,enable);
		if (gameObj.name.StartsWith ("HuntTap")) {
			if(currentTapIndex==TAP_HUNT_CONTENT) {
				GameObject content = getContent (TAP_HUNT_CONTENT);
				SoulHuntContent sc = content.GetComponent<SoulHuntContent> ();
				sc.tapButtonEventBase(gameObj);
			}
		} else {
			int tapIndex=int.Parse (gameObj.name)-1;
			StartCoroutine (initContent (tapIndex));
			this.currentTapIndex=tapIndex;
		}
	}
	/** button点击事件 */
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			//刷新限时猎魂
			if(LuckyEquipContent.needReload == 1)
				LuckyEquipContent.needReload =2;
			StorageManagerment.Instance.clearAllStarSoulNew();
			finishWindow ();
		} else {
			GameObject content = getContent (currentTapIndex);
			if (currentTapIndex == TAP_HUNT_CONTENT) {
				//StarSoulHuntContent sshc = content.GetComponent<StarSoulHuntContent> ();
				SoulHuntContent sshc = content.GetComponent<SoulHuntContent> ();
				sshc.hountSid = hountSid;
				sshc.buttonEventBase(gameObj);
			} else if (currentTapIndex == TAP_EQUIP_CONTENT) {
				StarSoulEquipContent ssec = content.GetComponent<StarSoulEquipContent> ();
				ssec.buttonEventBase(gameObj);
			} else if (currentTapIndex == TAP_STORE_CONTENT) {
				StarSoulStoreContent sssc = content.GetComponent<StarSoulStoreContent> ();
				sssc.buttonEventBase(gameObj);
			} else if (currentTapIndex == TAP_MAKE_CONTENT) {
				StarSoulMakeContent ssmc = content.GetComponent<StarSoulMakeContent> ();
				ssmc.buttonEventBase(gameObj);
			}
		}
	}
}