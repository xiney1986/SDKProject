using UnityEngine;
using System.Collections;

public class TextTipWindow : WindowBase {

	private static string lastTipString;

	const float animationTime = 0.6f;

	public UILabel label;
	public UISprite bg;
	public UISprite bgFront;

	[HideInInspector]
	private string str;
	private float showTime;


	void Start()
	{
		if (str == null) {
			str = "NULL";
		}
		label.text = str;
		Bounds b = NGUIMath.CalculateRelativeWidgetBounds(label.transform);
		bg.height = (int)b.extents.y * 2 + 80;
		bgFront.height = (int)b.extents.y * 2 + 85;
		
		animationIn ();


		float delay = animationTime;
		if (showTime == 0) {
			if (lastTipString == str) {
				delay += 0.1f + str.Length * 0.02f;
			} else {
				delay += 0.2f + str.Length * 0.02f;
			}
		} else {
			delay += showTime;
		}
		lastTipString = str;
		StartCoroutine (Utils.DelayRun(()=>{animationOut();},delay));

	}

	public void init(string str,float showTime)
	{
		this.str = str;
		this.showTime = showTime;
	}

	private void animationIn()
	{
		float time = animationTime;
		TweenAlpha ta = TweenAlpha.Begin (label.gameObject, time, 1f);
		ta.method = UITweener.Method.EaseOut;
		ta.from = 0f;
		
		ta = TweenAlpha.Begin (bg.gameObject, time, 1f);
		ta.method = UITweener.Method.EaseOut;
		ta.from = 0f;
		
		TweenPosition tp = TweenPosition.Begin (gameObject, time, Vector3.zero);
		tp.method = UITweener.Method.EaseOut;
		tp.from = new Vector3 (0,-300,0);
	}

	private void animationOut()
	{
		float time = animationTime;
		TweenAlpha ta = TweenAlpha.Begin (label.gameObject, time, 0);
		ta.method = UITweener.Method.EaseIn;
		
		ta = TweenAlpha.Begin (bg.gameObject, time, 0);
		ta.method = UITweener.Method.EaseIn;
		
		TweenPosition tp = TweenPosition.Begin (gameObject, time, new Vector3(0,300,0));
		tp.method = UITweener.Method.EaseIn;
		EventDelegate.Add (tp.onFinished,()=>{
			destoryWindow();
		},true);
	}

	//不可用是是否解锁遮罩层
	private bool disableDoUnlockUI=true;
	public override void DoDisable ()
	{
		if(disableDoUnlockUI)
		{
			base.DoDisable ();
		}
	}


	public static void Show(string str)
	{
		Show (str, 0);
	}

	public static void Show(string str,float showTime)
	{
        UiManager.Instance.openDialogWindow<TextTipWindow>((window)=>{
            window.init (str,showTime);
        });
	}

	/// <summary>
	/// 显示一个销毁时不解锁UIMask的TextTipWindow
	/// </summary>
	/// <param name="str">String.</param>
	public static void ShowNotUnlock(string str)
	{
		UiManager.Instance.openDialogWindow<TextTipWindow>((window)=>{
			window.disableDoUnlockUI=false;
			window.init (str,0);
		});
	}

}
