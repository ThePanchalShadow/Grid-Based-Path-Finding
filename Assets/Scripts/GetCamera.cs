using UnityEngine;
using UnityEngine.Animations;

public class GetCamera : MonoBehaviour
{
    [SerializeField] private AimConstraint aimConstraint;

    private void Start()
    {
        if (aimConstraint)
        {
            aimConstraint.AddSource(new ConstraintSource{sourceTransform = Camera.main?.transform, weight = 1});
        }
    }
}
