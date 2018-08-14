using System;
/// <summary>
/// 竞技场提示服务
/// </summary>
public class ArenaTipServerFPort : BaseFPort
{
	public override void read (ErlKVMessage message)
	{ 
		ErlType type = message.getValue ("hint") as ErlType;
		if(type==null)
		{
			return;
		}
		string content = type.getValueString ();//内容
		TextTipWindow.Show(content,2f);
	}
}
