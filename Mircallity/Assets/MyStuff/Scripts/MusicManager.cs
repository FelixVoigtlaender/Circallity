using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {
    public AudioClip[] clips;
    [Range(0f,0.5f)]
    public float rollOff;
    public float pitch;
    public float pitchRange;
    public bool repeat;
    public int startTrack;
    public float pitchTime;

    public float currentPitch;
    public float newPitch;
    float pitchVelocity;
    bool repeated;

    float pitchOffset;
    AudioSource source;
    public int i;
    void Start()
    {
        source = GetComponent<AudioSource>();
        float i = source.pitch;
        pitchOffset = pitch - 1;
        StartClip(Random.Range(0,clips.Length));    //VARIATION!
    }

    void Update()
    {
        currentPitch = source.pitch;
        newPitch = 1 + (Time.timeScale-1)*0.1f + pitchOffset;
        source.pitch = Mathf.SmoothDamp(source.pitch, newPitch, ref pitchVelocity, pitchTime);

        float length = source.clip.samples;
        float position = source.timeSamples;
        float percentage = position / length;
        float volume = 1;
        if (percentage < rollOff)
        {
            if (repeat && !repeated || !repeat)
            {
                volume = Mathf.Pow((percentage / rollOff), 2);
            }
        }
        if ((1 - percentage) < rollOff)
        {
            if (repeat && repeated || !repeat)
            {
                volume = Mathf.Pow((1 - percentage) / rollOff, 2);
            }
        }

        source.volume = volume;
        if (!source.isPlaying)
        {
            if (repeat && !repeated)
            {
                source.Play();
                repeated = true;
            }
            else
            {
                StartRandomClip();
            }
        }
    }

    void StartNextClip()
    {
        i++;
        if (i >= clips.Length)
        {
            i = 0;
        }
        StartClip(i);
    }
    void StartRandomClip()
    {
        StartClip((int)(Random.Range(0f, 1f) * clips.Length));
    }
    void StartClip(int n)
    {
        if (clips.Length <= 0 || clips.Length <= n)
        {
            return;
        }
        source.Stop();

        source.clip = clips[n];
        i = n;
        source.pitch = pitch + (Random.Range(-1f, 1f) * pitchRange);
        source.Play();
        repeated = false;

        //Hints
        HintManager.SetText(clips[n].name.ToUpper());
    }
}
   