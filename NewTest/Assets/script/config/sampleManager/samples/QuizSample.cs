using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 问题实体
 * @author 陈世惟
 * */
public class QuizSample {

	public QuizSample () {

	}

	public int sid;
	/** 答案 */
	public string[] answer;
	/** 正确答案 */
	public int rightAnswer;
	/** 正确得分 */
	public int rightScore;
	/** 错误得分 */
	public int wrongScore;
	/** 问题 */
	public string question;


	public void parse (int id, string str)
	{
		this.sid = id; 
		string[] strArr = str.Split ('|');
		this.answer = strArr [1].Split ('#');
		this.rightAnswer = StringKit.toInt(strArr [2]);
		this.rightScore = StringKit.toInt(strArr [3]);
		this.wrongScore = StringKit.toInt(strArr [4]);
		this.question = strArr [5];
	}

}

/**
 * 题库实体
 * @author 陈世惟
 * */
public class ExamSample {
	public ExamSample () {
	}

	/** 题库ID（奖励SID） */
	public int quizId;
	/** 答题结束时间 */
	public int overTime;
	/** 是否领取奖励:1未领取|2已领取 */
	public int getAwardType;
	/** 答对数量 */
	public int righeAnswerNum;
	/** 积分 */
	public int score;
	/** 剩余答题sid集合(sid) */
	public List<int> quizSids;
	/** 题目总数 */
	public int questionCount;

	/** 删除已答题 */
	public void removeQuizBySid()
	{
		if (quizSids != null && quizSids.Count > 0) {
			quizSids.RemoveAt(0);
		}
	}

	/** 获得题目SID */
	public int getQuestionSid ()
	{
		if (quizSids == null || quizSids.Count <= 0) {
			return -1;
		}
		return quizSids[0];
	}

}

/**
 * 题库奖励实体
 * @author 陈世惟
 * */
public class QuizAwardSample {
	public QuizAwardSample (string str) {
		parse (str);
	}

	public int sid;
	/** 奖励类型（1答题，2调查） */
	public int type;
	/** 需求积分 */
	public int needScore;
	/** 正确答案 */
	public List<PrizeSample> prizes;
	
	
	public void parse (string str)
	{
		string[] strArr = str.Split ('|');
		this.sid = StringKit.toInt(strArr [0]);
		this.type = StringKit.toInt(strArr [1]);
		this.needScore = StringKit.toInt(strArr [2]);
		addPrizes(strArr [3]);
	}

	void addPrizes(string str)
	{
		string[] strArr = str.Split ('#');
		prizes = new List<PrizeSample>();
		if (strArr != null) {
			PrizeSample prize;
			for (int i = 0; i < strArr.Length; i++) {
				string[] strPrize = strArr[i].Split (',');
				prize = new PrizeSample(StringKit.toInt(strPrize[0]),StringKit.toInt(strPrize[1]),StringKit.toInt(strPrize[2]));
				prizes.Add(prize);
			}
		}
	}
}
