using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBurst : MonoBehaviour
{
    [SerializeField]
    private float _speed = 10f;
    [SerializeField]
    private Vector3 _direction = new Vector3(0, 1, 0);
    [SerializeField]
    private GameObject[] _laserGOs;

    // Start is called before the first frame update
    void Start()
    {
        DoNullChecks();
    }

    // Update is called once per frame
    void Update()
    {
        MoveAllLasers();
    }

    private void MoveAllLasers()
    {
        int counter = 0;

        foreach (GameObject currentGO in _laserGOs)
        {
            if (currentGO != null)
            {
                currentGO.transform.Translate(_direction * _speed * Time.deltaTime);

                if (currentGO.transform.position.y > 15)
                {
                    Destroy(transform.gameObject);
                }
            }
            counter += 1;
        }
    }

    private void DoNullChecks()
    {
        int counter = 0;

        if (_laserGOs != null)
        {
            foreach (GameObject currentGO in _laserGOs)
            {
                if (currentGO == null)
                {
                    Debug.Log("LaserBurst::DoNullChecks() - _laserGOs(" + counter + ") is null!");
                }
                counter += 1;
            }
        } else
        {
            Debug.Log("LaserBurst::DoNullChecks() - _laserGOs is null!");
        }
    }
}
