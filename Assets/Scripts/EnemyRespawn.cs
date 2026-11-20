using UnityEngine;
using System.Collections;


public class EnemyRespawn : MonoBehaviour
{
    public GameObject[] parts;
    public float respawnTime;

    public int partsRemaining = 3;
    public void BreakPart()
    {
        partsRemaining -= 1;
        print("part broken");
        if (partsRemaining <= 0)
        {
            StartCoroutine(Respawn());
            partsRemaining = parts.Length;
        }
    }

    IEnumerator Respawn()
    {
        print("Respawn Initiated");
        yield return new WaitForSeconds(respawnTime);
        for (int i = 0; i < parts.Length; i++)
        {
            parts[i].GetComponent<EnemyShooter>().Respawn();
        }
    }
}
