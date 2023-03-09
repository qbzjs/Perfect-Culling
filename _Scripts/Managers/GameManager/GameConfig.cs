using Common;
using UnityEngine;

public class GameConfig : MonoBehaviour
{



    private static GameState game_state = GameState.Pause;
    public static GameState gameState
    {
        set
        {
            //if (TutorialManger.instance != null)
            //    if (TutorialManger.instance.currentState != TutorialState.None) return;
            game_state = value;
            Observer.Instance.Notify(ObserverKey.GameStateUpdated, value);
        }
        get
        {
            return game_state;
        }
    }

    private static float game_speed_root = 1;
    public static float gameSpeedRoot
    {
        set
        {
            game_speed_root = value;
            // Observer.Instance.Notify(ObserverKey.GameSpeedUpdated, value);
        }
        get
        {
            return game_speed_root;
        }
    }

    private static float game_speed = 1;
    public static float gameSpeed
    {
        set
        {
            game_speed = value;
            Observer.Instance.Notify(ObserverKey.GameSpeedUpdated, value);
        }
        get
        {
            return game_speed;
        }
    }

    private static bool game_start;
    public static bool gameStart
    {
        set
        {
            game_start = value;
            Observer.Instance.Notify(ObserverKey.StartGame, value);
        }
        get
        {
            return game_start;
        }
    }

    private static bool game_block_input = false;
    public static bool gameBlockInput
    {
        set
        {
            game_block_input = value;
            Observer.Instance.Notify(ObserverKey.GameBlockInput, value);
            if (game_player)
            {
                Observer.Instance.Notify(ObserverKey.GameBlockKeyboard, value);
            }
        }
        get
        {
            return game_block_input;
        }
    }
    public static bool game_player = false;
}
