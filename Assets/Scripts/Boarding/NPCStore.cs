
using System.Collections.Generic;
using UnityEngine;
using AppUtils;

[CreateAssetMenu(fileName = "NPCStore",menuName = "Store/NPC")]
public class NPCStore : ScriptableObject
{
    [Header("NPCS:")]
    [SerializeField] List<NPC> NPCs = new List<NPC>();


    public NPC GetRandomNPC()
    {
        return NPCs.Random();
    }
}
