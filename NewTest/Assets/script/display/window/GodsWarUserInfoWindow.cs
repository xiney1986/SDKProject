using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GodsWarUserInfoWindow : WindowBase 
{
	/// <summary>
	/// 条目点位
	/// </summary>
	public GameObject[] cards;
	/// <summary>
	/// 支持人数
	/// </summary>
	public UILabel lblSuportNum;
	/// <summary>
	/// 战斗力
	/// </summary>
	public UILabel lblPower;
	/// <summary>
	/// 服务器名字
	/// </summary>
	public UILabel lblSeverName;
	/// <summary>
	/// 标题
	/// </summary>
	public UILabel lbltitle;
	/// <summary>
	/// 玩家姓名
	/// </summary>
	public UILabel lblName;
	public GameObject itemPrefab;
	public GameObject mask;

	private string serverName;
	private string name;
	private string role_uid;
	private int big_uid;
	private int yu_ming;

	GodsWarFinalUserInfo info;


	CallBack callback;

	protected override void begin ()
	{
		base.begin ();
	    UiManager.Instance.godsWarUserInfoWindow = this;

	}
    void OnDestroy() {
        UiManager.Instance.godsWarUserInfoWindow = null;

    }
	public void initWindow(string serverName,string name,string role_uid,int big_id,int yu_ming, CallBack callback)
	{
		this.serverName =  serverName;
		this.role_uid = role_uid;
		this.big_uid = big_id;
		this.yu_ming = yu_ming;
		this.name = name;
		this.callback = callback;
		initData();
	}
	/// <summary>
	/// 初始化数据
	/// </summary>
	public void initData()
	{
		FPortManager.Instance.getFPort<GodsWarGetPlayerInfoFPort>().access(serverName,role_uid,big_uid,yu_ming,initUI);
	}

	/// <summary>
	/// 初始化UI
	/// </summary>
	public void initUI()
	{
		info = GodsWarManagerment.Instance.singlePlayer;
		if(info==null)return;
		lblName.text = name;
		lblSuportNum.text = LanguageConfigManager.Instance.getLanguage("godsWar_67",info.suportPlayerNum.ToString());
		lblPower.text = LanguageConfigManager.Instance.getLanguage("godsWar_68",info.power.ToString());
		lblSeverName.text = LanguageConfigManager.Instance.getLanguage("godsWar_69",info.serverName);
		loadCards();
	}

	/// <summary>
	/// 初始化card
	/// </summary>
	public void loadCards()
	{
		if(info == null)return;
		for (int i = 0; i < info.cardSids.Count; i++) {
			GameObject go = NGUITools.AddChild(cards[i],itemPrefab);
			if(info.cardSids[i].StartsWith("0"))continue;
		    string[] sidorlv = info.cardSids[i].Split(':');
            Card card = CardManagerment.Instance.createCard(StringKit.toInt(sidorlv[0]), StringKit.toInt(sidorlv[1]), StringKit.toInt(sidorlv[2]));
			GoodsView item = go.GetComponent<GoodsView>();
			item.init(card);
			item.rightBottomText.gameObject.SetActive(false);
		}
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if(gameObj.name=="button_close")
		{
			mask.gameObject.SetActive(false);
			finishWindow();
		}
	}

	public void updateUI()
	{

	}
}
