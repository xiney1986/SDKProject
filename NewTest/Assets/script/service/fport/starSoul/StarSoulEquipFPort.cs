using UnityEngine;
using System.Collections;

/// <summary>
/// 星魂装备通讯端口
/// </summary>
public class StarSoulEquipFPort : BaseFPort {
	
	/** 回调函数 */
	private CallBack callback;

	/// <summary>
	/// 星魂强化
	/// </summary>
	/// <param name="uid">Uid</param>
	/// <param name="exp">强化所用的经验值</param>
	/// <param name="callback">Callback.</param>
	public void doStrengStarSoulAccess (string uid,long exp,CallBack callback) {
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.STARSOUL_FLOOD_EXP);
		message.addValue ("star_soul_uid", new ErlString (uid));
		message.addValue ("exp", new ErlString (exp.ToString()));
		access (message);
	}
	/// <summary>
	/// 星魂穿戴-替换
	/// </summary>
	/// <param name="cardUid">卡片uid</param>
	/// <param name="uid">星魂uid</param>
	/// <param name="hole">星魂槽下标</param>
	/// <param name="callback">Callback.</param>
	public void putOnEquipStarSoulAccess (string cardUid,string uid,int hole,CallBack callback) {
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.STARSOUL_USE);
		message.addValue ("card_uid", new ErlString (cardUid));
		message.addValue ("hole", new ErlString (hole.ToString()));
		message.addValue ("star_soul_uid", new ErlString (uid));
		access (message);
	}
	/// <summary>
	/// 星魂卸下
	/// </summary>
	/// <param name="cardUid">卡片uid</param>
	/// <param name="hole">星魂槽下标</param>
	/// <param name="callback">Callback.</param>
	public void putOffEquipStarSoulAccess (string cardUid,int hole,CallBack callback) {
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.STARSOUL_NONUSE);
		message.addValue ("card_uid", new ErlString (cardUid));
		message.addValue ("hole", new ErlString (hole.ToString()));
		access (message);
	}
	/// <summary>
	/// 回调读取通讯
	/// </summary>
	/// <param name="message">Message.</param>
	public override void read (ErlKVMessage message) {
		string msgInfo = (message.getValue ("msg") as ErlType).getValueString ();
		if (msgInfo=="streng") {
			doStrengStarSoul(message);
		} else if (msgInfo == "puton") {
			doPutonStarSoul(message);
		} else if (msgInfo == "putoff") {
			doPutoffStarSoul(message);
		}
	}
	/// <summary>
	/// 强化星魂通讯读取处理
	/// </summary>
	/// <param name="message">Message.</param>
	private void doStrengStarSoul(ErlKVMessage message) {
		ErlType erlType = message.getValue ("value") as ErlType;
		if (erlType is ErlArray) {
			ErlArray arr= erlType as ErlArray;
			int index=0;
			string uid= arr.Value [index++].getValueString ();
			long exp= StringKit.toLong (arr.Value [index++].getValueString ());
			StorageManagerment smanager=StorageManagerment.Instance;
			StarSoulManager manager=StarSoulManager.Instance;
			StarSoul starSoul=smanager.getStarSoul(uid);
			if(starSoul!=null) {
				manager.delStarSoulExp(exp);
				starSoul.updateExp(starSoul.getEXP()+exp);
				starSoul.isNew=false;
			}
			StorageManagerment.Instance.starSoulStorageVersion++;
			if(callback!=null){
				callback();
				callback=null;
			}
		} else {
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, erlType.getValueString (),null);
			});
		}
	}
	/// <summary>
	/// 穿,替换星魂通讯读取处理
	/// </summary>
	/// <param name="message">Message.</param>
	private void doPutonStarSoul(ErlKVMessage message){
		ErlType erlType = message.getValue ("value") as ErlType;
		StarSoulManager manager=StarSoulManager.Instance;
		manager.clearActiveCard();
		if (erlType is ErlArray) {
			ErlArray arr= erlType as ErlArray;
			int index=0;
			// 卡片
			string cardUid= arr.Value [index++].getValueString ();
			// 星魂槽位
			int hole= StringKit.toInt (arr.Value [index++].getValueString ());
			// 星魂uid
			string starsoulUid= arr.Value [index++].getValueString ();

			StarSoulManager.Instance.setSoulStarState(cardUid,hole,starsoulUid);
//			StorageManagerment smanager=StorageManagerment.Instance;
//			Card card=smanager.getRole(cardUid);
//			if(card!=null) {
//				// 设置被替换的星魂状态为未装备(如果是直接穿装备则不执行)
//				StarSoulBore oldStarSoulBore=card.getStarSoulBoreByIndex(hole);
//				if(oldStarSoulBore!=null){
//					StarSoul oldStarSoul=smanager.getStarSoul(oldStarSoulBore.getUid());
//					if(oldStarSoul!=null) {
//						oldStarSoul.unState(EquipStateType.OCCUPY);
//						oldStarSoul.isNew=false;
//					}
//				}
//				// 设置被穿的星魂状态为装备
//				StarSoul starSoul=smanager.getStarSoul(starsoulUid);
//				if(starSoul!=null) {
//					starSoul.setState(EquipStateType.OCCUPY);
//					starSoul.isNew=false;
//				}
//				card.addStarSoulBore(starsoulUid,hole);
//			}
//			StorageManagerment.Instance.starSoulStorageVersion++;
			if(callback!=null){
				callback();
				callback=null;
			}
		} else {
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, erlType.getValueString (),null);
			});
		}
	}
	/// <summary>
	/// 卸星魂通讯读取处理
	/// </summary>
	/// <param name="message">Message.</param>
	private void doPutoffStarSoul(ErlKVMessage message){
		ErlType erlType = message.getValue ("value") as ErlType;
		if (erlType is ErlArray) {
			ErlArray arr= erlType as ErlArray;
			int index=0;
			string cardUid= arr.Value [index++].getValueString ();
			int hole= StringKit.toInt (arr.Value [index++].getValueString ());
			string starsoulUid= arr.Value [index++].getValueString ();
			StarSoulManager.Instance.delSoulStarState(cardUid,hole);
			StorageManagerment smanager=StorageManagerment.Instance;
			Card card=smanager.getRole(cardUid);
			if(card!=null) {
				card.delStarSoulBoreByIndex(hole);
			}
			StorageManagerment.Instance.starSoulStorageVersion++;
			if(callback!=null) {
				callback();
				callback=null;
			}
		} else {
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, erlType.getValueString (),null);
			});
		}
	}
}