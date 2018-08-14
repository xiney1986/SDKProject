using UnityEngine;
using System.Collections;

/// <summary>
/// 副本主界面英雄解锁进度
/// </summary>
public class MissionHeroPropessItem : MonoBase {
	/// <summary>
	/// 英雄头像.
	/// </summary>
	public UITexture headIcon;
	public UILabel jinduTitle;
	public UILabel info;
	public UISprite show_fore;
	private Card card;
	public UILabel name;
	private float oldNum;
	private float newNum;
	private float totalNum;
	/// <summary>
	/// 飞图标动画
	/// </summary>
	public void showHeroEffect(Card card,bool bo,int type){
		this.card=card;
		headIcon.gameObject.SetActive (true);
		if(type==7){
			ResourcesManager.Instance.LoadAssetBundleTexture (UserManager.Instance.self.getIconPath (), headIcon);
			headIcon.gameObject.transform.localScale=new Vector3(1.05f,1.05f,1f);
		}
		else if(type==5)ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + card.getIconID(), headIcon);
		else ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.NVSHENHEADPATH + card.getImageID()+"_head", headIcon);
		if(bo){
			headIcon.transform.localPosition = new Vector3(230,-150,0);
			TweenScale ts = TweenScale.Begin(headIcon.gameObject,0.4f,Vector3.one);
			ts.from = new Vector3(3f,3f,3f);
			EventDelegate.Add (ts.onFinished, () => {
				TweenPosition tp = TweenPosition.Begin(headIcon.gameObject,0.6f,new Vector3(0,0,0));
				EventDelegate.Add (tp.onFinished, () => {
					EffectManager.Instance.CreateEffect(headIcon.transform,"Effect/UiEffect/feature_open");
					MaskWindow.UnlockUI();
				},true);
			},true);
		}
		updatePropess();

	}
	private void updatePropess(){
		HeroGuideSample heroGuideSample=HeroGuideManager.Instance.getCurrectSample(MissionInfoManager.Instance.mission.getPlayerPointIndex ());
		if(heroGuideSample.prizeSample[0].type==7){
			name.text=QualityManagerment.getQualityColor(StringKit.toInt(heroGuideSample.prizeSample[0].num)+3)+LanguageConfigManager.Instance.getLanguage("teamEdit_err04l5",StringKit.toInt(heroGuideSample.prizeSample[0].num)==1?LanguageConfigManager.Instance.getLanguage("s0076"):LanguageConfigManager.Instance.getLanguage("s0077"));
			info.text=LanguageConfigManager.Instance.getLanguage("missionMain02l");
		}else{
			info.text=LanguageConfigManager.Instance.getLanguage("missionMain02");
			name.text=QualityManagerment.getQualityColor(card.getQualityId())+card.getName()+"[-]"+"X"+HeroGuideManager.Instance.getCurrectSample(MissionInfoManager.Instance.mission.getPlayerPointIndex ()).prizeSample[0].num.ToString();
		}
			totalNum=HeroGuideManager.Instance.getTotalNum(MissionInfoManager.Instance.mission.sid)-1;
		if(totalNum>0){

			newNum=(float)heroGuideSample.stepNum/(float)totalNum;;
		}
	}
	public  void updatePropess(bool bo,Card card,int type){
		HeroGuideSample heroGuideSample=HeroGuideManager.Instance.getCurrectSample(MissionInfoManager.Instance.mission.getPlayerPointIndex ());
		this.card=card;
		if(type==7){
			info.text=LanguageConfigManager.Instance.getLanguage("missionMain02l");
			name.text=QualityManagerment.getQualityColor(StringKit.toInt(heroGuideSample.prizeSample[0].num)+3)+LanguageConfigManager.Instance.getLanguage("teamEdit_err04l5",StringKit.toInt(heroGuideSample.prizeSample[0].num)==1?LanguageConfigManager.Instance.getLanguage("s0076"):LanguageConfigManager.Instance.getLanguage("s0077"));
		}else{
			info.text=LanguageConfigManager.Instance.getLanguage("missionMain02");
		
		name.text=QualityManagerment.getQualityColor(card.getQualityId())+card.getName()+"[-]"+"X"+HeroGuideManager.Instance.getCurrectSample(MissionInfoManager.Instance.mission.getPlayerPointIndex ()).prizeSample[0].num.ToString();
		}
		if(type==7){
			ResourcesManager.Instance.LoadAssetBundleTexture (UserManager.Instance.self.getIconPath (), headIcon);
			headIcon.gameObject.transform.localScale=new Vector3(1.05f,1.05f,1f);
		}
		else if(type==5)ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + card.getIconID(), headIcon);
		else ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.NVSHENHEADPATH + card.getImageID()+"_head", headIcon);
		totalNum=HeroGuideManager.Instance.getTotalNum(MissionInfoManager.Instance.mission.sid)-1;
		if(totalNum>0){
			if(!bo){
				int index=(heroGuideSample.stepNum-1)>=0?(heroGuideSample.stepNum-1):0;
				newNum=(float)(index)/(float)totalNum;
			}else{
				EffectManager.Instance.CreateEffect(headIcon.transform,"Effect/UiEffect/feature_open");
				newNum=(float)heroGuideSample.stepNum/(float)totalNum;
			}
		}
	}
	public  void updatePropess(bool bo,Card card,int type,HeroGuideSample hero){
		this.card=card;
		if(hero.prizeSample[0].type==7){
			name.text=QualityManagerment.getQualityColor(StringKit.toInt(hero.prizeSample[0].num)+3)+LanguageConfigManager.Instance.getLanguage("teamEdit_err04l5",StringKit.toInt(hero.prizeSample[0].num)==1?LanguageConfigManager.Instance.getLanguage("s0076"):LanguageConfigManager.Instance.getLanguage("s0077"));
			info.text=LanguageConfigManager.Instance.getLanguage("missionMain02l");
		}else{
			info.text=LanguageConfigManager.Instance.getLanguage("missionMain02");
		
		name.text=QualityManagerment.getQualityColor(card.getQualityId())+card.getName()+"[-]"+"X"+hero.prizeSample[0].num.ToString();
		}
		if(type==7){
			ResourcesManager.Instance.LoadAssetBundleTexture (UserManager.Instance.self.getIconPath (), headIcon);
			headIcon.gameObject.transform.localScale=new Vector3(1.05f,1.05f,1f);
		}
		else if(type==5)ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + card.getIconID(), headIcon);
		else ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.NVSHENHEADPATH + card.getImageID()+"_head", headIcon);
		totalNum=HeroGuideManager.Instance.getTotalNum(hero.missionSid)-1;
		if(totalNum>0){
			newNum=(float)hero.stepNum/(float)totalNum;
		}
	}
	void Update(){
		if(newNum>oldNum){
			oldNum += 0.15f * Time.deltaTime;
		}
		show_fore.fillAmount=oldNum;
		jinduTitle.text=(int)(oldNum*100)+"%";
		if(newNum==0.0f){
			show_fore.fillAmount=0;
			jinduTitle.text="0%";
		}
	}
}
