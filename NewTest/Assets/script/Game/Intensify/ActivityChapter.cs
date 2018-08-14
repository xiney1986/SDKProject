using System;

/**
 * 活动副本章节实体对象
 * @author huangzhenghan
 * */
public class ActivityChapter:Chapter
{
	protected int timeId;
	protected long[] times;//开始时间和结束时间
	protected int maxNum;//最大次数
	protected TimeLimit timeLimit ;
	private ActiveTime activeTime;
	private int[] days;//开启的周 比如 1 2 3 4 5

	public ActivityChapter (int sid):base(sid)
	{
	}




	/** 更新最大次数 */
	public void updateMaxNum (int _num)
	{
		this.maxNum = _num;
	}
    public int getNum()
    {
        //最大次数-打过的次数（买一次后台会把这个次数-1）
        return maxNum - num;
    }

	public int getActiveMaxNum ()
	{
		return maxNum;
	}

	public int getActiveNum ()
	{
		return maxNum - num >= 0 ? maxNum - num : 0;
        //return num;
	}
	
	public  void initTime (int _timeId)
	{
		timeId = _timeId;
		timeLimit = new TimeLimit (null);
		TimeInfoSample tsample = TimeConfigManager.Instance.getTimeInfoSampleBySid (timeId);
		days=tsample.mainTimeInfoo;
		activeTime = ActiveTime.getActiveTimeByType (tsample);
		activeTime.initTime (ServerTimeKit.getSecondTime ());
		times = new long[] {
			activeTime.getDetailStartTime (),
			activeTime.getDetailEndTime ()
		};
		updateTime ();
	}
	
	public  void updateTime ()
	{
		TimeLimitManagerment.updateTimeLimit (timeLimit, times);
	}
	/// <summary>
	/// 图片颜色
	/// </summary>
	/// <returns><c>true</c>, if color was iconed, <c>false</c> otherwise.</returns>
	public bool iconColor(){
		if(timeLimit.type == TimeLimit.FRONT){
			return false;
		}
		return true;
	}

	//获得时间描述
	public  string getTimeDesc ()
	{
		return timeLimit.toString ();
	}
	//获得时间
	public  long getTime ()
	{
		return timeLimit.time;
	}
	//获得开放的具体星期数 描述
	public string getTodyDec(){
		string str="";
		for(int i=0;i<days.Length;i++){
			if(days[i]==timeLimit.today){
				//days[i]=days[i]+10;
				if(i==days.Length-1)str+="[FF0000]"+LanguageConfigManager.Instance.getLanguage("total_"+(days[i]).ToString())+"[-]";
				else str+="[FF0000]"+LanguageConfigManager.Instance.getLanguage("total_"+(days[i]).ToString())+"[-],";
			}else{
				if(i==days.Length-1)str+="[FFF6DA]"+LanguageConfigManager.Instance.getLanguage("total_"+(days[i]).ToString())+"[-]";
				else str+="[FFF6DA]"+LanguageConfigManager.Instance.getLanguage("total_"+(days[i]).ToString())+"[-],";
			}
		}
		str=LanguageConfigManager.Instance.getLanguage("warChapterL01",str);
		return str;
	}
	//是否开启
	public override bool isOpen ()
	{
		if (timeLimit.type == TimeLimit.FRONT)
			return false;
		else
			return true;
	}

}