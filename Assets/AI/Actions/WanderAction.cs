using UnityEngine;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Navigation;
using RAIN.Navigation.Graph;

[RAINAction]
public class WanderAction : RAINAction
{
    private static float _time = 0f;
    public override void Start(RAIN.Core.AI ai)
    {
        base.Start(ai);
        _time += Time.time;
    }

    public override ActionResult Execute(RAIN.Core.AI ai)
    {
        var loc = Vector3.zero;

        var found = new List<RAINNavigationGraph>();

        do
        {
            loc = new Vector3(ai.Kinematic.Position.x + Random.Range(-8f, 8f),
                                ai.Kinematic.Position.y,
                                ai.Kinematic.Position.z + Random.Range(-8f, 8f));

            found = NavigationManager.Instance.GraphsForPoints(
                ai.Kinematic.Position, 
                loc, 
                ai.Motor.MaxHeightOffset, 
                NavigationManager.GraphType.Navmesh, 
                ((BasicNavigator)ai.Navigator).GraphTags);

        } while ((Vector3.Distance(ai.Kinematic.Position, loc) < 2f) || (found.Count == 0));

        ai.WorkingMemory.SetItem<Vector3>("varMoveTo", loc);

        if (_time>500f)
        {
            ai.WorkingMemory.SetItem("isSearching", false);
            _time = 0f;
        }

        return ActionResult.SUCCESS;
    }

    public override void Stop(RAIN.Core.AI ai)
    {
        base.Stop(ai);
    }
}