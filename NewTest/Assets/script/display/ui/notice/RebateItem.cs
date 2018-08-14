using UnityEngine;
using System.Collections;

public class RebateItem : ButtonBase {

	RebateDayInfo rdi;
	public GameObject effects;// 可领取特效//
	public GameObject collectEffects;// 收集特效//
	public UISprite icon;
	public UISprite stateIcon;

	string notJoinSpriteName = "rebate_notJoin";
	string receivedSpriteName = "rebate_recevied";
	string waittingSpriteName = "rebate_waiting";

	RebateContent rc;

	public void updateRebateItem(RebateDayInfo rdi)
	{
		this.rdi = rdi;
		if(rdi.rebateState == RebateState.NOT_JOIN)// 未参加活动//
		{
			stateIcon.spriteName = notJoinSpriteName;
			stateIcon.gameObject.SetActive(true);
			effects.SetActive(false);
			collectEffects.SetActive(false);
		}
		else if(rdi.rebateState == RebateState.UN_RECEIVE)// 未领取//
		{
			if(rdi.s_rebateState == S_RebateState.CAN_RECEIVE)// 可领取该福袋//
			{
				effects.SetActive(true);
				stateIcon.gameObject.SetActive(false);
				collectEffects.SetActive(false);
			}
			else if(rdi.s_rebateState == S_RebateState.COLLECTING)// 收集中//
			{
				collectEffects.SetActive(true);
				if(rc != null)
				{
					rc.effectObj = collectEffects;
				}
				stateIcon.gameObject.SetActive(false);
				effects.SetActive(false);
			}
			else if(rdi.s_rebateState == S_RebateState.WAIT_RECEIVE)// 等待中//
			{
				stateIcon.spriteName = waittingSpriteName;
				stateIcon.gameObject.SetActive(true);
				effects.SetActive(false);
				collectEffects.SetActive(false);
			}

		}
		else if(rdi.rebateState == RebateState.RECEIVED)// 已领取//
		{
			stateIcon.spriteName = receivedSpriteName;
			stateIcon.gameObject.SetActive(true);
			effects.SetActive(false);
			collectEffects.SetActive(false);
		}
		else if(rdi.rebateState == RebateState.NOT_ON_TIME)// 未到活动时间 表现为常态//
		{
			stateIcon.gameObject.SetActive(false);
			effects.SetActive(false);
			collectEffects.SetActive(false);
		}
	}

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		if(rdi.rebateState == RebateState.NOT_JOIN || rdi.rebateState == RebateState.RECEIVED || rdi.rebateState == RebateState.NOT_ON_TIME)// 未参加或已领取或未到活动时间无法点//
		{
			return;
		}

		if(rc.effectObj != null)
		{
			rc.effectObj.SetActive(false);
		}
		rc.setInfo(rdi);
		rc.showDetailInfo();
	}

	public void setRebateContent(RebateContent _rc)
	{
		rc = _rc;
	}
}
