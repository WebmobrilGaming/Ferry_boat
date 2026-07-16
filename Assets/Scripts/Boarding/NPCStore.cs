
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
