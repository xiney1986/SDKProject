using System;
 using System.Collections.Generic;

/**
 * 累积登陆奖励
 * @author longlingquan
 * */
public class TotalLoginManagerment
{ 
	private int[] receivedAwardId;//已领取的奖励id集
	private int total = 0;//累积次数
	private const int LEVEL = 5;//等级5级以下不弹出窗口
	private TotalLogin[] array;//天天送奖励信息
	private TotalLogin[] arrayWeekly;//周周送奖励信息
	private TotalLogin[] arrayHoliday;//周周送奖励信息
	private TotalLogin[] arraynewDay;//新天天送奖励信息
	private List<TotalLogin> listt;
	private List<TotalLogin> listholiday;
	private List<TotalLogin> listNewDay;
	public const int BIGPRIZE = 1;//大奖
	public const int SMALLPRIZE = 0;//小奖
	public const int EVERYDAY=0;//天天送的图标
	public const int NEWEVERYDAY=3;//新天天送的图标
	public const int WEEKLY=1;//周末送的图标
	public const int HOLIDAY=2;//节日送的图标
	private int holidayTotal;//节日送累计天数
	private int holidayDoneTotal;//节日送已经领了多少天
	private bool weeklyState=true;//周末送的开放状态
	private bool holidayState=true;//节日送的开放状态
	private bool everydayState=true;//天天送的开放状态
	private bool neweverydayState=false;//新天天送的开放状态
	bool hasAward=false;

	/// <summary>
	/// 添加已领取的奖励集
	/// <param name="sid">奖励sid</param>
	/// </summary>
	public void addReceivedAwardId(int sid)
	{
		int[] array=receivedAwardId;
		int i=array.Length;
		int[] temp=new int[i+1];
		if(i>0) Array.Copy(array,0,temp,0,i);
		temp[i]=sid;
		receivedAwardId=temp;
	}
	/// <summary>
	/// 改变新天天送的领取状态
	/// </summary>
	/// <param name="sid">Sid.</param>
	public void addNewrAwardId(int sid ){
		for(int i=0;i<arraynewDay.Length;i++){
			if(arraynewDay[i].prizeSid==sid)arraynewDay[i].isAward=1;
		}
	}
	public bool WeeklyState{
		get{ return weeklyState;}
		set{weeklyState=value;}
	}
	public bool HolidayState{
		get{return holidayState;}
		set{holidayState=value;}
	}
	public bool EverydayState {
		get { return everydayState; }
		set { everydayState = value; }
	}
	public bool NeweverydayState{
		get{return neweverydayState;}
		set{neweverydayState=value;}
	}
	public int getTotalDay ()
	{
		return total;
	}
	//是否还能领奖
	public bool IsHasAward()
	{
		return hasAward;
	}
	
