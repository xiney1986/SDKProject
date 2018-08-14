using System;
using System.Collections;
 
public class ServiceManager
{
	
	//单例
	private static ServiceManager _Instance;
	private static bool _singleton = true;
	private Hashtable services;
	
	public static ServiceManager Instance {
		get {
			
			if (_Instance == null) {
				_singleton = false;
				_Instance = new ServiceManager ();
				_singleton = true;
				return _Instance;
			} else
				return _Instance;
		}
		set { 
			_Instance = value;
		}
	}
	
	public ServiceManager ()
	{
		if (_singleton)
			return;  
		registService ();
	} 
	
	//服务器广播消息
	public void severRadio (Connect c, object o)
	{
		ErlKVMessage message = o as ErlKVMessage;
		BaseFPort service = getServiceByCmd (message.Cmd);
		if (service == null) {
			//没有找到对应的服务
		} else {
			service.read (message);
		}
	}
	
	public BaseFPort getServiceByCmd (string cmd)
	{
		return services [cmd] as BaseFPort;
	}
	
	//注册服务 每个服务都需要注册
	private void registService ()
	{
		services = new Hashtable ();
		services.Add (FPortService.BATTLE_REPORT, new BattleReportService ());//注册战报服务
		services.Add (FPortService.FUBEN_EVENT, new FuBenDoEventService ());//注册副本事件服务
		services.Add (FPortService.AWARD, new AwardService ());//注册奖励服务
		services.Add (FPortService.ACCOUNT, new AccountService ());//注册账号服务
		services.Add (FPortService.USER_UPDATE, new UserUpdateService ());//注册玩家数据更新服务
		services.Add (FPortService.STORAGE_UPDATE, new StorageUpdateService ());//注册仓库数据更新服务
		services.Add (FPortService.UPDATE, new UpdateService ());//注册钱,经验等非随时间变化而变化的资源更新服务
		services.Add (FPortService.RADIO, new RadioService ());//注册广播服务
		services.Add (FPortService.CHAT, new ChatService ());//注册聊天服务
		services.Add (FPortService.ACHIEVE, new TaskService ());//注册任务服务
		services.Add (FPortService.MAIL, new MailService ());//注册邮件服务
		services.Add (FPortService.FRIENDS, new FriendsService ());//注册邮件服务
		services.Add (FPortService.SHARE, new FriendsShareService ());//注册邮件服务
		services.Add (FPortService.GUILDMSG,new GuildService());//注册公会服务
		services.Add (FPortService.STAR,new GoddessAstrolabeService());//女神星盘服务
		services.Add (FPortService.RECHARGE,new RechargeService());//充值服务
		services.Add (FPortService.UPDATE_STORAGE_PROPS,new StoragePropsService());//增加物品服务
		services.Add(FPortService.LADDER_ATTR_EFFECT,new LaddersAttrEffectUpdateFPort());//称号加成服务
		services.Add(FPortService.ARENA_TIP,new ArenaTipServerFPort());//竞技场提示
        services.Add(FPortService.HAPPY_SUNDAY, new HappySundayService());//欢庆周末活动
        services.Add(FPortService.DOUBLE_RMB, new DoubleRMBService()); //双倍充值
		services.Add(FPortService.PRACTICETIME,new PracticetimeService() );//修炼次数
		services.Add(FPortService.GM_ACTIVE,new GMActiveService() );//GM修改活动
		services.Add (FPortService.GM_UPDATE_CONFIG, new GMUpdateConfigService ()); //gm修改配置表
		services.Add (FPortService.SUPERDRAW, new SuperDrawActiveService ()); //超级奖池
        services.Add(FPortService.UPDATE_GOD_STATE,new UpdateGodsCloseState());
		services.Add(FPortService.UPDATE_LASTBATLE,new UpdateLastBattleService());
	}
} 

