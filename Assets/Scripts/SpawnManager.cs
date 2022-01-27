using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemiesContainer;
    [SerializeField]
    private GameObject _enemyGO;
    [SerializeField]
    private Vector3 _defaultSpawnPosition = new Vector3(0, 8, 0);
    [SerializeField]
    private int _enemySpawnInterval = 5;
    [SerializeField]
    private GameObject _powerupsContainer;
    [SerializeField]
    private GameObject _powerupTripleShotGO;
    [SerializeField]
    private GameObject _powerupSpeedBoostGO;
    [SerializeField]
    private GameObject _powerupShieldGO;
    [SerializeField]
    private GameObject _powerupAmmoRechargeGO;
    [SerializeField]
    private GameObject _powerupHealthGO;
    [SerializeField]
    private GameObject _powerupLaserBurstGO;
    [SerializeField]
    private GameObject _powerupAddShieldPowerGO;
    [SerializeField]
    private GameObject _powerupRemoveShieldsGO;
    [SerializeField]
    private GameObject _powerupHomingMissileGO;
    [SerializeField]
    private float _spawnWave2DelayTime = 9999f;
    [SerializeField]
    private float _spawnWave3DelayTime = 9999f;
    [SerializeField]
    private float _spawnWave4DelayTime = 9999f;
    [SerializeField]
    private float _spawnWave5DelayTime = 9999f;
    [SerializeField]
    private GameObject _newWaveTextGO;
    [SerializeField]
    private bool _start2ndWaveActive = false;
    private bool _start3rdWaveActive = false;
    private bool _start4thWaveActive = false;
    private bool _start5thWaveActive = false;

    private bool _isDead = false;


    // Start is called before the first frame update
    void Start()
    {
        DoNulLChecks();
    }

    // Update is called once per frame
    void Update()
    {
        //Spawn enemy waves
        if (_start2ndWaveActive == true && Time.time > _spawnWave2DelayTime)
        {
            _newWaveTextGO.transform.GetComponent<TMP_Text>().SetText("Wave 2 has begun!");
            _newWaveTextGO.SetActive(true);
            StartCoroutine(HideNewWaveText());
            StartCoroutine(SpawnEnemyRoutine(_enemyGO, _defaultSpawnPosition, 6f, 15f));
            _start2ndWaveActive = false;
        }  
        else if (_start3rdWaveActive == true && Time.time > _spawnWave3DelayTime)
        {
            _newWaveTextGO.transform.GetComponent<TMP_Text>().SetText("Wave 3 has begun!");
            _newWaveTextGO.SetActive(true);
            StartCoroutine(HideNewWaveText());
            StartCoroutine(SpawnEnemyRoutine(_enemyGO, _defaultSpawnPosition, 9f, 20f));
            _start3rdWaveActive = false;
        }
        else if (_start4thWaveActive == true && Time.time > _spawnWave4DelayTime)
        {
            _newWaveTextGO.transform.GetComponent<TMP_Text>().SetText("Wave 4 has begun!");
            _newWaveTextGO.SetActive(true);
            StartCoroutine(HideNewWaveText());
            StartCoroutine(SpawnEnemyRoutine(_enemyGO, _defaultSpawnPosition, 11f, 20f));
            _start4thWaveActive = false;
        }
        else if (_start5thWaveActive == true && Time.time > _spawnWave5DelayTime)
        {
            _newWaveTextGO.transform.GetComponent<TMP_Text>().SetText("Wave 5 has begun!");
            _newWaveTextGO.SetActive(true);
            StartCoroutine(HideNewWaveText());
            StartCoroutine(SpawnEnemyRoutine(_enemyGO, _defaultSpawnPosition, 20f, 30f));
            _start5thWaveActive = false;
        }
    }

    public void StartSpawningWave1()
    {
        SetWaveTimes();
        _start2ndWaveActive = true;
        StartCoroutine(SpawnEnemyRoutine(_enemyGO, _defaultSpawnPosition, 3f, 9f));
        StartCoroutine(SpawnTripleShotRoutine(_powerupTripleShotGO, _defaultSpawnPosition, 12f, 25f));
        StartCoroutine(SpawnSpeedBoostRoutine(_powerupSpeedBoostGO, _defaultSpawnPosition, 19f, 35f));
        StartCoroutine(SpawnShieldRoutine(_powerupShieldGO, _defaultSpawnPosition, 35f, 65f));
        StartCoroutine(SpawnAmmoChargeRoutine(_powerupAmmoRechargeGO, _defaultSpawnPosition, 15f, 20f));
        StartCoroutine(SpawnHealthRoutine(_powerupHealthGO, _defaultSpawnPosition, 35f, 55f));
        StartCoroutine(SpawnBurstLaserRoutine(_powerupLaserBurstGO, _defaultSpawnPosition, 55f, 75f));
        StartCoroutine(SpawnAddShieldPowerup(_powerupAddShieldPowerGO, _defaultSpawnPosition, 25f, 35f));
        StartCoroutine(SpawnRemoveShieldsPowerup(_powerupRemoveShieldsGO, _defaultSpawnPosition, 20f, 45f));
        StartCoroutine(SpawnHomingMissilePowerupRoutine(_powerupHomingMissileGO, _defaultSpawnPosition, 25f, 45f));
    }

    private void SetWaveTimes()
    {
        _spawnWave2DelayTime = Time.time + 15f;
        StartCoroutine(StartNextWave(15f, 2));
        _spawnWave3DelayTime = Time.time + 45f;
        StartCoroutine(StartNextWave(45f, 3));
        _spawnWave4DelayTime = Time.time + 70f;
        StartCoroutine(StartNextWave(70f, 4));
        _spawnWave5DelayTime = Time.time + 85f;
        StartCoroutine(StartNextWave(85f, 5));
    }

    IEnumerator StartNextWave(float waitSeconds, int wave)
    {
        yield return new WaitForSeconds(waitSeconds);
        switch (wave)
        {
            case 2:
                _start2ndWaveActive = true;
                break;
            case 3:
                _start3rdWaveActive = true;
                break;
            case 4:
                _start4thWaveActive = true;
                break;
            case 5:
                _start5thWaveActive = true;
                break;
        }
    }

    IEnumerator HideNewWaveText()
    {

        yield return new WaitForSeconds(3f);
        _newWaveTextGO.SetActive(false);

    }

    IEnumerator SpawnHomingMissilePowerupRoutine(GameObject spawnedGameObject, Vector3 spawnPosition, float spawnInterval1, float spawnInterval2)
    {
        float randSpawnInterval = Random.Range(spawnInterval1, spawnInterval2);

        yield return new WaitForSeconds(3.0f);

        while (_isDead == false)
        {
            GameObject newEnemyGO = Instantiate(spawnedGameObject, spawnPosition, Quaternion.identity);
            newEnemyGO.transform.parent = _enemiesContainer.transform;
            yield return new WaitForSeconds(randSpawnInterval);
        }
    }

    IEnumerator SpawnEnemyRoutine(GameObject spawnedGameObject, Vector3 spawnPosition, float spawnInterval1, float spawnInterval2)
    {
        float randSpawnInterval = Random.Range(spawnInterval1, spawnInterval2);

        yield return new WaitForSeconds(3.0f);

        while (_isDead == false)
        {
            GameObject newEnemyGO = Instantiate(spawnedGameObject, spawnPosition, Quaternion.identity);
            newEnemyGO.transform.parent = _enemiesContainer.transform;
            yield return new WaitForSeconds(randSpawnInterval);
        }
    }

    IEnumerator SpawnAddShieldPowerup(GameObject spawnedGameObject, Vector3 spawnPosition, float spawnInterval1, float spawnInterval2)
    {
        float randSpawnInterval = Random.Range(spawnInterval1, spawnInterval2);

        yield return new WaitForSeconds(3.0f);

        //Spawn powerup according to the intervals passed in
        while (_isDead == false)
        {
            yield return new WaitForSeconds(randSpawnInterval);
            GameObject newPowerupGO = Instantiate(spawnedGameObject, spawnPosition, Quaternion.identity);
            newPowerupGO.transform.parent = _powerupsContainer.transform;
        }
    }

    IEnumerator SpawnRemoveShieldsPowerup(GameObject spawnedGameObject, Vector3 spawnPosition, float spawnInterval1, float spawnInterval2)
    {
        float randSpawnInterval = Random.Range(spawnInterval1, spawnInterval2);

        yield return new WaitForSeconds(3.0f);

        //Spawn powerup according to the intervals passed in
        while (_isDead == false)
        {
            yield return new WaitForSeconds(randSpawnInterval);
            GameObject newPowerupGO = Instantiate(spawnedGameObject, spawnPosition, Quaternion.identity);
            newPowerupGO.transform.parent = _powerupsContainer.transform;
        }
    }

    IEnumerator SpawnTripleShotRoutine(GameObject spawnedGameObject, Vector3 spawnPosition, float spawnInterval1, float spawnInterval2)
    {
        float randSpawnInterval = Random.Range(spawnInterval1, spawnInterval2);

        yield return new WaitForSeconds(3.0f);

        //Spawn powerup according to the intervals passed in
        while (_isDead == false)
        {
            yield return new WaitForSeconds(randSpawnInterval);
            GameObject newPowerupGO = Instantiate(spawnedGameObject, spawnPosition, Quaternion.identity);
            newPowerupGO.transform.parent = _powerupsContainer.transform;
        }
    }

    IEnumerator SpawnSpeedBoostRoutine(GameObject spawnedGameObject, Vector3 spawnPosition, float spawnInterval1, float spawnInterval2)
    {
        float randSpawnInterval = Random.Range(spawnInterval1, spawnInterval2);

        yield return new WaitForSeconds(3.0f);

        //Spawn powerup according to the intervals passed in
        while (_isDead == false)
        {
            yield return new WaitForSeconds(randSpawnInterval);
            GameObject newPowerupGO = Instantiate(spawnedGameObject, spawnPosition, Quaternion.identity);
            newPowerupGO.transform.parent = _powerupsContainer.transform;
        }
    }

    IEnumerator SpawnShieldRoutine(GameObject spawnedGameObject, Vector3 spawnPosition, float spawnInterval1, float spawnInterval2)
    {
        float randSpawnInterval = Random.Range(spawnInterval1, spawnInterval2);

        yield return new WaitForSeconds(3.0f);

        //Spawn powerup according to the intervals passed in
        while (_isDead == false)
        {
            yield return new WaitForSeconds(randSpawnInterval);
            GameObject newPowerupGO = Instantiate(spawnedGameObject, spawnPosition, Quaternion.identity);
            newPowerupGO.transform.parent = _powerupsContainer.transform;
        }
    }

    IEnumerator SpawnAmmoChargeRoutine(GameObject spawnedGameObject, Vector3 spawnPosition, float spawnInterval1, float spawnInterval2)
    {
        float randSpawnInterval = Random.Range(spawnInterval1, spawnInterval2);

        yield return new WaitForSeconds(3.0f);

        //Spawn powerup according to the intervals passed in
        while (_isDead == false)
        {
            yield return new WaitForSeconds(randSpawnInterval);
            GameObject newPowerupGO = Instantiate(spawnedGameObject, spawnPosition, Quaternion.identity);
            newPowerupGO.transform.parent = _powerupsContainer.transform;
        }
    }

    IEnumerator SpawnHealthRoutine(GameObject spawnedGameObject, Vector3 spawnPosition, float spawnInterval1, float spawnInterval2)
    {
        float randSpawnInterval = Random.Range(spawnInterval1, spawnInterval2);

        yield return new WaitForSeconds(3.0f);

        //Spawn powerup according to the intervals passed in
        while (_isDead == false)
        {
            yield return new WaitForSeconds(randSpawnInterval);
            GameObject newPowerupGO = Instantiate(spawnedGameObject, spawnPosition, Quaternion.identity);
            newPowerupGO.transform.parent = _powerupsContainer.transform;
        }
    }

    IEnumerator SpawnBurstLaserRoutine(GameObject spawnedGameObject, Vector3 spawnPosition, float spawnInterval1, float spawnInterval2)
    {
        float randSpawnInterval = Random.Range(spawnInterval1, spawnInterval2);

        yield return new WaitForSeconds(3.0f);

        //Spawn powerup according to the intervals passed in
        while (_isDead == false)
        {
            yield return new WaitForSeconds(randSpawnInterval);
            GameObject newPowerupGO = Instantiate(spawnedGameObject, spawnPosition, Quaternion.identity);
            newPowerupGO.transform.parent = _powerupsContainer.transform;
        }
    }

    private void DoNulLChecks()
    {
        if (_enemiesContainer == null)
        {
            Debug.Log("SpawnManager::DoNullChecks() - _enemiesContainer is null!");
        }

        if (_enemyGO == null)
        {
            Debug.Log("SpawnManager::DoNullChecks() - _enemyGO is null!");
        }

        if (_powerupsContainer == null)
        {
            Debug.Log("SpawnManager::DoNullChecks() - _powerupsContainer is null!");
        }

        if (_powerupTripleShotGO == null)
        {
            Debug.Log("SpawnManager::DoNullChecks() - _powerupTripleShotGO is null!");
        }

        if (_powerupSpeedBoostGO == null)
        {
            Debug.Log("SpawnManager::DoNullChecks() - _powerupSpeedBoostGO is null!");
        }

        if (_powerupShieldGO == null)
        {
            Debug.Log("SpawnManager::DoNullChecks() - _powerupShieldGO is null!");
        }

        if (_powerupAmmoRechargeGO == null)
        {
            Debug.Log("SpawnManager::DoNullChecks() - _powerupAmmoRechargeGO is null!");
        }

        if (_powerupHealthGO == null)
        {
            Debug.Log("SpawnManager::DoNullChecks() - _powerupHealthGO is null!");
        }

        if (_powerupLaserBurstGO == null)
        {
            Debug.Log("SpawnManager::DoNullChecks() - _powerupLaserBurstGO is null!");        
        }
        
        //if (_newWaveText == null)
        //{
        //    Debug.Log("SpawnManager::DoNullChecks() - _newWaveText is null!");
        //}

        //if(_newWaveText != null)
        //{
        //    _newWaveTextGO = _newWaveText.gameObject;
        //}

        if (_newWaveTextGO == null)
        {
            Debug.Log("SpawnManager::DoNullChecks() - _newWaveTextGO is null!");
        }
    }

    public void PlayerDied()
    {
        _isDead = true;
    }

}
