using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerScore : MonoBehaviour {
    int score;
    Vector3 smoothVelocity;
    public float smoothTime;
   // [HideInInspector]
    public Vector3 newPosition;
    Text text;
    [HideInInspector]
    public RectTransform rectTransform;
    RectTransform parentRectTransform;
    Canvas canvas;
    float aVelocity;
    void Start()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
        parentRectTransform = transform.parent.GetComponent<RectTransform>();
        text = GetComponent<Text>();
        canvas = GetComponentInParent<Canvas>();
        text.text = "" + score;

        Color color = text.color;
        color.a = 0;
        SetColor(color);
        //Horrible Hacks
        rectTransform.position = new Vector3(0.5f * parentRectTransform.rect.width * canvas.scaleFactor, newPosition.y * parentRectTransform.rect.height * canvas.scaleFactor, 0);
    }
    public int GetScore()
    {
        return score;
    }
    public void AddPoint(int deltaScore)
    {
        score += deltaScore;
        SetScore(score);
    }
    public void SetScore(int score)
    {
        this.score = score;
        if (text)
        {
            text.text = "" + score;
        }
    }
    void Update()
    {
        Vector3 position = new Vector3(newPosition.x * parentRectTransform.rect.width * canvas.scaleFactor, newPosition.y * parentRectTransform.rect.height * canvas.scaleFactor, 0);
        rectTransform.position = Vector3.SmoothDamp(rectTransform.position, position, ref smoothVelocity, smoothTime);

        Color color = text.color;
        color.a = Mathf.SmoothDamp(color.a, 1, ref aVelocity, smoothTime);
        SetColor(color);
    }

    public void SetColor(Color color)
    {
        if (text)
        {
            text.color = color;
        }
        else
        {
            GetComponent<Text>().color = color;
        }
    }
}
