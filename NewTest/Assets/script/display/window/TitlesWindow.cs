using UnityEngine;
using System.Collections;

public class TitlesWindow : WindowBase {

	public UILabel showLabel;
	public GameObject skipStoryButton;

	public override void OnAwake () {
		base.OnAwake ();
		skipStoryButton.SetActive(true);
	}

	protected override void begin ()
	{
		base.begin ();
		if(GameManager.Instance.skipGuide)
			showTalkWindowCallBack();
		else{
			showUI();
		}
		MaskWindow.UnlockUI ();
	}

	void showUI()
	{
		if (GameManager.Instance.skipStory)
			return;
		labelEffect(LanguageConfigManager.Instance.getLanguage("GuideTitles01"),()=>{
			labelEffect(LanguageConfigManager.Instance.getLanguage("GuideTitles02"),()=>{
				labelEffect(LanguageConfigManager.Instance.getLanguage("GuideTitles03"),()=>{
					labelEffect(LanguageConfigManager.Instance.getLanguage("GuideTitles04"),()=>{
						initWalk();
					});
				});
			});
		});
	}

	void initWalk()
	{
		if (GameManager.Instance.skipStory)
			return;
		UiManager.Instance.openDialogWindow<TalkWindow>((win)=>{
			win.init (15001, showTalkWindowCallBack, null);
		});
	}

	void showTalkWindowCallBack() {
		if (GameManager.Instance.skipStory)
			return;
		skipStoryButton.SetActive (false);
		UiManager.Instance.switchWindow<ChooseHoroscopesWindow>();
	}

	void labelEffect(string str,CallBack callback)
	{
		if (GameManager.Instance.skipStory)
			return;
		showLabel.text = str;
		TweenAlpha ta1 = TweenAlpha.Begin(showLabel.gameObject,0.5f,1);
		ta1.from = 0;
		EventDelegate.Add (ta1.onFinished, () => {
			StartCoroutine (Utils.DelayRun (() => {
				TweenAlpha ta2 = TweenAlpha.Begin(showLabel.gameObject,2f,0);
				ta2.from = 1;
				EventDelegate.Add (ta2.onFinished, () => {
					if(callback != null)
						callback();
				},true);
			}, 0.2f));

		},true);
	}

	public void gotoNext () {
		if (GameManager.Instance.skipStory)
			return;
		MaskWindow.LockUI ();
		skipStoryButton.SetActive (false);
		GameManager.Instance.skipStory = true;
		if (UiManager.Instance.getWindow<TalkWindow> () != null ) {
			UiManager.Instance.getWindow<TalkWindow> ().destoryWindow();
		}
		UiManager.Instance.switchWindow<ChooseHoroscopesWindow>();
	}
}