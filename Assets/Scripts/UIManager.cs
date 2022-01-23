using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _scoreText;
    [SerializeField]
    private TMP_Sprite[] _livesSprites;
    [SerializeField]
    private Image _livesDisplayImg;
    [SerializeField]
    private TMP_Text _gameOverText;
    [SerializeField]
    private TMP_Text _restartGameText;
    private GameObject _gameOverGO;
    private GameObject _restartGO;
    private bool _gameOverStatus;

    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = "Score: 0";
        if (_gameOverText != null)
        {
            _gameOverGO = _gameOverText.gameObject;
        }
        else
        {
            Debug.Log("_gameOverText is null in UIManager script!");
        }
        if (_restartGameText != null)
        {
            _restartGO = _restartGameText.gameObject;
        } else
        {
            Debug.Log("_restartGameText is null in UIManager script!");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdatePlayerScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore;
    }

    public void UpdateNumOfLivesDisplay(int playerLives)
    {
        if (playerLives != -1)
        {
            _livesDisplayImg.sprite = _livesSprites[playerLives].sprite;
        }
    }

    public void ShowGameOver()
    {
        _gameOverStatus = true;
        if (_gameOverGO != null)
        {
            _gameOverGO.SetActive(true);
            StartCoroutine(FlickerGameOverTextRoutine());      
        }
        else
        {
            Debug.Log("_gameOverGO game object not found in UIManager script!");
        }

        if (_restartGO != null)
        {
            _restartGO.SetActive(true);
        } else
        {
            Debug.Log("_restartGO game object in UIManager script is null!");
        }
    }

    IEnumerator FlickerGameOverTextRoutine()
    {
        while (_gameOverStatus == true)
        {
            _gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }
}
