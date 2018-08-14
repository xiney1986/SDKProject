using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 无敌幸运星投掷结果窗口
/// @author 丁杰
/// </summary>
public class GuildShakeElbowsResultWindow : WindowBase
{
	/** 奖励描述 */
	public UILabel awardDes;
	/** 重投按钮 */
	public ButtonBase buttonReshake;
	/** 领取按钮 */
	public ButtonBase buttonReceive;
	/** 骰子 */
	public EblowView[] eblowViews;
	/** 骰子 */
	public GuildShakeRewardContent shakeRewardContent;
	/** 结果数据 */
	public GuildLuckyNvShenShakeResult shakeResultData;
	public const int LOCK_MAX = 4;
	public void Init(GuildLuckyNvShenShakeResult resultData)
	{
		this.shakeResultData = resultData;
		/** 初始化结果显示 */
		string [] resultStrs = shakeResultData.getResultsString ();
		for (int i =0 ; i<resultStrs.Length ; i++) {
			eblowViews[i].Init(resultStrs[i]);
		}
		/** 初始化结果描述 */
		List<PrizeSample> rewards = ShakeEblowsRewardSampleManager.Instance().GetPrizeByResult(shakeResultData);
		awardDes.text = Language ("GuildLuckyNvShen_15");
		foreach(PrizeSample p in rewards)
		{
			awardDes.text += p.getPrizeName() + "+" + p.num + ",";
		}
		awardDes.text = awardDes.text.Substring (0,awardDes.text.Length - 1);

		/** 初始化按钮显示 */
		GuildLuckyNvShenInfo info = GuildManagerment.Instance.getGuildLuckyNvShenInfo ();
		if (info != null) {
			if(info.reShakeCount<=0){
				buttonReshake.textLabel.text = Language("GuildLuckyNvShen_17") + "(0)";
				buttonReshake.GetComponent<UIButton>().isEnabled = false;
			}
			else
			{
				buttonReshake.textLabel.text = Language("GuildLuckyNvShen_17") + "("+info.reShakeCount +")";
				buttonReshake.GetComponent<UIButton>().isEnabled = true;
			}
		}
		/** 初始化规则描述 */
		List<ShakeEblowsRewardSample> allSample = ShakeEblowsRewardSampleManager.Instance ().GetNormalShakeEblowRewardSamples ();
		shakeRewardContent.Init (allSample);
		shakeRewardContent.otherDes.text = Colors.RED + ShakeEblowsRewardSampleManager.Instance ().GetFiveDiffSample ().getPrizesDesc ();
	
		/** 初始化加锁信息 */
		lockEblows (GuildLuckyNvShenWindow.lockString);
		//MaskWindow.UnlockUI();
	}



	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "ButtonReshake") {
			windowAnim = GUI_anim_class.None;
			this.finishWindow ();
			if (fatherWindow is GuildLuckyNvShenWindow) {
				string locksString = getLocksString ();
				(fatherWindow as GuildLuckyNvShenWindow).beginReshake (locksString);
			}
		} else if (gameObj.name == "ButtonReceive") {
			this.finishWindow ();
			if (fatherWindow is GuildLuckyNvShenWindow) {
				(fatherWindow as GuildLuckyNvShenWindow).getShakeReward ();
			}
		} else if (gameObj.name.Contains("eblow")) {
			EblowView eblow = gameObj.GetComponent<EblowView>();
			if(isCanLock() || eblow.getLockState() ==1)
			{
				gameObj.GetComponent<EblowView>().changeClockState();
			}
			else
			{
				MessageWindow.ShowAlert(Language("GuildLuckyNvShen_22"));
			}
		}
	}


	/// <summary>
	/// 获取加锁信息
	/// </summary>
	public string getLocksString(){
		string lockString = "";
		foreach (EblowView e in eblowViews) {
			lockString += e.getLockState().ToString() + ",";
		}
		lockString = lockString.Substring(0,lockString.Length-1);
		return lockString;
	}

	public void lockEblows(string lockString){
		if (string.IsNullOrEmpty (lockString))
			return;
		string [] strs = lockString.Split (',');
		if (strs.Length != 5) 
			return;
		for (int i = 0; i<strs.Length; ++i) {
			if(strs[i] == "1")
			{
				eblowViews[i].setLockState(true);
			}
		}

	}

	public bool isCanLock()
	{
		int i = 0;
		foreach (EblowView e in eblowViews) {
		
			i +=e.getLockState();
		}
		if (i < LOCK_MAX)
			return true;
		else
			return false;
	}


}

