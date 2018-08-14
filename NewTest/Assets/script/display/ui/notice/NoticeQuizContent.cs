using UnityEngine;
using System.Collections.Generic;

public class NoticeQuizContent : MonoBase
{

	/** 常规组 */
	public GameObject NormalGroup;
	/** 回答组 */
	public GameObject AnswerGroup;
	/** 奖励展示组 */
	public GameObject AwardGroup;
	/** 常规-标题 */
	public UILabel normalTitle;
	/** 常规-日期 */
	public UILabel normalDate;
	/** 常规-规则 */
	public UILabel normalRule;
	/** 常规-状态 */
	public UILabel normalStatus;
	/** 常规-答题按钮 */
	public ButtonBase buttonAnswer;
	/** 回答-答题数量 */
	public UILabel answerTitle;
	/** 回答-问题 */
	public UILabel answerQuestion;
	/** 回答-当前得分 */
	public UILabel answerScore;
	/** 回答-回答按钮ABC */
	public ButtonBase[] buttonAnswerKey;
	/** 奖励-结果 */
	public UILabel awardTitle;
	/** 奖励-物品展示 */
	public DelegatedynamicContent awardContent;
	/** 奖励-领取按钮 */
	public ButtonBase buttonGetAward;
	/** Goods预制体 */
	public GameObject goodsPrefab;
	NoticeWindow faWin;
	QuizNotice notice;
	NoticeSample noticeSample;
	ExamSample examSample;
	QuizSample quizSample;
	QuizAwardSample quizAward;
	QuizManagerment instance = QuizManagerment.Instance;

	public void initData (NoticeWindow faWin, Notice notice)
	{
		this.faWin = faWin;
		this.notice = notice as QuizNotice;
		noticeSample = notice.getSample ();
		buttonAnswer.fatherWindow = faWin;
		buttonGetAward.fatherWindow = faWin;
		for (int i = 0; i < buttonAnswerKey.Length; i++) {
			buttonAnswerKey [i].fatherWindow = faWin;
		}

		//获取题库
		instance.getQuestions (() => {
			this.examSample = QuizManagerment.Instance.getExamSampleBySid (notice);
			if (examSample != null && examSample.getAwardType == 1 && examSample.getQuestionSid () == -1) {
				initAwardUI ();
			} else {
				initNormalUI ();
			}
		});
	}

	/** 显示规则 */
	void initNormalUI ()
	{
		NormalGroup.SetActive (true);
		AnswerGroup.SetActive (false);
		AwardGroup.SetActive (false);
		normalTitle.text = noticeSample.name;
		if (noticeSample.type == NoticeType.QUIZ_EXAM) {
			normalRule.text = LanguageConfigManager.Instance.getLanguage ("quizRule01").Replace ("~","\n");
		} else {
			normalRule.text = LanguageConfigManager.Instance.getLanguage ("quizRule02").Replace ("~","\n");
		}

		if (examSample != null) {
			if (examSample.getAwardType == 2 || examSample.getQuestionSid () == -1) {
				buttonAnswer.disableButton (true);
				if (examSample.getAwardType == 2 || examSample.getQuestionSid () == -1) {
					buttonAnswer.textLabel.text = LanguageConfigManager.Instance.getLanguage ("quiz10");
				}
			} else {
				buttonAnswer.textLabel.text = LanguageConfigManager.Instance.getLanguage ("quiz01");
			}
		} else {
			buttonAnswer.disableButton (true);
			buttonAnswer.textLabel.text = LanguageConfigManager.Instance.getLanguage ("quiz01");
		}


		normalDate.text = notice.getOpenTimeDesc ();
		normalStatus.text = notice.getTimeDesc ();
	}

	/** 显示答题 */
	void initAnswerUI ()
	{
		MaskWindow.UnlockUI ();
		NormalGroup.SetActive (false);
		AnswerGroup.SetActive (true);
		AwardGroup.SetActive (false);

		if (examSample == null) {
			return;
		}
		int sid = examSample.getQuestionSid ();
		string num = examSample.questionCount - examSample.quizSids.Count + 1 + "/" + examSample.questionCount;
		answerTitle.text = LanguageConfigManager.Instance.getLanguage ("quiz02", num);
		if (sid == -1) {
			initAwardUI ();
		} else {
			quizSample = QuizSampleManager.Instance.getQuizSampleBySid (examSample.getQuestionSid ());
		}

		answerQuestion.text = quizSample.question;
		answerScore.text = LanguageConfigManager.Instance.getLanguage ("quiz03", examSample.score.ToString ());

		for (int i = 0; i < quizSample.answer.Length; i++) {
			if (i >= buttonAnswerKey.Length) {
				break;
			}
			buttonAnswerKey [i].gameObject.SetActive (true);
			buttonAnswerKey [i].textLabel.text = quizSample.answer [i];
		}
	}

