using UnityEngine;
using System.Collections;

public class TitlesHoroscopesWinow : WindowBase
{

	public UILabel showLabel;
	public UITexture icon;
	private int horSid;

	protected override void begin ()
	{
		base.begin ();

		if (GameManager.Instance.skipGuide)
			showTalkWindowCallBack ();
		else
			StartCoroutine(showUI());
		MaskWindow.UnlockUI ();
	}

	protected override void DoEnable ()
	{
		base.DoEnable ();

		icon.alpha = 0;
	}

	public void init (int _horSid)
	{
		horSid = _horSid;
	}

	IEnumerator showUI ()
	{
		yield return new WaitForSeconds (0.1f);
		Horoscopes info = HoroscopesManager.Instance.getStarByType (horSid);
		icon.alpha = 0;
        if(CommandConfigManager.Instance.getNvShenClothType() == 0)
		    ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + info.getImageID()+  "c", icon);
        else ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.CARDIMAGEPATH + info.getImageID(), icon);
		iTween.ShakePosition (this.gameObject, iTween.Hash ("amount", new Vector3 (0.03f, 0.03f, 0.03f), "time", 0.4f));
		iTween.ShakePosition (this.gameObject, iTween.Hash ("amount", new Vector3 (0.01f, 0.01f, 0.01f), "time", 0.4f));

		yield return new WaitForSeconds (0.1f);
		EffectManager.Instance.CreateEffectCtrlByCache(icon.transform,"Effect/UiEffect/GoddessAppears",(passobj,effectCtrl)=>{
			passobj.obj.transform.parent = icon.transform;
			passobj.obj.transform.localScale = Vector3.one;
			passobj.obj.transform.localPosition = new Vector3 (0, 0, -600);
		});
		yield return new WaitForSeconds (0.5f);
		TweenAlpha ta1 = TweenAlpha.Begin (icon.gameObject, 2f, 1);
		ta1.from = 0;
		EventDelegate.Add (ta1.onFinished, () => {
//			int audioId = 400 + horSid;
			if(GameManager.Instance.skipStory){
				showTalkWindowCallBack();
				return ;
			}
				
			AudioManager.Instance.PlayAudio (601);
			labelEffect (LanguageConfigManager.Instance.getLanguage ("GuideTitles05", info.getName ()), () => {
				initWalk ();
			});
		}, true);
	}

	void initWalk ()
	{
		UiManager.Instance.openDialogWindow<TalkWindow>((win)=>{
			win.init (15002, showTalkWindowCallBack, null);
		});
	}

	void labelEffect (string str, CallBack callback)
	{
		showLabel.text = str;
		TweenAlpha ta1 = TweenAlpha.Begin (showLabel.gameObject, 1f, 1);
		ta1.from = 0;
		EventDelegate.Add (ta1.onFinished, () => {
			StartCoroutine (Utils.DelayRun (() => {
				TweenAlpha ta2 = TweenAlpha.Begin (showLabel.gameObject, 2f, 0);
				ta2.from = 1;
				EventDelegate.Add (ta2.onFinished, () => {
					if (callback != null)
						callback ();
				}, true);
			}, 0.2f));
		}, true);
	}

	void showTalkWindowCallBack ()
    {
        UiManager.Instance.switchWindow<RoleNameWindow>((win)=>{
            win.init(horSid);
        });
	//	finishWindow();
	}
}
