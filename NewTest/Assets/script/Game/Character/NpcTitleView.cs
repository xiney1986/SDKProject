using UnityEngine;
using System.Collections;

public class NpcTitleView :ButtonBase
{

	public UILabel UI_Name;
	public UILabel UI_PlayerTitle;
	public UISprite UI_PlayerBg;
	string uid;
	const int CYCING_OFFSET = 180;
	const int OFFSET = 110;

	public static NpcTitleView Create ()
	{
		passObj obj = Create3Dobj ("Effect/Other/npcTitleView");
		obj.obj.transform.parent = UiManager.Instance.missionMainWindow.UIEffectRoot;
		obj.obj.transform.localScale = Vector3.one;//new Vector3(0.75f, 0.75f, 0.75f);
		obj.obj.transform.localPosition = Vector3.zero;
		return obj.obj.GetComponent<NpcTitleView> ();
	}
	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		openUserInfoWindow (uid);
	}
	private void openUserInfoWindow (string uid)
	{
		if (uid == UserManager.Instance.self.uid) {
			MaskWindow.UnlockUI ();
			return;
		}
		//新手引导不能点
		if (GuideManager.Instance.isLessThanStep (GuideGlobal.NEWOVERSID)) {
			MaskWindow.UnlockUI ();
			return;
		}
		ChatGetPlayerInfoFPort fport = FPortManager.Instance.getFPort ("ChatGetPlayerInfoFPort") as ChatGetPlayerInfoFPort;
		fport.access (uid, PvpPlayerWindow.PVP_TEAM,PvpPlayerWindow.PVE_TEAM_TAPE,null, PvpPlayerWindow.FROM_FRIENDLOOK);
		
	}
	/// <summary>
	/// 更新名字版的位置
	/// </summary>
	/// <param name="isCycing">是否骑马</param>
	public void UpdatePos (Vector3 rolePos, bool isCycing)
	{
		if (MissionManager.instance == null) {
			Destroy (gameObject);
			return;
		}

		if (rolePos!= null && MissionManager.instance.backGroundCamera != null) {
			Vector3 pos = MissionManager.instance.backGroundCamera.WorldToScreenPoint (rolePos);
			if (isCycing)
				pos += new Vector3 (0, CYCING_OFFSET * ((float)Screen.height / 960f), 0);
			else
				pos += new Vector3 (0, OFFSET * ((float)Screen.height / 960f), 0);
			transform.position = UiManager.Instance.gameCamera.ScreenToWorldPoint (pos);
			transform.position = new Vector3 (transform.position.x, transform.position.y, 0);
		}
	}

	public void UpdateName (string roleName, int vip, string uid)
	{
		fatherWindow=UiManager.Instance.missionMainWindow;
		this.uid=uid;
		if (vip > 0) {
			if (uid == UserManager.Instance.self.uid)
				UI_Name.text = Colors.MISSSION_GREEN + roleName + "[-]" + Colors.CHAT_VIP + "[VIP" + vip + "][-]";
			else
				UI_Name.text = roleName + Colors.CHAT_VIP + "[VIP" + vip + "][-]";
		} else {
			if (uid == UserManager.Instance.self.uid)
				UI_Name.text = Colors.MISSSION_GREEN + roleName;
			else
				UI_Name.text = roleName;
		}
	}

	public void UpdatePlayerTitle (bool show, LaddersTitleSample sample_1, LaddersMedalSample sample_2)
	{
		UI_PlayerTitle.gameObject.SetActive (show);
		if (!show) {
			return;
		}

		if (sample_1 == null) {
			UI_PlayerTitle.text = Language ("laddersTip_20");
		} else {
			UI_PlayerTitle.text = sample_1.name;
		}

		if (sample_2 != null) {
			UI_PlayerBg.spriteName = "medal_" + Mathf.Min (sample_2.index + 1, 5);
		} else {
			UI_PlayerBg.spriteName = "medal_0";
		}
	}




}
