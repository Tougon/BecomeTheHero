using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : EntityController
{
    // Start is called before the first frame update
    void Start()
    {
        if(current != null)
            EventManager.Instance.RaiseEntityControllerEvent(EventConstants.ON_ENEMY_INITIALIZE, this);
    }


    protected override void OnDeath()
    {
        EventManager.Instance.RaiseEntityControllerEvent(EventConstants.ON_ENEMY_DEFEAT, this);
    }
}
