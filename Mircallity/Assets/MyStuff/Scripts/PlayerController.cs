using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TrailRenderer))]
public class PlayerController : MonoBehaviour {
    [Header("Movement")]
    public float speed;
    public LayerMask hitLayer;

    [Header("Life")]
    public float deathTime;
    public float deathTorque;
    public float deathSpeed;
    bool isDead = false;
    Rigidbody2D rigidbody;

    [Header("Gameplay")]
    public PlayerScore score;

    [Header("Looks")]
    public float lineLength;

    [Header("Sound")]
    public float pitch;
    public float pitchRange;
    public bool onJump;
    public AudioClip[] jumpSounds;
    public AudioClip respawnSound;
    public string startBallTag, endBallTag, killBallTag;

    [Header("Screen Shake")]
    public float shakeLand, shakeDie;
    public float pushLand;

    [Header("Particles")]
    public GameObject particlesJump;
    public GameObject particlesDeath;
    public GameObject particlesKill;

    [HideInInspector]
    public KeyCode keyCode;

    CameraController cameraController;
    Transform lastBall;
    // Use this for initialization
    MeshRenderer mesh;
    TrailRenderer trail;
    LineRenderer line;
    AudioSource audioSource;
    [HideInInspector]
    public GameLogic gameLogic;
	void Start () {
        trail = GetComponent<TrailRenderer>();
        mesh = GetComponent<MeshRenderer>();
        line = GetComponent<LineRenderer>();
        audioSource = GetComponent<AudioSource>();
        rigidbody = GetComponent<Rigidbody2D>();
        cameraController = Camera.main.gameObject.GetComponent<CameraController>();
        rigidbody.isKinematic = true;

        if (audioSource)
        {
            float pitchOffset = pitch - 1;
            audioSource.pitch = Time.timeScale+ pitchOffset + Random.Range(-1f, 1f) * pitchRange;
            audioSource.clip =respawnSound;
            audioSource.Play();
        }
    }
	
	// Update is called once per frame
	void Update () {
        //PlayerDeath
        if (isDead)
        {
            return;
        }
        bool isTap = false;
        foreach(Touch touch in Input.touches)
        {
            if(touch.phase == TouchPhase.Began)
            {
                isTap = true;
                break;
            }
        }
        bool jump = Input.GetKeyDown(KeyCode.Space) || isTap;

        Vector3 dir = Vector3.Normalize(transform.TransformVector(Vector3.up));

        //Moving Player
        if (!transform.parent)
        {
            Vector3 velocity = dir * speed * Time.deltaTime;
            Vector3 left = transform.position + transform.TransformDirection(Vector3.left) * transform.lossyScale.x / 2 + transform.TransformDirection(Vector3.up) * transform.lossyScale.y / 2;
            for (int i = 0; i < 2; i++)
            {
                Vector3 position = left + transform.TransformDirection(Vector3.right) * transform.lossyScale.x*i ;
                RaycastHit2D hit = Physics2D.Raycast(position, dir, velocity.magnitude, hitLayer);
                //New ball found
                if (hit && hit.transform != lastBall)
                {
                    lastBall = hit.transform;
                    if (hit.transform.tag == killBallTag || dir.y < 0)
                    {
                        Kill();
                        //ScreenShake
                        cameraController.Shake(shakeDie);
                    }
                    else
                    {
                        PutOnBall(hit.transform.gameObject, Vector3.Normalize((Vector2)(transform.position + dir*hit.distance) - (Vector2)hit.transform.position));
                        //ScreenShake
                        cameraController.Shake(shakeLand);
                        cameraController.Push(dir.normalized * pushLand);
                    }
                    velocity = Vector3.zero;

                    break;
                }
            }
            transform.Translate(Vector3.up * velocity.magnitude);
        }

        //Stuck to ball
        if (transform.parent)
        {
            transform.rotation = Quaternion.FromToRotation(Vector3.up, transform.position - transform.parent.position);
            line.SetPosition(1, transform.position - dir * transform.lossyScale.y / 2 + Vector3.back*5);
            line.SetPosition(0, transform.position - dir * (transform.lossyScale.y / 2+lineLength) + Vector3.back * 5);
            //Jump
            if (Input.GetKeyDown(keyCode) || jump)
            {
                Jump();
                Vibration.Vibrate(60);
            }
        }
        line.enabled = transform.parent;
    }

