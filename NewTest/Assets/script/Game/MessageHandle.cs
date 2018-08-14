using UnityEngine;
using System.Collections;

public enum msg_event
{
	dialogOK,
	dialogCancel,
}

public class MessageHandle
{

	public msg_event msgEvent;
	public 	object msgInfo;
	/** 外部参数用 */
	public string msgStr;
	public int  msgNum;
    public int costNum;
	public int buttonID;//对话框按钮id,目前只有0左边，1右边，2中
	public const int BUTTON_LEFT = 0;
	public const int BUTTON_RIGHT = 1;
	public const int BUTTON_MIDDLE = 2;
}
