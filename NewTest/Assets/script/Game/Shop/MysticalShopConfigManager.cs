using UnityEngine;
using System;
using System.Collections;

public class MysticalShopConfigManager : ConfigManager {

	/*filed */
	/**刷新一次需要的将军令数量 */
	public int needTokenNum;
	/**刷新一次需要的钻石数量 */
	public int[] needRMBnum;
	public string[] updateData;
	/* static fields */
	private static MysticalShopConfigManager instance;
	
	/* static methods */
	public static MysticalShopConfigManager Instance {
		get {
			if (instance == null)
				instance = new MysticalShopConfigManager();
			return instance;
		}
	}
	/* methods */
	public MysticalShopConfigManager() {
		base.readConfig(ConfigGlobal.CONFIG_MYSTICAL_OPERATOR);
	}
	/** 解析 */
	public override void parseConfig(string str) {
		string[] strs=str.Split('|');
		checkLength (strs.Length,4);
		// str[0] 配置文件说明
		// str[1]
		needTokenNum=StringKit.toInt(strs[1]);
		// str[2]
		needRMBnum=parseMoneyNum(strs[2]);
			//StringKit.toInt(strs[2]);
		updateData=getData(strs[3]);

	}
	private int[] parseMoneyNum(string str){
		string[] st=str.Split(',');
		int[] numm=new int[st.Length];
		for(int i=0;i<st.Length;i++){
			numm[i]=StringKit.toInt(st[i]);
		}
		return numm;
	}
	public void checkLength (int len, int max) { 
		if (len != max)
			throw new Exception (this.GetType () + " ConfigGlobal.CONFIG_STARSOUL_OPERATOR error len=" + len + " max=" + max);
	}
	/// <summary>
	///一次刷新需要的刷新令数量
	/// </summary>
	/// <returns>The need token number.</returns>
	public int getNeedTokenNum(){
		return needTokenNum;
	}
	public int getNeedRmbNum(){
		return ShopManagerment.Instance.getMyRushCount()>=(needRMBnum.Length-1)?needRMBnum[needRMBnum.Length-1]:needRMBnum[ShopManagerment.Instance.getMyRushCount()];
	}
	public string[] getData(String str){
		return str.Split('#');
	}
	public string getUseData(){

		for(int i=0;i<updateData.Length;i++){
			string temp=updateData[i];
			string[] temps=temp.Split(':');

			if((StringKit.toInt(temps[0])*60+StringKit.toInt(temps[1]))>(ServerTimeKit.getDateTime().Hour*60+ServerTimeKit.getDateTime().Minute)){
				return updateData[i];
			}
		}
		return updateData[0];
		//Debug.Log(TimeKit.dateToFormat((int)TimeKit.getTimeMillis(),LanguageConfigManager.Instance.getLanguage ("notice07")));
		//DateTime dt=DateTime.Now();
		//Debug.Log("dt.ToLongTimeString().ToString()"+dt.ToLongTimeString().ToString());
	}
	/// <summary>
	/// 得到下次刷新的时间差
	/// </summary>
	/// <returns>The next flush time.</returns>
	public long getNextFlushTime(){
		string temp=getUseData();
		string[] temps=temp.Split(':');
		long nowTime=ServerTimeKit.getCurrentSecond()*1000;//现在的时间
		long timeLoading=(StringKit.toInt(temps[0])*60+StringKit.toInt(temps[1]))*60*1000-nowTime;//时间差
		if(timeLoading<0){
			timeLoading=24*60*60*1000-nowTime+(StringKit.toInt(temps[0])*60+StringKit.toInt(temps[1]))*60*1000;
		}
		return timeLoading;
	}
	public string getUseData(DateTime dataTime){
		for(int i=0;i<updateData.Length;i++){
			string temp=updateData[i];
			string[] temps=temp.Split(':');
			if((StringKit.toInt(temps[0])*60+StringKit.toInt(temps[1]))>(dataTime.Hour*60+dataTime.Minute)){
				return updateData[i];
			}
		}
		return updateData[0];
	}
	public string getNextData(int num){
		DateTime nowTime=ServerTimeKit.getDateTime();
		for(int i=0;i<updateData.Length;i++){
			string temp=updateData[i];
			string[] temps=temp.Split(':');
			if((StringKit.toInt(temps[0])*60+StringKit.toInt(temps[1]))>num){
				return nowTime.DayOfYear.ToString()+":"+updateData[i];
			}
		}
		return (nowTime.DayOfYear+1).ToString()+":"+updateData[0];
	}
	/// <summary>
	/// 是否显示神秘商店的刷新标示
	/// </summary>
	public bool isCanShowFlag(string type){
		string falg=PlayerPrefs.GetString (UserManager.Instance.self.uid + "mysical"+type , "null");
		if(ShopListSamleManager.Instance.getMyoPenLv()>UserManager.Instance.self.getUserLevel()){
			return false;
		}
		if(falg=="null")return true;
		string[] times=falg.Split(':');
        int dayofYear=ServerTimeKit.getDateTime().DayOfYear;
        int saveDay=StringKit.toInt(times[0]);
        if (dayofYear > saveDay) return true;
        else if (dayofYear < saveDay) return false;
        else {
            if (ServerTimeKit.getCurrentSecond() >= (StringKit.toInt(times[1]) * 60 * 60 + StringKit.toInt(times[2]) * 60 + StringKit.toInt(times[3]))) return true;
        }
		return false;
	}
	/// <summary>
	/// 保存下次刷新时间
	/// </summary>
	public void saveShowFlagTime(string type){
		string time=getUseData();
		if(time==updateData[0]){
			time=(ServerTimeKit.getDateTime().DayOfYear+1).ToString()+":"+time;
		}else time=(ServerTimeKit.getDateTime().DayOfYear).ToString()+":"+time;
		PlayerPrefs.SetString(UserManager.Instance.self.uid + "mysical"+type ,time);
	}
	public string getlasttime(string str){
		for(int i=0;i<updateData.Length;i++){
			if(updateData[i]==str){
				if(i==0)return updateData[updateData.Length-1];
				return updateData[i-1];
			}
		}
		return updateData[0];
	}
}
