using UnityEngine;
using System.Collections;

public class ParticleManager : MonoBehaviour {
    AudioSource audioSource;
    float pitchOffset;
	// Use this for initialization
	void Start () {
        transform.Rotate(Vector3.right * -90);
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        Invoke("DestroyMe", particleSystem.duration+1);
        particleSystem.Play();

        audioSource = GetComponent<AudioSource>();
        if (audioSource)
        {
            pitchOffset = Random.Range(-1f, 1f) * 0.4f;
            audioSource.pitch = 1 + (Time.timeScale - 1) * 0.1f + pitchOffset;
        }
	}
    void Update()
    {
        if (audioSource)
        {
            audioSource.pitch = Time.timeScale + pitchOffset;
        }
    }
	
    void DestroyMe()
    {
        Destroy(gameObject);
    }
}
