//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

#if !UNITY_3_5 && !UNITY_FLASH
#define DYNAMIC_FONT
#endif

using UnityEngine;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Text list can be used with a UILabel to create a scrollable multi-line text field that's
/// easy to add new entries to. Optimal use: chat window.
/// </summary>

[AddComponentMenu("NGUI/UI/Text List")]
public class UITextList : MonoBehaviour
{
	public enum Style
	{
		Text,
		Chat,
	}

	/// <summary>
	/// Label the contents of which will be modified with the chat entries.
	/// </summary>

	public UILabel textLabel;

	/// <summary>
	/// Vertical scroll bar associated with the text list.
	/// </summary>

	public UIProgressBar scrollBar;

	/// <summary>
	/// Text style. Text entries go top to bottom. Chat entries go bottom to top.
	/// </summary>

	public Style style = Style.Text;

	/// <summary>
	/// Maximum number of chat log entries to keep before discarding them.
	/// </summary>

	public int paragraphHistory = 50;

	// Text list is made up of paragraphs
	protected class Paragraph
	{
		public string text;		// Original text
		public string[] lines;	// Split lines
	}

	protected char[] mSeparator = new char[] { '\n' };
	protected BetterList<Paragraph> mParagraphs = new BetterList<Paragraph>();
	protected float mScroll = 0f;
	protected int mTotalLines = 0;
	protected int mLastWidth = 0;
	protected int mLastHeight = 0;

	/// <summary>
	/// Whether the text list is usable.
	/// </summary>

#if DYNAMIC_FONT
	public bool isValid { get { return textLabel != null && textLabel.ambigiousFont != null; } }
#else
	public bool isValid { get { return textLabel != null && textLabel.bitmapFont != null; } }
#endif

	/// <summary>
	/// Relative (0-1 range) scroll value, with 0 being the oldest entry and 1 being the newest entry.
	/// </summary>

	public float scrollValue
	{
		get
		{
			return mScroll;
		}
		set
		{
			value = Mathf.Clamp01(value);

			if (isValid && mScroll != value)
			{
				if (scrollBar != null)
				{
					scrollBar.value = value;
				}
				else
				{
					mScroll = value;
					UpdateVisibleText();
				}
			}
		}
	}

	/// <summary>
	/// Height of each line.
	/// </summary>

	protected float lineHeight { get { return (textLabel != null) ? textLabel.fontSize + textLabel.spacingY : 20f; } }

	/// <summary>
	/// Height of the scrollable area (outside of the visible area's bounds).
	/// </summary>

	protected int scrollHeight
	{
		get
		{
			if (!isValid) return 0;
			int maxLines = Mathf.FloorToInt((float)textLabel.height / lineHeight);
			return Mathf.Max(0, mTotalLines - maxLines);
		}
	}

	/// <summary>
	/// Clear the text.
	/// </summary>

	public void Clear ()
	{
		mParagraphs.Clear();
		UpdateVisibleText();
	}

	/// <summary>
	/// Automatically find the values if none were specified.
	/// </summary>

	void Start ()
	{
		if (textLabel == null)
			textLabel = GetComponentInChildren<UILabel>();

		if (scrollBar != null)
			EventDelegate.Add(scrollBar.onChange, OnScrollBar);

		textLabel.overflowMethod = UILabel.Overflow.ClampContent;

		if (style == Style.Chat)
		{
			textLabel.pivot = UIWidget.Pivot.BottomLeft;
			scrollValue = 1f;
		}
		else
		{
			textLabel.pivot = UIWidget.Pivot.TopLeft;
			scrollValue = 0f;
		}
	}

	/// <summary>
	/// Keep an eye on the size of the label, and if it changes -- rebuild everything.
	/// </summary>

	void Update ()
	{
		if (isValid)
		{
			if (textLabel.width != mLastWidth || textLabel.height != mLastHeight)
			{
				mLastWidth = textLabel.width;
				mLastHeight = textLabel.height;
				Rebuild();
			}
		}
	}

	/// <summary>
	/// Allow scrolling of the text list.
	/// </summary>

	public void OnScroll (float val)
	{
		int sh = scrollHeight;

		if (sh != 0)
		{
			val *= lineHeight;
			scrollValue = mScroll - val / sh;
		}
	}

	/// <summary>
	/// Allow dragging of the text list.
	/// </summary>

	public void OnDrag (Vector2 delta)
	{
		int sh = scrollHeight;

		if (sh != 0)
		{
			float val = delta.y / lineHeight;
			scrollValue = mScroll + val / sh;
		}
	}

	/// <summary>
	/// Delegate function called when the scroll bar's value changes.
	/// </summary>

	void OnScrollBar ()
	{
		mScroll = UIScrollBar.current.value;
		UpdateVisibleText();
	}

	/// <summary>
	/// Add a new paragraph.
	/// </summary>

	public void Add (string text) { Add(text, true); }

