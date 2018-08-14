using UnityEngine;
using System.Collections;

public class PropLineCtrl : MonoBehaviour {

	public UILabel msgText;
	public UISprite backGround;
	public UISprite iconBg;
	public UITexture icon;
	public PrizeSample prize;
	public int targetY;
	PropMessageLineWindow fatherWindow;
	// Use this for initialization

	public void Initialize(PropMessageLineWindow father, int y,PrizeSample prize){
		targetY = y;
		fatherWindow = father;
		this.prize = prize;
        iconBg.gameObject.SetActive(false);
        icon.mainTexture = null;
        if (prize == null) return;
        if (prize.type == PrizeType.PRIZE_STARSOUL) {
			iconBg.gameObject.SetActive(true);
			iconBg.spriteName = "iconback_3";
            StarSoul starsoulView = StarSoulManager.Instance.createStarSoul(prize.pSid);
            msgText.text = starsoulView.getName();
            ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.STARSOUL_ICONPREFAB_PATH + starsoulView.getIconId(), icon.transform, (obj) => {
                GameObject gameObj = obj as GameObject;
                if (gameObj != null) {
                    Transform childTrans = gameObj.transform;
                    if (childTrans != null) {
                        StarSoulEffectCtrl effectCtrl = childTrans.gameObject.GetComponent<StarSoulEffectCtrl>();
                        effectCtrl.setColor(starsoulView.getQualityId());
                    }
                }
            });
        }else if (prize.type==-1)
        {
            iconBg.gameObject.SetActive(false);
            msgText.text = prize.prizeDec;
            return;
        } 
        else {
            iconBg.gameObject.SetActive(true);
            iconBg.spriteName = QualityManagerment.qualityIDToIconSpriteName(prize.getQuality());
            ResourcesManager.Instance.LoadAssetBundleTexture(prize.getIconPath(), icon);
            msgText.text = prize.getPrizeName();
        }
        //msgText.text =QualityManagerment.getQualityColor( prize.getQuality ()) +prize.getPrizeName ();
       
		msgText.text += " x " + prize.getPrizeNumByInt();
	}
	void Start () {
		iTween.ValueTo (gameObject, iTween.Hash ("from", 0, "to", 1, "onupdate", "changeAlpha", "oncomplete", "", "easetype", iTween.EaseType.linear, "time", 0.05f));
		iTween.ValueTo (gameObject, iTween.Hash ("from", new Vector3 (0, -600, 0), "to", new Vector3 (0, targetY, 0), "onupdate", "changePos", "oncomplete", "", "", iTween.EaseType.easeInOutExpo, "time", 0.1f));	
		iTween.ValueTo (gameObject, iTween.Hash ( "delay",1.5f,"from", new Vector3 (0, targetY, 0), "to", new Vector3 (-600, targetY, 0), "onupdate", "changePos", "oncomplete", "over", "", iTween.EaseType.easeInOutExpo, "time", 0.1f));	
		iTween.ValueTo (gameObject, iTween.Hash ( "delay",1.5f,"from", 1, "to", 0, "onupdate", "changeAlpha", "easetype", iTween.EaseType.linear, "time", 0.1f));	
		iTween.ValueTo (gameObject, iTween.Hash ("from", 1, "to", 0, "onupdate", "", "oncomplete", "readNext", "easetype", iTween.EaseType.linear, "time", 0.1f));	
		
	}
	
	void readNext () {
		fatherWindow.readOne ();	
	}
	
	void over () {
		Destroy (gameObject);
//		fatherWindow.RemovedPrize();
	}
	
	void 	changePos (Vector3 pos) {
		transform.localPosition = pos;
	}
	
	void 	changeAlpha (float alpha) {
		msgText.alpha = alpha;
		backGround.alpha = alpha;
	}
}
