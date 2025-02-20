using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path_Contoller : MonoBehaviour
{
    [SerializeField] List<PlatForm> platForms_Preafbs;
    [SerializeField] List<PlatForm> PlatFormList;
    PlatForm previousBlock;
    float zPos=-694.52f;
    [SerializeField] internal int _X=0;

    #region SINGLETON

    public static Path_Contoller Instance;

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Instiate_Path();
    }

    void Instiate_Path()
    {
        for (int i = 0;i < 3;i++)
        {
            PlatForm block = Instantiate(platForms_Preafbs[i],transform);
            block.transform.position = new Vector3(0,0,zPos);
            PlatFormList.Add(block);
            zPos += block.length;
        }
        previousBlock = PlatFormList[0];
    }

    public IEnumerator SpwanNextPath(float wait)
    {
        yield return new WaitForSeconds(wait);

        previousBlock.gameObject.SetActive(false);
        previousBlock.transform.position = new Vector3(0,0,zPos);
        previousBlock.gameObject.SetActive(true);
        PlatFormList.RemoveAt(0);
        PlatFormList.Add(previousBlock);
        previousBlock = PlatFormList[0];
        zPos += previousBlock.length;
    }

}
