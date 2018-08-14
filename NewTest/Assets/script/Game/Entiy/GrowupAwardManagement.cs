using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 成长奖励管理器
/// </summary>
public class GrowupAwardMangement {

	/* static methods */
	public static GrowupAwardMangement Instance {
		get {
			GrowupAwardMangement manager = SingleManager.Instance.getObj ("GrowupAwardMangement") as GrowupAwardMangement;
			return manager;
		}
	}

	/* fields */
	//获取奖励状态
	public string getAwardStatas;
	//投资状态
	public string investStatas;
	//预存金额
	public int prestoreMoney;
	//已领取等级
	public string[]  tookLevel;
	//报名时间ID
	public int timeID = -1;
	//等待领取的奖品
	private List<GrowupAwardSample> awards;
	/** 所有奖品列表 */
	private List<GrowupAwardSample> allAwards;
	public ActiveTime activeTime;

	/* methods */
	/// <summary>
	/// 获取所有奖品列表
	/// </summary>
	/// <returns>The all awards list.</returns>
	/// <param name="callback">Callback.</param>
	public void GetAllAwardsList (CallBack callback) {
		this.allAwards = GrowupSampleConfigManager.Instance.GetPrizeSamples ();
		if (callback != null)
			callback ();
		callback = null;
	}
	/// <summary>
	/// 获取所有奖品列表
	/// </summary>
	/// <returns>The all awards list.</returns>
	public List<GrowupAwardSample> GetAllAwardsList () {
		return this.allAwards;
	}
	/// <summary>
	/// 获取现等级可领取奖品的数量
	/// </summary>
	/// <returns>The award number.</returns>
	public int GetAwardNum(){
		int num = 0;
		if(prestoreMoney != 0){
			foreach(var tmp in awards){
				if(StringKit.toInt(tmp.needLevel) <= UserManager.Instance.self.getUserLevel()){
					num++;
				}
			}
		}
		return num;
	}
	/// <summary>
	/// 获取等待领取的奖品列表
	/// </summary>
	/// <returns>The award list.</returns>
	public List<GrowupAwardSample> GetAwardList () {
		return this.awards;
	}
	/// <summary>
	/// 从奖品列表移除一条奖品
	/// </summary>
	/// <param name="level">Level.</param>
	public void RemoveItem (string level) {
		awards.RemoveAll ((prize) => {
			return prize.needLevel == level;
		});
	}
	/// <summary>
	/// 投资一个项目
	/// </summary>
	/// <param name="money">Money.</param>
	public void Invest (int money, CallBack callback) {
		FPortManager.Instance.getFPort<GrowupAwardFPort> ().Invest (money.ToString (), callback);
	}
	/// <summary>
	/// 领取奖品
	/// </summary>
	/// <param name="needLevel">Need level.</param>
	/// <param name="callback">Callback.</param>
	public void GetAward (string needLevel, CallBack callback) {
		FPortManager.Instance.getFPort<GrowupAwardFPort> ().GetGrowupAwardAccess (needLevel, callback);
	}
	/// <summary>
	/// 初始化可领取奖品数据
	/// </summary>
	/// <param name="callback">Callback.</param>
	public void InitAwards (CallBack callback) {
		List<GrowupAwardSample> allPrizes = new List<GrowupAwardSample> ();
		foreach (GrowupAwardSample tmp in GrowupSampleConfigManager.Instance.GetPrizeSamples ()) {
			allPrizes.Add (tmp);
		}
		FPortManager.Instance.getFPort<GrowupAwardFPort> ().GetGrowupInfo (() => {
			foreach (GrowupAwardSample tmp in allPrizes) {
				tmp.prize.num = ((int)(tmp.backPercent * this.prestoreMoney / 100)).ToString ();
			}
			if (tookLevel != null && tookLevel.Length != 0) {
				foreach (string level in tookLevel) {
					allPrizes.RemoveAll ((prize) => {
						return prize.needLevel == level;});
				}
			}
			if( this.prestoreMoney != 0){
				awards = allPrizes;
			}

			if (callback != null)
				callback ();
			callback = null;
		});
	}
	/// <summary>
	/// 是否有效，请先初始化奖品数据
	/// </summary>
	public  bool isValid () {
		if (timeID == -1)
			return false;
		activeTime = ActiveTime.getActiveTimeByID (timeID);
		if (activeTime.getIsFinish ()) {
			return false;
		}
		return ServerTimeKit.getSecondTime () >= activeTime.getPreShowTime ();
	}
}