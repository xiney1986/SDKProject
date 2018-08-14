using UnityEngine;
using System.Collections;

/// <summary>
/// 限时抽卡活动通讯端口
/// 跨服务器
/// 萝卜
/// </summary>
public class LuckyCardFPort :  BaseFPort {

	/** 回调函数<活动积分,活动排名> */
	private CallBack<int,int> callback;
	
	public void access (int noticeSid,CallBack<int,int> callback) {
		this.callback = callback;
        ErlKVMessage message = new ErlKVMessage(FrontPort.NOTICE_LUCKY_XIANSHI_DRAW);
        message.addValue("sid", new ErlInt(noticeSid));
		access (message);
	}
	/** 读取数据 */
	public override void read (ErlKVMessage message) {
        ErlList tmp = message.getValue("msg") as ErlList;
        ErlArray erlType = tmp.Value[0] as ErlArray; 
		//ErlType erlType = message.getValue ("base") as ErlType;
		int integral = 0;
		int rank = -1;
		if (erlType is ErlArray) {
            ErlArray arr = (erlType as ErlArray).Value[1] as ErlArray;
			int index=0;
			// 活动积分
			integral=StringKit.toInt (arr.Value [index++].getValueString ());
			// 活动排名
			rank = StringKit.toInt (arr.Value [index++].getValueString ());
		}
		RadioManager.Instance.clearByType(RadioManager.RADIO_LUCKY_CARD_TYPE);
        //ErlType radioErlType = message.getValue("radio") as ErlType;
        ErlArray radioErlType = tmp.Value[1] as ErlArray; 
		if (radioErlType is ErlArray) {
            //ErlArray arr = radioErlType as ErlArray;
            ErlArray arr = (radioErlType as ErlArray).Value[1] as ErlArray;
			if (arr.Value.Length>0) {
				for (int m = 0,count=arr.Value.Length; m < count; m++) {
					ErlArray radioArray = arr.Value [m] as ErlArray;
                    string serverName = "("+radioArray.Value[0].getValueString()+")";//服务器名字
                    string name = serverName+radioArray.Value[1].getValueString();//角色名字
					int vipLevel=StringKit.toInt(radioArray.Value [2].getValueString ());//vip等级
					string cardType=radioArray.Value [3].getValueString ();//卡片类型
					int cardSid=StringKit.toInt(radioArray.Value [4].getValueString ());//卡片sid
					addMessageRadio(name,vipLevel,cardType,cardSid);
				}
			}
		}
		RankManagerment.Instance.luckyCardList.Clear ();
        //ErlType rankErlType = message.getValue ("rank") as ErlType;
        ErlArray rankErlType = tmp.Value[2] as ErlArray; 
		if (rankErlType is ErlArray) {
            //ErlArray arr= rankErlType as ErlArray;
            ErlArray arr = (rankErlType as ErlArray).Value[1] as ErlArray;
			if (arr.Value.Length>0) {
				RankItemLuckyCard rankLuckCard;
				for (int m = 0,count=arr.Value.Length; m<count; m++) {
					rankLuckCard=new RankItemLuckyCard();
					rankLuckCard.bytesRead(m,0,arr.Value [m] as ErlArray);
					addLuckyCardRanking(rankLuckCard);
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
	public void addMessageRadio(string name,int vipLevel,string cardType,int cardSid) {
		CardSample cardSample=CardSampleManager.Instance.getRoleSampleBySid(cardSid);
		string qualityName=QualityManagerment.getQualityNameByNone(cardSample.qualityId);
		string qualityColor = QualityManagerment.getQualityColor (cardSample.qualityId);
		string str1;
		if (vipLevel > 0) {
			str1 = "[F85C5C]" + LanguageConfigManager.Instance.getLanguage ("s0509", name, vipLevel.ToString());
		} else {
			str1 = "[F85C5C]" +name;
		}
		string str2 = qualityColor + qualityName + LanguageConfigManager.Instance.getLanguage ("cardName")+cardSample.name;
		string message="[FFFFFF]"+LanguageConfigManager.Instance.getLanguage("s0507",str1)+"[FFFFFF]"+LanguageConfigManager.Instance.getLanguage("s0561",str2);
		RadioManager.Instance.M_addRadioMsg (RadioManager.RADIO_LUCKY_CARD_TYPE,message);
	}
	/// <summary>
	/// 添加限时抽卡排行榜
	/// </summary>
	public void addLuckyCardRanking(RankItemLuckyCard rankLuckCard) {
		RankManagerment.Instance.luckyCardList.Add (rankLuckCard);
	}
}