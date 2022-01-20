using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _playerSpeed =  7.36f;
    [SerializeField]
    private float _playerBoostSpeed = 1f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private float _laserFireRate = 0.2f;
    [SerializeField]
    private float _tripleShotFireRate = 0.35f;
    private float _nextLaserFireTimeStamp = -1f;
    private float _noLogerStunnedTimeStamp = -1f;
    [SerializeField]
    private int _playerLives = 3;
    private SpawnManager _spawnManager;
    private bool _tripleShotActive = false;
    private bool _speedBoostActive = false;
    private bool _shieldActive = false;
    [SerializeField]
    private GameObject _playerShieldPrefabGO;
    private GameObject _playerShieldGO;
    [SerializeField]
    private int _playerScore;
    [SerializeField]
    private Canvas UIManagerCanvas;
    private UIManager UIManagerScript;
    [SerializeField]
    private GameManager _gameManager;
    [SerializeField]
    private GameObject _damagedFire1GO;
    [SerializeField]
    private GameObject _damagedFire2GO;
    [SerializeField]
    private AudioClip _laserAudioClip;
    private AudioSource _playerAudioSource;

    // Start is called before the first frame update
    void Start()
    {
       
        transform.position = new Vector3(0, -3, 0);

        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();

        if (_spawnManager == null)
        {
            Debug.Log("Player:: Start() - _spawnManager is null!");
        }

        _playerAudioSource = transform.GetComponent<AudioSource>();

        if (_playerAudioSource == null)
        {
            Debug.Log("Player:: Start() - _playerAudioSource is null!");
        }

        if (_laserAudioClip == null)
        {
            Debug.Log("Player:: Start() - _laserAudioClip is null!");
        }

        if (UIManagerCanvas != null)
        {
            UIManagerScript = UIManagerCanvas.GetComponent<UIManager>();
        }
        else
        {
            Debug.Log("UI Manager canvas game object not assigned!");
        }

        if (_gameManager == null)
        {
            Debug.Log("Player script Start() found _gameManager to be null!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > _noLogerStunnedTimeStamp)
        {
            CalcMovement();
        }
        //Fire laser at appropriate fire rate
        if (Input.GetButton("Fire1") && Time.time > _nextLaserFireTimeStamp)
        {
            FireLaser();
         }
    } 

    void CalcMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        //Move player based on user input
        if (transform.position.x < 9f && transform.position.x > -9f)
        { // In horizontal bounds
            if (transform.position.y < 5.5f && transform.position.y > -3.8f)  // In vertical bounds
            {
                //Move player
                transform.Translate(direction * _playerSpeed *_playerBoostSpeed * Time.deltaTime);
            }
            else  // Out of vertical bounds
            {
                if (transform.position.y > 0f) {transform.position = new Vector3(transform.position.x, 5.499f, transform.position.z);}
                else {transform.position = new Vector3(transform.position.x, -3.799f, transform.position.z);}
            }
        }
        else  // Out of horizontal bounds
        {
            //Reset to boundary
            if (transform.position.x > 0f) {transform.position = new Vector3(8.999f, transform.position.y, transform.position.z);}
            else {transform.position = new Vector3(-8.999f, transform.position.y, transform.position.z);}
        }
    }
    void FireLaser()
    {
        //Fire triple shot?
        if (_tripleShotActive == true) {
            _nextLaserFireTimeStamp = Time.time + _tripleShotFireRate;
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 0.8f, 0), Quaternion.identity);
            Instantiate(_laserPrefab, transform.position + new Vector3(-0.782f, -0.425f, 0), Quaternion.identity);
            Instantiate(_laserPrefab, transform.position + new Vector3(0.786f, -0.425f, 0), Quaternion.identity);
        } 
        else //Fire standard laser
        {
            _nextLaserFireTimeStamp = Time.time + _laserFireRate;
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 0.8f, 0), Quaternion.identity);
        }

        //Play laser audio
        _playerAudioSource.clip = _laserAudioClip;
        _playerAudioSource.Play();
    }

    public void Stunned(float duration)
    {
        //Stun the player game object for duration
        _noLogerStunnedTimeStamp = Time.time + duration;
    }

    public void TripleShotActive()
    {
        _tripleShotActive = true;
        StartCoroutine(TripleShotPowerDown());
    }

    IEnumerator TripleShotPowerDown()
    {
        yield return new WaitForSeconds(5.0f);
        _tripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        _speedBoostActive = true;
        _playerBoostSpeed = 2f;
        StartCoroutine(SpeedBoostPowerDown());
    }

    IEnumerator SpeedBoostPowerDown()
    {
        yield return new WaitForSeconds(10f);
        _speedBoostActive = false;
        _playerBoostSpeed = 1;
    }

    public void ShieldActive()
    {
        if (_shieldActive == false)  
        {
            _shieldActive = true;
            _playerShieldGO = Instantiate(_playerShieldPrefabGO, this.transform);
        }
    }

    public void LoseLife()
    {
        if (_shieldActive != true)
        {
            _playerLives -= 1;

            //Show player damage
            if (_playerLives == 2)
            {
                if(_damagedFire1GO != null )
                {
                    _damagedFire1GO.GetComponent<DamagedFire>().ActivateDamage();
                }
               
            } else if (_playerLives == 1)
            {
                if (_damagedFire2GO != null)
                {
                    _damagedFire2GO.GetComponent<DamagedFire>().ActivateDamage();
                }
                
            }

            UIManagerScript.UpdateNumOfLivesDisplay(_playerLives);
            UpdatePlayerScore(-1000);
        } else
        {
            _shieldActive = false;
            Destroy(_playerShieldGO);
        }

        //check if dead
        if (_playerLives < 1)
        {
            //Stop spawning enemies
            if (_spawnManager != null)
            {
                _spawnManager.PlayerDied();
            }
            else { Debug.Log("_spawnManager = null"); }

            if (UIManagerScript != null)
            {
                UIManagerScript.ShowGameOver();
            } else
            {
                Debug.Log("UIManagerScript not found in Player script, LoseLife() method!");
            }

            if (_gameManager != null)
            {
                _gameManager.GameOver();
            }

            Destroy(this.gameObject);
        }
    }

    public void UpdatePlayerScore(int addToPlayerScore)
    {
        _playerScore += addToPlayerScore;
        UIManagerScript.UpdatePlayerScore(_playerScore);
    }

}
