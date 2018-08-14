using System;

public class LaddersChestFactorSample
{
	public int[] factors;
	public LaddersChestFactorSample (string str)
	{
		string[] chestIndexFactors=str.Split(',');
		int length=chestIndexFactors.Length;
		factors=new int[length];
		for(int i=0;i<length;i++)
		{
			factors[i]=StringKit.toInt(chestIndexFactors[i]);
		}
	}
}


