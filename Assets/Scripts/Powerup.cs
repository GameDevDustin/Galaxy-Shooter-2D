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
    [SerializeField]
    private GameObject _playerGO;
    private Vector3 _towardsPlayerDirection;
    private bool _disablePickup = false;

    //Powerup ID values: 0 - Triple Shot | 1 - Speed | 2 - Shield | 3 - Health | 4 - Burst Laser | 5 - Ammo Recharge
    // 6 - Add Shield Power | 7 - Remove Shields | 8 - Homing Missile
    [SerializeField]
    private int _powerupID;

    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("Player") != null)
        {
            _playerGO = GameObject.Find("Player");
        }
        else
        {
            Debug.Log("Powerup:: Start() - _playerGO is null!");
        }

        if (_powerupAudioClip == null)
        {
            Debug.Log("Powerup:: Start() - _powerupAudioClip is null!");
        }

        _powerupAudioSource = transform.GetComponent<AudioSource>();

        if (_powerupAudioSource == null)
        {
            Debug.Log("Powerup:: Start() - _powerupAudioSource is null!");
        }

        //assign starting position immediately to ensure powerup spawns off screen
        transform.position = _startingPosition;

        //X values for powerup position must be between -8.999 and 8.999
        float randomX = (float)Random.Range(-8.5f, 8.5f);

        //Reset spawn position at random x position above top of screen
        transform.position = _startingPosition + new Vector3(randomX, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        float moveHorizontal;
        float moveVertical;

        //Use easy collect for powerups
        if (Input.GetKey(KeyCode.C))
        {
            //Determine the direction towards player
            if (transform.position.x < _playerGO.transform.position.x)
            {
                //Move to the right
                moveHorizontal = 1;
            }
            else
            {
                //Move to the left
                moveHorizontal = -1;
            }

            if (transform.position.y < _playerGO.transform.position.y)
            {
                //move up
                moveVertical = 1;
            }
            else
            {
                //move down
                moveVertical = -1;
            }

            _towardsPlayerDirection = new Vector3(moveHorizontal, moveVertical, 0);

            //Move towards player game object
            transform.Translate(_towardsPlayerDirection * (_powerupSpeed - 1) * Time.deltaTime);
        } else
        {
            //move down
            transform.Translate(_powerupDirection * _powerupSpeed * Time.deltaTime);
        }

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
            if (_disablePickup == false)
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
                        case 8: //Homing Missile
                            player.AddHomingMissile();
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

                SpriteRenderer[] powerupImages = transform.GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer currentSpriteRenderer in powerupImages)
                {
                    currentSpriteRenderer.enabled = false;
                }

                _disablePickup = true;
                Destroy(this.gameObject, 1f);
            }
        }
    }
}
