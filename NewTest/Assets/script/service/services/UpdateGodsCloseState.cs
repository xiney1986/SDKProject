using UnityEngine;
using System.Collections;

/**
 * 得到后台推送的跨服战关闭服务
 * @author huangzhenghan
 * */
public class UpdateGodsCloseState:BaseFPort
{
	

    public UpdateGodsCloseState()
	{
		
	}

	public override void read (ErlKVMessage message)
	{
        string flag = (message.getValue("god_war_state") as ErlType).getValueString();
        if (flag == "eliminate")
        {
            if (GodsWarManagerment.Instance.StateInfo == 2&&UiManager.Instance.godsWarFinalWindow != null &&
                UiManager.Instance.godsWarFinalWindow.gameObject.activeInHierarchy) {
                UiManager.Instance.godsWarFinalWindow.OnDataLoad();
            }
            if (GodsWarManagerment.Instance.StateInfo == 2) return;
            GodsWarManagerment.Instance.getGodsWarStateInfo(() => { });
            UiManager.Instance.openDialogWindow<MessageLineWindow>((win) =>
            {
                win.Initialize(LanguageConfigManager.Instance.getLanguage("godsWar_1411"));
            });
	    }
	    if (flag == "finals")
	    {
	        if (UiManager.Instance.godsWarFinalWindow != null &&
	            UiManager.Instance.godsWarFinalWindow.gameObject.activeInHierarchy)
	        {
	            UiManager.Instance.godsWarFinalWindow.OnDataLoad();
	        }
	    }
        if (flag == "not_open" || flag == "server_busy")
	    {
            if (UiManager.Instance.hasWndowByName("BattleWindow"))
	        {
	            UiManager.Instance.isInGodsBattle = true;
	            return;
	        }
	        UiManager.Instance.isInGodsBattle = false;
	        if (UiManager.Instance.godsBuyWind!=null)
	        {
	            UiManager.Instance.godsBuyWind.destoryWindow();
	        }
	        if (UiManager.Instance.godsWarReplayWindow != null)
	        {
	            UiManager.Instance.godsWarReplayWindow.destoryWindow();
	        }
	        if (UiManager.Instance.godsWarUserInfoWindow != null)
	        {
                UiManager.Instance.godsWarUserInfoWindow.destoryWindow();
	        }
	        if (UiManager.Instance.godsWarMySuportWindow != null)
	        {
                UiManager.Instance.godsWarMySuportWindow.destoryWindow();
	        }
	        if (UiManager.Instance.godsWarProgramWindow!=null)
	        {
                UiManager.Instance.godsWarProgramWindow.destoryWindow(); 
	        }
            if (UiManager.Instance.godsWarFinalWindow != null || UiManager.Instance.godsWarGroupStageWindow!=null) 
	        {
                UiManager.Instance.destoryWindowByName("MessageWindow");
                UiManager.Instance.BackToWindow<MainWindow>();
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) =>
                {
                    win.Initialize(LanguageConfigManager.Instance.getLanguage("godsCloseInfo"));
                });
	        }
	    }
	}
}

