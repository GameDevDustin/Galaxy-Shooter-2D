using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _playerSpeed =  7.36f;
    [SerializeField]
    private float _playerBoostPowerupSpeed = 1f;
    [SerializeField]
    private float _playerBoostSpeed = 1f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private float _laserFireRate = 0.2f;
    private float _nextLaserFireTimeStamp = -1f;
    [SerializeField]
    private GameObject _laserBurstPrefab;
    [SerializeField]
    private float _laserBurstFireRate = 0.2f;
    [SerializeField]
    private bool _laserBurstActive = false;
    [SerializeField]
    private float _tripleShotFireRate = 0.35f;
    private float _noLogerStunnedTimeStamp = -1f;
    [SerializeField]
    private int _playerLives = 3;
    private SpawnManager _spawnManager;
    private bool _tripleShotActive = false;
    private bool _speedBoostPowerupActive = false;
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
    [SerializeField]
    private GameObject _shieldStrengthGO;
    [SerializeField]
    private GameObject _shieldStrengthTextGO;
    [SerializeField]
    private int _shieldPower = 0;
    [SerializeField]
    private GameObject _ammoChargeGO;
    [SerializeField]
    private int _ammoChargeCount = 15;
    [SerializeField]
    private float _ammoChargeSize = 120;
    private GameObject _currentAmmoTextGO;
    [SerializeField]
    private int _currentHomingMissiles = 0;
    [SerializeField]
    private GameObject _homingMissilePrefabGO;
    [SerializeField]
    private GameObject _homingMissile2PrefabGO;
    [SerializeField]
    private GameObject[] _homingMissilesGO = new GameObject[2];
    [SerializeField]
    private float _homingMissileSpeed = 5f;
    private bool _moveHomingMissiles = false;
    private bool _missile1Fired = false;
    private bool _missile2Fired = false;
    private GameObject _enemyMissileTarget1;
    private GameObject _enemyMissileTarget2;
    [SerializeField]
    private GameObject[] _allEnemiesGO = new GameObject[100];
    private bool _fireHomingMissileCooldownActive = false;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, -3, 0);

        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();

        _playerAudioSource = transform.GetComponent<AudioSource>();

        _currentAmmoTextGO = GameObject.Find("CurrentAmmo_text");

        DoNullChecks();

        if (UIManagerCanvas != null)
        {
            UIManagerScript = UIManagerCanvas.GetComponent<UIManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckForThrusterBoost();

        if (Time.time > _noLogerStunnedTimeStamp)
        {
            CalcMovement();
        }

        if (_currentHomingMissiles > 0)
        {
            //Get all enemy GameObjects on screen so they can be targeted 
            _allEnemiesGO = GameObject.FindGameObjectsWithTag("Enemy");
        }

        if (_moveHomingMissiles == true)
        {
            MoveHomingMissiles();
        }

        //Fire laser at appropriate fire rate
        if (Input.GetButton("Fire1") && Time.time > _nextLaserFireTimeStamp)
        {
            FireLaser();
        } 

        if (Input.GetButton("Fire2"))
        {
            if (_fireHomingMissileCooldownActive == false)
            {
                FireHomingMissile();
            }
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
                transform.Translate(direction * _playerSpeed * _playerBoostPowerupSpeed * _playerBoostSpeed * Time.deltaTime);
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

    void CheckForThrusterBoost()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _speedBoostActive = true;
            _playerBoostSpeed = 1.5f;
        } else
        {
            _speedBoostActive = false;
            _playerBoostSpeed = 1;
        }
    }

    void FireLaser()
    {
        
        if (_laserBurstActive == true) {
            //_nextLaserBurstFireTimeStamp = Time.time + _laserBurstFireRate;
            _nextLaserFireTimeStamp = Time.time + _laserFireRate;
            Instantiate(_laserBurstPrefab, transform.position + new Vector3(0, 0.8f, 0), Quaternion.identity);
            //Play laser audio
            _playerAudioSource.clip = _laserAudioClip;
            _playerAudioSource.Play();
        } 
        else if (_tripleShotActive == true)
        {
            _nextLaserFireTimeStamp = Time.time + _tripleShotFireRate;
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 0.8f, 0), Quaternion.identity);
            Instantiate(_laserPrefab, transform.position + new Vector3(-0.782f, -0.425f, 0), Quaternion.identity);
            Instantiate(_laserPrefab, transform.position + new Vector3(0.786f, -0.425f, 0), Quaternion.identity);
            //Play laser audio
            _playerAudioSource.clip = _laserAudioClip;
            _playerAudioSource.Play();
        }
        else //Fire standard laser
        {
            if(_ammoChargeCount > 0)
            {
                _nextLaserFireTimeStamp = Time.time + _laserFireRate;
                Instantiate(_laserPrefab, transform.position + new Vector3(0, 0.8f, 0), Quaternion.identity);
                _ammoChargeCount -= 1;
                _ammoChargeSize -= 8;

                _ammoChargeGO.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _ammoChargeSize);
                UpdateCurrentAmmoTextGO();

                //Play laser audio
                _playerAudioSource.clip = _laserAudioClip;
                _playerAudioSource.Play();
            }
        }

    }

    private void UpdateCurrentAmmoTextGO()
    {
        if (_ammoChargeCount > 9)
        {
            _currentAmmoTextGO.transform.GetComponent<TMP_Text>().SetText(_ammoChargeCount.ToString());
        } else if (_ammoChargeCount < 10)
        {
            _currentAmmoTextGO.transform.GetComponent<TMP_Text>().SetText("  " + _ammoChargeCount.ToString());
        }
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
        yield return new WaitForSeconds(5f);
        _tripleShotActive = false;
    }

    public void LaserBurstActive()
    {
        _laserBurstActive = true;
        StartCoroutine(LaserBurstPowerDown());
    }

    IEnumerator LaserBurstPowerDown()
    {
        yield return new WaitForSeconds(5f);
        _laserBurstActive = false;
    }

    public void SpeedBoostPowerupActive() 
    {
        _speedBoostPowerupActive = true;
        _playerBoostPowerupSpeed = 2f;
        StartCoroutine(SpeedBoostPowerupPowerDown());
    }

    IEnumerator SpeedBoostPowerupPowerDown()  
    {
        yield return new WaitForSeconds(10f);
        _speedBoostPowerupActive = false;
        _playerBoostPowerupSpeed = 1;
    }

    public void ShieldActive(int power)
    {
        if (_shieldActive == false)  
        {
            _shieldActive = true;
            _shieldPower = power;
            _shieldStrengthTextGO.GetComponent<TMP_Text>().text = _shieldPower.ToString();
            _playerShieldGO = Instantiate(_playerShieldPrefabGO, this.transform);
            _shieldStrengthGO.SetActive(true);
        }
    }

    public void AddShieldPower(int power)
    {
        if (_shieldActive == true && _shieldPower < 3)
        {
            _shieldPower += power;
            _shieldStrengthTextGO.GetComponent<TMP_Text>().text = _shieldPower.ToString();
        }
        else if (_shieldActive == false)
        {
            ShieldActive(1);
            _shieldStrengthTextGO.GetComponent<TMP_Text>().text = _shieldPower.ToString();
        }
    }

    public void ShieldDeactivate()
    {
        if (_shieldActive == true)
        {
            _shieldActive = false;
            if (_playerShieldGO != null)
            {
                Destroy(_playerShieldGO);
            }
            _shieldStrengthGO.SetActive(false);
        }
    }

    public void AddHomingMissile()
    {
        if(_currentHomingMissiles < 2)
        {
            //Add sprite to player ship
            if(_currentHomingMissiles == 0)
            {
                _homingMissilesGO[0] = Instantiate(_homingMissilePrefabGO, transform);
            } else if (_currentHomingMissiles == 1)
            {
                _homingMissilesGO[1] = Instantiate(_homingMissile2PrefabGO, transform);
            }
            
            _currentHomingMissiles += 1;
        }
    }

    private void FireHomingMissile()
    {  
        GameObject targetEnemyGO = null;
        int highestEnemyStrength = -1;
        float targetEnemyDistance = -1;
        float currEnemyDistance = -1;

        if (_fireHomingMissileCooldownActive == false)
        {
            if (_currentHomingMissiles > 0)
            {
                foreach (GameObject enemyGO in _allEnemiesGO)
                {
                    //Check current enemyGO strength compared to highest strength so far
                    if (enemyGO.transform.GetComponent<Enemy>().GetEnemyStrength() > highestEnemyStrength)
                    {
                        //make this the targetEnemy until a better one is found
                        highestEnemyStrength = enemyGO.transform.GetComponent<Enemy>().GetEnemyStrength();
                        targetEnemyGO = enemyGO;
                        targetEnemyDistance = Vector3.Distance(transform.position, targetEnemyGO.transform.position);
                    }
                    else if (enemyGO.transform.GetComponent<Enemy>().GetEnemyStrength() == highestEnemyStrength)
                    {
                        //current enemyGO is = strength to targetEnemyGO, which one is closer?
                        currEnemyDistance = Vector3.Distance(transform.position, enemyGO.transform.position);

                        if (targetEnemyDistance != -1)
                        {
                            if (currEnemyDistance < targetEnemyDistance)
                            {
                                //make this the targetEnemy until a better one is found
                                highestEnemyStrength = enemyGO.transform.GetComponent<Enemy>().GetEnemyStrength();
                                targetEnemyGO = enemyGO;
                                targetEnemyDistance = Vector3.Distance(transform.position, targetEnemyGO.transform.position);
                            }
                        }
                        else
                        {
                            targetEnemyGO = _allEnemiesGO[0];
                            Debug.Log("Player::FireHomingMissile - targetEnemyDistance = -1");
                        }
                    }
                }

                if (targetEnemyGO != null && _currentHomingMissiles > 0)
                {
                    if (_currentHomingMissiles == 2)
                    {
                        _missile2Fired = true;
                        _enemyMissileTarget2 = targetEnemyGO;
                        _homingMissilesGO[1].transform.parent = null;
                        _currentHomingMissiles -= 1;
                        StartCoroutine(FireMissileCooldown());
                    }
                    else if (_currentHomingMissiles == 1)
                    {
                        _missile1Fired = true;
                        _enemyMissileTarget1 = targetEnemyGO;
                        _homingMissilesGO[0].transform.parent = null;
                        _currentHomingMissiles -= 1;
                        StartCoroutine(FireMissileCooldown());
                    }
                    else
                    {
                        Debug.Log("Player::FireHomingMissile() - method called but _currentHomingMissiles != 1 or 2");
                    }
                    _moveHomingMissiles = true;
                }
            }
            else
            {
                Debug.Log("Player::FireHomingMissile() - _currentHomingMissiles not > 0");
            }
        }
    }

    private void MoveHomingMissiles()
    {
        float moveHorizontal;
        float moveVertical;
        bool missileOnRight;
        Vector3 towardsEnemyDirection;
        Vector3 direction;
        Quaternion targetRotation;
        float rot_z;

        if (_missile1Fired && _missile2Fired)
        {
            //Move both missiles

            //Move missile 1
            if (_enemyMissileTarget1 != null && _homingMissilesGO[0] != null)
            {
                //Determine the direction towards enemy
                if (_homingMissilesGO[0].transform.position.x < _enemyMissileTarget1.transform.position.x)
                {
                    //Move to the right
                    moveHorizontal = 1;
                    missileOnRight = true;
                }
                else
                {
                    //Move to the left
                    moveHorizontal = -1;
                    missileOnRight = false;
                }

                if (_homingMissilesGO[0].transform.position.y < _enemyMissileTarget1.transform.position.y)
                {
                    //move up
                    moveVertical = 1;
                }
                else
                {
                    //move down
                    moveVertical = -1;
                }

                towardsEnemyDirection = new Vector3(moveHorizontal, moveVertical, 0);

                //Rotate towards enemy
                direction = _enemyMissileTarget1.transform.position - _homingMissilesGO[0].transform.position;
                targetRotation = Quaternion.LookRotation(Vector3.forward, direction);

                direction.Normalize();

                rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                _homingMissilesGO[0].transform.rotation = Quaternion.Euler(0f, 0f, rot_z + 180);

                //Move towards enemy   
                _homingMissilesGO[0].transform.Translate(towardsEnemyDirection * _homingMissileSpeed * Time.deltaTime);
            }

            if(_enemyMissileTarget2 != null && _homingMissilesGO[1] != null)
            {
                //Move missile 2


                //Determine the direction towards enemy
                if (_homingMissilesGO[1].transform.position.x < _enemyMissileTarget2.transform.position.x)
                {
                    //Move to the right
                    moveHorizontal = 1;
                    missileOnRight = true;
                }
                else
                {
                    //Move to the left
                    moveHorizontal = -1;
                    missileOnRight = false;
                }

                if (_homingMissilesGO[1].transform.position.y < _enemyMissileTarget2.transform.position.y)
                {
                    //move up
                    moveVertical = 1;
                }
                else
                {
                    //move down
                    moveVertical = -1;
                }

                towardsEnemyDirection = new Vector3(moveHorizontal, moveVertical, 0);

                //Rotate towards enemy
                direction = _enemyMissileTarget2.transform.position - _homingMissilesGO[1].transform.position;
                targetRotation = Quaternion.LookRotation(Vector3.forward, direction);

                direction.Normalize();

                rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                _homingMissilesGO[1].transform.rotation = Quaternion.Euler(0f, 0f, rot_z + 180);

                //Move towards enemy   
                _homingMissilesGO[1].transform.Translate(towardsEnemyDirection * _homingMissileSpeed * Time.deltaTime);
            }
        } else if (_missile2Fired)
        {
            //Move missile2

            if (_enemyMissileTarget2 != null && _homingMissilesGO[1] != null)
            {
                //Determine the direction towards enemy
                if (_homingMissilesGO[1].transform.position.x < _enemyMissileTarget2.transform.position.x)
                {
                    //Move to the right
                    moveHorizontal = 1;
                    missileOnRight = true;
                }
                else
                {
                    //Move to the left
                    moveHorizontal = -1;
                    missileOnRight = false;
                }

                if (_homingMissilesGO[1].transform.position.y < _enemyMissileTarget2.transform.position.y)
                {
                    //move up
                    moveVertical = 1;
                }
                else
                {
                    //move down
                    moveVertical = -1;
                }

                towardsEnemyDirection = new Vector3(moveHorizontal, moveVertical, 0);

                //Rotate towards enemy
                direction = _enemyMissileTarget2.transform.position - _homingMissilesGO[1].transform.position;
                targetRotation = Quaternion.LookRotation(Vector3.forward, direction);

                direction.Normalize();

                rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                _homingMissilesGO[1].transform.rotation = Quaternion.Euler(0f, 0f, rot_z + 180);

                //Move towards enemy   
                _homingMissilesGO[1].transform.Translate(towardsEnemyDirection * _homingMissileSpeed * Time.deltaTime);
            }
        }
        else if (_missile1Fired)
        {
            //Move missile1

            if(_enemyMissileTarget1 != null && _homingMissilesGO[0] != null)
            {
                //Determine the direction towards enemy
                if (_homingMissilesGO[0].transform.position.x < _enemyMissileTarget1.transform.position.x)
                {
                    //Move to the right
                    moveHorizontal = 1;
                    missileOnRight = true;
                }
                else
                {
                    //Move to the left
                    moveHorizontal = -1;
                    missileOnRight = false;
                }

                if (_homingMissilesGO[0].transform.position.y < _enemyMissileTarget1.transform.position.y)
                {
                    //move up
                    moveVertical = 1;
                }
                else
                {
                    //move down
                    moveVertical = -1;
                }

                towardsEnemyDirection = new Vector3(moveHorizontal, moveVertical, 0);

                //Rotate towards enemy
                direction = _enemyMissileTarget1.transform.position - _homingMissilesGO[0].transform.position;
                targetRotation = Quaternion.LookRotation(Vector3.forward, direction);

                direction.Normalize();

                rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                _homingMissilesGO[0].transform.rotation = Quaternion.Euler(0f, 0f, rot_z + 180);

                //Move towards enemy  
                _homingMissilesGO[0].transform.Translate(towardsEnemyDirection * _homingMissileSpeed * Time.deltaTime);
            }
        } else
        {
            Debug.Log("Player::MoveHomingMissiles() - method called but _missile1Fired and _missile2Fired are both false");
        }
    }

    private IEnumerator FireMissileCooldown()
    {
        _fireHomingMissileCooldownActive = true;
        yield return new WaitForSeconds(1);
        //_fireHomingMissileCooldownActive = false;
    }

    public void EnableHomingMissileFiring()
    {
        _fireHomingMissileCooldownActive = false;
    }

    public void LoseLife()
    {
        if (_shieldActive == false)
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
            ShakeCamera(1f, .5f);
            UIManagerScript.UpdateNumOfLivesDisplay(_playerLives);
            UpdatePlayerScore(-1000);
        } else
        {
            if (_shieldPower > 0)
            {
                _shieldPower -= 1;
                _shieldStrengthTextGO.GetComponent<TMP_Text>().text = _shieldPower.ToString();

                if (_shieldPower == 0) {
                    ShieldDeactivate();
                }
            } else
            {
                Debug.Log("Player::LoseLife() - _shieldpower is 0 when it should have a value!");
            }
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

            //Tell all enemies player died
            GameObject[] allEnemiesGO = GameObject.FindGameObjectsWithTag("Enemy");

            foreach(GameObject enemyGO in allEnemiesGO)
            {
                enemyGO.GetComponent<Enemy>().PlayerDied();
            }

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

    public void AddLives(int lives)
    {
        if (_playerLives < 3)
        {
            if (_playerLives == 2)
            {
                _damagedFire1GO.GetComponent<DamagedFire>().DeactivateDamage();
            } 
            else if (_playerLives == 1)
            {
                _damagedFire2GO.GetComponent<DamagedFire>().DeactivateDamage();
            }
            _playerLives += lives;
            UIManagerScript.UpdateNumOfLivesDisplay(_playerLives);
        }
    }

    public void UpdatePlayerScore(int addToPlayerScore)
    {
        _playerScore += addToPlayerScore;
        UIManagerScript.UpdatePlayerScore(_playerScore);
    }

    public void RefillAmmoCharge()
    {
        _ammoChargeCount = 15;
        _ammoChargeSize = 120;
        _ammoChargeGO.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _ammoChargeSize);
        UpdateCurrentAmmoTextGO();
    }

    private void ShakeCamera(float duration, float amount)
    {
        GameObject mainCameraGO;

        mainCameraGO = GameObject.FindGameObjectWithTag("MainCamera");

        if (mainCameraGO != null)
        {
            mainCameraGO.GetComponent<CameraShake>().ShakeCamera(duration, amount);
        } else
        {
            Debug.Log("Player::ShakeCamera() - mainCameraGO is null!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "EnemyLaser")
        {
            LoseLife();
        }
    }

    private void DoNullChecks()
    {
        if (_spawnManager == null)
        {
            Debug.Log("Player::DoNullChecks - _spawnManager is null!");
        }

        if (_playerAudioSource == null)
        {
            Debug.Log("Player::DoNullChecks - _playerAudioSource is null!");
        }

        if (_laserAudioClip == null)
        {
            Debug.Log("Player::DoNullChecks - _laserAudioClip is null!");
        }

        if (UIManagerCanvas == null)
        {
            Debug.Log("Player::DoNullChecks - _UIManagerCanvas is null!");
        }

        if (_gameManager == null)
        {
            Debug.Log("Player::DoNullChecks - _gameManager is null!");
        }

        if (_shieldStrengthGO == null)
        {
            Debug.Log("Player::DoNullChecks - _shieldStrengthGO is null!");
        }

        if (_ammoChargeGO == null)
        {
            Debug.Log("Player::DoNullChecks - _ammoChargeGO is null!");
        }

        if (_currentAmmoTextGO == null)
        {
            Debug.Log("Player::DoNullChecks - _currentAmmoTextGO is null!");
        }
    }

}
