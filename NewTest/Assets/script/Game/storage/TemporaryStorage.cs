using System;
using System.Collections;

/**
 * 临时仓库
 * @author longlingquan
 * */
public class TemporaryStorage:Storage
{
	public TemporaryStorage ()
	{
	}
	//根据临时道具uid获得临时道具
	public TempProp getPropByTempUid(string tempUid){
		ArrayList al=getStorageProp();
		for (int i=0,l=al.Count; i<l; i++) {
			TempProp sp = al [i]  as TempProp;
			if (sp.tempUid == tempUid) {
				sp.index = i;
				return sp;
			}
		}
		return null;
	}
	//根据临时道具uid获得临时道具，返回一组数据，没有的补null
	public TempProp[] getPropByTempUid (string[] tempUids)
	{
		int l = tempUids.Length;
		TempProp[] sps = new TempProp[l];
		for (int i=0; i<l; i++) {
			sps [i] = getPropByTempUid (tempUids [i]);
		}
		return sps;
	}

	public override void parse (ErlArray arr)
	{ 
		ErlArray ea1 = arr.Value [1] as ErlArray;
		if (ea1.Value.Length <= 0) {
			init (StringKit.toInt (arr.Value [0].getValueString ()), null);
		} else {
			ArrayList al = new ArrayList ();
			TempProp tp;
			for (int i=0; i < ea1.Value.Length; i++) {
				tp=TempManagerment.Instance.createTempProp ();
				tp.bytesRead(0,ea1.Value [i] as ErlArray);
				//后台索引是从1开始
				al.Add (tp);
			}
			init (StringKit.toInt (arr.Value [0].getValueString ()), al);
		}
	}
}

