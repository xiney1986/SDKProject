using UnityEngine;
using System.Collections;

public class ArenaRankWindowDrag : MonoBehaviour {

    public ArenaRankWindow window;

    void OnDrag (Vector2 delta)
    {
        window.OnDrag(delta);
    }
}
