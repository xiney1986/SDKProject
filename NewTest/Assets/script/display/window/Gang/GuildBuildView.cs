using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuildBuildView : ButtonBase {

	public const bool SHOW_TITLE = true,HIDE_TITLE = false;

	/** 建筑Sid */
	public int buildSid = 0;
	/** 是否显示标题 */
	public bool isShowTitle = true;
	/** 建筑图标 */
	public UISprite spriteIcon;
	/** 建筑标题背景 */
	public GameObject objTitle;
	/** 建筑等级标题 */
	public GameObject objLvTitle;
	/** 建筑锁图标 */
	public UISprite spriteLock;
	/** 建筑名称 */
	public UILabel labelName;
	/** 建筑等级 */
	public UILabel labelLv;
	/** 状态图标 **/
	public UISprite stateIcon;
    public GameObject leffectPoint;

	/** 建筑样板 */
	private GuildBuildSample buildSample;
	/** 所有公会建筑sid */
	private List<int> buildSidList;
	/** 建筑等级 */
	private int buildLevel;
	private CallBack callback;
	private Color pressed = new Color(183f / 255f, 163f / 255f, 123f / 255f, 1f);
	private Color hover = new Color(225f / 255f, 200f / 255f, 150f / 255f, 1f);
	private Color disabledColor = Color.grey;

	public override void OnAwake () {
		base.OnAwake ();
		UIButton ngui_buttonScript = this.GetComponent<UIButton>();
		if (ngui_buttonScript != null) {
			ngui_buttonScript.normalSprite = null;
			ngui_buttonScript.disabledColor = disabledColor;
			ngui_buttonScript.hover = hover;
			ngui_buttonScript.pressed = pressed;
		}
	}

	/// <summary>
	/// 刷新建筑信息
	/// </summary>
	/// <param name="sid">Sid.</param>
	public void updateInfo (int sid) {
		this.buildSid = sid;
		updateInfo ();
	}

	/// <summary>
	/// 初始化建筑
	/// </summary>
	/// <param name="sid">建筑Sid.</param>
	/// <param name="callback">回调.</param>
	public void initBuild (int sid, bool _isShowTitle, CallBack _callback) {
		this.buildSid = sid;
		this.isShowTitle = _isShowTitle;
		this.callback = _callback;
		updateInfo ();
	}

	/// <summary>
	/// 更新信息
	/// </summary>
	public void updateInfo () {
		buildSidList = GuildBuildSampleManager.Instance.getAllGuildBuild();
		if (!buildSidList.Contains (buildSid)) {
			this.gameObject.SetActive (false);
			Debug.LogError ("No GuildBuild By Sid =" + buildSid);
			return;
		}
		this.gameObject.SetActive (true);
		if (isShowTitle) {
			objTitle.SetActive (true);
		} else {
			objTitle.SetActive (false);
		}
		buildSample = GuildBuildSampleManager.Instance.getGuildBuildSampleBySid(buildSid);
		buildLevel = GuildManagerment.Instance.getBuildLevel(buildSid.ToString());
		labelName.text = buildSample.buildName;
		setBuildInfo ();
		/**显示状态图标*/
		if(GuildManagerment.Instance.isUpGuildBuildState(buildSample.sid,buildLevel)){
			stateIcon.gameObject.SetActive(true);
		}else{
			stateIcon.gameObject.SetActive(false);
		}
        updateEffect();
	}
    private void updateEffect()
    {
        if (buildSid == 5 && GuildFightSampleManager.Instance().isGuildFightAndFrontFiveMin())

            leffectPoint.SetActive(true);
        
        else
            leffectPoint.SetActive(false);
    }
	/// <summary>
	/// 设置图标&等级
	/// </summary>
	private void setBuildInfo()
	{
		if (buildLevel > 0) {
			spriteLock.gameObject.SetActive (false);
			labelLv.text = buildLevel.ToString ();
			objLvTitle.SetActive (true);
		}
		else {
			spriteLock.gameObject.SetActive (true);
			labelLv.text = "";
			objLvTitle.SetActive (false);
		}

		spriteIcon.spriteName = GuildManagerment.Instance.getBuildIcon (buildSample.sid);
        if (buildSample.sid == 1 || buildSample.sid == 2 || buildSample.sid == 5)
        {
            spriteIcon.gameObject.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
        }
	}

	public override void DoClickEvent () {
		base.DoClickEvent ();
		if (callback != null) {
			callback ();
		} else {
//			MaskWindow.UnlockUI ();
		}
	}


}
