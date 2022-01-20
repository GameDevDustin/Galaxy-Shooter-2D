using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private AudioSource _explosionAudioSource;
    [SerializeField]
    private AudioClip _explosionAudioClip;


    // Start is called before the first frame update
    void Start()
    {
        if(_explosionAudioClip == null)
        {
            Debug.Log("Explosion:: Start() - _explosionAudioClip is null!");
        }

        _explosionAudioSource = transform.GetComponent<AudioSource>();

        if (_explosionAudioSource == null)
        {
            Debug.Log("Explosion:: Start() - _explosionAudioSource is null!");
        }

        
        _explosionAudioSource.clip = _explosionAudioClip;
        _explosionAudioSource.Play();

        Destroy(this.gameObject, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
