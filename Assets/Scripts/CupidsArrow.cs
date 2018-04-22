using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupidsArrow : Projectile {
    public override void Hit() {
        Date date = GameObject.FindGameObjectWithTag("Date").GetComponent<Date>();
        date.dateProgress -= penalty;
        GameObject.FindGameObjectWithTag("Date").GetComponent<Date>().PlaySound(1);
        Destroy(this.gameObject);
    }
}