using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * 诸神战获取当前状态信息
 * @author gc
 * */
public class GodsWarGetStateInfoFPort : BaseFPort
{
	CallBack callback;
  
	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GODSWAR_GETSTATEINFO);
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlType stry = message.getValue ("msg") as ErlType;
		
		if (stry is ErlArray) {
			ErlArray Att = stry as ErlArray;
			int pos = 0;
			string arr = (Att.Value[pos++] as ErlAtom).Value;
			GodsWarManagerment.Instance.StateInfo = changeStatetype (arr);
			ErlType aa = Att.Value [pos++] as ErlType;
			if (aa is ErlArray) {
				ErlArray tt = aa as ErlArray;
				int pott = 0;
				int big_id = StringKit.toInt (tt.Value [pott++].getValueString());
				if(big_id==-1)
					GodsWarManagerment.Instance.godsWarZige = false;
				int yu_ming = StringKit.toInt (tt.Value [pott++].getValueString());
				if(yu_ming==-1)
					GodsWarManagerment.Instance.taoTaiZige = false;
				GodsWarManagerment.Instance.big_id = changetype(big_id);
				GodsWarManagerment.Instance.yu_ming = yu_ming;
			}
			string myRank = (Att.Value[pos++] as ErlAtom).Value;
			GodsWarManagerment.Instance.myFinalRank = changeRankInfo(myRank);
			callback ();
		} else {
			string str = (message.getValue ("msg") as ErlAtom).Value;
			GodsWarManagerment.Instance.StateInfo = changeStatetype (str);
			callback ();
		}
	}

	private int changeStatetype (string type)
	{
		int state;
		switch (type) {
		case "group":
			state = 1;
			break;
		case "eliminate":
			state = 2;
			break;
		case "finals":
			state = 3;
			break;
		case "clean":
		case "arrange":
			state = 4;
			break;
		case "not_open":
			state = 5;
			break;
        case "server_busy":
            state = 6;
            break;
		default:
			state = -1;
			break;
		}
		return state;
	}
	private int changetype(int type)
	{
		int t=0;
		switch (type) {
		case 110:
			t=0;
			break;
		case 111:
			t=1;
			break;
		case 112:
			t=2;
			break;
		default:
			t=2;
			break;
		}
		return t;
	}

	private int changeRankInfo(string str)
	{
		int rank;
		switch (str) {
		case "stage0":
			rank = 1;
			break;
		case "stage1":
			rank = 2;
			break;
		case "stage2":
			rank = 3;
			break;
		case "stage4":
			rank = 4;
			break;
		case "stage8":
			rank = 5;
			break;
		case "stage16":
			rank = 6;
			break;
		default:
			rank = -1; 
			break;
		}
		return rank;
	}
}
