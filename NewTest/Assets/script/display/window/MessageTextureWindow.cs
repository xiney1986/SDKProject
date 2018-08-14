using UnityEngine;
using System.Collections;

public class MessageTextureWindow : WindowBase
{
    public UITexture texture;
    public UILabel label;
    public UISprite background;
    private float life = 1.5f;
    private float fromY = -300.0f;
    private float toY = 0.0f;

    public void init(string path, string msg)
    {
        label.text = msg;
        ResourcesManager.Instance.LoadAssetBundleTexture(path, texture);
        if (texture == null)
            end();
        background.height = texture.height + 10;
    }

    public void init(string path, string msg, float fY, float ty)
    {
        fromY = fY;
        toY = ty;
        label.text = msg;
        ResourcesManager.Instance.LoadAssetBundleTexture(path, texture);
        if (texture == null)
            end();
        background.height = texture.height + 10;
    }

    protected override void begin()
    {
        base.begin();
        TweenPosition tp = TweenPosition.Begin(gameObject, 0.5f, new Vector3(0, toY, 0));
        tp.from = new Vector3(0, fromY, 0);
        iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "onupdate", "changeAlpha", "oncomplete", "", "easetype", iTween.EaseType.linear, "time", 0.05f));
        iTween.ValueTo(gameObject, iTween.Hash("from", new Vector3(0, -600, 0), "to", new Vector3(0, 0, 0), "onupdate", "changePos", "oncomplete", "", "", iTween.EaseType.easeInOutExpo, "time", 0.1f));
        iTween.ValueTo(gameObject, iTween.Hash("delay", 1.5f, "from", 1, "to", 0, "onupdate", "changeAlpha", "easetype", iTween.EaseType.linear, "time", 0.1f));
        iTween.ValueTo(gameObject, iTween.Hash("from", 1, "to", 0, "onupdate", "", "oncomplete", "readNext", "easetype", iTween.EaseType.linear, "time", 0.1f));	
		MaskWindow.UnlockUI ();
    }

    void Update()
    {
        life -= Time.deltaTime;
        if (life <= 0)
            end();
    }

    private void end()
    {
		finishWindow();
    }
}
