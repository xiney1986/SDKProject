using UnityEngine;
using System.Collections;

public class CardScrapView : MonoBehaviour {

	[HideInInspector]
	public CardStoreWindow fawin;
	public GameObject roleViewPrefab;
	public UILabel numLabel;
	public ButtonExchangeReceive buttonExchange;
	RoleView buttonRole;
	private GameObject item;


	public void init(Exchange ex)
	{
		ExchangeSample sample = ex.getExchangeSample ();
		//按钮显示判断
		int count = ExchangeManagerment.Instance.getCanExchangeNum (ex);
		if (count <= 0) {
			buttonExchange.disableButton(true);
		} else {
			buttonExchange.disableButton(false);
		}
		//数量
		numLabel.text = CardScrapManagerment.Instance.getNumString(sample);
		//存信息到兑换按钮
		buttonExchange.fatherWindow = fawin;
		buttonExchange.updateButton (ex);
		//显示卡片
		Card card = CardManagerment.Instance.createCard(sample.exchangeSid);

		if (buttonRole == null){
		item = NGUITools.AddChild (gameObject,roleViewPrefab);
		item.name = "roleView";
		buttonRole= item.GetComponent<RoleView> ();
		}

		if (buttonRole != null && card != null) {
			buttonRole.init(card,fawin,(sss)=>{
				CardBookWindow.Show (card, CardBookWindow.OTHER, null);
			});
		}

	}
}
