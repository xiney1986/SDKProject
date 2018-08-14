using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/** 
  * 特效管理器 
  * @author 李程
  * */

public class EffectManager
{

	List<MonoBehaviour> EffectList;
	/** 缓存的特效资源 */
	public Dictionary<string ,ResourcesData> cacheEffectRes;
	
	public static EffectManager Instance {
		get {
			return SingleManager.Instance.getObj ("EffectManager") as EffectManager;
		}
	}
	public EffectManager ()
	{
		EffectList = new List<MonoBehaviour> ();
 
	}
	/// <summary>
	/// 缓存特效资源
	/// </summary>
	/// <param name="Path">路径</param>
	/// <param name="callBack">ResourcesData=缓存好的资源,passObj=创建的对象</param>
	public void CacheEffect(string[] Paths,CallBack callBack)
	{
		ResourcesManager.Instance.cacheData (Paths, (list)=>{
			//			ResourcesData resData=null;
			//			if(list!=null&&list.Count>0){
			//				resData=list[0];
			//	cacheEffectRes.Add (Path,resData);
			//			}
			if(callBack!=null) {
				callBack();
			}
		}, "effect"); 
	}
	/// <summary>
	/// 缓存特效资源
	/// </summary>
	/// <param name="Path">路径</param>
	/// <param name="callBack">ResourcesData=缓存好的资源,passObj=创建的对象</param>
	public void CacheEffect(string Path,CallBack callBack)
	{
		ResourcesManager.Instance.cacheData (Path, (list)=>{
//			ResourcesData resData=null;
//			if(list!=null&&list.Count>0){
//				resData=list[0];
				//	cacheEffectRes.Add (Path,resData);
//			}
			if(callBack!=null) {
				callBack();
			}
		}, "effect"); 
	}

	void AddEffect (MonoBehaviour item)
	{
		EffectList.Add (item);
	}
	
	public void removeEffect (MonoBehaviour item)
	{
		//这里可以只关闭，不移除为之后的战斗优化
		EffectList.Remove (item);
		MonoBehaviour.Destroy (item.gameObject);
	}
	
