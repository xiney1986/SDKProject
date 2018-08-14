using UnityEngine;
using System.Collections;

public class WarChooseButton : ButtonBase
{

	public UISprite sign;
	public UILabel levelLimit;
	public UILabel missionName;
    public GameObject prizePoint;
    public GameObject prizePrefab;
	public GameObject effectPrefab;
	//public Mission mission;
	Mission mission;
    PrizeSample[] prizes;

	public void updateButton (Mission mis)
	{
		mission = mis;
		textLabel.text = mission.getMissionName ();
        changeSign(mission.getMissionType());
        prizes = MissionSampleManager.Instance.getMissionSampleBySid(mission.sid).prizes;
        showPrizes();
	}
	 
	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		GuideManager.Instance.doGuide();
//		fatherWindow.hideWindow (); 
//		UiManager.Instance.openWindow<BossViewWindow>((win) => {
//			win.Initialize (mission, (fatherWindow as WarChooseWindow).content.missonList);
//		});
		UiManager.Instance.openWindow<TeamPrepareWindow>((win) => {
			win.Initialize (mission,TeamPrepareWindow.WIN_BOSS_ITEM_TYPE, (fatherWindow as WarChooseWindow).content.missonList);
		});
	}
    /// <summary>
    /// ÏÔÊ¾½±Àø£¨½ö½ö¿¨Æ¬ËéÆ¬£©
    /// </summary>
    public void showPrizes() {
        if (prizes != null) {
            if (prizePoint.transform.FindChild("goodsView(Clone)") != null) {
                DestroyImmediate(prizePoint.transform.FindChild("goodsView(Clone)").gameObject);
            }
            for (int i = 0; i < prizes.Length; i++) {
                if (prizes[i].type == PrizeType.PRIZE_PROP) {
                    Prop prop = PropManagerment.Instance.createProp(PropSampleManager.Instance.getPropSampleBySid(prizes[i].pSid).sid);
                    if (prop.isCardScrap() || prop.isRedOmnipotentCardOrScrap()) {
                        GameObject prizeItem = NGUITools.AddChild(prizePoint, prizePrefab);
                        prizeItem.transform.localScale = new Vector3(0.8f, 0.8f, 0);
                        GoodsView prize = prizeItem.GetComponent<GoodsView>();
                        prize.fatherWindow = fatherWindow;
                        prize.init(prizes[i]);
                    }
                }
            }
        }
    }
	public void changeSign (string spName)
	{
		if (spName == MissionShowType.NEW || spName == MissionShowType.COMPLET) {
			sign.gameObject.SetActive (true);
			sign.spriteName = spName;
			if(spName==MissionShowType.NEW)
				effectPrefab.gameObject.SetActive(true);
			if (spName == MissionShowType.NEW && fatherWindow is WarChooseWindow) {
				(fatherWindow as WarChooseWindow).newWar = this;
			}
		} else {
			sign.gameObject.SetActive (false);
		}
		textLabel.gameObject.SetActive(true);
		textLabel.text = mission.getMissionName () ;
        //showPrizes();
		if (spName == MissionShowType.LEVEL_LOW) {
			levelLimit.gameObject.SetActive (true);
			textLabel.gameObject.SetActive(false);
			effectPrefab.gameObject.SetActive(false);
			levelLimit.text = Colors.RED+textLabel.text + "      Lv." + mission.getRequirLevel () + LanguageConfigManager.Instance.getLanguage ("s0160");
			//levelLimit.transform.localPosition=new Vector3(textLabel.transform.localPosition.x,levelLimit.transform.localPosition.y,levelLimit.transform.localPosition.z);
			levelLimit.transform.localScale=new Vector3(1.2f,1.2f,1);
		}
		else 
		{
			if (mission.getMissionType () == MissionShowType.NOT_COMPLETE_LAST_MISSION) {
				textLabel.gameObject.SetActive(true);
				levelLimit.gameObject.SetActive (false);
				textLabel.text += "      "+Colors.RED + LanguageConfigManager.Instance.getLanguage ("s0175"); 
			}else{
				//	textLabel.gameObject.SetActive (false);
				levelLimit.gameObject.SetActive (false);
			}
		}

	}

}