	public TotalLoginManagerment ()
	{ 
		array = TotalLoginConfigManager.Instance.getTotalLogins ();
		arraynewDay=NewTotalLoginConfigManager.Instance.getNewTotalLogins();
		arrayWeekly=WeeklyAwardConfigManager.Instance.getWeeklyAward();
		arrayHoliday=WeeklyAwardConfigManager.Instance.getHolidayAward();
	}
	public static TotalLoginManagerment Instance {
		get{return SingleManager.Instance.getObj("TotalLoginManagerment") as TotalLoginManagerment;}
	}
	//更新周末送的奖励
	public void updateWeeklyAwarrdData(ErlArray list){
		ErlArray arr;
		for(int i=0;i<list.Value.Length;i++){
			arr=list.Value[i]as ErlArray;
			for(int j=0;j<arrayWeekly.Length;j++){
				if(StringKit.toInt((arr.Value[0] as ErlByte).getValueString())==arrayWeekly[j].week){
					arrayWeekly[j].isloginn=1;
					arrayWeekly[j].isAward=StringKit.toInt((arr.Value[1] as ErlByte).getValueString());
				}
			}
		}
	}
	//更新节日送的奖励
	public void updateHolidasyAwardData(ErlArray list){
		ErlArray arr;
		holidayTotal=StringKit.toInt((list.Value[0]as ErlByte).getValueString());
		arr=list.Value[1]as ErlArray;
		for(int i=0;i<arr.Value.Length;i++){
			holidayDoneTotal=arr.Value.Length;
			for(int j=0;j<arrayHoliday.Length;j++){
				if(StringKit.toInt((arr.Value[i]as ErlByte).getValueString())==arrayHoliday[j].day){
					arrayHoliday[j].isAward=1;
				}
			}
		}
	}
	//得到节日送的数据
	public TotalLogin[] getHolidayAwardData(){
		listholiday=new List<TotalLogin>();
		int actionSid=getHolidayActionsTate();
		if(actionSid==0){
			return null;
		}else{
			for(int i=0;i<arrayHoliday.Length;i++){
				if(arrayHoliday[i].holidayAllSid==actionSid){
					listholiday.Add(arrayHoliday[i]);
				}
			}
		}
		return listholiday.ToArray();
	}
	public void addReceivedAwardWeek(int sid){
		for(int i=0;i<arrayWeekly.Length;i++){
			if(arrayWeekly[i].prizeSid==sid)arrayWeekly[i].isAward=1;
		}
	}
	public void addReceivedAwardHoliday(int sid){
		for(int i=0;i<arrayHoliday.Length;i++){
			if(arrayHoliday[i].prizeSid==sid)arrayHoliday[i].isAward=1;
			holidayDoneTotal+=1;
		}
	}
	//得到活动开启状态
	public int getHolidayActionsTate(){
		DateTime timen=TimeKit.getDateTimeMillis(ServerTimeKit.getMillisTime());
		string date=timen.ToShortDateString().ToString();
		string[] atcieTime=WeeklyAwardConfigManager.Instance.getTimee();
		for(int i=0;i<atcieTime.Length;i++){
			DateTime begin=Convert.ToDateTime(atcieTime[i].Split(',')[2]);
			DateTime end=Convert.ToDateTime(atcieTime[i].Split (',')[3]);
			if(TimeKit.getTimeMillis(begin)<=TimeKit.getTimeMillis(timen)&&TimeKit.getTimeMillis(end)>=TimeKit.getTimeMillis(timen)){
				return StringKit.toInt(atcieTime[i].Split(',')[0]);
			}
		}
		return 0;
	


	}
	//得到周末送的数据
	public  TotalLogin[]  getWeeklyAwardData(){
		DateTime timen=TimeKit.getDateTimeMillis(ServerTimeKit.getMillisTime());
		int today=(int)timen.DayOfWeek;
		listt=new List<TotalLogin>();
		//都要显示
		for(int i=0;i<arrayWeekly.Length;i++){
			if(listt==null)listt=new List<TotalLogin>();
			listt.Add(arrayWeekly[i]);
		}
		return  listt.ToArray();
	}
	public TotalLogin getWeeklyButtonBySid(int sid){
		foreach(TotalLogin each in arrayWeekly){
			if(each.prizeSid==sid)return each;
		}
		return null;
	}
	public TotalLogin getHolidayBySid(int sid){
		foreach(TotalLogin each in arrayHoliday){
			if(each.prizeSid==sid)return each;
		}
		return null;
	}
	public TotalLogin getTotalLoginBySid(int sid)
	{
		foreach(TotalLogin each in array)
		{
			if(each.prizeSid==sid)
			{
				return each;
			}
		}
		return null;
	}
	/// <summary>
	/// 指定的新天天送奖励
	/// </summary>
	/// <returns>The new total login by sid.</returns>
	/// <param name="sid">Sid.</param>
	public TotalLogin getNewTotalLoginBySid(int sid)
	{
		foreach(TotalLogin each in arraynewDay)
		{
			if(each.prizeSid==sid)
			{
				return each;
			}
		}
		return null;
	}
	public int getDataCountBySid(int sid)
	{
		foreach(TotalLogin each in array)
		{
		 	if(each.prizeSid==sid)
				{
					return each.totalDays;
				}
			}
		return 0;
	}
	/** 获取所有未领取的登陆奖励 */
	public TotalLogin[] getAvailableArray()
	{
		List<TotalLogin> availableAward=new List<TotalLogin>();
		if (receivedAwardId == null) return availableAward.ToArray();
		TotalLogin totalLogin;
		for(int i=0;i<array.Length;i++)
		{
			totalLogin=array[i];
			int j=0;
			for(;j<receivedAwardId.Length;j++)
			{
				if(totalLogin.prizeSid==receivedAwardId[j]) break;
			}
			//确保循环到最后一个
			if(j==receivedAwardId.Length)
				availableAward.Add(totalLogin);
		}
		return availableAward.ToArray ();
	}
	/** 获取所有可领取奖励的数量 */
	public int getActiveAwardNum()
	{
		if(!everydayState)return 0;
		int number=0;
		int totalDay = TotalLoginManagerment.Instance.getTotalDay ();
		if(totalDay>60)return 0;
		if(TotalLoginManagerment.Instance.NeweverydayState)return 0;
		TotalLogin loginAward;
		TotalLogin[] availableArray=getAvailableArray ();
		for(int j=0;j<availableArray.Length;j++)
		{
			loginAward=availableArray[j];
			if(totalDay<loginAward.totalDays) break;
			number++;
		}
		return number;
	}
	/** 获取所有的登陆奖励 */
	public TotalLogin[] getTotalArray()
	{
		return array;
	}