	public BulletCtrl CreateBulletEffect (CharacterData owner, CharacterData target, List<BuffCtrl> buffs, string Path, bool lastBullet)
	{
        passObj _obj=null;
        //if(owner.characterCtrl.activeAction.Skill.serverData.sample.sid==21113||
        //    owner.characterCtrl.activeAction.Skill.serverData.sample.sid == 22113||
        //    owner.characterCtrl.activeAction.Skill.serverData.sample.sid == 23113||
        //    owner.characterCtrl.activeAction.Skill.serverData.sample.sid == 24113||
        //    owner.characterCtrl.activeAction.Skill.serverData.sample.sid == 25113) {//关羽技能正反两面
        //        if (target.camp == 2) _obj = MonoBase.Create3Dobj(Path,"Guanyu_AOE_0");
        //        else _obj = MonoBase.Create3Dobj(Path ,"Guanyu_AOE_1");

        //} else if (owner.characterCtrl.activeAction.Skill.serverData.sample.sid == 22117 ||
        //     owner.characterCtrl.activeAction.Skill.serverData.sample.sid == 23117 ||
        //     owner.characterCtrl.activeAction.Skill.serverData.sample.sid == 24117 ||
        //     owner.characterCtrl.activeAction.Skill.serverData.sample.sid == 25117 ||
        //     owner.characterCtrl.activeAction.Skill.serverData.sample.sid == 26117||
        //    owner.characterCtrl.activeAction.Skill.serverData.sample.sid==21117)
        //{
        //    if (target.camp == 2) _obj = MonoBase.Create3Dobj(Path, "Xiaolongnv_AOE1");
        //    else _obj = MonoBase.Create3Dobj(Path, "Xiaolongnv_AOE2");
        //}
	    int[] tempSid = CommandConfigManager.Instance.doubleEffectSkillSids;
	    int sidd = owner.characterCtrl.activeAction.Skill.serverData.sample.sid;
	    for (int i=0;i<tempSid.Length;i++)
	    {
	        if (sidd == tempSid[i])
	        {
	            string fileNmae = PathKit.getFileName(Path);
	            _obj = MonoBase.Create3Dobj(Path, fileNmae + (target.camp == 2 ? "1" : "2"));
                break;
	        }
	    }
        if(_obj==null)_obj = MonoBase.Create3Dobj(Path);
		//子弹类型数据在prefab上定义
		//passObj _obj = MonoBase.Create3Dobj (Path);
		BulletCtrl _ctrl = _obj.obj.GetComponent<BulletCtrl> ();
		_ctrl.transform.parent = BattleManager.Instance.battleFieldRoom.transform;
		_ctrl.initBullet (target, owner, buffs, lastBullet);
		AddEffect (_ctrl);
		return _ctrl;
	}
	public ReboundLabelCtrl CreateReboundLabel (Vector3 pos)
	{
		passObj _obj = MonoBase.Create3Dobj ("Effect/Other/ReboundLabel");
		ReboundLabelCtrl _ctrl = _obj.obj.GetComponent<ReboundLabelCtrl> ();
		_ctrl.transform.parent = UiManager.Instance.UIEffectRoot.transform;
		Vector3 pos2 = BattleManager.Instance.BattleCamera.WorldToScreenPoint (pos);
		_ctrl.transform.position = UiManager.Instance.gameCamera.ScreenToWorldPoint (pos2);
		_ctrl.transform.localScale = Vector3.one;
		AddEffect (_ctrl);
		return _ctrl;
	}
	public HitTextCtrl CreateHitText (CharacterCtrl target, int damage, int damageType, float offset)
	{
		passObj _obj = MonoBase.Create3Dobj ("Effect/Other/hitNumLabel");
		HitTextCtrl _ctrl = _obj.obj.GetComponent<HitTextCtrl> ();
		_ctrl.transform.parent = UiManager.Instance.UIEffectRoot.transform;
		Vector3 offsetVector=new Vector3 (0, 0.2f, offset);
		Vector3 pos = BattleManager.Instance.BattleCamera.WorldToScreenPoint (target.hitPoint.transform.position + offsetVector);
		_ctrl.transform.position = UiManager.Instance.gameCamera.ScreenToWorldPoint (pos);
		
		_ctrl.transform.localScale = Vector3.one;
		_ctrl.init (target, damage, damageType,offsetVector);
		AddEffect (_ctrl);
		return _ctrl;
	}
	public buffTextCtrl CreateBuffNumText (Transform target, string text, float offset)
	{
		passObj _obj = MonoBase.Create3Dobj ("Effect/Other/buffNumLabel");
		buffTextCtrl _ctrl = _obj.obj.GetComponent<buffTextCtrl> ();
		_ctrl.transform.parent = UiManager.Instance.UIEffectRoot.transform;
		Vector3 pos = BattleManager.Instance.BattleCamera.WorldToScreenPoint (target.position + new Vector3 (0, 0.2f, offset));
		_ctrl.transform.position = UiManager.Instance.gameCamera.ScreenToWorldPoint (pos);
		
		_ctrl.transform.localScale = Vector3.one;
		_ctrl.init (text, "");
		AddEffect (_ctrl);
		return _ctrl;
	}
	public buffTextCtrl CreateBuffNumTextForLastBattle (Transform target, string text, float offset)
	{
		passObj _obj = MonoBase.Create3Dobj ("Effect/Other/buffNumLabel_lastBattle");
		buffTextCtrl _ctrl = _obj.obj.GetComponent<buffTextCtrl> ();
		_ctrl.transform.parent = UiManager.Instance.UIEffectRoot.transform;
		Vector3 pos = BattleManager.Instance.BattleCamera.WorldToScreenPoint (target.position + new Vector3 (0, 0.2f, offset));
		_ctrl.transform.position = UiManager.Instance.gameCamera.ScreenToWorldPoint (pos);
		
		_ctrl.transform.localScale = Vector3.one;
		_ctrl.init (text, "");
		AddEffect (_ctrl);
		return _ctrl;
	}
	public buffTipCtrl CreateBuffTipText (Transform target, string text, float offset)
	{
		passObj _obj = MonoBase.Create3Dobj ("Effect/Other/buffLabel");
		buffTipCtrl _ctrl = _obj.obj.GetComponent<buffTipCtrl> ();
		_ctrl.transform.parent = UiManager.Instance.UIEffectRoot.transform;
		Vector3 pos = BattleManager.Instance.BattleCamera.WorldToScreenPoint (target.position + new Vector3 (0, 0.2f, offset));
		_ctrl.transform.position = UiManager.Instance.gameCamera.ScreenToWorldPoint (pos);
		
		_ctrl.transform.localScale = Vector3.one;
		_ctrl.init(text,target);
		AddEffect (_ctrl);
		return _ctrl;
	}
	public EffectCtrl CreateSkillBanner (string text)
	{
		passObj _obj = MonoBase.Create3Dobj ("Effect/UiEffect/battleSkillBanner");
		EffectCtrl _ctrl = _obj.obj.GetComponent<EffectCtrl> (); 
		_ctrl.transform.parent = UiManager.Instance.UIEffectRoot.transform;
		_ctrl.transform.localPosition = new Vector3 (0, 0, 0);
		_ctrl.transform.localScale = Vector3.one;
		_ctrl.transform.GetChild (0).GetComponent<UILabel> ().text = text;
		AddEffect (_ctrl);
		return _ctrl;
	}
	public ActionCastCtrl CreateActionCast (string text)
	{
		passObj _obj = MonoBase.Create3Dobj ("Effect/Other/actionCastLabel");
		ActionCastCtrl _ctrl = _obj.obj.GetComponent<ActionCastCtrl> (); 
		_ctrl.transform.parent = UiManager.Instance.UIEffectRoot.transform;
		//	_ctrl.transform.position =  MissionManager.instance.character.transform.position + new Vector3 (0, 1.2f, -0.1f);
		_ctrl.transform.localPosition = UiManager.Instance.MissionWorldToUIScreenPos (MissionManager.instance.character.gameObject.transform.position);
		_ctrl.transform.localScale = Vector3.one;
		_ctrl.init (text,ActionCastCtrl.PVE_TYPE);
		AddEffect (_ctrl);
		return _ctrl;
	}

