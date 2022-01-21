using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 _originalCameraPosition;
    [SerializeField]
    private float _shakeDuration = 0f;
    private float _shakeAmount;
    private bool _shakeCamera = false;


    // Start is called before the first frame update
    void Start()
    {
        _originalCameraPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
            if (_shakeDuration > 0)
            {
                transform.localPosition = _originalCameraPosition + Random.insideUnitSphere * _shakeAmount;
                _shakeDuration -= Time.deltaTime;
            } else
            {
            _shakeCamera = false;
            _shakeDuration = 0;
            transform.localPosition = _originalCameraPosition;
            }
    }

    public void ShakeCamera(float duration, float amount)
    {
        _shakeDuration = duration;
        _shakeAmount = amount;
        _shakeCamera = true;
    }
}
