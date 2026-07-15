
using System.Collections.Generic;
using UnityEngine;
using AppUtils;
using Unity.VisualScripting;
using System.Linq;

[CreateAssetMenu(fileName = "NPCStore", menuName = "Store/NPC")]
public class NPCStore : ScriptableObject
{
    [Header("NPCS:")]
    [SerializeField] List<NPC> NPCs = new List<NPC>();

    [Header("Vehicles")]
    [SerializeField] List<Vehicle> vehicles = new List<Vehicle>();

    public NPC GetRandomNPC()
    {
        var distinctNPCs = NPCs.Distinct().ToList();
        if (distinctNPCs.Count == 0)
            return null;

        return distinctNPCs[Random.Range(0, distinctNPCs.Count)];
    }

    public Vehicle GetRandomCar() 
    {
        List<Vehicle> cars=  vehicles.Where(x => x.Type == BoardCharType.car).ToList();
        return cars.Random(); 
    }

    public Vehicle GetRandomTruck()
    {
        List<Vehicle> trucks = vehicles.Where(x => x.Type == BoardCharType.truck).ToList();
        return trucks.Random();
    }

}
