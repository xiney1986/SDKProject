
using System;
public class MissionAward
{
	public static MissionAward Instance
	{
		get
		{
			return SingleManager.Instance.getObj("MissionAward") as MissionAward;
		}
	}
	public Award[] parcticeAwards;

}


