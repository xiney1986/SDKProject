using System;
using UnityEngine;

public class ByteKit
{
	public static bool readBoolean (byte[] bytes, int pos)
	{
		return bytes [pos] != 0;
	}
	
	public static sbyte readByte (byte[] bytes, int pos)
	{
		return unchecked((sbyte)bytes [pos]);
	}
	
	public static int readUnsignedByte (byte[] bytes, int pos)
	{
		return bytes [pos] & 0xff;
	}
	
	public static char readChar (byte[] bytes, int pos)
	{
		return (char)readUnsignedShort (bytes, pos);
	}
	
	public static short readShort (byte[] bytes, int pos)
	{
		return (short)readUnsignedShort (bytes, pos);
	}
	
	public static int readUnsignedShort (byte[] bytes, int pos)
	{
		return (bytes [pos + 1] & 0xff) + ((bytes [pos] & 0xff) << 8);
	}
	
	public static int readInt (byte[] bytes, int pos)
	{
		return ((bytes [pos + 3] & 0xff)) + ((bytes [pos + 2] & 0xff) << 8)
			+ ((bytes [pos + 1] & 0xff) << 16) + ((bytes [pos] & 0xff) << 24);
	}
	
	public static float readFloat (byte[] bytes, int pos)
	{
		return Convert.ToSingle (readInt (bytes, pos));
	}
	
	public static long readLong (byte[] bytes, int pos)
	{
		return (bytes [pos + 7] & 0xffL) + ((bytes [pos + 6] & 0xffL) << 8)
			+ ((bytes [pos + 5] & 0xffL) << 16) + ((bytes [pos + 4] & 0xffL) << 24)
				+ ((bytes [pos + 3] & 0xffL) << 32) + ((bytes [pos + 2] & 0xffL) << 40)
				+ ((bytes [pos + 1] & 0xffL) << 48) + ((bytes [pos] & 0xffL) << 56);
	}
	
	public static double readDouble (byte[] bytes, int pos)
	{
		return Convert.ToDouble (readLong (bytes, pos));
	}
	
	public static int getReadLength (byte b)
	{
		int n = b & 0xff;
		if (n >= 0x80)
			return 1;
		if (n >= 0x40)
			return 2;
		if (n >= 0x20)
			return 4;
		throw new Exception (typeof(ByteKit).ToString ()
		                     + " getReadLength, invalid number:" + n);
	}
	
	public static int readLength (byte[] data, int pos)
	{
		int n = data [pos] & 0xff;
		if (n >= 0x80)
			return n - 0x80;
		else if (n >= 0x40)
			return (n << 8) + (data [pos + 1] & 0xff) - 0x4000;
		else if (n >= 0x20)
			return (n << 24) + ((data [pos + 1] & 0xff) << 16) + ((data [pos + 2] & 0xff) << 8)
				+ (data [pos + 3] & 0xff) - 0x20000000;
		else
			throw new Exception (typeof(ByteKit).ToString ()
			                     + " readLength, invalid number:" + n);
	}
	
	public static void writeBoolean (bool b, byte[] bytes, int pos)
	{
		bytes [pos] = (byte)(b ? 1 : 0);
	}
	
	public static void writeByte (byte b, byte[] bytes, int pos)
	{
		bytes [pos] = b;
	}
	
	public static void writeChar (char c, byte[] bytes, int pos)
	{
		writeShort ((short)c, bytes, pos);
	}
	
	public static void writeShort (short s, byte[] bytes, int pos)
	{
		byte[] temp = BitConverter.GetBytes (s);
		//	Array.Reverse (temp);
		Array.Copy (temp, 0, bytes, pos, 2);
	}
	
	public static void writeInt (int i, byte[] bytes, int pos)
	{
		byte[] temp = BitConverter.GetBytes (i);
		//	Array.Reverse (temp);
		Array.Copy (temp, 0, bytes, pos, 4);
	}
	