    /// <summary>
    /// 在屏幕中央创建行动力消耗效果
    /// </summary>
    /// <param name="text">文字</param>
    /// <param name="type">类型</param>
    /// <param name="position">位置</param>
    /// <returns></returns>
    public ActionCastCtrl CreateActionCast(string text,int type)
    {
        passObj _obj = MonoBase.Create3Dobj("Effect/Other/actionCastLabel");
        ActionCastCtrl _ctrl = _obj.obj.GetComponent<ActionCastCtrl>();
        _ctrl.transform.parent = UiManager.Instance.UIEffectRoot.transform;
        _ctrl.transform.localPosition = Vector3.zero;
        _ctrl.transform.localScale = Vector3.one;
        _ctrl.init(text,ActionCastCtrl.GUILD_FIGHT_TYPE);
        AddEffect(_ctrl);
        return _ctrl;
    }
	public GetExpCtrl CreateGetExpLabel (int _type, CharacterCtrl target, int str, int _lv, int _combat)
	{
		passObj _obj = MonoBase.Create3Dobj ("Effect/Other/getExpLabel");
		GetExpCtrl _ctrl = _obj.obj.GetComponent<GetExpCtrl> ();
		_ctrl.transform.parent = UiManager.Instance.UIEffectRoot.transform;
		if (target != null) {
			Vector3 pos = BattleManager.Instance.BattleCamera.WorldToScreenPoint (target.hitPoint.transform.position + new Vector3 (0, 0.2f, 0.2f));
			_ctrl.transform.position = UiManager.Instance.gameCamera.ScreenToWorldPoint (pos);
			_ctrl.transform.localScale = new Vector3 (0.6f, 0.6f, 0.6f);
			_ctrl.init (_type, target, str, _lv, _combat);
			AddEffect (_ctrl);
		}
		return _ctrl;
	}
	
