using UnityEngine;
using System.Collections;

public class SweepGetAwardFPort : BaseFPort
{
	private CallBack<Award> callback;

	/// <summary>
	/// 获取扫荡奖励信息
	/// </summary>
	/// <param name="callback">Callback.</param>
	public void getSweepAwardInfo (CallBack<Award> _callback)
	{
		callback = _callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.SWEEP_AWARD);
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		base.read (message); 

		ErlType msg = message.getValue ("msg") as ErlType;
		
		if (msg is ErlArray) {
			ErlArray array = msg as ErlArray;
			
			if (array == null)
				return;
			
			Award award = new Award ();
			(ServiceManager.Instance.getServiceByCmd(FPortService.AWARD) as AwardService).parse (array, award);
			
			if (callback != null) {
				callback (award);
			}
			SweepManagement.Instance.calculatePvePoint();
		} else if (msg.getValueString() == "no_award") {
			//Award award = new Award ();			
			if (callback != null) {
				callback (null);
			}
			SweepManagement.Instance.rebackCost();
		} else {
			string str = (message.getValue ("msg") as ErlAtom).Value;

			if (str == "already_finish") {
				SweepManagement.Instance.clearSweepAward ();
				UiManager.Instance.openMainWindow ();
				UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("s0120"));
			}
		}
		

	}
}
