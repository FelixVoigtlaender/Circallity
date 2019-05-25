using UnityEngine;
using System.Collections;

public class Rotation : MonoBehaviour {
    [SerializeField]
    private float speed;

    Rigidbody2D myRigidbody;
    public void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myRigidbody.angularVelocity = speed;
    }

    public float GetSpeed()
    {
        return speed;
    }
    public void SetSpeed(float speed)
    {
        if (!myRigidbody)
        {
            myRigidbody = GetComponent<Rigidbody2D>();
        }
        myRigidbody.angularVelocity = speed;
        this.speed = speed;
    }

    void OnDrawGizmos()
    {

        Gizmos.color = speed <= 0 ? Color.red : Color.green;
        float n = speed <= 0 ? 1 : -1;
        n = n * 0.1f;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 1 + Vector3.right*n);
    }
}
