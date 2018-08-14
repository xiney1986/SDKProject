using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//献祭面板中旋转木马上的木马控制器
//李程
public class SacrificeShowerCtrl : ButtonBase
{

    /** 这个容器上搭载的卡片 */
	public Card  card;
    /** 这个容器上搭载的万能卡 */
    public Prop  prop;
	public UITexture cardImage;
	public UITexture background;
	public UISprite qualityBg;
	public UISprite jobBg;
	public UILabel job;
	public UILabel level;
	public UILabel evoLevel;
	public UISprite bian;
	public UISprite jobtext;
	public UISprite inTeamSprite;
    public GameObject starsPrefab;

	int  orgCardImageDepth = 10;
	int  backgroundDepth = 0;

	//获取卡片唯一引索
	public string getCardUid ()
	{
		return (card == null) ? "0" : card.uid;
	}

	public Card getCard ()
	{
		return card;
	}

    public Prop getProp()
    {
        return prop;
    }

	public void changeColorByDepth (int offset)
	{
		float dir = ((float)(offset) / 237) * 0.3f + 0.7f;
		cardImage.color = new Color (dir, dir, dir, 1);

	}
	//public int lastdDepth;
	public void changeDepth (int offset)
	{
		//	-238----------0-------------  238
		cardImage.depth = orgCardImageDepth + offset;
		if (background != null) {
			background.depth = backgroundDepth + offset;
		}
	}
	
