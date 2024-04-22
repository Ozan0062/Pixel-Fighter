using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage
{
    // Start is called before the first frame update
    public void TakeDamage(int damage);
    public bool HasTakenDamage { get; set;}
}
