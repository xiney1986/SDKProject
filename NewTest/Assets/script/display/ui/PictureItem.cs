using UnityEngine;
using System.Collections;

public class PictureItem : MonoBehaviour {

	public CardSample card;
	public int index;

	public void init(CardSample card,int index)
	{
		this.card = card;
		this.index = index;
	}
}
