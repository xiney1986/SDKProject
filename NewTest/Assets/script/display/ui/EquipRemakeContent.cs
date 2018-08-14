using UnityEngine;
using System.Collections;

/// <summary>
/// 装备改造活动
/// </summary>
public class EquipRemakeContent  : MonoBase {

	/* gameobj fields */
	/*const */
	const int IN_MAIL = 3;//在仓库
	const int IN_TAMP_STORE=2;//在临时仓库
	const int IN_STORE = 1;//在邮箱
	/** 旧的名字 */
	public UILabel oldNameLabel;
	/** 旧的形象 */
	public UITexture oldIconSprite;
	/** 旧的品质 */
	public UISprite oldQualitySprite;
	/** 旧的属性 */
	public UILabel oldAttrLabel;
	/** 新的名字 */
	public UILabel newNameLabel;
	/** 新的形象 */
	public UITexture newIconSprite;
	/** 新的品质 */
	public UISprite newQulitySprite;
	/** 新的属性 */
	public UILabel newAttrLabel;
	/** 倒计时 */
	public UILabel countdownTimeLabel;
	/** 找回按钮 */
	public ButtonBase recoverButton;
	/** 改造按钮 */
	public ButtonBase remakeButton;
	/** 装备显示组 */
	public GameObject equipGroup;
	/** 没装备提示 */
	public GameObject noEquipGroup;
	/** 新装备按钮*/
	public ButtonBase newClick;
	/** 老装备按钮*/
	public ButtonBase oldClick;

	/* fields */
	/** 活动窗口 */
	NoticeWindow win;
	/** 装备改造活动 */
	EquipRemakeNotice equipReamkeNotice;
	/** 旧装备 */
	Equip oldEquip;
	/** 新装备 */
	Equip newEquip;
	/** 是否有装备可以改造 */
	bool isHaveEquip = false;
	/** 公告模板 */
	NoticeSample noticeSample;
	/** 装备改造升级公告配置 */
	EquipRemakeNoticeContent noticeContent;
	/** 定时器 */
	private Timer timer;
	/** 活动结束时间 */
	int noticeCloseTime;

