using System;
using System.Collections;

/**
 * 召唤兽仓库
 * @author longlingquan
 * */
public class BeastStorage:Storage
{
	public BeastStorage ()
	{
		
	}
	public override void parse (ErlArray arr)
	{
		ErlArray ea1 = arr.Value [1] as ErlArray;
		if (ea1.Value.Length <= 0) {
			init (StringKit.toInt (arr.Value [0].getValueString ()), null);
		} else {
			ArrayList al = new ArrayList ();
			Card card;
			for (int i=0; i < ea1.Value.Length; i++) {
				card = CardManagerment.Instance.createCard();
				card.bytesRead(0,ea1.Value [i] as ErlArray);
				al.Add (card);
			}
			init (StringKit.toInt (arr.Value [0].getValueString ()), al); 
		}
	}
	 
	 
} 

