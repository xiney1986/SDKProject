using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RankItemButton : ButtonBase
{
	[HideInInspector]
	public RankItemView
		rankItemView;
	private int type;
	
	public  void initialize (RankItemView rankItemView,int type)
	{
		this.rankItemView = rankItemView;
		this.type = type;
	}

	public  void initialize (RankItemView rankItemView,int type,WindowBase _fatherWin)
	{
		this.rankItemView = rankItemView;
		this.type = type;
		this.fatherWindow = _fatherWin;
	}

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		switch (type) {
		case RankManagerment.TYPE_COMBAT:
			OnClickCombat ();
			break;
		case RankManagerment.TYPE_PVP:
			OnClickPVP ();
			break;
		case RankManagerment.TYPE_MONEY:
			OnClickMoney ();
			break;
		case RankManagerment.TYPE_ROLE:
			OnClickRole ();
			break;
		case RankManagerment.TYPE_GUILD:
			OnClickGuild ();
			break;
		case RankManagerment.TYPE_ROLE_LV:
			OnClickLevel ();
			break;
		case RankManagerment.TYPE_ROLE_CARD:
			OnClickRoleCard();
			break;
		case RankManagerment.TYPE_GODDESS:
			OnClickGoddess();
			break;
		case RankManagerment.TYPE_LUCKY_CARD:
//			OnClickLuckyCard();
            MaskWindow.UnlockUI();
			break;
		case RankManagerment.TYPE_LUCKY_EQUIP:
			OnClickLuckyEquip();
			break;
		case RankManagerment.TYPE_LUCKY_LIEHUN:
//			OnClickLuckyLiehun();
            MaskWindow.UnlockUI();
			break;
		case RankManagerment.TYPE_LUCKY_LIANJIN:
//			OnClickLuckyLianjin();
            MaskWindow.UnlockUI();
			break;
		case RankManagerment.TYPE_GUILD_SHAKE:
			OnClickGuildShakeRank();
			break;
		case RankManagerment.TYPE_GUILD_FIGHT:
			OnClickGuildFight();
			break;
        case RankManagerment.TYPE_BOSSDAMAGE:
            MaskWindow.UnlockUI();
            break;
		case RankManagerment.TYPE_LASTBATTLE:
			MaskWindow.UnlockUI();
			break;
		}
	}

	//-----------------------按钮事件处理--------------------------------
	void OnClickCombat ()
	{
		RankItemCombat rankItemCombat = rankItemView.data as RankItemCombat;
		openUserInfoWindow (rankItemCombat.uid);
	}
	
	void OnClickPVP ()
	{
		RankItemPVP rankItemPVP = rankItemView.data as RankItemPVP;
		openUserInfoWindow (rankItemPVP.uid);
	}
	
	void OnClickMoney ()
	{
		RankItemMoney rankItemMoney = rankItemView.data as RankItemMoney;
		openUserInfoWindow (rankItemMoney.uid);
	}
	
	void OnClickRole ()
	{
		RankItemRole rankItemRole = rankItemView.data as RankItemRole;
		openUserInfoWindow (rankItemRole.uid);
	}

	void OnClickLevel ()
	{
		RankItemRoleLv rankItemLv = rankItemView.data as RankItemRoleLv;
		openUserInfoWindow (rankItemLv.uid);
	}

	void OnClickGuildShakeRank ()
	{
		GuildShakeRankItem guildShakeRankItem = rankItemView.data as GuildShakeRankItem;
		openUserInfoWindow (guildShakeRankItem.uid);
	}
	
	void OnClickRoleCard ()
	{
		RankItemRole rankItemRole = rankItemView.data as RankItemRole;
		GetPlayerCardInfoFPort fport = FPortManager.Instance.getFPort ("GetPlayerCardInfoFPort") as GetPlayerCardInfoFPort;
		fport.getCard (rankItemRole.uid, rankItemRole.cardUid, null);
	}
	
	void OnClickGuild ()
	{
		RankItemGuild rankItemGuild = rankItemView.data as RankItemGuild;
		RankGetGuildInfoFPort fport = FPortManager.Instance.getFPort ("RankGetGuildInfoFPort") as RankGetGuildInfoFPort;
		fport.access (rankItemGuild.gid, OnGetGuildInfoBack);
	}

	void OnClickGuildFight(){
		RankItemGuildFight rankItemGuildFight = rankItemView.data as RankItemGuildFight;
		RankGetGuildInfoFPort fport = FPortManager.Instance.getFPort ("RankGetGuildInfoFPort") as RankGetGuildInfoFPort;
		fport.access (rankItemGuildFight.uid, OnGetGuildInfoBack);
	}


	void OnClickGoddess ()
	{
		RankItemGoddess rankItemGodess = rankItemView.data as RankItemGoddess;
		openUserInfoWindow (rankItemGodess.uid);
	}

	void OnClickLuckyCard()
	{
		RankItemLuckyCard rankItemLuckyCard = rankItemView.data as RankItemLuckyCard;
		openUserInfoWindow (rankItemLuckyCard.uid);
	}

	void OnClickLuckyEquip()
	{
		RankItemLuckyEquip rankItemLuckyEquip = rankItemView.data as RankItemLuckyEquip;
		openUserInfoWindow (rankItemLuckyEquip.uid);
	}
	void OnClickLuckyLiehun()
	{
		RankItemLuckyLiehun rankItemLuckyLiehun = rankItemView.data as RankItemLuckyLiehun;
		openUserInfoWindow (rankItemLuckyLiehun.uid);
	}
	void OnClickLuckyLianjin()
	{
		RankItemLuckyLianjin rankItemLuckyLianjin = rankItemView.data as RankItemLuckyLianjin;
		openUserInfoWindow (rankItemLuckyLianjin.uid);
	}
	//---------------------------end-----------------------------
	
	private void openUserInfoWindow (string uid)
	{
		ChatGetPlayerInfoFPort fport = FPortManager.Instance.getFPort ("ChatGetPlayerInfoFPort") as ChatGetPlayerInfoFPort;
		//fport.access (uid, null, null, PvpPlayerWindow.FROM_RANK);
		fport.access (uid,10,null, null, PvpPlayerWindow.FROM_RANK);
	}
	
	void OnGetGuildInfoBack (Guild guild, List<GuildMember> memebers)
	{
		if (guild == null) {
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("Guild_104"));
			return;
		}
		UiManager.Instance.openWindow<GuildInfoWindow> ((win1) => {
			win1.init (guild, memebers);
		});
	}
}
