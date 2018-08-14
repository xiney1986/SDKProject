using UnityEngine;
using System.Collections;

public class ButtonTeamPlayer : ButtonBase
{
	public UITexture cardImage;
	public bool beDrag = false;
	public Card card;//关联的card对象
	public UISprite quality;//品质图标
	public UISprite dizuo;
	public UISprite noCardSprite;
	public UISprite levelBg;
	public UISprite sign;//职业图标 
	public UILabel level;//等级不解释
	public UISprite state;
	public UILabel attrAll;
	public int  showType ;//
	public const int TYPE_VIEW = 1;//仓库浏览模式
	public const int TYPE_TEAMEDIT = 2;//队伍编辑中的妥协拖动模式
	public const int TYPE_PVP = 3;//PVP中的队伍显示
	public int index;
	public bool IsAlternate;	//是否替补
	// 等你初始化诗句
	public void Initialize (Card _card, int type)
	{ 
		updateButton (_card, type); 
	}

	public override void DoUpdate ()
	{
		if (attrAll == null)
			return;
		
		if (attrAll.gameObject.activeSelf == true) {
			attrAll.alpha =sin ();
		}
	}

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		if (showType == TYPE_TEAMEDIT) {
			//队伍编辑窗口模式
			if (card == null) {
				//无卡片直接开卡片选择窗口
				UiManager.Instance.openWindow<CardChooseWindow>( (win)=>{
					win.Initialize (CardChooseWindow.ROLECHOOSE);
				});
				fatherWindow.hideWindow ();
				return;
			}
			UiManager.Instance.openWindow<CardBookWindow>( (win)=>{
				fatherWindow.hideWindow ();
				win.init (card, CardBookWindow.CARDCHANGE, showTeamEditWindow);
			});
			//记录当前选中的按钮，便于更换卡片后更新
			//(fatherWindow as TeamEditWindow).chooseButton = this;
			
		} else if (showType == TYPE_PVP) {
			
		} else {
			
			//这里选择隐藏窗口为了保存滑动位置，如果完全删除，滑动从头开始了
			fatherWindow.hideWindow ();
			(fatherWindow as CardChooseWindow).instandCard = card;
			
			if ((fatherWindow as CardChooseWindow).getShowType () == CardChooseWindow. ROLECHOOSE) {
				UiManager.Instance.openWindow<CardBookWindow>( (win)=>{
					win.init (card, CardBookWindow.INTOTEAM, showCardChooseWindow); 
				});
			}
			

			//聊天展示卡片
			if ((fatherWindow as CardChooseWindow).getShowType () == CardChooseWindow.CHATSHOW) {
				UiManager.Instance.openWindow<CardChooseWindow>( (win)=>{
					sendMsgFPort(win.chatChannelType);
				});
				fatherWindow.destoryWindow();
				UiManager.Instance.openWindow<ChatWindow>((win)=>{
					win.initChatWindow(ChatManagerment.Instance.sendType - 1);
				});

			} 
		}
	}


	//聊天展示接口
	private void sendMsgFPort(int _chatChannelType)
	{
		ChatSendMsgFPort fport = FPortManager.Instance.getFPort("ChatSendMsgFPort") as ChatSendMsgFPort;
		fport.access(_chatChannelType,null,ChatManagerment.SHOW_CARD,card.uid,null);
	}
	
	private void showTeamEditWindow ()
	{
		UiManager.Instance.openWindow<TeamEditWindow>( (win)=>{
			win.reLoadTeam();
		});

	}

	private void showCardChooseWindow ()
	{
		UiManager.Instance.openWindow<CardChooseWindow>( (win)=>{
			win.Initialize (CardChooseWindow.ROLECHOOSE);
		});
	}

	
	void OnDrag (Vector2 delta)
	{ 
		if (card == null)
			return;
		
		//不在队伍编辑面板不能拖动卡片
		if (showType != TYPE_TEAMEDIT)
			return; 
		
		UITexture dragCard = (fatherWindow as TeamEditWindow).dragCardObj; 
		
		if (beDrag == false) {
			dragCard.mainTexture = cardImage.mainTexture;
			dragCard.transform.position = new Vector3 (cardImage.transform.position.x, cardImage.transform.position.y, dragCard.transform.position.z);	
			beDrag = true;
			
			level.text = "";
			quality.gameObject.SetActive (false);
			sign.gameObject.SetActive (false);
		} 
		
		dragCard.gameObject.SetActive (true);
 
		dragCard.transform.localPosition += new Vector3 (delta.x / UiManager.Instance.screenScaleX, delta.y / UiManager.Instance.screenScaleY, 0);
		cardImage.gameObject.SetActive (false);
		
		//很悲剧的不能响应自己拖进自己范围内的Drop事件。。。只有关闭碰撞，让背景碰撞代替
		collider.enabled = false;		
	}

	void cleanBtton ()
	{
		cardImage.gameObject.SetActive (false);
		cardImage.mainTexture = null;
		level.text = "";
		quality.gameObject.SetActive (false);
		card = null;
		sign.gameObject.SetActive (false);
		if(dizuo != null)
			dizuo.spriteName = "roleBack_2";
		if(noCardSprite != null)
			noCardSprite.gameObject.SetActive (true);
		if(levelBg != null)
			levelBg.gameObject.SetActive (false);
		if (state != null)
			state.gameObject.SetActive (false);
	}

	void calculateAllAttr ()
	{
		if (attrAll == null)
			return;
		
		int count = 0;
		//计算附加等级
		count += card.getHPGrade ();
		count += card.getATTGrade();
		count += card.getDEFGrade();
		count += card.getMAGICGrade();
		count += card.getAGILEGrade();
		
		if (count > 0) {
			attrAll.gameObject.SetActive (true);
			attrAll.text = "+" + count;
		} else {
			attrAll.gameObject.SetActive (false);
		}
		
	}
	
	public void updateButton (Card newCard, int type)
	{
		showType = type;
		card = newCard;
		if (newCard == null) {

			if (IsAlternate)	
				ArmyManager.Instance.ActiveEditArmy.alternate [index] = "0";
			else
				ArmyManager.Instance.ActiveEditArmy.players [index] = "0";
			cleanBtton ();
			return;
		} else { 
			
			if (showType == TYPE_TEAMEDIT) {
				if (IsAlternate)	
					ArmyManager.Instance.ActiveEditArmy.alternate [index] = newCard.uid;
				else
					ArmyManager.Instance.ActiveEditArmy.players [index] = newCard.uid;
			}
			if(noCardSprite != null)
				noCardSprite.gameObject.SetActive (false);
			cardImage.gameObject.SetActive (true);
			quality.gameObject.SetActive (true);
			if(levelBg != null)
				levelBg.gameObject.SetActive (true);
			card = newCard;
			calculateAllAttr ();
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + card.getImageID () , cardImage);
			cardImage.gameObject.SetActive (true);
			quality.spriteName = QualityManagerment.qualityIDToString (card.getQualityId ());
			level.text = "Lv." + card.getLevel ();
			sign.gameObject.SetActive (true);
			sign.spriteName = CardManagerment.Instance.jobIDToString (card.getJob ());
			if(dizuo != null)
				dizuo.spriteName = QualityManagerment.qualityIDToBackGround (card.getQualityId());
			
			if (state != null) {
				if (ArmyManager.Instance.getAllArmyPlayersIds ().Contains (card.uid) || ArmyManager.Instance.getAllArmyAlternateIds ().Contains (card.uid)) {
					state.gameObject.SetActive (true);
					state.spriteName = "inTeam";
					state.width = 33;
					state.height = 53;
				} else {
					state.gameObject.SetActive (false);
				}
			}
		}
	}
	
	public override void OnDrop (GameObject drag)
	{
		base.OnDrop (drag);
		//卡片选择面板中，不再响应拖动事件
		if (showType == TYPE_VIEW)
			return;
		else if (showType == TYPE_PVP)
			return;
		 
		ButtonTeamPlayer button = drag.GetComponent<ButtonTeamPlayer> (); 
		//如果当前上阵人数为1 
		if (ArmyManager.Instance.ActiveEditArmy.getPlayerNum () <= 1 && card == null && IsAlternate && button.IsAlternate == false) {
			(fatherWindow as TeamEditWindow).releaseCard (button.gameObject);
			UiManager.Instance.openDialogWindow<MessageWindow>( (win)=>{
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0040"), "", LanguageConfigManager.Instance.getLanguage ("s0069"), null);
			});
			return;
		}
		if (button != null) {
			if (button.card == null)
				return;
//			//替换前的 
//			Texture oldImage = cardImage.mainTexture;
			UITexture dragCard = (fatherWindow as TeamEditWindow).dragCardObj;
			dragCard.gameObject.SetActive (false);
//			cardImage.mainTexture = dragCard.mainTexture;
			
			
//			button.cardImage.mainTexture = oldImage;
			button.cardImage.gameObject.SetActive (true);
			button.beDrag = false;
			//记得重新打开碰撞体哦！~
			button.collider.enabled = true;
			
			//card交换
			Card oldCard = card;
			
			updateButton (button.card, ButtonTeamPlayer.TYPE_TEAMEDIT);
			button.updateButton (oldCard, ButtonTeamPlayer.TYPE_TEAMEDIT);
			(fatherWindow as TeamEditWindow).rushCombat();
			
		}
		
	}	
}
