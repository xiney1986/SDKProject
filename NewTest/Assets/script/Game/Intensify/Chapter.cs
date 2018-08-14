using System;
 
/**
 * 副本章节实体对象
 * @author longlingquan
 * */
public class Chapter
{
	public int sid;
	public int num;//当前已使用次数
	private int buyed;//当前已购买次数
    public bool isattack = true;//今日是否可以挑战
    public int reAttackNum = 0;//今日重置次数
    public int lotteryMaxNum = 0;//每日最大挑战次数
    public int reAttackMaxNum = 0;//每日最大可重置次数
    public int relotteryNum=0;//今日已经开宝箱的次数
    public int relotteryBuyNum = 0;//今日已经购买开宝箱次数

    public int Buyed { get { return this.buyed; }}

	public Chapter (int sid)
	{
		this.sid = sid;
//		update (ChapterSampleManager.Instance.getChapterSampleBySid (sid).num);
	}

	//可购买次数
	public int getCanBuyNum ()
	{
		if (UserManager.Instance.self.getVipLevel () <= 0)
			return 0;
		else {
			int a = VipManagerment.Instance.getVipbyLevel (UserManager.Instance.self.getVipLevel ()).privilege.bossCountBuyAdd;
			return (a - buyed) > 0 ? (a - buyed) : 0;
		}
	}

	//可用次数=最大次数+已购买次数-已使用次数
	public int getNum ()
	{
		return  getMaxNum () + buyed - num;
	}

	public void update (int num)
	{
		this.num = num;

	}

	public void update (int num, int buyed)
	{
		this.num = num;
		this.buyed = buyed;
	}
    public void updateTower(int num,int reNum,int lonum,int lobuy) {
        isattack = num==1;//0就代表不可以挑战了
        reAttackNum = reNum;
        relotteryNum=lonum;
        relotteryBuyNum = lobuy;
        ClmbTowerManagerment.Instance.relotteryNum = lonum; ;
        ClmbTowerManagerment.Instance.relotteryBuyNum = lobuy;
        int[] maxs = CommandConfigManager.Instance.getTowerMaxNum();
        lotteryMaxNum = maxs[0];
        reAttackMaxNum = maxs[1];
    }

	//购买讨伐次数成功
	public void addBuyed ()
	{
		this.buyed++;
	}
	//购买讨伐次数成功
	public void addBuyed (int buyCount)
	{
		this.buyed += buyCount;
	}

	//讨伐成功
	public void costNum ()
	{
		this.num++;
	}
	
	public void addNum (int num)
	{
		this.num -= num;
	}

	//获得配置最大数量  有vip以后这个地方会变
	public int getMaxNum ()
	{
		int vipNum = 0;
		Vip _vip = VipManagerment.Instance.getVipbyLevel (UserManager.Instance.self.getVipLevel ());
		switch (getChapterType ()) {
		case ChapterType.WAR:
			vipNum = _vip == null ? 0 : _vip.privilege.bossCountAdd;
			break;
		default:
			break;
		}
		return ChapterSampleManager.Instance.getChapterSampleBySid (sid).num + vipNum;
	}
	
	//获得章节类型
	public int getChapterType ()
	{
		return ChapterSampleManager.Instance.getChapterSampleBySid (sid).type;
	}
	
	//获得章节描述
	public string getDescript ()
	{
		return ChapterSampleManager.Instance.getChapterSampleBySid (sid).describe;
	}
	
	//获得星星奖励
	public ChapterAwardSample[] getChapterAward ()
	{
		return ChapterSampleManager.Instance.getChapterSampleBySid (sid).prizes;
	}
	//获得章节缩略图
	public int getThumbIconID ()
	{
		return ChapterSampleManager.Instance.getChapterSampleBySid (sid).thumbIcon;
	}
	//获得章节名字
	public string getChapterName ()
	{
		return ChapterSampleManager.Instance.getChapterSampleBySid (sid).name;
	}


	
	//是否开启
	public virtual bool isOpen ()
	{
		return true;
	}
	public int getReBuyNum(){
		return buyed;
	}
    public void setReBuyNum(int num){
        buyed=num;
    }

} 

