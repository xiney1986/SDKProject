using System;  
using System.IO;  
using System.Collections.Generic;
using UnityEngine;
using zlib;

public class ZIPUtil
{
	public ZIPUtil ()
	{
	} 
	 
	//级别暂时�?
	public static byte[] Compress (byte[] inputBytes)
	{  
		using (MemoryStream ms = new MemoryStream()) {
			using (ZOutputStream zOut = new ZOutputStream( ms,0 )) {
				zOut.Write (inputBytes, 0, inputBytes.Length);
				zOut.finish ();
				return ms.ToArray ();
			}
		} 
	}

	public static byte[] Decompress (Byte[] inputBytes)
	{
		using (MemoryStream ms = new MemoryStream()) {
			using (ZOutputStream zOut = new ZOutputStream( ms )) {
				zOut.Write (inputBytes, 0, inputBytes.Length);
				zOut.finish ();
				return ms.ToArray ();
			}
		}
            
	}  
}
 

