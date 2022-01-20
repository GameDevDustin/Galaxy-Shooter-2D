using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagedFire : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateDamage()
    {
        transform.gameObject.SetActive(true);
    }

    public void DeactivateDamage()
    {
        transform.gameObject.SetActive(false);
    }
}
