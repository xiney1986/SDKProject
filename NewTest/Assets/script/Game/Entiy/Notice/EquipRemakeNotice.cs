using UnityEngine;
using System.Collections;

/// <summary>
/// 装备提升活动
/// </summary>
public class EquipRemakeNotice : Notice {
	
	public ActiveTime activeTime;
	
	public EquipRemakeNotice (int sid):base(sid) {
		this.sid = sid;
	}
	/** 是否有效 */
	public override bool isValid () {
		NoticeSample noticeSample = getSample ();
		EquipRemakeNoticeContent noticeContent=noticeSample.content as EquipRemakeNoticeContent;
		if(noticeContent!=null) {
			User user=UserManager.Instance.self;
			if(user.getVipLevel()<noticeContent.getShowVipLevel()||user.getUserLevel()<noticeContent.getShowUserLevel())
				return false;
		}
		activeTime = ActiveTime.getActiveTimeByID (getSample ().timeID);
		if (activeTime.getIsFinish ())
			return false;
		object obj=NoticeActiveManagerment.Instance.getActiveInfoBySid(sid);
		if(obj is DoubleRMBInfo) {
			DoubleRMBInfo doubleRMBInfo=obj as DoubleRMBInfo;
			if(doubleRMBInfo!=null&&doubleRMBInfo.state)
				return false;
		}
		return ServerTimeKit.getSecondTime () >= activeTime.getPreShowTime ();
	}
}
