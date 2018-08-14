using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**
 * 创建角色窗口
 * @author 汤琦
 * */
public class  RoleNameWindow : WindowBase
{
	public class Item
	{
		public int id;
		public Card card;
		public UITexture images;
	}

	public UIScrollView scrollView;
	public UIPanel panel;
	public UITexture[] images;
	private int[] sids = new int[]{1,2,3,4,5,6};
	private bool[] gender = new bool[]{false,false,true,false,true,false};//角色性别 false 为女 true 为男
	/** 角色原有名称 */
	public UILabel jobText;
	public EffectPrinterWord skillName;
	public EffectPrinterWord skillDes;
	public EffectPrinterWord heroStory;
	public RoleNameInput inputNick;//主角名字
	/** 职业图标 */
	public UISprite jobSprite;
	public UILabel[] infoTexts;
	public GameObject infoContent;
	public string config;
	public UISprite[] attackStars;
	public UISprite[] skillStars;
	public UISprite[] lifeStars;
	public UISprite[] speedStars;
	private int heroIconId;
	private int heroSid;
	private LinkedList<Item> list;
	LinkedListNode<Item> current = null;
	public GameObject effect_Bat;
	public GameObject effect_leaf;
	public GameObject effect_Petal;
	public Transform effectPoint;
	public int star;
	private int  nameState = 2;
	bool randomName = false;
	string defName;
	Dictionary<string,int[]> roleStarDic;


	//解析简单配置文件
	void parseConfig ()
	{
		string[] strs = config.Split ('|');
		roleStarDic = new Dictionary<string, int[]> ();
		foreach (string each in strs) {
			parseOne (each);
		}

	}

