using System;

/**
 * @author huangzhenghan
 */
public class ErlKit
{

	/* static fields */
	public static string[] ErlArray2String(ErlArray array){
		string[] arr = new string[array.Value.Length];
		for (int i = 0; i < arr.Length; i++)
			arr [i] = (array.Value [i] as ErlType).getValueString ();
		return arr;
	}
	public static int[] ErlArray2Int(ErlArray array){
		int[] arr = new int[array.Value.Length];
		for (int i = 0; i < arr.Length; i++)
			arr [i] = StringKit.toInt((array.Value [i] as ErlType).getValueString ());
		return arr;
	}

	/* constructors */
	private ErlKit ()
	{
	}

}
