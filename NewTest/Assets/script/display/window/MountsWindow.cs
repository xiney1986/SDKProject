using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 坐骑窗口
/// </summary>
public class MountsWindow : WindowBase {
	
	/* const */
	/** 容器下标常量 */
	public const int TAP_ATTR_CONTENT=0, // 坐骑属性
						TAP_STORE_CONTENT=1, // 坐骑仓库
						TAP_SKILL_CONTENT=2; // 坐骑特技

	/* fields */
	/** 预制件容器数组-坐骑容器,坐骑仓库容器,特技容器 */
	public GameObject[] contentPrefabs;
	/** tap容器 */
	public TapContentBase tapContent;
	/** 容器数组-坐骑容器,坐骑仓库容器,特技容器 */
	public GameObject[] contents;
	/** 当前tap下标--0开始 */
	public int currentTapIndex;
	/**是否不显示正在骑乘坐骑的标示*/
	public int buttonType;
	/* methods */
	/***/
	protected override void DoEnable () {
		base.DoEnable ();
//		UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
	}
	/** 断线重链 */
	public override void OnNetResume () {
		base.OnNetResume ();
		UpdateContent();
	}
	/** 初始化 */
	public void init(int tapIndex) {
		this.currentTapIndex = tapIndex-1;
	}
	/** 改变Tap */
	public void changeTapPage(int tapIndex) {
		if(tapIndex>tapContent.tapButtonList.Length-1)
			return;
		tapContent.resetTap ();
		tapContent.changeTapPage (tapContent.tapButtonList [tapIndex]);
	}
	/** begin */
	protected override void begin () {
		base.begin ();
		cacheModel();
	}
	void cacheModel () {
		string[] _list = new string[]{	
			"mission/ez",
			"mission/girl",
			"mission/mage",
			"mission/maleMage",
			"mission/archer",
			"mission/swordsman",
		};
//		string[] _list2 = new string[_list.Length + MountsResourceManager.Instance.GetPaths().Length];
//		_list.CopyTo(_list2,0);
//		MountsResourceManager.Instance.GetPaths().CopyTo(_list,_list.Length);
		ResourcesManager.Instance.cacheData (_list, (list) => {doBegin ();}, "other");
	}
	private void doBegin() {
		if(!isAwakeformHide) {
			tapContent.changeTapPage(tapContent.tapButtonList[currentTapIndex]);
		} else {
			UpdateContent();
		}
		GuideManager.Instance.guideEvent();
		MaskWindow.UnlockUI ();
	}
	/** 更新节点容器 */
	public void UpdateContent() {
		GameObject content = getContent (currentTapIndex);
		if (currentTapIndex == TAP_ATTR_CONTENT) {
			MountsAttrContent mac = content.GetComponent<MountsAttrContent> ();
			mac.UpdateUI();
		} else if (currentTapIndex == TAP_STORE_CONTENT) {
			MountStoreContent msc = content.GetComponent<MountStoreContent> ();
			msc.UpdateUI();
		} else if (currentTapIndex == TAP_SKILL_CONTENT) {
			MountLifeSkillContent mlsc = content.GetComponent<MountLifeSkillContent> ();
			mlsc.UpdateUI();
		}
	}
	public void changeTapContent(){
		tapContent.changeTapPage(tapContent.tapButtonList[currentTapIndex]);
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
	/** 重置容器激活状态 */
	private void resetContentsActive() {
		foreach (GameObject item in contents) {
			item.SetActive(false);
		}
	}
	/** 初始化容器 */
	private void initContent(int tapIndex) {
		// 消耗大就删除掉老节点
		//		GameObject lastPoint = contents [this.currentTapIndex];
		//		if(lastPoint.transform.childCount>0)
		//			Utils.RemoveAllChild (lastPoint.transform);
		resetContentsActive ();
		GameObject content = getContent (tapIndex);
		switch (tapIndex) {
			case TAP_ATTR_CONTENT:
				MountsAttrContent mac = content.GetComponent<MountsAttrContent> ();
				mac.init(this);
				break;
			case TAP_STORE_CONTENT: 
				MountStoreContent msc = content.GetComponent<MountStoreContent> ();
				msc.init(this,buttonType);
				break;
			case TAP_SKILL_CONTENT:
				MountLifeSkillContent mlsc = content.GetComponent<MountLifeSkillContent> ();
				mlsc.init(this);
				break;
		}
	}
	/** tap 事件 */
	public override void tapButtonEventBase (GameObject gameObj, bool enable) {
		if (!enable)
			return;
		base.tapButtonEventBase (gameObj,enable);
		int tapIndex=int.Parse (gameObj.name)-1;
		initContent (tapIndex);
		this.currentTapIndex=tapIndex;
	}
	/** 点击事件 */
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
		} else {
			GameObject content = getContent (currentTapIndex);
			if (currentTapIndex == TAP_ATTR_CONTENT) {
				MountsAttrContent mac = content.GetComponent<MountsAttrContent> ();
				mac.buttonEventBase(gameObj);
			} else if (currentTapIndex == TAP_STORE_CONTENT) {
				MountStoreContent msc = content.GetComponent<MountStoreContent> ();
				msc.buttonEventBase(gameObj);
			} else if (currentTapIndex == TAP_SKILL_CONTENT) {
				MountLifeSkillContent mlsc = content.GetComponent<MountLifeSkillContent> ();
				mlsc.buttonEventBase(gameObj);
			}
		}
	}
	/// <summary>
	/// 设置进入仓库是否显示正在骑乘的坐骑标示
	/// </summary>
	/// <param name="type">Type.</param>
	public void setButtonState(int type){
		buttonType=type;
	}
}