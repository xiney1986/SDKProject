using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using UnityEngine;

// 连接建立并成功与后台握手后的回调函数
public delegate void CallBackHandle (); 
 
/**
 * 通讯连接类
 * 
 * @author longlingquan
 * */
public class Connect:IDisposable
{
	
	/** 连接的默认超时时间3分钟 */
	public  const int TIMEOUT = 180000;
	/** 默认的消息最大长度，400k */
	public const int MAX_DATA_LENGTH = 126 * 1024;
	 
	/** 连接的本地地址 */
	private string _localAddress;
	/** 连接的本地端口 */
	private int _localPort;
	/** 连接活动的标志 */
	volatile bool _active;
	/** 连接的开始时间 */
	private long _startTime;
	/** 连接的最近活动时间 */
	private long _activeTime;
	/** 连接的ping值 */
	private long _ping = -1;
	/** 连接的超时时间 */
	private int _timeout = TIMEOUT;
	/** 连接发出ping的时间 */
	private long _pingTime;
	/** socket */
	private Socket  _socket;
	/** 转发通讯类 */
	protected PortHandler _portHandler;
	private CallBackHandle _callback;
	public Socket socket;
	private int len = 0;
	Timer timer;//连接超时的监控
	//--------------------------------------------get set start--------------------------------------------------------------------------------
	public string LocalAddress {
		get {
			return _localAddress;
		} 
	}
	
	public int LocalPort {
		get {
			return _localPort;
		}
	}
	
	public bool Active {
		get {
			return _active && socket != null && socket.Connected;
		}
	}
	
	public long StartTime {
		get {
			return _startTime;
		}
	}
	
	public long ActiveTime {
		get {
			return _activeTime;
		}
		set {
			this._activeTime = value;
		}
	}
	
	public long ping {
		get {
			return _ping;	
		}
		set {
			this._ping = value;
		}
	}
	
	public int TimeOut {
		get {
			return _timeout;
		}
		set {
			this._timeout = value;
		}
	}
	
	public long PingTime {
		get {
			return _pingTime;
		}
		set {
			this._pingTime = value;
		}
	}
	
	public PortHandler portHandler {
		get {
			return _portHandler;
		}
		set {
			this._portHandler = value;
		}
	}
	
	public CallBackHandle CallBack {
		get {
			return _callback;
		}
		set {
			this._callback = value;
		}
	} 
	//--------------------------------------------get set over-------------------------------------------------------------------------------- 
	
	public Connect ()
	{
	}
	
	// 通过地址判断是否是相同的连接
	public bool isSameConnect (string localAddress, int localPort)
	{
		return _localAddress == localAddress && _localPort == localPort;
	}
	
	protected void init (string address, int port)
	{
		this._localAddress = address;
		this._localPort = port;
	}
	
	public void open (string address, int port)
	{   
		init (address, port);
		if (Active)
			throw new Exception (this.GetType () + ", open, connect is active"); 

		socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

		if (ServerManagerment.Instance.lastServer != null) {
			if (ServerManagerment.Instance.lastServer.ipEndPoint == null) {
				IPAddress[] ips = Dns.GetHostAddresses (address);
				IPEndPoint ipe = new IPEndPoint (ips [0], port); 
				ServerManagerment.Instance.lastServer.ipEndPoint = ipe;
			} 

				socket.BeginConnect (ServerManagerment.Instance.lastServer.ipEndPoint, suceess, socket);
				timer = TimerManager.Instance.getTimer (10000, 1);
				timer.addOnTimer (connectFail);
				timer.start ();
		
		}else{

			Debug.LogWarning("no server ~");
			
		}

	}
	void connectFail( )
	{
		socket.Close ();
		timer.stop ();
//		if (GameManager.Instance != null) {
//			GameManager.Instance.OnLostConnect (true);
//		}

	}
	void suceess(IAsyncResult asyncresult)
	{
		if (socket.Connected) { 
			_active = true;
			socket.ReceiveBufferSize=20480;
			_activeTime = _startTime = DateTime.Now.ToFileTime ();  
			timer.stop();
		} 
	}
	/** 根据头信息创建字节缓存对象 */
	protected virtual ByteBuffer createDataByHead (ByteBuffer body)
	{
		int len = body.length (); 
		ByteBuffer data = new ByteBuffer ();
		ByteKit.writeLength (data, len); 
		data.writeBytes (body.toArray ());
		return data;
	}
	
	/** 发送字节数组 */
	public void send (ByteBuffer data)
	{ 
		if (!_active)
			return; 
		data = createDataByHead (data);  
		send (data.toArray (), 0, data.length ()); 
	}
	
	public  void send (byte[] data, int offset, int len)
	{
		if (!_active || socket == null || !socket.Connected || data == null || data.Length == 0) 
			return;

		if (len > MAX_DATA_LENGTH)
			Debug.LogWarning (this.GetType () + ", send, data overflow:" + len + ", " + this);

		try {  		
			socket.Send (data, 0, data.Length, SocketFlags.None);  
		} catch {
            if(GameManager.Instance != null)
            {
                GameManager.Instance.OnLostConnect(true);
            }
		} 
	}
	
	/** 连接的消息接收方法 */
	public virtual void receive ()
	{ 
		if (Active && socket.Connected) { 
			if (socket.Available > 0) { 
				if (len <= 0) {
					len = readLength (); 
				}
				if (len > socket.Available)
					return;
				ByteBuffer data = new ByteBuffer (len);
				data.setTop (len);
				socket.Receive (data.getArray (), SocketFlags.None);
				len = 0;
				receive (data);
			}
		 
		}
	}
	
	public virtual void receive (ByteBuffer data)
	{
		
		if (_portHandler == null)
			return;
		_activeTime = TimeKit.getMillisTime ();
		try {
			_portHandler.receive (this, data);
		} catch (Exception e) {
			Log.error (this.GetType () + ", receive error, " + this, e);
		}
	}
	
	public int readLength ()
	{
		byte[] b1 = new byte[1];
		socket.Receive (b1, SocketFlags.None);
		int n = b1 [0];  
		if (n >= 0x80) {
			return n - 0x80;
		} else if (n >= 0x40) {
			b1 = new byte[1];
			socket.Receive (b1, SocketFlags.None);
			return (n << 8) + ByteKit.readUnsignedByte (b1, 0) - 0x4000;
		} else if (n >= 0x20) {
			b1 = new byte[3];
			return (n << 24) + (ByteKit.readUnsignedByte (b1, 0) << 16)
					+ ByteKit.readUnsignedByte (b1, 1) - 0x20000000;
		} else {
			throw new Exception (this.GetType () + ", readLength, invalid number:" + n + ", " + this);
		}
	}

	
	public virtual void Dispose ()
	{
		lock (this) {
			if (!Active)
				return;
			_active = false;

			if (socket != null && socket.Connected) {
				socket.Disconnect (true);
			}
		} 
	} 
} 

