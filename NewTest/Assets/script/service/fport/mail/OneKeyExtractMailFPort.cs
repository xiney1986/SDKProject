using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

/**
 * 一键提取邮件接口
 * @author 汤琦
 * */
public class OneKeyExtractMailFPort : BaseFPort
{

	private CallBack<bool,bool> callback;
	
	public void access (CallBack<bool,bool> callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.MAIL_ONEKEYEXTRACT);
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		// 是否全部领取//
		bool totalExtract = false;
		// 所有邮件中拥有附件的邮件的个数//
		int emailHasAnnexCount = 0;
		ErlType type = message.getValue ("msg") as ErlType;

		if (type is ErlArray) {
			List<PrizeSample> list = new List<PrizeSample> ();
			ErlArray array = type as ErlArray;

			for (int i = 0; i < MailManagerment.Instance.getAllMail().Count; i++)
			{
				if(MailManagerment.Instance.getAllMail()[i].annex != null && MailManagerment.Instance.getAllMail()[i].status != 2)
				{
					emailHasAnnexCount ++;
				}
			}
			// 判断是否已全部领完//
			if(emailHasAnnexCount == array.Value.Length)
			{
				totalExtract = true;
			}


			for (int i = 0; i < array.Value.Length; i++) {
				//MailManagerment.Instance.extractMailByUid (array.Value [i].getValueString ());
				MailManagerment.Instance.extractMailByUidWithAnnex(array.Value[i].getValueString(),list);
			}
			if (list != null) {
				Card c;
				CardManagerment cardInstance = CardManagerment.Instance;
				for (int i = 0; i < list.Count; i++) {
					if (list[i].type == PrizeType.PRIZE_CARD) {
						c = cardInstance.createCard (list[i].pSid);
						for (int k = 0; k < list[i].getPrizeNumByInt (); k++) {
							if (HeroRoadManagerment.Instance.activeHeroRoadIfNeed (c))
								TextTipWindow.Show(LanguageConfigManager.Instance.getLanguage("s0418"));
						}
					}
				}
				UiManager.Instance.openDialogWindow<AllAwardViewWindow>((win)=>{
					win.Initialize(list,LanguageConfigManager.Instance.getLanguage ("s0115"));
				});
				callback (true,totalExtract);
			} else {
				callback (true,totalExtract);
			}
		} else {
			callback (false,totalExtract);
		}
	}
}
