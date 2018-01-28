using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;

[RAINAction]
public class idleanim : RAINAction
{
    public override void Start(RAIN.Core.AI ai)
    {
       // ai.Body.GetComponentInChildren<Animator>().Play("idle");
        base.Start(ai);
    }

    public override ActionResult Execute(RAIN.Core.AI ai)
    {

        //Debug.Log("ai.Motor.MoveTarget idle:" + ai.Motor.MoveTarget + " this.Pos:" + ai.Body.transform.position);
        return ActionResult.SUCCESS;
    }

    public override void Stop(RAIN.Core.AI ai)
    {
        base.Stop(ai);
    }
}