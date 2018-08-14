using UnityEngine;
using System.Collections;

public class NoticeQuizFPort : BaseFPort {

	private const int TYPE_GETQUESTIONS = 1, TYPE_ANSWER = 2, TYPE_GETAWARD = 3;
	private CallBack callback;
	private CallBack<int> callInt;
	private int type;

	/// <summary>
	/// 获取问题
	/// </summary>
	public void getQuestions (CallBack _callback)
	{   
		type = TYPE_GETQUESTIONS;
		this.callback = _callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.QUIZ_GETQUESTIONS);
		access (message);
	}

	/// <summary>
	/// 回答问题
	/// </summary>
	/// <param name="sid">这组题目的sid.</param>
	/// <param name="answer">答案.</param>
	/// <param name="callback">Callback.</param>
	public void answer (int sid, int answer, CallBack<int> callback)
	{   
		type = TYPE_ANSWER;
		this.callInt = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.QUIZ_ANSWER);
		message.addValue ("sid", new ErlInt (sid));//题目SID
		message.addValue ("answer", new ErlInt (answer));//答案
		access (message);
	}

	/// <summary>
	/// 获取奖励
	/// </summary>
	/// <param name="sid">这组题目的sid.</param>
	/// <param name="callback">Callback.</param>
	public void getAward (int sid, CallBack callback)
	{   
		type = TYPE_GETAWARD;
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.QUIZ_GETAWARD);
		message.addValue ("sid", new ErlInt (sid));//题库SID
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		if (type == TYPE_GETQUESTIONS) {
			QuizManagerment.Instance.initQuiz(message,callback);
		}

		else if (type == TYPE_ANSWER) {

			string msg = (message.getValue ("msg") as ErlType).getValueString ();
			if (msg == "1" || msg == "2") {
				if (callInt != null) {
					callInt(StringKit.toInt(msg));
				}
			} else if (msg == "ok") {
				if (callInt != null) {
					callInt(-1);
				}
			} else {
				MaskWindow.UnlockUI ();
			}
		}

		else if (type == TYPE_GETAWARD) {
			string msg = (message.getValue ("msg") as ErlType).getValueString ();
			if (msg == "ok") {
				if (callback != null) {
					callback();
				}
			} else {
				MaskWindow.UnlockUI ();
				NGUIDebug.print ("NoticeQuizFPort.getAward==" + msg);
			}
		}
	}
}
