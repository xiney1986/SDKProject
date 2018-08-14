using UnityEngine;
using System.Collections;

/// <summary>
/// 限时活动通讯端口
/// </summary>
public class LuckyXianshiFPort :  BaseFPort {

	/** 回调函数<活动积分,活动排名> */
	private CallBack<int,int> callback;
	private int clearType;
	public void access (int noticeSid,CallBack<int,int> callback,int cType) {
		this.callback = callback;
		this.clearType = cType;
		ErlKVMessage message = new ErlKVMessage (FrontPort.NOTICE_LUCKY_XIANSHI_DRAW);
		message.addValue ("sid", new ErlInt (noticeSid));
		access (message);
	}
	/** 读取数据 */
	public override void read (ErlKVMessage message) {
		ErlList tmp = message.getValue ("msg") as ErlList;
		//ErlType erlType = message.getValue ("base") as ErlType;
		ErlType baseErlType = tmp.Value[0] as ErlType;
		int integral = 0;
		int rank = -1;
		if (baseErlType is ErlArray) {
			ErlArray arr= (baseErlType as ErlArray).Value[1] as ErlArray;
			// 活动积分
			integral=StringKit.toInt (arr.Value [0].getValueString ());
			// 活动排名
			rank = StringKit.toInt (arr.Value [1].getValueString ());
		}
		if(clearType == 3)
			RadioManager.Instance.clearByType(clearType);
		ErlType radioErlType = tmp.Value[1] as ErlType; ;//message.getValue ("radio") as ErlType;
		if (radioErlType is ErlArray) {
			ErlArray arr= (radioErlType as ErlArray).Value[1] as ErlArray;
			if (arr.Value.Length>0 && clearType == 3) {
				for (int m = 0,count=arr.Value.Length; m < count; m++) {
					ErlArray radioArray = arr.Value [m] as ErlArray;
					if(radioArray.Value.Length==3){
						string name = radioArray.Value[0].getValueString();
						int vipLevel=StringKit.toInt(radioArray.Value [1].getValueString ());
						int soulSid=StringKit.toInt(radioArray.Value [2].getValueString ());
						addMessageRadio(name, vipLevel, soulSid);
					}else{
	                    string serverName = radioArray.Value[0].getValueString();
	                    string name = radioArray.Value[1].getValueString();
						int vipLevel=StringKit.toInt(radioArray.Value [2].getValueString ());
						int soulSid=StringKit.toInt(radioArray.Value [3].getValueString ());
	                    addMessageRadio("【" + serverName+"】"+name, vipLevel, soulSid);
					}
				}
			}
		}
		if(clearType == 3)
			RankManagerment.Instance.luckyLiehunList.Clear ();
		else if(clearType ==4)
			RankManagerment.Instance.luckyLianjinList.Clear ();
		ErlType rankErlType = tmp.Value[2] as ErlType;//message.getValue ("rank") as ErlType;
		if (rankErlType is ErlArray) {
			ErlArray arr = (rankErlType as ErlArray).Value [1] as ErlArray;
			if (clearType == 3) {
				if (arr.Value.Length > 0) {
					for (int m = 0,count=arr.Value.Length; m<count; m++) {
						RankItemLuckyLiehun rankLuckyLiehun = new RankItemLuckyLiehun ();
						rankLuckyLiehun.bytesRead (0, arr.Value [m] as ErlArray, m + 1);
						addLuckyLiehunRanking (rankLuckyLiehun);
					}
				}
			}else if(clearType == 4){
				if (arr.Value.Length > 0) {
					for (int m = 0,count=arr.Value.Length; m<count; m++) {
						RankItemLuckyLianjin rankLuckLianjin = new RankItemLuckyLianjin ();
						rankLuckLianjin.bytesRead (0, arr.Value [m] as ErlArray, m + 1);
						addLuckyLianjinRanking (rankLuckLianjin);
					}
				}
			}
		}
		if (callback != null) {
			callback(integral,rank);
		}
	}
	/// <summary>
	/// 添加广播公告
	/// </summary>
	public void addMessageRadio(string name,int vipLevel,int soulSid) {
		StarSoulSample soulSample = StarSoulSampleManager.Instance.getStarSoulSampleBySid (soulSid);
		string qualityName= QualityManagerment.getQualityNameByNone(soulSample.qualityId);
		string qualityColor = QualityManagerment.getQualityColor (soulSample.qualityId);
		string str1;
		if (vipLevel > 0) {
			str1 = "[F85C5C]" + LanguageConfigManager.Instance.getLanguage ("s0509", name, vipLevel.ToString());
		} else {
			str1 = "[F85C5C]" +name;
		}
		string str2 = qualityColor + qualityName + LanguageConfigManager.Instance.getLanguage ("Guide_28")+soulSample.name;
		string message="[FFFFFF]"+LanguageConfigManager.Instance.getLanguage("s0507",str1)+"[FFFFFF]"+LanguageConfigManager.Instance.getLanguage("s0561",str2);
		RadioManager.Instance.M_addRadioMsg (clearType,message);
	}
	/// <summary>
	/// 添加限时猎魂排行榜
	/// </summary>
	public void addLuckyLiehunRanking(RankItemLuckyLiehun rankLuckLiehun) {
		RankManagerment.Instance.luckyLiehunList.Add (rankLuckLiehun);
	}
	/// <summary>
	/// 添加限时炼金排行榜
	/// </summary>
	public void addLuckyLianjinRanking(RankItemLuckyLianjin rankLuckLianjin) {
		RankManagerment.Instance.luckyLianjinList.Add (rankLuckLianjin);
	}
}