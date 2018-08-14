using UnityEngine;
using System.Collections;

/** 
 * 按钮基础类
 * @author 李程
 * */
//[RequireComponent (typeof(UIButton))]
public class ButtonBase : MonoBase
{



	public static bool hasButtonClick = false;
    public static bool isBloodItem = false;
	public GUI_state buttonState;
	public WindowBase fatherWindow;

	public UISprite spriteBg {
		get {
			if (ngui_buttonScript != null)
				return ngui_buttonScript.tweenTarget.GetComponent<UISprite> ();
			return null;
		}
	}

	string  normalSpriteName;
	string disableSpriteName;
	//...

	protected UIButton ngui_buttonScript;
	public UILabel textLabel;
	public bool playAudio = true;
	/** 是否加锁 */
	public  bool LockOnClick = true;
	/** 是否点击缩放 */
	public bool isPressScale = false;
	/** 是否设置默认颜色 */
	public bool isDefButtonColor = true;
	/** 初始文本描边颜色 */
	protected Color outLineColor=Colors.BACKGROUND_NONE;

	[HideInInspector]
	public CallBack<GameObject>
		onClickEvent;
	bool isLoad = false;
	float passTime;
	bool isPassing = false;//是否正在按下中
	/** 是否正在缩放中 */
	bool isPressScaleing = false;
    bool isShowStare = false;
    float tempTime;
	/**扩展字段*/
	public Hashtable exFields ;
	public const float LONGPASSTIME = 0.5f;


	// overrive this code for click!
	public virtual void DoClickEvent ()
	{
		if (onClickEvent != null)
			onClickEvent (gameObject);
	}

	public virtual void OnAwake () {
	}

	public virtual void begin ()
	{

	}

	void Awake ()
	{
		ngui_buttonScript = GetComponent<UIButton> ();
		if(isDefButtonColor)
			setDefButtonColor ();
		OnAwake ();
	}

	public void setNormalSprite (string spriteName)
	{

		if (ngui_buttonScript != null) {
		
			ngui_buttonScript.normalSprite = spriteName;

		}
	
	}

	void setDefButtonColor ()
	{
		if (ngui_buttonScript != null) {
			ngui_buttonScript.defaultColor = Colors.BUTTON_BG_NROMAL;
			ngui_buttonScript.disabledColor = Colors.BUTTON_BG_NROMAL;
			ngui_buttonScript.hover = Colors.BUTTON_BG_NROMAL;
			ngui_buttonScript.pressed = Colors.BUTTON_BG_NROMAL;
		}
	}

	void ScaleButton (Vector3 scaleP)
	{
		gameObject.transform.localScale = scaleP;
	}

	void Update ()
	{
        if (isBloodItem) {
            if (isPassing && !isShowStare) {
                passTime += Time.deltaTime;
                if (passTime >= 0.01f) {
                    DoLongPass();
                    passTime = 0;
                    //isPassing = false;
                    isShowStare = true;
                }

            } else {
                if (!isPassing && isShowStare) {
                    tempTime += Time.deltaTime;
                    if (tempTime > 0.1f) {
                        LongPassFinish();
                        isShowStare = false;
                        tempTime = 0;
                    }
                    // isPressed = false;
                }
            }
        } else {
            if (isPassing && !isShowStare) {
                passTime += Time.deltaTime;
                if (passTime >= LONGPASSTIME) {
                    DoLongPass();
                    passTime = 0;
                    //isPassing = false;
                    isShowStare = true;
                }

            } else {
                if (!isPassing && isShowStare) {
                    LongPassFinish();
                    isShowStare = false;
                    // isPressed = false;
                }
            }
        }
		DoUpdate ();
	}

    public virtual void LongPassFinish() { 
        
    }
	public virtual void DoUpdate ()
	{
		
		
	}

	public void disableButton (bool onOFF)
	{
		if (gameObject == null)
			return;


		if (ngui_buttonScript == null)
			ngui_buttonScript = gameObject.GetComponent<UIButton> ();

		if (ngui_buttonScript == null)
			return;

        if (ngui_buttonScript != null)
            ngui_buttonScript.isEnabled = !onOFF;

        if (textLabel != null) {
			if(outLineColor==Colors.BACKGROUND_NONE) {
				outLineColor = textLabel.effectColor;
			}
			if (onOFF) {
				textLabel.color = Colors.BUTTON_TEXT_DISABLEL;
				textLabel.effectColor = Colors.BUTTON_TEXT_OUTLINE_DISABLEL;
			}	
			else {
				textLabel.color = Colors.BUTTON_TEXT_NROMAL;
				textLabel.effectColor = outLineColor;
			}
		}

	}

