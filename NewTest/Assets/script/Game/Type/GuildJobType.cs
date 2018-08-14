using UnityEngine;
using System.Collections;

public class GuildJobType 
{
	public const int JOB_PRESIDENT = 1;//会长
	public const int JOB_VICE_PRESIDENT = 2;//副会长
	public const int JOB_OFFICER = 3;//官员
	public const int JOB_COMMON = 4;//普通

	public static string getJobName(int job)
	{
		switch(job)
		{
		case JOB_PRESIDENT:
			return LanguageConfigManager.Instance.getLanguage("Guild_13");
		case JOB_VICE_PRESIDENT:
			return LanguageConfigManager.Instance.getLanguage("Guild_14");
		case JOB_OFFICER:
			return LanguageConfigManager.Instance.getLanguage("Guild_15");
		case JOB_COMMON:
			return LanguageConfigManager.Instance.getLanguage("Guild_16");
		default:
			return "";
		}
	}
}
