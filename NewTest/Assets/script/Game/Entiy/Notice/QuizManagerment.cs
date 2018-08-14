using UnityEngine;
using System.Collections.Generic;

/**
 * 答题管理器
 * @author 陈世惟
 * */
public class QuizManagerment
{

	public QuizManagerment () { 
	}
	public List<ExamSample> examSamples;
	
	public static QuizManagerment Instance {
		get{ return SingleManager.Instance.getObj ("QuizManagerment") as QuizManagerment;}
	}

	/** 根据题目类型获得题库及答题信息 */
	public ExamSample getExamSampleBySid (Notice _notice)
	{
		if (examSamples == null || examSamples.Count == 0 || _notice == null) {
			return null;
		}
		int quizId = getQuizIndex (_notice);
		for (int i = 0; i < examSamples.Count; i++) {
			if (examSamples[i].quizId == quizId) {
				return examSamples[i];
			}
		}
		return null;
	}

	/** 更新答题信息 */
	public void updateExamSampleScore (Notice _notice,int score,bool isRight)
	{
		if (examSamples == null || examSamples.Count == 0) {
			return;
		}
		int quizId = getQuizIndex (_notice);
		for (int i = 0; i < examSamples.Count; i++) {
			if (examSamples[i].quizId == quizId) {
				examSamples[i].removeQuizBySid ();
				examSamples[i].score += score;
				if (isRight) {
					examSamples[i].righeAnswerNum++;
				}
			}
		}
	}

	/** 更新答题奖励领取信息 */
	public void updateExamSampleAwardType (Notice _notice,bool isGet)
	{
		if (examSamples == null || examSamples.Count == 0) {
			return;
		}
		int quizId = getQuizIndex (_notice);
		for (int i = 0; i < examSamples.Count; i++) {
			if (examSamples[i].quizId == quizId) {
				if (isGet) {
					examSamples[i].getAwardType = 2;
				} else {
					examSamples[i].getAwardType = 1;
				}
			}
		}
	}

	/** 获得指定答题活动的题目类型 */
	public int getQuizIndex (Notice _notice)
	{
		NoticeSample noticeSample = _notice.getSample();
		int[] sids = (noticeSample.content as SidNoticeContent).sids;
		return sids[0];
	}

	/** 初始化题库数据 */
	public void getQuestions(CallBack _callback)
	{
		NoticeQuizFPort fport = FPortManager.Instance.getFPort ("NoticeQuizFPort") as NoticeQuizFPort;
		fport.getQuestions (_callback);
	}

	/** 初始化后台题库数据 */
	public void initQuiz (ErlKVMessage message,CallBack _callback)
	{
		ErlType msg = message.getValue ("msg") as ErlType;
		
		if (msg is ErlArray && msg != null) {
			
			ErlArray arr = msg as ErlArray;
			
			ErlArray arr2;
			examSamples = new List<ExamSample>();
			ExamSample sample = null;
			for (int i = 0; i < arr.Value.Length; i++) {
				if (arr.Value[i] is ErlArray) {
					arr2 = arr.Value[i] as ErlArray;
					sample = new ExamSample();
					sample.quizId = StringKit.toInt (arr2.Value [0].getValueString ());//题库ID
					sample.overTime = StringKit.toInt (arr2.Value [1].getValueString ());//答题结束时间
					sample.getAwardType = StringKit.toInt (arr2.Value [2].getValueString ());//是否领取奖励
					sample.righeAnswerNum = StringKit.toInt (arr2.Value [3].getValueString ());//答对数量
					sample.score = StringKit.toInt (arr2.Value [4].getValueString ());//积分
					
					ErlArray arr3 = arr2.Value [5] as ErlArray;//剩余题目sid组
//					int index;
					int sid;
					List<int> sids = new List<int>(arr3.Value.Length);
					for (int j=0; j <arr3.Value.Length; j++) {
//						index = StringKit.toInt(((arr3.Value [j] as ErlArray).Value [0] as ErlType).getValueString ());
						sid = StringKit.toInt(((arr3.Value [j] as ErlArray).Value [1] as ErlType).getValueString ());
						sids.Add(sid);
					}
					sample.quizSids = sids;
					sample.questionCount = StringKit.toInt (arr2.Value [6].getValueString ());//总数
					examSamples.Add(sample);
				}
			}

		} else {
			MonoBase.print (GetType () + "==error:" + msg);
		}

		if (_callback != null)
			_callback ();
	}
}
