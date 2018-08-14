using UnityEngine;
using System.Collections;

public class ButtonOneManyRechargeReceive : ButtonBase
{
	[HideInInspector]
    public NoticeOneManyRechargeContent content;
	public Recharge recharge;
	
	public void updateButton (Recharge recharge)
	{
		this.recharge = recharge;
		
	}
	
	public override void DoClickEvent ()
	{
		string str = LanguageConfigManager.Instance.getLanguage ("s0204");
		base.DoClickEvent ();
		RechargeSample sample = RechargeSampleManager.Instance.getRechargeSampleBySid (recharge.sid);
        if (recharge != null && !StorageManagerment.Instance.checkStoreFull(sample.prizes, out str))
        {
            disableButton(true);
            if (recharge.GetType() == typeof(Recharge))
            {
                NoticeGetActiveAwardFPort fport = FPortManager.Instance.getFPort("NoticeGetActiveAwardFPort") as NoticeGetActiveAwardFPort;
                fport.access(recharge.sid, (b) =>
                {
                    if (b) {
                        content.reload();
                        UiManager.Instance.createMessageLintWindow(Language("s0205"));
                    }
                    MaskWindow.UnlockUI();
                });
            }
        }
	}
	
	private bool isComplete ()
	{
		return recharge.isComplete ();
	}
	
}