	/* methods */
	public void initContent (NoticeWindow win, Notice notice) {
		this.equipReamkeNotice = notice as EquipRemakeNotice;
		this.win = win;
		noticeSample = notice.getSample ();
		noticeContent = noticeSample.content as EquipRemakeNoticeContent;
		initButton ();
		initUI ();
	}
	/** 初始化button */
	private void initButton () {
		recoverButton.fatherWindow = win;
		remakeButton.fatherWindow = win;
		oldClick.fatherWindow = win;
		newClick.fatherWindow = win;
		recoverButton.onClickEvent = HandleRecoverEvent;
		remakeButton.onClickEvent = HandleRemakeEvent;
		oldClick.onClickEvent = HandleOldClickEvent;
		newClick.onClickEvent = HandleNewClickEvent;
	}
	/// <summary>
	/// 更新装备信息
	/// </summary>
	private void initUI () {
		updateTimer ();
		updateEquipItem ();
		updateButton ();
	}
	/**判断装备在那个地方 1是在仓库中 3是在邮件中，2是在临时仓库中 如果都没有就返回0*/
	private int checkEquipIndex(){
		Equip tempEquip = StorageManagerment.Instance.getEquipTypeBySid (noticeContent.getSourceEquipSid ());
		if (tempEquip != null)return IN_STORE;
		if (MailManagerment.Instance.checkEquipinMain (noticeContent.getSourceEquipSid ()))return IN_MAIL;
		if (StorageManagerment.Instance.getPropInTempPropsBySid (noticeContent.getSourceEquipSid ()) != null)return IN_TAMP_STORE;
		return 0;
		}
	private void updateEquipItem () {
		Equip tempEquip = StorageManagerment.Instance.getEquipTypeBySid (noticeContent.getSourceEquipSid ());
		if (checkEquipIndex () != 0)isHaveEquip = true;

		if(IN_STORE == checkEquipIndex())
			oldEquip = StorageManagerment.Instance.getEquipTypeBySid (noticeContent.getSourceEquipSid ());
		else 
			oldEquip = EquipManagerment.Instance.createEquip (noticeContent.getSourceEquipSid ());
		if (newEquip == null) {
			newEquip = EquipManagerment.Instance.createEquip ("", noticeContent.getTargetEquipSid (), oldEquip.getEXP (), 0,oldEquip.getrefineEXP());
		}
		if (isHaveEquip) {
			noEquipGroup.SetActive (false);
			equipGroup.SetActive (true);
			if (oldEquip != null) {
				oldNameLabel.text = QualityManagerment.getQualityColor (oldEquip.getQualityId ()) + "Lv." + oldEquip.getLevel () + oldEquip.getName ();
				oldQualitySprite.spriteName = QualityManagerment.qualityIDToIconSpriteName (oldEquip.getQualityId ());
				ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + oldEquip.getIconId (), oldIconSprite);
				AttrChange[] attrs = oldEquip.getAttrChanges ();
				if (attrs != null) {
					if (attrs.Length > 0 && attrs [0] != null)
						oldAttrLabel.text = attrs [0].typeToString () + attrs [0].num;
				}
			}
			if (newEquip != null) {
				newNameLabel.text = QualityManagerment.getQualityColor (newEquip.getQualityId ()) + "Lv." + newEquip.getLevel () + newEquip.getName ();
				newQulitySprite.spriteName = QualityManagerment.qualityIDToIconSpriteName (newEquip.getQualityId ());
				ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + newEquip.getIconId (), newIconSprite);
				AttrChange[] attrs = newEquip.getAttrChanges ();
				if (attrs != null) {
					if (attrs.Length > 0 && attrs [0] != null)
						newAttrLabel.text = attrs [0].typeToString () + attrs [0].num;
				}
			}
		}
		else {
			noEquipGroup.SetActive (true);
			equipGroup.SetActive (false);
		}
	}
	/** 更新计时器 */
	private void updateTimer () {
		if (timer == null) {
			setNoticeOpenTime ();
			timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY);
			timer.addOnTimer (refreshNoticeTime);
			timer.start (true);
		}
		else {
			refreshNoticeTime ();
		}
	}
	/** 设置活动开启时间 */
	public void setNoticeOpenTime () {
		noticeCloseTime = equipReamkeNotice.activeTime.getDetailEndTime ();
	}
	/** 刷新活动时间 */
	private void refreshNoticeTime () {
		long remainCloseTime = noticeCloseTime - ServerTimeKit.getSecondTime ();
		if (remainCloseTime >= 0) {
			countdownTimeLabel.gameObject.SetActive (true);
			countdownTimeLabel.text = TimeKit.timeTransformDHMS (remainCloseTime);
		}
		else {
			countdownTimeLabel.gameObject.SetActive (false);
		}
	}
	/** 初始化button */
	private void updateButton () {
		newClick.disableButton (false);
		oldClick.disableButton (true);
		if (!isHaveEquip) {
			recoverButton.disableButton (false);
			remakeButton.disableButton (true);
		}
		else {
			recoverButton.disableButton (true);
			remakeButton.disableButton (false);
		}
	}
	/// <summary>
	/// 确定提升装备
	/// </summary>
	void remakeBack (MessageHandle msg) {
		if (msg.msgEvent == msg_event.dialogOK) {
			(FPortManager.Instance.getFPort ("NoticeEquipRemakeFPort") as NoticeEquipRemakeFPort).remakeEquip (equipReamkeNotice.sid, msg.msgNum, () => {
				MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("notice_equipRemake_ReOK"));
				win.initTopButton (true, 0,noticeSample.entranceId);
				MaskWindow.UnlockUI ();
			});
		}
	}
	/** 装备找回 */
	private void HandleRecoverEvent (GameObject gameObj) {
		if (UserManager.Instance.self.getVipLevel () < noticeContent.getUsedGetBackVipLevel ()) {
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("notice_equipRemake_er01", noticeContent.getUsedGetBackVipLevel ().ToString ()));
			return;
		}
		if ((StorageManagerment.Instance.getAllEquip ().Count + 1) > StorageManagerment.Instance.getEquipStorageMaxSpace ()) {
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("storeFull_equip"));
			return;
		}
		(FPortManager.Instance.getFPort ("NoticeEquipRemakeFPort") as NoticeEquipRemakeFPort).getBackEquip (equipReamkeNotice.sid, () => {
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, LanguageConfigManager.Instance.getLanguage ("notice_equipRemake_gbOK"), null);
				EventDelegate.Add (win.OnHide, () => {
					UiManager.Instance.openWindow<MailWindow> ((wins) => {
						wins.Initialize (0);
						EventDelegate.Add (wins.OnHide, () => {
							this.initUI ();
						});
					});
				});
			});
			MaskWindow.UnlockUI ();
		});
	}
	/** 装备提升 */
	private void HandleRemakeEvent (GameObject gameObj) {
		if(checkEquipIndex()==IN_TAMP_STORE){
			UiManager.Instance.openDialogWindow<MessageLineWindow> ((winn) => {
				winn.Initialize (LanguageConfigManager.Instance.getLanguage 
				                 ("notice_equipRemake_button_remake1"));
			});
			return;
		}
		if(checkEquipIndex()==IN_MAIL){
			UiManager.Instance.openDialogWindow<MessageLineWindow> ((winn) => {
				winn.Initialize (LanguageConfigManager.Instance.getLanguage 
				                 ("notice_equipRemake_button_remake2"));
			});
			return;
		}
		if ((StorageManagerment.Instance.getAllEquip ().Count + 1) > StorageManagerment.Instance.getEquipStorageMaxSpace ()) {
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("storeFull_equip"));
			return;
		}
		if (UserManager.Instance.self.getVipLevel () < noticeContent.getUsedVipLevel ()) {
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("notice_equipRemake_er01", noticeContent.getUsedVipLevel ().ToString ()));
			return;
		}
		else if (!isHaveEquip) {
			MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("notice_equipRemake_er02"));
			return;
		}
		Prop tmpProp = PropManagerment.Instance.createProp (noticeContent.getExchangePropSid ());
		int noticeSid = equipReamkeNotice.sid;
		UiManager.Instance.openDialogWindow<RemakeBuyWindow> ((win) => {
			win.init (tmpProp, noticeContent.getExchangePropRate (), noticeContent.getConsumRmbValue (), remakeBack);
		});
	}
	/** 消耗 */
	void OnDestroy () {
		clear ();
	}
	/** 清理 */
	private void clear () {
		if (timer != null)
			timer.stop ();
		timer = null;
	}
	/***/
	private void HandleOldClickEvent (GameObject gameObj) {
		UiManager.Instance.openWindow<EquipAttrWindow> (
			(winEquip) => {
			winEquip.Initialize (oldEquip, EquipAttrWindow.OTHER, null);
		});
	}
	/***/
	private void HandleNewClickEvent (GameObject gameObj) {
		UiManager.Instance.openWindow<EquipAttrWindow> (
			(winEquip) => {
			winEquip.Initialize (newEquip, EquipAttrWindow.OTHER, null);
		});
	}
}