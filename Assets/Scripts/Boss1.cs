using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1 : MonoBehaviour
{
    //Game object references
    [SerializeField]
    private GameObject _spawnManagerGO;
    [SerializeField]
    private GameObject _playerGO;
    private SpawnManager _spawnManagerScript;
    private Player _playerScript;

    //Game object prefabs
    [SerializeField]
    private GameObject _mine1PrefabGO;
    [SerializeField]
    private GameObject _mine2PrefabGO;
    [SerializeField]
    private GameObject _mine3PrefabGO;
    [SerializeField]
    private GameObject _laserPrefabGO;

    //General boss variables
    private bool _playerHasDied = false;
    [SerializeField]
    private int _bossHealth = 300;
    private bool _rotateTowardsPlayer = false;
    private bool _rotateTowardsTarget = false;
    [SerializeField]
    private Vector3 _directionOfPlayer;
    [SerializeField]
    private Vector3 _directionOfTarget;
    private Vector3 _targetPosition;
    private int _enemyStrength = 25;
    private bool _isDying = false;

    //Laser attack variables
    private Vector3 _laser1FireFromPosition;
    private Quaternion _laser1FireFromRotation;
    private Vector3 _laser2FireFromPosition;
    private Quaternion _laser2FireFromRotation;
    private Vector3[] _laser3Targets = new Vector3[13];
    [SerializeField]
    private float _laserAttack1DelayInterval = 2f;
    [SerializeField]
    private float _laserAttack2DelayInterval = 1f;
    [SerializeField]
    private float _laserAttack3DelayInterval = 0.25f;
    [SerializeField]
    private GameObject[] _laserGO = new GameObject[30];

    //Mine attack variables
    [SerializeField]
    private float _mine1Speed = 4f;
    [SerializeField]
    private float _mine2Speed = 4f;
    [SerializeField]
    private float _mine3Speed = 4f;
    [SerializeField]
    private bool _minesAlive = false;
    private GameObject[] _mines1GO = new GameObject[10];
    private GameObject[] _mines2GO = new GameObject[10];
    private GameObject[] _mines3GO = new GameObject[10];

    //Ram attack variables
    [SerializeField]
    private float _bossMoveSpeed = 3f;
    [SerializeField]
    private float _bossRamSpeed = 7f;
    [SerializeField]
    private float _bossRamDelay = 3f;
    [SerializeField]
    private float _ramPlayerDelay = 45f;
    [SerializeField]
    private bool _ramPlayerActive = false;

    //Boss action boolean flags
    [SerializeField]
    private bool _disableBossActions = true;
    [SerializeField]
    private bool _moveBossForRam = false;
    [SerializeField]
    private bool _bossAboveScreen = true;
    [SerializeField]
    private bool _bossActionInProgress = false;
    [SerializeField]
    private bool _startRamPlayer = false;


    // Start is called before the first frame update
    void Start()
    {
        _spawnManagerGO = GameObject.Find("SpawnManager");

        if (_spawnManagerGO != null)
        {
            _spawnManagerScript = _spawnManagerGO.GetComponent<SpawnManager>();
        }

        _playerGO = GameObject.FindGameObjectWithTag("Player");

        if (_playerGO != null)
        {
            _playerScript = _playerGO.GetComponent<Player>();
        }

        DoNullChecks();

        //TESTING PURPOSES ONLY
        //StartCoroutine(StartRamPlayerTrue());
        //StartCoroutine(StartMineDeployment());
        //StartCoroutine(StartLaserAttack());
    }

    // Update is called once per frame
    void Update()
    {
        //Do not move mines if all mines destroyed
        CheckMinesAlive();

        if (_playerHasDied == false)
        {
            //Has boss reached starting position?
            if (_bossAboveScreen == true && transform.position.y > 5f)
            {
                _disableBossActions = true;

                //Move down onto player screen
                transform.Translate(new Vector3(0, -1f, 0) * _bossMoveSpeed * Time.deltaTime);
            }
            else //Boss reached starting position
            {
                _disableBossActions = false;
                _bossAboveScreen = false;
                _ramPlayerActive = false;
            }

            //Start a boss action
            if (_bossAboveScreen == false && _bossActionInProgress == false && _disableBossActions == false && _minesAlive == false && _ramPlayerActive == false)
            {
                int randomNumber = (int)Random.Range(0, 10);
                _bossActionInProgress = true;

                //Fire laser, initiate mine attack, or Ram player randomly
                if (randomNumber > 6) //Start random laser attack
                {
                    StartRandomLaserAttack();
                }
                else if (randomNumber > 2) //Start random mine attack
                {
                    DeployRandomMineAttack();
                }
                else //Start ram player attack
                {
                    _startRamPlayer = true;
                    _ramPlayerActive = true;
                }
            }

            //Move mines
            if (_minesAlive == true)
            {
                MoveMines1();
                MoveMines2();
                MoveMines3();
            }

            //Initiate Ram action if triggered
            if (_startRamPlayer == true)
            {
                InitiateRamPlayer();
                _startRamPlayer = false;
            }

            //Move boss at ram speed
            if (_moveBossForRam == true)
            {
                transform.Translate(Vector3.down * _bossRamSpeed * Time.deltaTime);
            } 

            if (_rotateTowardsPlayer == true)
            {
                RotateTowardsPlayer();
            }

            if (_rotateTowardsTarget == true)
            {
                RotateToTarget();
            }

            CheckBossHealth();
        }
    }

    //TESTING PURPOSES
    private IEnumerator StartRamPlayerTrue()
    {
        yield return new WaitForSeconds(4f);
        _startRamPlayer = true;
    }

    //TESTING PURPOSES
    private IEnumerator StartMineDeployment()
    {
        yield return new WaitForSeconds(4f);

        //DeployRandomMineAttack();
        //DeployMineAttack1(_mine1PrefabGO);
        //DeployMineAttack2(_mine2PrefabGO);
        //DeployMineAttack3(_mine3PrefabGO);
    }

    //TESTING PURPOSES
    private IEnumerator StartLaserAttack()
    {
        yield return new WaitForSeconds(4f);

        //StartRandomLaserAttack();
        //StartCoroutine(LaserAttack1());
        //StartCoroutine(LaserAttack2());
        StartCoroutine(LaserAttack3A());
        //StartCoroutine(LaserAttack3B());
    }

    private void StartRandomLaserAttack()
    {
        int randomNumber = (int)Random.Range(0, 9);

        //Randomly determine laser attack
        if (randomNumber > 7)
        {
            StartCoroutine(LaserAttack3A());
        }
        else if (randomNumber == 6)
        {
            StartCoroutine(LaserAttack3B());
        }
        else if (randomNumber > 2)
        {
            StartCoroutine(LaserAttack2());
        }
        else
        {
            StartCoroutine(LaserAttack1());
        }
    }

    //Track player for 3 seconds, then fire 5 bursts with a small delay between each
    private IEnumerator LaserAttack1()
    {
        _rotateTowardsPlayer = true;
        PopulateLaserGOArray(10);
        yield return new WaitForSeconds(3f);

        FireLasers(_laserGO[0], _laserGO[1], _directionOfPlayer);

        StartCoroutine(LaserAttackDelay(_laserGO[2], _laserGO[3], _laserAttack1DelayInterval, false));
        StartCoroutine(LaserAttackDelay(_laserGO[4], _laserGO[5], _laserAttack1DelayInterval * 2, false));
        StartCoroutine(LaserAttackDelay(_laserGO[6], _laserGO[7], _laserAttack1DelayInterval * 3, false));
        StartCoroutine(LaserAttackDelay(_laserGO[8], _laserGO[9], _laserAttack1DelayInterval * 4, true));
    }

    //Track player for 5 seconds, then fire 7 rapid bursts
    private IEnumerator LaserAttack2()
    {
        _rotateTowardsPlayer = true;
        PopulateLaserGOArray(14);
        yield return new WaitForSeconds(5f);

        FireLasers(_laserGO[0], _laserGO[1], _directionOfPlayer);

        StartCoroutine(LaserAttackDelay(_laserGO[2], _laserGO[3], _laserAttack2DelayInterval, false));
        StartCoroutine(LaserAttackDelay(_laserGO[4], _laserGO[5], _laserAttack2DelayInterval * 2, false));
        StartCoroutine(LaserAttackDelay(_laserGO[4], _laserGO[5], _laserAttack2DelayInterval * 3, false));
        StartCoroutine(LaserAttackDelay(_laserGO[4], _laserGO[5], _laserAttack2DelayInterval * 4, false));
        StartCoroutine(LaserAttackDelay(_laserGO[4], _laserGO[5], _laserAttack2DelayInterval * 5, false));
        StartCoroutine(LaserAttackDelay(_laserGO[4], _laserGO[5], _laserAttack2DelayInterval * 6, true));
    }

    //Does not aim at player, aims at the top left and rotates in 180 degrees until aimed at top right of screen
    //Fires every second along the way
    //A - left to right B - Right to left
    private IEnumerator LaserAttack3A()
    {
        PopulateLaserGOArray(26);
        PopulateLaser3AttackTargets();
        //rotate towards first target
        _targetPosition = (_laser3Targets[0]);
        _rotateTowardsTarget = true;
        yield return new WaitForSeconds(1f);

        //fire lasers
        FireLasers(_laserGO[0], _laserGO[1], _directionOfTarget);

        //cycle through each remaining targets and fire
        StartCoroutine(LaserAttackTargetDelay(_laserGO[2], _laserGO[3], _laserAttack3DelayInterval, _laser3Targets[1], false));
        StartCoroutine(LaserAttackTargetDelay(_laserGO[4], _laserGO[5], _laserAttack3DelayInterval * 2, _laser3Targets[2], false));
        StartCoroutine(LaserAttackTargetDelay(_laserGO[6], _laserGO[7], _laserAttack3DelayInterval * 3, _laser3Targets[3], false));
        StartCoroutine(LaserAttackTargetDelay(_laserGO[8], _laserGO[9], _laserAttack3DelayInterval * 4, _laser3Targets[4], false));
        StartCoroutine(LaserAttackTargetDelay(_laserGO[10], _laserGO[11], _laserAttack3DelayInterval * 5, _laser3Targets[5], false));
        StartCoroutine(LaserAttackTargetDelay(_laserGO[12], _laserGO[13], _laserAttack3DelayInterval * 6, _laser3Targets[6], false));
        StartCoroutine(LaserAttackTargetDelay(_laserGO[14], _laserGO[15], _laserAttack3DelayInterval * 7, _laser3Targets[7], false));
        StartCoroutine(LaserAttackTargetDelay(_laserGO[16], _laserGO[17], _laserAttack3DelayInterval * 8, _laser3Targets[8], false));
        StartCoroutine(LaserAttackTargetDelay(_laserGO[18], _laserGO[19], _laserAttack3DelayInterval * 9, _laser3Targets[9], false));
        StartCoroutine(LaserAttackTargetDelay(_laserGO[20], _laserGO[21], _laserAttack3DelayInterval * 10, _laser3Targets[10], false));
        StartCoroutine(LaserAttackTargetDelay(_laserGO[22], _laserGO[23], _laserAttack3DelayInterval * 11, _laser3Targets[11], false));
        StartCoroutine(LaserAttackTargetDelay(_laserGO[24], _laserGO[25], _laserAttack3DelayInterval * 12, _laser3Targets[12], true));        
    }

    private IEnumerator LaserAttack3B()
    {
        PopulateLaserGOArray(26);
        PopulateLaser3AttackTargets();
        //rotate towards first target
        _targetPosition = (_laser3Targets[12]);
        _rotateTowardsTarget = true;
        yield return new WaitForSeconds(1f);

        //fire lasers
        FireLasers(_laserGO[0], _laserGO[1], _directionOfTarget);

        //cycle through each remaining targets and fire
        StartCoroutine(LaserAttackTargetDelay(_laserGO[2], _laserGO[3], _laserAttack3DelayInterval, _laser3Targets[11], false));
        StartCoroutine(LaserAttackTargetDelay(_laserGO[4], _laserGO[5], _laserAttack3DelayInterval * 2, _laser3Targets[10], false));
        StartCoroutine(LaserAttackTargetDelay(_laserGO[6], _laserGO[7], _laserAttack3DelayInterval * 3, _laser3Targets[9], false));
        StartCoroutine(LaserAttackTargetDelay(_laserGO[8], _laserGO[9], _laserAttack3DelayInterval * 4, _laser3Targets[8], false));
        StartCoroutine(LaserAttackTargetDelay(_laserGO[10], _laserGO[11], _laserAttack3DelayInterval * 5, _laser3Targets[7], false));
        StartCoroutine(LaserAttackTargetDelay(_laserGO[12], _laserGO[13], _laserAttack3DelayInterval * 6, _laser3Targets[6], false));
        StartCoroutine(LaserAttackTargetDelay(_laserGO[14], _laserGO[15], _laserAttack3DelayInterval * 7, _laser3Targets[5], false));
        StartCoroutine(LaserAttackTargetDelay(_laserGO[16], _laserGO[17], _laserAttack3DelayInterval * 8, _laser3Targets[4], false));
        StartCoroutine(LaserAttackTargetDelay(_laserGO[18], _laserGO[19], _laserAttack3DelayInterval * 9, _laser3Targets[3], false));
        StartCoroutine(LaserAttackTargetDelay(_laserGO[20], _laserGO[21], _laserAttack3DelayInterval * 10, _laser3Targets[2], false));
        StartCoroutine(LaserAttackTargetDelay(_laserGO[22], _laserGO[23], _laserAttack3DelayInterval * 11, _laser3Targets[1], false));
        StartCoroutine(LaserAttackTargetDelay(_laserGO[24], _laserGO[25], _laserAttack3DelayInterval * 12, _laser3Targets[0], true));
    }

    private IEnumerator LaserAttackDelay(GameObject laser1, GameObject laser2, float delay, bool lastBurst)
    {
        yield return new WaitForSeconds(delay);

        FireLasers(laser1, laser2, _directionOfPlayer);

        if (lastBurst == true)
        {
            _rotateTowardsPlayer = false;
            //Reset boss rotation to standard look down
            RotateToLookDown();
            StartCoroutine(EndBossAction(1f));
        }
    }

    private IEnumerator LaserAttackTargetDelay(GameObject laser1, GameObject laser2, float delay, Vector3 targetPosition, bool lastBurst)
    {
        yield return new WaitForSeconds(delay);
        _targetPosition = targetPosition;

        FireLasers(laser1, laser2, _directionOfTarget);

        if (lastBurst == true)
        {
            _rotateTowardsTarget = false;
            //Reset boss rotation to standard look down
            RotateToLookDown();
            StartCoroutine(EndBossAction(1f));
        }
    }

    private void FireLasers(GameObject laser1, GameObject laser2, Vector3 direction)
    {
        //Set laser fire from positions and rotations
        _laser1FireFromPosition = transform.Find("LaserPosition1").position;
        _laser2FireFromPosition = transform.Find("LaserPosition2").position;
        _laser1FireFromRotation = transform.Find("LaserPosition1").rotation;
        _laser2FireFromRotation = transform.Find("LaserPosition2").rotation;

        //Apply positions and rotations to laser game objects
        if (laser1 != null)
        {
            laser1.transform.SetPositionAndRotation(_laser1FireFromPosition, _laser1FireFromRotation);
            //Apply direction to laser game objects
            laser1.transform.GetComponent<Laser>().SetDirection(direction);
            //Apply desired laser speed
            laser1.transform.GetComponent<Laser>().SetSpeed(6f);
        }

        if (laser2 != null)
        {
            laser2.transform.SetPositionAndRotation(_laser2FireFromPosition, _laser2FireFromRotation);
            //Apply direction to laser game objects
            laser2.transform.GetComponent<Laser>().SetDirection(direction);
            //Apply desired laser speed
            laser2.transform.GetComponent<Laser>().SetSpeed(6f);
        }
    }

    private void PopulateLaserGOArray(int lasersNeeded)
    {
        //Clear out any existing laser game objects
        for (int i = 0; i < 30; i++)
        {
            if (_laserGO[i].gameObject != null)
            {
                Destroy(_laserGO[i].gameObject);
            }
        }

        for (int i = 0; i < lasersNeeded; i++)
        {
            _laserGO[i] = Instantiate(_laserPrefabGO, new Vector3(-5f, 15f, 0f), Quaternion.identity);
            _laserGO[i].transform.GetComponent<Laser>().SetDirection(new Vector3(0, 0, 0));
            _laserGO[i].gameObject.tag = "EnemyLaser";
        }
    }

    private void PopulateLaser3AttackTargets()
    {
        _laser3Targets[0] = new Vector3(-15.23f, 2.5f, 0f);
        _laser3Targets[1] = new Vector3(-15.3f, -2.5f, 0f);
        _laser3Targets[2] = new Vector3(-13.75f, -7.5f, 0f);
        _laser3Targets[3] = new Vector3(-9.65f, -12.5f, 0f);
        _laser3Targets[4] = new Vector3(-6.35f, -14.47f, 0f);
        _laser3Targets[5] = new Vector3(-3.18f, -15.47f, 0f);
        _laser3Targets[6] = new Vector3(0f, -15.73f, 0f);
        _laser3Targets[7] = new Vector3(3.18f, -15.47f, 0f);
        _laser3Targets[8] = new Vector3(6.35f, -14.47f, 0f);
        _laser3Targets[9] = new Vector3(9.65f, -12.5f, 0f);
        _laser3Targets[10] = new Vector3(13.75f, -7.5f, 0f);
        _laser3Targets[11] = new Vector3(15.3f, -2.5f, 0f);
        _laser3Targets[12] = new Vector3(15.23f, 2.5f, 0f);
    }

    private void DeployRandomMineAttack()
    {
        int randomNumber = (int)Random.Range(0, 10);
        int randomNumber2 = (int)Random.Range(0, 10);

        //Randomly select a mine attack
        if (randomNumber > 5) //Mine attack 1
        {
            if (randomNumber2 > 4)
            {
                DeployMineAttack1(_mine1PrefabGO);
                //Debug.Log("Boss1::DeployRandomMineAttack() - Mine attack 1, _mine1PrefabGO used");
            }
            else //Freeze or Stunner mines
            {
                DeployMineAttack1(_mine3PrefabGO);
                //Debug.Log("Boss1::DeployRandomMineAttack() - Mine attack 1, _mine3PrefabGO used");
            }
        }
        else if (randomNumber > 1) //Mine attack 2
        {
            if (randomNumber2 > 4)
            {
                DeployMineAttack2(_mine2PrefabGO);
                //Debug.Log("Boss1::DeployRandomMineAttack() - Mine attack 2, _mine2PrefabGO used");
            }
            else //Freeze or Stunner mines
            {
                DeployMineAttack2(_mine3PrefabGO);
                //Debug.Log("Boss1::DeployRandomMineAttack() - Mine attack 2, _mine3PrefabGO used");
            }
        }
        else //Mine attack 3
        {
            //Freeze or Stunner mines
            DeployMineAttack3(_mine3PrefabGO);
            //Debug.Log("Boss1::DeployRandomMineAttack() - Mine attack 3, _mine3PrefabGO used");
        }
    }
   
    // 3 player seeking mines are deployed to hunt down the player and explode on contact
    private void DeployMineAttack1(GameObject mine) //, int mineType)
    {
        _minesAlive = true;
        Vector3 randomPosition = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 1f), 0f);

        //_mines1GO[0] = Instantiate(mine, transform.position + randomPosition, Quaternion.identity);
        StartCoroutine(DelayDeployMine(mine, 1, 0, transform.position + randomPosition, 0f));

        randomPosition = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 1f), 0f);

        StartCoroutine(DelayDeployMine(mine, 1, 1, transform.position + randomPosition, 4f));

        randomPosition = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 1f), 0f);
        StartCoroutine(DelayDeployMine(mine, 1, 2, transform.position + randomPosition, 9f));
        StartCoroutine(EndBossAction(10f));
    }

    private void MoveMines1() //(int mineType)
    {
        if (_mines1GO != null)
        {
            foreach (GameObject mine in _mines1GO)
            {
                if (mine != null)
                {
                    MoveMineToDestination(mine, _playerGO.transform.position);
                }
            }
        }
        else
        {
            Debug.Log("Boss1::MoveMines1()- _mines1GO is null! Cannot move associated mines.");
        }
    }

    // Multiple mines deployed that move in a set half circle pattern, moving straight off of the player screen,
    // but exploding on player contact
    private void DeployMineAttack2(GameObject mine) //, int mineType)
    {
        _minesAlive = true;
        Vector3 mine1Position = new Vector3(-1.75f, 4f, 0f);
        Vector3 mine2Position = new Vector3(1.75f, 4f, 0f);
        Vector3 mine3Position = new Vector3(-1.15f, 2.77f, 0f);
        Vector3 mine4Position = new Vector3(1.15f, 2.77f, 0f);
        Vector3 mine5Position = new Vector3(0f, 1.8f, 0f);

        _mines2GO[0] = Instantiate(mine, mine1Position, Quaternion.identity);
        _mines2GO[1] = Instantiate(mine, mine2Position, Quaternion.identity);
        _mines2GO[2] = Instantiate(mine, mine3Position, Quaternion.identity);
        _mines2GO[3] = Instantiate(mine, mine4Position, Quaternion.identity);
        _mines2GO[4] = Instantiate(mine, mine5Position, Quaternion.identity);
        StartCoroutine(EndBossAction(4f));
    }

    private void MoveMines2() //(int mineType)
    {
        //Set direction for each mine
        Vector3 mine1Destination = new Vector3(-15f, 1f, 0f);
        Vector3 mine2Destination = new Vector3(15f, 1f, 0f);
        Vector3 mine3Destination = new Vector3(-15f, -6f, 0f);
        Vector3 mine4Destination = new Vector3(15f, -6f, 0f);
        Vector3 mine5Destination = new Vector3(0f, -8f, 0f);

        //Check if mines are off screen, if so Destroy()
        if (_mines2GO != null)
        {
            foreach (GameObject mine in _mines2GO)
            {
                if (mine != null)
                {
                    if (mine.transform.position.x > 12 || mine.transform.position.x < -12 || mine.transform.position.y > 8 || mine.transform.position.y < -7)
                    {
                        Destroy(mine.gameObject);
                    }
                }
            }

            //Radiate out in half circle pattern until off screen
            MoveMineToDestination(_mines2GO[0], mine1Destination);
            MoveMineToDestination(_mines2GO[1], mine2Destination);
            MoveMineToDestination(_mines2GO[2], mine3Destination);
            MoveMineToDestination(_mines2GO[3], mine4Destination);
            MoveMineToDestination(_mines2GO[4], mine5Destination);
        }
        else
        {
            Debug.Log("Boss1::MoveMines2()- _mines1GO is null! Cannot move associated mines.");
        }
    }

    // 2 player seeking mines that stun the player on contact
    private void DeployMineAttack3(GameObject mine)
    {
        _minesAlive = true;

        Vector3 randomPosition = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 1f), 0f);
        _mines3GO[0] = Instantiate(mine, transform.position + randomPosition, Quaternion.identity);

        randomPosition = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 1f), 0f);
        StartCoroutine(DelayDeployMine(mine, 3, 1, transform.position + randomPosition, 4f));
        StartCoroutine(EndBossAction(8f));
    }

    private void MoveMines3()
    {
        if (_mines3GO != null)
        {
            foreach (GameObject mine in _mines3GO)
            {
                if (mine != null)
                {
                    MoveMineToDestination(mine, _playerGO.transform.position);
                }
            }
        }
        else
        {
            Debug.Log("Boss1::MoveMines1()- _mines3GO is null! Cannot move associated mines.");
        }
    }

    //Delay deploy mines when needed
    private IEnumerator DelayDeployMine(GameObject mine, int mineAttackType, int arrayID, Vector3 spawnPosition, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        switch(mineAttackType)
        {
            case 1: //MineAttack1 | _mines1GO[] - 3 Total
                _mines1GO[arrayID] = Instantiate(mine, spawnPosition, Quaternion.identity);
                break;
            case 2: //MineAttack2 | _mines2GO[] - 5 Total
                _mines2GO[arrayID] = Instantiate(mine, spawnPosition, Quaternion.identity);
                break;
            case 3: //MineAttack3 | _mines3GO[] - 2 Total
                _mines3GO[arrayID] = Instantiate(mine, spawnPosition, Quaternion.identity);
                break;
        }
    }

    private void MoveMineToDestination(GameObject mine, Vector3 mineDestination)
    {
        if (mine != null)
        {
            Vector3 mineDirection = mineDestination - mine.transform.position;

            mineDirection.Normalize();

            //Rotate in mineDirection
            float rot_z = Mathf.Atan2(mineDirection.y, mineDirection.x) * Mathf.Rad2Deg;
            mine.transform.rotation = Quaternion.Euler(0f, 0f, rot_z + 90);

            //Move towards player   
            mine.transform.Translate(Vector3.down * _mine2Speed * Time.deltaTime);
        }
    }

    // Check if any mines are alive
    private void CheckMinesAlive()
    {
        GameObject[] minesAlive = GameObject.FindGameObjectsWithTag("Mine");
        GameObject[] freezeMinesAlive = GameObject.FindGameObjectsWithTag("FreezeMine");

        if (minesAlive == null && freezeMinesAlive == null)
        {
            _minesAlive = false;
        }
        else
        {
            _minesAlive = true;
        }

        if (minesAlive.Length == 0 && freezeMinesAlive.Length == 0)
        {
            _minesAlive = false;
        }
    }

    private void InitiateRamPlayer()
    {
        //Aim at player
        _rotateTowardsPlayer = true;

        //Wait _bossRamDelay seconds before moving
        StartCoroutine(RamPlayerRoutine());
    }

    private IEnumerator RamPlayerRoutine()
    {
        yield return new WaitForSeconds(_bossRamDelay);
        RamPlayer();
    }

    private void RamPlayer()
    {
        _rotateTowardsPlayer = false;

        StartCoroutine(RamDelay());
    }

    private IEnumerator RamDelay()
    {
        yield return new WaitForSeconds(0.2f);
        _moveBossForRam = true;

        StartCoroutine(ReturnToStart());
    }

    private IEnumerator ReturnToStart()
    {
        yield return new WaitForSeconds(5f);
        ReturnToStartPosition();
    }

    private void ReturnToStartPosition()
    {
        _moveBossForRam = false;

        //Reset above player screen with normal rotation
        transform.SetPositionAndRotation(new Vector3(0, 15, 0), Quaternion.identity);

        //Move back to start position
        _bossAboveScreen = true;
        _bossActionInProgress = false;
    }

    private IEnumerator EndBossAction(float delay)
    {
        yield return new WaitForSeconds(delay);
        _bossActionInProgress = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            _bossHealth -= 2;

            Destroy(other.gameObject);
        }
        else if (other.tag == "Missile")
        {
            _bossHealth -= 5;
            _playerScript.EnableHomingMissileFiring();

            Destroy(other.gameObject);
        }
        else if (other.tag == "Player")
        {
            _playerScript.LoseLife();
            _bossHealth -= 5;
        }
    }

    private void RotateTowardsPlayer()
    {
        //Rotate boss to face player

        if (_moveBossForRam == false)
        {
            //Rotate towards player
            _directionOfPlayer = _playerGO.transform.position - transform.position;

            _directionOfPlayer.Normalize();

            float rot_z = Mathf.Atan2(_directionOfPlayer.y, _directionOfPlayer.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rot_z + 90);
        }
    }

    private void RotateToLookDown()
    {
        //Rotate towards start position, looking down
        Vector3 downDirection = new Vector3(0f, -15f, 0f) - transform.position;

        downDirection.Normalize();

        float rot_z = Mathf.Atan2(downDirection.y, downDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z + 90);
    }
    private void RotateToTarget()
    {
        //Rotate towards player
        _directionOfTarget = _targetPosition - transform.position;

        _directionOfTarget.Normalize();

        float rot_z = Mathf.Atan2(_directionOfTarget.y, _directionOfTarget.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z + 90);
    }

    private void CheckBossHealth()
    {
        if (_bossHealth < 1)
        {
            OnDeath();
        }
    }

    private void DestroyAllMines()
    {
        foreach (GameObject mine in _mines1GO)
        {
            if (mine != null)
            {
                Destroy(mine);
            }
        }

        foreach (GameObject mine in _mines2GO)
        {
            if (mine != null)
            {
                Destroy(mine);
            }
        }

        foreach (GameObject mine in _mines3GO)
        {
            if (mine != null)
            {
                Destroy(mine);
            }
        }
    }

    private void OnDeath()
    {
        _isDying = true;

        //Kill all mines still alive
        DestroyAllMines();

        _spawnManagerScript.VictoryGameOver();
        transform.GetComponent<SpriteRenderer>().enabled = false;
        Destroy(transform.gameObject, 1f);
    }

    public void PlayerDied()
    {
        _playerHasDied = true;
    }

    public bool EnemyHasDied()
    {
        return _isDying;
    }

    public int GetEnemyStrength()
    {
        return _enemyStrength;
    }

    private void DoNullChecks()
    {
        if(_playerGO == null)
        {
            Debug.Log("Boss1::DoNullChecks()- _playerGO is null!");
        }

        if(_playerScript == null)
        {
            Debug.Log("Boss1::DoNullChecks()- _playerScript is null!");
        }

        if (_spawnManagerGO == null)
        {
            Debug.Log("Boss1::DoNullChecks()- _spawnManagerGO is null!");
        }

        if(_spawnManagerScript == null)
        {
            Debug.Log("Boss1::DoNullChecks()- _spawnManagerScript is null!");
        }

        if(_mine1PrefabGO == null)
        {
            Debug.Log("Boss1::DoNullChecks()- _mine1PrefabGO is null!");
        }

        if (_mine2PrefabGO == null)
        {
            Debug.Log("Boss1::DoNullChecks()- _mine2PrefabGO is null!");
        }

        if (_mine3PrefabGO == null)
        {
            Debug.Log("Boss1::DoNullChecks()- _mine3PrefabGO is null!");
        }
    }
}
