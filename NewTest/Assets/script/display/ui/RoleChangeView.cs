using UnityEngine;
using System.Collections;

public class RoleChangeView : MonoBehaviour {
	public UISprite inbattle;
	public UITexture icon;
	public UISprite qualityBg;
	public UISprite quality;
	public GameObject jobBg;
	public GameObject hpBg;
	public UILabel job;
	public UILabel level;
	public bool hideInBattle = true;
	
	//这4个字段RoleView本身并未使用,可用于关联
	public GameObject tempGameObj;
	public GameObject[] tempGameObjs;
	public object tempObj;
	public object[] tempObjs;
	
	public Card card;
	public CardSample sample;
	public PrizeSample prize;
	public CallBack<RoleChangeView> onClickCallback;
	public WindowBase fatherWindow;
	
	
	public int showType = CardBookWindow.SHOW;
	
	public void init(Card card,WindowBase fatherWindow,CallBack<RoleChangeView> onClickCallback)
	{
		this.card = card;
		this.fatherWindow = fatherWindow;
		this.onClickCallback = onClickCallback;
		updateInfo ();
	}
	
	public void init(CardSample sample,WindowBase fatherWindow,CallBack<RoleChangeView> onClickCallback)
	{
		this.sample = sample;
		this.fatherWindow = fatherWindow;
		this.onClickCallback = onClickCallback;
		updateInfo ();
	}
	
	public void init(PrizeSample prize, CallBack<RoleChangeView> onClickCallback)
	{
		this.prize = prize;
		ResourcesManager.Instance.LoadAssetBundleTexture(prize.getIconPath(), icon);
		qualityBg.spriteName = QualityManagerment.qualityIDToBackGround(prize.getQuality());
	}
	
	/// <summary>
	/// 从仓库中取出卡片刷新显示
	/// </summary>
	public void updateCard()
	{
		if (card != null)
		{
			Card c = StorageManagerment.Instance.getRole(card.uid);
			if(c != null)
			{
				card = c;
				updateInfo();
			}
		}
	}
	
	public void updateInfo()
	{ 
		
		if (card != null) {
			inbattle.gameObject.SetActive (!hideInBattle && (ArmyManager.Instance.getAllArmyPlayersIds ().Contains (card.uid) || ArmyManager.Instance.getAllArmyAlternateIds ().Contains (card.uid)));
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + card.getImageID (), icon);
			quality.spriteName = QualityManagerment.qualityIDToString (card.getQualityId ());
			quality.gameObject.SetActive(true);
			qualityBg.spriteName = QualityManagerment.qualityIDToBackGround (card.getQualityId ());
			qualityBg.gameObject.SetActive(true);
						if(jobBg != null)
							jobBg.gameObject.SetActive(true);
						if(hpBg !=null )
							hpBg.gameObject.SetActive(true);
			job.text = CardManagerment.Instance.jobIDToString (card.getJob ());
			job.gameObject.SetActive(true);
			level.text = "Lv." + card.getLevel ();
			level.gameObject.SetActive(true);
		} else if (sample != null) {
			inbattle.gameObject.SetActive (false);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + sample.imageID , icon);
			quality.spriteName = QualityManagerment.qualityIDToString (sample.qualityId);
			qualityBg.spriteName = QualityManagerment.qualityIDToBackGround (sample.qualityId);
			job.text = CardManagerment.Instance.jobIDToString (sample.job);
			level.text = "Lv.1";
		}
	}
	
	void OnClick()
	{
		if (onClickCallback != null)
			onClickCallback (this);
		GuideManager.Instance.doGuide();
	} 
	
	public void DefaultClickEvent(RoleChangeView view)
	{
		if (card != null) {
			CardBookWindow.Show(card, showType, () => {
				if (fatherWindow)
					fatherWindow.restoreWindow ();
			});
			if (fatherWindow)
				fatherWindow.hideWindow ();
		}
	}
}
