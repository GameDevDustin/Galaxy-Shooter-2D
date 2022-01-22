using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private Vector3 _rotationDirection = new Vector3(0, 0, 1f);
    [SerializeField]
    private float _rotationSpeed = 19f;
    [SerializeField]
    private GameObject _explosionPrefabGO;
    private SpawnManager _spawnManager;
    private bool _asteroidDestroyed = false;


    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(_rotationDirection * _rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
            if (other.gameObject.tag == "Laser" && _asteroidDestroyed != true)
            {
                Instantiate(_explosionPrefabGO, transform.position, Quaternion.identity);
                Destroy(other.gameObject);
                AsteroidDestroyed();
            }
        
    }

    private void AsteroidDestroyed()
    {
        _asteroidDestroyed = true;

        if (_spawnManager != null)
        {
            _spawnManager.StartSpawningWave1();
        }
        
        Destroy(this.gameObject, 0.25f);
    }
}
