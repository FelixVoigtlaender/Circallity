using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpulseEffekt : MonoBehaviour {
    public float startSize;
    public float timeInterval;
    public float deltaSize;
    public float startA;

    float startTime;
    Renderer myRenderer;
    public void Start()
    {
        startTime = Time.time;
        myRenderer = GetComponent<Renderer>();

        transform.Translate(Vector3.forward * 100);     //Pushing the "Effekt" behind the Ball (Or what ever) :P

        Color currentColor = myRenderer.material.color;
        currentColor.a = 0;
        myRenderer.material.color = currentColor;
    }

    void Update()
    {
        if(Time.time>startTime + timeInterval)
        {
            Destroy(gameObject);
            return;
        }

        float percentage = (Time.time - startTime)/timeInterval;
        percentage = Mathf.Clamp01(percentage);
        


        //Size
        float currentSize = startSize + deltaSize * percentage;
        transform.localScale = Vector3.one * currentSize;
        //Color
        float currentA = startA * (1 - percentage);
        Color currentColor = myRenderer.material.color;
        currentColor.a = currentA;
        myRenderer.material.color = currentColor;
    }


}
