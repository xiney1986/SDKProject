using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RankItemView : MonoBase
{
	/** 各碰撞体 */
	public BoxCollider[] boxes;
	/** 各Label */
	public UILabel[] texts;
	/** 标识自己的背景 */
	public UISprite bg;
	/** 奖杯 */
	public UISprite cup;
	/** 排名 */
	public UILabel rankingNumber;
	[HideInInspector]
	public WindowBase
		fatherWindow;
	[HideInInspector]
	public object
		data;
	[HideInInspector]
	public int
		index;
	[HideInInspector]
	public int
		type;

	void Start ()
	{
		//cup.spriteName = null;
	}

	public void setFatherWindow(WindowBase father){
		fatherWindow = father;
		foreach (BoxCollider box in boxes) {
			RankItemButton item = box.GetComponent<RankItemButton>();
			if(item != null){
				item.setFatherWindow(father);
			}
		}
	}

	public void init (object data, int type, int index)
	{
		this.data = data;
		this.index = index;
		this.type = type;

		if (data == null) {
			setText (1, LanguageConfigManager.Instance.getLanguage ("rankWindow_nodata"));
			return;
		}
		setSpriteBg ();
		setText (0, index + 1);
		switch (type) {
		case RankManagerment.TYPE_COMBAT:
			RankItemCombat rankItemCombat = data as RankItemCombat;
			setText (1, rankItemCombat.name, rankItemCombat.vipLevel, RankManagerment.TYPE_COMBAT);
			setText (2, rankItemCombat.combat);
			break;
			
		case RankManagerment.TYPE_PVP:
			RankItemPVP rankItemPVP = data as RankItemPVP;
			setText (1, rankItemPVP.name, rankItemPVP.vipLevel, RankManagerment.TYPE_PVP);
			setText (2, rankItemPVP.win);
			break;
		case RankManagerment.TYPE_LUCKY_CARD:
			RankItemLuckyCard rankItemLuckyCard = data as RankItemLuckyCard;
			setText (1, rankItemLuckyCard.name, rankItemLuckyCard.vipLevel, RankManagerment.TYPE_LUCKY_CARD);
			setText (2, rankItemLuckyCard.integral);
			break;

		case RankManagerment.TYPE_LUCKY_EQUIP:
			RankItemLuckyEquip rankItemLuckyEquip = data as RankItemLuckyEquip;
            setText(1, rankItemLuckyEquip.name, rankItemLuckyEquip.vipLevel, RankManagerment.TYPE_LUCKY_EQUIP);
			setText (2, rankItemLuckyEquip.integral);
			break;

		case RankManagerment.TYPE_LUCKY_LIEHUN:
			RankItemLuckyLiehun rankItemLuckyLiehun = data as RankItemLuckyLiehun;
            setText(1, rankItemLuckyLiehun.name, rankItemLuckyLiehun.vipLevel, RankManagerment.TYPE_LUCKY_LIEHUN);
			setText (2, rankItemLuckyLiehun.integral);
			break;
		case RankManagerment.TYPE_LUCKY_LIANJIN:
			RankItemLuckyLianjin rankItemLuckyLianjin = data as RankItemLuckyLianjin;
            setText(1, rankItemLuckyLianjin.name, rankItemLuckyLianjin.vipLevel, RankManagerment.TYPE_LUCKY_LIANJIN);
			setText (2, rankItemLuckyLianjin.integral);
			break;

		case RankManagerment.TYPE_MONEY:
			RankItemMoney rankItemMoney = data as RankItemMoney;
			setText (1, rankItemMoney.name, rankItemMoney.vipLevel, RankManagerment.TYPE_MONEY);
			setText (2, rankItemMoney.money);
			break;
        case RankManagerment.TYPE_BOSSDAMAGE:
            RankItemTotalDamage rankItemDamage = data as RankItemTotalDamage;
            setText(1, rankItemDamage.name,rankItemDamage.vipLevel, RankManagerment.TYPE_BOSSDAMAGE);
            setText(2, rankItemDamage.damage);
            break;

		case RankManagerment.TYPE_ROLE:
			RankItemRole rankItemRole = data as RankItemRole;
			setText (1, rankItemRole.name, rankItemRole.vipLevel, RankManagerment.TYPE_ROLE);
			setText (2, rankItemRole.cardName, RankManagerment.TYPE_ROLE_CARD);
			break;
			
		case RankManagerment.TYPE_GUILD:
			RankItemGuild rankItemGuild = data as RankItemGuild;
			setText (1, rankItemGuild.name, rankItemGuild.vipLevel, RankManagerment.TYPE_GUILD);
			setText (2, rankItemGuild.score);
			break;
		case RankManagerment.TYPE_ROLE_LV:
			RankItemRoleLv rankItemLevel = data as RankItemRoleLv;
			setText (1, rankItemLevel.name, rankItemLevel.vipLevel, RankManagerment.TYPE_ROLE_LV);
			setText (2, rankItemLevel.lv);
			break;

		case RankManagerment.TYPE_GODDESS:
			RankItemGoddess rankItemGoddess = data as RankItemGoddess;
			setText (1, rankItemGoddess.name, rankItemGoddess.vipLevel, RankManagerment.TYPE_GODDESS);
			setText (2, rankItemGoddess.addPer + "%");
			break;

		case RankManagerment.TYPE_GUILD_SHAKE:
			GuildShakeRankItem guildShakeRankItem = data as GuildShakeRankItem;
			setText (1, guildShakeRankItem.name, RankManagerment.TYPE_GUILD_SHAKE);
			setText (2, guildShakeRankItem.integral);
			setText (3, guildShakeRankItem.contribution);
			break;

		case RankManagerment.TYPE_GUILD_AREA_CONTRIBUTION:
			GuildAreaHurtRankItem item = data as GuildAreaHurtRankItem;
			setText(1,item.name);
			setText(2,item.warNum);
			setText(3,item.hurtNum);
			break;
		case RankManagerment.TYPE_GUILD_FIGHT:
			RankItemGuildFight guild = data as RankItemGuildFight;
			setText(1,guild.name,RankManagerment.TYPE_GUILD_FIGHT);
			setText(2,guild.getJudgeString());
			setText(3,guild.judgeScore);
			break;
		}

        
		//给前三名加金杯，银杯，铜杯
		if (cup != null) {
			switch (index) {
			case 0:
				cup.spriteName = "rank_1";
				cup.gameObject.SetActive (true);
				rankingNumber.transform.localScale = new Vector3 (0.8f, 0.8f, 0.8f);
				texts[0].text = "";
				break;
			case 1:
				cup.spriteName = "rank_2";
				cup.gameObject.SetActive (true);
				rankingNumber.transform.localScale = new Vector3 (0.8f, 0.8f, 0.8f);
				texts[0].text = "";				
				break;
			case 2:
				cup.spriteName = "rank_3";
				cup.gameObject.SetActive (true);
				rankingNumber.transform.localScale = new Vector3 (0.8f, 0.8f, 0.8f);
				texts[0].text = "";
				break;
			default:
				cup.spriteName = null;
				cup.gameObject.SetActive (false);
				rankingNumber.transform.localScale = new Vector3 (0.6f, 0.6f, 0.6f);
				break;
			}
			        
		}       
	}
	/** 设置背景 */
	private void setSpriteBg ()
	{
		bool isMe = false;
		if (fatherWindow is RankWindow) {
			isMe = (fatherWindow as RankWindow).getMyRankWithShow () == index + 1;
		} else if (fatherWindow is ArenaRankWindow) {
			isMe = (fatherWindow as ArenaRankWindow).getMyRankWithShow () == index + 1;
		} else if (fatherWindow is NoticeWindow) {
			NoticeWindow noticeWin = fatherWindow as NoticeWindow;
			if (noticeWin.show != null) {
				LuckyCardContent content = noticeWin.show.GetComponent<LuckyCardContent> ();
				if (content != null) {
					isMe = content.rank == index + 1;
				}
			}
		}

		if (isMe) {
			bg.gameObject.SetActive(true);
		} else {
			bg.gameObject.SetActive(false);
		}
	}

	void setText (int index, object text)
	{
		setText (index, text, 0);
	}
	//专门针对于名字的 要显示vip等级
	void setText (int index, string name, int vipLevel, int clickType)
	{
		setText (index, name, clickType);
	}

	void setText (int index, object text, int clickType)
	{
		bool enableButton = clickType > 0;
		boxes [index].enabled = enableButton;
		if (enableButton) {
			if(fatherWindow is NoticeWindow)
				boxes [index].GetComponent<RankItemButton> ().initialize (this, clickType,fatherWindow);
			else
				boxes [index].GetComponent<RankItemButton> ().initialize (this, clickType);
			texts [index].text = text.ToString () ;
		} else {
			texts [index].text = text.ToString ();
		}
	}
}
