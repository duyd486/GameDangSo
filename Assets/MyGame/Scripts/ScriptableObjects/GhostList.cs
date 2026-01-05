using UnityEngine;

public class GhostList : ScriptableObject
{
    public GhostData[] ghosts;

    public GhostData GetGhostByName(string name)
    {
        foreach (GhostData ghost in ghosts)
        {
            if (ghost.name.Equals(name))
            {
                return ghost;
            }
        }
        return null;
    }
}
