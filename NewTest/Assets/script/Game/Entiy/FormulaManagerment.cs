using System;
 
/**
 * 公式管理器
 * @author longlingquan
 * */
public class FormulaManagerment
{
	public FormulaManagerment ()
	{
		
	}
	
	public static int formula (int fid, int num, int[] arr)
	{ 
		if (fid == 1) {
			return formula1 (num, arr [0]);
		} else if (fid == 2) {
			return formula2 (num, arr [0], arr [1], arr [2], arr [3]);
		}
		return -1;
	}
	
	//公式1
	private static int formula1 (int num, int factor)
	{
		return factor;
	}
	
	//公式2
	private static int formula2 (int num, int factor1, int factor2, int factor3, int factor4)
	{
		return Math.Min ((int)(num / factor2) * factor3 + factor1, factor4);
	} 
	
} 

