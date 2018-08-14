using UnityEngine;
using System.Collections;
using System;

public class GuildMsgItem : MonoBehaviour 
{
	public GuildMsg msg;
	public UILabel showLabel;
	private string date;
	private	string time;
	private	string content;
	
	public void initUI(GuildMsg msg)
	{
		this.msg = msg;
		content = msg.content;
		string[] strs = content.Split(' ');
		showLabel.text = "[FFB84D]" + strs[0] + " [FFB84D]" + strs[1] + " [FFFFFF]" + strs[2];
	}
}
