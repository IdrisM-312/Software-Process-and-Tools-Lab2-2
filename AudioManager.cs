using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioSource audioSrc;

    public static AudioClip pickGem;
    public static AudioClip pickCherry;
    public static AudioClip boom;
    public static AudioClip button;

    // Start is called before the first frame update
    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
        pickGem = Resources.Load<AudioClip>("PickGem");
        pickCherry = Resources.Load<AudioClip>("PickCherry");
        boom = Resources.Load<AudioClip>("Boom");
        button = Resources.Load<AudioClip>("Button");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void PlayPickGem()
    {
        audioSrc.PlayOneShot(pickGem);
    }

    public static void PlayPickCherry()
    {
        audioSrc.PlayOneShot(pickCherry);
    }

    public static void PlayBoom()
    {
        audioSrc.PlayOneShot(boom);
    }
}
