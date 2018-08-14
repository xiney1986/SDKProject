using UnityEngine;

public class ButtonChat : ButtonBase
{
	public GameObject UI_MsgTips;
    public UIScrollView UI_StopMove;
    public Rect UI_MaxPos;
    public UISpriteAnimation UI_Bg;

    private float mSpeed = 1f;
    
    
    public override void begin()
    {
        base.begin();
        mSpeed = 960f / Screen.height;
    }

    public void setShowTips(int type)
    {
        UI_MsgTips.SetActive(type == 2);
        if (type != 0)
        {
            UI_Bg.enabled = true;
        }
        else
        {
            UI_Bg.enabled = false;
            
        }
        
    }

    protected override void OnDrag(Vector2 delta)
    {
        base.OnDrag(delta);

//        Vector3 pos = transform.localPosition + (Vector3)delta * mSpeed;
//        if ((pos.x > UI_MaxPos.width || pos.x < UI_MaxPos.x) || (pos.y > UI_MaxPos.height || pos.y < UI_MaxPos.y))
//        {
//            return;
//        }

//		transform.localPosition = pos;
//
//        if (UI_StopMove != null)
//            UI_StopMove.enabled = false;

    }

    protected override void OnPress(bool isDown)
    {
        base.OnPress(isDown);

//        if (UI_StopMove != null)
//            UI_StopMove.enabled = true;

    }


	
} 

