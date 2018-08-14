using System;
using UnityEngine;

/**
 * 常用颜色列表  直接用于UILabel
 * */
public class Colors
{

	public const string WHITE = "[FFFFFF]";//白色
	public const string RED = "[FF0000]";//纯红
	public const string GREEN = "[00F000]";//文字用绿色
	public const string GRENN="[3A9663]";
	public const string REDD="[C65843]";
	public const string BLUE = "[0000FF]";//纯蓝
	public const string ORANGE = "[E48902]";
	public const string DEEP_GRENN = "[3A9663]";//深绿色
	

	public static Color BUTTON_TEXT_OUTLINE_NROMAL = new Color (0.031f, 0.246f, 0.475f, 1);//65,55,41
	public static Color BUTTON_TEXT_OUTLINE_DISABLEL = new Color (0.22f, 0.22f, 0.22f, 1);//65,55,41

	public static Color BUTTON_TEXT_NROMAL = new Color (1f, 1f, 1f, 1);//221,210,165
	public static Color BUTTON_TEXT_NROMALLINE = new Color (0.25f, 0.2f, 0.16f, 1);//65,55,41
	public static Color BUTTON_TEXT_DISABLEL = new Color (0.6f, 0.6f, 0.6f, 1);
	public static Color BUTTON_TEXT_DISABLELLINE = Color.black;

	public static Color BUTTON_BG_NROMAL = new Color (1f, 1f, 1f, 1);//221,210,165

	public static Color BACKGROUND_DARK = new Color (0.07f, 0.07f, 0.07f, 1);
	public static Color BACKGROUND_LIGHT = new Color (0.1f, 0.1f, 0.1f, 1);
	public static Color BACKGROUND_BLACK = new Color (0, 0, 0, 1);
	public static Color BACKGROUND_NONE = new Color (0, 0, 0, 0);
	public static Color BACKGROUND_HALF = new Color (0.5f, 0.5f, 0.5f, 1);
	public static Color BACKGROUND_BLACKALPHA = new Color (0f, 0f, 0f, 0.9f);
	public static Color SERVER_HOT = new Color (0.97f, 0.36f, 0.36f, 1f);
	public static Color SERVER_NEW = new Color (0.21f, 0.81f, 0.32f, 1f);
	public static Color SERVER_NORMAL = new Color (0.94f, 0.70f, 0.24f, 1f);

	public static string CHAT_WORLD = "[418159]";//世界
    public static string CHAT_FRIEND = "[c040b7]"; //好友私聊
	public static string CHAT_SYSTEM = "[ff0000]";//系统
    public static string CHAT_UNION = "[0000FF]";//公会
	public static string CHAT_RADIO = "[20809b]";//广播
	public static string CHAT_VIP = "[a02a2a]";//VIP染色
	public static string CHAT_CONTENT = "[6A4540]";
	public static string MISSSION_GREEN = "[06ff00]";
	public static string CHAT_GRAY = "[777777]";
	public static string CHAT_USER = "[A55543]";

	public static Color EVOLUTIONCOLOR_BLUE = new Color (0, 1f, 1f, 1f);//突破0次蓝球
	public static Color EVOLUTIONCOLOR_YELLOW = new Color (1f, 1f, 0, 1f);//突破2次以上黄球
	public static Color EVOLUTIONCOLOR_PURPLE = new Color (1f, 0, 1f, 1f);//突破1次紫球
	public static Color EVOLUTIONCOLOR_RED = new Color (1f, 0, 0f, 1f);//红色
    public static Color EVOLUTIONCOLOR_GTEEN=new Color(0,1f,0,1f);
	public Colors ()
	{
	}
} 

