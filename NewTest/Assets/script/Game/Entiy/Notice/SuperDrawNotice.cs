using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SuperDrawNotice : Notice {
	public ActiveTime activeTime;
	public SuperDrawNotice (int sid): base(sid)
	{
		this.sid = sid;
	}
	
	public override bool isValid () {
		//TODO   取时间
//		activeTime = ActiveTime.getActiveTimeByID (getSample ().timeID);
		//这里用兑换商店的持续时间来确定活动的开启
		int[] sids = (getSample().content as SidNoticeContent).sids;//获取商店的时间sid
		activeTime = ActiveTime.getActiveTimeByID(sids[0]);
        if (activeTime.getIsFinish ())
			return false;
        if (UserManager.Instance.self.getUserLevel() < getSample().levelLimit)
            return false;
        return ServerTimeKit.getSecondTime () >= activeTime.getPreShowTime ();
	}

    public override string ToString()
    {
        List<PrizeSample> list = SuperDrawSampleManager.Instance.getSuperDrawSampleBySid(sid).list;
        string info1 = "";
        for (int i = 0; i < list.Count; i++)
        {
            info1 += list[i];
            if (i < list.Count - 1)
            {
                info1 += "，";
            }
        }
        return base.ToString()+"\t"+ info1;
    }
}
