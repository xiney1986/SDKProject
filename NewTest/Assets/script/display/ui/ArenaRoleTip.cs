using UnityEngine;
using System.Collections;

public class ArenaRoleTip : MonoBehaviour {
    public UILabel lblName;
    public UILabel lblAward;

    public void init(ArenaUserInfo user)
    {
		lblName.text = user.name+"[fefeb7]Lv."+user.level+"[-]";
        string intertal ="[fcf6a7]"+ LanguageConfigManager.Instance.getLanguage("Arena05")+"[-][74e682]  +40" + "[-]";
        string merit ="[fcf6a7]"+ LanguageConfigManager.Instance.getLanguage("Arena06")+"[-][74e682]  +" +user.getMeritAward().ToString()+"[-]";
		lblAward.text = intertal +"  "+merit ;
    }
}