	public bool isDisable ()
	{
        if (ngui_buttonScript == null)
            return true;
		return	!ngui_buttonScript.isEnabled;
	}

	[System.Obsolete("方法已弃用,换成disablebutton")]
	public void disableImageButton (bool onOFF)
	{
		if (gameObject == null)
			return;
		UIImageButton imageButton = gameObject.GetComponent<UIImageButton> ();
		if (imageButton != null) {
			imageButton.isEnabled = !onOFF;
			if (!onOFF) {
				textLabel.color = Colors.BUTTON_TEXT_NROMAL;
				textLabel.effectColor = Colors.BUTTON_TEXT_NROMALLINE;				
			} else {
				textLabel.color = Colors.BUTTON_TEXT_DISABLEL;
				textLabel.effectColor = Colors.BUTTON_TEXT_DISABLELLINE;	
			}
		}
	}

	[System.Obsolete("方法已弃用,换成disablebutton")]
	public void disableUIButton (bool onOFF)
	{
		if (gameObject == null)
			return;

		collider.enabled = !onOFF;
		if (!onOFF) {
			if (textLabel != null)
				textLabel.color = Colors.BUTTON_TEXT_NROMAL;
			if (spriteBg != null)
				spriteBg.spriteName = normalSpriteName;
		} else {
			if (textLabel != null)
				textLabel.color = Colors.BUTTON_TEXT_DISABLEL;
			if (spriteBg != null)
				spriteBg.spriteName = disableSpriteName;
		}
	}
	//以上过时

	protected virtual  void OnPress (bool isDown)
	{
		if (isDown == true) {
			isPassing = true;
		} else {
			isPassing = false;
		}
		OnPressScale(isDown);
	}
	/** 鼠标按下时缩放 */
	protected virtual void OnPressScale(bool isDown) {
		// 是否缩放或者正在缩放
		if(!isPressScale || isPressScaleing) return;
		isPressScaleing=true;
		if(isDown) {
			TweenScale ts = TweenScale.Begin (gameObject, 0.1f, new Vector3 (0.9f, 0.9f, 1f));
			EventDelegate.Add (ts.onFinished, () => {
				ts = TweenScale.Begin (gameObject, 0.1f, new Vector3 (1.1f, 1.1f, 1f));
				EventDelegate.Add (ts.onFinished, () => {
					isPressScaleing=false;
				},true);
			},true);
		} else {
			TweenScale ts = TweenScale.Begin (gameObject, 0.1f, new Vector3 (1f, 1f, 1f));
			EventDelegate.Add (ts.onFinished, () => {
				isPressScaleing=false;
			},true);
		}
	}
	/** 还原按下时缩放值 */
	private void ResetPressScale() {
		if(!isPressScale) return;
		gameObject.transform.localScale=Vector3.one;
	}

	public virtual void DoLongPass () {
	
	}

	public virtual void DoEnable () {

	}

	public virtual void DoDisable () {
		ResetPressScale();
	}

	protected virtual void OnDrag (Vector2 delta)
	{ 
		isPassing = false;
	}

	public virtual void OnDrop (GameObject drag)
	{


		if (fatherWindow != null) {
			if (typeof(TeamEditWindow) == fatherWindow.GetType ()) {
				(fatherWindow as TeamEditWindow).releaseCard (drag);
			}

		}
	}

	void OnEnable ()
	{
		DoEnable ();
		if (isLoad == true) {
			startAnim ();
		}
	}

	void OnDisable ()
	{
		DoDisable ();

	}

	void buttonStartAnimComplete ()
	{

		buttonState = GUI_state.normal;
		begin ();
	}

	void startAnim ()
	{
		if (buttonState == GUI_state.Onstart)
			return;

		buttonState = GUI_state.normal;
		begin ();
	}

	void Start ()
	{
		//		print ("buttom start");
		isLoad = true;
		startAnim ();


	}

	public void setFatherWindow (WindowBase father)
	{

		fatherWindow = father;

	}

	void OnClick ()
	{
		passTime = 0;
		if (hasButtonClick == true)
			return;
        if (fatherWindow == null)
			return;
        hasButtonClick = true;


		if (buttonState != GUI_state.normal || fatherWindow.getState () != GUI_state.normal) {
			hasButtonClick = false;
			return;
		}
        try {
			if (LockOnClick)
				MaskWindow.LockUI ();
            fatherWindow.buttonEventBase (gameObject);
			DoClickEvent ();
		} catch (System.Exception ex) {
			if (GameManager.DEBUG) {
				throw ex;
			}
		}
		hasButtonClick = false;
		
		
		if (playAudio) {
			PlayAudio ();
		}
	}
	
	protected virtual void PlayAudio ()
	{
		AudioManager.Instance.PlayAudio (103); 
	}
}
