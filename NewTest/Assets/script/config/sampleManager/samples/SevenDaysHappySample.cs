using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SevenDaysHappySample
{
	public int dayId;// dayID//
	public Dictionary<int,SevenDaysHappyDetail> detailsDic = new Dictionary<int,SevenDaysHappyDetail>();// 内容条目//

	public SevenDaysHappySample(int _id)
	{
		this.dayId = _id;
	}
}

public class SevenDaysHappyDetail
{
	public int type;// 内容类型//
	public string detailName;// 内容名称//
	public List<SevenDaysHappyMisson> missonList = new List<SevenDaysHappyMisson>();// 内容具体任务//
	
	public SevenDaysHappyDetail(SevenDaysHappyMisson misson)
	{
		this.type = misson.detailType;
		this.detailName = misson.detailName;
		missonList.Add(misson);
	}
}

public class SevenDaysHappyMisson
{
	public int dayID;
	public int missonID;
	public int detailType;
	public int missonType;// 任务类型 充值为1 登录为2 其他为0//
	public PrizeSample[] prizes;// 任务奖励//
	public int[] conditions;// 任务完成条件//
	public int missonOrder;//  任务排序order//
	public string detailName;// 内容名称//
	public string conditionDes;// 条件描述//
	public int missonState;// 任务完成状态//
	public int[] missonProgress;// 完成进度//
	public int price;// 半价商品价格//
	public int goodsID;// 只有半价商品才有的商品条目id//
	public int prizeType;// 领奖类型//

	public SevenDaysHappyMisson(int missonID,int dayID,int detailType,string detailName,int missonType,string conditions,string prizes,int order)
	{
		this.dayID = dayID;
		this.missonID = missonID;
		this.detailType = detailType;
		this.missonType = missonType;
		parseMissonCondition(conditions);
		if(detailType == SevenDaysHappyDetailType.banjiaqianggou)// 为半价购买时//
		{
			parseMissonPrizeByGoodsID(prizes);
		}
		else
		{
			parseMissonPrize(prizes);
		}
		this.missonOrder = order;
		this.detailName = detailName;
	}

	public void parseMissonCondition(string str)
	{
		string[] str1 = str.Split('/');
		this.conditionDes = str1[1];

		string[] strArr = str1[0].Split(',');
		conditions = new int[strArr.Length];
		for(int i=0;i<strArr.Length;i++)
		{
			conditions[i] = StringKit.toInt(strArr[i]);
		}
	}
	public void parseMissonPrize(string str)
	{
		string[] str1 = str.Split('/');
		this.prizeType = StringKit.toInt(str1[0]);
		this.price = StringKit.toInt(str1[2]);

		string[] strArr = str1[1].Split('#');
		prizes = new PrizeSample[strArr.Length];
		for(int i=0;i<strArr.Length;i++)
		{
			prizes[i] = new PrizeSample(strArr[i],',');
		}
	}
	public void parseMissonPrizeByGoodsID(string str)
	{
		string[] str1 = str.Split('/');
		this.prizeType = StringKit.toInt(str1[0]);
		this.price = StringKit.toInt(str1[2]);
		
		string[] strArr = str1[1].Split(',');
		int goodID = StringKit.toInt(strArr[1]);
		this.goodsID = goodID;
		GoodsSample goodsample = GoodsSampleManager.Instance.getGoodsSampleBySid(goodID);
		string strCompose = strArr[0] + "," + goodsample.goodsSID + "," +  strArr[2];
		prizes = new PrizeSample[strArr.Length];
		for(int i=0;i<strArr.Length;i++)
		{
			prizes[i] = new PrizeSample(strCompose,',');
		}
	}

}

public class MissonFromServer
{
	public int missonID;
	public int missonState;
	public int[] missonProgress;

	public MissonFromServer()
	{

	}

	public MissonFromServer(int missonID,int missonState,int[] missonProgress)
	{
		this.missonID = missonID;
		this.missonState = missonState;
		this.missonProgress = missonProgress;
	}
}

public class SevenDaysHappyConditionType
{

}

public class SevenDaysHappyDetailType
{
//	        每日福利 = 1
//		    主线副本 = 2
//			主角培养 = 3
//			半价抢购 = 4
//			装备强化 = 5
//			天梯     = 6
//			讨伐副本 = 7
//			卡片进化 = 8
//			宝藏掠夺 = 9
//			猎魂     = 10
//			精英副本 = 11
//			神秘商店 = 12
//			女神试炼 = 13
//			圣器强化 = 14
//			战力突破 = 15
//			等级突破 = 16
	public const int meirifuli = 1;
	public const int zhuxianfuben = 2;
	public const int zhujiaopeiyang = 3;
	public const int banjiaqianggou = 4;
	public const int zhuangbeiqianghua = 5;
	public const int tianti = 6;
	public const int taofafuben = 7;
	public const int kapianjinhua = 8;
	public const int bangzanglvduo = 9;
	public const int liehun = 10;
	public const int jingyingfuben = 11;
	public const int shenmishangdian = 12;
	public const int nvshenshilian = 13;
	public const int shengqiqianghua = 14;
	public const int zhandoulitupo = 15;
	public const int dengjitupo = 16;
}
public class SevenDaysHappyMissonType
{
	public const int Recharge = 1;// 充值任务//
	public const int Login = 2;// 登录//
	public const int CompleteByCount = 3;// 按数量完成的任务(存在数量进度)//
	public const int CompleteNotByCount = 4;// 非按数量完成的任务(只有完成任务和没有完成任务两种状态)//
	public const int Login_AwardCard = 5;// 第七天登录送卡//
}

// 任务状态//
public class SevenDaysHappyMissonState
{
	public const int Completed = 0;// 已完成可领取//
	public const int Doing = 1;// 进行中//
	public const int Recevied = 2;// 已领取//
}

// 奖励类型//
public class SevenDaysHappyPrizeType
{
	public const int Not_MoreChooseOne = 0;// 非多选一//
	public const int MoreChooseOne = 1;// 多选一//
}

