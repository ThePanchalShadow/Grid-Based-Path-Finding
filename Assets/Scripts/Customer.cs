using System.Threading.Tasks;
using UnityEngine;
using Pathfinding;

public class Customer : MonoBehaviour
{
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    public ColorType colorType;
    [SerializeField] private AIPath aiPath;
    [SerializeField] private Seeker seeker;
    [SerializeField] private Renderer indicatorRenderer;
    [SerializeField] internal GameObject obstacle;
    private bool canReachDestination;
    private bool onStand;
    private void Start()
    {
        aiPath.canMove = false;
        aiPath.destination = TargetBus.Instance.currentTarget.position;

        UpdateReachabilityVisual();
    }

    public void CustomerReachedDestination()
    {
        GridPathManager.Instance.RemoveThisCustomer(this);
    }

    private void OnMouseDown()
    {
        Debug.Log("CustomerOnMouseDown");
        if (!canReachDestination) return;

        obstacle.SetActive(canReachDestination);
        var bus = TargetBus.Instance; // Get the singleton instance

        if (bus)
        {
            if (bus.currentColor == colorType)
            {
                // Set destination to the current target of the bus
                SetDestination(bus.currentTarget, true);
            }
            else
            {
                // Assign the customer to a wait stand
               onStand = bus.AssignWaitStand(this);
            }
        }
        else
        {
            Debug.LogError("TargetBus instance not found!");
        }
    }

    public void SetDestination(Transform destination, bool canMove = false)
    {
        aiPath.destination = destination.position;
        aiPath.canMove = canMove;
    }

    public Task UpdateReachability()
    {
        if (onStand) return Task.CompletedTask;
        if (aiPath.destination == Vector3.zero) return Task.CompletedTask;

        obstacle.SetActive(false);
        AstarPath.active.Scan();
        
        // Snap to the nearest valid node
        var snappedStart = AstarPath.active.GetNearest(transform.position).position;
        var snappedEnd = AstarPath.active.GetNearest(aiPath.destination).position;

        // Debug positions
        Debug.Log($"Snapped Start Position: {snappedStart}, Snapped End Position: {snappedEnd}");

        // Get the corresponding nodes
        var startNode = AstarPath.active.GetNearest(transform.position).node;
        var endNode = AstarPath.active.GetNearest(aiPath.destination).node;

        if (startNode == null || endNode == null)
        {
            Debug.LogError("Start or End node is null!");
            return Task.CompletedTask;
        }

        Debug.Log($"Start Node Walkable: {startNode.Walkable}, End Node Walkable: {endNode.Walkable}");

        // Check path connectivity
        canReachDestination = PathUtilities.IsPathPossible(startNode, endNode);

        Debug.Log(canReachDestination
            ? "Path is possible!"
            : "Path is not possible.");
        
        obstacle.SetActive(true);
        AstarPath.active.Scan();

        UpdateReachabilityVisual();
        return Task.CompletedTask;
    }

    private void UpdateReachabilityVisual()
    {
        if (!indicatorRenderer) return;
        var colorToSet = canReachDestination ? Color.green : Color.red;
        SetEmissionColor(indicatorRenderer, colorToSet);
    }

    private static void SetEmissionColor(Renderer renderer, Color color)
    {
        // Ensure the material has an emission property
        var material = renderer.material;
        if (material.HasProperty(EmissionColor))
        {
            // Enable the emission keyword to make it visible
            material.EnableKeyword($"_EMISSION");

            // Set the emission color
            material.SetColor(EmissionColor, color * 10);
        }
        else
        {
            Debug.LogWarning("The material does not have an _EmissionColor property.");
        }
    }
}
