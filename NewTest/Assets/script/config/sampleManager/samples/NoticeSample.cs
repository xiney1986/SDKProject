using UnityEngine;
using System.Collections;

/**公告模板   
 * 详见配置说明文件
 *@author 汤琦
 **/
public class NoticeSample : Sample {

	public NoticeSample () {

	}
	public int entranceId;
	public string icon;//图标
	public int[] timeLimit;//是否是限时活动 0非限时活动 或 [1限时活动,开始时间,结束时间]
	public string name;//公告名
	public string activiteDesc;//活动描述
	public int levelLimit;//等级限制
	public int order;//排列顺序; 在天梯争霸中sid 16，161，162，163 这个字段中分别代表 sid=15，17，18，19 的天梯争霸活动开启时间,  在福袋返利活动中代表返利间隔天数
	public int type;//活动类型
	public NoticeContent content;//公告内容 暂定，后期需求再根据不同活动实现
	//活动时间id
	public int timeID;



	#region server set config
	private void parse_icon ( string args ) {
		this.icon = args;
	}

	private void parse_name ( string args ) {
		this.name = args;
	}

	private void parse_type ( string args ) {
		this.type = StringKit.toInt (args);
	}

	private void parse_timeLimit ( string str ) {
		string[] strs = str.Split (',');
		if (strs.Length == 2) {
			timeLimit = new int[] { StringKit.toInt (strs[0]) };
			timeID = StringKit.toInt (strs[1]);
		}
		else if (strs.Length == 3) {
			timeLimit = new int[strs.Length];
			for (int i = 0; i < timeLimit.Length; i++) {
				timeLimit[i] = StringKit.toInt (strs[i]);
			}
		}
	}

	private void parse_content ( string args ) {
		content.parse (args);
	}
	#endregion



	public override void parse ( int sid, string str ) {

		this.sid = sid;
		string[] strArr = str.Split ('|');
		checkLength (strArr.Length, 9);
		this.entranceId = StringKit.toInt (strArr[1]);
		this.icon = strArr[2];
		parse_timeLimit (strArr[3]);
		this.name = strArr[4];
		this.activiteDesc = strArr[5];
		this.levelLimit = StringKit.toInt (strArr[6]);
		this.order = StringKit.toInt (strArr[7]);
		this.type = StringKit.toInt (strArr[8]);
		if(this.type == NoticeType.XIANSHI_FANLI)
		{
			RebateInfoManagement.Instance.rebateNoticeID = sid;
		}
		this.content = getContent (type);
		parse_content (strArr[9]);
	}

	//各自实现具体数据解析 暂时都是sids
	private NoticeContent getContent ( int noticeType ) {
		switch (noticeType) {
			case NoticeType.CONSUME_REBATE:
				return new NewExchangeNoticeContent ();
			case NoticeType.NEW_EXCHANGE:
				return new NewExchangeNoticeContent ();
			case NoticeType.REMAKE_EQUIP:
				return new EquipRemakeNoticeContent ();
			default:
				return new SidNoticeContent (this);
		}
	}

	public override void copy ( object destObj ) {
		base.copy (destObj);
	}
}