using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attack
{
    public int vertKb;
    public int horizKb;
    public int damage;
    public int multi;

    public Attack(int vertKb, int horizKb, int damage, int multi = 0)
    {
        this.vertKb = vertKb;
        this.horizKb = horizKb;
        this.damage = damage;
        this.multi = multi;
    }
}
