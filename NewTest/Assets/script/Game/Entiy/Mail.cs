using UnityEngine;
using System.Collections;

public class Mail 
{
	public Mail (string uid,int src,int type,int stime,int time,string theme,string content,Annex[] annex,int status)
	{
		this.uid = uid;
		this.src = src;
		this.type = type;
		this.stime = stime;
		//time时长
		this.etime = ServerTimeKit.getSecondTime()+time;
		this.theme = theme;
		this.content = content;
		this.annex = annex;
		this.status = status;
	} 
	public string uid = "0";//唯一索引
	public int src ;//发件人
	public int type;//类型
	public int stime;//收到时间
	public int etime;//到期时间
	public string theme;//主题
	public string content;//内容
	public Annex[] annex;//附件
	public int status;//状态 1未操作过 2提取过
	//只用status 来判断是否阅读过，还不够，加一个变量和配合使用
	public bool hasRead=false;
}
