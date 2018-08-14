using UnityEngine;
using System.Collections;

/// <summary>
/// 卡片星魂槽
/// </summary>
public class StarSoulBore {

	/* fields */
	/** 星魂uid */
	string uid;
	/** 卡片星魂槽位置 */
	int index;
    /** 星魂的sid*/
    int sid;
    /**星魂的经验 */
    long exp;

	/* methods */
	public StarSoulBore() {
	}
	public StarSoulBore(string uid,int index) {
		this.uid=uid;
		this.index=index;
	}
    public StarSoulBore(int sid,int index,long exp) {
        this.sid = sid;
        this.index = index;
        this.exp = exp;
    }
	/** 序列化读取可变属性数据 */
	public void bytesRead (int j, ErlArray ea) {
		this.index = StringKit.toInt (ea.Value [j++].getValueString ());
		this.uid = ea.Value [j++].getValueString ();
	}
    public void bytesOtherRead(int j,ErlArray er) {
        this.index = StringKit.toInt(er.Value[j++].getValueString());
        this.sid = StringKit.toInt(er.Value[j++].getValueString());
        this.exp = StringKit.toLong(er.Value[j++].getValueString());
    }
	/// <summary>
	/// 校验是否为指定星魂槽
	/// </summary>
	/// <param name="index">Index.</param>
	public bool checkStarSoulBoreByIndex(int index) {
		if (this.index == index)
			return true;
		return false;
	}
	/// <summary>
	/// 校验是否为指定星魂槽
	/// </summary>
	/// <param name="uid">uid</param>
	public bool checkStarSoulBoreByUid(string uid) {
		if (this.uid == uid)
			return true;
		return false;
	}
	public string ToString(){
		return "[uid=" + uid + ",index=" + index + "]";
	}


	/* properties */
	/** 卡片星魂槽位置 */
	public int getIndex() {
		return index;
	}
	/** 设置卡片星魂槽的星魂uid */
	public void setUid(string uid) {
		this.uid = uid;
	}
	/** 星魂uid */
	public string getUid() {
		return uid;
	}
    public int getSid() {
        return sid;
    }
    public long getExp() {
        return exp;
    }
}
