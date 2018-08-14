using UnityEngine;
using System.Collections;

public class EvolutionChooseWindow : WindowBase {

	/** 万能卡按钮 */
	public ButtonBase universalCardButton;
	private CallBack callback;
	private CallBack<int> callbackI;

	protected override void begin ()
	{
		base.begin ();
		GuideManager.Instance.guideEvent();
		MaskWindow.UnlockUI ();
	}

	public void initWin(CallBack _callback,CallBack<int> _callbackI)
	{
		callback = _callback;
		callbackI = _callbackI;
		Card mianCard = IntensifyCardManager.Instance.getMainCard();
		if(mianCard==null)
			return;
		int sid = EvolutionManagerment.Instance.getUniversalCardSid (mianCard);
		if (sid == -1) {
			universalCardButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("evolutionChooseWindow_UniversalCard");
			return;
		} else {
			Prop tmpProp = PropManagerment.Instance.createProp (sid);
			universalCardButton.textLabel.text = tmpProp.getName ();
            if (tmpProp.sid == 71196 || tmpProp.sid == 71197 || tmpProp.sid == 71198 || tmpProp.sid == 71199 || tmpProp.sid == 71200) {
                universalCardButton.textLabel.fontSize = 20;
            }
		}
	}

	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);

		switch(gameObj.name)
		{
		case "close":
			finishWindow();
			break;
			
		case "button_SameCard":
			GuideManager.Instance.doGuide(); 
			callbackI(1);
			callback();
			finishWindow();
			break;
			
		case "button_UniversalCard":
			Card mianCard = IntensifyCardManager.Instance.getMainCard();
			if(mianCard==null)
				return;
			Prop props = EvolutionManagerment.Instance.getCardByQuilty(mianCard);
			if(props == null) {     
				UiManager.Instance.openDialogWindow<MessageWindow> ((MessageWindow win) => {
					win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("Evo15"), LanguageConfigManager.Instance.getLanguage ("s0093"), LanguageConfigManager.Instance.getLanguage ("Evo09"), gotoShop);
				});
				return;
			}
            if ((fatherWindow as IntensifyCardWindow).getIntensifyType() == IntensifyCardManager.INTENSIFY_CARD_EVOLUTION)
            {
                IntensifyCardLSEContent content = (fatherWindow as IntensifyCardWindow).learnSkillAndEvoContent;
                content.initEvoChooseByProp(props);
                callbackI(2);
                finishWindow();
            }
            else if ((fatherWindow as IntensifyCardWindow).getIntensifyType() == IntensifyCardManager.INTENSIFY_CARD_SUPRE_EVO)
            {
                /** 由于超进化会选择多张卡片,这里要判断数量是否足够 */
                if ( !IntensifyCardManager.Instance.checkPropCanChoose(props))
                {
                    MessageWindow.ShowConfirm(Language("Intensify27", props.getName()), null);
                    finishWindow();
                    return;
                }
                else
                {
                    IntensifyCardSuperEvoContent content = (fatherWindow as IntensifyCardWindow).superEvoContent;
                    content.chooseFoodProp(props);
                    callbackI(2);
                    finishWindow();
                }
            }

			break;
		}
	}

	private void gotoShop (MessageHandle msg)
	{
		if (msg.buttonID == MessageHandle.BUTTON_LEFT) {
			EventDelegate.Add (this.OnHide,()=>{
				UiManager.Instance.openWindow<ExChangeWindow> ();
			});
		}
		finishWindow ();
	}
}
