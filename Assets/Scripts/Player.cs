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
    private float _homingMissileSpeed = 10f;
    private bool _moveHomingMissiles = false;
    [SerializeField]
    private bool _missile1Fired = false;
    [SerializeField]
    private bool _missile2Fired = false;
    private GameObject _enemyMissileTarget1;
    private GameObject _enemyMissileTarget2;
    [SerializeField]
    private GameObject[] _allEnemiesGO = new GameObject[100];
    private int[] _allEnemiesTypes = new int[100];              // 1 - Enemy | 2 - Mine | 3 - Boss
    private bool _fireHomingMissileCooldownActive = false;
    private bool _delayAddHomingMissile1 = false;
    private bool _delayAddHomingMissile2 = false;

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
            if (_fireHomingMissileCooldownActive == false && _currentHomingMissiles > 0)
            {
                //_allEnemiesGO = GameObject.FindGameObjectsWithTag("Enemy");
                SetAllCurrentEnemies();
                FireHomingMissile();
            }
        }

        if(_delayAddHomingMissile1 == true || _delayAddHomingMissile2 == true)
        {
            DelayAddHomingMissile();
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
        } else if (_shieldActive == true && _shieldPower < 3)
        {
            _shieldPower = 3;
            _shieldStrengthTextGO.GetComponent<TMP_Text>().text = _shieldPower.ToString();
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
            //Add first missile (right side)
            if(_currentHomingMissiles == 0)
            {
                //_missile1Fired being true means it is still alive but not on the ship
                if (_missile1Fired || _homingMissilesGO[0].gameObject != null)
                {
                    //delay adding the missile
                    _delayAddHomingMissile1 = true;
                    _currentHomingMissiles += 1;
                }
                else if (_missile1Fired == false && _homingMissilesGO[0].gameObject == null)
                {
                    //add missile 1 to right side of player ship
                    _homingMissilesGO[0] = Instantiate(_homingMissilePrefabGO, transform);
                    _currentHomingMissiles += 1;
                    _delayAddHomingMissile1 = false;
                }
            }
            else if (_currentHomingMissiles == 1)
            {
                //Is the count 1 because of a delay?
                if (_delayAddHomingMissile1 == true)
                {
                    //Has the original missile 1 that delayed this missile 1 died?
                    if(_missile1Fired == false && _homingMissilesGO[0].gameObject == null)
                    {
                        //add missile 1 to right side of player ship
                        _homingMissilesGO[0] = Instantiate(_homingMissilePrefabGO, transform);
                        _delayAddHomingMissile1 = false;
                    }
                }
                else //count is 1, but not because of a delay
                {
                    if (_missile2Fired || _homingMissilesGO[1].gameObject != null)
                    {
                        //delay adding the missile
                        _delayAddHomingMissile2 = true;
                        _currentHomingMissiles += 1;
                    }
                    else if (_missile2Fired == false && _homingMissilesGO[1].gameObject == null)
                    {
                        //add missile 2 to left side of player ship
                        _homingMissilesGO[1] = Instantiate(_homingMissile2PrefabGO, transform);
                        _currentHomingMissiles += 1;
                        _delayAddHomingMissile2 = false;
                    }
                }
            }
        }
        else if (_currentHomingMissiles == 2)
        {
            //Is the count 2 because of a delay?
            if (_delayAddHomingMissile2 == true)
            {
                //Has the original missile 2 that delayed this missile 2 died?
                if (_missile2Fired == false && _homingMissilesGO[1].gameObject == null)
                {
                    //add missile 2 to the left side of the player ship
                    _homingMissilesGO[1] = Instantiate(_homingMissile2PrefabGO, transform);
                    _delayAddHomingMissile2 = false;
                }
            }
        }
    }

    private void DelayAddHomingMissile()
    {
            AddHomingMissile();
    }

    private void FireHomingMissile()
    {  
        GameObject targetEnemyGO = null;

        if (_fireHomingMissileCooldownActive == false && _currentHomingMissiles > 0)
        {
            targetEnemyGO = UpdateHomingMissileTarget();

            if (targetEnemyGO != null)
            {
                if (_currentHomingMissiles == 2)
                {
                    _missile2Fired = true;
                    _enemyMissileTarget2 = targetEnemyGO;
                    if(_homingMissilesGO[1].transform.parent != null)
                    {
                        _homingMissilesGO[1].transform.parent = null;
                    }
                    //Give it an initial forward motion
                    _homingMissilesGO[1].GetComponent<Rigidbody2D>().AddForce(new Vector3(-1f, 0, 0));
                    _currentHomingMissiles -= 1;
                    StartCoroutine(FireMissileCooldown());
                }
                else if (_currentHomingMissiles == 1)
                {
                    _missile1Fired = true;
                    _enemyMissileTarget1 = targetEnemyGO;
                    if(_homingMissilesGO[0].transform.parent != null)
                    {
                        _homingMissilesGO[0].transform.parent = null;
                    }
                    //Give it an initial forward motion
                    _homingMissilesGO[0].GetComponent<Rigidbody2D>().AddForce(transform.up);
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
    }

    private void SetAllCurrentEnemies()
    {
        GameObject[] enemiesGO = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] mines1GO = GameObject.FindGameObjectsWithTag("Mine");
        GameObject[] mines2GO = GameObject.FindGameObjectsWithTag("FreezeMine");
        GameObject boss = GameObject.FindGameObjectWithTag("Boss");

        int allEnemiesLength = allEnemiesLength = enemiesGO.Length + mines1GO.Length + mines2GO.Length;

        if (boss != null)
        {
            allEnemiesLength += 1;
        }
        
        int currLength;

        GameObject[] allEnemiesGO = new GameObject[allEnemiesLength];
        int[] allEnemyTypes = new int[allEnemiesLength];

        //Add all enemyGO game objects to allEnemiesGO
        currLength = enemiesGO.Length;

        for (int i = 0; i < currLength; i++)
        {
            allEnemiesGO[i] = enemiesGO[i];
            allEnemyTypes[i] = 1;
        }

        //Add all mines1GO
        currLength = mines1GO.Length;

        for (int i = 0 + enemiesGO.Length; i < enemiesGO.Length + currLength; i++)
        {

            allEnemiesGO[i] = mines1GO[i - enemiesGO.Length];
            allEnemyTypes[i] = 2;
        }

        //Add all mines2GO
        currLength = mines2GO.Length;

        for (int i = 0 + enemiesGO.Length + mines1GO.Length; i < enemiesGO.Length + mines1GO.Length + currLength; i++)
        {
            allEnemiesGO[i] = mines2GO[i - enemiesGO.Length - mines1GO.Length];
            allEnemyTypes[i] = 2;
        }

        //Add boss if exists
        if(boss != null)
        {
            allEnemiesGO[allEnemiesLength - 1] = boss.gameObject;
            allEnemyTypes[allEnemiesLength - 1] = 3;
        }

        _allEnemiesGO = allEnemiesGO;
        _allEnemiesTypes = allEnemyTypes;
    }

    private GameObject UpdateHomingMissileTarget()
    {
        int highestEnemyStrength = -1;
        float targetEnemyDistance = -1;
        float currEnemyDistance = -1;
        GameObject targetEnemyGO = null;
        bool enemyHasDied = true;
        int currEnemyStrength = -1;
        int enemyIndex = -1;

        SetAllCurrentEnemies();

        foreach (GameObject enemyGO in _allEnemiesGO)
        {
            enemyIndex += 1;

            if (_allEnemiesTypes[enemyIndex] == 1)
            {
                //Enemy game object
                enemyHasDied = enemyGO.transform.GetComponent<Enemy>().EnemyHasDied();
                currEnemyStrength = enemyGO.transform.GetComponent<Enemy>().GetEnemyStrength();
            } else if (_allEnemiesTypes[enemyIndex] == 2)
            {
                //Mine game object
                enemyHasDied = enemyGO.transform.GetComponent<Mine>().EnemyHasDied();
                currEnemyStrength = enemyGO.transform.GetComponent<Mine>().GetEnemyStrength();
            } else if (_allEnemiesTypes[enemyIndex] == 3)
            {
                //Boss game object
                enemyHasDied = enemyGO.transform.GetComponent<Boss1>().EnemyHasDied();
                currEnemyStrength = enemyGO.transform.GetComponent<Boss1>().GetEnemyStrength();
            }

            if (enemyHasDied == false)
            {
                //Check current enemyGO strength compared to highest strength so far
                if (currEnemyStrength > highestEnemyStrength)
                {
                    //make this the targetEnemy until a better one is found
                    highestEnemyStrength = currEnemyStrength;
                    targetEnemyGO = enemyGO;
                    targetEnemyDistance = Vector3.Distance(transform.position, targetEnemyGO.transform.position);
                }
                else if (currEnemyStrength == highestEnemyStrength)
                {
                    //current enemyGO is = strength to targetEnemyGO, which one is closer?
                    currEnemyDistance = Vector3.Distance(transform.position, enemyGO.transform.position);

                    if (targetEnemyDistance != -1)
                    {
                        if (currEnemyDistance < targetEnemyDistance)
                        {
                            //make this the targetEnemy until a better one is found
                            highestEnemyStrength = currEnemyStrength;
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
        }
        return targetEnemyGO;
    }

    private void MoveHomingMissiles()
    {
        if (_missile2Fired && _homingMissilesGO[1] != null)
        {
            //Move missile2

            if (_enemyMissileTarget2 == null)
            {
                _enemyMissileTarget2 = UpdateHomingMissileTarget();
            }
            MoveHomingMissile(1, _enemyMissileTarget2);
        }
        else if (_missile1Fired & _homingMissilesGO[0] != null)
        {
            //Move missile1

            if (_enemyMissileTarget1 == null)
            {
                _enemyMissileTarget1 = UpdateHomingMissileTarget();
            }
            MoveHomingMissile(0, _enemyMissileTarget1);
        }
        else
        {
            //Debug.Log("Player::MoveHomingMissiles() - method called but _missile1Fired and _missile2Fired are both false");
        }
    }

    private void MoveHomingMissile(int missileID, GameObject missileTargetGO)
    {
        Vector3 direction;
        float rot_z;

        if (missileTargetGO == null)
        {
            missileTargetGO = UpdateHomingMissileTarget();
        }

        if (missileTargetGO != null && _homingMissilesGO[missileID] != null)
        {
            //Rotate towards enemy
            direction = missileTargetGO.transform.position - _homingMissilesGO[missileID].transform.position;

            direction.Normalize();

            rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _homingMissilesGO[missileID].transform.rotation = Quaternion.Euler(0f, 0f, rot_z + -180);

            _homingMissilesGO[missileID].transform.Translate(direction * _homingMissileSpeed * Time.deltaTime);
        }
        else
        {
            //Debug.Log("Player::MoveHomingMissile(): either missileTargetGO = null or _homingMissilesGO[missileID] = null");
        }
    }

    private IEnumerator FireMissileCooldown()
    {
        _fireHomingMissileCooldownActive = true;
        yield return new WaitForSeconds(1);
    }

    public void EnableHomingMissileFiring()
    {
        _fireHomingMissileCooldownActive = false;
        _moveHomingMissiles = false;

        if (_missile1Fired == true)
        {
            _missile1Fired = false;
        }
        
        if (_missile2Fired == true)
        {
            _missile2Fired = false;
        }
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

            //Tell bosses player died
            GameObject[] allBossesGO = GameObject.FindGameObjectsWithTag("Boss");

            if (allEnemiesGO != null) {
                foreach (GameObject enemyGO in allEnemiesGO)
                {
                    enemyGO.GetComponent<Enemy>().PlayerDied();
                }
            } else { Debug.Log("Player::LoseLife()- allEnemiesGO is null!"); }
           
            if (allBossesGO != null)
            {
                foreach (GameObject bossGO in allBossesGO)
                {
                    bossGO.GetComponent<Boss1>().PlayerDied();
                }
            } else { Debug.Log("Player::LoseLife()- allBossesGO is nulL!"); }

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

            if (_spawnManager != null)
            {
                _spawnManager.GameOver();
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
        } else if (other.tag == "Mine")
        {
            Destroy(other.gameObject);
            LoseLife();
        } else if (other.tag == "FreezeMine")
        {
            Destroy(other.gameObject);
            ShakeCamera(1f, 0.5f);
            Stunned(3f);
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
