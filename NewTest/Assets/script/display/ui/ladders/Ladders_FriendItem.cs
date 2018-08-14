using UnityEngine;
using System.Collections;
/// <summary>
/// 天梯好友列表中的每一项
/// </summary>
public class Ladders_FriendItem :MonoBase {
	
	public UITexture iconHead;//头像
	public UISprite spriteVip;//VIP

	public UILabel label_Level;//等级	
	public UILabel label_Guild;
	public UILabel label_CombatPower;
	public UILabel label_Name;//名称
	public UILabel label_Horoscope;
	public UILabel label_BeinviteNum;//被邀请次数
	public ButtonBase btn_battle;

	public FriendInfo data;
	private int inviteNum=-100;
	/// <summary>
	/// 传入数据更新
	/// </summary>
	/// <param name="_data">_data.</param>
	public void M_update(FriendInfo _data)
	{
		data = _data;
		
		if(data == null)
			return;
		showUI();
		MaskWindow.UnlockUI();
	}
	/// <summary>
	/// 传入数据更新
	/// </summary>
	/// <param name="_data">_data.</param>
	public void M_update(FriendInviteInfo _data)
	{
		data = new FriendInfo(_data.getUid(),_data.getHeadIco(),_data.getName(),_data.getExp(),_data.getVipExp(),_data.getGuild(),_data.getCombatPower(),_data.getStar(),_data.getIsOnline(),_data.getGiftReceiveStatus(),_data.getGiftSendStatus());
		inviteNum = _data.inviteNum;
		if(data == null)
			return;
		showUI();
		MaskWindow.UnlockUI();
	}
	/// <summary>
	/// 更新视图
	/// </summary>
	private void showUI()
	{
		label_Level.text = "LV." + data.getLevel();
		
		ResourcesManager.Instance.LoadAssetBundleTexture(UserManager.Instance.getIconPath(StringKit.toInt( data.getHeadIco())),iconHead);
	
		if(data.getVipLevel() >0) 
		{
			spriteVip.gameObject.SetActive (true);
			spriteVip.spriteName="vip"+data.getVipLevel();
		}else 
		{
			spriteVip.gameObject.SetActive (false);
		}	
		//buttonInfo.starLabel.text = HoroscopesManager.Instance.getStarByType (oppItem.star).getName ();
		//buttonInfo.starIco.spriteName = HoroscopesManager.Instance.getStarByType (oppItem.star).getSpriteName ();
		label_Name.text = Language("pvpPlayerWindow01")+data.getName();
		label_Guild.text=Language("pvpPlayerWindow02")+data.getGuild();
		label_CombatPower.text=Language("laddersPrefix_02")+"[EEC211]"+data.getCombatPower().ToString()+"[-]";
		if(inviteNum==-100)label_BeinviteNum.gameObject.SetActive(false);
		label_BeinviteNum.text= inviteNum+"/"+LaddersManagement.Instance.BeInviteMaxNum;
		if(inviteNum<=0)btn_battle.disableButton(true);

	}
}


