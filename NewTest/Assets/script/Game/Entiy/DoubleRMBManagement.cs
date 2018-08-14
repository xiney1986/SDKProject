using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoubleRMBManagement : IGMUpdateConfigManager {
	public static DoubleRMBManagement Instance {
		get { return SingleManager.Instance.getObj ("DoubleRMBManagement") as DoubleRMBManagement; }
	}

	public bool IsRecharge;
    public bool isEnd = false;
	public Hashtable MoneyRate;

	public DoubleRMBManagement () {
		MoneyRate = new Hashtable ();
		MoneyRate[NoticeType.DOUBLE_RMB_SID] = new DoubleRMBSample ();
	}

	public bool IsCanShow ( int activeID ) {
		DoubleRMBInfo info = NoticeActiveManagerment.Instance.getActiveInfoBySid (activeID) as DoubleRMBInfo;
		//if (info.state)
		//return false;
		ActiveTime activeTime = ActiveTime.getActiveTimeByID (NoticeSampleManager.Instance.getNoticeSampleBySid (activeID).timeID);

		int now = ServerTimeKit.getSecondTime ();
		if(now>activeTime.getPreShowTime ()&&now<activeTime.getStartTime()&&activeTime.getStartTime()!=-1){
			return true;
		}
		if (activeTime.getIsFinish ())
			return false;
		if (now < activeTime.getPreShowTime () || (now > activeTime.getEndTime () && activeTime.getEndTime () != -1))
			return false;
		return true;
	}

	public Hashtable getSamples () {
		return MoneyRate;
	}

	public void createSample ( int sid ) {
	}

}


public class DoubleRMBSample {
	public float MoneyRate;

	private void parse_MoneyRate ( string v ) {
		float.Parse (v);
	}
}