	public GetResourceLabel CreateGetResourceLabel (Vector3 wordPos, string spName, int data)
	{
		passObj _obj = MonoBase.Create3Dobj ("Effect/Other/getResourceLabel");
		GetResourceLabel _ctrl = _obj.obj.GetComponent<GetResourceLabel> ();
		_ctrl.transform.parent = UiManager.Instance.UIEffectRoot.transform;
		Vector3 pos = MissionManager.instance.backGroundCamera .WorldToScreenPoint (wordPos);
		_ctrl.transform.position = UiManager.Instance.gameCamera.ScreenToWorldPoint (pos) + new Vector3 (0, 0.2f, 0f);
		_ctrl.transform.localScale = Vector3.one;
		_ctrl.ico.spriteName = spName;
		_ctrl.label.text = "+" + data.ToString ();
		AddEffect (_ctrl);
		return _ctrl;
	}
	//普通技能显示板
	public SkillNameCtrl CreateSkillNameEffect (CharacterCtrl target, string str)
	{
		if (BattleManager.Instance.isCameraScaleNow == true)
			return null;
		passObj _obj = MonoBase.Create3Dobj ("Effect/Other/skillNameLabel");
		SkillNameCtrl _ctrl = _obj.obj.GetComponent<SkillNameCtrl> ();
		_ctrl.transform.parent = UiManager.Instance.UIEffectRoot.transform;
		Vector3 pos = BattleManager.Instance.BattleCamera.WorldToScreenPoint (target.hitPoint.transform.position + new Vector3 (0, 0.1f, 0.2f));
		_ctrl.transform.position = UiManager.Instance.gameCamera.ScreenToWorldPoint (pos);
		_ctrl.transform.localScale = new Vector3 (0.001f, 0.001f, 0.001f);
		_ctrl.init (target, str);
		AddEffect (_ctrl);
		return _ctrl;
	}
	/** 创建特效控制器 */
	public EffectCtrl CreateEffect (Transform point, string Path,string fileName)
	{
		passObj _obj = MonoBase.Create3Dobj (Path,fileName);
		if (_obj.obj == null) {
			Debug.LogError ("error:" + Path);
			return null;
		}
		return loadEffectCtrl (point,_obj); 
	}
	/** 创建特效控制器 */
	public EffectCtrl CreateEffect (Transform point, string Path)
	{
		passObj _obj = MonoBase.Create3Dobj (Path);
		if (_obj.obj == null) {
			Debug.LogError ("error:" + Path);
			return null;
		}
		return loadEffectCtrl (point,_obj); 
	}
	/// <summary>
	/// 缓存特效控制器 
	/// </summary>
	/// <param name="path">Path.</param>
	/// <param name="callBack">Call back.</param>
	public void CacheEffectCtrl (string[] paths,CallBack callBack)
	{
		EffectManager.Instance.CacheEffect(paths,()=>{
			if(callBack!=null){
				callBack();
				callBack=null;
			}
		});
	}
	/// <summary>
	/// 缓存特效控制器 
	/// </summary>
	/// <param name="path">Path.</param>
	/// <param name="callBack">Call back.</param>
	public void CacheEffectCtrl (string path,CallBack callBack)
	{
		EffectManager.Instance.CacheEffect(path,()=>{
			if(callBack!=null){
				callBack();
				callBack=null;
			}
		});
	}
	/// <summary>
	/// 缓存并创建游戏对象
	/// <param name="path">路径</param>
	/// <param name="callBack">Call back</param>
	/// </summary>
	public void CreateObjByCache (string path,CallBack<passObj> callBack)
	{
		EffectManager.Instance.CacheEffect(path,()=>{
			passObj passobj= MonoBase.Create3Dobj (path);
			if (passobj.obj == null) {
				Debug.LogError ("error:" + path);
				return;
			}
			if(callBack!=null){
				callBack(passobj);
				callBack=null;
			}
		});
	}
	/// <summary>
	/// 缓存并创建特效控制器---快速连续多次缓冲加载请使用CacheEffectCtrl.例如:for中连续加载
	/// <param name="point"></param>
	/// <param name="path">路径</param>
	/// <param name="name">特效名</param>
	/// <param name="callBack">Call back</param>
	/// </summary>
	public void CreateEffectCtrlByCache (Transform point, string path,string name,CallBack<passObj,EffectCtrl> callBack)
	{
		EffectManager.Instance.CacheEffect(path,()=>{
			passObj passobj= MonoBase.Create3Dobj (path,name);
			if (passobj.obj == null) {
				Debug.LogError ("error:" + path);
				return;
			}
			EffectCtrl ctrl=EffectManager.Instance.loadEffectCtrl (point,passobj);
			if(callBack!=null){
				callBack(passobj,ctrl);
				callBack=null;
			}
		});
	}
	/// <summary>
	/// 缓存并创建特效控制器---快速连续多次缓冲加载请使用CacheEffectCtrl.例如:for中连续加载
	/// <param name="point"></param>
	/// <param name="path">路径</param>
	/// <param name="callBack">Call back</param>
	/// </summary>
	public void CreateEffectCtrlByCache (Transform point, string path,CallBack<passObj,EffectCtrl> callBack)
	{
		EffectManager.Instance.CacheEffect(path,()=>{
			passObj passobj= MonoBase.Create3Dobj (path);
			if (passobj.obj == null) {
				Debug.LogError ("error:" + path);
				return;
			}
			EffectCtrl ctrl=EffectManager.Instance.loadEffectCtrl (point,passobj);
			if(callBack!=null){
				callBack(passobj,ctrl);
				callBack=null;
			}
		});
	}
	/** 加载特效控制器  */
	public EffectCtrl loadEffectCtrl (Transform point, passObj _obj)
	{
		if (_obj == null||_obj.obj==null) return null;
		EffectCtrl _ctrl = _obj.obj.GetComponent<EffectCtrl> ();
		_ctrl.transform.parent = point;
		_ctrl.transform.localPosition = Vector3.zero;
		_ctrl.transform.localScale = Vector3.one;
		_ctrl.initEffect (point);
		AddEffect (_ctrl);
		return _ctrl;
	}
}