	public static void writeFloat (float f, byte[] bytes, int pos)
	{
		writeInt (Convert.ToInt32 (f), bytes, pos);
	}
	
	public static void writeLong (long l, byte[] bytes, int pos)
	{
		byte[] temp = BitConverter.GetBytes (l);
		//	Array.Reverse (temp);
		Array.Copy (temp, 0, bytes, pos, 8);
	}
	
	public static void writeDouble (double d, byte[] bytes, int pos)
	{
		writeLong (Convert.ToInt32 (d), bytes, pos);
	}
	
	public static int writeLength (int len, byte[] bytes, int pos)
	{
		if (len >= 0x20000000 || len < 0)
			throw new Exception (typeof(ByteKit).ToString ()
			                     + " writeLength, invalid len:" + len);
		if (len >= 0x4000) {
			writeInt (len + 0x20000000, bytes, pos);
			return 4;
		} else if (len >= 0x80) {
			writeShort ((short)(len + 0x4000), bytes, pos);
			return 2;
		} else {
			writeByte ((byte)(len + 0x80), bytes, pos);
			return 1;
		}
	}
	
	/** 写入动态长度 */
	public static int writeLength (ByteBuffer data, int len)
	{
		if (len >= 0x20000000 || len < 0)
			throw new Exception (typeof(ByteKit).ToString ()
			                     + " writeLength, invalid len:" + len);
		if (len >= 0x4000) {
			data.writeInt (len + 0x20000000);
			return 4;
		} else if (len >= 0x80) {
			data.writeShort ((len + 0x4000));
			return 2;
		} else {
			data.writeByte ((len + 0x80));
			return 1;
		}
	}
	
	public static int writeLength (ByteBuffer data, int len, int pos)
	{
		if (len >= 0x20000000 || len < 0)
			throw new Exception (typeof(ByteKit).ToString ()
			                     + " writeLength, invalid len:" + len);
		if (len >= 0x4000) {
			data.writeInt (len + 0x20000000);
			return 4;
		} else if (len >= 0x80) {
			data.writeShort ((short)(len + 0x4000));
			return 2;
		} else {
			data.writeByte ((byte)(len + 0x80));
			return 1;
		}
	}
	
	public static int getWriteLength (int len)
	{
		if (len >= 0x20000000 || len < 0)
			throw new Exception (typeof(ByteKit).ToString ()
			                     + " getWriteLength, invalid len:" + len);
		if (len >= 0x4000) {
			return 4;
		} else if (len >= 0x80) {
			return 2;
		} else {
			return 1;
		}
	}
	
	public static string readISO8859_1 (byte[] data)
	{
		return readISO8859_1 (data, 0, data.Length);
	}
	
	public static string readISO8859_1 (byte[] data, int pos, int len)
	{
		char[] array = new char[len];
		for (int i=pos+len-1,j=array.Length-1; i>=pos; i--,j--)
			array [j] = (char)data [i];
		return new String (array);
	}
	
	public static string readUTF (byte[] data)
	{
		char[] array = new char[data.Length];
		int n = readUTF (data, 0, data.Length, array);
		return (n >= 0) ? new String (array, 0, n) : null;
	}
	
	public static string readUTF (byte[] data, int pos, int length)
	{
		char[] array = new char[length];
		int n = readUTF (data, pos, length, array);
		return (n >= 0) ? new String (array, 0, n) : null;
	}
	
