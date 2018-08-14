using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

/**
 * ErlConnect
 * 
 * @author longlingquan
 * */
public class ErlConnect:Connect
{
	
	/** 版本号 */
	public const int VERSION = 0;
	/** 默认加密效验 */
	public const int ENCRYPTION = 1;
	/** 默认CRC效验 */
	public const int CRC = 1;
	/** 默认压缩 */
	public const int COMPRESS = 1;
	/** 默认kv类型 为0表示消息为二进制数据，为1表示消息为KeyValue类型，key为字符串，Value为标准格式的数据 */
	public const int KV = 1;
	/***/
	private const int RAND_MASK = 123459876;
	/***/
	private const int RAND_A = 16807;
	private const int RAND_Q = 127773;
	
	/** 发送挑战码 */
	private int[] _sendChallengeCode;
	/** 接受挑战码 */
	private int[]  _receiveChallengeCode;
	/** 是否握手成功 */
	private bool _isConnectReady = false;
	/** 是否 加密效验*/
	private int _encryption = ENCRYPTION;
	/** 是否 CRC效验*/
	private int _crc = CRC;
	/** 是否压缩 */
	private int _compress = COMPRESS;
	/** kv类型 */
	private int _kv = KV;
	/** 消息接收到的长度 */
	private int length = 0;
	
	public ErlConnect ()
	{
	}
	
	public bool isActive {
		get { 
			return Active && _isConnectReady;
		}
	}
	
	/** 根据头信息创建字节缓存对象 */
	override protected ByteBuffer createDataByHead (ByteBuffer head)
	{
		ByteBuffer data = new ByteBuffer ();
		int versionInfo = (VERSION << 4) | (_encryption << 3) | (_crc << 2) | (_compress << 1) | _kv;
		data.writeShort (head.length () + 1);
		data.writeByte (versionInfo);
		data.writeBytes (head.toArray ()); 
		return data;
	}
	
/** 发送方法 
	 * @param data 数据
	 * @param isEncryption 是否加密
	 * @param isCrc 是否crc
	 * @param isCompress 是否压缩
	 * @param kv kv类型 为0表示消息为二进制数据，为1表示消息为KeyValue类型，key为字符串，Value为标准格式的数据
	 * */
	public void sendErl (ByteBuffer data, int encryption, int crc, int compress, int kv)
	{   
		//没有得到pk码,一般出现在连接有,但是接不到后台消息
		if(_sendChallengeCode==null || _sendChallengeCode.Length<0)
			return;

		_encryption = encryption;
		_crc = crc;
		_compress = compress;
		_kv = kv;
		int crcValue = 0;
		ByteBuffer data1 = new ByteBuffer ();
		 
		if (_compress == COMPRESS && data.length () >= 64) {// 根据参数和数据长度判断是否执行压缩  
			byte[] bb = ZIPUtil.Compress (data.toArray ());
			data = new ByteBuffer (bb); 
		} else {
			_compress = 0;
		}  
		
		if (_crc == 1 && _compress == 0) {
			crcValue = (int)ChecksumUtil.Adler32 (data);
			data1.writeInt (crcValue);
		} else {
			_crc = 0;
		}  
		data1.writeBytes (data.toArray ());  
		
		if (_encryption == 1) {
			data1 = encryptionCode (data1, _sendChallengeCode);// 执行加密
		}  
		 
		send (data1);
		_encryption = ENCRYPTION;
		_crc = CRC;
		_compress = COMPRESS;
        _kv = KV;
	}
	
	/** 连接的消息接收方法 */
	public override void receive ()
	{ 
		if(GameManager.Instance.disconnetTest)
			return;

		if(!socket.Connected)
			return;

		ActiveTime = TimeKit.getMillisTime ();   
		if (socket.Available > 0) {
			if (!_isConnectReady) { 
				//设置 _isConnectReady=true  connect pk receive 
				//抛掉前两位
				byte[] b1 = new byte[1];
				socket.Receive (b1, SocketFlags.None);
				byte[] b2 = new byte[1];
				socket.Receive (b2, SocketFlags.None); 
					
				byte[] b3 = new byte[4];
				socket.Receive (b3, SocketFlags.None);   
				Array.Reverse (b3);
				int i = BitConverter.ToInt32 (b3, 0);
					
				byte[] b4 = new byte[4];
				socket.Receive (b4, SocketFlags.None);  
				Array.Reverse (b4);
				int ii = BitConverter.ToInt32 (b4, 0);  
					
				_sendChallengeCode = getPK (i);
				_receiveChallengeCode = getPK (ii);
				_isConnectReady = true; 
				if (this.CallBack != null)
					this.CallBack ();
			} else { 

				if (length <= 0) {  
					if (socket.Available < 2)
						return; 
					//	length = data.readUnsignedShort (); 
					byte[] b = new byte[2];
					socket.Receive (b, SocketFlags.None);   
					length = ByteKit.readUnsignedShort (b, 0);    
					//length = readLength ();
				}
				if (length > 0 && socket.Available >= length) {

					ByteBuffer data = new ByteBuffer (length);
					data.setTop (length);
					socket.Receive (data.getArray (), SocketFlags.None);   
						
                    parseMessage (data); 

				}
			}
		}
	 
	}
	
