using UnityEngine;
using System.Collections;

public class GuildNoAndDeContent : MonoBase
{
	public UIInput input;//输入内容
	public UILabel count;//输入文字数
	public const string NOTICE = "notice";//公告内容
	public const string DECLARATION = "declaration";//宣言内容
	private const int COUNTSUM = 100;//最多文字数
	public UISprite buttonNotice;
	public UISprite buttonDec;

	public void updateInput(string contentType)
	{
		if(contentType == NOTICE)
		{
			buttonNotice.color = Color.white;
			buttonDec.color = Color.gray;
			input.value = GuildManagerment.Instance.getGuild().notice;
		}
		else
		{
			buttonNotice.color = Color.gray;
			buttonDec.color = Color.white;
			input.value = GuildManagerment.Instance.getGuild().declaration;
		}
		MaskWindow.UnlockUI();
	}

	void Update()
	{
		count.text = input.value.Length + "/" + COUNTSUM;
	}

}