	/** 显示奖励 */
	void initAwardUI ()
	{
		MaskWindow.UnlockUI ();
		NormalGroup.SetActive (false);
		AnswerGroup.SetActive (false);
		AwardGroup.SetActive (true);

		if (noticeSample.type == NoticeType.QUIZ_EXAM) {
			awardTitle.text = LanguageConfigManager.Instance.getLanguage ("quiz07",
			                                                              examSample.questionCount.ToString (),
			                                                              examSample.righeAnswerNum.ToString (),
			                                                              "\n",
			                                                              examSample.score.ToString ());
		} else if (noticeSample.type == NoticeType.QUIZ_SURVEY) {
			awardTitle.text = LanguageConfigManager.Instance.getLanguage ("quiz08");
		}


		awardContent.fatherWindow = faWin;
		awardContent.SetUpdateItemCallback (onUpdateItem);
		awardContent.SetinitCallback (initItem);
		updateAwardContent ();
	}

	void updateAwardContent ()
	{
		if (noticeSample.type == NoticeType.QUIZ_EXAM) {
			quizAward = QuizAwardSampleManager.Instance.getQuizPrizesByScore (examSample.score);
		} else if (noticeSample.type == NoticeType.QUIZ_SURVEY) {
			quizAward = QuizAwardSampleManager.Instance.getQuizPrizeByCheck ();
		}

		if (quizAward == null) {
			awardContent.cleanAll ();
			return;
		}
		if (quizAward == null || examSample.getAwardType == 2) {
			buttonGetAward.disableButton (true);
		} else {
			buttonGetAward.disableButton (false);
		}
		awardContent.reLoad (quizAward.prizes.Count);
	}

	GameObject onUpdateItem (GameObject item, int i)
	{
		if (item == null) {
			item = NGUITools.AddChild (awardContent.gameObject, goodsPrefab);
		}
		
		GoodsView button = item.GetComponent<GoodsView> ();
		button.fatherWindow = faWin;
		button.init (quizAward.prizes [i]);
		
		return item;
	}
	GameObject initItem (  int i)
	{
	 
			GameObject		item = NGUITools.AddChild (awardContent.gameObject, goodsPrefab);
		GoodsView button = item.GetComponent<GoodsView> ();
		button.fatherWindow = faWin;
		button.init (quizAward.prizes [i]);
		
		return item;
	}
	public void clickButton (GameObject gamObj)
	{
		if (gamObj.name == "Quiz_buttonAnswer") {
			initAnswerUI ();
		} else if (gamObj.name == "Quiz_buttonGetAward") {
			if (noticeSample.type == NoticeType.QUIZ_EXAM) {
				getAwardFP ();
				return;
			}
			if (AllAwardViewManagerment.Instance.isFull (quizAward.prizes)) {
				TextTipWindow.Show (Language ("laddersTip_08"));
				MaskWindow.UnlockUI ();
			}
			if (quizAward != null) {
				getAwardFP ();
			} else {
				buttonGetAward.disableButton (true);
				MaskWindow.UnlockUI ();
			}
		} else if (gamObj.name == "Quiz_buttonKeyA") {
			answerFP (1);
		} else if (gamObj.name == "Quiz_buttonKeyB") {
			answerFP (2);
		} else if (gamObj.name == "Quiz_buttonKeyC") {
			answerFP (3);
		}
	}
	
	void answerFP (int buttonId)
	{
		NoticeQuizFPort fport = FPortManager.Instance.getFPort ("NoticeQuizFPort") as NoticeQuizFPort;
		fport.answer (examSample.quizId, buttonId, (int num) => {

			//系统调查没有正确答案，直接下一题
			if (num == -1) {
				instance.updateExamSampleScore (notice, 0, false);
				initAnswerUI ();
			} else if (num == 1 && buttonId == quizSample.rightAnswer) {
				instance.updateExamSampleScore (notice, quizSample.rightScore, true);
				string str = LanguageConfigManager.Instance.getLanguage ("quiz04", "\n", quizSample.rightScore.ToString ());
				UiManager.Instance.createMessageWindowByOneButton (str, (MessageHandle msg) => {
					this.examSample = QuizManagerment.Instance.getExamSampleBySid (notice);
					initAnswerUI ();
				});
			} else {
				instance.updateExamSampleScore (notice, quizSample.wrongScore, false);
				string rightAnswer = "";
				if (quizSample.rightAnswer == 1) {
					rightAnswer = "A";
				} else if (quizSample.rightAnswer == 2) {
					rightAnswer = "B";
				} else if (quizSample.rightAnswer == 3) {
					rightAnswer = "C";
				}
				string str = LanguageConfigManager.Instance.getLanguage ("quiz05", rightAnswer, "\n", quizSample.wrongScore.ToString ());
				UiManager.Instance.createMessageWindowByOneButton (str, (MessageHandle msg) => {
					this.examSample = QuizManagerment.Instance.getExamSampleBySid (notice);
					initAnswerUI ();
				});
			}

		});
	}

	void getAwardFP ()
	{
		NoticeQuizFPort fport = FPortManager.Instance.getFPort ("NoticeQuizFPort") as NoticeQuizFPort;
		fport.getAward (examSample.quizId, () => {
			TextTipWindow.Show (LanguageConfigManager.Instance.getLanguage ("s0120"));
			instance.updateExamSampleAwardType (notice, true);
			buttonGetAward.disableButton (true);
			this.examSample = QuizManagerment.Instance.getExamSampleBySid (notice);
			initNormalUI ();
		});
	}
}
