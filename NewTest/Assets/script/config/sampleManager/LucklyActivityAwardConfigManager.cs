using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
/// <summary>
/// 抽奖奖励配置文件
/// </summary>
public class LucklyActivityAwardConfigManager : SampleConfigManager {

	//单例
	private static LucklyActivityAwardConfigManager instance;
	private List<RankAward> list;//排行列表
	private List<RankAward> sourceList;
	private int mySource;//我的积分

	public LucklyActivityAwardConfigManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_LUCKLYACTIVITY);
	}
	public static LucklyActivityAwardConfigManager Instance {
		get{
			if(instance==null)
				instance=new LucklyActivityAwardConfigManager();
			return instance;
		}
	}
	//解析配置
	public override void parseConfig (string str)
	{  
		String[] strArr=str.Split('|');
		if(StringKit.toInt(strArr[0])==1){
			parseRink(str);//解析排行数据
		}else if(StringKit.toInt(strArr[0])==2){
			parseSource(str);//解析积分数据
		}

	}
	private void parseRink(string str){
		RankAward bb =new RankAward();
		String[] strArr=str.Split('|');
		bb.type=StringKit.toInt(strArr[0]);
		bb.noticeSid=StringKit.toInt(strArr[1]);//0是装备积分 1是卡片积分(活动Sid)
		bb.rinkNum=StringKit.toInt(strArr[2]);
		bb.dec=strArr[3];
		bb.prizes=parsePrizes(strArr[4]);
		if (list == null)
			list = new List<RankAward> ();
		list.Add (bb);
	}
	private void parseSource(string str){
		RankAward bb=new RankAward();
		String[] strArr=str.Split('|');
		bb.type=StringKit.toInt(strArr[0]);
		bb.noticeSid=StringKit.toInt(strArr[1]);//0是装备积分 1是卡片积分
		bb.needSource=StringKit.toInt(strArr[2]);
		bb.awardSid=StringKit.toInt(strArr[3]);
		bb.prizes=parsePrizes(strArr[4]);
		if (sourceList == null)
			sourceList = new List<RankAward> ();
		sourceList.Add (bb);

	}
	//解析奖品
	private PrizeSample[] parsePrizes (string str)
	{
		string[] strArr = str.Split ('#');
		PrizeSample[] prizes = new PrizeSample[strArr.Length];
		for (int i = 0; i < strArr.Length; i++) {
			prizes[i]=new PrizeSample();
			string[] strs = strArr[i].Split(',');
			prizes[i].type = StringKit.toInt(strs[0]);
			prizes[i].pSid = StringKit.toInt(strs[1]);
			prizes[i].num = strs[2];
		}
		return prizes;
	}
	//得到排行
	public RankAward[] getAward(int noticeSid){
		List<RankAward> listt=new List<RankAward>();
		for(int i=0;i<list.Count;i++){
			if(list[i].noticeSid==noticeSid){
				listt.Add(list[i]);
			}
		}
		return listt.ToArray();
	}
	//得到积分奖励
	public RankAward[] getSource(int type){
		List<RankAward> listtt=new List<RankAward>();
		for(int i=0;i<sourceList.Count;i++){
			if(sourceList[i].noticeSid==type){
				listtt.Add(sourceList[i]);
			}
		}
		return listtt.ToArray();
	}
	//得到积分奖励
	public RankAward getSourceByIntegral(int type,int integral){
		RankAward tmp = null;
		int cur = 0;
		for(int i=0;i<sourceList.Count;i++){
			if(sourceList[i].noticeSid==type && sourceList[i].needSource >= integral && sourceList[i].needSource > cur){
				tmp = sourceList[i];
				cur = sourceList[i].needSource;
			}
		}
		return tmp;
	}
	//得到第一个积分奖励
	public RankAward getFirstSource(int type) {
		for(int i=0;i<sourceList.Count;i++){
			if(sourceList[i].noticeSid==type){
				return sourceList[i];
			}
		}
		return null;
	}
	public void updateAwardDate(int sid,List<string> sidList){
		if(sourceList!=null){
			for(int i=0;i<sidList.Count;i++){
				for(int j=0;j<sourceList.Count;j++){
					if(sourceList[j].noticeSid==sid&&sourceList[j].awardSid==StringKit.toInt(sidList[i])){
						sourceList[j].isAward=true;
						break;
					}
				}
			}
		}
	}
	public void updateAwardDateByIntegral(int sid,List<int> sidList){
		if(sourceList!=null){
			for(int i=0;i<sidList.Count;i++){
				for(int j=0;j<sourceList.Count;j++){
					if(sourceList[j].noticeSid == sid && sourceList[j].needSource == sidList[i]){
						sourceList[j].isAward=true;
						break;
					}
				}
			}
		}
	}
	public RankAward updateAwardDateByIntegral (int sid, int integral) {
		if (sourceList != null) {
			for (int j=0; j<sourceList.Count; j++) {
				if (sourceList [j].noticeSid == sid && sourceList [j].needSource == integral)
					return sourceList [j];
			}
		}
		return null;
	}
	public void setMySource(int num){
		mySource=num;
	}
	public int getMySource(){
		return mySource;
	}
}
