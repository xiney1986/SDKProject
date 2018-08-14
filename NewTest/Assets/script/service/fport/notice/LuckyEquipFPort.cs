using UnityEngine;
using System.Collections;

/// <summary>
/// 限时抽取装备通讯端口
/// </summary>
public class LuckyEquipFPort :  BaseFPort {
	
	/** 回调函数<活动积分,活动排名> */
	private CallBack<int,int> callback;
	
	public void access (int noticeSid,CallBack<int,int> callback) {
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.NOTICE_LUCKY_DRAW);
		message.addValue ("noticeSid", new ErlInt (noticeSid));
		access (message);
	}
	/** 读取数据 */
	public override void read (ErlKVMessage message) {
		ErlType erlType = message.getValue ("base") as ErlType;
		int integral = 0;
		int rank = -1;
		if (erlType is ErlArray) {
			ErlArray arr= erlType as ErlArray;
			int index=0;
			// 活动积分
			integral=StringKit.toInt (arr.Value [index++].getValueString ());
			// 活动排名
			rank = StringKit.toInt (arr.Value [index++].getValueString ());
		}
		RadioManager.Instance.clearByType(RadioManager.RADIO_LUCKY_EQUIP_TYPE);
		ErlType radioErlType = message.getValue ("radio") as ErlType;
		if (radioErlType is ErlArray) {
			ErlArray arr= radioErlType as ErlArray;
			if (arr.Value.Length>0) {
				for (int m = 0,count=arr.Value.Length; m < count; m++) {
					ErlArray radioArray = arr.Value [m] as ErlArray;
					string name=radioArray.Value [0].getValueString ();
					int vipLevel=StringKit.toInt(radioArray.Value [1].getValueString ());
					string propType=radioArray.Value [2].getValueString ();
					int propSid=StringKit.toInt(radioArray.Value [3].getValueString ());
					addMessageRadio(name,vipLevel,propType,propSid);
				}
			}
		}
		RankManagerment.Instance.luckyEquipList.Clear ();
		ErlType rankErlType = message.getValue ("rank") as ErlType;
		if (rankErlType is ErlArray) {
			ErlArray arr= rankErlType as ErlArray;
			if (arr.Value.Length>0) {
				RankItemLuckyEquip rankLuckEquip;
				for (int m = 0,count=arr.Value.Length; m<count; m++) {
					rankLuckEquip=new RankItemLuckyEquip();
					rankLuckEquip.bytesRead(0,arr.Value [m] as ErlArray);
					addLuckyEquipRanking(rankLuckEquip);
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
	public void addMessageRadio(string name,int vipLevel,string propType,int propSid) {
		string qualityName="";
		string qualityColor="";
		string propName="";
		if(propType==TempPropType.EQUIPMENT) {
			EquipSample equipSample=EquipmentSampleManager.Instance.getEquipSampleBySid(propSid);
			qualityName=QualityManagerment.getQualityNameByNone(equipSample.qualityId);
			qualityColor = QualityManagerment.getQualityColor (equipSample.qualityId);
			propName=equipSample.name;
		} else if(propType==TempPropType.GOODS) {
			PropSample propSample=PropSampleManager.Instance.getPropSampleBySid(propSid);
			qualityName=QualityManagerment.getQualityNameByNone(propSample.qualityId);
			qualityColor = QualityManagerment.getQualityColor (propSample.qualityId);
			propName=propSample.name;
		}
		string str1;
		if (vipLevel > 0) {
			str1 = "[FF0000]" + LanguageConfigManager.Instance.getLanguage ("s0509", name, vipLevel.ToString());
		} else {
			str1 = "[FF0000]" +name;
		}
		string str2 = qualityColor + qualityName + LanguageConfigManager.Instance.getLanguage ("equipName")+propName;
		string message="[FFFFFF]"+LanguageConfigManager.Instance.getLanguage("s0507",str1)+"[FFFFFF]"+LanguageConfigManager.Instance.getLanguage("s0561",str2);
		RadioManager.Instance.M_addRadioMsg (RadioManager.RADIO_LUCKY_EQUIP_TYPE,message);
	}
	/// <summary>
	/// 添加限时抽装备排行榜
	/// </summary>
	public void addLuckyEquipRanking(RankItemLuckyEquip rankLuckEquip) {
		RankManagerment.Instance.luckyEquipList.Add (rankLuckEquip);
	}
}
