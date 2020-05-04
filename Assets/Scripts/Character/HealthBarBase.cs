using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarBase : MonoBehaviour
{
    protected HealthManager hpMngr;
    protected Character origin;
    public virtual Character Origin
    {
        get => origin;
        set
        {
            if (origin != null)
                origin.HealthChanged -= ChangeFillAmount;

            origin = value;
            hpMngr = origin.GetComponent<HealthManager>();

            origin.HealthChanged += ChangeFillAmount;
        }
    }

    protected virtual void ChangeFillAmount(object sender, HealthChangedEventArgs e)
    {
        hpMngr.SpawnHitNumber();
    }
}
