using UnityEngine;


    Animation Animation;

	// Use this for initialization
        InteractionType = E_InteractionObjects.UseLever;
	}
        OnInteractionStart();

        if (DisableAfterUse)
            InteractionObjectUsable = false;

        return Animation[s].length;
        OnInteractionEnd();


    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }

    public override void Restart()
    {
        State = E_State.E_OFF;
        gameObject.SetActiveRecursively(true);
        Animation.Stop();
        AnimON.SampleAnimation(InteractionObject, 0);

        base.Restart();
    }
