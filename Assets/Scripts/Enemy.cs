using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Vector3 _startingPosition = new Vector3(0, 8, 0);
    private Vector3 _enemyDirection;
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

    // Enemy Move Types: 0 - Standard | 1 - Diagonal | 2 - Zig Zag | 3 -
    [SerializeField]
    private int _enemyMoveType = 0;


    // Start is called before the first frame update
    void Start() 
    {
        DoNullChecks();

        //assign starting position immediately to ensure enemy spawns off screen
        transform.position = _startingPosition;

        float randomX = Random.Range(-8.5f, 8.5f);

        transform.position = _startingPosition + new Vector3(randomX, 0, 0);

        //Randomly assign enemy move type
        _enemyMoveType = Random.Range(0, 3);

        switch (_enemyMoveType)
        {
            case 0:  //MoveStandard
                _enemyDirection = new Vector3(0, -1f, 0);
                break;
            case 1:  //Diagonal
                _enemyDirection = new Vector3(1, -0.25f, 0);
                break;
            case 2: //Zig Zag
                _enemyDirection = new Vector3(-1, -0.25f, 0);
                break;
            default:
                _enemyDirection = new Vector3(0, -1f, 0);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Move based on _enemyMoveType
        switch (_enemyMoveType)
        {
            case 0:
                MoveStandard();
                break;
            case 1:
                MoveDiagonallyDown();
                break;
            case 2:
                MoveZigZag();
                break;
            default:
                MoveStandard();
                break;
        }

        RespawnAtTop();
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

    private void RespawnAtTop()
    {
        //generate random X value every frame
        float randomX = (float)Random.Range(-8.5f, 8.5f);

        //if out of lower bounds
        if (transform.position.y < -6f)
        {
            //respawn at top with a new random x position at top of screen
            transform.position = _startingPosition + new Vector3(randomX, 0, 0);
        }
    }

    private void MoveStandard()
    {
        transform.Translate(_enemyDirection * _enemySpeed * Time.deltaTime);
    }

    private void MoveDiagonallyDown()
    {
        //if out of horizontal bounds
        if (transform.position.x < -12f)
        {
            transform.position = new Vector3(11.9f, transform.position.y, transform.position.z);
        }
        else if (transform.position.x > 12f)
        {
            transform.position = new Vector3(-11.9f, transform.position.y, transform.position.z);
        }

        transform.Translate(_enemyDirection * _enemySpeed * Time.deltaTime);
    }

    private void MoveZigZag()
    {
        //if out of horizontal bounds
        if (transform.position.x < -9.4f)
        {
            _enemyDirection = new Vector3(1, -0.25f, 0);
        } else if (transform.position.x > 9.4f)
        {
            _enemyDirection = new Vector3(-1, -0.25f, 0);
        }

        transform.Translate(_enemyDirection * _enemySpeed * Time.deltaTime);
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

    private void DoNullChecks()
    {
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

        _playerGO = GameObject.FindGameObjectWithTag("Player");

        if (_playerGO != null)
        {
            _playerScript = _playerGO.GetComponent<Player>();
        }
        else
        {
            Debug.Log("Enemy script did not find Player game object!");
        }
    }
}
