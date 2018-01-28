using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;

[RAINAction]
public class punchanim : RAINAction
{
    public override void Start(RAIN.Core.AI ai)
    {
        //ai.Body.GetComponent<Animation>().Play("punch");
        //ai.Body.GetComponent<Animator>().Play("walkUp");
        base.Start(ai);
        /*
         *      Aqui chama o GameOver
         */
        GameManager.Instance.GameOver();
    }

    public override ActionResult Execute(RAIN.Core.AI ai)
    {

        //Debug.Log("ai.Motor.MoveTarget punch:" + ai.Motor.MoveTarget + " this.Pos:" + ai.Body.transform.position);
        return ActionResult.SUCCESS;
    }

    public override void Stop(RAIN.Core.AI ai)
    {
        base.Stop(ai);
    }
}