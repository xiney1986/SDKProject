using UnityEngine;
using System.Collections;

public class TeamPrepareCardCtrl : ButtonBase
{
	public UITexture image;
	public UISprite jobTextSprite;
	public UISprite jobSprite;
	public UISprite jobBian;
    public UISprite spriteBgQuality;
	public UILabel level;
	public UILabel evoLevel;
	private string _role_uid;
	private string _card_sid;
	private CallBack callback;
	public Card card;

	public void initInfo(string role_uid,string card_uid,CallBack callback)
	{
		this._role_uid = role_uid;
		this._card_sid = card_uid;
		this.callback = callback;
	}
	public void updateButton(PvpOppTeam info)
	{
		card = null;
		CardSample cs = CardSampleManager.Instance.getRoleSampleBySid(info.sid);
		level.text ="Lv."+ EXPSampleManager.Instance.getLevel (cs.levelId,info.exp, 0).ToString();

        card = CardManagerment.Instance.createCard(info.sid, info.evoLevel, info.surLevel);
		image.gameObject.SetActive (true);
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + card.getImageID (), image);
		int qualityid = card.getQualityId();
		spriteBgQuality.spriteName = QualityManagerment.qualityIDToBackGround (qualityid);

		jobSprite.spriteName=QualityManagerment.qualityIconBgToBackGround(card.getQualityId());
		jobTextSprite.spriteName=CardManagerment.Instance.qualityIconTextToBackGround(card.getJob());
		jobBian.spriteName=QualityManagerment.qualityBianToBackGround(card.getQualityId());

		this.setNormalSprite( QualityManagerment.qualityIDToBackGround(qualityid));

		updateEvoLv();
	}

	public void updateEvoLv()
	{
		if (evoLevel != null&&card!=null) {
			if (card.getEvoLevel () > 0) {				
				if(card.isMainCard()){
					if(card.getSurLevel() > 0){
						evoLevel.gameObject.SetActive(true);
						evoLevel.text = "[FF0000]+" + card.getSurLevel().ToString();
					}
					else
						evoLevel.gameObject.SetActive(false);
				}
				else{
					evoLevel.gameObject.SetActive (true);
					evoLevel.text = "[FF0000]+" + card.getEvoLevel ();
				}
			} else
				evoLevel.gameObject.SetActive (false);
		}
	}

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		//新手引导不能点
		if (GuideManager.Instance.isLessThanStep (GuideGlobal.NEWOVERSID)) {
			MaskWindow.UnlockUI();
			return;
		}
		GetPlayerCardInfoFPort fport = FPortManager.Instance.getFPort("GetPlayerCardInfoFPort") as GetPlayerCardInfoFPort;
		fport.getCard(_role_uid,_card_sid,null);
	}

	public override void DoUpdate ()
	{
		if (evoLevel != null && evoLevel.gameObject != null && evoLevel.gameObject.activeSelf)
			evoLevel.alpha = sin ();
	}
}
