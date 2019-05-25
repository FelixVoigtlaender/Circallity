using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FadeInOut : MonoBehaviour {
    public float fadeInTime;
    public float fadeOutTime;
    float fadeVelocity;
    public float tolerance;

    bool isFadeIn = true;
    bool isForced = false;
    
    //Private Variables
    Renderer renderer;

    void Start()
    {
        renderer = GetComponent<Renderer>();
    }
    
    void FixedUpdate()
    {
        Color color = renderer.material.color;
        if (isFadeIn)
        {
            gameObject.SetActive(true);
            if(color.a < 1-tolerance)
            {
                color.a = Mathf.SmoothDamp(color.a, 1, ref fadeVelocity, fadeInTime);
            }
            else
            {
                if(color.a == 1)
                {
                    return;
                }
                color.a = 1;
            }
        }
        else
        {
            if(color.a > tolerance)
            {
                color.a = Mathf.SmoothDamp(color.a, 0, ref fadeVelocity, fadeOutTime);
            }
            else
            {
                color.a = 0;
                RespawnPlayers();
                if (isForced)
                {
                    gameObject.SetActive(false);    //"Deaktivate" NO Refade
                    //isForced = false;
                }
                else
                {
                    gameObject.SetActive(true);
                    isFadeIn = true;    //Refading
                }
            }
        }
        renderer.material.color = color;
    }
    void RespawnPlayers()
    {
        List<Transform> players = new List<Transform>();
        foreach (Transform player in transform)
        {
            players.Add(player);
        }
        foreach (Transform player in players)
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc)
            {
                pc.Kill();
            }
        }
    }
    public void HardASet(float a, bool isFadeIn)
    {
        if (!renderer)
        {
            renderer = GetComponent<Renderer>();
        }
        if (isFadeIn)
        {
            gameObject.SetActive(true);
        }
        this.isFadeIn = isFadeIn;
        Color color = renderer.material.color;
        color.a = a;
        renderer.material.color = color;
    }
    public void SetIsFadeIn(bool isFadeIn, bool isForced = true)
    {
        if (isFadeIn)
        {
            gameObject.SetActive(true);     //Activating gameobject 
        }
        if(!isFadeIn && isForced)
        {
            this.isForced = true;   //No Refading
        }
        else
        {
            this.isForced = false;
        }
        this.isFadeIn = isFadeIn;
    }
}
