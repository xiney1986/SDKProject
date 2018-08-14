using UnityEngine;
using System.Collections.Generic;

/**
 * 答题奖励管理器
 * @author 陈世惟
 * */
public class QuizAwardSampleManager : SampleConfigManager
{
	private static QuizAwardSampleManager instance;
	private List<QuizAwardSample> quizPrizes;

	public static QuizAwardSampleManager Instance {
		get {
			if (instance == null)
				instance = new QuizAwardSampleManager ();
			return instance;
		}
	}
	
	public QuizAwardSampleManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_QUIZAWARD);
	}
	
	public override void parseConfig (string str)
	{
		QuizAwardSample be = new QuizAwardSample (str);
		if (quizPrizes == null)
			quizPrizes = new List<QuizAwardSample> ();
		quizPrizes.Add (be);
	}

	/** 根据积分获得答题奖励 */
	public QuizAwardSample getQuizPrizesByScore (int score)
	{
		if (quizPrizes == null || quizPrizes.Count == 0) {
			return null;
		}
		for (int i = 0; i < quizPrizes.Count; i++) {
			if (quizPrizes [i].type == 1 && score >= quizPrizes [i].needScore) {
				return quizPrizes [i];
			}
		}

		return null;
	}

	/** 获得调查奖励 */
	public QuizAwardSample getQuizPrizeByCheck ()
	{
		if (quizPrizes == null || quizPrizes.Count == 0) {
			return null;
		}
		for (int i = 0; i < quizPrizes.Count; i++) {
			if (quizPrizes [i].type == 2) {
				return quizPrizes [i];
			}
		}
		return null;
	}
}
