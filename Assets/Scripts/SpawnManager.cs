using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private bool _isDead = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine(_enemyGO, _defaultSpawnPosition, _enemySpawnInterval));
        StartCoroutine(SpawnTripleShotRoutine(_powerupTripleShotGO, _defaultSpawnPosition, 7f, 15f));
        StartCoroutine(SpawnSpeedBoostRoutine(_powerupSpeedBoostGO, _defaultSpawnPosition, 12f, 25f));
        StartCoroutine(SpawnShieldRoutine(_powerupShieldGO, _defaultSpawnPosition, 18f, 45f));
        StartCoroutine(SpawnAmmoChargeRoutine(_powerupAmmoRechargeGO, _defaultSpawnPosition, 15f, 25f));
        StartCoroutine(SpawnHealthRoutine(_powerupHealthGO, _defaultSpawnPosition, 30f, 55f));
    }

    IEnumerator SpawnEnemyRoutine(GameObject spawnedGameObject, Vector3 spawnPosition, int spawnInterval)
    {
        yield return new WaitForSeconds(3.0f);

        while (_isDead == false)
        {
            GameObject newEnemyGO = Instantiate(spawnedGameObject, spawnPosition, Quaternion.identity);
            newEnemyGO.transform.parent = _enemiesContainer.transform;
            yield return new WaitForSeconds(spawnInterval);
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

    public void PlayerDied()
    {
        _isDead = true;
    }

}
