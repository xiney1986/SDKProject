using UnityEngine;
using System.Collections;
/// <summary>
/// 玩家当前的 奖章&称号 窗口
/// </summary>
public class LaddersMedalWindow : WindowBase
{

	public GameObject prefab_des;
	public GameObject root_title_currentDes;
	public GameObject root_medal_Des;


	public barCtrl bar_title;//称号exp条
	public UILabel label_title;//称号
	public UILabel label_medal_title;//专属奖章
	public UISprite sprite_medal;//奖章图片

	public UILabel label_medal_tip;
	public UILabel label_barProgress;


	public override void OnStart ()
	{
		prefab_des.SetActive(false);
		updateView();
	}

	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();	
	}

	public override void OnNetResume ()
	{
		base.OnNetResume ();
	}

	void updateMedalAndTitle ()
	{/*
		Prestige pr = PrestigeManagerment.Instance.createPrestigeByLevel (100);
		titleLabel.text = pr.prestigeName;
		TitleExpBar.updateValue (pr.expNow, pr.expUp);
		titleEffectLabel.text = pr.effects [0].descript;
		*/
	}
	/// <summary>
	/// 更新视图 包括自己的奖章 和 自己的称号信息
	/// </summary>
	private void updateView()
	{
		int userP=UserManager.Instance.self.prestige;

		LaddersTitleSample currentTitleSample=LaddersConfigManager.Instance.config_Title.M_getTitle(userP);
		label_title.text=currentTitleSample.name;
		updateDes(root_title_currentDes,currentTitleSample.addDescriptions);

		LaddersTitleSample nextTitleSample=LaddersConfigManager.Instance.config_Title.M_getTitleByIndex(currentTitleSample.index+1);
		if(nextTitleSample!=null)
		{
			bar_title.updateValue(userP,nextTitleSample.minPrestige);
			label_barProgress.text=userP+"/"+nextTitleSample.minPrestige;
		}else
		{
			bar_title.updateValue(userP,userP);
			label_barProgress.text=userP+"/"+userP;
		}

		//int userRank=LaddersManagement.Instance.currentPlayerRank;
		int sid=LaddersManagement.Instance.currentPlayerMedalSid;
		LaddersMedalSample medalSample=LaddersConfigManager.Instance.config_Medal.M_getMedalBySid(sid);
		if(medalSample!=null)
		{
			label_medal_title.text=medalSample.name;
			sprite_medal.gameObject.SetActive(true);
			sprite_medal.spriteName="medal_"+Mathf.Min(medalSample.index+1,5);

			label_medal_tip.gameObject.SetActive(false);
			updateDes(root_medal_Des,medalSample.addDescriptions);
		}else
		{
			sprite_medal.gameObject.SetActive(false);
			label_medal_tip.gameObject.SetActive(true);

			label_medal_title.text=Language("laddersTip_09");
			updateDes(root_medal_Des,null);
		}

	}
	/// <summary>
	/// 更新称号加成描述
	/// </summary>
	/// <param name="_parent">_parent.</param>
	/// <param name="_des">_des.</param>
	private void updateDes(GameObject _parent,string[] _des)
	{
		UIUtils.M_removeAllChildren(_parent);
		if(_des==null)
		{
			return;
		}
		GameObject itemDes;
		for(int i=0,length=_des.Length;i<length;i++)
		{
			if(i>5)
			{
				break;
			}
			itemDes=NGUITools.AddChild (_parent, prefab_des);
			itemDes.SetActive(true);
			itemDes.GetComponent<UILabel>().text="[87373E]"+_des[i]+"[-]";
		}
		_parent.GetComponent<UIGrid>().Reposition();
	}



	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);

		if (gameObj.name == "close") { 
			finishWindow ();
		} else if (gameObj.name == "button_titleView") {
			//称号一览按钮
			UiManager.Instance.openWindow<LaddersTitleViewWindow> ();

		} else if (gameObj.name == "button_medalView") {
			//奖章一览按钮
			UiManager.Instance.openWindow<LaddersMedalViewWindow> ();
		}

	}

}
