using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 公会申请窗口
 * @author 汤琦
 * */
public class GuildApplyWindow : WindowBase
{
	/** 申请面板 */
	public SampleDynamicContent dynamicContent;
	// 搜索框
	public UIInput inputFind;
	/** 左箭头 */
	public UISprite leftArrow;
	/** 右箭头 */
	public UISprite rightArrow;
	/** 页数显示 */
	public UILabel pageLabel;
	/** 当前激活的图鉴页 */
	public SysSet IsShowFull;
	/** 向右取的起始位置 */
	private int rightIndex = 1;
	/** 当前显示的公会列表 */
	private List<GuildRankInfo> currentGuildList = new List<GuildRankInfo> ();
	/** 下一显示的公会列表 */
	private List<GuildRankInfo> nextGuildList = new List<GuildRankInfo> ();
	/** 是否取满员的公会 1:不取 2：取 */
	private int  type = 2;
	/** 是否已经取完数据 */
	private bool isMax = false;
	/** 每次取的最大数量 */
	private const int EACH_COUNT = 15;
	/** 每页的数量 */
	private const int PAGE_COUNT = 5;
	protected override void begin ()
	{
		base.begin ();
		if (!isAwakeformHide) {
			dynamicContent.callbackUpdateEach = updatePage;
			dynamicContent.onCenterItem = updateActivePage;
			dynamicContent.callbackRigthFilp = right;
			dynamicContent.transform.parent.gameObject.SetActive (true);
			GuildGetApplyListFPort ();
		}
		MaskWindow.UnlockUI ();
	}


	private void GuildGetApplyListFPort ()
	{
		GuildGetApplyListFPort fport = FPortManager.Instance.getFPort ("GuildGetApplyListFPort") as GuildGetApplyListFPort;
		fport.access (getApplyListCallBack);
	}

	private void getApplyListCallBack ()
	{
		updateWindow (rightIndex);
	}


	private void updateWindow (int start)
	{
		if (isMax)
			return;
		GuildGetListFPort fport = FPortManager.Instance.getFPort ("GuildGetListFPort") as GuildGetListFPort;
		fport.access (start, type, getListCallBack, getIsMaxCallBack);
	}

