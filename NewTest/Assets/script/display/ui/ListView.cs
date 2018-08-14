using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ListView : MonoBehaviour {
	
	public delegate GameObject CreateViewCallback(object obj,GameObject view);

	private class Item
	{
		public LinkedListNode<object> objNode;
		public GameObject view;
		public Vector4 rect; 
	}

	public GameObject content;
	public float factor = 0.5f; //超出窗口的倍数后移除元素,反之创建.
	public float space; //间距
	public bool cacheView = true; //元素UI重用


	UIScrollView scrollView;
	UIPanel panel;
	CreateViewCallback createViewCallBack;
	LinkedList<object> dataList = new LinkedList<object>();
	LinkedList<Item> viewList = new LinkedList<Item>();
	bool horizontal;
	LinkedListNode<Item> first;
	LinkedListNode<Item> last;
	Stack<GameObject> cacheViewStack = new Stack<GameObject>();
	Vector2 lastOffset = new Vector2(-1000,-100000);

	public int Count{get{return dataList.Count;}}

	void Awake()
	{
		scrollView = gameObject.GetComponent<UIScrollView>();
		panel = gameObject.GetComponent<UIPanel>();
		horizontal = scrollView.movement == UIScrollView.Movement.Horizontal;
	}

	void Start () {
		while (UpdateContent());
		StartAnimation ();
	}

	void StartAnimation()
	{
		LinkedListNode<Item> first = viewList.First;
		if (first != null) {
			LinkedListNode<Item> node = viewList.First.Next;
			while (node != null) {
				Vector3 pos = node.Value.view.transform.localPosition;
				node.Value.view.transform.localPosition = node.Previous.Value.view.transform.localPosition;
				SpringPosition.Begin(node.Value.view, pos, 15f);
				//iTween.MoveFrom (node.Value.view,iTween.Hash("islocal",true,"position",first.Value.view.transform.localPosition,"easeType",iTween.EaseType.easeOutCubic,"time",0.3f));
				node = node.Next;
			}
		}

	}

	void Update()
	{
		UpdateContent ();
	}

	bool UpdateContent () {
		bool result = false;

		first = viewList.First;  
		last = viewList.Last;
		bool addFirst = false;
		bool addLast = false;
		bool removeFirst = false;
		bool removeLast = false;
		if(first == null && dataList.First != null)
			addFirst = true;
		else if(first != null)
		{
			Vector4 rect = first.Value.rect;
			if(horizontal)
			{
				if(rect.x - panel.clipOffset.x - panel.clipRange.x - rect.w > -panel.clipRange.z * factor)
				{
					addFirst = true;
				}
				else if(first.Next != null)
				{
					rect = first.Next.Value.rect;
					if(rect.x - panel.clipOffset.x - panel.clipRange.x - rect.w < -panel.clipRange.z * factor)
						removeFirst = true;
				}
			}
			else
			{
				if( rect.y - panel.clipOffset.y - panel.clipRange.y + rect.z  < panel.clipRange.w * factor)
				{
					addFirst = true;
				}
				else if(first.Next != null)
				{
					rect = first.Next.Value.rect;
					if( rect.y - panel.clipOffset.y - panel.clipRange.y + rect.z  > panel.clipRange.w * factor)
						removeFirst = true;
				}

			}
		}
		if(last != null)
		{
			Vector4 rect = last.Value.rect;
			if(horizontal)
			{
				if(rect.x - panel.clipOffset.x  - panel.clipRange.x + rect.w < panel.clipRange.z*2 * factor)
				{
					addLast = true;
				}
				else if(last.Previous != null)
				{
					rect = last.Previous.Value.rect;
					if(rect.x - panel.clipOffset.x  - panel.clipRange.x + rect.w > panel.clipRange.z*2 * factor)
						removeLast = true;
				}
			}
			else
			{
				if(rect.y - panel.clipOffset.y - panel.clipRange.y - rect.z > -panel.clipRange.w*2 * factor)
				{
					addLast = true;
				}
				else if(last.Previous != null)
				{
					rect = last.Previous.Value.rect;
					if(rect.y - panel.clipOffset.y - panel.clipRange.y - rect.z < -panel.clipRange.w*2 * factor)
						removeLast = true;
				}
			}
		}

		if(addFirst)
		{
			Item item = null;
			if(first == null && dataList.First != null)
			{
				item = new Item();
				item.objNode = dataList.First;
			}
			else if(first.Value.objNode.Previous != null)
			{
				item = new Item();
				item.objNode = first.Value.objNode.Previous;
			}
			if(item != null)
			{
				GameObject contentView = cacheView && cacheViewStack.Count > 0 ? cacheViewStack.Pop() : null;
				item.view = createViewCallBack(item.objNode.Value,contentView);
				item.view.transform.parent = content.transform;
				item.view.transform.localScale = Vector3.one;
				item.view.SetActive(true);

				item.rect = new Vector4();
				Bounds size = NGUIMath.CalculateRelativeWidgetBounds(item.view.transform);
				item.rect.w = size.extents.x;
				item.rect.z = size.extents.y;
				if(horizontal)
					if(first == null)
						item.rect.x = item.rect.w;
					else
						item.rect.x = first.Value.rect.x - first.Value.rect.w - item.rect.w - space;
				else
					if(first == null)
						item.rect.y = item.rect.z;
					else
						item.rect.y = first.Value.rect.y + first.Value.rect.z + item.rect.z + space;
				item.view.transform.localPosition = new Vector3(item.rect.x,item.rect.y,0);
				viewList.AddFirst(item);
				result = true;
			}
		}
		else if(removeFirst)
		{
			if(first != null)
			{
				if(cacheView && cacheViewStack.Count < 10){
					first.Value.view.SetActive(false);
					cacheViewStack.Push(first.Value.view);
				}else{
					Destroy(first.Value.view);
				}
				viewList.RemoveFirst();
			}
		}

		if(addLast)
		{
			Item item = null;
			if(last != null && last.Value.objNode.Next != null)
			{
				item = new Item();
				item.objNode = last.Value.objNode.Next;
			}
			if(item != null)
			{
				GameObject contentView = cacheView && cacheViewStack.Count > 0 ? cacheViewStack.Pop() : null;
				item.view = createViewCallBack(item.objNode.Value,contentView);
				item.view.transform.parent = content.transform;
				item.view.transform.localScale = Vector3.one;
				item.view.SetActive(true);
				
				item.rect = new Vector4();
				Bounds size = NGUIMath.CalculateRelativeWidgetBounds(item.view.transform);
				item.rect.w = size.extents.x;
				item.rect.z = size.extents.y;
				if(horizontal)
					item.rect.x = last.Value.rect.x + last.Value.rect.w + item.rect.w + space;
				else
					item.rect.y = last.Value.rect.y - last.Value.rect.z - item.rect.z - space;
				item.view.transform.localPosition = new Vector3(item.rect.x,item.rect.y,0);
				viewList.AddLast(item);
				result = true;
			}
		}
		else if(removeLast)
		{
			if(last != null)
			{
				if(cacheView && cacheViewStack.Count < 10){
					last.Value.view.SetActive(false);
					cacheViewStack.Push(last.Value.view);
				}else{
					Destroy(last.Value.view);
				}
				viewList.RemoveLast();
			}
		}

		return result;
	}

	public void SetCreateViewCallBack(CreateViewCallback createViewCallBack)
	{
		this.createViewCallBack = createViewCallBack;
	}

	public void Add(object obj)
	{
		dataList.AddLast(obj);
	}

	public void AddFirst(object obj)
	{
		dataList.AddFirst(obj);
	}

	public void AddAfter(object aferObj,object obj)
	{
		LinkedListNode<object> node = dataList.First;
		while(node != null)
		{
			if(node.Value == aferObj)
			{
				dataList.AddAfter(node,new LinkedListNode<object>(obj));
				break;
			}
			node = node.Next;
		}
	}

	public void RemoveFirst()
	{
		if(viewList.First.Value.objNode == dataList.First)
		{
			Destroy(viewList.First.Value.view);
			viewList.RemoveFirst();
		}
		dataList.RemoveFirst();
	}
	 
	public void clear()
	{
		dataList.Clear (); 
		viewList.Clear ();
		Utils.DestoryChilds (content);
	}

	public void RemoveLast()
	{
		if(viewList.Last.Value.objNode == dataList.Last)
		{
			Destroy(viewList.Last.Value.view);
			viewList.RemoveLast();
		}
		dataList.RemoveLast();
	}

	/// <summary>
	/// 滑动条置底
	/// </summary>
	/// <param name="forcibly">在不够滑动的情况下是否强制置底</param>
	public void ScrollToEnd(bool  forcibly)
	{
		Update();
		scrollView.ResetPosition();
		if(!horizontal && scrollView.shouldMoveVertically || horizontal && scrollView.shouldMoveHorizontally)
		{
			scrollView.SetDragAmount(horizontal ? 1 : 0,horizontal ? 0 : 1,false);
		}
	}

}