    public void Kill() {
        if (isDead)
        {
            return;
        }
        transform.parent = null;
        isDead = true;

        rigidbody.angularVelocity = 0;
        rigidbody.isKinematic = false;
        rigidbody.simulated = true;
        rigidbody.velocity = Quaternion.Euler(0, 0, Random.Range(-1f,1f)*90) *  transform.TransformDirection(Vector3.up) * deathSpeed ;
        rigidbody.AddTorque(deathTorque);

        if (particlesKill)
        {
            GameObject particleSystem = (GameObject)Instantiate(particlesKill, transform.position, transform.rotation);
            particleSystem.GetComponent<ParticleSystem>().startColor = GetComponent<MeshRenderer>().material.color;
        }
        gameLogic.Loose();
        Invoke("DestroyMe", deathTime);
    }

    public void DestroyMe()
    {
        if (particlesDeath)
        {
            GameObject particles = (GameObject)Instantiate(particlesDeath, transform.position, transform.rotation);
            ParticleSystem particlesSystem = particles.GetComponent<ParticleSystem>();
            particlesSystem.startColor = GetComponent<MeshRenderer>().material.color;
        }
        Destroy(gameObject);
    }
    public void Spawn(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
        Spawn();
    }
    public void Spawn()
    {
        if (audioSource)
        {
            float pitchOffset = pitch - 1;
            audioSource.pitch = Time.timeScale + pitchOffset + Random.Range(-1f, 1f) * pitchRange;
            audioSource.clip = respawnSound;
            audioSource.Play();
        }

        lastBall = null;
        transform.parent = null;
        isDead = false;
        trail.enabled = true;
        mesh.enabled = true;
        rigidbody.simulated = false;
        rigidbody.isKinematic = true;
        //rigidbody.rotation = 0;
        trail.Clear();
    }

    public void Jump()
    {
        transform.parent = null;

        if (audioSource && onJump)
        {
            float pitchOffset = pitch - 1;
            audioSource.pitch = Time.timeScale + pitchOffset + Random.Range(-1f, 1f) * pitchRange;
            audioSource.clip = jumpSounds[(int)Random.Range(0f, 1f) * jumpSounds.Length];
            audioSource.Play();
        }
        if (particlesJump)
        {
            GameObject particleSystem = (GameObject) Instantiate(particlesJump, transform.position, transform.rotation);
            particleSystem.GetComponent<ParticleSystem>().startColor = GetComponent<MeshRenderer>().material.color;
        }
    }
    public void PutOnBall(GameObject ball, Vector3 dir)
    {
        gameLogic.DeltaScore(1);    //DEBUG
        gameLogic.SetLastBall(ball);
        transform.parent = ball.transform;
        dir = Vector3.Normalize((Vector2) dir);
        lastBall = ball.transform;
        CircleCollider2D collider = ball.GetComponent<CircleCollider2D>();
        transform.position = transform.parent.transform.position + (transform.lossyScale.y / 2 + transform.parent.transform.lossyScale.x/2) * dir;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, transform.position - ball.transform.position);
        transform.SetParent(ball.transform);

        BallController bc = ball.GetComponent<BallController>();
        if (bc)
        {
            bc.Stick(gameObject);
        }

        if (audioSource && !onJump)
        {
            float pitchOffset = pitch - 1;
            audioSource.pitch = Time.timeScale + pitchOffset + Random.Range(-1f, 1f) * pitchRange;
            audioSource.clip = jumpSounds[(int)Random.Range(0f, 1f) * jumpSounds.Length];
            audioSource.Play();
        }
        if (particlesJump)
        {
            GameObject particleSystem = (GameObject)Instantiate(particlesJump, transform.position + (transform.localScale.y+0.01f )* dir, transform.rotation);
            particleSystem.GetComponent<ParticleSystem>().startColor = GetComponent<MeshRenderer>().material.color;
        }
    }

    void PutTrailBack(float time)
    {
        trail.time = time;
    }

    public void SetColor(Color color)
    {
        GetComponent<MeshRenderer>().material.color = color;
        GetComponent<TrailRenderer>().material.color = color;
        GetComponent<LineRenderer>().material.color = color;
    }
}
