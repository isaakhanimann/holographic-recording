using Microsoft.MixedReality.Toolkit.Utilities.Solvers;

public class InFront : Solver
{

    public override void SolverUpdate()
    {
        if (SolverHandler != null && SolverHandler.TransformTarget != null)
        {
            var target = SolverHandler.TransformTarget;
            GoalPosition = target.position + target.forward * 0.8f;
            GoalRotation = target.rotation;
        }
    }
}