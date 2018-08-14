using UnityEngine;
using System.Collections;


//行动力消耗显示控制器
// @author 李程

public class ActionCastCtrl : MonoBase
{
    /** PVE行动力类型 */
    public const int PVE_TYPE = 0;
    /** 公会战行动力消耗 */
    public const int GUILD_FIGHT_TYPE = 1;
	public float deadDelay ;	
	public UILabel label;
    public UISprite sprite;

    public void init(string str, int type) {
        label.text = str.ToString();
        sprite.spriteName = getSpriteNameByType(type);
    }

    public string getSpriteNameByType(int type) {
        switch (type) { 
            case PVE_TYPE:
                return "icon_pve";
            case GUILD_FIGHT_TYPE:
                return "warPower";
            default:
                return "icon_pve";
        }
    }
	
	// Use this for initialization
	void Start (){
	iTween.ValueTo (gameObject, iTween.Hash ("onupdate", "moveUpdate", "from",  transform.localPosition, "to",  transform.localPosition+new Vector3(0,50f,0), "oncomplete", "DoOver", "time", deadDelay , "easetype", iTween.EaseType.easeOutCubic));
	}
	

	
	void moveUpdate (Vector3 data)
	{
		transform.localPosition = data; 
	}

	void DoOver ()
	{
 			EffectManager.Instance.removeEffect (this); 
	}

}
