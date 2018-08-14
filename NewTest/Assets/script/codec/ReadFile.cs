using System.IO;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public static class ReadFile
{
	public static void CreateFile(string path,string nameserver,string info)
	{
		
		StreamWriter sw;
		string path1 = path+"//"+ nameserver;

		if (File.Exists(path1))
		{
			sw = new StreamWriter(path1);
		} else {
			FileInfo t = new FileInfo(path1);
			sw = t.CreateText();
		}
		
		sw.WriteLine(info);
		sw.Close();
		sw.Dispose();
	} 
	
	
	public static string[] readMsg(string path,string name)
	{
		StreamReader sr =null;
		string[] servers = null;
		try{
			sr = File.OpenText(path+"//"+ name);
		} catch(Exception e) {
			return null;
		}
		string line = sr.ReadToEnd();
		servers = line.Split(',');
	
		sr.Close();
		sr.Dispose();
		
		return servers;
	}
	
	public static Json_ServerInfo[] readFile()
	{
		TextAsset asset = (TextAsset) Resources.Load("server/serverlist", typeof(TextAsset));
//		if (GameManager.Instance == null || GameManager.Instance.serverAsset == null) 
//		{
//			return null;
//		}
		
		string[] servers = null;
		
		Json_ServerInfo[] serverc = null;
		
		string line = asset.text;
		
		servers = line.Split('\n');
		string[] serverMsg = null;
		
		if (servers != null && servers.Length > 0)
		{
			serverc = new Json_ServerInfo[servers.Length];
			
			for (int i = 0 ; i < servers.Length; i++)
			{
				serverMsg = servers[i].Split(',');
				if (serverMsg.Length != 4)
					continue;
				serverc [i] = new Json_ServerInfo(serverMsg[0],serverMsg[1],
				                                  int.Parse(serverMsg[2]),serverMsg[3]);
			}
			
		}
		
		return serverc.Where(t => t != null).ToArray();
	}
	
	public static  Json_ServerInfo[] readFile(string path,string name)
	{
		StreamReader sr =null;
		string[] servers = null;
		try{
			sr = File.OpenText(path+"//"+ name);
		} catch(Exception e) {
			return null;
		}
		Json_ServerInfo[] serverc = null;

		string line = sr.ReadToEnd();

		servers = line.Split('\n');
		string[] serverMsg = null;

		if (servers != null && servers.Length > 0)
		{
			serverc = new Json_ServerInfo[servers.Length];

			for (int i = 0 ; i < servers.Length; i++)
			{
				serverMsg = servers[i].Split(',');
				if (serverMsg.Length != 4)
					continue;
				serverc [i] = new Json_ServerInfo(serverMsg[0],serverMsg[1],
				                                  int.Parse(serverMsg[2]),serverMsg[3]);
			}

		}
		
		sr.Close();
		sr.Dispose();
		
		return serverc.Where(t => t != null).ToArray();
	}  

	public static void DeleteData(string path,string nameserver,string deldata,Action func)
	{
		string path1 = path+"//"+nameserver;
		if (File.Exists(path1))
		{
			string[] ary = File.ReadAllLines(path1,System.Text.Encoding.Default);
			ary = ary.Where(t =>{ if (t.Contains(deldata)) func(); return !t.Contains(deldata);}).ToArray();
			string str = string.Join("\n",ary);
			File.WriteAllText(path1,str);

		} else {
			Log.debug("delete msg filed! delete target path isn't Exist!");
		}

	}
	
	public static void DeleteFile(string path,string nameserver)
	{
		File.Delete(path+"//"+ nameserver);
	}
}
