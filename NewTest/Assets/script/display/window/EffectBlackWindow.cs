using UnityEngine;
using System.Collections;

public class EffectBlackWindow : WindowBase
{
	public Transform effectPoint;
	public UITexture DrawCard;
	/** 品质图标 */
	public UISprite qualitySprite;
	public PvpComboBarCtrl pvpComboBar;
    private Card mainCard;
	public UITexture bg;

	/** 隐藏进化组件 */
	public void hideEvoComponent()
	{
		effectPoint.gameObject.SetActive (false);
		qualitySprite.gameObject.SetActive (false);
		bg.gameObject.SetActive(false);
	}
    public void playLearnSkillEffect(Card main, Card sub, CallBack callback) {
        EffectManager.Instance.CreateEffect(effectPoint, "Effect/UiEffect/Role_evolution");
        EvoEffectCtrl ctrl = EffectManager.Instance.CreateEffect(effectPoint, "Effect/UiEffect/Role_evolution2") as EvoEffectCtrl;
        StartCoroutine(playLearnSkillEffect(ctrl, main, sub, callback));
    }

    public void playBloodEffect(Card main,CallBack callBack)
    {
        EffectManager.Instance.CreateEffect(effectPoint, "Effect/UiEffect/Role_evolution");
        EvoEffectCtrl ctrl = EffectManager.Instance.CreateEffect(effectPoint, "Effect/UiEffect/Role_evolution2") as EvoEffectCtrl;
        StartCoroutine(playBloodEffect(ctrl, main, callBack, ResourcesManager.CARDIMAGEPATH, ResourcesManager.CARDIMAGEPATH));
    }
	public void playEvoEffect (Card main, int propIconId, CallBack callback)
	{
		EffectManager.Instance.CreateEffect (effectPoint, "Effect/UiEffect/Role_evolution");
		EvoEffectCtrl ctrl = EffectManager.Instance.CreateEffect (effectPoint, "Effect/UiEffect/Role_evolution2") as EvoEffectCtrl;
		StartCoroutine (playEvoEffect(ctrl,main, propIconId, callback,ResourcesManager.CARDIMAGEPATH,ResourcesManager.ICONIMAGEPATH));
	}
	public void playEvoEffect (Card main, Card sub, CallBack callback)
	{
		EffectManager.Instance.CreateEffect (effectPoint, "Effect/UiEffect/Role_evolution");
		EvoEffectCtrl ctrl = EffectManager.Instance.CreateEffect (effectPoint, "Effect/UiEffect/Role_evolution2") as EvoEffectCtrl;
		StartCoroutine (playEvoEffect(ctrl,main, sub.getImageID (), callback,ResourcesManager.CARDIMAGEPATH,ResourcesManager.CARDIMAGEPATH));
	}
	public IEnumerator playEvoEffect(EvoEffectCtrl ctrl,Card main, int iconId, CallBack callback,string imagePath1,string imagePath2)
	{
		Card newCard = StorageManagerment.Instance.getRole(main.uid);
		ResourcesManager.Instance.LoadAssetBundleTexture (imagePath1 + main.getImageID () , ctrl.main,(obj)=>{
			ctrl.main.alpha=0;
		});
		ResourcesManager.Instance.LoadAssetBundleTexture (imagePath2 + iconId, ctrl.after,(obj)=>{
			ctrl.sub.alpha=0;
		});
		ResourcesManager.Instance.LoadAssetBundleTexture (imagePath1 + newCard.getImageID(), ctrl.sub,(obj)=>{
			ctrl.after.alpha=0;
		});
		yield return new WaitForSeconds(1.5f);
        if (main.isInSuperEvo())
        {
			EffectManager.Instance.CreateEffectCtrlByCache(effectPoint,"Effect/UiEffect/EvolutionarySuccess_super",null);
		} else {
			EffectManager.Instance.CreateEffectCtrlByCache(effectPoint,"Effect/UiEffect/EvolutionarySuccess",null);
		}
		yield return new WaitForSeconds(1.5f);
		qualitySprite.spriteName=QualityManagerment.qualityIDToString (newCard.getQualityId ()) + "Bg";
		qualitySprite.gameObject.SetActive (true);
	
		TweenScale ts = TweenScale.Begin (qualitySprite.gameObject, 0.2f, Vector3.one);
		ts.method = UITweener.Method.EaseIn;
		ts.from = new Vector3 (5, 5, 1);
		EventDelegate.Add (ts.onFinished, () =>
		 {
			iTween.ShakePosition (qualitySprite.gameObject, iTween.Hash ("amount", new Vector3 (0.03f, 0.03f, 0.03f), "time", 0.4f));
			iTween.ShakePosition (qualitySprite.gameObject, iTween.Hash ("amount", new Vector3 (0.01f, 0.01f, 0.01f), "time", 0.4f));
		}, true);
		bg.gameObject.SetActive(true);
		iTween.MoveTo(ctrl.gameObject, iTween.Hash("position", new Vector3(0.0f, 150.0f, 0.0f), "time", 0.5f, "easetype", "linear","islocal",true));
		yield return new WaitForSeconds(1f);
		if (newCard.getEvoLevel () > main.getEvoLevel ()) {
			UiManager.Instance.openDialogWindow<CardAttrLevelInfo>((win)=>{
				win.Initialize (main,newCard,callback); 
			});
		} else {
			yield return new WaitForSeconds(1.5f);
			hideEvoComponent ();
			finishWindow ();
			callback ();
		}
	}
    public IEnumerator playBloodEffect(EvoEffectCtrl ctrl, Card main, CallBack callback, string imagePath1, string imagePath2) {
        Card newCard = StorageManagerment.Instance.getRole(main.uid);
        ResourcesManager.Instance.LoadAssetBundleTexture(imagePath1 + main.getImageID(), ctrl.main, (obj) => {
            ctrl.main.alpha = 0;
        });
        ResourcesManager.Instance.LoadAssetBundleTexture(imagePath2 + main.getImageID(), ctrl.after, (obj) => {
            ctrl.sub.alpha = 0;
        });
        //ResourcesManager.Instance.LoadAssetBundleTexture(imagePath1 + BloodConfigManager.Instance.getCurrentBloodImage(newCard.sid,newCard.cardBloodLevel), ctrl.sub, (obj) => {
        //    ctrl.after.alpha = 0;
        //});
        ResourcesManager.Instance.LoadAssetBundleTexture(imagePath2 + main.getImageID(), ctrl.sub, (obj) => {
            ctrl.after.alpha = 0;
        });
        yield return new WaitForSeconds(1.5f);
        if (main.isInSuperEvo()) {
            EffectManager.Instance.CreateEffectCtrlByCache(effectPoint, "Effect/UiEffect/EvolutionarySuccess_super", null);
        } else {
            EffectManager.Instance.CreateEffectCtrlByCache(effectPoint, "Effect/UiEffect/EvolutionarySuccess", null);
        }
        yield return new WaitForSeconds(1.5f);
        qualitySprite.spriteName = QualityManagerment.qualityIDToString(newCard.getQualityId()) + "Bg";
        qualitySprite.gameObject.SetActive(true);

        TweenScale ts = TweenScale.Begin(qualitySprite.gameObject, 0.2f, Vector3.one);
        ts.method = UITweener.Method.EaseIn;
        ts.from = new Vector3(5, 5, 1);
        EventDelegate.Add(ts.onFinished, () => {
             iTween.ShakePosition(qualitySprite.gameObject, iTween.Hash("amount", new Vector3(0.03f, 0.03f, 0.03f), "time", 0.4f));
             iTween.ShakePosition(qualitySprite.gameObject, iTween.Hash("amount", new Vector3(0.01f, 0.01f, 0.01f), "time", 0.4f));
         }, true);
        bg.gameObject.SetActive(true);
        iTween.MoveTo(ctrl.gameObject, iTween.Hash("position", new Vector3(0.0f, 150.0f, 0.0f), "time", 0.5f, "easetype", "linear", "islocal", true));
        yield return new WaitForSeconds(1f);
        if (newCard.getEvoLevel() > main.getEvoLevel()) {
            UiManager.Instance.openDialogWindow<CardAttrLevelInfo>((win) => {
                win.Initialize(main, newCard, callback);
            });
        } else {
            yield return new WaitForSeconds(1.5f);
            hideEvoComponent();
            finishWindow();
            callback();
        }
    }
    
	public IEnumerator playLearnSkillEffect(EvoEffectCtrl ctrl,Card main, Card sub, CallBack callback)
	{
		ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.CARDIMAGEPATH + main.getImageID() ,ctrl.main,(obj)=>{
			ctrl.main.alpha=0;
		});
		ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.CARDIMAGEPATH + sub.getImageID() ,ctrl.after,(obj)=>{
			ctrl.sub.alpha=0;
		});
		ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.CARDIMAGEPATH + main.getImageID(),ctrl.sub,(obj)=>{
			ctrl.after.alpha=0;
		});
		yield return new WaitForSeconds(6);
		if (callback != null)
			callback();
	}

}
