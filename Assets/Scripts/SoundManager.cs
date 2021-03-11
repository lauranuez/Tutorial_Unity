using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
     public AudioSource efxSource;                    
     public AudioSource musicSource;                   
     public static SoundManager instance = null;                        
     public float lowPitchRange = .95f;  //pitch=tono Representa 5 por ciento menos del tono normal           
     public float highPitchRange = 1.05f;            

    
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy (gameObject);

     DontDestroyOnLoad (gameObject);
    }

    public void PlaySingle(AudioClip clip)
    {
        efxSource.clip = clip;

        efxSource.Play ();
    }

    //params = para analizar en una lista separada por comas de argumentos del mismo tipo, según lo especificado por el parámetro
    public void RandomizeSfx (params AudioClip[] clips) //Toma como parametro una lista de canciones
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
        efxSource.pitch = randomPitch;
        efxSource.clip = clips[randomIndex];
        efxSource.Play();
    }

}
