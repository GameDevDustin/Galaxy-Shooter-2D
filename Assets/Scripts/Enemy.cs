using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Vector3 _startingPosition = new Vector3(0, 8, 0);
    private Vector3 _enemyDirection = new Vector3(0, -1, 0);
    [SerializeField]
    private float _enemySpeed = 4f;
    [SerializeField]
    private GameObject _playerGO;
    private Player _playerScript;
    [SerializeField]
    private int _scoreValue = 250;
    private Animator _enemyAnimator;
    private bool _isDying = false;
    private AudioSource _enemyAudioSource;
    [SerializeField]
    private AudioClip _enemyAudioClip;


    // Start is called before the first frame update
    void Start() {

        _enemyAnimator = this.GetComponent<Animator>();

        if (_enemyAnimator == null)
        {
            Debug.Log("Enemy::Start() _enemyAnimator is null!");
        }

        if (_enemyAudioClip == null)
        {
            Debug.Log("Enemy:: Start() - _enemyAudioClip is null!");
        }

        _enemyAudioSource = transform.GetComponent<AudioSource>();

        if (_enemyAudioSource == null)
        {
            Debug.Log("Enemy:: Start() - _enemyAudioSource is null!");
        }

        //assign starting position immediately to ensure enemy spawns off screen
        transform.position = _startingPosition;

        //X values for enemy position must be between -8.999 and 8.999
        float randomX = (float)Random.Range(-8.5f, 8.5f);

        //Reset spawn position at random x position above top of screen
        transform.position = _startingPosition + new Vector3(randomX, 0, 0);

        _playerGO = GameObject.FindGameObjectWithTag("Player");

        if (_playerGO != null)
        {
            _playerScript = _playerGO.GetComponent<Player>();
        } else
        {
            Debug.Log("Enemy script did not find Player game object!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //generate random X value every frame
        float randomX = (float)Random.Range(-8.5f, 8.5f);

        //move down at 4 meters per second
        transform.Translate(_enemyDirection * _enemySpeed * Time.deltaTime);

        //if out of lower bounds
        if (transform.position.y < -6f)
        {
            if (_isDying == false)
            {
                //respawn at top with a new random x position at top of screen
                transform.position = _startingPosition + new Vector3(randomX, 0, 0);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //if hits player
        if (other.tag == "Player")
        {
            //knock player back
            other.GetComponent<Transform>().Translate(new Vector3(0, -2, 0));
            
            //Stun player for 1 second
            other.GetComponent<Player>().Stunned(1f);

            //damage player
            other.GetComponent<Player>().LoseLife();

            //destroy this enemy game object
            OnDeath();
        }

  
        //if hits laser
        if (other.tag == "Laser")
        {
            _playerGO.GetComponent<Player>().UpdatePlayerScore(_scoreValue);

            //destroy  laser game object
            Destroy(other.gameObject);

            //destroy this enemy game object
            OnDeath();
        }
    }

    private void OnDeath()
    {
        _isDying = true;
        _enemyAnimator.SetTrigger("OnEnemyDeath");
        this.GetComponent<BoxCollider2D>().enabled = false;
        _enemyAudioSource.clip = _enemyAudioClip;
        _enemyAudioSource.Play();

        //destroy this enemy game object
        Destroy(this.gameObject, 2.5f);
    }

}
