using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    private GameObject _playerGO;
    private Player _playerScript;
    [SerializeField]
    private int _scoreValue = 500;
    [SerializeField]
    private AudioClip _explosionAudioClip;
    private AudioSource _mineAudioSource;
    [SerializeField]
    private GameObject _explosionPrefabGO;
    private int _enemyStrength = 0;
    private bool _isDying = false;

    // Start is called before the first frame update
    void Start()
    {
        _playerGO = GameObject.Find("Player");

        if (_playerGO == null)
        {
            Debug.Log("Mine::Start()- _playerGo is null!");
        } else
        {
            _playerScript = _playerGO.GetComponent<Player>();
        }

        _mineAudioSource = transform.GetComponent<AudioSource>();

        if (_mineAudioSource == null)
        {
            Debug.Log("Mine::Start()- _mineAudioSource is null!");
        }

        if (transform.gameObject.tag == "Mine")
        {
            //Exploding mine
            _enemyStrength += 2;
        } else
        {
            //Freeze mine
            _enemyStrength += 1;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public int GetEnemyStrength()
    {
        return _enemyStrength;
    }

    public bool EnemyHasDied()
    {
        return _isDying;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //if hits laser or missile
        if (other.tag == "Laser" || other.tag == "Missile")
        {
            _isDying = true;

            if (_playerGO != null)
            {
                _playerGO.GetComponent<Player>().UpdatePlayerScore(_scoreValue);
            }

            //destroy laser or missile game object
            Destroy(other.gameObject);

            if (other.tag == "Missile")
            {
                _playerScript.EnableHomingMissileFiring();
            }

            Instantiate(_explosionPrefabGO, transform.position, Quaternion.identity);

            this.GetComponent<CircleCollider2D>().enabled = false;
            this.GetComponent<SpriteRenderer>().sprite = null;
            _mineAudioSource.clip = _explosionAudioClip;
            _mineAudioSource.Play();

            Destroy(gameObject, 2.5f);
        }
    }
}
