using UnityEngine;
using System.Collections.Generic;

public class HappySundayManagerment
{

    public static HappySundayManagerment Instance
    {
        get { return SingleManager.Instance.getObj("HappySundayManagerment") as HappySundayManagerment; }
    }



    public int CurrentScore = 0;
    public List<int> ReceiveSidList;


    private CallBack mInitCallback;


	public HappySundayManagerment()
	{
	}


    public void InitData(CallBack callback)
    {
        mInitCallback = callback;
        HappySundayInit fport = FPortManager.Instance.getFPort("HappySundayInit") as HappySundayInit;
        fport.access(onReceiveInit);
    }


    public void onReceiveInit(int score, List<int> receiveSid)
    {
        CurrentScore = score;
        ReceiveSidList = receiveSid;
        if (mInitCallback != null)
            mInitCallback();
    }




    public bool IsReceive(int sid)
    {
        return ReceiveSidList.Contains(sid);
    }



    public void UpdateScore(int newScore)
    {
        CurrentScore = newScore;
        //////
        //
    }


    public void AddReceive(int sid)
    {
        ReceiveSidList.Add(sid);
    }


    public int getCanReceiveNum()
    {
        if (ReceiveSidList == null)
        {
            InitData(null);
            return 0;
        }

        int num = 0;
        System.Collections.Hashtable table = HappySundaySampleManager.Instance.samples;
        System.DateTime date = TimeKit.getDateTime(ServerTimeKit.getSecondTime());
        int onlineDay = (ServerTimeKit.getSecondTime() - ServerTimeKit.onlineTime) / 3600 / 24;
        foreach (System.Collections.DictionaryEntry item in table)
        { 
            HappySundaySample sample = item.Value as HappySundaySample;
			if ( sample.OnlineDay > onlineDay || sample.MaxScore >= CurrentScore)//sample.Week != (int)date.DayOfWeek ||
                continue;
            if (!ReceiveSidList.Contains(sample.Sid))
                num++;
        }
        return num;
    }

}
