using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 守护天使展示窗口
/// </summary>
public class DefendingAngelWindow : WindowBase {
	//位置
	private Vector3 pos1 = new Vector3(0f,0f,0f);
	private Vector3 pos2 = new Vector3(0f,-43f,0f);
	private Vector3 pos3 = new Vector3(0f,-39f,0f);
	private Vector3 pos4 = new Vector3 (-122f, 0f, 0f);

	//守护天使展示容器
	public SampleDynamicContent sampleContent;
	//**描述UILabel*/
	public UILabel[] des;
	//当前显示的天使
	private AngelSample curAngel;
	//天使列表
	private List<AngelSample> angelList;
	//**箭头*/
	public GameObject arrowLeft;
	public GameObject arroRight;
	//**描述*/
	public GameObject desObj;
	//**属性*/
	public GameObject attrObj;
    public GameObject bg;
    public GameObject bg1;
	//**天使名字*/
	public UILabel angelName;
	//**天使等級*/
	public UILabel angelLevel;
	//**天使描述*/
	public UILabel angelDes;
	//**天使经验条*/
	public barCtrl angelExpBar;
	//**拖动控制*/
	public UIDragScrollView angelDrag;
	//**激活按钮*/
	public ButtonBase activateButton;
	private int curAngelIndex = 1;
	private int ownIndex = 0;
	/***/
	protected override void DoEnable () {
		base.DoEnable ();
		UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
	}
	/** 断线重链 */
	public override void OnNetResume () {
		base.OnNetResume ();
		UpdateUI();
	}
	/** 初始化 */
	public void init () {
		angelList = AngelSampleManager.Instance.getAllAngelSamples ();
		curAngel = AngelSampleManager.Instance.getAngelSampleByVipLevel (UserManager.Instance.self.getVipLevel());
		if (curAngel == null || PlayerPrefs.GetString(PlayerPrefsComm.ANGEL_USER_NAME+UserManager.Instance.self.uid)== "not") {
			activateButton.gameObject.SetActive (true);
			attrObj.SetActive(false);
            bg.SetActive(false);
            bg1.SetActive(true);
			angelDrag.enabled = false;
			curAngel = angelList [0];
			desObj.transform.localPosition = pos1;
		}
		else {
			ownIndex = curAngel.index;
			activateButton.gameObject.SetActive (false);
			angelDrag.enabled = true;
			desObj.transform.localPosition = pos2;
			attrObj.SetActive(true);
            bg.SetActive(true);
            bg1.SetActive(false);
			Transform item = sampleContent.gameObject.transform.GetChild(ownIndex >= 3 ? 2 : ownIndex - 1).GetComponent<Transform>();
			item.GetChild(0).GetComponent<Transform>().localPosition = pos4;
		}
		sampleContent.maxCount = angelList.Count;
		sampleContent.startIndex = curAngel.index - 1;
		sampleContent.onLoadFinish = onContentFinish;
		sampleContent.callbackUpdateEach = updatePage;
		sampleContent.onCenterItem = updateActivePage;
		sampleContent.init ();
 		UpdateUI();
	}
	/** begin */
	protected override void begin () {
		base.begin ();
		MaskWindow.UnlockUI ();
	}
	/** 更新UI */
	public void UpdateUI(){
		int index = 0;
		while (index < curAngel.description.Length - 1) {
			des[index].gameObject.SetActive(true);
			des[index].text = curAngel.description[index++];
		}
		while (index<4) {
			des[index++].gameObject.SetActive(false);
		}
	}
	/** 点击事件 */
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
		} else if(gameObj.name=="activate") {
            if (UserManager.Instance.self.getVipLevel() > 5)
            {
                PlayerPrefs.SetString(PlayerPrefsComm.ANGEL_USER_NAME + UserManager.Instance.self.uid, "ok");
                UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("Angel02"));
                init();
            }
            else
            {
                UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("Angel03"));
                MaskWindow.UnlockUI();
            }
			//UiManager.Instance.createMessageLintWindow("your vip Level is zero !");
		}
	}
	//滑动后更新
	private void updateActivePage (GameObject obj)
	{
		curAngelIndex = StringKit.toInt (obj.name) - 1;
		curAngel = angelList [curAngelIndex];
		angelName.text = curAngel.name;
		Transform angelTran = obj.transform.GetChild (0).GetComponent<Transform> ();
		if (curAngel.index != ownIndex) {
			attrObj.SetActive (false);
            bg.SetActive(false);
            bg1.SetActive(true);
			angelTran.localPosition = new Vector3(0f,0f,0f);
		}
		else {
			User user=UserManager.Instance.self;
			attrObj.SetActive(true);
            bg.SetActive(true);
            bg1.SetActive(false);
			angelTran.localPosition = pos4;
			angelLevel.text = "Lv." + user.getVipLevel();
			angelDes.text = curAngel.description[curAngel.description.Length - 1];
			angelExpBar.updateValue(user.getVipLevel(),12);
		}
		AngelItem item = obj.GetComponentInChildren<AngelItem> ();
		item.init (this,curAngel);
		UpdateUI ();
		AngelItem[] list = sampleContent.gameObject.GetComponentsInChildren<AngelItem> ();
		for (int i = 0; i < list.Length; i++) {
			if(StringKit.toInt(list[i].gameObject.name)-1 != curAngelIndex)
				list[i].removeAngelModel();
		}
		//更新箭头
		if (angelList.Count == 1) {
			arrowLeft.gameObject.SetActive (false);
			arroRight.gameObject.SetActive (false);
		} else if (curAngel.index == 1) {
			arrowLeft.gameObject.SetActive (false);
            arroRight.gameObject.SetActive(angelDrag.enabled);
		} else if (curAngel.index == angelList.Count) {
			arrowLeft.gameObject.SetActive (true);
			arroRight.gameObject.SetActive (false);
		} else {
			arrowLeft.gameObject.SetActive (true);
			arroRight.gameObject.SetActive (true);
		}
	}
	//更新完后的处理
	private void onContentFinish(){
		//TODO
	}
	//更新选中页
	private void updatePage (GameObject obj)
	{
		//TODO
	}
}