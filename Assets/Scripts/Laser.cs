using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8;
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
        transform.Translate(_direction * _speed * Time.deltaTime, Space.World);
        //transform.Translate()

        if (transform.position.x > 20 || transform.position.x < -20 || transform.position.y > 20 || transform.position.y < -20) { Destroy(this.gameObject); }
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    public void SetDirection(Vector3 direction)
    {
        _direction = direction;
    }
}
