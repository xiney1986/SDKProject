using UnityEngine;
using System.Collections;

public class NvshenShopWindow : WindowBase
{
	/* fields */
	/** 预制件容器数组-钻石商城,神秘商城*/
	public GameObject contentPrefabs;
	/**预制件挂接点 */
	public GameObject contentPoints;
	public Goods goods;
	public GameObject nvShenButtonPrefab;
	private CallBack callback;
	
	protected override void begin ()
	{
		base.begin ();
        initContent();
        MaskWindow.UnlockUI();//解除UI的遮罩
	}
	public override void OnNetResume ()
	{
		base.OnNetResume ();
        initContent();
	}

	/** 更新节点容器 */
    public void updateContent() 
    {
        GameObject content = getContent();
        NvShenStore nvshen = content.GetComponent<NvShenStore>();
        nvshen.updateUI();
    }
	/// <summary>
	/// 初始化容器
	/// </summary>
	/// <param name="tapIndex">Tap index.</param>
	public void initContent()
    {
		GameObject content = getContent ();
        NvShenStore nvshen = content.GetComponent<NvShenStore>();
        nvshen.init(this);//给fatherWindow赋值
	}
    /// <summary>
    /// 点击事件
    /// </summary>
    /// <param name="gameObj"></param>
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj); 
		if (gameObj.name == "close") {
			finishWindow ();
		}
	}
	

	/// <summary>
	/// 获取容器
	/// </summary>
	/// <param name="contentPoint">容器点</param>
	/// <param name="tapIndex">下标</param>
	private GameObject getContent() {
		GameObject contentPoint = contentPoints;
		contentPoint.SetActive (true);
		GameObject content;
        if (contentPoint.transform.childCount > 0) {
            Transform childContent = contentPoint.transform.GetChild(0);
            content = childContent.gameObject;
        } else {
            content = NGUITools.AddChild(contentPoint, contentPrefabs);
        }
		return content;
	}
}
