using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SevenDaysHappyInfoFPort : BaseFPort
{
	private CallBack callBack;
	Dictionary<int,MissonFromServer> dic = new Dictionary<int, MissonFromServer>();

	public void SevenDaysHappInfoAccess(CallBack _callBack)
	{
		this.callBack = _callBack;
		
		ErlKVMessage message = new ErlKVMessage (FrontPort.SEVENDAYSHAPPY_INFO);
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		//SevenDaysHappyManagement.Instance.canReceviedCount = 0;
		dic.Clear();
		MissonFromServer misson;
		ErlArray info = message.getValue ("msg") as ErlArray;
		SevenDaysHappyManagement.Instance.calculateActiveOverTime(StringKit.toInt(info.Value[0].getValueString()));
		ErlArray infoArr = info.Value[1] as ErlArray;
		ErlArray missonInfo;
		ErlArray progressInfo;
		for(int i=0;i<infoArr.Value.Length;i++)
		{
			missonInfo = infoArr.Value[i] as ErlArray;
			misson = new MissonFromServer();
			misson.missonID = StringKit.toInt(missonInfo.Value[0].getValueString());
			int state = StringKit.toInt(missonInfo.Value[1].getValueString());
			if(state == 0)// 进行中//
			{
				misson.missonState = SevenDaysHappyMissonState.Doing;
			}
			else if(state == 1)// 已经完成,还没有领奖//
			{
				misson.missonState = SevenDaysHappyMissonState.Completed;
				//SevenDaysHappyManagement.Instance.canReceviedCount++;
			}
			else if(state == 2)// 已经领取奖励//
			{
				misson.missonState = SevenDaysHappyMissonState.Recevied;
			}

			progressInfo = missonInfo.Value[2] as ErlArray;
			misson.missonProgress = new int[progressInfo.Value.Length];
			if(progressInfo.Value.Length > 0 )
			{
				for(int j=0;j<progressInfo.Value.Length;j++)
				{
					misson.missonProgress[j] = StringKit.toInt(progressInfo.Value[j].getValueString());
				}
			}

			dic.Add(misson.missonID,misson);
		}

		SevenDaysHappyManagement.Instance.initData(dic);
		SevenDaysHappyManagement.Instance.sortMisson();

		if(callBack != null)
		{
			callBack();
		}
	}
}
