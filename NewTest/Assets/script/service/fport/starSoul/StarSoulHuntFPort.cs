using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 猎杀星魂端口
/// </summary>
public class StarSoulHuntFPort : BaseFPort {

	/** 裂魂回调函数(<星魂列表,碎片数,星魂库id>) */
	private CallBack<ArrayList,int,int> callback;

	/// <summary>
	/// 单次裂魂通讯
	/// </summary>
	/// <param name="huntType">裂魂类型(0=普通,1=钻石)</param>
	/// <param name="callback">Callback.</param>
	public void huntAccess (int huntType,CallBack<ArrayList,int,int> callback) {
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.STARSOUL_HUNT);
		message.addValue ("type", new ErlInt (huntType));
		access (message);
	}
	/// <summary>
	/// 一键裂魂裂魂通讯
	/// </summary>
	/// <param name="huntType">裂魂类型(0=普通,1=钻石)</param>
	/// <param name="callback">Callback.</param>
	public void autoHuntAccess (int huntType,CallBack<ArrayList,int,int> callback) {
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.STARSOUL_AUTO_HUNT);
		message.addValue ("type", new ErlInt (huntType));
		access (message);
	}

	/// <summary>
	/// 回调读取通讯
	/// </summary>
	/// <param name="message">Message.</param>
	public override void read (ErlKVMessage message) {
		int index = 0;
		ErlType erlType = message.getValue ("msg") as ErlType;
		if (erlType is ErlArray) {
			ErlArray arr= erlType as ErlArray;
			int huntType= StringKit.toInt (arr.Value [index++].getValueString ());
			if (huntType==StarSoulConfigManager.HUNT_MONEY_TYPE) { // 普通裂魂
				huntStarSoul(index,arr);
			}
			else if (huntType==StarSoulConfigManager.HUNT_RMB_TYPE) { // 钻石裂魂
				HuntStarSoulByRmb(index,arr);
			}	
		}
		else {
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, erlType.getValueString (),null);
			});
		}
	}

	/// <summary>
	/// 普通裂魂
	/// </summary>
	private void huntStarSoul(int index, ErlArray arr) {
		StarSoulManager manager=StarSoulManager.Instance;
		StorageManagerment smanager=StorageManagerment.Instance;
		ArrayList starSouls=new ArrayList();
		// 猎魂库sid
		int huntLibSid= StringKit.toInt (arr.Value [index++].getValueString ());
		manager.setHuntQuality (huntLibSid);
		ErlArray starSoulArr =arr.Value [index++] as ErlArray;
		if (starSoulArr != null && starSoulArr.Value.Length>0) {
			StarSoul starSoul;
			for (int i = 0; i < starSoulArr.Value.Length; i++) {
				starSoul=manager.createStarSoul(starSoulArr.Value[i] as ErlArray);
				smanager.addHuntStarSoulStorage(starSoul,true);
				starSoul.isNew=false;
				starSouls.Add(starSoul);
			}
			StorageManagerment.Instance.huntStarSoulStorageVersion++;
		}
		int num = StringKit.toInt(arr.Value[index++].getValueString());
		manager.setDebrisNumber(manager.getDebrisNumber()+num);
		if (callback != null) {
			callback (starSouls,num,huntLibSid);
			callback= null;
		}
	}

	/// <summary>
	/// 钻石裂魂
	/// </summary>
	private void HuntStarSoulByRmb(int index, ErlArray arr) {
		StarSoulManager manager=StarSoulManager.Instance;
		StorageManagerment smanager=StorageManagerment.Instance;
		ArrayList starSouls=new ArrayList();
		int debrisNumber= StringKit.toInt (arr.Value [index++].getValueString ());
		if (debrisNumber > 0) {
			manager.addDebrisNumber(debrisNumber);		
		}
		ErlArray starSoulArr =arr.Value [index++] as ErlArray;
		if (starSoulArr != null && starSoulArr.Value.Length>0) {
			StarSoul starSoul;
			for (int i = 0; i < starSoulArr.Value.Length; i++) {
				starSoul=manager.createStarSoul(starSoulArr.Value[i] as ErlArray);
				smanager.addHuntStarSoulStorage(starSoul,true);
				starSoul.isNew=false;
				starSouls.Add(starSoul);
			}
			StorageManagerment.Instance.huntStarSoulStorageVersion++;
		}
		if (callback != null) {
			callback (starSouls, debrisNumber,0);
			callback= null;
		}
	}
}
