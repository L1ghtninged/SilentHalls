
using UnityEngine;

public class StairsDown : MonoBehaviour
{
    private void Start()
    {
        manager = GameObject.FindGameObjectWithTag("InputUI").GetComponent<InputManagerScript>();
    }
    public InputManagerScript manager;
    public void OnPlayerEnter()
    {
        Debug.Log("Trigger enter player (DOWN)!");
        MapGenerator generator = GameObject.FindGameObjectWithTag("Map").GetComponent<MapGenerator>();
        int nextIndex = generator.GetCurrentLevelIndex() + 1;
        Debug.Log("Next index is: " + nextIndex);

        if (generator.HasLevel(nextIndex))
        {
            manager.ShowScreen(nextIndex);
            Level level = generator.GetLevel(nextIndex);
            generator.SetCurrentLevelIndex(nextIndex);
            generator.GenerateLevel(level);
            generator.SetPlayerPosition(level.startX, level.startY);
        }
        else
        {
            manager.ShowScreen(0);
        }
    }
    
}