	public static int readUTF (byte[] data, int pos, int length, char[] array)
	{
		int i, c, cc, ccc;
		int n = 0, end = pos + length;
		while (pos<end) {
			c = data [pos] & 0xff;
			i = c >> 4;
			if (i < 8) {
				// 0xxx xxxx
				pos++;
				array [n++] = (char)c;
			} else if (i == 12 || i == 13) {
				// 110x xxxx 10xx xxxx
				pos += 2;
				if (pos > end)
					return -1;
				cc = data [pos - 1];
				if ((cc & 0xC0) != 0x80)
					return -1;
				array [n++] = (char)(((c & 0x1f) << 6) | (cc & 0x3f));
			} else if (i == 14) {
				// 1110 xxxx 10xx xxxx 10xx
				// xxxx
				pos += 3;
				if (pos > end)
					return -1;
				cc = data [pos - 2];
				ccc = data [pos - 1];
				if (((cc & 0xC0) != 0x80) || ((ccc & 0xC0) != 0x80))
					return -1;
				array [n++] = (char)(((c & 0x0f) << 12) | ((cc & 0x3f) << 6) | (ccc & 0x3f));
			} else
				// 10xx xxxx 1111 xxxx
				return -1;
		}
		return n;
	}
	
	public static int getUTFLength (string str, int index, int len)
	{
		int utfLen = 0;
		int c;
		char[] chars = str.ToCharArray ();
		for (int i=index; i<len; i++) {
			c = chars [i];
			if ((c >= 0x0001) && (c <= 0x007f))
				utfLen++;
			else if (c > 0x07ff)
				utfLen += 3;
			else
				utfLen += 2;
		}
		return utfLen;
	}
	
	public static int getUTFLength (char[] chars, int index, int len)
	{
		int utfLen = 0;
		int c;
		for (int i=index; i<len; i++) {
			c = chars [i];
			if ((c >= 0x0001) && (c <= 0x007f))
				utfLen++;
			else if (c > 0x07ff)
				utfLen += 3;
			else
				utfLen += 2;
		}
		return utfLen;
	}
	
	public static byte[] writeUTF (string str)
	{
		return writeUTF (str, 0, str.Length);
	}
	
	public static byte[] writeUTF (string str, int index, int len)
	{
		byte[] data = new byte[getUTFLength (str, index, len)];
		writeUTF (str, index, len, data, 0);
		return data;
	}
	
	public static void writeUTF (string str, int index, int len, byte[] data,
	                             int pos)
	{
		int c;
		char[] chars = str.ToCharArray ();
		for (int i=index; i<len; i++) {
			c = chars [i];
			if ((c >= 0x0001) && (c <= 0x007f)) {
				data [pos++] = (byte)c;
			} else if (c > 0x07ff) {
				data [pos++] = (byte)(0xe0 | ((c >> 12) & 0x0f));
				data [pos++] = (byte)(0x80 | ((c >> 6) & 0x3f));
				data [pos++] = (byte)(0x80 | (c & 0x3f));
			} else {
				data [pos++] = (byte)(0xc0 | ((c >> 6) & 0x1f));
				data [pos++] = (byte)(0x80 | (c & 0x3f));
			}
		}
	}
	
	public static void writeUTF (char[] chars, int index, int len, byte[] data,
	                             int pos)
	{
		int c;
		for (int i=index; i<len; i++) {
			c = chars [i];
			if ((c >= 0x0001) && (c <= 0x007f)) {
				data [pos++] = (byte)c;
			} else if (c > 0x07ff) {
				data [pos++] = (byte)(0xe0 | ((c >> 12) & 0x0f));
				data [pos++] = (byte)(0x80 | ((c >> 6) & 0x3f));
				data [pos++] = (byte)(0x80 | (c & 0x3f));
			} else {
				data [pos++] = (byte)(0xc0 | ((c >> 6) & 0x1f));
				data [pos++] = (byte)(0x80 | (c & 0x3f));
			}
		}
	}
	
	private ByteKit ()
	{
	}
	
