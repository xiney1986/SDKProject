using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 天梯所有奖章展示窗口
/// </summary>
public class LaddersMedalViewWindow : WindowBase
{
	public GameObject medalViewPrefab;
	public DelegatedynamicContent  content;
	public GameObject filpArrow;//下翻页箭头指示

	public List<LaddersMedalSample> samples;
	int playerMedalIndex=-1;

	public override void OnStart ()
	{
		base.OnStart ();
		medalViewPrefab.SetActive(false);
	}

	protected override void begin ()
	{
		base.begin ();

		samples=LaddersConfigManager.Instance.config_Medal.M_getMedals();
		content.SetUpdateItemCallback (onUpdateItem);
		content.SetinitCallback (initItem);
		updateData();
	}
	void updateData()
	{
		if(samples.Count >=4)
		{
			filpArrow.SetActive(true);
		}
		else
		{
			filpArrow.SetActive(false);		
		}

		//根据sid获得称号的index
		int sid=LaddersManagement.Instance.currentPlayerMedalSid;
		playerMedalIndex=LaddersConfigManager.Instance.config_Medal.M_getMedalIndexBySid(sid);


		content.reLoad(samples.Count,playerMedalIndex<0?0:playerMedalIndex);
		MaskWindow.UnlockUI();
	}
	GameObject onUpdateItem (GameObject item, int i)
	{
		if (item== null){
			item= NGUITools.AddChild (content.gameObject,medalViewPrefab);
			item.SetActive(true);
		}
		ButtonMedalView button = item.GetComponent<ButtonMedalView> ();
		button.fatherWindow =this;
		button.updateButton(samples[i]);
		return item;
	}
	GameObject initItem (int i)
	{

		GameObject	item= NGUITools.AddChild (content.gameObject,medalViewPrefab);
			item.SetActive(true);
		ButtonMedalView button = item.GetComponent<ButtonMedalView> ();
		button.fatherWindow =this;
		button.updateButton(samples[i]);
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
