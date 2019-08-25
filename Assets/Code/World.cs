using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{

    public static List<Color> colors = new List<Color>{Color.red, Color.green, Color.blue};
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public static Color GetRandomColor()
    {
        return colors[Random.Range(0, colors.Count)];
    }
    
    public static void Impact(Cube player, Cube target)
    {
        if(target.content.color == Color.black)
        {
            player.content.RemoveColored(target.content.rows * target.content.rows);
            target.Kill();

        }
        else if (colors.Contains(target.content.color))
        {
            player.content.AddColored(target.content.rows * target.content.rows, target.content.color);
            target.Kill();
        }
        UpdateInterface(player);

    }

    public static void IsGameOver(Cube player)
    {
        Debug.Log("IsGameOver - " + player.name);
        GameOver(player);
    }

    public static void GameOver(Cube player)
    {
        Interface.instance.GameLose();
    }

    public static void IsGameWin(Cube player)
    {
        Debug.Log("IsWin - " + player.name);
        GameWin(player);
    }

    public static void GameWin(Cube player)
    {
        Interface.instance.GameWin();
    }

    public static void UpdateInterface(Cube player)
    {
        Debug.Log("UpdateInterface");
        int colored = player.GetComponent<CubeContent>().GetCubesCount(colors);
        Interface.instance.SetStates(colored, player.content.content.Length);

    }

}
