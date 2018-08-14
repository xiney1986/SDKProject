using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResolveCardShowerCtrl : ButtonBase {
	
	public Card  card;
	public Equip equip;
    public MagicWeapon magicscrapt;
	public UITexture cardImage;
	public UITexture background;
	public UILabel showLevel;
	private string type;
	int  orgCardImageDepth = 10;
	int  backgroundDepth = 0;

	public void changeColorByDepth (int offset) {
		float dir = ((float)(offset) / 237) * 0.3f + 0.7f;
		cardImage.color = new Color (dir, dir, dir, 1);
		
	}
	public void changeDepth (int offset) {
		cardImage.depth = orgCardImageDepth + offset;
		showLevel.depth = cardImage.depth + 1;
		if (background != null) {
			background.depth = backgroundDepth + offset;
		}
	}
	
	public void updateShower (Card newCard) {
		type = "card";
		gameObject.SetActive (true);
		//动态加载碰撞器
		Object collider = this.gameObject.GetComponent ("BoxCollider");
		if (this.name != "cardmain" && collider == null) {
			this.gameObject.AddComponent ("BoxCollider");
			this.gameObject.GetComponent<BoxCollider> ().isTrigger = true;
			this.gameObject.GetComponent<BoxCollider> ().center = new Vector3 (0, 20, -20);
			this.gameObject.GetComponent<BoxCollider> ().size = new Vector3 (120, 180, 1);
		}
		card = newCard;
		cardImage.transform.localScale = new Vector3 (1.3f,1.3f,1.3f);
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + card.getImageID (), cardImage);
		if (background != null) {
			background.gameObject.SetActive (true);
		}
		if (card.getEvoLevel () > 0) {
			showLevel.gameObject.SetActive(true);
			showLevel.text = "+" + card.getEvoLevel();
		}
		else {
			showLevel.gameObject.SetActive(false);
		}
	}
	public void updateShower (Equip newEquip) {
		type = "equip";
		gameObject.SetActive (true);
		//动态加载碰撞器
		Object collider = this.gameObject.GetComponent ("BoxCollider");
		if (this.name != "cardmain" && collider == null) {
			this.gameObject.AddComponent ("BoxCollider");
			this.gameObject.GetComponent<BoxCollider> ().isTrigger = true;
			this.gameObject.GetComponent<BoxCollider> ().center = new Vector3 (0, 20, -20);
			this.gameObject.GetComponent<BoxCollider> ().size = new Vector3 (120, 180, 1);
		}
		equip = newEquip;
		cardImage.transform.localScale = new Vector3 (1f,1f,1f);
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + equip.getIconId (), cardImage);
		if (background != null) {
			background.gameObject.SetActive (true);
		}
		if (equip.equpStarState > 0) {
			showLevel.gameObject.SetActive (true);
			showLevel.text = "+" + equip.equpStarState;
		}
		else {
			showLevel.gameObject.SetActive(false);
		}
	}
    public void updateShower(MagicWeapon magic) {
        type = "magic";
        gameObject.SetActive(true);
        //动态加载碰撞器
        Object collider = this.gameObject.GetComponent("BoxCollider");
        if (this.name != "cardmain" && collider == null) {
            this.gameObject.AddComponent("BoxCollider");
            this.gameObject.GetComponent<BoxCollider>().isTrigger = true;
            this.gameObject.GetComponent<BoxCollider>().center = new Vector3(0, 20, -20);
            this.gameObject.GetComponent<BoxCollider>().size = new Vector3(120, 180, 1);
        }
        magicscrapt = magic;
        cardImage.transform.localScale = new Vector3(1f, 1f, 1f);
        ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.ICONIMAGEPATH + magicscrapt.getIconId(), cardImage);
       
        if (background != null) {
            background.gameObject.SetActive(true);
        }
         showLevel.gameObject.SetActive(false);
    }
	public override void DoClickEvent () {
		showLevel.gameObject.SetActive (false);
		base.DoClickEvent ();
		if (type == "card") {
			(fatherWindow as ResolveWindow).offSelectCard (card);
		}
		if (type == "equip") {
			(fatherWindow as ResolveWindow).offSelectEquip (equip);
		}
        if(type=="magic"){
            (fatherWindow as ResolveWindow).offSelectmagic(magicscrapt);
        }
		cleanAll ();
		MaskWindow.UnlockUI ();
	}
	
	//清理所有
	public void cleanAll () {
		card = null;
		equip = null;
        magicscrapt = null;
		cardImage.gameObject.SetActive (false);

		if (background != null) {
			background.gameObject.SetActive (false);
		}
		gameObject.SetActive (false);
	}
	//清理底座上的人物
	public void cleanData ()
	{
		Object collider = this.gameObject.GetComponent ("BoxCollider");
		if (this.name != "cardmain" && collider != null) {
			(collider as BoxCollider).isTrigger = false;
		}
		gameObject.SetActive (true);
		card = null;
		equip = null;
        magicscrapt = null;
		cardImage.gameObject.SetActive (false);
		if (background != null) {
			background.gameObject.SetActive (false);
		}
	}
	void Update ()
	{
		if (showLevel != null && showLevel.gameObject != null && showLevel.gameObject.activeSelf)
			showLevel.alpha = sin ();
	}
}
