using UnityEngine;
using System.Collections;
using System;

/**
 * 公告条目组件
 * @author 汤琦
 * */
public class ButtonNoticeItem : ButtonBase
{
	public UITexture background;//背景
	public UILabel noticeName;//公告名
	public UILabel time;//公告发布时间
	public UISprite readIcon;//读取图标
	public GameObject msgNum; //信息数量
	public UILabel num;//信息数量
	private const string NOREAD = "sign_noread";//未读精灵名
	private const string READED = "sign_read";//已读精灵名
	public NoticeWindow win;
	public Notice notice;
	public ButtonNotice button;
	private bool noticing = false;
	
	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		if (notice.sid == 1) {
			if (GuideManager.Instance.isLessThanStep (GuideGlobal.NEWFUNSHOW27)) {
				MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("GuideError_01"));
				return;
			}

			GuideManager.Instance.doGuide ();
//			UiManager.Instance.openWindow<HoroscopesPrayWindow> ((wnd) => {
//				wnd.init ();
//			});
//			GetStarInfoFPort fport = FPortManager.Instance.getFPort ("GetStarInfoFPort") as GetStarInfoFPort;
//			fport.access (() =>
//			{
//				UiManager.Instance.openWindow<HoroscopesPrayWindow> ((wnd) => {
//					wnd.init ();
//				});
//			});
		} else if (notice.sid == 2) {

		} else {
			MaskWindow.UnlockUI();
		}
	}

	public  void Initialize (Notice _notice)
	{
		win = fatherWindow as NoticeWindow;
		updateNotice (_notice);

	}
	
	public void updateNotice (Notice newNotice)
	{
		if (newNotice == null)
			return;
		else {
			notice = newNotice;
			NoticeSample sample = NoticeSampleManager.Instance.getNoticeSampleBySid (notice.sid);
			if (sample.type == NoticeType.STICKNOTICE) {
				ResourcesManager.Instance.LoadAssetBundleTexture ("texture/announcement/" + "announcement_" + sample.icon, background);
			} else {
				if (NoticeManagerment.Instance.isNoticeBySidDraw (notice.sid)) {
					noticing = true;
				} else {
					noticing = false;
				}
				button.fatherWindow = fatherWindow;
				button.UpdateTemp (notice);
				noticeName.text = sample.name;
//				time.text = timeTransform (newNotice.time[0]);
				if (notice.readed == 0) {
					readIcon.spriteName = NOREAD;
				} else {
					readIcon.spriteName = READED;
				}
			}

			if(notice.sid == 1) {
				GetStarInfoFPort fp = FPortManager.Instance.getFPort ("GetStarInfoFPort") as GetStarInfoFPort;
				fp.access (()=>{

					if(this==null || this.gameObject==null || win==null)
						return;

					int nowTime = ServerTimeKit.getCurrentSecond ();
					int beginTime = HoroscopesManager.Instance.getBeginTime ();
					int endTime = HoroscopesManager.Instance.getEndTime ();
					if ((nowTime > beginTime) && (nowTime < endTime) && HoroscopesManager.Instance.getPrayTime() <= ServerTimeKit.getSecondTime()) {
						msgNum.SetActive (true);
						num.text = "1";
					}
					else {
						msgNum.SetActive (false);
					}
				});
//				int nowTime = ServerTimeKit.getCurrentSecond ();
//				int beginTime = HoroscopesManager.Instance.getBeginTime ();
//				int endTime = HoroscopesManager.Instance.getEndTime ();
//				if ((nowTime > beginTime) && (nowTime < endTime)) {
//					msgNum.SetActive (true);
//					num.text = "1";
//				}
//				else {
//					msgNum.SetActive (false);
//				}

			}
		}
	}
	 
	public override void DoDisable ()
	{
		base.DoDisable ();
		
	}
	
	//转换时间格式 单位:秒  
	private string timeTransform (int time)
	{  
		DateTime dt = TimeKit.getDateTime (time);
		if (string.Format ("{0:t}", dt).Contains ("AM")) {
			return dt.Month + "/" + dt.Day + "   " + string.Format ("{0:t}", dt).Replace ("AM", "");
		} else {
			return dt.Month + "/" + dt.Day + "   " + string.Format ("{0:t}", dt).Replace ("PM", "");
		}
	}

	void Update ()
	{
		if (noticing) {
			button.background.color = new Color (button.background.color.r, sin (), button.background.color.g);
		}
	}
}