	/// <summary>
	/// 简单数据的解析 byte=97 int=98 binary=109(暂时用不到) string=107(非二进制数据内的字符串)
	/// </summary>
	/// <returns>The analyse.</returns>
	/// <param name="data">Data.</param>
	public static ErlType simpleAnalyse (ByteBuffer data)
	{
		int position = data.position;
		int tag = data.readByte ();
		data.position = position;
		
		if (tag == ErlByte.TAG) {
			ErlByte erlByte = new ErlByte (0);
			erlByte.bytesRead (data);
			return erlByte;
		} 
		//		else if (tag == ErlByteArray.TAG) {
		//			ErlByteArray erlByteArray = new ErlByteArray ();
		//			erlByteArray.bytesRead (data);
		//			return erlByteArray;
		//		} 
		else if (tag == ErlInt.TAG) {
			ErlInt erlInt = new ErlInt (0);
			erlInt.bytesRead (data);
			return erlInt;
		} else if (tag == ErlString.TAG) { 
			ErlString erlString = new ErlString ("");
			erlString.bytesRead (data); 
			return erlString;
		} else {
			return null;
		}
	}
	
	/// <summary>
	/// 二进制数据解析 走到这里，肯定是erlang标准二进制数据
	/// </summary>
	/// <returns>The analyse.</returns>
	/// <param name="data">Data.</param>
	public static ErlType  complexAnalyse (ByteBuffer data)
	{
		int position = data.position;
		int tag = data.readByte ();
		data.position = position;  
		//			 MonoBehaviour.print("----------complexAnalyse--------------"+tag);
		if (tag == ErlArray.TAG [0] || tag == ErlArray.TAG [1]) {
			ErlArray erlArray = new ErlArray (null);
			erlArray.bytesRead (data);
			return erlArray;
		} else if (tag == ErlNullList.TAG) {
			ErlNullList erlNullList = new ErlNullList ();
			erlNullList.bytesRead (data);
			return erlNullList;
		} else if (tag == ErlList.TAG) {
			ErlList erlList = new ErlList (null);
			erlList.bytesRead (data);
			return erlList;
		} else if (tag == ErlAtom.TAG) {
			ErlAtom erlAtom = new ErlAtom (null);
			erlAtom.bytesRead (data);
			return erlAtom;
		} else if (tag == ErlString.TAG) {
			ErlString erlString = new ErlString (null);
			erlString.sampleBytesRead (data);
			return erlString;
		} 
		else {
			return null;
		}
	}
	
	/// <summary>
	/// 二进制数据解析成具体数据
	/// </summary>
	/// <returns>The analyse.</returns>
	/// <param name="data">Data.</param>
	public static ErlType natureAnalyse (ByteBuffer data)
	{
		uint p = (uint)data.position;
		uint tag = (uint)data.readUnsignedByte ();
		//			MonoBehaviour.print("=======natureAnalyse======= tag="+tag);
		if (tag == ErlKVMessage.VER) {
			// 如果走到这里 表示二进制里嵌套二进制
			return complexAnalyse (data);
		} else {
			data.position = (int)p; 
			if (tag == ErlArray.TAG [0] || tag == ErlArray.TAG [1] || tag == ErlNullList.TAG || tag == ErlList.TAG || tag == ErlAtom.TAG) {
				return complexAnalyse (data);
			} else {
				return natureSampleAnalyse (data);
			} 
		}
	}
	
	public static ErlType natureSampleAnalyse (ByteBuffer data)
	{
		int position = data.position;
		int tag = data.readByte ();
		data.position = position;
		
		if (tag == ErlByte.TAG) {
			ErlByte erlByte = new ErlByte (0);
			erlByte.bytesRead (data);
			return erlByte;
		} 
		//		else if (tag == ErlByteArray.TAG) {
		//			ErlByteArray erlByteArray = new ErlByteArray (null);
		//			erlByteArray.bytesRead(data);
		//			return erlByteArray;
		//		}
		else if (tag == ErlInt.TAG) {
			ErlInt erlInt = new ErlInt (0);
			erlInt.bytesRead (data);
			return erlInt;
		} else if (tag == ErlString.TAG) {
			ErlString erlString = new ErlString ("");
			erlString.sampleBytesRead (data);
			return erlString;
		} else
			return null;
	}
	
	
}
