using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// vip奖励窗口
/// </summary>
public class VipAwardViewWindow : WindowBase { 

	/* fields */
	/** VIP领取排序 */
	VipReceivedComp comp=new VipReceivedComp();
	/** vip图标 */
	public UISprite sprite_vipIcon;
	/** vip等级描述 */
	public UILabel vipLevelLabel;
	/** 奖励容器 */
	public VipAwardContent awardContent;
	
	protected override void begin () {
		base.begin ();
		updateUI ();
		MaskWindow.UnlockUI ();
	}
	//断线重连
	public override void OnNetResume () {
		base.OnNetResume ();
		updateUI ();
	}
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
		}
	}
	/** 更新UI */
	public void updateUI () {
		updateVipInfo ();
		updateAwardContent ();
	}
	/** 更新VIP信息 */
	private void updateVipInfo() {
		int level = UserManager.Instance.self.getVipLevel ();
		if (level > 0) {
			sprite_vipIcon.gameObject.SetActive (true);
			sprite_vipIcon.spriteName = "vip" + level;
			sprite_vipIcon.MakePixelPerfect ();
		} else {
			sprite_vipIcon.gameObject.SetActive (false);
		}
		if (UserManager.Instance.self.getVipLevel () == 0) {
			vipLevelLabel.text = LanguageConfigManager.Instance.getLanguage ("s0319");
		} else {
			//当前级别
			vipLevelLabel.text = LanguageConfigManager.Instance.getLanguage ("s0317") + ":";
		}
	}
	/** 更新vip奖励容器 */
	public void updateAwardContent () {
		Vip[] vips = VipManagerment.Instance.getAllVip ();
		/*最新的说不用排序了
        Vip[] newVip = new Vip[12];

		if (vips!=null&&vips.Length>1) {

            //没有领取过奖励的链表
            List<Vip> haveVip = new List<Vip>();
            //领取过奖励的链表
            List<Vip> noVip = new List<Vip>();
            //组合的奖励链表
            List<Vip> combinationVip = new List<Vip>();

            //遍历数组
            for (int i = 0; i < vips.Length; i++)
            {
                if (!VipManagerment.Instance.alreadyGetAward(vips[i].vipAwardSid))
                { //还没有拿奖励的
                    haveVip.Add(vips[i]);
                }
                else
                {   //已经拿了奖励的
                    noVip.Add(vips[i]);
                }
            }
            for (int i = 0; i < noVip.Count; i++)
            {
                haveVip.Add(noVip[i]);
            }

            
            for (int i = 0; i < haveVip.Count; i++)
            {
                newVip[i] = haveVip[i];
            }


			//SetKit.sort (vips,comp);
		}
		*/
		awardContent.reLoad(vips);
	}

}