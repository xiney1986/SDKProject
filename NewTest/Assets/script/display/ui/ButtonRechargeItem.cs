using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonRechargeItem : ButtonBase {
	public UILabel rmbLabel;
	public UILabel rmbCoverNum;
	public UILabel rmbCoverNum1;
	public UILabel DiscLabel;
	public UILabel nameLabel;
	public UISprite titleIcon;
	public json_Goods goods;
	public bool isBanner;//banner置顶
	public GameObject doubleTag;
	private int rmbCoverNumber;
    private List<string> sidList;//已经有过的充值列表
	public override void DoClickEvent () {
		base.DoClickEvent ();
		MaskWindow.LockUI ();
		StartCoroutine (waitTime (1.5f));


		if (goods.type == 1) {
			MonthCardTimeFport fport = FPortManager.Instance.getFPort<MonthCardTimeFport> ();
			fport.apply ((timeResult) => {
                if (timeResult == 0)
                {
                    UiManager.Instance.openDialogWindow<MessageLineWindow>((win) =>
                    {
                        win.Initialize(LanguageConfigManager.Instance.getLanguage("recharge04"));
                    });
                }
                else
                {
                    if (isBanner)
                        return;

                    SdkManager.INSTANCE.Pay(goods.id, int.Parse(goods.amount));
                }
			});	
		}
		else {
			if (isBanner)
				return;

            SdkManager.INSTANCE.Pay(goods.id, int.Parse(goods.amount));
        }
	}

	private IEnumerator waitTime(float time)
	{
		yield return new WaitForSeconds (time);
		MaskWindow.UnlockUI ();
	}

	private IEnumerator test () {
		WWW w = FPortManager.Instance.getFPort<CashFPort> ().httpCash (((StringKit.toLong (UserManager.Instance.self.uid) << 32) >> 32).ToString (), "1", "1", StringKit.toInt (goods.amount));
		yield return w;
		fatherWindow.finishWindow ();
	}

    public void updateButton(json_Goods goods) {
        if (isBanner)
            return;
        if (sidList == null) {
            rmbCoverNumber = StringKit.toInt(goods.rate) * 3;
            doubleTag.SetActive(true);
        } else {
            if (sidList.Contains(goods.id)) {
                doubleTag.SetActive(false);
            } else {
                rmbCoverNumber = StringKit.toInt(goods.rate) * 3;
                if (goods.name != null && goods.type != 1 && goods.type != 2) doubleTag.SetActive(true);
            }
        }
        this.goods = goods;
        rmbLabel.text = LanguageConfigManager.Instance.getLanguage("moneyCover") + goods.amount;
        rmbCoverNum.text = goods.rate;
        rmbCoverNum.text = doubleTag.activeSelf ? rmbCoverNumber.ToString() : goods.rate;
        DiscLabel.text = goods.desc1;

        titleIcon.spriteName = goods.desc2;
        nameLabel.text = goods.name;
        if (goods.type == 0)
        {
            DiscLabel.gameObject.SetActive(doubleTag.activeSelf);
        }
		if (goods.type == 1) {
			doubleTag.SetActive (false);
			rmbCoverNum.gameObject.SetActive (false);
			if (NoticeManagerment.Instance.monthCardDueDate == null)
				rmbCoverNum1.text = Language ("monthCardName1");
			else
				rmbCoverNum1.text = string.Format (Language ("monthCardName2"), (NoticeManagerment.Instance.monthCardDueSeconds - ServerTimeKit.getSecondTime ()) / 86400);
		}

		if(goods.type == 2)
		{
			WeekCardInfoFPort fPort = FPortManager.Instance.getFPort ("WeekCardInfoFPort") as WeekCardInfoFPort;
			fPort.WeekCardInfoAccess(()=>{
				doubleTag.SetActive (false);
				rmbCoverNum.gameObject.SetActive (false);
				if(WeekCardInfo.Instance.weekCardState == WeekCardState.not_open)// 没有购买//
				{
					rmbCoverNum1.text = Language ("weekCardName1");
				}
				else
				{
					if(WeekCardInfo.Instance.endTime - ServerTimeKit.getSecondTime () > 0)
					{
						if((WeekCardInfo.Instance.endTime - ServerTimeKit.getSecondTime ()) / 86400 > 0)
						{
							// 显示剩余天数//
							rmbCoverNum1.text = string.Format (Language ("weekCardName2"), (WeekCardInfo.Instance.endTime - ServerTimeKit.getSecondTime ()) / 86400);
						}
						else
						{
							// 不足一天显示一天//
							rmbCoverNum1.text = string.Format (Language ("weekCardName2"), 1);
						}
					}
					else
					{
						rmbCoverNum1.text = Language ("weekCardName3");
					}
					
				}
			});
		}
		
		//没有充值再送,后面不显示
		if (string.IsNullOrEmpty (goods.desc1) || goods.desc1 == "0") {
			//discountObj.gameObject.SetActive (false);
		}
		else {
			//	discountObj.gameObject.SetActive (true);
			DiscLabel.text = goods.desc1;
		}

	}
    public void setCashSidList(List<string> list) {
        sidList = list;
    }
	
}
