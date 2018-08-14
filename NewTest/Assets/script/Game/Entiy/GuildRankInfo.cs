using UnityEngine;
using System.Collections;

/**
 * 公会排名信息类
 * @author 汤琦
 **/
public class GuildRankInfo 
{
	public GuildRankInfo(){
	}
	public GuildRankInfo(string uid ,int level ,string name ,int membership ,int membershipMax ,string declaration,int liveness)
	{
		this.uid = uid;
		this.level = level;
		this.name = name;
		this.membership = membership;
		this.membershipMax = membershipMax;
		if(declaration == "[]")
			declaration = "";
		this.declaration = declaration;
		this.liveness = liveness;
	}
	public string uid = "";//公会id
	public int level = 0;//公会等级
	public string name = "";//公会名称
	public int membership = 0;//人数
	public int membershipMax = 0;//人数上限
	public string declaration = "";//宣言
	public bool isApply = false;//是否申请过
	public int liveness = 0;//活跃度

}
