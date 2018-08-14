using UnityEngine;
using System.Collections;

public class flyItemCtrl : MonoBehaviour {
	float angel;
	BeastSummonWindow fatherWindow;
	BeastAttrWindow  fatherWindoww;
    BloodEvolutionWindow fatherwindowww;
    public UITexture itemImage;
	// Use this for initialization
	void Start () {
		
	}
	public void Initialize (Texture image,BeastSummonWindow fatherWindow)
	{
		itemImage.mainTexture=image;
		this.fatherWindow=fatherWindow;
		transform.localScale=Vector3.one;
		iTween.MoveTo ( gameObject, iTween.Hash ("position",   transform.position+new Vector3(0,0.3f,0), "easetype",  iTween.EaseType.easeInOutCubic, "time", 1f));	
		iTween.ScaleTo ( gameObject, iTween.Hash ("scale",  new Vector3(1.4f,1.4f,1.4f), "easetype", iTween.EaseType.easeInOutCubic, "time", 1f));		
		Vector3 pos=new Vector3(fatherWindow.beastEffectPoint .position.x+Random.Range(-0.2f,0.2f),fatherWindow.beastEffectPoint .position.y+Random.Range(0,0.4f),fatherWindow.beastEffectPoint .position.z);
		iTween.MoveTo ( gameObject, iTween.Hash ("delay",1.1f,"position",pos, "easetype", "easeInQuad", "time", 0.2f));
		iTween.ScaleTo ( gameObject, iTween.Hash ("delay",1.1f,"scale",  new Vector3(0.2f,0.2f,0.2f), "easetype","easeInQuad", "oncomplete","over", "time", 0.2f));	
		EffectManager.Instance.CreateEffect(transform, "Effect/UiEffect/SummonBeast2");
	}

    public void Initialize(int  imageSid, BloodEvolutionWindow fatherWindow)
    {
        SkillSample sk = SkillSampleManager.Instance.getSkillSampleBySid(imageSid);
        if (sk != null) {
            ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.SKILLIMAGEPATH + sk.iconId, itemImage);//如果实在没有图标就只有显示特效了哇
        }
       // ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.SKILLIMAGEPATH + imageSid, itemImage);
        this.fatherwindowww = fatherWindow;
        transform.localScale = Vector3.zero;
        iTween.MoveTo(gameObject, iTween.Hash("position", transform.localPosition + new Vector3(0, 100f, 0), "easetype", iTween.EaseType.easeInOutCubic, "time", 1f, "islocal", true));
        iTween.ScaleTo(gameObject, iTween.Hash("scale", new Vector3(1.2f, 1.2f, 1.2f), "easetype", iTween.EaseType.easeInOutCubic, "time", 1f));
        Vector3 pos = new Vector3(fatherWindow.propflyPoint.localPosition.x, fatherWindow.propflyPoint.localPosition.y, fatherWindow.propflyPoint.localPosition.z);
        iTween.MoveTo(gameObject, iTween.Hash("delay", 1.1f, "position", pos, "easetype", "easeInQuad", "time", 1.5f, "islocal", true));
        iTween.ScaleTo(gameObject, iTween.Hash("delay", 1.1f, "scale", new Vector3(0.2f, 0.2f, 0.2f), "easetype", "easeInQuad", "oncomplete", "overrr", "time", 1.5f));	
    }
	public void Initialize (Texture image,BeastAttrWindow fatherWindow)
	{
		itemImage.mainTexture=image;
		this.fatherWindoww=fatherWindow;
		transform.localScale=Vector3.one;
		iTween.MoveTo ( gameObject, iTween.Hash ("position",transform.position + new Vector3(0,0.3f,0), "easetype",  iTween.EaseType.easeInOutCubic, "time", 1f));	
		iTween.ScaleTo ( gameObject, iTween.Hash ("scale",  new Vector3(1.4f,1.4f,1.4f), "easetype", iTween.EaseType.easeInOutCubic, "time", 1f));		
		Vector3 pos=new Vector3(fatherWindow.beastEffectPoint .position.x+Random.Range(-0.2f,0.2f),fatherWindow.beastEffectPoint .position.y+Random.Range(0,0.4f),fatherWindow.beastEffectPoint .position.z);
		iTween.MoveTo ( gameObject, iTween.Hash ("delay",1.1f,"position",pos, "easetype", "easeInQuad", "time", 0.2f));
		iTween.ScaleTo ( gameObject, iTween.Hash ("delay",1.1f,"scale",  new Vector3(0.2f,0.2f,0.2f), "easetype","easeInQuad", "oncomplete","overr", "time", 0.2f));	
		EffectManager.Instance.CreateEffect(transform, "Effect/UiEffect/SummonBeast2");
	}
	void over()
	{
		EffectManager.Instance.CreateEffectCtrlByCache(fatherWindow.beastEffectPoint, "Effect/UiEffect/SummonBeast1",(obj,ctrl)=>{
			ctrl.transform.position=transform.position;
		});
		gameObject.SetActive(false);
	}
	void overr()
	{
		EffectManager.Instance.CreateEffectCtrlByCache(fatherWindoww.beastEffectPoint, "Effect/UiEffect/SummonBeast1",(obj,ctrl)=>{
			ctrl.transform.position=transform.position;
		});
		gameObject.SetActive(false);
	}
    void overrr() {
        gameObject.SetActive(false);
        fatherwindowww.showTheAddSkillInfo();
    }
	void Update () {
				angel += 200f*Time.deltaTime;
			transform.localRotation = Quaternion.AngleAxis (angel, Vector3.forward);
	}
}
