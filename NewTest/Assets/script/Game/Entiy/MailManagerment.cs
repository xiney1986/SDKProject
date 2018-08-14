using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MailManagerment
{

	private const int MAX = 2100000000;
	private List<Mail> mails = new List<Mail> ();
	private List<string> needDelUids = new List<string> ();
	private string str = LanguageConfigManager.Instance.getLanguage ("s0204");
	public static bool isUpdateMailInfo = false;

	public static MailManagerment Instance {
		get{return SingleManager.Instance.getObj("MailManagerment") as MailManagerment;}
	}
	
	public void createMail (string uid, int src, int type, int stime, int etime, string theme, string content, Annex[] annex, int status)
	{
		mails.Add (new Mail (uid, src, type, stime, etime, theme, content, annex, status));  
	}
	//得到没有读过的邮件
	public int getUnReadMailNum ()
	{
		int num = 0;
		for (int i = 0; i < mails.Count; i++) {
			if (mails [i].status == 0) {
				num ++;
			}
		}
		return num;
	}

	/** 指定邮件是否存在 */
	public Mail getMail (string uid)
	{
		for (int i = 0; i < mails.Count; i++) {
			if (mails [i].uid == uid) {
				return mails [i];
			}
		}
		return null;
	}

	/** 指定邮件是否有附件 */
	public bool isMailHaveExtract(string uid)
	{
		Mail mail = getMail(uid);
		if (mail == null)
			return false;
		else if (mail.annex == null)
			return false;
		else
			return true;
	}

	//点击邮件提取判断
	public bool isMailExtract (Mail mail)
	{
		if (mail.annex == null)
			return false;
		for (int i = 0; i < mail.annex.Length; i++) {
			if (mail.annex [i].exp != null && mail.annex [i].exp.getPrizeNumByInt () + UserManager.Instance.self.getEXP () > MailManagerment.MAX) {
				str = LanguageConfigManager.Instance.getLanguage ("s0197");
				return false;
			} else if (mail.annex [i].money != null && mail.annex [i].money.getPrizeNumByInt () + UserManager.Instance.self.getMoney () > MailManagerment.MAX) {
				str = LanguageConfigManager.Instance.getLanguage ("s0198");
				return false;
			} else if (mail.annex [i].rmb != null && mail.annex [i].rmb.getPrizeNumByInt () + UserManager.Instance.self.getRMB () > MailManagerment.MAX) {
				str = LanguageConfigManager.Instance.getLanguage ("s0199");
				return false;
			} else if (mail.annex [i].pve != null && mail.annex [i].pve.getPrizeNumByInt () + UserManager.Instance.self.getPvEPoint () > UserManager.Instance.self.getPvEPointMax ()) {
				str = LanguageConfigManager.Instance.getLanguage ("s0200");
				return false;
			} else if (mail.annex [i].pvp != null && mail.annex [i].pvp.getPrizeNumByInt () + UserManager.Instance.self.getPvPPoint () > UserManager.Instance.self.getPvPPointMax ()) {
				str = LanguageConfigManager.Instance.getLanguage ("s0201");
				return false;
			} else if (mail.annex [i].prop != null && isPropStorageFull (mail.annex [i].prop)) {
				return false;
			}
		}
		return true;
	}
	
	public string getStr ()
	{
		return str;
	}
	
	private bool isPropStorageFull (PrizeSample prop)
	{
		if (prop == null)
			return false;
		bool isfull = false;
		switch (prop.type) {
		case PrizeType.PRIZE_CARD:
			if (prop.getPrizeNumByInt () + StorageManagerment.Instance.getAllRole ().Count > StorageManagerment.Instance.getRoleStorageMaxSpace ()) {
				str = LanguageConfigManager.Instance.getLanguage ("s0192", LanguageConfigManager.Instance.getLanguage ("s0193"));
				isfull = true;
			} else {
				isfull = false;
			}
			break;
		case PrizeType.PRIZE_BEAST:
			if (prop.getPrizeNumByInt () + StorageManagerment.Instance.getAllBeast ().Count > StorageManagerment.Instance.getBeastStorageMaxSpace ()) {
				str = LanguageConfigManager.Instance.getLanguage ("s0192", LanguageConfigManager.Instance.getLanguage ("s0194"));
				isfull = true;
			} else {
				isfull = false;
			}
			break;
		case PrizeType.PRIZE_EQUIPMENT:
			if (prop.getPrizeNumByInt () + StorageManagerment.Instance.getAllEquip ().Count > StorageManagerment.Instance.getEquipStorageMaxSpace ()) {
				str = LanguageConfigManager.Instance.getLanguage ("s0192", LanguageConfigManager.Instance.getLanguage ("s0195"));
				isfull = true;
			} else {
				isfull = false;
			}
			break;
        case PrizeType.PRIZE_MAGIC_WEAPON:
            if (prop.getPrizeNumByInt() + StorageManagerment.Instance.getAllMagicWeapon().Count > StorageManagerment.Instance.getMagicWeaponStorageMaxSpace()) {
                str = LanguageConfigManager.Instance.getLanguage("s0192", LanguageConfigManager.Instance.getLanguage("s0195"));
                isfull = true;
            } else {
                isfull = false;
            }
            break;
		case PrizeType.PRIZE_PROP:
			if (StorageManagerment.Instance.getProp (prop.pSid) != null) {
                //徽记积分有上限
                if (prop.pSid == CommandConfigManager.Instance.getHuiJiMoneySid() && StorageManagerment.Instance.getProp(prop.pSid) != null
                   && StorageManagerment.Instance.getProp(prop.pSid).getNum() >= CommandConfigManager.Instance.getMaxNum()) {
                       str = LanguageConfigManager.Instance.getLanguage("OneOnOneBoss_029");
                    return true;
                }
				// 军功有上限//
				if(prop.pSid == CommandConfigManager.Instance.lastBattleData.junGongSid && StorageManagerment.Instance.getProp(prop.pSid).getNum() + StringKit.toInt(prop.num) > CommandConfigManager.Instance.lastBattleData.junGongMaxNum)
				{
					str = LanguageConfigManager.Instance.getLanguage("LastBattle_awardFailed");
					return true;
				}
				isfull = false;
			} else {
				// 军功有上限//
				if(prop.pSid == CommandConfigManager.Instance.lastBattleData.junGongSid && StringKit.toInt(prop.num) > CommandConfigManager.Instance.lastBattleData.junGongMaxNum)
				{
					str = LanguageConfigManager.Instance.getLanguage("LastBattle_awardFailed");
					return true;
				}
				if (1 + StorageManagerment.Instance.getAllProp ().Count > StorageManagerment.Instance.getPropStorageMaxSpace ()) {
					str = LanguageConfigManager.Instance.getLanguage ("s0192", LanguageConfigManager.Instance.getLanguage ("s0196"));
					isfull = true;
				} else {
					isfull = false;
				}
			}
			break;
		}
		return isfull;
	}
	
	//点击一键提取邮件
	public bool isOneKeyMailExtract ()
	{
		if (mails == null || mails.Count == 0)
			return false;
		for (int i = 0; i < mails.Count; i++) {
			if (mails [i].status != 2) {
				if (mails [i].annex != null) {
					//return isMailExtract (mails [i]);
					// 可领取//
					if(isMailExtract (mails [i]))
					{
						return isMailExtract (mails [i]);
					}
					else
					{
						continue;
					}
				}	
			}
		}
		return false;
	}

	public void oneKeyUpdateState (string uid)
	{
		for (int i = 0; i < mails.Count; i++) {
			if (mails [i].uid == uid) {
				mails [i].status = 1;
			}
		}
	}

	//得到所有邮件
	public List<Mail> getAllMail ()
	{
		return mails; 
	}
	//得到排过序的所有邮件
	public List<Mail> getSortAllMail ()
	{
		for (int i = 0; i < mails.Count-1; i++) {
			for (int j = 0; j < mails.Count-1-i; j++) {
				if (mails [j].stime < mails [j + 1].stime) {
					Mail temp = mails [j];
					mails [j] = mails [j + 1];
					mails [j + 1] = temp;
				}
			}
		}
		return mails;
	}

	//删除指定Id集的邮件(当邮件数超过100封时,自动顶掉时间小的邮件)
	public void runDelete (string[] uids)
	{
		for (int i = 0; i < uids.Length; i++) {
			deleteMailByUid (uids [i]);
		}
	}

	//删除过期的邮件
	public void runDeleteUids ()
	{
		if (needDelUids.Count > 0) {
			for (int i = 0; i < needDelUids.Count; i++) {
				deleteMailByUid (needDelUids [i]);
			}
		}
	}

	//收集过期的邮件UID，待处理
	public void addNeedDelUids (string str)
	{
		needDelUids.Add (str);
	}

	public void clearMail ()
	{
        mails.Clear();
	}

    /// <summary>
    /// 清除全部邮件,如果为false,这里将不清除没有接收附件的
    /// </summary>
    public void clearMail(bool isAll)
    {
        if (!isAll)
        {
            for (int i = 0; i < mails.Count; i++)
            {
                if (!(mails[i].status != 2 && mails[i].annex != null))
                {
                    mails.RemoveAt(i);
                    i--;
                }
            }
        }
        else
        {
            clearMail();
        }
    }



	
	public void deleteMailByUid (string uid)
	{
		for (int i = 0; i < mails.Count; i++) {
			if (mails [i].uid == uid) {
				mails.Remove (mails [i]);
			}
		}
	}

	//标记为已领取
	public void extractMailByUid (string uid)
	{
		for (int i = 0; i < mails.Count; i++) {
			if (mails [i].uid == uid) {
				mails [i].status = 2;
			}
		}
	} 
	public void extractMailByUidWithAnnex(string uid,List<PrizeSample> list)
	{
		for (int i = 0; i < mails.Count; i++) {
			if (mails [i].uid == uid) {
				mails [i].status = 2;
				for(int j = 0 ; j < mails [i] .annex.Length;j++)
				{
					list.Add (getPrize (mails [i] .annex[j]));
					mails[i].annex[j]=null;
				}
			}
		}
	}
	private PrizeSample getPrize(Annex annex)
	{

        if (annex.exp != null)
            return annex.exp;
        else if (annex.money != null)
            return annex.money;
        else if (annex.prop != null)
            return annex.prop;
        else if (annex.pve != null)
            return annex.pve;
        else if (annex.pvp != null)
            return annex.pvp;
        else if (annex.rmb != null)
            return annex.rmb;
        else if (annex.starsoulDebris != null)
            return annex.starsoulDebris;
        else if (annex.ladder != null)
            return annex.ladder;
        else if (annex.contribution != null)
            return annex.contribution;
        else if (annex.mounts != null)
            return annex.mounts;
        else if (annex.magicWeapon != null) return annex.magicWeapon;
        else
            return null;

	}
	//标记为已读
	public void readMail (Mail _mail)
	{
		if (_mail.status == 1)
			return;
		for (int i = 0; i < mails.Count; i++) {
			if (mails [i].uid == _mail.uid) {
				mails [i].status = 1;
			}
		}
	}

	//是否有附件
	public bool isHaveAnnex ()
	{
		if (mails == null)
			return false;
		int currentTime = ServerTimeKit.getSecondTime ();

		for (int i = 0; i < mails.Count; i++) {
			int mailTime = mails [i].etime - currentTime;
			if (mailTime > 0) {	//加上一个是否已过期判断
				if (mails [i].status != 2 && mails [i].annex != null) {
					return true;
				}
			}
		}
		return false;
	}
	/// <summary>
	/// 检查所有邮件中是否有指定sid的装备。。
	/// </summary>
	public bool checkEquipinMain(int sid){
		if(mails==null || !isHaveAnnex()){
			return false;
		}
		for(int i=0;i<mails.Count;i++){
			if(mails[i].annex!=null && mails[i].status != 2){
				Annex[] annexs=mails[i].annex;
				for(int j=0;j<annexs.Length;j++){
					if(annexs[j].prop!=null&&annexs[j].prop.pSid==sid)return true;
				}
			}
		}
		return false;
	}
	public Annex[] parseAnnex (ErlList list)
	{
		if (list == null)
			return null;
		Annex[] annex = new Annex[list.Value.Length];
		for (int i = 0; i < list.Value.Length; i++) {
			annex [i] = new Annex ();
			string str = ((list.Value [i] as ErlArray).Value [0] as ErlAtom).getValueString ();
			if (str == "exp") {
				string strs = PrizeType.PRIZE_EXP + "," + "0" + "," + ((list.Value [i] as ErlArray).Value [1] as ErlType).getValueString ();
				annex [i].exp = new PrizeSample (strs, ',');
			} else if (str == "rmb") {
				string strs = PrizeType.PRIZE_RMB + "," + "0" + "," + ((list.Value [i] as ErlArray).Value [1] as ErlType).getValueString ();
				annex [i].rmb = new PrizeSample (strs, ',');
			} else if (str == "money") {
				string strs = PrizeType.PRIZE_MONEY + "," + "0" + "," + ((list.Value [i] as ErlArray).Value [1] as ErlType).getValueString ();
				annex [i].money = new PrizeSample (strs, ',');
			} else if (str == "pve") {
				string strs = PrizeType.PRIZE_PVE + "," + "0" + "," + ((list.Value [i] as ErlArray).Value [1] as ErlType).getValueString ();
				annex [i].pve = new PrizeSample (strs, ',');
			} else if (str == "pvp") {
				string strs = PrizeType.PRIZE_PVP + "," + "0" + "," + ((list.Value [i] as ErlArray).Value [1] as ErlType).getValueString ();
				annex [i].pvp = new PrizeSample (strs, ',');
			} else if (str == "prop") {
				string strs = (((list.Value [i] as ErlArray).Value [1] as ErlArray).Value [0]).getValueString () + "," + (((list.Value [i] as ErlArray).Value [1] as ErlArray).Value [1]).getValueString () + "," + (((list.Value [i] as ErlArray).Value [1] as ErlArray).Value [2]).getValueString ();
				annex [i].prop = new PrizeSample (strs, ',');
			} else if (str == "ladder_score") {
				//string strs = (((list.Value[i] as ErlArray).Value[1] as ErlArray).Value[0]).getValueString() + "," + (((list.Value[i] as ErlArray).Value[1] as ErlArray).Value[1]).getValueString() + "," + (((list.Value[i] as ErlArray).Value[1] as ErlArray).Value[2]).getValueString();
				string strs1 = PrizeType.PRIZE_LEDDER_SCORE + "," + "0" + "," + ((list.Value [i] as ErlArray).Value [1] as ErlType).getValueString ();
				annex [i].ladder = new PrizeSample (strs1, ',');
			} else if (str == "contribution") {
				string strs1 = PrizeType.PRIZE_CONTRIBUTION + "," + "0" + "," + ((list.Value [i] as ErlArray).Value [1] as ErlType).getValueString ();
				annex [i].contribution = new PrizeSample (strs1, ',');
			} else if (str == "mounts") {
				string strs = PrizeType.PRIZE_MOUNT + "," + ((list.Value [i] as ErlArray).Value [1] as ErlArray).Value [0].getValueString () + "," + ((list.Value [i] as ErlArray).Value [1] as ErlArray).Value [1].getValueString () + "," + ((list.Value [i] as ErlArray).Value [1] as ErlArray).Value [2].getValueString ();
				annex [i].mounts = new PrizeSample (strs, ',');
			}else if(str=="artifact"){
                string strs = PrizeType.PRIZE_MAGIC_WEAPON + "," + ((list.Value[i] as ErlArray).Value[1] as ErlArray).Value[0].getValueString() + "," + ((list.Value[i] as ErlArray).Value[1] as ErlArray).Value[1].getValueString();
            }
		}
		return annex;
	}  
}
