using System.Collections;
using TMPro;
using UnityEngine;

public class StairsUp : MonoBehaviour
{
    private void Start()
    {
        manager = GameObject.FindGameObjectWithTag("InputUI").GetComponent<InputManagerScript>();
    }
    public InputManagerScript manager;
    public void OnPlayerEnter()
    {
        Debug.Log("Trigger enter player (UP)!");
        MapGenerator generator = GameObject.FindGameObjectWithTag("Map").GetComponent<MapGenerator>();
        int nextIndex = generator.GetCurrentLevelIndex() - 1;
        Debug.Log("Next index is: " + nextIndex);

        if (generator.HasLevel(nextIndex))
        {
            manager.ShowScreen(nextIndex);
            Level level = generator.GetLevel(nextIndex);
            generator.SetCurrentLevelIndex(nextIndex);
            generator.GenerateLevel(level);
            generator.SetPlayerPosition(level.endX, level.endY);
        }
        else
        {
            Debug.LogWarning("Level neexistuje!");
        }

    }


}
