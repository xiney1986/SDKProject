using UnityEngine;
using System.Collections;

public class GodsWarShenMoItem : ButtonBase
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
	/// label-对手服务器
	/// </summary>
	public UILabel lblServerName;
	/// <summary>
	/// 组别
	/// </summary>
	public int big_id;
	/// <summary>
	/// 域名
	/// </summary>
	public int yu_ming;
	private GodsWarUserInfo enemyInfo = new GodsWarUserInfo ();

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		doViewEvent ();
	}

	/// <summary>
	/// 初始化对手信息
	/// </summary>
	public void initEnemyInfo (string uid, string serverName, string name, int style, int type, int index,bool tf)
	{
		this.uid = uid;
		this.serverName = serverName;
		this.name = name;
		this.style = style;
		this.big_id = type;
		this.yu_ming = index;
		this.isBeated = tf;
		enemyInfo.uid = uid;
		enemyInfo.serverName = serverName;
		enemyInfo.name = name;
		enemyInfo.headIcon = style;
//		enemyInfo.bigTeam = type;
//		enemyInfo.smallTeam = index;

		initUI ();
		initRender ();
	}
	/// <summary>
	/// 初始化渲染模型
	/// </summary>
	private void initRender ()
	{
		render.init (UserManager.Instance.getModelPath (style), modelTextrue, isBeated);
	}

	/// <summary>
	/// 初始化界面
	/// </summary>
	private void initUI ()
	{
		lblName.text = name;
		lblServerName.text = serverName;
		if (isBeated) {
//			beated.gameObject.SetActive (true);
		}
	}
	/// <summary>
	/// 执行查看玩家信息操作
	/// </summary>
	private void doViewEvent ()
	{
		if (enemyInfo.serverName == "") {
			MaskWindow.UnlockUI ();
			return;
		}
		UiManager.Instance.openDialogWindow<GodsWarUserInfoWindow> ((win) => {
			win.initWindow (enemyInfo.serverName, enemyInfo.name, enemyInfo.uid,big_id, yu_ming, () => {});
		});
	}
}
