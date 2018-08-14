using UnityEngine;
using System.Collections;

// 战斗结束Exp显示控制
//@author 李程

public class GetExpCtrl : MonoBase
{
	public const int TYPE_BEAST = 1;
	public const int TYPE_CARD = 2;

	/** 经验 */
	public UILabel label;
	/** vip经验 */
	public UILabel viplabel;
	/** 等级 */
	public UILabel lvLabel;
	/** 战力 */
	public UILabel combatLabel;
	CharacterCtrl character;
	float TextTime = 0.1f;
	/** 目标经验 */
	int vipExpMax;
	/** 目标经验 */
	int expMax;
	float delayDead = 0.5f;
	/** 当前显示经验 */
	long exp;
	int step;
	int stepVip;
	/** 卡片类型:1女神，2卡片 */
	int cardtype;
	/** 经验加成 */
	float expAdd;
	/** 提升等级 */
	int lv;
	/** 提升战力 */
	int combat;
	int phase = 0; //0计算普通exp  1: 播放vip label 2:计算vip exp加成 3:计算提升等级和战力 4:计算完成
	public Vector2 scaleFromTo = new Vector2 (0.2f, 1f);

	/// <summary>
	/// 初始化信息
	/// </summary>
	/// <param name="_type">1卡片 2女神.</param>
	/// <param name="target">Target.</param>
	/// <param name="_newExp">_new exp.</param>
	/// <param name="_lv">_lv.</param>
	/// <param name="_combat">_combat.</param>
	public void init (int _type, CharacterCtrl target, int _newExp, int _lv, int _combat)
	{
		cardtype = _type;
		expAdd = 0;
		character = target;
		vipExpMax = _newExp;
		lv = _lv;
		combat = _combat;

		if (_type == TYPE_CARD)
			expAdd += GuildManagerment.Instance.getSkillAddExpPorCardPve () * 0.01f;
		else if (_type == TYPE_BEAST)
			expAdd += GuildManagerment.Instance.getSkillAddExpPorBeastPve () * 0.01f;

		if (UserManager.Instance.self.getVipLevel () > 0) {
			expAdd += VipManagerment.Instance.getVipbyLevel (UserManager.Instance.self.getVipLevel ()).privilege.expAdd * 0.0001f;
			expMax = (int)((float)_newExp / (1 + expAdd));
			stepVip = (int)((float)(_newExp - expMax) / (TextTime * 53));
			
			if (stepVip < 1)
				stepVip = 1;
		} else {
			expMax = vipExpMax;
		}
		step = (int)((float)expMax / (TextTime * 53));
		if (step < 1)
			step = 1;

		if (expMax == 0) {
			Card tmpCard = character.characterData.role;
			//达到自身等级上限（非主角等级限制）且未进化满10次的卡片在获得经验时飘字： 需进化
			if (tmpCard != null && !EvolutionManagerment.Instance.isMaxEvoLevel (tmpCard) && tmpCard.isMaxLevel ()) {
				label.text = LanguageConfigManager.Instance.getLanguage ("Evo19");
			}
			//如果卡片确实已经达到了进化10次的上限，则飘字提示 已满级
			else if (tmpCard != null && EvolutionManagerment.Instance.isMaxEvoLevel (tmpCard) && tmpCard.isMaxLevel ()) {
				label.text = LanguageConfigManager.Instance.getLanguage ("Evo20");
            } 
            else if (tmpCard != null && cardtype == TYPE_BEAST)
            {
                BeastEvolve tmp = BeastEvolveManagerment.Instance.getBeastEvolveBySid(tmpCard.sid);//StorageManagerment.Instance.getBeast(tmpCard.uid);
                if (tmp.getBeast().isMaxLevel() && tmp.getBeast().getLevel() == 125)
                    label.text = LanguageConfigManager.Instance.getLanguage("Evo20");
                else if (tmp.getBeast().isMaxLevel() && tmp.getBeast().getLevel() != 125)
                {
                    label.text = LanguageConfigManager.Instance.getLanguage("Evo19");
                }
            }
		}
	}
	
