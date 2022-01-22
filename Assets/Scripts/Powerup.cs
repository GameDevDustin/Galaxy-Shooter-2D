using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    private Vector3 _startingPosition = new Vector3(0, 8, 0);
    private Vector3 _powerupDirection = new Vector3(0, -1, 0);
    [SerializeField]
    private float _powerupSpeed = 3f;
    [SerializeField]
    private AudioClip _powerupAudioClip;
    private AudioSource _powerupAudioSource;

    //Powerup ID values: 0 - Triple Shot | 1 - Speed | 2 - Shield | 3 - Health | 4 - Burst Laser | 5 - Ammo Recharge
    // 6 - Add Shield Power | 7 - Remove Shields
    [SerializeField]
    private int _powerupID;

    // Start is called before the first frame update
    void Start()
    {
        if (_powerupAudioClip == null)
        {
            Debug.Log("Powerup:: Start() - _powerupAudioClip is null!");
        }

        _powerupAudioSource = transform.GetComponent<AudioSource>();

        if (_powerupAudioSource == null)
        {
            Debug.Log("Powerup:: Start() - _powerupAudioSource is null!");
        }

        //assign starting position immediately to ensure enemy spawns off screen
        transform.position = _startingPosition;

        //X values for powerup position must be between -8.999 and 8.999
        float randomX = (float)Random.Range(-8.5f, 8.5f);

        //Reset spawn position at random x position above top of screen
        transform.position = _startingPosition + new Vector3(randomX, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        //move down at 3 meters per second
        transform.Translate(_powerupDirection * _powerupSpeed * Time.deltaTime);

        //if out of lower bounds
        if (transform.position.y < -6f)
        {
            //destroy once off screen
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //if hits player
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                //Choose action based on PowerupID
                switch (_powerupID)
                {
                    case 0:  //Triple Shot
                        player.TripleShotActive();
                        break;
                    case 1:  //Speed
                        player.SpeedBoostPowerupActive();
                        break;
                    case 2:  //Shield
                        player.ShieldActive(3);
                        break;
                    case 3:  //Health
                        player.AddLives(1);
                        break;
                    case 4:  //Burst Laser
                        player.LaserBurstActive();
                        transform.gameObject.SetActive(false);
                        break;
                    case 5:  //Ammo recharge
                        player.RefillAmmoCharge();
                        break;
                    case 6:  //Add Shield Power
                        player.AddShieldPower(1);
                        break;
                    case 7:  //Remove shields
                        player.ShieldDeactivate();
                        break;
                    default:  //powerupID value is unexpected
                        Debug.Log("powerupID value is unexpected!");
                        break;
                }           
            } 
            else
            {
                Debug.Log("Powerup script did not find Player component on collision other");
            }

            _powerupAudioSource.clip = _powerupAudioClip;
            _powerupAudioSource.Play();

            if (_powerupID < 4)
            {
                transform.GetComponent<SpriteRenderer>().enabled = false;
            } else
            {
                SpriteRenderer[] powerupImages = transform.GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer currentSpriteRenderer in powerupImages)
                {
                    currentSpriteRenderer.enabled = false;
                } 
            }
            
            Destroy(this.gameObject, 1f);
        }
    }
}
