using UnityEngine;
using System.Collections;

public class UVanimCtrl : MonoBehaviour
{
	public bool random = false;
	public float delay;
	//public float  MoveDistanceX;
//	public float  MoveDistanceY;
	public Vector2 grid;
	public bool loop=true;
	Vector2 _grid;
	float _delay;
	float offsetX;
	float offsetY;
 
	// Use this for initialization
	void Start ()
	{
		_delay = delay;
		_grid = new Vector2 (1, 1);
		offsetX=1/grid.x;
		offsetY=1/grid.y;

		if(grid==Vector2.one)
		{
			return;
		}

		renderer.material.mainTextureOffset = new Vector2 (_grid.x *offsetX , _grid.y *offsetY);
		renderer.material.mainTextureScale=new Vector2 (offsetX  , offsetY );
	}

	void moveUV ()
	{

		if (_grid.x < grid.x) {
			_grid.x += 1;
		} else {
			_grid.x = 1;
			if (_grid.y < grid.y) {
				_grid.y += 1;
			} else {
				if(loop){
				_grid.x=1;
				_grid.y=1;
				}else{
					return;
				}
			}
		}
		

			renderer.material.mainTextureOffset = new Vector2 ((_grid.x-1) *offsetX , (_grid.y-1) *offsetY);
		renderer.material.mainTextureScale=new Vector2 (offsetX  , offsetY );
	}

	// Update is called once per frame
	void Update ()
	{
		if(grid==Vector2.one)
		{
			return;
		}

		_delay -= Time.deltaTime;
		if (_delay <= 0) {
			_delay = delay;
			moveUV ();
		} else {

		}
	
	}
}
