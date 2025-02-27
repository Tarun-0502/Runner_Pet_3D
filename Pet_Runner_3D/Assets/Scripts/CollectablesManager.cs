using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;

public class CollectablesManager : MonoBehaviour
{
    [SerializeField] GameObject CoinPrefab;
    [SerializeField] List<GameObject> coinsList;
    [SerializeField] GameObject coinsParentTransform;
    [SerializeField] TextMeshProUGUI coinsText;
    [SerializeField] internal GameObject player;
    [SerializeField] internal bool doubleCoinEnabled, coinMagnetEnabled = false;
    public int Coins = 0;

    #region SINGLETON

    public static CollectablesManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 100; i++)
        {
            GameObject coin = Instantiate(CoinPrefab, coinsParentTransform.transform);
            coinsList.Add(coin);
            coinsList[i].SetActive(false);
        }
        coinsParentTransform.transform.GetComponent<RotateChildern>().GetList(coinsList);
    }

    // Update is called once per frame
    void Update()
    {
        coinsText.text = Coins.ToString("0#");
    }


    #region COINS-FUNCTIONS

    int coinIndex = 0;

    public void PlaceCoin(Transform t, float xVal)
    {
        if (coinsList.Count > 0)
        {
            float roundX = xVal;
            float zPosition = t.position.z - 2.0f;
            GameObject coin = coinsList[coinIndex];
            PositionCoin(coin, roundX, zPosition);
        }

        if (coinIndex >= 99)
            coinIndex = 0;
    }

    private void PositionCoin(GameObject coin, float roundX, float zPosition)
    {
        coin.transform.localPosition = new Vector3( roundX, 0.75f, zPosition);
        coin.SetActive(true);
        coinIndex++;
    }

    #endregion

}
