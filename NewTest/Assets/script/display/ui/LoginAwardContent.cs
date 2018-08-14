using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 登陆奖励容器
/// </summary>
public class LoginAwardContent  : dynamicContent
{

	public GameObject loginAwardItemPrefab;
	List<TotalLogin> loginAwards;
	private int typee;

	public void reLoad (TotalLogin[] awards,int dayType)
	{
		this.typee=dayType;
		loginAwards = new List<TotalLogin>();
		if(typee==TotalLoginManagerment.EVERYDAY){
			if (awards != null){
				
				int endDay=60;
				//小于30天显示30天,大于30天显示60天
				if(TotalLoginManagerment.Instance.getTotalDay()<=30){
					endDay=30;
				}
				
				//只显示小于endday的奖励
				for(int i=0;i<awards.Length;i++){
					if(i<awards.Length && awards[i]!=null && awards[i].totalDays<=endDay){
						loginAwards.Add(awards[i]);
					}else{
						break;
					}
				}
				
				base.reLoad (loginAwards.Count);
			}
		}else if(typee==TotalLoginManagerment.WEEKLY||typee==TotalLoginManagerment.HOLIDAY||typee==TotalLoginManagerment.NEWEVERYDAY){
			for(int i=0;i<awards.Length;i++){
				loginAwards.Add(awards[i]);
			}
			if(typee==TotalLoginManagerment.NEWEVERYDAY){
				int index=TotalLoginManagerment.Instance.getFirstAward();
				base.reLoad (loginAwards.Count,index);
			}else{
				base.reLoad (loginAwards.Count);
			}

		}		
	}
	
	public override void updateItem (GameObject item, int index)
	{
		LoginAwardItem awardItem = item.GetComponent<LoginAwardItem> ();
		awardItem.updateAwardItem(loginAwards [index],fatherWindow,typee);
	}
	
	public override void initButton (int  i)
	{
		if (nodeList [i] == null){
		nodeList [i] = NGUITools.AddChild (gameObject, loginAwardItemPrefab);
		}
	}
}