	void parseOne (string str)
	{
		//配置文件中的一个条目
		// 卡形象ID,攻击星值,技能星值,生命星值,速度星值

		string[] strs = str.Split (',');
		roleStarDic.Add (strs [0], new int[] {
			StringKit.toInt (strs [1]),
			StringKit.toInt (strs [2]),
			StringKit.toInt (strs [3]),
			StringKit.toInt (strs [4])
		});

	}

	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
		EventDelegate.Add (inputNick.onChange, onInputRoleNameChange);
		inputNick.onClickFun += onInputRoleNameClick;
	}

	protected override void DoEnable () {
		base.DoEnable ();
		UiManager.Instance.backGround.switchBackGround ("ChouJiang_BeiJing");
	}

	protected void onInputRoleNameChange ()
	{
		if (!randomName)
			nameState = 1;
	}

	protected void onInputRoleNameClick ()
	{
		randomName = false;
	}

	public  override void OnAwake ()
	{
		parseConfig ();
		list = new LinkedList<Item> ();
		for (int i = 0; i < 6; i++) {
			Card c = CardManagerment.Instance.createCard (sids [i]);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + c.getImageID (), images [i]);
			images [i].transform.localPosition = new Vector3 ((i - 2) * 270, 0, 0);
			Item item = new Item ();
			item.card = c;
			item.id = i;
			item.images = images [i];
			list.AddLast (item);
		}
		current = list.First;
		EventDelegate.Add (scrollView.horizontalScrollBar.onChange, DoUpdate);

		DoUpdate ();





		updateInfo (current.Value.card);

		StartCoroutine (Utils.DelayRunNextFrame (() => {
			SpringPanel.Begin (scrollView.gameObject, -images [UnityEngine.Random.Range (0, images.Length)].transform.localPosition, 9);
		}));
	}

	protected override void DoUpdate ()
	{
//		Debug.LogError("------------->>>2current" + current.Value.id);
		float offsetx = panel.clipOffset.x;
		LinkedListNode<Item> first = list.First;
		LinkedListNode<Item> last = list.Last;
		if (first.Value.images.transform.localPosition.x - offsetx > -400) {
			last.Value.images.transform.localPosition = new Vector3 (first.Value.images.transform.localPosition.x - 270, 0, 0);
			list.RemoveLast ();
			list.AddFirst (last);
		} else if (last.Value.images.transform.localPosition.x - offsetx < 400) {
			first.Value.images.transform.localPosition = new Vector3 (last.Value.images.transform.localPosition.x + 270, 0, 0);
			list.RemoveFirst ();
			list.AddLast (first);
		}

		LinkedListNode<Item> select = current;
		float maxScale = 0;
		LinkedListNode<Item> node = list.First;
		while (node != null) {
			float len = Math.Abs (node.Value.images.transform.localPosition.x - offsetx);
			//if(len <= 600)
			{
				float scale = Math.Max ((600 - len) / 600, 0.5f);
				node.Value.images.transform.localScale = new Vector3 (scale, scale, 1);
				node.Value.images.color = new Color (scale, scale, scale);
				if (scale > maxScale) {
					maxScale = scale;
					current = node;
				}
			}
			node = node.Next;
		}
		setInfoAlpha ((maxScale - 0.75f) * 4);

		if (select != current) {
			updateInfo (current.Value.card);
		}
	}

	private void setInfoAlpha (float alpha)
	{
		foreach (UILabel lbl in infoTexts) {
			lbl.alpha = alpha;
		}
	}

	private void setInfoActive (bool active)
	{
		if (infoContent.activeInHierarchy != active) {
			infoContent.SetActive (active);
		}
	}
	
	private string getJobName (int index)
	{
		switch (index) {
		case JobType.POWER:
			return LanguageConfigManager.Instance.getLanguage ("s0272");
		case JobType.MAGIC:
			return LanguageConfigManager.Instance.getLanguage ("s0273");
		case JobType.AGILE:
			return LanguageConfigManager.Instance.getLanguage ("s0274");
		case JobType.POISON:
			return LanguageConfigManager.Instance.getLanguage ("s0275");
		case JobType.COUNTER_ATTACK:
			return LanguageConfigManager.Instance.getLanguage ("s0276");
		case JobType.ASSIST:
			return LanguageConfigManager.Instance.getLanguage ("s0277");
		default:
			return "";
		}
	}

	void  changeRoleEffect (int sid)
	{

		effect_Petal.SetActive (false);
		effect_leaf.SetActive (false);
		effect_Bat.SetActive (false);
		//p 3,2  g:1,4  b:6,5
		if (sid == 2 || sid == 3) {
			effect_Petal.SetActive (true);
		} else if (sid == 1 || sid == 4) {
			effect_leaf.SetActive (true);
		} else if (sid == 5 || sid == 6) {
			effect_Bat.SetActive (true);
		}
	}

	private bool lastCardGender;


	void updateStar(string heroIconId ){
		//update star
		
		if(roleStarDic.ContainsKey(heroIconId)){
			
			int count=	roleStarDic[heroIconId.ToString()][0];
			for(int i=0;i<=2;i++)
			{
				if(i<=count-1)
					attackStars[i].gameObject.SetActive(true);
				else
					attackStars[i].gameObject.SetActive(false);
			}
			
			count=	roleStarDic[heroIconId.ToString()][1];
			for(int i=0;i<=2;i++)
			{
				if(i<=count-1)
					skillStars[i].gameObject.SetActive(true);
				else
					skillStars[i].gameObject.SetActive(false);
			}
			
			count=	roleStarDic[heroIconId.ToString()][2];
			for(int i=0;i<=2;i++)
			{
				if(i<=count-1)
					lifeStars[i].gameObject.SetActive(true);
				else
					lifeStars[i].gameObject.SetActive(false);
			}
			
			count=	roleStarDic[heroIconId.ToString()][3];
			for(int i=0;i<=2;i++)
			{
				if(i<=count-1)
					speedStars[i].gameObject.SetActive(true);
				else
					speedStars[i].gameObject.SetActive(false);
			}
			
		}
	}

	public void updateInfo (Card card)
	{
		AudioManager.Instance.PlayAudio (143);
		heroIconId = card.getImageID ();

		updateStar(heroIconId.ToString());


		heroSid = card.sid;
		jobText.text = CardSampleManager.Instance.getRoleSampleBySid (card.sid).name;
		jobSprite.spriteName = CardManagerment.Instance.qualityIconTextToBackGround (card.getJob ()) + "s";
		Skill sk = card.getSkills () [0];
		skillName.text = sk.getName ();
		skillDes.text = LanguageConfigManager.Instance.getLanguage ("RoleSkillDes_" + card.sid);
		heroStory.text = card.getFeatures () [0];
		changeRoleEffect (heroSid);

		lastCardGender = gender [card.sid - 1];

		if (nameState == 2) {
			inputNick.value = RandomNameManagerment.Instance.getRandomName (lastCardGender);
		}
	}

	void createRoleSuccess ()
	{
		this.gameObject.SetActive (false);
		UserManager.Instance.login ();
	}

	public void init (int star)
	{
		this.star = star;
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "confirm") {
			if (Utils.EncodeToValid (inputNick.value)) {
				UiManager.Instance.openDialogWindow<MessageWindow> ((window) => {
					window.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, LanguageConfigManager.Instance.getLanguage ("roleNameWindow_Validname"), null);
				});
				return;
			} else if (getLength (inputNick.value) <= 0) {
				UiManager.Instance.openDialogWindow<MessageWindow> ((window) => {
					window.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, LanguageConfigManager.Instance.getLanguage ("s0438"), null);
				});
				return;
			}
			if (getLength (inputNick.value) > 12) {
				UiManager.Instance.openDialogWindow<MessageWindow> ((window) => {
					window.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, LanguageConfigManager.Instance.getLanguage ("s0278"), null);
				});
				return;
			}
			if (ShieldManagerment.Instance.isContainShield (inputNick.value)) {
				UiManager.Instance.openDialogWindow<MessageWindow> (
					(window) => {
					window.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, LanguageConfigManager.Instance.getLanguage ("s0279"), null);
				}
				);

				return;
			}
			CreateRoleFPort crf = FPortManager.Instance.getFPort ("CreateRoleFPort") as CreateRoleFPort;
			crf.access (inputNick.value, current.Value.card.getImageID (), star, current.Value.card.sid, createRoleSuccess);
			//	destoryWindow();
		} else if (gameObj.name == "close") {
			finishWindow ();
		} else if (gameObj.name == "randomButton") {
			nameState = 2;
			randomName = true;
			string newName = RandomNameManagerment.Instance.getRandomName (lastCardGender);
			inputNick.value = newName;
			inputNick.UpdateLabel ();
			MaskWindow.UnlockUI ();
		}
	}
	//获得字符串字节长度
	private int getLength (string str)
	{
		if (string.IsNullOrEmpty (str))
			return 0;

		int len = 0;
		foreach (char each in str.ToCharArray()) {
			if ((int)each >= 19968 && (int)each <= 40959) {
				len += 2;
			} else
				len += 1;
		}
		return len;

	}

}
