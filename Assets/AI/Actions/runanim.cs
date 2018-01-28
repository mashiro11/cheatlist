using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;

[RAINAction]
public class runanim : RAINAction
{
    public override void Start(RAIN.Core.AI ai)
    {

        base.Start(ai);
    }

    public override ActionResult Execute(RAIN.Core.AI ai)
    {

       // Debug.Log("ai.Motor.MoveTarget RUN:" + ai.Motor.MoveTarget + " this.Pos:"+ ai.Body.transform.position);

        //ai.Body.GetComponentInChildren<Animator>().Play("walkUp");

        return ActionResult.SUCCESS;
    }

    public override void Stop(RAIN.Core.AI ai)
    {
        base.Stop(ai);
    }
}