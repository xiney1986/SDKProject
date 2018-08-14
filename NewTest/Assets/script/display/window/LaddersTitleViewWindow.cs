using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 天梯中称号 一览
/// </summary>
public class LaddersTitleViewWindow : WindowBase
{
	public GameObject titleViewPrefab;
	public DelegatedynamicContent  content;
	public GameObject filpArrow;//下翻页箭头指示
	public List<LaddersTitleSample> samples;
	int userTitleIndex = 0;

	public override void OnStart ()
	{
		base.OnStart ();
		titleViewPrefab.SetActive (false);
	}

	protected override void begin ()
	{
		base.begin ();
		samples = LaddersConfigManager.Instance.config_Title.M_getTitles ();
		content.SetUpdateItemCallback (onUpdateItem);
		content.SetinitCallback (initItem);
		updateData ();
	}
	/// <summary>
	/// 断线重连
	/// </summary>
	public override void OnNetResume ()
	{
		base.OnNetResume ();
		updateData ();
	}
	/// <summary>
	/// 更新数据
	/// </summary>
	void updateData ()
	{
		if (samples.Count >= 4) {
			filpArrow.SetActive (true);
		} else {
			filpArrow.SetActive (false);		
		}

		//根据声望获得对应的称号位置
		int userP = UserManager.Instance.self.prestige;
		userTitleIndex = LaddersConfigManager.Instance.config_Title.M_getTitleIndex (userP);


		content.reLoad (samples.Count, userTitleIndex);
		MaskWindow.UnlockUI ();
	}
	/// <summary>
	/// DelegatedynamicContent  中回调的更新单个项 的数据
	/// </summary>
	/// <returns>The update item.</returns>
	/// <param name="item">Item.</param>
	/// <param name="i">The index.</param>
	GameObject onUpdateItem (GameObject item, int i)
	{
		if (item == null) {
			item = NGUITools.AddChild (content.gameObject, titleViewPrefab);
			item.SetActive (true);
		}
		ButtonTitleView button = item.GetComponent<ButtonTitleView> ();
		button.fatherWindow = this;
		button.updateButton (samples [i]);
		return item;
	}

	GameObject initItem (int i)
	{

		GameObject item = NGUITools.AddChild (content.gameObject, titleViewPrefab);
		item.SetActive (true);
		ButtonTitleView button = item.GetComponent<ButtonTitleView> ();
		button.fatherWindow = this;
		button.updateButton (samples [i]);	
		return item;
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);

		if (gameObj.name == "close" || gameObj.name == "btn_back") { 
			finishWindow ();
		} 
	}

}
