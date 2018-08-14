using System;
 
/**
 * 通讯常量
 * @author longlingquan
 * */
public class FPortGlobal
{ 
	
	//=============================sys==============================
	public const string SYSTEM_INFO_ERROR = "info_error";//传递的参数异常
	public const string SYSTEM_OK = "ok";//
	public const string SYSTEM_FALSE_L="false";//
	//=============================sys over==========================
	//=============================login============================= 
	public const string LOGIN_CREATE_USER_OK = "create_user_ok";//没有账号,创建用户成功(不需要验证)
	public const string LOGIN_NO_USER = "no_user";//没有账号,跳转到注册(需要验证)
	public const string LOGIN_PASSWORD_ERROR = "password_error";//用户密码错误
	public const string LOGIN_NO_ROLE = "no_role";//没有角色,跳转角色创建
	public const string LOGIN_LOGIN_OK = "login_ok";//登录成功
	public const string LOGIN_RELOGIN_OK = "relogin_ok";//断开重连
	public const string LOGIN_CLOSE = "login_limit";//服务器暂未开放
	public const string LOGIN_TRUST = "trusting";//玩家被Gm托管中
	public const string LOGIN_SIGERROR = "sig_error";//签名错误
	public const string LOGIN_LIMIT = "login_limit";//登陆限制
	public const string LOGIN_COUNT = "limit_count";// 服务器人数已满
	public const string LOGIN_CREATE = "limit_create";// 创建限制
	public const string LOGIN_USER_DISABLE = "user_disable";// 冻结账户
	//=============================login over==========================
	//=============================role ==============================
	public const string ROLE_CREATE_ROLE_OK = "create_role_ok";//创建角色成功 
	public const string ROLE_ALREADY_CREATE_ROLE = "already_create_role";//已经创建过主角
	public const string ROLE_FULL = "full";//服务器人数已满
	//=============================role over===========================
	//=============================register============================
	public const string REGISTER_CREATE_USER_OK = "create_user_ok";//创建账号成功
	public const string REGISTER_USER_EXIST = "exist";//用户已存在 
	//=============================register  over========================
	//======================army===============================
	public const string ARMY_LENGTH_ERROR = "arrays_length_error";//角色队伍个数已达上线,不能继
	public const string ARMY_INDEX_ERROR = "index_error";//没有队伍需要匹配的阵型
	public const string ARMY_CHECK_ERROR = "check_error";//角色条件没达到,不能使用该阵型
	public const string ARMY_ADD_SUCCESS = "array_add_success";//队伍添加成功
	public const string ARMY_LENGTH_ALTERNATE_ERROR = "length_alternate_error";//队伍主力个数错误
	public const string ARMY_LENGTH_LEAD_ERROR = "length_lead_error";//队伍替补个数错误
	public const string ARMY_SCHEME_ERROR = "scheme_error";//队伍Id错误 
	public const string ARMY_SAME_CARD = "same_card";//同意队伍里相同卡片只能占一个位置
	public const string ARMY_NO_CARD = "no_card";//没有此卡片
	public const string ARMY_NOT_BEAST = "no_beast";//没有此召唤兽
	public const string ARMY_SUCCESS = "success";//配置成功 
	public const string ARMY_USED = "array_used";//使用中 不能修改
	//======================army over===========================
	//======================FuBenUseArray============================
	public const string FUBEN_USEARRAY_SUCCESS = "success";//使用队伍成功
	public const string FUBEN_USEARRAY_NOFB = "no_fb";//没有副本
	public const string FUBEN_USEARRAY_ERR = "err";//参数错误
	public const string FUBEN_USEARRAY_EXIST = "array_not_exist";//阵型不存在
	//======================FuBenUseArray over=======================
	//======================intensify============================
	public const string INTENSIFY_SUCCESS = "ok";//成功
	public const string INTENSIFY_LIMIT = "card_busy";//卡片在上阵
	public const string INTENSIFY_MAINLOSE = "eat_main_card";//不能吃主卡
	public const string INTENSIFY_PLENGTHLIMIT = "pability_length_error";//长度错误
	public const string INTENSIFY_ALENGTHLIMIT = "ability_length_error";//属性错误
	public const string INTENSIFY_OUTCASH = "lack_of_money";//缺钱
	public const string INTENSIFY_ERROR = "error";
	public const string INTENSIFY_NOTEATANYMORE = "not_eat_anymore";//满级
	public const string INTENSIFY_EQUIP_NOHAVE = "equip_not_exist";//装备不存在
	public const string INTENSIFY_EQUIP_SERROR = "state_error";//装备状态异常
	public const string INTENSIFY_EQUIP_LLEVEL = "level_limit";//装备满级
	public const string INTENSIFY_FOODUID_ERROR = "";//食物卡uid异常
	public const string INTENSIFY_CARD_NONENTITY = "card_not_exist";//卡片不存在
	public const string INTENSIFY_MAINCARD_ERROR = "main_card_error";//主卡不能吃
	public const string INTENSIFY_NO_BEAST = "no_beast";//没有召唤兽
	public const string INTENSIFY_LIMIT_GOODS = "goods_limit";//缺少道具
	public const string INTENSIFY_CARD_SERROR = "card_state_error";//卡片状态异常
	public const string INTENSIFY_CARD_SIDERROR = "card_sid_error";//卡片sid和条件异常
	public const string INTENSIFY_EQUIP_SIDERROR = "equip_sid_error";//装备sid和条件异常
	public const string INTENSIFY_LIMIT_MONEY = "money_limit";//钞票不足
	//======================intensify over==========================
	//======================BuyGoods ==============================
	public const string SHOP_BUY_TIME_LIMIT = "time_limit";//购买商品时间限制
	public const string SHOP_BUY_GOODS_SID_ERROR = "goods_id_error";//商品sid错误
	public const string SHOP_BUY_CONSUME_LIMIT = "consume_limit";//消耗不足
	public const string SHOP_STORAGE_NOT_ENOUGH = "storage_not_enough";//仓库空间不足
	public const string SHOP_SHOP_TYPE_ERROR = "type_error";//购买商品shopType错误 
	//======================BuyGoods over===========================
	//======================equip===================================
	public const string EQUIP_NOHAVE = "no_equipment";//脱装备时，装备不存在
	public const string EQUIP_NOEXIST = "equipment_not_exist";//装备不存在
	public const string EQUIP_BEUSED = "equipment_be_used";//装备已使用
	public const string CARD_SID_NOEQUAL = "cardsid_not_equal";//卡片sid不相等
	public const string PART_NOT_SUIT = "part_not_suit";//部位不一致
	//======================equip over===================================
	//======================beast===================================
	public const string BEAST_LEVEL = "level_limit";//角色等级不够
	//======================beast over===================================
} 

