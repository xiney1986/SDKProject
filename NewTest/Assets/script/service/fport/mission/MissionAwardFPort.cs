using UnityEngine;
using System.Collections.Generic;

public class MissionAwardFPort : BaseFPort {

	public const int TYPE_INFO = 1;//初始化
	public const int TYPE_GET = 2;//领奖
	private CallBack callback;
	private int sendType;
	private int chapterSid;
	private int awardSid;

	/// <summary>
	/// 初始化信息
	/// </summary>
//	public void getMissionInfo (int chapterSid, CallBack callback)
//	{
//		this.callback = callback;
//		sendType = TYPE_INFO;
//		ErlKVMessage message = new ErlKVMessage (FrontPort.MISSIONAWARD_INFO);
//		message.addValue("chapter",new ErlInt(chapterSid));//chapter=sid,all=0
//		access (message);
//	}

	/// <summary>
	/// 初始化信息
	/// </summary>
	public void getMissionInfo (CallBack callback)
	{
		this.callback = callback;
		sendType = TYPE_INFO;
		ErlKVMessage message = new ErlKVMessage (FrontPort.MISSIONAWARD_INFO);
		message.addValue("chapter",new ErlInt(0));//chapter=sid,all=0
		access (message);
	}

	/// <summary>
	/// 领奖
	/// </summary>
	public void getMissionAward (int _chapterSid, int _awardSid, CallBack callback)
	{
		this.callback = callback;
		this.chapterSid = _chapterSid;
		this.awardSid = _awardSid;
		sendType = TYPE_GET;
		ErlKVMessage message = new ErlKVMessage (FrontPort.MISSIONAWARD_GET);
		message.addValue("award_sid",new ErlInt(_awardSid));
		access (message);
	}
	public override void read (ErlKVMessage message)
	{
		base.read (message);
		
		if(sendType == TYPE_INFO) {
			
			ErlType msg = message.getValue ("msg") as ErlType;
			
			if (msg is ErlArray) {
				ErlArray array = msg as ErlArray;
				
				if (array == null)
					return ;

				List<ChapterAwardServerSample> list = new List<ChapterAwardServerSample>();
				for (int i = 0; i < array.Value.Length; i++) {
					ChapterAwardServerSample info = new ChapterAwardServerSample();
					info.chapterSid = StringKit.toInt (((array.Value [i] as ErlArray).Value[0] as ErlType).getValueString ());

					ErlArray one = (array.Value [i] as ErlArray).Value[1] as ErlArray;
					info.awardSids = new int[one.Value.Length];
					for (int j = 0; j < one.Value.Length; j++) {
						info.awardSids[j] = StringKit.toInt (one.Value [j].getValueString ());
					}
					list.Add(info);
				}

				FuBenManagerment.Instance.getChapterAwardSeverSampleBySid(list);
			}
			else {
				MonoBase.print (GetType () + "error:"+msg);
			}
			if (callback != null)
				callback ();
			callback = null;
		}

		else if(sendType == TYPE_GET) {

			string str = (message.getValue ("msg") as ErlAtom).Value;
			
			switch(str){
			case "ok":
                //UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("s0120"));
				ChapterAwardServerSample item = new ChapterAwardServerSample();
				item.chapterSid = chapterSid;
				item.awardSids = new int[1]{awardSid};
				FuBenManagerment.Instance.addChapterAwardSeverSample(item);
				if (callback != null)
					callback ();
				chapterSid = 0;
				awardSid = 0;
				callback = null;
				break;
			}
		}
	}
}
