using UnityEngine;
using System.Collections;

public class CodecKit  {
 
	/**
	 * 字节数组加密
	 * 
	 * @param bytes 源数据
	 * @param keys 密钥
	 * @return byte[] 加密后的数据
	 */
	public static byte[] encodeXor(byte[] bytes,byte[] keys)
	{
		if(bytes==null||bytes.Length<1||keys==null
			||keys.Length<1)
			return null;
		
		int blength=bytes.Length;
		int klength=keys.Length;
		byte[] result=new byte[blength];
		int j=0;
		for(int i=0;i<blength;i++)
		{
			if(j==klength) j=0;
			int k=(bytes[i]^keys[j]);
			k<<=24;
			k>>=24;
			result[i]=(byte)k;
			j++;
		}
		return result;
	}
}
