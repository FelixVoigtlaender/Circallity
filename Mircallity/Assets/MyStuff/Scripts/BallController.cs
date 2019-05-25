using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class BallController : MonoBehaviour {
    //Level
    [Header("Level")]
    public int level;
    public Vector2 size, offSet;
    public int points = 1;
    [HideInInspector]
    public GameObject[] levelEntities;
    FadeInOut[] entityFaders;
    FadeInOut myFader;


    [Header("Ball")]
    public bool isSpawnBall;

    [Header("Abilites")]
    public bool isAttackable;
    public bool fadeOnTouch, fadeOnRelease;
    public bool isLightning;
    public bool isColorChanger;
    public bool isColorReset;
    public bool isActivator;
    public bool isImpulse;

    [Header("Prefabs")]
    public GameObject impulsePrefab;

    [Header("Ability - Variables")]
    public float colorTime;
    public Color backgroundColor;
    bool activatorFlag;
    public float freezeScale, freezeTime;
    public GameObject[] activateEntities;

    [Header("Sound")]
    public bool playOnStick;
    public bool playOnUnStick;
    AudioSource source;

    //Private Variables
    float goalTimeScale = 1;
    float lastSpawnTime;
    int lastSpawnN;

    //Methods
	void Start () {
        myFader = GetComponent<FadeInOut>();
        source = GetComponent<AudioSource>();
        foreach(GameObject go in activateEntities)      //Deactivating Stuff :P
        {
            FadeInOut entity = go.GetComponent<FadeInOut>();
            if (entity)
            {
                entity.SetIsFadeIn(false);
            }
        }
    }

    public void Stick(GameObject player)
    {
        if (isSpawnBall)
        {
            points = 0;
        }
        if (playOnStick)
        {
            float pitchOffset = UnityEngine.Random.Range(-1f, 1f) * 0.4f;
            source.pitch = 1 + (Time.timeScale - 1) * 0.1f + pitchOffset;
            source.Play();
        }

        //Abilities
        if (isAttackable)   //Kick off players
        {
            Ab_Attack(player);
        }
        if (fadeOnTouch)    //Fade ball, and eliminate players on ball
        {
            Ab_Fade();
        }
        if (isLightning)    //Eliminate all players not on ball
        {
            Ab_Lightning();
        }
        if (isColorChanger) //Changes backgroundcolor for Timeinterval
        {
            Ab_ChangeColor();
        }
        if (isActivator)    //Activates/De-Activates Objects
        {
            Ab_Activate();
        }
        if (isColorReset)
        {
            CameraController cameraController = Camera.main.gameObject.GetComponent<CameraController>();
            if (cameraController)
            {
                //cameraController.ResetColor();
            }
        }
        if (isImpulse)
        {
            Ab_Impulse();
        }
    }
    public void UnStick(GameObject player)
    {
        if (fadeOnRelease)
        {
            Ab_Fade();
        }
        if (playOnUnStick)
        {
            source.pitch = Time.timeScale + UnityEngine.Random.Range(-1f, 1f) * 0.4f;
            source.Play();
        }
    }

    public void SearchAllLevelEntities()
    {
        List<FadeInOut> entityFaders = new List<FadeInOut>();
        List<GameObject> entities = new List<GameObject>();
        RaycastHit2D[] hits = Physics2D.BoxCastAll((Vector2)transform.position + offSet + Vector2.right*size.x/2, size, 0, Vector2.zero);
        foreach(RaycastHit2D hit in hits)
        {
            if (hit.transform.gameObject.tag != "ManagerBall")
            {
                entities.Add(hit.transform.gameObject);
                entityFaders.Add(hit.transform.GetComponent<FadeInOut>());
            }
        }
        this.levelEntities = entities.ToArray();
        this.entityFaders = entityFaders.ToArray();
    }

    public void ActivateLevel(bool flag, bool isHard=false) {
        foreach(FadeInOut entity in entityFaders)
        {
            if (isHard)
            {
                entity.HardASet(flag ? 1 : 0, flag);
            }
            else
            {
                entity.SetIsFadeIn(flag);
            }
        }
        if (isHard)
        {
            myFader.HardASet(flag ? 1 : 0, flag);
        }
        else
        {
            if (myFader)
            {
                myFader.SetIsFadeIn(flag);
            }
            else
            {
                myFader = GetComponent<FadeInOut>();
                myFader.SetIsFadeIn(flag);
            }
        }
    }

    public void ActivateBall(bool flag, bool isHard = false)
    {
        if (isHard)
        {
            myFader.HardASet(flag ? 1 : 0, flag);
        }
        else
        {
            myFader.SetIsFadeIn(flag);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 position =(Vector2)transform.position + offSet;
        Vector3 bottomLeft = (Vector2)position - size/2 +Vector2.right * size.x / 2;

        Gizmos.DrawLine(bottomLeft, bottomLeft + Vector3.right * size.x);   //Bottom
        Gizmos.DrawLine(bottomLeft + Vector3.right * size.x,(Vector2) bottomLeft + size);   //Right
        Gizmos.DrawLine((Vector2) bottomLeft + size, bottomLeft + Vector3.up * size.y);
        Gizmos.DrawLine(bottomLeft + Vector3.up * size.y, bottomLeft);
    }
    
    //
    //Abilities
    //

    void Ab_Attack(GameObject player)       //Kicking all players from ball, except for cause!
    {
        foreach (Transform childTrans in transform)
        {
            if (childTrans != player.transform)
            {
                PlayerController playerC = childTrans.GetComponent<PlayerController>();
                if (playerC)
                {
                    playerC.Jump();
                }
            }
        }
    }
    void Ab_Fade()      //Fading ball, and killing all players on ball after fade
    {
        myFader.SetIsFadeIn(false, false);
    }
    void Ab_Lightning() //Killing all players not on ball
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            bool canKill = true;
            foreach (Transform childTrans in transform)
            {
                if (childTrans == player.transform)
                {
                    canKill = false;
                    break;
                }
            }
            if (canKill)
            {
                PlayerController pc = player.GetComponent<PlayerController>();
                if (pc)
                {
                    pc.Kill();
                }
            }
        }
    }
    void Ab_ChangeColor()
    {
        CameraController cameraController = Camera.main.gameObject.GetComponent<CameraController>();
        if (cameraController)
        {
            cameraController.ChangeColor(backgroundColor, colorTime);
        }
    }
    void Ab_Activate()
    {
        activatorFlag = !activatorFlag;
        foreach (GameObject go in activateEntities)
        {
            FadeInOut entity = go.GetComponent<FadeInOut>();
            if (entity)
            {
                entity.SetIsFadeIn(activatorFlag);
            }
        }
    }
    void Ab_Impulse()
    {
        if (!impulsePrefab)
        {
            return;
        }

        GameObject myImpulse = Instantiate(impulsePrefab, transform.position,Quaternion.Euler(Vector3.zero), null);
        myImpulse.GetComponent<ImpulseEffekt>().startSize = transform.lossyScale.x;
    }
}
