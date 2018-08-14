using UnityEngine;
using System.Collections;

public class BoxPropAnim : MonoBehaviour
{
    public GameObject Icon;
    private int move_FX; //移动方向
    private float life = 2.5f; //存在时间
    private bool isDown = false; //是否向下
    private float waitTime = 1.0f;
    private float movex;
    private float movey;

	void Start () 
    {
        move_FX = Random.Range(0, 1);
        movex = (float)Random.Range(3, 5);
        movey = (float)Random.Range(3, 5);
        //Vector3 v;
        //if (move_FX == 0)
        //    v = new Vector3(-Random.Range(20, 40), Random.Range(70, 130), 0);
        //else
        //    v = new Vector3(Random.Range(10, 20), Random.Range(50, 70), 0);

        //TweenPosition tp = TweenPosition.Begin(this.gameObject, life, v);
        //tp.animationCurve
	}

	void Update () 
    {
       life -= Time.deltaTime;
       move();

       if (life <= 0.0f)
           Destroy(this.gameObject);
	}

    private void move()
    {
        if (waitTime >= 0.0f)
            waitTime -= Time.deltaTime;
        else
        {
            if (Icon.active == false)
                Icon.SetActive(true);
            if (this.transform.localPosition.y < 200.0f && !isDown)
            {
                //左
                if (move_FX == 0)
                {
                    this.transform.localPosition = new Vector3(this.transform.localPosition.x - movex / 10, this.transform.localPosition.y, 0.0f);
                }
                //右
                else if (move_FX == 1)
                {
                    this.transform.localPosition = new Vector3(this.transform.localPosition.x + movex / 10, this.transform.localPosition.y, 0.0f);
                }

                this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y + movey, 0.0f);
            }
            else
                isDown = true;
            if (isDown)
            {
                //左
                if (move_FX == 0)
                {
                    this.transform.localPosition = new Vector3((this.transform.localPosition.x - (movex / 10f + 0.5f)), this.transform.localPosition.y, 0.0f);
                }
                //右
                else if (move_FX == 1)
                {
                    this.transform.localPosition = new Vector3((this.transform.localPosition.x + (movex / 10f + 0.5f)), this.transform.localPosition.y, 0.0f);
                }

                this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y - movey, 0.0f);
            }
        }
    }
}
