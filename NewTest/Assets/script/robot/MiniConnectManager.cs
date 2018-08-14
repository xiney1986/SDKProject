using UnityEngine;
using System.Collections;

public class MiniConnectManager : MonoBehaviour
{
    public static bool IsRobot;
    public static string ip;
    public static int port;
    public static long now;

    public GameObject content;
    public GameObject itemPreafb;
    
    public int startId;
    public int userCount;
	public string _ip;
    public int _port;
    public bool log;
    int fps;
    int showFps;
    float fpsTime;

    void Start()
	{
        IsRobot = !log;
        ip = _ip;
        port = _port;

        DataAccess.getInstance ().defaultHandle = OnReceveRadio; 

        StartCoroutine(BuildItems());
    }

    void Update()
    {
        fpsTime += Time.deltaTime;
        if (fpsTime > 1)
        {
            fpsTime = 0;
            showFps = fps;
            fps = 0;
            Debug.LogError("fight count : "+showFps+"/s");
        }
		now = ServerTimeKit.getMillisTime();
    }


    IEnumerator BuildItems()
    {
        int end = startId + userCount;
        float y = 414;
        float x = -158;
        for (int i = startId; i < end; i++)
        {
            GameObject obj = NGUITools.AddChild(content,itemPreafb);
            obj.name = i.ToString();
            obj.SetActive(true);
            MiniConnectItem item = obj.GetComponent<MiniConnectItem>();
            item.Init(ip,port,i);

            obj.transform.localPosition = new Vector3(x,y,0);
            x *= -1;
            if(x < 0)
                y -= 80;

            yield return 1;
        }
    }

    public void OnReceveRadio(Connect c ,object obj)
    {
        ErlKVMessage msg = obj as ErlKVMessage;
        if(msg.getValue("report") != null)
        {
            fps++;
        }
    }
}
