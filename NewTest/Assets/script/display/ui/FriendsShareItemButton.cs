using UnityEngine;
using System;
using System.Collections;

public class FriendsShareItemButton : ButtonBase {

	public FriendsShareShowButton showItem;
	public UILabel showLabel;
	public UISprite useSprite;
	private ShareInfo info;
	private int tapType;
	private string showType;

	public override void begin ()
	{
		base.begin ();
	}

	public void initUI(int _type,ShareInfo _info)
	{
		info = _info;
		tapType = _type;
		showType = info.type;
		showItem.fatherWindow = fatherWindow;
		showUI();
	}

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		MaskWindow.UnlockUI();
	}

	public void showUI()
	{
		switch(tapType)
		{
		case 0:
			if(info.isUse == 0) {
				useSprite.alpha = 0;
			}
			else {
				useSprite.alpha = 1;
				useSprite.spriteName = "text_share";
			}
			showItem.GetComponent<BoxCollider>().enabled = false;

			string sidOne = "";
			if (showType != FriendsShareManagerment.TYPE_JINHUA) {
				sidOne = info.sid.getValueString();
			}
			switch(showType)
			{
			case FriendsShareManagerment.TYPE_CARD:
				showItem.GetComponent<BoxCollider>().enabled = true;
				showItem.info = info;
				Card card = CardManagerment.Instance.createCard(StringKit.toInt(sidOne));
				string quilityCard = QualityManagerment.getQualityName(card.getQualityId()) + "[-]";
				string nameCard = QualityManagerment.getQualityColor(card.getQualityId()) + card.getName() + "[-]";
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_Mine" + showType,quilityCard,nameCard);
				break;
			case FriendsShareManagerment.TYPE_EQUIP:
				showItem.GetComponent<BoxCollider>().enabled = true;
				showItem.info = info;
				Equip equip = EquipManagerment.Instance.createEquip(StringKit.toInt(sidOne));
				string quilityEquip = QualityManagerment.getQualityName(equip.getQualityId()) + "[-]";
				string nameEquip = QualityManagerment.getQualityColor(equip.getQualityId()) + equip.getName() + "[-]";
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_Mine" + showType,quilityEquip,nameEquip);
				break;
            case FriendsShareManagerment.TYPE_MAGICWEAPON:
                showItem.GetComponent<BoxCollider>().enabled = true;
                showItem.info = info;
                MagicWeapon mw = MagicWeaponManagerment.Instance.createMagicWeapon(StringKit.toInt(sidOne));
                string quilitymw = QualityManagerment.getQualityName(mw.getMagicWeaponQuality()) + "[-]";
                string namemw = QualityManagerment.getQualityColor(mw.getMagicWeaponQuality()) + mw.getName() + "[-]";
                showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_Mine" + showType, quilitymw, namemw);
                break;
			case FriendsShareManagerment.TYPE_XIULIAN:
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_Mine" + showType,sidOne);
				break;
			case FriendsShareManagerment.TYPE_TAOFA:
				Mission mss = MissionInfoManager.Instance.getMissionBySid(StringKit.toInt(sidOne));
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_Mine" + showType,mss.getMissionName());
				break;
			case FriendsShareManagerment.TYPE_JUQING:
				Mission mssJq = MissionInfoManager.Instance.getMissionBySid(StringKit.toInt(sidOne));
				ChapterSample chapterJq = ChapterSampleManager.Instance.getChapterSampleBySid(mssJq.getChapterSid());
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_Mine" + showType,chapterJq.name);
				break;
			case FriendsShareManagerment.TYPE_PVP:
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_Mine" + showType,sidOne);
				break;
			case FriendsShareManagerment.TYPE_SHENGQI:
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_Mine" + showType,sidOne);
				break;
			case FriendsShareManagerment.TYPE_XINGPAN:
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_Mine" + showType,sidOne);
				break;
			case FriendsShareManagerment.TYPE_JINHUA:
				showItem.GetComponent<BoxCollider>().enabled = true;
				showItem.info = info;
//				Card cardJinhua = CardManagerment.Instance.createCard(StringKit.toInt(sid));
				ServerCardMsg cardServer = CardManagerment.Instance.createCardByChatServer(info.sid as ErlArray);
				string nameCardJinhua = QualityManagerment.getQualityColor(cardServer.card.getQualityId()) + cardServer.card.getName() + "[-]";
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_Mine" + showType,nameCardJinhua);
				break;
			case FriendsShareManagerment.TYPE_SHENGJI:
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_Mine" + showType,sidOne);
				break;
			case FriendsShareManagerment.TYPE_NVSHEN:
				Card cardNvshen = CardManagerment.Instance.createCard(StringKit.toInt(sidOne));
				string nvshenName = QualityManagerment.getQualityColor(cardNvshen.getQualityId()) + cardNvshen.getName() + "[-]";
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_Mine" + showType,nvshenName);
				break;
			case FriendsShareManagerment.TYPE_TUPO:
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_Mine" + showType);
				break;
			case FriendsShareManagerment.TYPE_YXZHIZHANG:
				MissionSample mission = MissionSampleManager.Instance.getMissionSampleBySid(StringKit.toInt(sidOne));
				ChapterSample chsam = ChapterSampleManager.Instance.getChapterSampleBySid(mission.chapterSid);
				int gc = StringKit.toInt(sidOne) - chsam.missions[0] + 1;
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_Mine" + showType,mission.name,gc.ToString());
				break;
			case FriendsShareManagerment.TYPE_JWTISHENG:
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_Mine" + showType,KnighthoodConfigManager.Instance.getKnighthoodByGrade (StringKit.toInt(info.sid.getValueString().ToString())).kName);
				break;
			case FriendsShareManagerment.TYPE_XINGHUN:
				StarSoulSample sample = StarSoulSampleManager.Instance.getStarSoulSampleBySid(StringKit.toInt(sidOne));
				string  sampleName= QualityManagerment.getQualityColor(sample.qualityId) + sample.name + "[-]";
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_Mine" + showType,sampleName);
				break;
			case FriendsShareManagerment.TYPE_ZUOQI:
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_Mine" + showType,MountsManagerment.Instance.createMounts(StringKit.toInt(sidOne)).getName());
				break;
			case FriendsShareManagerment.TYPE_VIP:
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_Mine" + showType,UserManager.Instance.self.nickname,sidOne);
				break;
			case FriendsShareManagerment.TYPE_LADDER:
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_Mine" + showType,sidOne);
				break;
			case FriendsShareManagerment.TYPE_ARENA:
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_Mine" + showType,sidOne);
				break;
			case FriendsShareManagerment.TYPE_JINGCAI:
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_Mine" + showType);
				break;
			case FriendsShareManagerment.TYPE_BEAST:
				//showItem.GetComponent<BoxCollider>().enabled = true;
				//showItem.info = info;
				BeastEvolve beast = BeastEvolveManagerment.Instance.getBeastEvolveBySid(StringKit.toInt(sidOne));
				string beastName = QualityManagerment.getQualityColor(beast.getBeast().getQualityId()) + beast.getBeast().getName() + "[-]";
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_Mine" + showType,beastName);
				break;
			case FriendsShareManagerment.TYPE_QISHU:
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_Mine" + showType,sidOne);
				break;
			}
			break;
		case 1:
			CardSample cs;
			if(info.isUse == 0) {
				useSprite.alpha = 0;
			}
			else {
				useSprite.alpha = 1;
				useSprite.spriteName = "text_applaud";
			}
			string name = info.name;
			string vip1 =info.vip;
			string vip="";
			if(vip1=="0")vip="";
			else vip="<VIP"+vip1+">";
			DateTime dt = TimeKit.getDateTime(StringKit.toInt(info.time));
			string time = "[" + dt.Hour + ":" + dt.Minute + "]";
			showItem.GetComponent<BoxCollider>().enabled = false;

			string sidTwo = "";
			if (showType != FriendsShareManagerment.TYPE_JINHUA) {
				sidTwo = info.sid.getValueString();
			}
			switch(info.type)
			{
			case FriendsShareManagerment.TYPE_CARD:
				showItem.GetComponent<BoxCollider>().enabled = true;
				showItem.info = info;
				cs=CardSampleManager.Instance.getRoleSampleBySid(StringKit.toInt(sidTwo));
				if(cs==null){
					showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_F" + showType,name,vip,time,"[-]",sidTwo);
					break;
				}else{
					Card card = CardManagerment.Instance.createCard(StringKit.toInt(sidTwo));
					string quilityCard = QualityManagerment.getQualityName(card.getQualityId()) + "[-]";
					string nameCard = QualityManagerment.getQualityColor(card.getQualityId()) + card.getName() + "[-]";
					showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_F" + showType,name,vip,time,quilityCard,nameCard);
					break;
				}
			case FriendsShareManagerment.TYPE_EQUIP:
				showItem.GetComponent<BoxCollider>().enabled = true;
				showItem.info = info;
				if(EquipmentSampleManager.Instance.getEquipSampleBySid(StringKit.toInt(sidTwo))==null){
					showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_F" + showType,name,vip,time,"[-]",sidTwo);
					break;
				}else{
				Equip equip = EquipManagerment.Instance.createEquip(StringKit.toInt(sidTwo));
				string quilityEquip = QualityManagerment.getQualityName(equip.getQualityId()) + "[-]";
				string nameEquip = QualityManagerment.getQualityColor(equip.getQualityId()) + equip.getName() + "[-]";
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_F" + showType,name,vip,time,quilityEquip,nameEquip);
				break;
				}
            case FriendsShareManagerment.TYPE_MAGICWEAPON:
                showItem.GetComponent<BoxCollider>().enabled = true;
                showItem.info = info;
                if (MagicWeaponManagerment.Instance.createMagicWeapon(StringKit.toInt(sidTwo)) == null) {
                    showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_F" + showType, name, vip, time, "[-]", sidTwo);
                    break;
                } else {
                    MagicWeapon equip = MagicWeaponManagerment.Instance.createMagicWeapon(StringKit.toInt(sidTwo));
                    string quilityMagic = QualityManagerment.getQualityName(equip.getMagicWeaponQuality()) + "[-]";
                    string nameMagic = QualityManagerment.getQualityColor(equip.getMagicWeaponQuality()) + equip.getName() + "[-]";
                    showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_F" + showType, name,time, quilityMagic, nameMagic);
                    break;
                }
			case FriendsShareManagerment.TYPE_XIULIAN:
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_F" + showType,name,vip,time,sidTwo);
				break;
			case FriendsShareManagerment.TYPE_TAOFA:
				if(MissionSampleManager.Instance .getMissionSampleBySid (StringKit.toInt(sidTwo))==null){
					showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_F" + showType,name,vip,time,sidTwo);
					break;
				}else{
				Mission mss = MissionInfoManager.Instance.getMissionBySid(StringKit.toInt(sidTwo));
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_F" + showType,name,vip,time,mss.getMissionName());
				break;
				}
			case FriendsShareManagerment.TYPE_JUQING:
				if(MissionSampleManager.Instance .getMissionSampleBySid (StringKit.toInt(sidTwo))==null){
					showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_F" + showType,name,vip,time,sidTwo);
					break;
				}else{
					Mission mssJuqing = MissionInfoManager.Instance.getMissionBySid(StringKit.toInt(sidTwo));
					if(ChapterSampleManager.Instance.getChapterSampleBySid(mssJuqing.getChapterSid())==null){
						showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_F" + showType,name,vip,time,sidTwo);
						break;
					}else{
						ChapterSample chapterJq = ChapterSampleManager.Instance.getChapterSampleBySid(mssJuqing.getChapterSid());
						showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_F" + showType,name,vip,chapterJq.name);
						break;
					}
				}
			case FriendsShareManagerment.TYPE_PVP:
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_F" + showType,name,vip,sidTwo);
				break;
			case FriendsShareManagerment.TYPE_SHENGQI:
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_F" + showType,name,vip,sidTwo);
				break;
			case FriendsShareManagerment.TYPE_XINGPAN:
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_F" + showType,name,vip,sidTwo);
				break;
			case FriendsShareManagerment.TYPE_JINHUA:
				showItem.GetComponent<BoxCollider>().enabled = true;
				showItem.info = info;
//				Card cardJinhua = CardManagerment.Instance.createCard(StringKit.toInt(sidTwo));
				ServerCardMsg cardServer = CardManagerment.Instance.createCardByChatServer(info.sid as ErlArray);
				string nameCardJinhua = QualityManagerment.getQualityColor(cardServer.card.getQualityId()) + cardServer.card.getName() + "[-]";
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_F" + showType,name,vip,nameCardJinhua);
				break;
			case FriendsShareManagerment.TYPE_SHENGJI:
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_F" + showType,name,vip,sidTwo);
				break;
			case FriendsShareManagerment.TYPE_NVSHEN:
				cs=CardSampleManager.Instance.getRoleSampleBySid(StringKit.toInt(sidTwo));
				if(cs==null){
					showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_F" + showType,name,vip,sidTwo);
					break;
				}else{
				Card cardNvshen = CardManagerment.Instance.createCard(StringKit.toInt(sidTwo));
				string nvshenName = QualityManagerment.getQualityColor(cardNvshen.getQualityId()) + cardNvshen.getName() + "[-]";
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_F" + showType,name,vip,nvshenName);
				break;
				}
			case FriendsShareManagerment.TYPE_TUPO:
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_F" + showType,name,vip);
				break;

			case FriendsShareManagerment.TYPE_YXZHIZHANG:	
				MissionSample mission = MissionSampleManager.Instance.getMissionSampleBySid(StringKit.toInt(sidTwo));
				if(mission==null){
					showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_F" + showType,name,vip,sidTwo,sidTwo);
					break;
				}
				ChapterSample chsam = ChapterSampleManager.Instance.getChapterSampleBySid(mission.chapterSid);
				if(chsam==null){
					showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_F" + showType,name,vip,mission.name,sidTwo);
					break;
				}
				int gc = StringKit.toInt(sidTwo) - chsam.missions[0] + 1;
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_F" + showType,name,vip,mission.name,gc.ToString());
				break;
			case FriendsShareManagerment.TYPE_JWTISHENG:
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_F" + showType,name,vip);
				break;
			case FriendsShareManagerment.TYPE_XINGHUN:
				StarSoulSample sample = StarSoulSampleManager.Instance.getStarSoulSampleBySid(StringKit.toInt(sidTwo));
				string  sampleName= QualityManagerment.getQualityColor(sample.qualityId) + sample.name + "[-]";
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_F" + showType,name,vip,sampleName);
				break;
			case FriendsShareManagerment.TYPE_ZUOQI:
				if(MountsSampleManager.Instance.getMountsSampleBySid(StringKit.toInt(sidTwo))==null){
					showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_F" + showType,name,vip,sidTwo);
					break;
				}
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_F" + showType,name,vip,MountsManagerment.Instance.createMounts(StringKit.toInt(sidTwo)).getName());
				break;
			case FriendsShareManagerment.TYPE_VIP:
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_F" + showType,name,vip);
				break;
			case FriendsShareManagerment.TYPE_LADDER:
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_F" + showType,name,vip);
				break;
			case FriendsShareManagerment.TYPE_ARENA:
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_F" + showType,name,vip,sidTwo);
				break;
			case FriendsShareManagerment.TYPE_JINGCAI:
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_F" + showType,name,vip);
				break;
			case FriendsShareManagerment.TYPE_BEAST:
				BeastEvolve beast = BeastEvolveManagerment.Instance.getBeastEvolveBySid(StringKit.toInt(sidTwo));
				if(beast==null){
					showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_F" + showType,name,vip,sidTwo);
					break;
				}
				string beastName = QualityManagerment.getQualityColor(beast.getBeast().getQualityId()) + beast.getBeast().getName() + "[-]";
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_F" + showType,name,vip,beastName);
				break;
			case FriendsShareManagerment.TYPE_QISHU:
				showLabel.text = LanguageConfigManager.Instance.getLanguage("Share_F" + showType,name,vip,sidTwo);
				break;
			}
			break;
		}
	}

}