	// Use this for initialization
	void Start ()
	{
		Card tmpCard = character.characterData.role;
		if (cardtype == TYPE_CARD) {
			if (UserManager.Instance.self.getVipLevel () > 0 || GuildManagerment.Instance.getSkillAddExpPorCardPve () > 0) {
				if(ServerTimeKit.getSecondTime() < BackPrizeLoginInfo.Instance.endTimes)// 双倍经验期间//
				{
					viplabel.text = LanguageConfigManager.Instance.getLanguage ("EXPADD") + (2 + expAdd);
				}
				else
				{
					if(expAdd == 0 || (tmpCard != null && EvolutionManagerment.Instance.isMaxEvoLevel (tmpCard) && tmpCard.isMaxLevel ()))
					{
						viplabel.text = "";
					}
					else
					{
						viplabel.text = LanguageConfigManager.Instance.getLanguage ("EXPADD") + (1 + expAdd);
					}
				}
			}
		    else
			{
				if(ServerTimeKit.getSecondTime() < BackPrizeLoginInfo.Instance.endTimes)// 双倍经验期间//
				{
					viplabel.text = LanguageConfigManager.Instance.getLanguage ("EXPADD") + (2 + expAdd);
				}
				else
				{
					if(expAdd == 0 || (tmpCard != null && EvolutionManagerment.Instance.isMaxEvoLevel (tmpCard) && tmpCard.isMaxLevel ()))
					{
						viplabel.text = "";
					}
					else
					{
						viplabel.text = LanguageConfigManager.Instance.getLanguage ("EXPADD") + (1 + expAdd);
					}
				}
			}
		} else if (cardtype == TYPE_BEAST) {
			if (UserManager.Instance.self.getVipLevel () > 0 || GuildManagerment.Instance.getSkillAddExpPorBeastPve () > 0) {
				if(ServerTimeKit.getSecondTime() < BackPrizeLoginInfo.Instance.endTimes)// 双倍经验期间//
				{
					viplabel.text = LanguageConfigManager.Instance.getLanguage ("EXPADD") + (2 + expAdd);
				}
				else
				{
					if(expAdd == 0 || (tmpCard != null && EvolutionManagerment.Instance.isMaxEvoLevel (tmpCard) && tmpCard.isMaxLevel ()))
					{
						viplabel.text = "";
					}
					else
					{
						viplabel.text = LanguageConfigManager.Instance.getLanguage ("EXPADD") + (1 + expAdd);
					}
				}
			}
			else
			{
				if(ServerTimeKit.getSecondTime() < BackPrizeLoginInfo.Instance.endTimes)// 双倍经验期间//
				{
					viplabel.text = LanguageConfigManager.Instance.getLanguage ("EXPADD") + (2 + expAdd);
				}
				else
				{
					if(expAdd == 0 || (tmpCard != null && EvolutionManagerment.Instance.isMaxEvoLevel (tmpCard) && tmpCard.isMaxLevel ()))
					{
						viplabel.text = "";
					}
					else
					{
						viplabel.text = LanguageConfigManager.Instance.getLanguage ("EXPADD") + (1 + expAdd);
					}
				}
			}
		}
	}
	
	void moveUpdate (Vector3 Pos)
	{
		transform.localPosition = Pos; 
	}
	
	void scaleUpdate (Vector3 scleP)
	{
		transform.localScale = scleP; 
	}

	void DoOver ()
	{
 
	}

	IEnumerator playVip ()
	{
		viplabel.gameObject.GetComponent<Animation> ().Play ();
		yield return new WaitForSeconds (0.2f);
		phase = 2;
	}

	// Update is called once per frame
	void Update ()
	{ 
		changeText ();


		if (phase == 4) {

			delayDead -= Time.deltaTime;
			if (delayDead <= 0)
				EffectManager.Instance.removeEffect (this); 
		}
			
	}

	IEnumerator lvUpEffect ()
	{
		yield return new WaitForSeconds (0.2f);
		if (lv > 0) {
			label.gameObject.SetActive (false);
			viplabel.gameObject.SetActive (false);
			lvLabel.gameObject.SetActive (true);
			lvLabel.text = "Lv +" + lv;
			TweenScale ts2 = TweenScale.Begin (lvLabel.gameObject, 0.3f, new Vector3 (0.8f, 0.8f, 1));
			ts2.method = UITweener.Method.EaseIn;
			ts2.from = new Vector3 (3, 3, 1);
			
			EventDelegate.Add (ts2.onFinished, () => {
				combatLabel.gameObject.SetActive (true);
				combatLabel.text = LanguageConfigManager.Instance.getLanguage ("s0369") + " + " + combat;
				TweenScale ts = TweenScale.Begin (combatLabel.gameObject, 0.3f, new Vector3 (1.5f, 1.5f, 1));
				ts.method = UITweener.Method.EaseIn;
				ts.from = new Vector3 (5, 5, 1);
				EventDelegate.Add (ts.onFinished, () => {
					StartCoroutine (Utils.DelayRun(()=>{
						lvLabel.gameObject.SetActive (false);
						combatLabel.gameObject.SetActive (false);
						phase = 4;
					},1f));
				}, true);
			}, true);
		} else {
			phase = 4;
		}

	}
	
	void changeText ()
	{
		//0计算普通exp  1: 播放vip label 2:计算vip exp加成 3:计算提升等级和战力 4:计算完成
		if (phase == 0) {
			if (TextTime > 0) {
				TextTime -= Time.deltaTime;
				exp += step;

				if (exp >= expMax)
					exp = expMax;
			
				transform.localPosition += new Vector3 (0, 0, 0.1f);
			} else {
				exp = expMax;
				phase = 1;
				TextTime = 0.3f;
				StartCoroutine (playVip ());
			}

		} else if (phase == 1) {
			return;
		} else if (phase == 2) {
			if (UserManager.Instance.self.getVipLevel () == 0) {
				phase = 3;
				StartCoroutine (lvUpEffect ());
				return;
			}

			if (TextTime > 0) {
				TextTime -= Time.deltaTime;
				exp += stepVip;
				if (exp >= vipExpMax)
					exp = vipExpMax;
			} else {
				exp = vipExpMax;
				phase = 3;
				StartCoroutine (lvUpEffect ());
			}
		}

		if (label.gameObject.activeSelf && expMax != 0) {
			label.text = "Exp " + exp;
		} 
	}
	
	
}
