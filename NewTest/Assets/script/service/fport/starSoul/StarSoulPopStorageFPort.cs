using UnityEngine;
using System.Collections;

/// <summary>
/// 星魂拾取通讯
/// </summary>
public class StarSoulPopStorageFPort : BaseFPort {
	
	/** 拾取回调函数<转化经验,转化经验的星魂,放入星魂仓库的星魂> */
	private CallBack<int,StarSoul[],StarSoul[]> callback;
	
	/// <summary>
	/// 点击单个拾取通讯
	/// </summary>
	/// <param name="uid">拾取星魂uid</param>
	/// <param name="callback">Callback.</param>
	public void popStorageAccess (string uid,CallBack<int,StarSoul[],StarSoul[]> callback) {
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.STARSOUL_GET);
		message.addValue ("star_soul_uid", new ErlString (uid));
		access (message);
	}
	/// <summary>
	/// 一键拾取通讯
	/// </summary>
	/// <param name="callback">Callback.</param>
	public void autoPopStorageAccess(CallBack<int,StarSoul[],StarSoul[]> callback) {
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.STARSOUL_AUTO_GET);
		int quality = PlayerPrefs.GetInt (UserManager.Instance.self.uid + PlayerPrefsComm.STARSOUL_CHOOSE_QUALITY);
		message.addValue ("quality", new ErlInt (quality));
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
			StarSoulManager manager=StarSoulManager.Instance;
			StorageManagerment smanager=StorageManagerment.Instance;
			// 转化的经验
			int exchangeExp= StringKit.toInt (arr.Value [index++].getValueString ());
			if(exchangeExp>0)
				StarSoulManager.Instance.addStarSoulExp(exchangeExp);
			StarSoul[] exchangeStarSouls=null;
			StarSoul[] popStorageStarSouls=null;
			// 转换经验的星魂
			ErlType exchangeStarSoulType=arr.Value [index++];
			if (exchangeStarSoulType is ErlArray) {
				ErlArray exchangeStarSoulArr =exchangeStarSoulType as ErlArray;
				if (exchangeStarSoulArr.Value.Length>0) {
					exchangeStarSouls=new StarSoul[exchangeStarSoulArr.Value.Length];
					StarSoul starSoul;
					for (int i = 0; i < exchangeStarSoulArr.Value.Length; i++) {
						starSoul=smanager.getHuntStarSoul(exchangeStarSoulArr.Value[i].getValueString());
						if(starSoul==null) continue;
						// 清理猎魂仓库
						smanager.delHuntStarSoulStorage(starSoul.uid);
						// 构建显示数据
						exchangeStarSouls[i]=starSoul;
					}
					StorageManagerment.Instance.huntStarSoulStorageVersion++;
				}
			}
			// 放入星魂仓库的星魂
			ErlType popStorageStarSoulType=arr.Value [index++];
			if (popStorageStarSoulType is ErlArray) {
				ErlArray popStorageStarSoulArr =popStorageStarSoulType as ErlArray;
				if (popStorageStarSoulArr.Value.Length>0) {
					popStorageStarSouls=new StarSoul[popStorageStarSoulArr.Value.Length];
					StarSoul starSoul;
					for (int i = 0; i < popStorageStarSoulArr.Value.Length; i++) {
						starSoul=smanager.getHuntStarSoul(popStorageStarSoulArr.Value[i].getValueString());
						if(starSoul==null) continue;
						// 清理猎魂仓库
						smanager.delHuntStarSoulStorage(starSoul.uid);
						// 添加星魂仓库
						smanager.addStarSoulStorage(starSoul,true);
						// 构建显示数据
						popStorageStarSouls[i]=starSoul;
					}
					StorageManagerment.Instance.starSoulStorageVersion++;
					StorageManagerment.Instance.huntStarSoulStorageVersion++;
				}
			}
			if (callback != null) {
				callback (exchangeExp, exchangeStarSouls,popStorageStarSouls);
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