	public void update (int total, int[] receivedAward)
	{
		this.total = total;
		this.receivedAwardId = receivedAward;
	}
	//获得当天奖励不管是否可领
	public TotalLogin getTodayPrizeOnly ()
	{
		return array[total-1];
	}
	//是否显示图标
	public bool isShowPrize ()
	{
		//玩家等级限制
		if (UserManager.Instance.self.getUserLevel () < LEVEL)
			return false;
		//所有奖励领取完毕;
		if((!TotalLoginManagerment.Instance.EverydayState && !TotalLoginManagerment.Instance.HolidayState && !TotalLoginManagerment.Instance.WeeklyState) ||
			getHolidayActionsTate()==0&&!getHaveWeeklyAwardShow()&&receivedAwardId.Length == array.Length)
			return false;
		
		//if (receivedAwardId.Length == array.Length)
		//	return false;
		return true;
	}
	//得到节日送累计登陆天数
	public int getTotalHoliday(){
		return holidayTotal;
	}
	//得到节日送已经领了多少天了
	public int getTotalDoneHoliday(){
		return holidayDoneTotal;
	}
	/// <summary>
	/// 有没有奖励
	/// </summary>
	/// <returns><c>true</c>, if have weekly award was gotten, <c>false</c> otherwise.</returns>
	public bool getHaveWeeklyAward(){
		DateTime timen=TimeKit.getDateTimeMillis(ServerTimeKit.getMillisTime());
		DateTime  ServerOpenTime=TimeKit.getDateTimeMin(ServerTimeKit.onlineTime);
		int today=(int)timen.DayOfWeek;
		if(today==0)today=7;
		for(int i=0;i<arrayWeekly.Length;i++){
			if(arrayWeekly[i].isAward!=1&&arrayWeekly[i].isloginn==1&&arrayWeekly[i].week<=today){
				return true;
			}
		}
		return false;
	}
	/**节日送 */
	public bool getHaveHolidayAward(){
		if(!holidayState)return false;
		if(getHolidayActionsTate()==0)return false;
		if(holidayTotal-holidayDoneTotal>0)return true;
		return false;
	}
	/// <summary>
	/// 是否显示周末送，开服5天 GM工具开启
	/// </summary>
	/// <returns><c>true</c>, if have weekly award show was gotten, <c>false</c> otherwise.</returns>
	public bool getHaveWeeklyAwardShow(){
		if(!weeklyState)return false;
		DateTime timen=TimeKit.getDateTimeMillis(ServerTimeKit.getMillisTime());
		DateTime  ServerOpenTime=TimeKit.getDateTimeMin(ServerTimeKit.onlineTime);
		TimeSpan ts=timen.Subtract(ServerOpenTime);
		if(ts.Days>=5)return true;
		return false;
	}
	public int getWeeklyAwardNum(){
		int num=0;
		if(!getHaveWeeklyAwardShow())return 0;
		DateTime timen=TimeKit.getDateTimeMillis(ServerTimeKit.getMillisTime());
		DateTime  ServerOpenTime=TimeKit.getDateTimeMin(ServerTimeKit.onlineTime);
		int today=(int)timen.DayOfWeek;
		if(today==0)today=7;
		for(int i=0;i<arrayWeekly.Length;i++){
			if(arrayWeekly[i].isAward!=1&&arrayWeekly[i].isloginn==1&&arrayWeekly[i].week<=today){
				num+=1;
			}
		}
		return num;
	}
	public int getHolidayAwardNum(){
		int num=0;
		if(getHolidayActionsTate()==0)return 0;
		int actionSid=getHolidayActionsTate();
		for(int i=0;i<arrayHoliday.Length;i++){
			if(arrayHoliday[i].holidayAllSid==actionSid&&arrayHoliday[i].isAward!=1&&arrayHoliday[i].day<=holidayTotal){
				num+=1;
			}
		}
		return num;
	}
	/// <summary>
	/// 更新那些天登陆过（新天天送）
	/// </summary>
	/// <param name="array">Array.</param>
	public void updateNewdayLoginDate(ErlArray array){
		for(int i=0;i<array.Value.Length;i++){
			int day=StringKit.toInt((array.Value[i]as ErlType).getValueString());
			for(int j=0;j<arraynewDay.Length;j++){
				if(day==0)day=7;
				if(day==arraynewDay[j].totalDays){
					arraynewDay[j].isloginn=1;
					break;
				}
			}
		}
	}
	/// <summary>
	/// 那些SID的奖励被领取过
	/// </summary>
	public void updateNewDayLoginAward(ErlArray array){
		for(int i=0;i<array.Value.Length;i++){
			for(int j=0;j<arraynewDay.Length;j++){
				if(StringKit.toInt((array.Value[i] as ErlType).getValueString())==arraynewDay[j].prizeSid){
					arraynewDay[j].isAward=1;
				}
			}
		}
	}
	/// <summary>
	/// 得到新天天送的数据
	/// </summary>
	/// <returns>The new day date.</returns>
	public  TotalLogin[] getNewDayDate(){
		return arraynewDay;
	}
	/// <summary>
	/// 返回新天天送有没有奖励可以领
	/// </summary>
	/// <returns>The fist award.</returns>
	public bool isNewAward(){
		if(!everydayState)return false;
		for(int j=0;j<arraynewDay.Length;j++){
		  if(arraynewDay[j].isloginn==1&&arraynewDay[j].isAward!=1){
				return true;
			}
		}
		return false;
	}
	/// <summary>
	/// 新天天送第一个没有领取的奖励 默认第0个
	/// </summary>
	/// <returns>The first award.</returns>
	public int getFirstAward(){
		for(int j=0;j<arraynewDay.Length;j++){
			if(arraynewDay[j].isloginn==1&&arraynewDay[j].isAward!=1){
				return j;
			}
		}
		return 0;
	}

}

