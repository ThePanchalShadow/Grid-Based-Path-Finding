using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinding;
using UnityEngine;

public class GridPathManager : MonoBehaviour
{

    [SerializeField] private List<Customer> customers = new();
    [SerializeField] private List<GameObject> grids = new();
GridGraph gridGraph;
    #region Singleton

    public static GridPathManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Debug.LogWarning("Multiple instances of GridPathManager detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    #endregion

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void UpdateCustomersPath()
    {
        _ = UpdateCustomersPathAsync();
    }

    private async Task UpdateCustomersPathAsync()
    {
        var count = 0;
        foreach (var customer in customers)
        {
            await customer.UpdateReachability();
             await Task.Delay(500);
            Debug.Log($"UpdateCustomersPathAsync {++count}");
        }
    }

    public void DisableGrids()
    {
        foreach (var grid in grids)
        {
            grid.SetActive(false);
        }
    }

    public void RemoveThisCustomer(Customer customer)
    {
        customers.Remove(customer);
        UpdateCustomersPath();
    }
}