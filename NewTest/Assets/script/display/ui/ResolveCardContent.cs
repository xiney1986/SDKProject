using UnityEngine;
using System.Collections;

/// <summary>
/// 卡片晶炼
/// </summary>
public class ResolveCardContent : dynamicContent {
	[HideInInspector]
	public ResolveWindow
		resolveWin;
	[HideInInspector]
	public ResolveCardChooseWindow
		cardChooseWin;
	ArrayList cards;
	 
	public void Initialize (ArrayList _cards, ResolveWindow _resolveWin, ResolveCardChooseWindow _cardChooseWin) {
		cards = topTeamRole(_cards);
		resolveWin = _resolveWin;
		cardChooseWin = _cardChooseWin;
		base.reLoad (cards.Count); 
	}
	public void reLoad (ArrayList _cards) {
		cards = _cards;
		base.reLoad (cards.Count);
	}
	public override void updateItem (GameObject item, int index) {
		RoleView button = item.GetComponent<RoleView> ();
		button.init (cards [index] as Card, cardChooseWin, (roleView) => {
			OnButtonClick (roleView);
		}); 
		button.tempGameObj.SetActive (resolveWin.isSelect (button.card));
	}
	public override void initButton (int  i) {
		if (nodeList [i] == null) {
			nodeList [i] = NGUITools.AddChild (gameObject, cardChooseWin.cardButtonPrefab);
			RoleView view = nodeList [i].GetComponent<RoleView> ();
			view.transform.localScale = new Vector3 (0.9f, 0.9f, 1);
			view.init (cards [i] as Card, cardChooseWin, (roleView) => {
				OnButtonClick (roleView);
			});

			if (view.tempGameObj == null) {
				UISprite us = NGUITools.AddChild<UISprite> (view.gameObject);
				us.depth = 50;
				us.atlas = view.qualityBg.atlas;
				us.spriteName = "gou_3";
				us.MakePixelPerfect ();
				us.transform.localScale = new Vector3 (2, 2, 1);
				us.gameObject.SetActive (resolveWin.isSelect (view.card));	
				view.tempGameObj = us.gameObject;
			}
		}
	}
	void OnButtonClick (RoleView view) {
		if (GuideManager.Instance.isEqualStep (114005000)) {
			GuideManager.Instance.doGuide ();
			GuideManager.Instance.guideEvent ();
		}
		if (resolveWin.isSelect (view.card)) {
			resolveWin.offSelectCard (view.card);
			view.tempGameObj.SetActive (false);
        } else if (resolveWin.selectMagicList.Count + resolveWin.selectedCardList.Count + resolveWin.selectedEquipList.Count < 8) {
			if(view.card.state==4)
				MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("tips_001"));
			else if(view.card.state==1)
				MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("tips_002"));
			else if(view.card.state==5)
				MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("tips_003"));
			else
			{
				resolveWin.onSelectCard (view.card);
				view.tempGameObj.SetActive (true);
			}				
        } else if (resolveWin.selectMagicList.Count + resolveWin.selectedCardList.Count + resolveWin.selectedEquipList.Count >= 8) {
			TextTipWindow.ShowNotUnlock (Language("resolveChooseMax_1"));
		}
		MaskWindow.UnlockUI ();
	}

	public void updateAllItems () {
		for (int i=0; i<nodeList.Count; i++) {
			updateItem (nodeList [i], i);
		}
	}
	public override void jumpToPage (int index) {
		base.jumpToPage (index);
		if (GuideManager.Instance.isEqualStep (114004000)) {
			GuideManager.Instance.guideEvent ();
		}
	}
    // 交换
    private void swap(ArrayList list, int left, int right) {
        object temp;
        temp = list[left];
        list[left] = list[right];
        list[right] = temp;
    }
	//排序卡片（品质降、上阵卡片在同品质卡片之后）
	ArrayList topTeamRole(ArrayList list) {
        for (int i = 0; i < list.Count; i++) {//按品质降序排
            for (int j = 0; j < list.Count - 1 - i; j++) {
                Card card1 = list[j] as Card;
                Card card2 = list[j + 1] as Card;
                if (card1.getQualityId() < card2.getQualityId())
                    swap(list, j, j + 1);
                else if (card1.getQualityId() == card2.getQualityId() && card1.sid < card2.sid)
                    swap(list, j, j + 1);
                else if (card1.getQualityId() == card2.getQualityId() && card1.sid == card2.sid && card1.getLevel() < card2.getLevel())
                    swap(list, j, j + 1);
            }
        }
        for (int i = 0; i < list.Count; i++) {//同品质上阵卡排在最后
            if (i >= list.Count - 1) return list;
            if (!ArmyManager.Instance.getTeamCardUidList().Contains((list[i] as Card).uid)) continue;
            //是上阵卡片
            object card = list[i];
            for (int k = i + 1; k < list.Count; k++) {
                if (ArmyManager.Instance.getTeamCardUidList().Contains((list[k] as Card).uid)) continue;
                if ((list[k] as Card).getQualityId() == (card as Card).getQualityId()) {
                    object tmp = list[k];
                    list[i] = tmp;
                    list[k] = card;
                    break;
                }
            }
        }
        return list;
	}
}