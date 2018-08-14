using UnityEngine;
using System.Collections;

public class BackRechargeBarCtrl : MonoBase
{
	public ButtonBackRechargeReceive receiveButton;
	public Recharge recharge;
	public RechargeSample res;
	public UILabel costValue;
	public UILabel timeLabel;
	public UISprite costIcon;
	public UISprite background;
	public UILabel titleLabel;
	public UILabel rechargeExplain;
	public UILabel rechargeValue;
	public UILabel costExplain;
    public UILabel oneTipLabel;
	public GameObject showAwardPos;
	public GameObject goodsViewPre;
	private Notice notice;
	private NoticeSample sample;
	private GoodsView[] awardButtons;
	public BackRechargeContent fatherWindow;
	public BackRecharge backRecharge;

	private string parseDesc (string desc, int condition) {
		if (string.IsNullOrEmpty (desc))
			return "";
		return desc.Replace ("%1", condition.ToString ());
	}
	public void updateItem (BackRecharge br,NoticeSample noticeSample, Notice notice)
	{
		backRecharge = br;
		res = br.getRechargeSample();
		this.notice = notice;
		this.sample = noticeSample;
		changeButton();
		receiveButton.fatherWindow = fatherWindow.win;
		receiveButton.content = fatherWindow;
		receiveButton.updateButton (br);
		setItemText();
		if (awardButtons == null) {
			awardButtons = new GoodsView[4];
			for (int i = 0; i < awardButtons.Length; i++) {
				awardButtons [i] = NGUITools.AddChild (showAwardPos, goodsViewPre).GetComponent<GoodsView> ();
				awardButtons [i].transform.localPosition = new Vector3 (i * 120, 0, 0);
				awardButtons [i].fatherWindow = fatherWindow.win;
				awardButtons [i].gameObject.SetActive (false);
			}

			//显示充值奖励内容 位移差X=120
			for (int i = 0; i < res.prizes.Length && i < 4; i++) {
				awardButtons [i].gameObject.SetActive (true);
				awardButtons [i].init (res.prizes [i]);
			}
		} else {
			for (int i = 0; i < awardButtons.Length; i++) {
				awardButtons [i].gameObject.SetActive (false);
			}
			for (int i = 0; i < res.prizes.Length && i < 4; i++) {
				awardButtons [i].gameObject.SetActive (true);
				awardButtons [i].init (res.prizes [i]);
			}
		}
	}

	private void setItemText()
	{
		costValue.gameObject.SetActive (false);
		costExplain.gameObject.SetActive (false);
		costIcon.gameObject.SetActive (false);
		if(backRecharge.state == BackRechargeState.recevied)
		{
			//titleLabel.text = parseDesc (res.desc, res.condition / 10) + "(0/" + res.count + ")";
			titleLabel.text = parseDesc (res.desc, res.condition) + "(0/" + res.count + ")";
		}
		else
		{
			//titleLabel.text = parseDesc (res.desc, res.condition / 10) + "(" + res.count + "/" + res.count + ")";
			titleLabel.text = parseDesc (res.desc, res.condition) + "(" + res.count + "/" + res.count + ")";
		}

		if (backRecharge.state == BackRechargeState.recevied) {
			rechargeExplain.text = LanguageConfigManager.Instance.getLanguage ("s0209");
			rechargeValue.gameObject.SetActive(false);
			receiveButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("recharge02");
			receiveButton.disableButton (true);
		}
		else {
			int num = 0;
			//if (res.condition / 10 - backRecharge.num <= 0) {
			if (res.condition - backRecharge.num <= 0) {
				num = 0;
			}
			else {
				//num = res.condition / 10 - backRecharge.num;
				num = res.condition - backRecharge.num;
			}
			receiveButton.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0309");
			rechargeExplain.text = LanguageConfigManager.Instance.getLanguage ("notice24");
			rechargeValue.gameObject.SetActive(true);
			rechargeValue .text = (num).ToString () + LanguageConfigManager.Instance.getLanguage ("notice23");
		}
	}
	void changeButton ()
	{
		if (receiveButton == null)
			return;
		//if(BackPrizeRechargeInfo.Instance.rechargeNum >= res.condition / 10 && backRecharge.state != BackRechargeState.recevied)// 累计充值达到条件//
		if(BackPrizeRechargeInfo.Instance.rechargeNum >= res.condition && backRecharge.state != BackRechargeState.recevied)// 累计充值达到条件//
		{
			receiveButton.disableButton (false);
		}
		else
		{
			receiveButton.disableButton (true);
		}
	}
}
