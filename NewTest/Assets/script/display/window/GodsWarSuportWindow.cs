using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GodsWarSuportWindow : WindowBase 
{
	/// <summary>
	/// 条目点位
	/// </summary>
	public GameObject[] points;
	/// <summary>
	/// 标题
	/// </summary>
	public UILabel lbltitle;
	public GameObject itemPrefab;
	public GameObject mask;
	/// <summary>
	/// 奖励提示
	/// </summary>
	public UILabel lblAwardTips;

	private int big_id;
	private int yu_ming;
	private int localId;
	List<GodsWarFinalUserInfo> players;

	CallBack callback;

	protected override void begin ()
	{
		base.begin ();
	    UiManager.Instance.godsWarSuportWindow = this;
		initData();
	}

	public void initWindow(int big_id,int yu_ming,int localId,CallBack callback)
	{
		this.big_id = big_id;
		this.yu_ming = yu_ming;
		this.localId = localId;
		this.callback = callback;
	}
	/// <summary>
	/// 初始化数据
	/// </summary>
	public void initData()
	{
		FPortManager.Instance.getFPort<GodsWarGetPlayerInfoFPort>().access(big_id,yu_ming,localId,initUI);
	}

	/// <summary>
	/// 初始化UI
	/// </summary>
	public void initUI()
	{
		MaskWindow.UnlockUI();
		lbltitle.text = getTypeByLocalId(localId);
		players = GodsWarManagerment.Instance.pvpGodsWarFinalInfo;
		for (int i = 0; i < points.Length; i++) {
			GameObject go = NGUITools.AddChild(points[i],itemPrefab);
			GodsWarSuportItem item = go.GetComponent<GodsWarSuportItem>();
			item.initItem(players[i],big_id,yu_ming,localId,this,()=>{
				if(callback!=null)
					callback();
				finishWindow();
			});
		}
	}

	public string getTypeByLocalId(int localid)
	{
		int a = GodsWarManagerment.Instance.getTypeByLocalId(localid);
		List<GodsWarPrizeSample> prize = GodsWarPrizeSampleManager.Instance.getSuportPrize();
		if(prize.Count >= a)
			lblAwardTips.text = LanguageConfigManager.Instance.getLanguage("godsWar_93",prize[a-1].item[0].getPrizeNumByInt()+prize[a-1].item[0].getPrizeName());
		if(a==1)
		{
			return LanguageConfigManager.Instance.getLanguage("godsWar_55","8");
		}
		if (a==2) 
		{
			return LanguageConfigManager.Instance.getLanguage("godsWar_55","4");
			
		}
		if (a==3) 
		{
			return LanguageConfigManager.Instance.getLanguage("godsWar_55","2");
		}
			
		if(a==4)
		{
			return LanguageConfigManager.Instance.getLanguage("godsWar_56");
			
		}
		if(a==5)
		{
			return LanguageConfigManager.Instance.getLanguage("godsWar_92");
		}
		return "";
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
    public override void DoDisable() {
        base.DoDisable();
        UiManager.Instance.godsWarSuportWindow = null;
    }
}
