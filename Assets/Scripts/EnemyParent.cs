using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyParent : MonoBehaviour
{
    public float respawnTime;
    public float shootInterval = 1f;
    public int partsRemaining = 3;
    private bool isDead = false;
    private Dictionary<part, EnemyShooter> parts = new Dictionary<part, EnemyShooter>();

    public enum part
    {
        LEFTWING,
        CENTER,
        RIGHTWING
    }

    private bool leftNext = true;

    void Start()
    {
        // populate parts dictionary with part + object combos
        foreach (Transform child in transform)
        {
            EnemyShooter shooter = child.GetComponent<EnemyShooter>();
            if (shooter != null)
            {
                parts.Add(shooter.part, shooter);
            }
        }

        StartCoroutine(ShootBalls());
    }

    // called by parts of the ship when broken
    public void BreakPart()
    {
        partsRemaining -= 1;
        print("part broken");
        if (partsRemaining <= 0)
        {
            StartCoroutine(Respawn());
            partsRemaining = parts.Count;
            isDead = true;
        }
    }

    // can only be called by the center part of the ship, breaks all other parts
    public void BreakShip()
    {
        if (!parts[part.LEFTWING].isDead)
        {
            parts[part.LEFTWING].Die();
        }
        if (!parts[part.RIGHTWING].isDead)
        {
            parts[part.RIGHTWING].Die();
        }
    }

    
    IEnumerator ShootBalls()
    {
        while (true)
        {
            // if its dead check again next frame
            if (isDead)
            {
                yield return null;
                continue;
            }
            // Wait before shooting another ball
            yield return new WaitForSeconds(shootInterval);

            bool leftAlive = !parts[part.LEFTWING].isDead;
            bool rightAlive = !parts[part.RIGHTWING].isDead;

            if (leftAlive && rightAlive)
            {
                if (leftNext) {
                    parts[part.LEFTWING].ShootBall();
                    leftNext = !leftNext;
                }
                else
                {
                    parts[part.RIGHTWING].ShootBall();
                    leftNext = !leftNext;
                }
            }
            else if (leftAlive)
            {
                parts[part.LEFTWING].ShootBall();
            }
            else if (rightAlive)
            {
                parts[part.RIGHTWING].ShootBall();
            }
            else
            {
                parts[part.CENTER].ShootBall();
            }
        }
    }

    IEnumerator Respawn()
    {
        print("Respawn Initiated");
        yield return new WaitForSeconds(respawnTime);
        // iterate through the enum using it to lookup parts in the parts dict
        foreach (part p in Enum.GetValues(typeof(part)))
        {
            parts[p].Respawn();
        }
        isDead = false;
    }
}
