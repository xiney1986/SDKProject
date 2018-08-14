using UnityEngine;
using System.Collections;

public class HoroscopesPrayContent : MonoBase
{
	public UITexture goddess;
	public ButtonHoroscopesPray buttonPray;
	public UILabel time;
	public UILabel openTime; //开放时间

	public GameObject msg; //消息
	public UILabel msgLabel; //消息文本

	public UILabel describe;
	public UILabel starName;
	public UISprite iconSprite;
	[HideInInspector]
	public WindowBase
		win;

	//奖励卡片
	public GameObject cardMove;
	public UITexture roleMove;
	public UISprite qualityMove;
	public GameObject cardStop;
	public UITexture roleStop;
	public UISprite qualityStop;
	private Vector3 beginMove; //奖励卡片移动起始位置

	private CallBack closeBack;
	private int coolTime;//祈祷时间
	private float oneSecond = -1; //一秒, 用于倒计时
	private bool isActive = false; //是否在活动时间内

	private int beginTime;
	private int endTime;
	private bool isPlayAnimation = false;
	private int setp = 0;
	private int nextSetp = 1;
	private Horoscopes myStar;//我的星座
	private Horoscopes prayStar;//赐福星座

	private Timer timer;

	public void initContent (WindowBase win)
	{

		this.win = win;
		buttonPray.content = this;
		buttonPray.fatherWindow = win;
		buttonPray.callback = starPray;
		beginMove = cardMove.transform.localPosition;

		coolTime = Mathf.Max (0, HoroscopesManager.Instance.getPrayTime () - ServerTimeKit.getSecondTime ());
		goddess.color = Color.black;
		myStar = HoroscopesManager.Instance.getStarByType (UserManager.Instance.self.star);
		starName.text = myStar.getName ();
		iconSprite.spriteName = myStar.getSpriteName ();

		beginTime = HoroscopesManager.Instance.getBeginTime ();
		endTime = HoroscopesManager.Instance.getEndTime ();
		checkTime ();
		timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY); 
		timer.addOnTimer (checkTime);
		timer.start ();
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + myStar.getImageID (), goddess);
	}

	private void NextSetp ()
	{
		++nextSetp;
	}

	//开始祈祷动画
	private void starPray (int ok)
	{
		if (ok != 0) {
			setp = 0;
			nextSetp = 1;
			prayStar = HoroscopesManager.Instance.getStarByType (ok);
			isPlayAnimation = true;
			coolTime = Mathf.Max (0, HoroscopesManager.Instance.getPrayTime () - ServerTimeKit.getSecondTime ());
			;
			buttonPray.disableButton (true);
			if (ok == UserManager.Instance.self.star)
				msgLabel.text = LanguageConfigManager.Instance.getLanguage ("horoscopesPray02", prayStar.getName ());
			else
				msgLabel.text = LanguageConfigManager.Instance.getLanguage ("horoscopesPray01", prayStar.getName ());
			checkTime ();
		}
	}

	private void playAnimation ()
	{
		if (setp == nextSetp)
			return;
		//放狗
		if (setp == 0) {
			EffectManager.Instance.CreateEffectCtrlByCache(goddess.gameObject.transform, "Effect/UiEffect/SummonBeast",(obj,ec)=>{
				ec.transform.localPosition = new Vector3 (0, goddess.transform.localPosition.y - 300, 0);
			});
			StartCoroutine (Utils.DelayRun (() =>
			{
				iTween.ValueTo (gameObject, iTween.Hash ("delay", 0.3f, "from", 0, "to", 1f, "easetype", iTween.EaseType.easeInCubic, "onupdate", "colorChange", "time", 0.4f));

				StartCoroutine (Utils.DelayRun (() =>
				{
					NextSetp ();
				}, 2f));
			}, 1f));
		}
		//骂街
        else if (setp == 1) {
			msg.SetActive (true);
			TweenPosition tp = TweenPosition.Begin (msg, 0.3f, msg.transform.localPosition);
			tp.from = new Vector3 (msg.transform.localPosition.x, -500, msg.transform.localPosition.z);
			EventDelegate.Add (tp.onFinished, () =>
			{
				NextSetp ();
			}, true);
		}
        //卡片扑过去
        else if (setp == 2) {
			AwardCache ac = AwardsCacheManager.getAwardCache (AwardManagerment.AWARDS_STAR_PRAY);

			Award aw = ac.getAward ();
			if (aw == null) {
				NextSetp ();
				return;
			}
			float delay = 0.0f;
			Card card;
			foreach (CardAward each in aw.cards) {
				StartCoroutine (Utils.DelayRun (() =>
				{
					cardMove.transform.localPosition = beginMove;
					card = CardManagerment.Instance.createCard (each.sid);
					ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + card.getImageID (), roleMove);
					qualityMove.spriteName = QualityManagerment.qualityIDToString (card.getQualityId ());
					cardMove.SetActive (true);
					TweenPosition tp = TweenPosition.Begin (cardMove, 0.8f, cardStop.transform.localPosition);
					tp.from = beginMove;
					TweenScale ts = TweenScale.Begin (cardMove, 0.8f, Vector3.one);
					ts.from = Vector3.zero;
					EventDelegate.Add (tp.onFinished, () =>
					{
						ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + card.getImageID (), roleStop);
						qualityStop.spriteName = QualityManagerment.qualityIDToString (card.getQualityId ());

						cardMove.transform.localScale = Vector3.zero;
						cardMove.SetActive (false); 
						cardStop.SetActive (true);
					});
				}, delay));
				delay += 1.2f;
			}

			StartCoroutine (Utils.DelayRun (() =>
			{
				NextSetp ();
			}, delay));
		}

		//扑过去的都扑街了
		else if (setp == 3) {
			TweenScale ts = TweenScale.Begin (cardStop, 0.8f, Vector3.zero);
			ts.from = Vector3.one;
			EventDelegate.Add (ts.onFinished, () =>
			{
				isPlayAnimation = false;
				goddess.color = Color.black;
				msg.SetActive (false);
				cardStop.SetActive (false);
				cardStop.transform.localScale = Vector3.one;
				MaskWindow.UnlockUI ();
			});
		}

		++setp;
	}

	private void checkTime ()
	{
		if (buttonPray != null) {
			int nowTime = ServerTimeKit.getCurrentSecond ();
			//在活动时间内
			if ((nowTime > beginTime) && (nowTime < endTime)) {
				if (coolTime > 0) {
					buttonPray.disableButton (true);
				} else
					buttonPray.disableButton (false);
				isActive = true;
			} else {
				isActive = false;
				buttonPray.disableButton (true);
			}
			setTimeText ();
		
		} else {
			if (timer != null) {
				timer.stop ();
				timer = null;
			}
		}
	}

	void Update ()
	{
		if (isPlayAnimation) 
			playAnimation ();
	}

	//设置时间倒计时
	private void setTimeText ()
	{
		int h, m, s;
		string str = string.Empty;
		if (--coolTime > 0) {
			h = coolTime / 3600;
			m = (coolTime % 3600) / 60;
			s = coolTime % 60;
			str = h + ":" + m + ":" + s;
		} else {
			str = "0:0:0";
		}
		time.text = str;
	}

	private string getTimeText (int time)
	{
		if (time < 10)
			return " : 0" + time.ToString ();
		else
			return " : " + time.ToString ();
	}

	void colorChange (float data)
	{
		goddess.color = new Color (data, data, data, 1);
	}
}
