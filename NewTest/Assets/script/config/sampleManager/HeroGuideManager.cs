using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 解锁特殊英雄管理器
/// </summary>
public class HeroGuideManager : SampleConfigManager {

	//单例
	private static HeroGuideManager instance;
	private List<HeroGuideSample> list;
	public bool doBegin;
	public float oldNumm;
	public HeroGuideManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_HERO_GUIDE);
	}
	public static HeroGuideManager Instance {
		get{
			if(instance==null)
				instance=new HeroGuideManager();
			return instance;
		}
	}
	//获得模板对象
	public HeroGuideSample getHeroGuideSampleBySid (int sid)
	{ 
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as HeroGuideSample;
	}
	public override void parseConfig (string str)
	{
		HeroGuideSample sample = new HeroGuideSample (str); 
		if (list == null)
			list = new List<HeroGuideSample> ();
		list.Add (sample);
	}
	/// <summary>
	/// 得到该部分一共需要多少次指引才能完成
	/// </summary>
	/// <returns>The total number.</returns>
	public int getTotalNum(int missionSid){
		int num=0;
		for(int i=0;i<list.Count;i++){
			if(list[i].missionSid==missionSid){
				for(int j=0;j<list.Count;j++){
					if(list[i].prizeSample[0].pSid==list[j].prizeSample[0].pSid)num++;
				}
				return num;
			}
		}
		return num;
	}
	/// <summary>
	/// 检查该副本有没有解锁进度.
	/// </summary>
	/// <returns><c>true</c>, if have GUID was checked, <c>false</c> otherwise.</returns>
	public bool checkHaveGuid(){
		if(MissionInfoManager.Instance.mission!=null){
			for(int i=0;i<list.Count;i++){
				if(MissionInfoManager.Instance.mission.sid ==list[i].missionSid && FuBenManagerment.Instance.isNewMission (ChapterType.STORY,list[i].missionSid)){//当前的副本是不是新副本和是不是在进度列表里面的副本
					return true;
				}
			}
			return false;
		}
		return false;

	}
	/// <summary>
	/// 检查这个兑换完成了没有
	/// </summary>
	/// <returns><c>true</c>, if have exist GUID was checked, <c>false</c> otherwise.</returns>
	public bool checkHaveExistGuid(){
		if(MissionInfoManager.Instance.mission!=null){
			int lastSid=FuBenManagerment.Instance.getLastStoryMissionSid();
			for(int i=0;i<list.Count;i++){
				if(lastSid==list[i].missionSid&&!getLastMissionForGuide().Contains(lastSid)){
					return true;
				}
			}
			return false;
		}
		return false;
	}
	/// <summary>
	/// 检查这个兑换完成了没有
	/// </summary>
	/// <returns><c>true</c>, if have exist GUID was checked, <c>false</c> otherwise.</returns>
	public bool checkHaveExistGuidInMain(){
		int lastSid=FuBenManagerment.Instance.getLastStoryMissionSid();
		for(int i=0;i<list.Count;i++){
			if(lastSid==list[i].missionSid&&!getLastMissionForGuide().Contains(lastSid)){
				return true;
			}
		}
		return false;
	}
	/// <summary>
	/// 这个章节通了没有
	/// </summary>
	/// <returns><c>true</c>, if pass chapter was ised, <c>false</c> otherwise.</returns>
	private bool isPassChapter(int sid){
		ChapterSample csample = ChapterSampleManager.Instance.getChapterSampleBySid (sid);
		for(int k=0;k<csample.missions.Length;k++){
			if(FuBenManagerment.Instance.isNewMission(1,csample.missions[k]))return false;
		}
		return true;
	}
	public int getTheNewMission(){
		List<int> missionSids=getFistMissionForGuide();
		for(int i=0;i<missionSids.Count;i++){
			if(FuBenManagerment.Instance.isNewMission(1,missionSids[i])){
				return missionSids[i];//没有激活（可以显示等级奖励）
			}
		}
		return -1;//
	}
	public bool isShowLevelAward(){
		if(getTheNewMission()!=-1){
			return true;//可以显示
		}else{
			int lastSid=FuBenManagerment.Instance.getLastStoryMissionSid();
			for(int i=0;i<list.Count;i++){
				if(lastSid==list[i].missionSid&&!getLastMissionForGuide().Contains(lastSid)){
					return false;
				}
			}
			return true;
		}
	}
	/// <summary>
	/// 拿到每个进度奖励的第一个missionSid
	/// </summary>
	/// <returns>The fist mission for guide.</returns>
	public List<int> getFistMissionForGuide(){
		List<int> listt=new List<int>();
		for(int i=0;i<list.Count;i++){
			if(list[i].stepNum==0){
				listt.Add(list[i].missionSid);
			}
		}
		return listt;
	}
	/// <summary>
	/// 拿到每个进度奖励的最后一个missionSid
	/// </summary>
	/// <returns>The fist mission for guide.</returns>
	public List<int> getLastMissionForGuide(){
		List<int> listt=new List<int>();
		for(int i=0;i<list.Count-1;i++){
			if(list[i].prizeSample[0].pSid!=list[i+1].prizeSample[0].pSid)listt.Add(list[i].missionSid);

		}
		listt.Add(list[list.Count-1].missionSid);
		return listt;
	}
	/// <summary>
	/// 如果没通就拿最后一个Sid
	/// </summary>
	/// <returns>The no pass sid.</returns>
	private int getNoPassSid(int sid){
		ChapterSample csample = ChapterSampleManager.Instance.getChapterSampleBySid (sid);
		for(int k=0;k<csample.missions.Length;k++){
			if(FuBenManagerment.Instance.isNewMission(1,csample.missions[k])){
				return k;
			}
		}
		return -1;
	}
	/// <summary>
	/// 拿到过时的激活任务进度
	/// </summary>
	/// <returns>The old sample.</returns>
	public HeroGuideSample getOldSample(){
		if(MissionInfoManager.Instance.mission!=null){
			int lastSid=FuBenManagerment.Instance.getLastStoryMissionSid();
			int missionSid=0;
			for(int i=0;i<list.Count;i++){
				if(lastSid==list[i].missionSid&&!getLastMissionForGuide().Contains(lastSid)){
					missionSid=lastSid;
				}
			}
			int k=-1;
			for(int j=0;j<list.Count;j++){
				if(missionSid==list[j].missionSid){
					k=j;
				}
			}
			if(k!=-1)return list[k];
		}
		return null;
	}
	/// <summary>
	/// 拿到过时的激活任务进度(主窗口)
	/// </summary>
	/// <returns>The old sample.</returns>
	public HeroGuideSample getOldSampleInMain(){
		int lastSid=FuBenManagerment.Instance.getLastStoryMissionSid();
		int missionSid=0;
		for(int i=0;i<list.Count;i++){
			if(lastSid==list[i].missionSid&&!getLastMissionForGuide().Contains(lastSid)){
				missionSid=lastSid;
			}
		}
		int k=-1;
		for(int j=0;j<list.Count;j++){
			if(missionSid==list[j].missionSid){
				k=j;
			}
		}
		if(k!=-1)return list[k];
		return null;
	}
	/// <summary>
	/// 拿到当前的进度模板（最合适的）
	/// </summary>
	/// <returns>The currect sample.</returns>
	public HeroGuideSample getCurrectSample(int index){
		if(MissionInfoManager.Instance.mission!=null){
			for(int i=0;i<list.Count;i++){
				if(MissionInfoManager.Instance.mission.sid ==list[i].missionSid &&list[i].pointNum>=index){
					return list[i];
				}
			}
			for(int j=0;j<list.Count;j++){
				if(MissionInfoManager.Instance.mission.sid ==list[j].missionSid){
					return list[j];
				}
			}
			return null;
		}
		return null;
	}
	public void openNvShenWindow(HeroGuideSample heroGuideSample,bool flag,int type ){
		Card cardd=HeroGuideManager.instance.getSuitCard(heroGuideSample);
		if(MissionInfoManager.Instance.autoGuaji&&UiManager.Instance.missionMainWindow!=null){
			UiManager.Instance.missionMainWindow.showEffectForHero(flag);
		}else if(!MissionInfoManager.Instance.autoGuaji){
			UiManager.Instance.openDialogWindow<OpenNvShenWindow> (( win ) => {
				win.initWindow(cardd,type,flag,heroGuideSample.dec);
			});
		}

	}
	private Card getFistGoddess(int sid){
		List<BeastEvolve> list =BeastEvolveManagerment.Instance.getAllBest();//女神样本
		ArrayList beastList=StorageManagerment.Instance.getAllBeast();//已经有的女神
		bool flag=false;
		int flagNUm=0;
		if(beastList==null){
			return  CardManagerment.Instance.createCard (sid);
		}else{
			for(int i=0;i<list.Count;i++){
				flag=false;
				for(int j=0;j<beastList.Count;j++){
					flagNUm=i;
					if((list[i] as BeastEvolve).getBeast(0).getName()==(beastList[j] as Card).getName()){
						flag=true;
						break;
					}
				}
				if(!flag)return(list[flagNUm] as BeastEvolve).getBeast(0);
			}
			return  CardManagerment.Instance.createCard (sid);
		}
	}
	public Card getSuitCard(HeroGuideSample hero){
		Card cardd;
		if(hero.prizeSample[0].type==6){
			cardd=getFistGoddess(hero.prizeSample[0].pSid);
		}else if(hero.prizeSample[0].type==7){
			cardd=StorageManagerment.Instance.getRole(UserManager.Instance.self.mainCardUid);
		}
		else{
			cardd=CardManagerment.Instance.createCard(hero.prizeSample[0].pSid);
		}
		return cardd;
	}
}
