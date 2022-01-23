using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    private Vector3 _direction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //translate laser up
        transform.Translate(_direction * _speed * Time.deltaTime);

        if (transform.position.y > 6.8 || transform.position.y < -5.5) { Destroy(this.gameObject); }
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }
}