	public void updateShower (Card newCard)
	{
		gameObject.SetActive (true);
		//动态加载碰撞器
		Object collider = this.gameObject.GetComponent ("BoxCollider");
		if (this.name != "cardmain" && collider == null) {
			this.gameObject.AddComponent ("BoxCollider");
			this.gameObject.GetComponent<BoxCollider> ().isTrigger = true;
			this.gameObject.GetComponent<BoxCollider> ().center = new Vector3 (0, -15, -20);
			this.gameObject.GetComponent<BoxCollider> ().size = new Vector3 (140, 190, 1);
		}
		card = newCard;
        showStar(card);
        prop = null;
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + card.getImageID (), cardImage);
		if (background != null) {
			background.gameObject.SetActive (true);
		}
		if (qualityBg != null) {
			qualityBg.gameObject.SetActive (true);
			qualityBg.spriteName = QualityManagerment.qualityIDToBackGround (card.getQualityId ());
		}
		if (job != null) {
			job.text = CardManagerment.Instance.jobIDToString (card.getJob ());
			job.gameObject.SetActive (true);
		}
		if(bian!=null){
			bian.spriteName=QualityManagerment.qualityBianToBackGround(card.getQualityId());
			bian.gameObject.SetActive(true);
		}
		if(jobtext!=null){
			jobtext.spriteName=CardManagerment.Instance.qualityIconTextToBackGround(card.getJob());
			jobtext.gameObject.SetActive(true);
		}
		if(jobBg!=null){
			jobBg.spriteName=QualityManagerment.qualityIconBgToBackGround(card.getQualityId());
			jobBg.gameObject.SetActive(true);
		}
		if (level != null) {
			level.text = "Lv." + card.getLevel ();
			level.gameObject.SetActive (true);
		}
		if (inTeamSprite != null) {
			inTeamSprite.gameObject.SetActive (card.isInTeam ());
		}
//		if (quality != null) {
//			quality.spriteName = QualityManagerment.qualityIDToString (card.getQualityId ());
//			quality.gameObject.SetActive (true);
//		}
		updateEvoLevel ();
	}

	public void updateShowerByProp (Prop prop)
	{     
        cleanData();
        this.prop = prop;
		gameObject.SetActive (true);
		if(evoLevel!=null)evoLevel.gameObject.SetActive(false);
		//动态加载碰撞器
		Object collider = this.gameObject.GetComponent ("BoxCollider");
		if (this.name != "cardmain" && collider == null) {
			this.gameObject.AddComponent ("BoxCollider");
			this.gameObject.GetComponent<BoxCollider> ().isTrigger = true;
			this.gameObject.GetComponent<BoxCollider> ().center = new Vector3 (0, 20, -20);
			this.gameObject.GetComponent<BoxCollider> ().size = new Vector3 (120, 180, 1);
		}
		card = null;
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + prop.getIconId (), cardImage);
		if (background != null) {
			background.gameObject.SetActive (true);
		}
	}
    /// <summary>
    /// 显示卡片的星级
    /// </summary>
    public void showStar(Card card) {
        if (starsPrefab != null) {
            if (this.gameObject.transform.FindChild("starContent(Clone)") != null) {
                DestroyImmediate(this.gameObject.transform.FindChild("starContent(Clone)").gameObject);
            }
            if (card != null && this.gameObject.transform.FindChild("starContent(Clone)") == null && CardSampleManager.Instance.getStarLevel(card.sid) > 0) {
                GameObject star = NGUITools.AddChild(this.gameObject, starsPrefab);
                ShowStars show = star.GetComponent<ShowStars>();
                show.initStar(CardSampleManager.Instance.getStarLevel(card.sid), CardSampleManager.USEDBYSHOW);
            }
        }
    }
	private void updateEvoLevel ()
	{
		if (evoLevel != null && card != null) {
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
		}else{
			if(evoLevel!=null)evoLevel.gameObject.SetActive(false);
		}
	}

	public override void DoUpdate ()
	{
		if (evoLevel != null && evoLevel.gameObject != null && evoLevel.gameObject.activeSelf)
			evoLevel.alpha = sin ();
	}

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		updateCardInfo ();
		MaskWindow.UnlockUI ();
	}

	//这里处理点击卡片后食物卡一起隐藏
	public void updateCardInfo ()
	{
        /** 如果点击的是主卡,将主卡,副卡,全部清理掉 */
		if (IntensifyCardManager.Instance.compareMainCard (card)) {
			IntensifyCardManager.Instance.removeMainCard ();
			cleanInvalidCard ();
			int intensifyType = (fatherWindow  as IntensifyCardWindow).getIntensifyType ();
            if (intensifyType == IntensifyCardManager.INTENSIFY_CARD_EVOLUTION || intensifyType == IntensifyCardManager.INTENSIFY_CARD_SUPRE_EVO){
                IntensifyCardManager.Instance.clearFood ();
			}   
			cleanData ();
		} 
        /** 副卡 */
        else{
		
            if(card != null){
				IntensifyCardManager.Instance.removeFoodCard (card);   
			}
            if(prop!= null)
                IntensifyCardManager.Instance.removeFoodProp(prop);
			cleanAll ();
		}	
		(fatherWindow as IntensifyCardWindow).updateInfo ();
	}

	/** 清理无效的卡片 */
	private void cleanInvalidCard ()
	{
		List<Card> foodCard = IntensifyCardManager.Instance.getFoodCard ();
		if (foodCard != null) {
			foreach (Card card in foodCard) {
				if (StorageManagerment.Instance.getRole (card.uid) == null) {
					cleanAll ();
				}
			}
		}
	}

	//清理所有
	public void cleanAll ()
	{
		card = null;
        prop = null;
		cardImage.gameObject.SetActive (false);
//		Object collider = this.gameObject.GetComponent ("BoxCollider");
//		if (this.name != "cardmain" && collider != null) {
//			Destroy (collider);
//		}
		if (background != null) {
			background.gameObject.SetActive (false);
		}
		if (qualityBg != null) {
			qualityBg.gameObject.SetActive (false);
		}
		if (job != null) {
			job.gameObject.SetActive (false);
		}if(bian!=null){
			bian.gameObject.SetActive(false);
		}
		if(jobtext!=null){
			jobtext.gameObject.SetActive(false);
		}
		if(jobBg!=null){
			jobBg.gameObject.SetActive(false);
		}
		if (level != null) {
			level.gameObject.SetActive (false);
		}
//		if (quality != null) {
//			quality.gameObject.SetActive (false);
//		}
		if (evoLevel != null) {
			evoLevel.gameObject.SetActive (false);
		}
		gameObject.SetActive (false);
	}
	//清理底座上的人物
	public void cleanData ()
	{
        if (this.gameObject.transform.FindChild("starContent(Clone)") != null) {
            DestroyImmediate(this.gameObject.transform.FindChild("starContent(Clone)").gameObject);
        }
		Object collider = this.gameObject.GetComponent ("BoxCollider");
		if (this.name != "cardmain" && collider != null) {
			(collider as BoxCollider).isTrigger = false;
		}
		gameObject.SetActive (true);
		card = null;
        prop = null;
		cardImage.gameObject.SetActive (false);
		if (background != null) {
			background.gameObject.SetActive (false);
		}
		if (qualityBg != null) {
			qualityBg.gameObject.SetActive (false);
		}
		if (job != null) {
			job.gameObject.SetActive (false);
		}
		if(bian!=null){
			bian.gameObject.SetActive(false);
		}
		if(jobtext!=null){
			jobtext.gameObject.SetActive(false);
		}
		if(jobBg!=null){
			jobBg.gameObject.SetActive(false);
		}
		if (level != null) {
			level.gameObject.SetActive (false);
		}
//		if (quality != null) {
//			quality.gameObject.SetActive (false);
//		}
		if (evoLevel != null) {
			evoLevel.gameObject.SetActive (false);
		}
        if (inTeamSprite != null){
            inTeamSprite.gameObject.SetActive(false);
        }
	}	

 

}
