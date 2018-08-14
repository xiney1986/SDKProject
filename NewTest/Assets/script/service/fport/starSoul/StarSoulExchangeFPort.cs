using UnityEngine;
using System.Collections;

/// <summary>
/// 兑换星魂通讯
/// </summary>
public class StarSoulExchangeFPort : BaseFPort {
	
	/** 兑换回调函数<兑换的经验,转化经验的星魂> */
	private CallBack<int,StarSoul[]> callback;
	
	/// <summary>
	/// 点击筛选兑换通讯
	/// </summary>
	/// <param name="uids">兑换的星魂uid集</param>
	/// <param name="callback">Callback.</param>
	public void exchangeAccess (string convertinfo,CallBack<int,StarSoul[]> callback) {
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.STARSOUL_CONVERT_EXP);		
		message.addValue ("star_soul_uid", new ErlString (convertinfo));
		access (message);
	}
//	/// <summary>
//	/// 一键转化经验通讯
//	/// </summary>
//	/// <param name="storageType">仓库类型(star_soul_storage=星魂仓库,star_soul_draw_storage=猎魂仓库)</param>
//	/// <param name="callback">Callback.</param>
//	public void autoExchangeAccess(string storageType,CallBack<int,StarSoul[]> callback) {
//		this.callback = callback;
//		ErlKVMessage message = new ErlKVMessage (FrontPort.STARSOUL_AUTO_CONVERT_EXP);		
//		message.addValue ("storage_type", new ErlString (storageType));
//		access (message);
//	}
	
	/// <summary>
	/// 回调读取通讯
	/// </summary>
	/// <param name="message">Message.</param>
	public override void read (ErlKVMessage message) {
		int index = 0;
		ErlType erlType = message.getValue ("msg") as ErlType;
		if (erlType is ErlArray) {
			ErlArray arr= erlType as ErlArray;
			StarSoulManager manager=StarSoulManager.Instance;
			StorageManagerment smanager=StorageManagerment.Instance;
			// 仓库类型
			string storageType= arr.Value [index++].getValueString ();
			// 转化的经验
			int exchangeExp= StringKit.toInt (arr.Value [index++].getValueString ());
			if(exchangeExp>0){
				manager.addStarSoulExp(exchangeExp);
			}
			StarSoul[] exchangeStarSouls=null;
			StarSoul[] popStorageStarSouls=null;
			bool isUpdateStorageVersion=false;
			bool isUpdateHuntStorageVersion=false;
			// 转换经验的星魂
			ErlType exchangeStarSoulType=arr.Value [index++];
			if (exchangeStarSoulType is ErlArray) {
				ErlArray exchangeStarSoulArr =exchangeStarSoulType as ErlArray;
				if (exchangeStarSoulArr.Value.Length>0) {
					exchangeStarSouls=new StarSoul[exchangeStarSoulArr.Value.Length];
					StarSoul starSoul=null;
					for (int i = 0; i < exchangeStarSoulArr.Value.Length; i++) {
						if(storageType == "star_soul_storage") { // 清理星魂仓库
							starSoul=smanager.getStarSoul(exchangeStarSoulArr.Value[i].getValueString());
							if(starSoul==null) continue;
							smanager.delStarSoulStorage(starSoul.uid);
							isUpdateStorageVersion=true;
						} else if(storageType == "star_soul_draw_storage") { // 清理猎魂仓库
							starSoul=smanager.getHuntStarSoul(exchangeStarSoulArr.Value[i].getValueString());
							if(starSoul==null) continue;
							smanager.delHuntStarSoulStorage(starSoul.uid);
							isUpdateHuntStorageVersion=true;
						}
						exchangeStarSouls[i]=starSoul;
					}
				}
			}
			if(isUpdateStorageVersion) StorageManagerment.Instance.starSoulStorageVersion++;
			if(isUpdateHuntStorageVersion) StorageManagerment.Instance.huntStarSoulStorageVersion++;
			if (callback != null) {
				callback (exchangeExp, exchangeStarSouls);
				callback= null;
			}
		}
		else {
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, erlType.getValueString (),null);
			});
		}
	}
}
