using UnityEngine;
using System.Collections;

public class MissionRoleCtrl : MonoBase
{

	public bool isCycling=false;//是否在骑马
    public NpcTitleView TitleView;
	//以下为AI变量
	
	public FuBenCardCtrl activeAnimCtrl;//当前动画控制器
	public FuBenCardCtrl animCtrl;//人物动画控制器
	public FuBenCardCtrl mountsAnimCtrl;//对应的坐骑控制器
    public FuBenCardCtrl angelAnimCtrl;//对应的精灵控制器
    Transform roleRoot;

	/// <summary>
	/// 初始化
	/// </summary>
	public virtual void  initRoleAniCtrl (FuBenCardCtrl _animCtrl,Mounts mounts,int vipLevel)
	{	
		animCtrl = _animCtrl;
		activeAnimCtrl = animCtrl;
		if (mounts != null) {
			ResourcesManager.Instance.LoadAssetBundleTexture (mounts.getModelPath(),gameObject.transform,(obj)=>{
				GameObject gameObj=obj as GameObject;
                if (mounts.getMountsSample().modelID == "miluDeer") gameObj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                else gameObj.transform.localScale = new Vector3(1,1,1);
				mountsAnimCtrl=gameObj.transform.FindChild("body").gameObject.GetComponent<FuBenCardCtrl> ();
                initMountsAniCtrl(mountsAnimCtrl);
			});
		} else {
            animCtrl.gameObject.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
			isCycling = false;
			setShadows();
		}
        string angelPath = AngelSampleManager.Instance.get3DObjPath(vipLevel);//这个函数没判断守护天使是否激活！！！！
        if (PlayerPrefs.GetString(PlayerPrefsComm.ANGEL_USER_NAME + UserManager.Instance.self.uid) != "ok")
            angelPath = "";
        if (angelPath != "")
        {
            ResourcesManager.Instance.LoadAssetBundleTexture(angelPath, gameObject.transform, (obj) =>
            {
                GameObject gameObj = obj as GameObject;
                angelAnimCtrl = gameObj.transform.FindChild("body").gameObject.GetComponent<FuBenCardCtrl>();
                initAngeAniCtrl(angelAnimCtrl);
            });
        }
	}
    /// <summary>
    /// 初始化
    /// </summary>
    public virtual void initRoleAniCtrl(FuBenCardCtrl _animCtrl, MountsSample mountsSample, int vipLevel) {
        animCtrl = _animCtrl;
        activeAnimCtrl = animCtrl;
        if (mountsSample != null) {
            ResourcesManager.Instance.LoadAssetBundleTexture("mounts/" + mountsSample.modelID, gameObject.transform, (obj) => {
                GameObject gameObj = obj as GameObject;
                if (mountsSample.modelID == "miluDeer") gameObj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                else gameObj.transform.localScale = new Vector3(1, 1, 1);
                mountsAnimCtrl = gameObj.transform.FindChild("body").gameObject.GetComponent<FuBenCardCtrl>();
                initMountsAniCtrl(mountsAnimCtrl);
            });
        } else {
            isCycling = false;
            setShadows();
        }
        string angelPath = AngelSampleManager.Instance.get3DObjPath(vipLevel);//这个函数没判断守护天使是否激活！！！！
        if (PlayerPrefs.GetString(PlayerPrefsComm.ANGEL_USER_NAME + UserManager.Instance.self.uid) != "ok")
            angelPath = "";
        if (angelPath != "") {
            ResourcesManager.Instance.LoadAssetBundleTexture(angelPath, gameObject.transform, (obj) => {
                GameObject gameObj = obj as GameObject;
                angelAnimCtrl = gameObj.transform.FindChild("body").gameObject.GetComponent<FuBenCardCtrl>();
                initAngeAniCtrl(angelAnimCtrl);
            });
        }
    }
	/** 初始化坐骑 */
	private void initMountsAniCtrl(FuBenCardCtrl mountsAnimCtrl) {
		Transform saddle = mountsAnimCtrl.transform.FindChild ("saddle");
		//把人插进马
		
		//npc和主角场景里的结构稍微不同
		if(GetType()==typeof(MissionNpcCtrl))
			roleRoot= animCtrl.transform;
		else {
			animCtrl.transform.localRotation = Quaternion.identity;
			roleRoot= animCtrl.transform.parent;
		}
		roleRoot.parent = saddle;
		roleRoot.localPosition = Vector3.zero;
		roleRoot.transform.localRotation = Quaternion.identity;
        //roleRoot.localScale = Vector3.one;
        roleRoot.localScale = new Vector3(1.2f, 1.2f, 1.2f);
		activeAnimCtrl = mountsAnimCtrl;
		isCycling = true;
		setShadows();
	}
    /**把精灵插入人 */
    private void initAngeAniCtrl(FuBenCardCtrl angelAnimCtrl)
    {
        if (GetType() == typeof(MissionNpcCtrl))
            roleRoot = animCtrl.transform;
        else {
            animCtrl.transform.localRotation = Quaternion.identity;
            roleRoot = animCtrl.transform.parent;
        }
        Transform angelPoint1 = roleRoot.transform.FindChild("jingling") == null ? roleRoot.transform.GetChild(0).FindChild("jingling") : roleRoot.transform.FindChild("jingling");
        if (angelPoint1 != null)
        {    
            Transform angelPoint = angelPoint1.FindChild("jinglingPoint");
            angelAnimCtrl.transform.parent = angelPoint;
            angelAnimCtrl.gameObject.transform.localScale = new Vector3(1, 1, 1);
            angelAnimCtrl.gameObject.transform.localPosition = Vector3.zero;
            angelAnimCtrl.gameObject.transform.localRotation = new Quaternion(0, 0, 0, 1);
        }
    }
	private void setShadows() {
		if(isCycling) { // 有马儿
			animCtrl.setShadowsActive(false);
			if(mountsAnimCtrl!=null)
				mountsAnimCtrl.setShadowsActive(true);
		} else {
			animCtrl.setShadowsActive(true);
			if(mountsAnimCtrl!=null)
				mountsAnimCtrl.setShadowsActive(false);
		}
	}
	protected virtual void OnUpdate(){

	}
	void Update(){
		OnUpdate();
	}
	public void playMountsMove () {
		if(mountsAnimCtrl!=null)
			mountsAnimCtrl.playMove ();
	}
	public void playMountsStand () {
		if(mountsAnimCtrl!=null)
			mountsAnimCtrl.playStand ();
	}
	public void playMountsHappy () {
		if(mountsAnimCtrl!=null)
			mountsAnimCtrl.playHappy ();
	}
	public void playMountsIdle () {
		if(mountsAnimCtrl!=null)
			mountsAnimCtrl.playIdle ();
	}
}
