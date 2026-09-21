
using AppUtils;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCStore", menuName = "Store/NPC")]
public class NPCStore : ScriptableObject
{
    [Header("NPCS:")]
    [SerializeField] List<NPC> NPCs = new List<NPC>();

    [Header("Vehicles")]
    [SerializeField] List<Vehicle> vehicles = new List<Vehicle>();


    private List<NPC> _npcBag = new();

    public NPC GetRandomNPC()
    {
        if (NPCs == null || NPCs.Count == 0)
            return null;

        // Refill the bag when it's empty
        if (_npcBag.Count == 0)
        {
            _npcBag = new List<NPC>(NPCs);

            // Shuffle the bag
            for (int i = 0; i < _npcBag.Count; i++)
            {
                int randomIndex = Random.Range(i, _npcBag.Count);
                (_npcBag[i], _npcBag[randomIndex]) = (_npcBag[randomIndex], _npcBag[i]);
            }
        }

        NPC npc = _npcBag[0];
        _npcBag.RemoveAt(0);

        return npc;
    }

    private int lastCarIndex;

    public Vehicle GetRandomCar() 
    {
        List<Vehicle> cars = vehicles
         .Where(x => x.Type == BoardCharType.car)
         .ToList();

        if (cars.Count == 0)
            return null;

        Debug.Log($"Car index: {lastCarIndex}");

        if (lastCarIndex >= cars.Count)
            lastCarIndex = 0;

        Vehicle randomCar = cars[lastCarIndex];

       lastCarIndex ++;

        return randomCar;
    }

    private Vehicle lastTruck;
    public Vehicle GetRandomTruck()
    {
        List<Vehicle> trucks = vehicles
         .Where(x => x.Type == BoardCharType.truck)
         .ToList();

        if (trucks.Count == 0)
            return null;

        // If only one car exists, return it
        if (trucks.Count == 1)
            return trucks[0];

        // Remove the previously selected car
        if (lastTruck != null)
            trucks.Remove(lastTruck);

        Vehicle randomCar = trucks.Random();

        lastTruck = randomCar;

        return randomCar;
    }

}
