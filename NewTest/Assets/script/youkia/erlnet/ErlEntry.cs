using System;
using System.Collections;
using System.Collections.Generic;
 
public class ErlEntry
{
	public Connect connect;
		
	public int  number;
	
	public ReceiveFun receiveFun;
	
	public List<Object> argus;
		
	public long timeOut;
	
	public  ErlEntry(Connect connect,int number,ReceiveFun receiveFun,List<Object> argus,long timeOut)
	{
		this.connect=connect;
		this.number=number;
		this.receiveFun=receiveFun;
		this.argus=argus;
		this.timeOut=timeOut;
	}
}