	/** 解析单次消息内容 */
	public void parseMessage (ByteBuffer socketbuffer)
    {  	   
		int versionInfo = socketbuffer.readByte ();
		bool encryption = ((versionInfo & 8) != 0);
		bool crc = ((versionInfo & 4) != 0);
		bool compress = ((versionInfo & 2) != 0); 
        //if(!MiniConnectManager.IsRobot)
		//MonoBehaviour.print ("length=>" + length + "  versionInfo=>" + versionInfo + "  encryption=>" + encryption + "  crc=>" + crc + "  compress=>" + compress);
		ByteBuffer data = new ByteBuffer (length - 1); 
		data.write (socketbuffer.toArray (), 0, length - 1);  
		   
		//为下次数据处理做判断
		if (socket.Available >= 2) {
			byte[] b = new byte[2];
			socket.Receive (b, SocketFlags.None);   
			length = ByteKit.readUnsignedShort (b, 0);    
		} else 
			length = 0; 
		 
		if (encryption) {
			data = encryptionCode (data, _receiveChallengeCode);
		}   
		if (compress) {  
			byte[] bb = ZIPUtil.Decompress (data.toArray ()); 
            data = new ByteBuffer (bb); 
        } 
		  
		if (crc) {
			int crcValue = data.readInt (); 
			ByteBuffer data1 = new ByteBuffer ();  
			data1.writeBytes (data.toArray (), 0, (data.top - data.position));  
			int nowCrc = (int)ChecksumUtil.Adler32 (data1);
			if (crcValue != nowCrc) {
				MonoBehaviour.print ("crc is err,crcValue" + crcValue + ",nowCrc=" + nowCrc); 
				return;
			}  
        }   
		ErlKVMessage message = new ErlKVMessage (null);
		message.bytesRead (data);  
		if (_portHandler != null) {// _portHandler可以是DataAccess或者ErlTransmitPort，如果要保存funcUid就要设置为DataAccess
			_portHandler.erlReceive (this, message);
        }
	}
	
	/** 连接关闭方法 */
	public void close ()
	{ 
		Dispose ();
		//ErlConnectFactory.getFactory ().removeConnect (this);
	}
	
	/** 发送消息时用作加密，接收消息时用作解密 */
	private ByteBuffer encryptionCode (ByteBuffer data, int[] code)
	{
		byte[] bytes = data.toArray (); 
		bytes = CodecKit.encodeXor (bytes, nextPK (code));
		data = new ByteBuffer ();
		for (int i=0; i<bytes.Length; i++) {
			data.writeByte (bytes [i]);
		}
		data.position = 0;
		return data;
	}
	
	/** 获取指定种子的密码 */
	protected int[] getPK (int seed)
	{ 
		//ByteBuffer data = new ByteBuffer ();
		int seed1 = getRandome (seed + 11);
		int seed2 = getRandome (seed1 + 13);
		int seed3 = getRandome (seed2 + 17);
		int seed4 = getRandome (seed3 + 19);
		int seed5 = getRandome (seed4 + 23);
		int seed6 = getRandome (seed5 + 29);
		int seed7 = getRandome (seed6 + 31);
		int seed8 = getRandome (seed7 + 37);
		return new int[]{seed1,seed2,seed3,seed4,seed5,seed6,seed7,seed8};
	}
	
	/** 获取指定种子的随机数 */
	private int getRandome (int seed)
	{
		int r = seed ^ RAND_MASK; 
		int s = RAND_A * r - (int)Mathf.Round (r / RAND_Q) * int.MaxValue;
		if (s < 0)
			return s + int.MaxValue;
		else
			return s;
	}
	
	/** 获取下一个密码 */
	protected byte[] nextPK (int[] pk)
	{
		if (pk == null)
			return null;
		for (int i =0,length = pk.Length; i<length; i++) {
			pk [i] = getRandome (pk [i]);
		} 
		return toPK (pk);
	}
	
	/** 获取指定密码的字节数组 */
	protected byte[] toPK (int[] pks)
	{
		ByteBuffer data = new ByteBuffer ();
		data.writeInt (pks [0]);
		data.writeInt (pks [1]);
		data.writeInt (pks [2]);
		data.writeInt (pks [3]);
		data.writeInt (pks [4]);
		data.writeInt (pks [5]);
		data.writeInt (pks [6]);
		data.writeInt (pks [7]);
		return data.getArray ();
	}
	
	public string toString ()
	{
		return "[_sendChallengeCode=" + _sendChallengeCode + ",_receiveChallengeCode=" + _receiveChallengeCode + "]";
	} 
} 

