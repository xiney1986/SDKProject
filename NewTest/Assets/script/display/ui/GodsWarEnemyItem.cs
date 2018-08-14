using UnityEngine;
using System.Collections;

public class GodsWarEnemyItem : ButtonBase
{
	/// <summary>
	/// 变量-对手uid
	/// </summary>
	private string uid;
	/// <summary>
	/// 变量-对手服务器
	/// </summary>
	private string serverName;
	/// <summary>
	/// 变量-名字
	/// </summary>
	private string name;
	/// <summary>
	/// 变量-等级
	/// </summary>
	private int lv;
	/// <summary>
	/// 变量-打败可获得积分
	/// </summary>
	private int winScore;
	/// <summary>
	/// 变量-是否被打败
	/// </summary>
	private bool isBeated = false;
	/// <summary>
	/// 变量-模型id
	/// </summary>
	private int style;
	/// <summary>
	/// texture-3d投影
	/// </summary>
	public UITexture modelTextrue;
	/// <summary>
	/// sprite-是否被打败
	/// </summary>
	public UISprite beated;
	/// <summary>
	/// gameObject-3d模型渲染
	/// </summary>
	public RenderView render;
	/// <summary>
	/// label- 对手名字
	/// </summary>
	public UILabel lblName;
	/// <summary>
	/// label-对手等级
	/// </summary>
	public UILabel lblLv;
	/// <summary>
	/// label-对手服务器
	/// </summary>
	public UILabel lblServerName;
	/// <summary>
	/// label-打败可获得积分
	/// </summary>
	public UILabel lblWinScore;
	/// <summary>
	/// 模型的下标（用于把模型丢开，避免摄像机重复照射）
	/// </summary>
	private int index;
	public GameObject buttomInfo;
	/// <summary>
	/// 点击回调
	/// </summary>
	public CallBack<GodsWarUserInfo> callback;
	private GodsWarUserInfo enemyInfo = new GodsWarUserInfo();

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		inToFight();
	}

	/// <summary>
	/// 初始化对手信息
	/// </summary>
	public void initEnemyInfo(int index,string uid,string serverName,string name,int level,int winScore,bool isbeated,int style,CallBack<GodsWarUserInfo> callback)
	{
		this.index = index;
		this.uid = uid;
		this.serverName = serverName;
		this.name = name;
		this.lv = level;
		this.winScore = winScore;
		this.isBeated =isbeated;
		this.style = style;
		this.callback = callback;
		enemyInfo.uid = uid;
		enemyInfo.serverName = serverName;
		enemyInfo.name = name;
		enemyInfo.level = level;
		enemyInfo.winScore = winScore;
		enemyInfo.challengedWin = isbeated;
		enemyInfo.headIcon = style;

		initUI();
		initRender();
	}
	/// <summary>
	/// 初始化渲染模型
	/// </summary>
	private void initRender()
	{
		render.init(UserManager.Instance.getModelPath (style), modelTextrue,isBeated);
		render.gameObject.transform.localPosition = new Vector3(index*500f,index*500f,0);
	}

	/// <summary>
	/// 初始化界面
	/// </summary>
	private void initUI()
	{
        lblName.text = "[fff8e2]" + name + "[-]" + "[fff8e2]" + " Lv. " + lv.ToString() + "[-]";
		//lblLv.text = "Lv. "+lv.ToString();
		if(isBeated)
		{
			beated.gameObject.SetActive(true);
			buttomInfo.gameObject.SetActive(false);
			lblWinScore.gameObject.SetActive(false);
		}

		int stage_20 = GodsWarInfoConfigManager.Instance ().getSampleBySid (4001).num[0];
		int stage_20_35 = GodsWarInfoConfigManager.Instance ().getSampleBySid (4002).num[0];
		int stage_35_50 = GodsWarInfoConfigManager.Instance ().getSampleBySid (4003).num[0];
		if (winScore < stage_20) {
			lblWinScore.text = Colors.RED+LanguageConfigManager.Instance.getLanguage("godsWar_100")+"+"+winScore.ToString();
		}else if(winScore < stage_20_35){
            lblWinScore.text = Colors.WHITE + LanguageConfigManager.Instance.getLanguage("godsWar_100") + "+" + winScore.ToString();
		}else if(winScore < stage_35_50){
            lblWinScore.text = Colors.GREEN + LanguageConfigManager.Instance.getLanguage("godsWar_100") + "+" + winScore.ToString();
		}else if(winScore >= stage_35_50){
            lblWinScore.text = Colors.GREEN + LanguageConfigManager.Instance.getLanguage("godsWar_100") + "+" + winScore.ToString() + "(Max)";
		}
//		if(index==1||index==2)
//		{
//			buttomInfo.gameObject.transform.localPosition = new Vector3(0,150f,0);
//		}
//		if(index==3||index==4)
//		{
//			modelTextrue.width = 200;
//			modelTextrue.height = 200;
//		}
//		if(index ==5)
//		{
//			modelTextrue.width = 210;
//			modelTextrue.height = 210;
//		}

		if(index==2 || index==4)
		{
			buttomInfo.gameObject.transform.localPosition = new Vector3(0,150f,0);
		}
		if(index==1||index==5)
		{
			modelTextrue.width = 200;
			modelTextrue.height = 200;
		}
		if(index ==3)
		{
			modelTextrue.width = 210;
			modelTextrue.height = 210;
		}
	}
	/// <summary>
	/// 进入战斗
	/// </summary>
	public void inToFight()
	{
		Debug.Log("there will intoFight");
	    if (callback != null && enemyInfo != null)
	    {
            System.DateTime serverDate = ServerTimeKit.getDateTime();
            int currentTime = serverDate.Hour * 3600 + serverDate.Minute * 60 + serverDate.Second;
	        if (currentTime >= 86340)
	        {
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) =>
                {
                      win.Initialize(LanguageConfigManager.Instance.getLanguage("godswAR_l415555"));
                 });
                return;
	        }
            callback(enemyInfo);
	    }
			
	}

}
