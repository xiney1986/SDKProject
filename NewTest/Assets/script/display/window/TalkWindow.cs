using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

/**
 * 剧情对话
 * @author 汤琦
 * */
 
public class TalkWindow : WindowBase
{
	private const string TALKANIMLEFT = "talkAnim1";//显示左边动画
	private const string TALKANIMRIGHT = "talkAnim2";//显示右边动画
	private int dialogNum = 0;//对话数量
	
	
	public TalkCtrl[] talks;//存放会话框
	private TalkCtrl activeTalk;//存放活动会话框
	public Animation talkAnim;//动画
	public GameObject nextButton; //按钮
	private DialogueSample dialogue;//存放会话信息
	int index = 0;//控制会话内容节点
	private int[] location = {0,0,0};//位置是否被占用判断,0代表未被占用,1代表被占用,依次是左,右,中
	private string[] talkerNames = {"",""};//存放名字
	private bool controlClick = false;//控制点击
	private bool isSkip = false;//是否跳过
	float pi = 3.14f;//圆周率
	float time = 0;//时间变量
	int wordsLong;//存储当前会话内容的长度 
	int wordsCount = 0;//获得一句会话内容的段数
	bool isShow = false;//控制是否显示会话内容提示图
	CallBack talkCallBack;
	public ButtonBase skipButton;//剧情跳过按钮
	private const int SHOWSKIPBUTTONLEVEL = 8;//显示剧情跳过按钮等级
	CallBack calllBackSpecial;
	bool isTalkOver=false;//是否对话已经结束
	private bool isAutoMove=false;
	/** 计时器 */
	private Timer timer;
	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();

	}
	
	void Update ()
	{
		time += Time.deltaTime;
		if (isShow) {
			showTalkPen ();
		}
	}

	private string getName (string _name)
	{
		if (_name == LanguageConfigManager.Instance.getLanguage ("userName"))
			return UserManager.Instance.self.nickname;
		else
			return _name;
	}
	
	//刷新对话框
	private void reLoadDialogBox (DialogueSample dialog, int dialogIndex)
	{
		StartCoroutine ("printer", dialogIndex);
		if (dialog.iconId == 0)
			talks [dialog.loction - 1].talkerName.text = dialog.name;
		else
			talks [dialog.loction - 1].talkerName.text = getName (dialog.name);
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + dialog.iconId, talks [dialog.loction - 1].image.mainTexture);
	}
	//判读位置上是否有对话 
	private bool isLoctionHaveDialog (int index)
	{
		if (location [index - 1] != 0) {
			return true;
		} else {
			return false;
		}
	}
	//判断一段话是否结束
	private bool isWordsOver (int index)
	{
		if (index > wordsCount) {
			return true;
		} else {
			return false;
		}
	}
	//判断会话是否结束
	private bool isDialogOver ()
	{
		dialogue = PlotManagerment.Instance.getNextDialogues ();
		if (dialogue == null) {
			return false;
		} else {
			if (activeTalk != null) {	
				dialogNum --;
				if (activeTalk == talks [1]) {
					cardChange (talks [1], talks [0], "talkAnim2");
					talks [1].talkText.text = "";
				} else if (activeTalk == talks [0]) {
					cardChange (talks [0], talks [1], "talkAnim1");
					talks [0].talkText.text = "";
				}
				index = 0;
			}
			
			return true;
		}
	}
	//如果切换的对话人物已经在界面中存在
	private void interfaceHaveName (string name)
	{
		for (int i = 0; i < talkerNames.Length; i++) {
			if (talkerNames [i] == name) {
				talks [1].animCtrl.transform.localPosition = new Vector3 (900, 0, 0);
				talkerNames [1] = "";
				StartCoroutine (showTalk ());
			}
		}
	}
	//如果切换的对话人物已经在界面中存在
	private bool isName (string name)
	{
		for (int i = 0; i < talkerNames.Length; i++) {
			if (talkerNames [i] == name) {
				return true;
			}
		}
		return false;
	}
	//初始化
	public void Initialize (int sid, CallBack callBack, CallBack calllBackSpecial)
	{
		isTalkOver=false;
		if (GuideManager.Instance.isOverStep(GuideGlobal.NEWOVERSID)) {
			skipButton.gameObject.SetActive (true);
		} else {
			skipButton.gameObject.SetActive (false);
		}
		this.calllBackSpecial = calllBackSpecial;
		talkCallBack = callBack;
		talkAnim [TALKANIMRIGHT].speed = 2f;
		talkAnim [TALKANIMLEFT].speed = 2f;
		controlClick = false;
		PlotManagerment.Instance.start (sid);
		dialogNum = PlotManagerment.Instance.getSamplesLength ();
		dialoging ();
	}
	//初始化
	public void Initialize (int sid, CallBack callBack, CallBack calllBackSpecial,bool bo)
	{
		isAutoMove=bo;
		isTalkOver=false;
		if (GuideManager.Instance.isOverStep(GuideGlobal.NEWOVERSID)) {
			skipButton.gameObject.SetActive (true);
		} else {
			skipButton.gameObject.SetActive (false);
		}
		this.calllBackSpecial = calllBackSpecial;
		talkCallBack = callBack;
		talkAnim [TALKANIMRIGHT].speed = 2f;
		talkAnim [TALKANIMLEFT].speed = 2f;
		controlClick = false;
		PlotManagerment.Instance.start (sid);
		dialogNum = PlotManagerment.Instance.getSamplesLength ();
		dialoging ();
		if(bo){
			timer = TimerManager.Instance.getTimer (200);
			timer.addOnTimer (autoClick);
			timer.start ();
		}
	}
	void autoClick(){
		if (this == null || !gameObject.activeInHierarchy) {
			if (timer != null) {
				timer.stop ();
				timer = null;
			}
			return;
		}
		if (controlClick == false) {
			if (talks [dialogue.loction - 1].talkText.text.Length >= wordsLong) {
				index++;
				roteFulcrum (activeTalk);
			} else {
				StopCoroutine ("printer");
				string newwords = dialogue.dialogues [index];
				if (UserManager.Instance.self != null && UserManager.Instance.self.star != 0) {
					newwords = newwords.Replace ("%-1", UserManager.Instance.self.nickname);
					newwords = newwords.Replace ("%-2", HoroscopesManager.Instance.getStarByType (UserManager.Instance.self.star).getName ());
				}
				talks [dialogue.loction - 1].talkText.text = newwords;
			}
		} 

	}

	//特殊用途，未创建玩家信息时对话
	public void init (int sid, CallBack callBack, CallBack calllBackSpecial)
	{
		isTalkOver=false;
		skipButton.gameObject.SetActive (false);
		this.calllBackSpecial = calllBackSpecial;
		talkCallBack = callBack;
		talkAnim [TALKANIMRIGHT].speed = 2f;
		talkAnim [TALKANIMLEFT].speed = 2f;
		controlClick = false;
		PlotManagerment.Instance.start (sid);
		dialogNum = PlotManagerment.Instance.getSamplesLength ();
		dialoging ();
	}
	
	public void setIsSkip (bool isSkip)
	{
		this.isSkip = isSkip;
	}
	
	//会话进行
	public void dialoging ()
	{
		//判断是否有下一个会话
		if (isDialogOver ()) {
			wordsCount = dialogue.dialogues.Length;
			//判断某个位置上是否有会话
			if (!isLoctionHaveDialog (dialogue.loction)) {
				walkOn (dialogue.loction, dialogue.intoLoc, dialogue.name, dialogue.iconId);
				return;
			} else {
				//判断界面上会话的人物名字是否存在
				if (!isName (dialogue.name)) {
					talks [1].animCtrl.transform.localPosition = new Vector3 (900, 0, 0);
					talkerNames [1] = "";
					StartCoroutine (showTalk ());
					return;
				} else {
					walkOn (dialogue.loction, dialogue.intoLoc, dialogue.name, dialogue.iconId);
					return;
				}
			}
		} else {
			activeTalk = null;
//			if(isAutoMove){
//				StartCoroutine(MissionManager.instance.autoMove(1.0f));
//				isAutoMove=false;
//			}
			StartCoroutine (talkOver ());
		}
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		MaskWindow.UnlockUI ();
		if (controlClick == false) {
			if (talks [dialogue.loction - 1].talkText.text.Length >= wordsLong) {
				index++;
				roteFulcrum (activeTalk);
			} else {
				StopCoroutine ("printer");
				string newwords = dialogue.dialogues [index];
				if (UserManager.Instance.self != null && UserManager.Instance.self.star != 0) {
					newwords = newwords.Replace ("%-1", UserManager.Instance.self.nickname);
					newwords = newwords.Replace ("%-2", HoroscopesManager.Instance.getStarByType (UserManager.Instance.self.star).getName ());
				}
				talks [dialogue.loction - 1].talkText.text = newwords;
			}
		}
		if (gameObj.name == "skipButton" ) {
			StartCoroutine (talkOver ());

		}
	} 
	//对话切换
	public void roteFulcrum (TalkCtrl _activeTalk)
	{


		if (controlClick == true) {
			return;
		}
		if (index >= wordsCount) {
			dialoging ();
		} else {
			wordsCount--;
			StartCoroutine ("printer", index);
		}
	}
	//图片切换
	private void cardChange (TalkCtrl talk1, TalkCtrl talk2, string talkAnimName)
	{
		talkAnim [talkAnimName].time = 0;
		talkAnim.Play (talkAnimName);
		talk1.changeBlack ();
		talk1.changeDepth (false);

		talk2.changeLight ();
		talk2.changeDepth (true);
		AudioManager.Instance.PlayAudio (103);
	}

	public IEnumerator talkOver ()
	{
		//防止重复调用
		if(isTalkOver)
			yield break;

		isTalkOver=true;
		skipButton.gameObject.SetActive (false);
		controlClick = true; 
		isShow = false;
		index = 0;
		talks [0].talkOut (4);
		talks [1].talkOut (2);
		yield return new WaitForSeconds (0.5f);

		finishWindow ();
		EventDelegate.Add (onDestroy, () => {
			if (calllBackSpecial != null){
				calllBackSpecial ();
				calllBackSpecial=null;
			}
			if (talkCallBack != null){
				talkCallBack ();
				talkCallBack=null;
			}
		});

		PlotManagerment.Instance.over();//jira:2438 剧情跳过后未清空缓存,对话重复出现2次BUG,待验证 2014.4.14
		StopCoroutine ("talkOver");
	}
	
	//进场
	private void walkOn (int loction, int intoLoc, string name, int iconId)
	{
		talks [loction - 1].image.mainTexture = null;

		//无图无真相
		if (iconId == 0) {
			talks [loction - 1].image.gameObject.SetActive (false);
			talks [loction - 1].image.height = 512;
			talks [loction - 1].image.width = 512;
		}
		//玩家
		else if (iconId == -1) {
			talks [loction - 1].image.gameObject.SetActive (true);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + StorageManagerment.Instance.getRole (UserManager.Instance.self.mainCardUid).getImageID (), talks [loction - 1].image);
			talks [loction - 1].image.height = 512;
			talks [loction - 1].image.width = 512;
		}
		//女神2050-2061
		else if (iconId >= 2050 && iconId <= 2061) {
			BeastEvolveManagerment instance = BeastEvolveManagerment.Instance;
			int beastIndex = instance.getBeastIndexByImageId (iconId)-1;
			BeastEvolve be = instance.getBeastEvolveByIndex (beastIndex);
			talks [loction - 1].image.gameObject.SetActive (true);
			if(be != null && be.getBeast ().getQualityId() > 1){
				ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + iconId + "c", talks [loction - 1].image);
			}else{
                if(CommandConfigManager.Instance.getNvShenClothType() == 0)
				    ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + iconId+"c", talks [loction - 1].image);
                else ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.CARDIMAGEPATH + iconId, talks[loction - 1].image);
			}

			talks [loction - 1].image.height = 1024;
			talks [loction - 1].image.width = 1024;
			talks [loction - 1].image.transform.localPosition = new Vector2 (35, 200);
		}
		//我的星座女神-2
		else if (iconId == -2) {
			talks [loction - 1].image.gameObject.SetActive (true);
		    if (UserManager.Instance.self != null && UserManager.Instance.self.star != 0)
		    {

		        if (CommandConfigManager.Instance.getNvShenClothType() == 0)
		            ResourcesManager.Instance.LoadAssetBundleTexture(
		                ResourcesManager.CARDIMAGEPATH +
		                HoroscopesManager.Instance.getStarByType(UserManager.Instance.self.star).getImageID() + "c",
		                talks[loction - 1].image);
		        else
		            ResourcesManager.Instance.LoadAssetBundleTexture(
		                ResourcesManager.CARDIMAGEPATH +
		                HoroscopesManager.Instance.getStarByType(UserManager.Instance.self.star).getImageID(),
		                talks[loction - 1].image);
		    }
		    else
		    {
                if(CommandConfigManager.Instance.getNvShenClothType() == 0)
		            ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.CARDIMAGEPATH + iconId + "c",
		            talks[loction - 1].image);
                else ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.CARDIMAGEPATH + iconId,
                    talks[loction - 1].image);
		    }
		    talks [loction - 1].image.height = 1024;
			talks [loction - 1].image.width = 1024;
			talks [loction - 1].image.transform.localPosition = new Vector2 (35, 200);
		} else {
			talks [loction - 1].image.gameObject.SetActive (true);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + iconId, talks [loction - 1].image);
			talks [loction - 1].image.height = 512;
			talks [loction - 1].image.width = 512;
		}

		location [loction - 1] = 1;
		talkerNames [loction - 1] = name;
		talks [loction - 1].talkIn (intoLoc);

		if (iconId == -1)//玩家
			talks [loction - 1].talkerName.text = getName (name);
		else if (iconId >= 2050 && iconId <= 2061)//女神
		{
			talks [loction - 1].talkerName.text = HoroscopesManager.Instance.getStarByType(BeastEvolveManagerment.Instance.getBeastIndexByImageId (iconId)).getName()
				+ LanguageConfigManager.Instance.getLanguage ("goddess");
		}
		else if (iconId == -2)//我的星座女神-2
		{
			if (UserManager.Instance.self != null && UserManager.Instance.self.star != 0)
				talks [loction - 1].talkerName.text = HoroscopesManager.Instance.getStarByType(UserManager.Instance.self.star).getName() + LanguageConfigManager.Instance.getLanguage ("goddess");
			else
				talks [loction - 1].talkerName.text = name;
		}
		else
			talks [loction - 1].talkerName.text = name;

		StartCoroutine ("printer", 0);
		activeTalk = talks [loction - 1];
	}
	//会话延迟展现
	public IEnumerator showTalk ()
	{
		yield return new WaitForSeconds (0.1f);
		if (dialogue != null) {
			walkOn (dialogue.loction, dialogue.intoLoc, dialogue.name, dialogue.iconId);
		}
	}
	//打字机效果
	private IEnumerator printer (int index)
	{ 
		if (dialogNum == 1) {
			isShow = false;
		}
		if (dialogNum != 1 || wordsCount != 1) {
			isShow = true;
		}
		string str = string.Empty;
		bool isPrint = true;
		//
		//talks [dialogue.loction - 1].talkPen.alpha = 0;
		talks [dialogue.loction - 1].talkArrow.SetActive(false);
		talks [dialogue.loction - 1].talkText.text = "";
		string words = dialogue.dialogues [index];
		//这里是筛选关键字替换
		if (UserManager.Instance.self != null && UserManager.Instance.self.star != 0) {
			words = words.Replace ("%-1", UserManager.Instance.self.nickname);
			words = words.Replace ("%-2", HoroscopesManager.Instance.getStarByType (UserManager.Instance.self.star).getName ());
		}
		wordsLong = words.Length;
		foreach (char item in words) {
			if (item == '[') {
				isPrint = false;
			} else if (item == ']') {
				isPrint = true;
				
			} 
			if (isPrint) {
				talks [dialogue.loction - 1].talkText.text += str + item.ToString ();
				str = string.Empty;
				yield return new WaitForSeconds (0.055f);
			} else {
				str += item;
			}
		}
	}
	
	private void showTalkPen ()
	{
		if (talks [dialogue.loction - 1].talkText.text.Length >= wordsLong) {
			//talks [dialogue.loction - 1].talkPen.alpha = 0;
			//talks [dialogue.loction - 1].talkPen.transform.localPosition = new Vector3 (280, -110 + 10 * Mathf.Sin (50 * time * pi * 0.05f), -15);
			talks [dialogue.loction - 1].talkArrow.SetActive(true);
		}
	}

	public override void DoDisable ()
	{
		base.DoDisable ();
		talks[0].image.mainTexture=null;
		talks[1].image.mainTexture=null;

		if (fatherWindow is EmptyWindow) {
			fatherWindow.finishWindow ();
		}

		MaskWindow.UnlockUI ();
 
	}
}
