using UnityEngine;
using System.Collections;

/**
 * 队伍编辑专用
 * 用于拖拽卡片更换位置
 * yxl
 */
public class TeamEditDragDropItem : UIDragDropItem 
{
	public TeamEditWindow window;

	protected override void OnDragDropRelease (GameObject surface)
	{
		base.OnDragDropRelease (surface);
		//至少保留一名上阵英雄

		CallBack cancelDrag = () => {
			//TweenPosition.Begin (gameObject, 0.3f, window.getEmptyPositionWithItem(gameObject));
			iTween.MoveTo(gameObject,iTween.Hash("islocal",true,"position",window.getEmptyPositionWithItem(gameObject),"time",0.3f));
		};

		if (ArmyManager.Instance.isEditArmyActive ()) {
			//公会战队伍单独处理响应弹窗
			if(ArmyManager.Instance.ActiveEditArmy.armyid == ArmyManager.PVP_GUILD)
			{
				UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("GuildArea_102"));
				cancelDrag();
			}
			else
			{
//				UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
//					win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("teamEdit_err03"),
//					                LanguageConfigManager.Instance.getLanguage ("teamEdit_err02"),(msgHandle) => {
//						if (msgHandle.buttonID == MessageHandle.BUTTON_RIGHT) {
//							EventDelegate.Add (window.OnHide,()=>{
//								window.destoryWindow ();
//							});
//							FuBenManagerment.Instance.inToFuben ();
//						}
//					}
//					);
//				});
				if (window.getDstRoleViewWidthItem(gameObject).card.isMainCard() && window.IsAlternateWidthItem(surface)) 
				{
					//主角不能为替补
					TextTipWindow.Show(LanguageConfigManager.Instance.getLanguage("teamEdit_err01"));
					cancelDrag();
				}
				else
				{
				    if (surface == null || !surface.name.StartsWith("team_edit_item_"))
				        cancelDrag();
				    else
				    {
                        if (surface != null &&surface.transform.parent.name == "suber" &&
				            window.openAnmi[window.getIndexWidthItem(surface)].activeInHierarchy)
				            cancelDrag();
                        else
				            changePlayer(surface, cancelDrag);
				    }
				}
			}
			//cancelDrag();
		}
		else if (surface == null || !surface.name.StartsWith ("team_edit_item_") || (surface.transform.parent == gameObject.transform.parent && window.getIndexWidthItem(gameObject) == window.getIndexWidthItem(surface))) {
			cancelDrag();
		}else if(surface != null&&surface.name.StartsWith ("team_edit_item_")&&surface.transform.parent.name=="suber"&&window.openAnmi[window.getIndexWidthItem(surface)].activeInHierarchy){
			cancelDrag();
		}
		else if (window.getDstRoleViewWidthItem(gameObject).card.isMainCard() && window.IsAlternateWidthItem(surface)) {
			//主角不能为替补
			TextTipWindow.Show(LanguageConfigManager.Instance.getLanguage("teamEdit_err01"));
			cancelDrag();
		}
		else if (ArmyManager.Instance.ActiveEditArmy.getPlayerNum () <= 1 && !window.IsAlternateWidthItem(gameObject) &&
		         window.IsAlternateWidthItem(surface) && window.getDstRoleViewWidthItem(surface).card == null) {
			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
				win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0040"),null,LanguageConfigManager.Instance.getLanguage("s0390"),null);
			});
			cancelDrag();
		}
//		else if (!window.IsAlternateWidthItem(surface) && ArmyManager.Instance.ActiveEditArmy.getPlayerNum () <= 1) {
//			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
//				win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0040"),null,LanguageConfigManager.Instance.getLanguage("s0390"),null);
//			});
//			cancelDrag();
//		}
		else {
			changePlayer(surface,cancelDrag);
		}
	}

	void changePlayer(GameObject surface,CallBack cancelDrag)
	{
		RoleView src = gameObject.GetComponent<RoleView>();
		RoleView dst = window.getDstRoleViewWidthItem(surface);
		
		if(dst.gameObject.activeInHierarchy)
		{
			//主角不能为替补
			if (dst != null && dst.card.isMainCard() && window.IsAlternateWidthItem(gameObject)) {
				TextTipWindow.Show(LanguageConfigManager.Instance.getLanguage("teamEdit_err01"));
				cancelDrag();
				return;
			}
			
			Card c = dst.card;
			dst.init(src.card,window,null);
			src.init(c,window,null);
			
			dst.transform.localPosition = transform.localPosition;
			iTween.MoveTo(dst.gameObject,iTween.Hash("islocal",true,"position",window.getEmptyPositionWithItem(dst.gameObject),"EaseType",iTween.EaseType.linear,"time",0.25f));
			gameObject.transform.localPosition = window.getEmptyPositionWithItem(dst.gameObject);
			iTween.MoveTo(gameObject,iTween.Hash("islocal",true,"position",window.getEmptyPositionWithItem(gameObject),"EaseType",iTween.EaseType.linear,"time",0.25f));
			
			
			
			if(window.IsAlternateWidthItem(src.gameObject))
				ArmyManager.Instance.ActiveEditArmy.alternate [window.getIndexWidthItem(src.gameObject)] = src.card.uid;
			else
				ArmyManager.Instance.ActiveEditArmy.players [window.getIndexWidthItem(src.gameObject)] = src.card.uid;
			
			if(window.IsAlternateWidthItem(dst.gameObject))
				ArmyManager.Instance.ActiveEditArmy.alternate [window.getIndexWidthItem(dst.gameObject)] = dst.card.uid;
			else
				ArmyManager.Instance.ActiveEditArmy.players [window.getIndexWidthItem(dst.gameObject)] = dst.card.uid;
		}
		else
		{
			dst.gameObject.SetActive(true);
			dst.init(src.card,window,null);
			
			dst.transform.localPosition = transform.localPosition;
			iTween.MoveTo(dst.gameObject,iTween.Hash("islocal",true,"position",window.getEmptyPositionWithItem(dst.gameObject),"EaseType",iTween.EaseType.linear,"time",0.25f));
			src.gameObject.SetActive(false);
			gameObject.transform.localPosition = window.getEmptyPositionWithItem(gameObject);
			
			if(window.IsAlternateWidthItem(src.gameObject))
				ArmyManager.Instance.ActiveEditArmy.alternate [window.getIndexWidthItem(src.gameObject)] = "0";
			else
				ArmyManager.Instance.ActiveEditArmy.players [window.getIndexWidthItem(src.gameObject)] = "0";
			
			if(window.IsAlternateWidthItem(dst.gameObject))
				ArmyManager.Instance.ActiveEditArmy.alternate [window.getIndexWidthItem(dst.gameObject)] = dst.card.uid;
			else
				ArmyManager.Instance.ActiveEditArmy.players [window.getIndexWidthItem(dst.gameObject)] = dst.card.uid;
		}
		
		window.reLoadCard ();
		window.rushCombat();
	}

	protected override void OnDragDropMove (Vector3 delta)
	{
		delta.x /= UiManager.Instance.screenScaleX;
		delta.y /= UiManager.Instance.screenScaleY;
		base.OnDragDropMove (delta);
	}

	protected override void OnDragDropStart ()
	{
		base.OnDragDropStart ();
	}


}
