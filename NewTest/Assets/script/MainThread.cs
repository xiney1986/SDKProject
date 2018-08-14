using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// UI线程工具类
/// </summary>
public class MainThread : MonoBehaviour
{
    private static MainThread instance;

    LinkedList<CallBack> list;

    void Awake()
    {
        instance = this;
        list = new LinkedList<CallBack>();
    }

    void Update()
    {
        lock (list)
        {
            LinkedListNode<CallBack> node = list.First;
            while (node != null)
            {
                try
                {
                    node.Value();
                } catch (System.Exception ex)
                {
                    Debug.LogError(ex.Message);
                }
                node = node.Next;
            }
            list.Clear();
        }
    }


    public void post(CallBack callback)
    {
        lock (list)
        {
            list.AddLast(callback);
        }
    }

    /// <summary>
    /// 提交一个方法到UI线程执行
    /// </summary>
    public static void Post(CallBack callback)
    {
        if (instance != null)
        {
            instance.post(callback);
        }
    }
}
