using UnityEngine;
using System.Collections;

/// <summary>
/// 限时抽卡活动通讯端口
/// 本地服务器
/// 萝卜
/// </summary>
public class LuckyCardLocalFPort :  BaseFPort {

	/** 回调函数<活动积分,活动排名> */
	private CallBack<int,int> callback;
	
	public void access (int noticeSid,CallBack<int,int> callback) {
		this.callback = callback;
        ErlKVMessage message = new ErlKVMessage(FrontPort.NOTICE_LUCKY_DRAW);
        message.addValue("noticeSid", new ErlInt(noticeSid));
		access (message);
	}
	/** 读取数据 */
	public override void read (ErlKVMessage message) {
        ErlType erlType = message.getValue("base") as ErlType;
        int integral = 0;
        int rank = -1;
        if (erlType is ErlArray)
        {
            ErlArray arr = erlType as ErlArray;
            int index = 0;
            // 活动积分
            integral = StringKit.toInt(arr.Value[index++].getValueString());
            // 活动排名
            rank = StringKit.toInt(arr.Value[index++].getValueString());
        }
        RadioManager.Instance.clearByType(RadioManager.RADIO_LUCKY_CARD_TYPE);
        ErlType radioErlType = message.getValue("radio") as ErlType;
        if (radioErlType is ErlArray)
        {
            ErlArray arr = radioErlType as ErlArray;
            if (arr.Value.Length > 0)
            {
                for (int m = 0, count = arr.Value.Length; m < count; m++)
                {
                    ErlArray radioArray = arr.Value[m] as ErlArray;
                    string name = radioArray.Value[0].getValueString();
                    int vipLevel = StringKit.toInt(radioArray.Value[1].getValueString());
                    string cardType = radioArray.Value[2].getValueString();
                    int cardSid = StringKit.toInt(radioArray.Value[3].getValueString());
                    addMessageRadio(name, vipLevel, cardType, cardSid);
                }
            }
        }
        RankManagerment.Instance.luckyCardList.Clear();
        ErlType rankErlType = message.getValue("rank") as ErlType;
        if (rankErlType is ErlArray)
        {
            ErlArray arr = rankErlType as ErlArray;
            if (arr.Value.Length > 0)
            {
                RankItemLuckyCard rankLuckCard;
                for (int m = 0, count = arr.Value.Length; m < count; m++)
                {
                    rankLuckCard = new RankItemLuckyCard();
					rankLuckCard.bytesReadLuo(m,1, arr.Value[m] as ErlArray);
                    addLuckyCardRanking(rankLuckCard);
                }
            }
        }
        if (callback != null)
        {
            callback(integral, rank);
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