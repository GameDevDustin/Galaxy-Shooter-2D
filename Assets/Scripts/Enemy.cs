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
    [SerializeField]
    private AudioClip _laserAudioClip;
    [SerializeField]
    private GameObject _enemyLaserPrefabGO;
    [SerializeField]
    private GameObject _enemyLaserBeamPrefabGO;
    private GameObject _tempEnemyLaserGO;
    private GameObject _tempEnemyLaserBeamGO;
    [SerializeField]
    private GameObject _enemyShieldPrefabGO;
    private GameObject _tempEnemyShieldGO;
    [SerializeField]
    private bool _shieldActive = false;
    [SerializeField]
    private int _enemyStrength = 1;

    // Enemy Move Types: 0 - Standard | 1 - Diagonal | 2 - Zig Zag | 3 -
    [SerializeField]
    private int _enemyMoveType = 0;

    //Enemy Firing Types: 0 - Standard (Not firing) | 1 - Laser | 2 - Beam Laser
    [SerializeField]
    private int _enemyFiringType = 0;

    private float _randomFiringTime;
    private float _nextFiringTime = 3f;
    private float _nextBeamFiringTime = 5f;

    [SerializeField]
    private bool _playerNearby = false;
    private Vector3 _towardsPlayerDirection;
    [SerializeField]
    private float _withinPlayerTrackingDistance = 4;
    [SerializeField]
    private float _playerAttractionSpeed = 3;
    private bool _playerDied = false;
    private bool _disableEnemyFiring = false;


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

        //TESTING PURPOSES
        //Force enemyMoveType
        //_enemyMoveType = 1;

        //Randomly assign enemy firing type
        _enemyFiringType = Random.Range(0, 2);

        //Weight odds of changing to the BeamLaser enemy randomly
        int randomNumber = (int)Random.Range(0, 5);

        if (randomNumber == 4 && _enemyMoveType != 0)
        {
            _enemyFiringType = 2;
        }

        //TESTING PURPOSES
        //Force enemyFiringType
        //_enemyFiringType = 2;       

        switch (_enemyFiringType)
        {
            case 0:  //Does not fire
                break;
            case 1:  //Regular laser fire
                _enemyStrength += 1;
                break;

            case 2:  //Fire beam
                _enemyStrength += 3;
                break;
            default:  //Does not fire
                break;
        }

        switch (_enemyMoveType)
        {
            case 0:  //MoveStandard
                _enemyDirection = new Vector3(0, -1f, 0);
                break;
            case 1:  //Diagonal
                _enemyDirection = new Vector3(1, -0.25f, 0);
                _enemyStrength += 1;
                break;
            case 2: //Zig Zag
                _enemyDirection = new Vector3(-1, -0.25f, 0);
                _enemyStrength += 2;
                break;
            default:
                _enemyDirection = new Vector3(0, -1f, 0);
                break;
        }

        //Randomly give enemies shields
        randomNumber = (int)Random.Range(0, 7);
        if (randomNumber == 2)
        {
            AddShield();
            _enemyStrength += 2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(_playerDied == false && _isDying == false)
        {
            CheckPlayerDistance();
        }

        if (_playerNearby == true && _playerDied == false && _isDying == false)
        {
            _disableEnemyFiring = true;

            RamPlayer();
        }
        else
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
        }

        if(_disableEnemyFiring == false && _isDying == false)
        {
            switch (_enemyFiringType)
            {
                case 0:  //Does not fire

                    break;
                case 1:  //Regular laser fire
                    if (Time.time > _nextFiringTime)
                    {
                        FireLaser();
                    }
                    break;

                case 2:  //Fire beam
                    if (Time.time > _nextBeamFiringTime)
                    {
                        FireBeam();
                    }
                    break;
                default:  //Does not fire

                    break;
            }
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
  
        //if hits laser or missile
        if (other.tag == "Laser" || other.tag == "Missile")
        {
            if (_shieldActive != true)
            {
                if (_playerGO != null)
                {
                    _playerGO.GetComponent<Player>().UpdatePlayerScore(_scoreValue);
                }

                //destroy this enemy game object
                OnDeath();
            } else
            {
                DeactivateShield();
            }

            //destroy laser or missile game object
            Destroy(other.gameObject);

            if(other.tag == "Missile")
            {
                _playerScript.EnableHomingMissileFiring();
            }
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
            transform.rotation = Quaternion.Euler(0, 0, 0);
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

    private void FireLaser()
    {
        _randomFiringTime = Random.Range(3f, 9f);
        _nextFiringTime = Time.time + _randomFiringTime;

        _tempEnemyLaserGO = Instantiate(_enemyLaserPrefabGO, transform.position + new Vector3(0, -1.9f, 0), Quaternion.identity);
        _tempEnemyLaserGO.GetComponent<Laser>().SetSpeed(4);

        //Play laser audio
        _enemyAudioSource.clip = _laserAudioClip;
        _enemyAudioSource.Play();
    }

    private void FireBeam()
    {
        if (_disableEnemyFiring == false && _isDying == false)
        {
            _randomFiringTime = Random.Range(3f, 6f);
            _nextBeamFiringTime = Time.time + _randomFiringTime;

            _tempEnemyLaserBeamGO = Instantiate(_enemyLaserBeamPrefabGO, transform.position + new Vector3(0, -10.5f, 0), Quaternion.identity);
            _tempEnemyLaserBeamGO.transform.parent = transform;

            StartCoroutine(BeamLaserCooldown(5f));
        }
    }

    private IEnumerator BeamLaserCooldown(float delay)
    {
        yield return new WaitForSeconds(delay);
        DestroyLaserBeam();
    }

    private void DestroyLaserBeam()
    {
        if (_tempEnemyLaserBeamGO != null)
        {
            _tempEnemyLaserBeamGO.GetComponent<BoxCollider2D>().enabled = false;
            _tempEnemyLaserBeamGO.GetComponent<SpriteRenderer>().enabled = false;
            Destroy(_tempEnemyLaserBeamGO);
        }
    }

    private void AddShield()
    {
        _tempEnemyShieldGO = Instantiate(_enemyShieldPrefabGO, transform);
        _shieldActive = true;
    }

    private void DeactivateShield()
    {
        _shieldActive = false;
        Destroy(_tempEnemyShieldGO);
    }

    private void OnDeath()
    {
        _disableEnemyFiring = true;
        _isDying = true;

        if (_enemyFiringType == 2)
        {
            DestroyLaserBeam();
        }

        _enemyAnimator.SetTrigger("OnEnemyDeath");
        this.GetComponent<BoxCollider2D>().enabled = false;
        _enemyAudioSource.clip = _enemyAudioClip;
        _enemyAudioSource.Play();
        
        //destroy this enemy game object
        Destroy(this.gameObject, 2.5f);
    }

    private void RamPlayer()
    {
        //Rotate towards player
        Vector3 direction = _playerGO.transform.position - transform.position;
        
        direction.Normalize();

        float rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z + 90);

        //Move towards player   
        transform.Translate(direction * _playerAttractionSpeed * Time.deltaTime);
    }

    private void CheckPlayerDistance()
    {
        float playerY = _playerGO.transform.position.y;
        float enemyY = transform.position.y;

        float distance = Vector3.Distance(transform.position, _playerGO.transform.position);

        if(distance < _withinPlayerTrackingDistance)
        {
            if (playerY < enemyY) 
            {
                _playerNearby = true;
                if(_tempEnemyLaserBeamGO != null)
                {
                    Destroy(_tempEnemyLaserBeamGO);
                }
            } else
            {
                _playerNearby = false;
                transform.rotation = Quaternion.Euler(0, 0, 0);
                _disableEnemyFiring = false;
            }
        } else
        {
            _playerNearby = false;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            _disableEnemyFiring = false;
        }
    }

    public void PlayerDied()
    {
        _playerDied = true;
    }

    public int GetEnemyStrength()
    {
        return _enemyStrength;
    }

    public bool EnemyHasDied()
    {
        return _isDying;
    }

    private void DoNullChecks()
    {
        _enemyAnimator = this.GetComponent<Animator>();

        if (_enemyAnimator == null)
        {
            Debug.Log("Enemy:: DoNulLChecks() - _enemyAnimator is null!");
        }

        if (_enemyAudioClip == null)
        {
            Debug.Log("Enemy:: DoNulLChecks() - _enemyAudioClip is null!");
        }

        _enemyAudioSource = transform.GetComponent<AudioSource>();

        if (_enemyAudioSource == null)
        {
            Debug.Log("Enemy:: DoNulLChecks() - _enemyAudioSource is null!");
        }

        _playerGO = GameObject.FindGameObjectWithTag("Player");

        if (_playerGO != null)
        {
            _playerScript = _playerGO.GetComponent<Player>();
        }
        else
        {
            _playerDied = true;
            Debug.Log("Enemy:: DoNulLChecks() - did not find Player game object!");
        }

        if (_laserAudioClip == null)
        {
            Debug.Log("Enemy:: DoNulLChecks() - _laserAudioClip is null!");
        }

        if (_enemyLaserPrefabGO == null)
        {
            Debug.Log("Enemy:: DoNulLChecks() - _enemyLaserPrefabGO is null!");
        }

        if (_enemyShieldPrefabGO == null)
        {
            Debug.Log("Enemy:: DoNulLChecks() - _enemyShieldPrefabGO is null!");
        }
    }
}