	private void getListCallBack (int nowIndex)
	{
		if (currentGuildList.Count == 0) {
			currentGuildList.AddRange (GuildManagerment.Instance.getGuildList ());	
			if (currentGuildList.Count != 0) {
				dynamicContent.maxCount = getPageCount ();
				dynamicContent.init ();
				updateWindow (nowIndex);
			}
			else
			{
				UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("Guild_110"));				
			}
		} else {
			nextGuildList.Clear ();
			nextGuildList.AddRange (GuildManagerment.Instance.getGuildList ());
		}
		rightIndex = nowIndex;	
		MaskWindow.UnlockUI ();

	}

	private void getIsMaxCallBack (bool isMax)
	{
		this.isMax = isMax;
	}

	
	/// <summary>
	/// 搜索公会
	/// </summary>
	private void searchGuild (string name)
	{
		GuildSearchGuildFport fport = FPortManager.Instance.getFPort ("GuildSearchGuildFport") as GuildSearchGuildFport;
		fport.searchGuild (name, searchGuildCallBack);
	}


	private void searchGuildCallBack (GuildRankInfo searchResult)
	{
		if (searchResult == null)
			UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("Guild_109", inputFind.value));
		else {
			resetData ();
			currentGuildList.Clear ();
            GuildManagerment.Instance.createGuildList(searchResult);
			currentGuildList.Add (searchResult);
			dynamicContent.maxCount = getPageCount ();
			dynamicContent.init ();
		}
		MaskWindow.UnlockUI ();
	}
	/// <summary>
	/// 向右滑
	/// </summary>
	private void right ()
	{
		currentGuildList.AddRange (nextGuildList);
		nextGuildList.Clear ();
		dynamicContent.maxCount = getPageCount ();
		updateArrow (dynamicContent.getCenterObj ());
		updateWindow (rightIndex);
	}

	/// <summary>
	/// 更新当前页
	/// </summary>
	private void updatePage (GameObject obj)
	{
		GuildApplyPage bookitem = obj.GetComponent<GuildApplyPage> ();
		int index = StringKit.toInt (obj.name) - 1;
		if (getListWithIndex (index) == null)
			return;
		bookitem.updatePage (getListWithIndex (index));
//		pageLabel.text = (index +1).ToString();
	}
	
	
	private void updateActivePage (GameObject obj)
	{
		//更新箭头
		int index = StringKit.toInt (obj.name) - 1;		
		if (getPageCount () == 1) {
			leftArrow.gameObject.SetActive (false);
			rightArrow.gameObject.SetActive (false);
		} else if (index == 0) {
			leftArrow.gameObject.SetActive (false);
			rightArrow.gameObject.SetActive (true);
		} else if (index == getPageCount () - 1) {
			leftArrow.gameObject.SetActive (true);
			rightArrow.gameObject.SetActive (false);
		} else {
			leftArrow.gameObject.SetActive (true);
			rightArrow.gameObject.SetActive (true);
		}
		pageLabel.text = LanguageConfigManager.Instance.getLanguage ("Guild_108", (index + 1).ToString ());
	}

	private void updateArrow (GameObject obj)
	{
		int index = StringKit.toInt (obj.name) - 1;		
		if (getPageCount () == 1) {
			leftArrow.gameObject.SetActive (false);
			rightArrow.gameObject.SetActive (false);
		} else if (index == 0) {
			leftArrow.gameObject.SetActive (false);
			rightArrow.gameObject.SetActive (true);
		} else if (index == getPageCount () - 1) {
			leftArrow.gameObject.SetActive (true);
			rightArrow.gameObject.SetActive (false);
		} else {
			leftArrow.gameObject.SetActive (true);
			rightArrow.gameObject.SetActive (true);
		}
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
		} else if (gameObj.name == "buttonFind") {
			if (inputFind.value.Replace (" ", "") == "" || inputFind.value == null) {
				MaskWindow.UnlockUI();
				return;
			}
			searchGuild (inputFind.value);
		} else if (gameObj.name == "IsShowFull") {
			/** 显示满员公会 */
			if (type == 1) {
				type = 2;
				IsShowFull.choose.gameObject.SetActive (false);
			}
			/** 不显示满员公会 */
			else if (type == 2) {
				type = 1;
				IsShowFull.choose.gameObject.SetActive (true);
			}
			resetData ();
			updateWindow (rightIndex);
		}
	}


	private void resetData ()
	{
		GuildManagerment.Instance.clearGuildList ();
		currentGuildList.Clear ();
		nextGuildList.Clear ();
		rightIndex = 1;
		isMax = false;
	}



	/// <summary>
	/// 根据下标,返回对应的list
	/// </summary>
	private List<GuildRankInfo> getListWithIndex (int index)
	{
		if (index > getPageCount () - 1) {
			return null;
		}
		List<GuildRankInfo> pageList = new List<GuildRankInfo> ();
		int start = index * PAGE_COUNT;
		int end = start + PAGE_COUNT;
		for (int i = start; i<end && i < currentGuildList.Count; i++) {
			pageList.Add (currentGuildList [i]);
		}
		return pageList;
		
		
	}
	/// <summary>
	/// 计算总共的页数
	/// </summary>
	private int getPageCount ()
	{
		if (currentGuildList == null)
			return 0;
		if (currentGuildList.Count % PAGE_COUNT == 0)
			return currentGuildList.Count / PAGE_COUNT;
		else
			return currentGuildList.Count / PAGE_COUNT + 1;
	}

	public override void OnNetResume ()
	{
		base.OnNetResume ();
		resetData ();
		updateWindow (rightIndex);
	}

}
