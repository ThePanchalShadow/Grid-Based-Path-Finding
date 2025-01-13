using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.Serialization;

public class TargetBus : MonoBehaviour
{
    public static TargetBus Instance { get; private set; } // Singleton instance

    [FormerlySerializedAs("CurrentColor")] public ColorType currentColor; // The bus's current color
    [FormerlySerializedAs("CurrentTarget")] public Transform currentTarget; // The current target destination
    public List<Transform> listOfWaitStands = new(); // Available wait stands
    public List<Transform> occupiedWaitStands = new(); // Occupied wait stands

    [SerializeField] private List<Transform> customerPositions = new();
    private void Awake()
    {
        // Ensure only one instance exists
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple instances of TargetBus detected! Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    public bool AssignWaitStand(Customer customer)
    {
        if (listOfWaitStands.Count > 0)
        {
            // Take the first available wait stand
            var waitStand = listOfWaitStands[0];

            // Assign the customer to the wait stand
            customer.SetDestination(waitStand, true);
            customer.obstacle.SetActive(false);
            
            // Update the lists
            listOfWaitStands.Remove(waitStand);
            occupiedWaitStands.Add(waitStand);
            return true;
        }
        else
        {
            // Debug message if no space is left
            Debug.Log("No space left at the wait stands!");
            return false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"OnTriggerEnter {other.name}");
        
        if (!other.TryGetComponent(out Customer customer)) return;
        
        Debug.Log($"Customer {customer.name} entered");
        customer.CustomerReachedDestination();
        customer.obstacle.SetActive(false);
        
        var newPosition = customerPositions[0].position;
        customerPositions.RemoveAt(0);
        SetNewPositionOfCustomer(customer, newPosition);
    }

    private static void SetNewPositionOfCustomer(Customer customer, Vector3 newPosition)
    {
        if(customer.TryGetComponent(out AIPath aiPath)) aiPath.enabled = false;
        if(customer.TryGetComponent(out Seeker seeker)) seeker.enabled = false;
        if (customer.TryGetComponent(out Transform customerTransform)) customerTransform.position = newPosition;
        customer.enabled = false;
    }
}