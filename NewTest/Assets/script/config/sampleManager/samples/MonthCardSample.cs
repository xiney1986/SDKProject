/**
 * 月卡配置模板
 * @author longlingquan
 * */
public class MonthCardSample
{

	//goods_id,1个月（30天）,20，奖励s
	public MonthCardSample(string str)
	{
		parse (str);
	}
	public string des;
	public int month;
	public int rmb;
	public int sid;

	private void parse (string str)
	{
		string[] strArr = str.Split (',');
		sid=StringKit.toInt(strArr[0]);
		des=strArr[1];
		rmb=StringKit.toInt(strArr[2]);
		month=StringKit.toInt(strArr[3]);
	}
}