	/// <summary>
	/// Add a new paragraph.
	/// </summary>

	protected void Add (string text, bool updateVisible)
	{
		Paragraph ce = null;

		if (mParagraphs.size < paragraphHistory)
		{
			ce = new Paragraph();
		}
		else
		{
			ce = mParagraphs[0];
			mParagraphs.RemoveAt(0);
		}

		ce.text = text;
		mParagraphs.Add(ce);
		Rebuild();
	}

	/// <summary>
	/// Rebuild the visible text.
	/// </summary>

	protected void Rebuild ()
	{
		if (isValid)
		{
			// Although we could simply use UILabel.Wrap, it would mean setting the same data
			// over and over every paragraph, which is not ideal. It's faster to only do it once
			// and then do wrapping ourselves in the 'for' loop below.
			textLabel.UpdateNGUIText();
			NGUIText.rectHeight = 1000000;
			mTotalLines = 0;

			for (int i = 0; i < mParagraphs.size; ++i)
			{
				string final;
				Paragraph p = mParagraphs.buffer[i];
				NGUIText.WrapText(p.text, out final);
				p.lines = final.Split('\n');
				mTotalLines += p.lines.Length;
			}

			// Recalculate the total number of lines
			mTotalLines = 0;
			for (int i = 0, imax = mParagraphs.size; i < imax; ++i)
				mTotalLines += mParagraphs.buffer[i].lines.Length;

			// Update the bar's size
			if (scrollBar != null)
			{
				UIScrollBar sb = scrollBar as UIScrollBar;
				if (sb != null) sb.barSize = (mTotalLines == 0) ? 1f : 1f - (float)scrollHeight / mTotalLines;
			}

			// Update the visible text
			UpdateVisibleText();
		}
	}

	private string firstColor = null;
	//解决聊天换行颜色不对的bug，取色方法1
	private string lastColor(string str)
	{
		Stack<string> tempcolor = new Stack<string>();
		for (int i = 0; i < str.Length; i++)
		{
			if (str[i] == '[' )
			{
				if (i+1 < str.Length)
				{
					if (str[i+1] != '-' && str[i+1] != '/' &&
						str[i+1] != 'u' && (i+8) <= str.Length &&
					    str.Substring(i,8) != Colors.CHAT_VIP) {
						tempcolor.Push(str.Substring(i,8));
					}
				}
			}
		}
		return  tempcolor.Count > 0 ? tempcolor.Pop():null;
	}
	//解决聊天换行颜色不对的bug，取色方法2
	private string getLastColor (string str)
	{
		for (int i = str.Length - 1; i >= 0; i--) {
			if (i - 8 >= 0) {
				if (str[i] == ']' && str[i - 7] == '[' && str.Substring(i - 7,8) != Colors.CHAT_VIP) {
					return str.Substring(i - 7,8);
				}
			}
		}
		return null;
	}
	//判断广播时是否一行就结束颜色标识
	private bool isOverByOneLineRadio (string str)
	{
		for (int i = str.Length - 1; i >= 0; i--) {
			if (i - 9 >= 0) {
				if (str[i] == ']' && str[i - 8] == '[' && str.Substring(i - 8,9) == "[/url][-]") {
					return true;
				}
			}
		}
		return false;
	}

	/// <summary>
	/// Refill the text label based on what's currently visible.
	/// </summary>
	protected void UpdateVisibleText ()
	{
		if (isValid)
		{
			if (mTotalLines == 0)
			{
				textLabel.text = "";
				return;
			}

			int maxLines = Mathf.FloorToInt((float)textLabel.height / lineHeight);
			int sh = Mathf.Max(0, mTotalLines - maxLines);
			int offset = Mathf.RoundToInt(mScroll * sh);
			if (offset < 0) offset = 0;

			StringBuilder final = new StringBuilder();
			for (int i = 0, imax = mParagraphs.size; maxLines > 0 && i < imax; ++i)
			{
				Paragraph p = mParagraphs.buffer[i];
				for (int b = 0, bmax = p.lines.Length; maxLines > 0 && b < bmax; ++b)
				{
					string s = p.lines[b];

//					if (s.StartsWith("[") && !s.StartsWith("[666666]"))
//					{
//						if (s.StartsWith(Colors.CHAT_RADIO) && isOverByOneLineRadio (s)) {
//							firstColor = Colors.CHAT_RADIO;
//						} else {
//							firstColor = lastColor(s);
//							s = s + "[-]";
//						}
//					} 
//					if ( !s.StartsWith("[") && firstColor != null)
//					{
//						s = firstColor + s;
//					}
				
					if (offset > 0)
					{
						--offset;
					}
					else
					{
						if (final.Length > 0) final.Append("\n");
						final.Append(s);
						--maxLines;
					}
				}
			}
			textLabel.text = final.ToString();
			firstColor = null;
		}
	}
}
