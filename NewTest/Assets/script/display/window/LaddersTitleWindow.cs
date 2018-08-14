using UnityEngine;
using System.Collections;
/// <summary>
/// 天梯玩家称号升级 展示窗口
/// </summary>
public class LaddersTitleWindow : WindowBase
{
	public GameObject prefab_des;
	public GameObject root_title_currentDes;
	public GameObject root_title_nextDes;
	public barCtrl bar_title;//称号exp条
	public UILabel label_title;//称号
	public UISprite label_title_bg;
	public Transform root_effect;

	public override void OnStart ()
	{
		prefab_des.SetActive (false);
	}

	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();	
		updateView ();
	}
	/// <summary>
	/// 断线重连
	/// </summary>
	public override void OnNetResume ()
	{
		base.OnNetResume ();
		updateView ();
	}
	/// <summary>
	/// 播放特效
	/// </summary>
	public void playPrestigeLevelUpEffect()
	{
		EffectManager.Instance.CreateEffect(root_effect,"Effect/UiEffect/UpgradeTitle_effect");
	}
	/// <summary>
	/// 更新视图
	/// </summary>
	private void updateView ()
	{
		int userP = UserManager.Instance.self.prestige;
		LaddersTitleSample currentTitleSample = LaddersManagement.Instance.M_getCurrentPlayerTitle ();

		label_title_bg.spriteName = getTitleBg ();

		label_title.text = currentTitleSample.name;
		updateDes (root_title_currentDes, currentTitleSample.addDescriptions);

		LaddersTitleSample nextTitleSample = LaddersConfigManager.Instance.config_Title.M_getTitleByIndex (currentTitleSample.index + 1);
		updateDes (root_title_nextDes, nextTitleSample.addDescriptions);
	}
	/// <summary>
	/// 更新称号加成描述
	/// </summary>
	/// <param name="_parent">_parent.</param>
	/// <param name="_des">_des.</param>
	private void updateDes (GameObject _parent, string[] _des)
	{
		UIUtils.M_removeAllChildren (_parent);
		if (_des == null) {
			return;
		}
		GameObject itemDes;
		for (int i=0,length=_des.Length; i<length; i++) {
			itemDes = NGUITools.AddChild (_parent, prefab_des);
			itemDes.SetActive (true);
			itemDes.GetComponent<UILabel> ().text = _des [i];
		}
		_parent.GetComponent<UIGrid> ().Reposition ();
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "confirm") {
			this.finishWindow();
		}
	}
	/// <summary>
	/// 点击整个窗口后关闭窗口
	/// </summary>
	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		finishWindow ();
	}
	/// <summary>
	/// 返回称号图标背景url
	/// </summary>
	/// <returns>The title background.</returns>
	private string getTitleBg(){

		int currentSid=LaddersManagement.Instance.currentPlayerMedalSid;
		LaddersMedalSample currentMedal=LaddersConfigManager.Instance.config_Medal.M_getMedalBySid(currentSid);
		if(currentMedal!=null)
		{
			return "medal_"+Mathf.Min(currentMedal.index+1,5);
		}else
		{
			return "medal_0";
		}
	}
}
