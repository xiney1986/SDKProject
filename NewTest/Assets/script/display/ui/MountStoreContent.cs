using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 坐骑仓库容器
/// </summary>
public class MountStoreContent : MonoBase {

	/* const */
	/** 容器下标常量 */
	public const int TAP_AGENRAL_CONTENT=0, // 普通坐骑
				TAP_BACTICE_CONTENT=1, // 活动坐骑
				 UN_SHOW_RIDE_MOUNT=1,//不显示出战坐骑
					SHOW_RIDE_MOUNT=0;//显示出战坐骑
	/* fields */
	public static Color activeLabelColor =Color.white;
	public static  Color normalLabelColor = new Color (0.7f, 0.7f, 0.7f, 1f);
	public static string activeSprite="button_big2";
	public static string normalSprite="button_big2_clickOn";
	public Bgeneralscrollview bgeneralsrollview;//坐骑节点
	public GameObject[] rollviewPoints;//挂perfab的节点 两种坐骑类型
	public GameObject mountStoreItemPerfab;//每个坐骑显示的预制体
	/** tap butotn */
	public ButtonBase ageneralbutton;//顶部普通坐骑按钮
	public ButtonBase bactivebutton;//顶部活动坐骑按钮
	private int currentTapIndex;//当前活动的页面 
	private WindowBase fatherWindo;
	public UILabel mountCounts;//拥有的坐骑总数
	private int buttonType;//显示出战坐骑的标示符号

	/* methods */
	void Awake() {
		
	}
	void Start () {
		
	}
	/***/
	public void init(WindowBase fatherWindow,int type) {
		buttonType=type;
		initButton (fatherWindow);
	}
	/** 初始化button */
	public void initButton(WindowBase fatherWindow) {
		//初始化普通坐骑的信息
		initTapButton(fatherWindow);
		updateTopButtonState(TAP_AGENRAL_CONTENT);
		initContent(TAP_AGENRAL_CONTENT);

	}
	/** 更新UI */
	public void UpdateUI() {
		updateContent();
	}
	//对顶部两个选择按钮进行父类窗口赋值
	public void initTapButton(WindowBase fatherWindow){
		ageneralbutton.setFatherWindow(fatherWindow);
		bactivebutton.setFatherWindow(fatherWindow);
		bgeneralsrollview.fatherWindow=fatherWindow;
		this.fatherWindo=fatherWindow;
	}
	//初始化容器UI
	public void initContent(int index){
		resetContentsActive ();
		updateMountCount();
		bool  flag=false;
		if(buttonType==UN_SHOW_RIDE_MOUNT){
			flag=true;
		}else{
			flag=false;
		}
		//buttonType=0;
		//GameObject content = getContent (index);
		switch (index) {
		case TAP_AGENRAL_CONTENT:
			bgeneralsrollview.gameObject.SetActive(true);
			bgeneralsrollview.init(fatherWindo,TAP_AGENRAL_CONTENT,flag);
			break;
		case TAP_BACTICE_CONTENT:
			bgeneralsrollview.gameObject.SetActive(true);

			bgeneralsrollview.init(fatherWindo,TAP_BACTICE_CONTENT,flag);
			break;
		default:
				break;
		
		}

	}
	//更新容器UI
	public void updateContent(){
		bgeneralsrollview.updateUI();
	}
	private void updateMountCount(){
		List<Mounts> mountList=MountsManagerment.Instance.getAllMountsList();
		int num=0;
		if(mountList!=null||mountList.Count!=0){
			num=mountList.Count;
		}
		mountCounts.text=LanguageConfigManager.Instance.getLanguage("mount_count",num.ToString()+"/"+(ExchangeManagerment.Instance.getAllExchangesMount(ExchangeType.MOUNT).Count+ExchangeManagerment.Instance.getAllExchangesMount(ExchangeType.BACTICMOUNT).Count).ToString());
	}
	private void resetContentsActive (){
		foreach (GameObject item in rollviewPoints) {
			item.SetActive(false);
		}
	}
	/** 顶级选择button点击事件 */
	public void buttonEventBase (GameObject gameObj) {
		if(gameObj.name=="ageneralbutton"){
			if(currentTapIndex==TAP_AGENRAL_CONTENT){
				MaskWindow.UnlockUI();
				return;
			}
			updateTopButtonState(TAP_AGENRAL_CONTENT);
			initContent(currentTapIndex);
			MaskWindow.UnlockUI();

		}else if(gameObj.name=="bactivebutton"){
			if(currentTapIndex==TAP_BACTICE_CONTENT){
				MaskWindow.UnlockUI();
				return;
			}
			updateTopButtonState(TAP_BACTICE_CONTENT);
			initContent(currentTapIndex);
			MaskWindow.UnlockUI();
		}
	}
	//更新top button的状态
	private void updateTopButtonState(int type){
		currentTapIndex=type;
		if(type==TAP_AGENRAL_CONTENT){
			ageneralbutton.textLabel.color=activeLabelColor;
			bactivebutton.textLabel.color=normalLabelColor;
			ageneralbutton.setNormalSprite(activeSprite);
			bactivebutton.setNormalSprite(normalSprite);
		}else{
			ageneralbutton.textLabel.color=normalLabelColor;
			bactivebutton.textLabel.color=activeLabelColor;
			ageneralbutton.setNormalSprite(normalSprite);
			bactivebutton.setNormalSprite(activeSprite);
		}
	}

}
