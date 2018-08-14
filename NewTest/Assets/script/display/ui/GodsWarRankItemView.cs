using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GodsWarRankItemView : MonoBase
{
	/** 各Label */
	public UILabel[] texts;
	/** 标识自己的背景 */
	public UISprite bg;
	/** 奖杯 */
	public UISprite cup;
	/** 排名 */
	public UILabel rankingNumber;
	[HideInInspector]
	public WindowBase
		fatherWindow;
	[HideInInspector]
	public GodsWarRankUserInfo
		data;
	[HideInInspector]
	public int
		index;

	public void init (GodsWarRankUserInfo data,int index)
	{
		this.data = data;
		this.index = index;

		setText(data,index);
	  
		//给前三名加金杯，银杯，铜杯
		if (cup != null) {
			switch (index) {
			case 0:
				cup.spriteName = "rank_1";
				cup.gameObject.SetActive (true);
				texts[0].transform.localScale = new Vector3 (0.8f, 0.8f, 0.8f);
				texts[0].text = "";
				break;
			case 1:
				cup.spriteName = "rank_2";
				cup.gameObject.SetActive (true);
				texts[0].transform.localScale = new Vector3 (0.8f, 0.8f, 0.8f);
				texts[0].text = "";				
				break;
			case 2:
				cup.spriteName = "rank_3";
				cup.gameObject.SetActive (true);
				texts[0].transform.localScale = new Vector3 (0.8f, 0.8f, 0.8f);
				texts[0].text = "";
				break;
			default:
				cup.spriteName = null;
				cup.gameObject.SetActive (false);
				texts[0].transform.localScale = new Vector3 (1f, 1f, 1f);
				break;
			}	  
		} 
		setSpriteBg();      
	}
	/** 设置背景 */
	private void setSpriteBg ()
	{
		bool isMe = false;
		if(data.uid == UserManager.Instance.self.uid && data.serverName == ServerManagerment.Instance.lastServer.name)
			isMe = true;
		if (isMe) {
			bg.gameObject.SetActive(true);
		} else {
			bg.gameObject.SetActive(false);
		}
	}

	void setText (GodsWarRankUserInfo data,int index)
	{
		int pos=0;
		texts[pos++].text = (index+1).ToString();
		texts[pos++].text = "["+data.serverName+"]"+data.name;
		texts[pos++].text = data.num.ToString();
	}

}
