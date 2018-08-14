using UnityEngine;
using System.Collections;


public class HappySundayService : BaseFPort 
{

    public HappySundayService()
	{
	}
	
	public override void read (ErlKVMessage message)
	{
        int score = StringKit.toInt((message.getValue("msg") as ErlType).getValueString());
        HappySundayManagerment.Instance.UpdateScore(score);
	}

	
}
