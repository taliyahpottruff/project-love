using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DateKiller : Projectile {
    public override void Hit() {
        Date date = GameObject.FindGameObjectWithTag("Date").GetComponent<Date>();
        date.dateProgress -= penalty;
        GameObject.FindGameObjectWithTag("Date").GetComponent<Date>().PlaySound(0);
        Destroy(this.gameObject);
    }
}
