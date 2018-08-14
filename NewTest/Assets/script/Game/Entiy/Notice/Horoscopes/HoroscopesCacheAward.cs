using System;

public class HoroscopesCacheAward
{
	public HoroscopesCacheAward (ErlArray array)
	{
		resultType = StringKit.toInt ((array.Value [0] as ErlType).getValueString ());
		if (resultType == ROLE)
			user = new SimpleUser (array.Value [1] as ErlArray);
		else if (resultType == NVSHEN)
			star = StringKit.toInt ((array.Value [1] as ErlType).getValueString ());
		awards = new int[4] {StringKit.toInt ((array.Value [2] as ErlType).getValueString ()),StringKit.toInt ((array.Value [3] as ErlType).getValueString ()),
			StringKit.toInt ((array.Value [4] as ErlType).getValueString ()),StringKit.toInt ((array.Value [5] as ErlType).getValueString ())};
	}

	//摇一摇结果临时存储，展示使用后记得清理
	private int resultType;//1玩家2女神
	private SimpleUser user; // 根据resultType user和star二选一
	private int star;
	private int[] awards;//[普通,幸运,相同星座,女神]

	public const int ROLE = 1, NVSHEN = 2;

	public int getResultType ()
	{
		return resultType;
	}

	public SimpleUser getUser ()
	{
		return user;
	}

	public int getStar ()
	{
		return star;
	}

	public int[] getAwards ()
	{
		return awards;
	}
}

