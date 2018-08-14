using UnityEngine;
using System.Collections.Generic;

public class GoddessAstrolabeMainWindow : WindowBase {

	/** 星云按钮 */
	public ButtonBase[] nebulaArray;
	/** 主线数目 */
	public UILabel[] mainStarCount;
	/** 支线数目 */
	public UILabel[] extensionStarCount;
	/** 激活 */
	public GameObject[] starCountObj;
	/** 未激活 */
	public GameObject[] noStarCountObj;
	/** 未激活 */
	public GameObject[] starEffect;
	/** 光线背景 */
	public UITexture lightTexture;
	//  星屑商店按钮//
	public ButtonBase buttonShop;

	/* methods */
	public override void OnAwake () {
		base.OnAwake ();
	}

	protected override void DoEnable () {
		base.DoEnable ();
		UiManager.Instance.backGround.switchBackGround ("ChouJiang_BeiJing");
	}

	protected override void begin () {
		initUI ();
		GuideManager.Instance.guideEvent ();
		MaskWindow.UnlockUI ();
	}

	private void initUI () {
		GoddessAstrolabeManagerment instance = GoddessAstrolabeManagerment.Instance;
		List<GoddessAstrolabeSample> newInfo;//指定星星集合
		int mainStar = 0;//所有主线
		int mainOpenStar = 0;//激活的主线
		lightTexture.fillAmount = 0;
		int amount = 0;
		for (int i = 0; i < nebulaArray.Length; i++) {
			newInfo = instance.getStarByNebulaId (i + 1);
			if (newInfo != null && instance.isHaveOpenStarByNebulaId (i + 1)) {
				amount++;
				mainStar = instance.getMainStarNUmByNebulaId (i + 1);
				starEffect[i].SetActive(true);
				starCountObj [i].SetActive(true);
				noStarCountObj [i].SetActive(false);
				mainOpenStar = instance.getOpenMainStarNumByNebulaId (i + 1);
				mainStarCount [i].text = LanguageConfigManager.Instance.getLanguage ("goddess12") + mainOpenStar + "/" + mainStar;
				// 最后一个被激活则显示商店按钮//
				if(i+1 == nebulaArray.Length && mainOpenStar == mainStar)
					buttonShop.gameObject.SetActive(true);
				extensionStarCount [i].text = LanguageConfigManager.Instance.getLanguage ("goddess13") + (instance.getOpenStarNumByNebulaId (i + 1) - mainOpenStar) + "/" + (newInfo.Count - mainStar);
			} else {
				starEffect[i].SetActive(false);
				starCountObj [i].SetActive(false);
				noStarCountObj [i].SetActive(true);
			}
		}
		amount = Mathf.Max (0,amount - 1);
		lightTexture.fillAmount = amount * 1.0f / nebulaArray.Length;
	}

	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);

		if (gameObj.name == "buttonRecommend") {
			UiManager.Instance.openWindow<GoddessAstrolabeInfoWindow> ((win) => {
				win.initUI ();
			});
		}
		else if (gameObj.name == "close") {
			finishWindow ();
		}
		else if (gameObj.name == "Nebula1") {
			openWin (1);
		}
		else if (gameObj.name == "Nebula2") {
			openWin (2);
		}
		else if (gameObj.name == "Nebula3") {
			openWin (3);
		}
		else if (gameObj.name == "Nebula4") {
			openWin (4);
		}
		else if (gameObj.name == "Nebula5") {
			openWin (5);
        } else if (gameObj.name == "Nebula6")
		{
		    openWin(6);
		}
		// 打开星屑商店//
		else if(gameObj.name == "buttonShop")
		{
			UiManager.Instance.openWindow<StarShopWindow>();
		}
	}

	private void openWin (int id) {
		if (GoddessAstrolabeManagerment.Instance.getStarByNebulaId (id) == null)
			return;

		GuideManager.Instance.doGuide ();
		UiManager.Instance.switchWindow<GoddessAstrolabeWindow> ((win) => {
			win.initUI (GoddessAstrolabeManagerment.Instance.getStarByNebulaId (id), id);
		});
	}
}
