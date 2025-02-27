using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Coin : MonoBehaviour
{
    public LayerMask layer;
    public Material coinMaterial;
    public MeshFilter coinMesh;
    float coinOffset = 0.65f;
    CollectablesManager cMgr;
    int coinMultiplier = 1;
    float DoubleCoinTime = 10.0f;
    float xVal = 0.0f;
    float DrawnCoinHeight;

    // Start is called before the first frame update
    void Start()
    {
        cMgr = CollectablesManager.Instance;
        CheckObstacles();
    }

    // Update is called once per frame
    void Update()
    {
        if (cMgr.doubleCoinEnabled==true)
        {
            DoubleCoins();
        }
        if (cMgr.coinMagnetEnabled == true)
        {
            Magnet();
        }
    }

    void CheckObstacles()
    {
        var v = Physics.OverlapSphere(transform.position, 2.0f, layer);
        if (v.Length > 0)
        {
            gameObject.SetActive(false);
        }
    }

    #region DOUBLE-COINS

    void DoubleCoins()
    {
        Vector3 _position = transform.position;
        Quaternion q = transform.rotation;
        q.y = -transform.rotation.y;
        Graphics.DrawMesh(coinMesh.mesh, _position + Vector3.up * coinOffset, q, coinMaterial, 0);
        StartCoroutine(DoubleCoinCoroutine());
    }
    IEnumerator DoubleCoinCoroutine()
    {
        coinMultiplier = 2;
        cMgr.doubleCoinEnabled = true;
        xVal = transform.position.x;
        Debug.Log("Double Coins Enumerator Started");
        yield return new WaitForSeconds(DoubleCoinTime);
        cMgr.doubleCoinEnabled = false;
        coinMultiplier = 1;
        Vector3 pos = transform.position;
        pos.x = xVal;
        transform.position = pos;
        Debug.Log("Double Coins Enumerator Ended");
    }

    #endregion

    #region MAGNET

    void Magnet()
    {
        GameObject playerPos = cMgr.player.gameObject;
        if (transform.position.z < (playerPos.transform.position.z + 15.0f) && transform.position.z > playerPos.transform.position.z + 1.0f)
        {
            coinOffset = Mathf.Lerp(coinOffset, 0.0f, 2.0f * Time.deltaTime);
            transform.DOMove(playerPos.transform.position, 0.35f).OnComplete(() => gameObject.SetActive(false));
        }
    }

    #endregion

}
