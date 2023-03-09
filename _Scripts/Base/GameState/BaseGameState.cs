using Common;
using UnityEngine;

public class BaseGameState : MonoBehaviour
{
    private GameState game_state;
    public GameState gameState
    {
        get
        {
            if(game_state != GameConfig.gameState)
                game_state = GameConfig.gameState;
            return game_state;
        }
    }

    private float game_speed;
    public float gameSpeed
    {
        get
        {
            if (game_speed != GameConfig.gameSpeed)
            {
                game_speed = GameConfig.gameSpeed;
            }
            return game_speed;
        }
    }

    private void Awake()
    {
        Observer.Instance.AddObserver(ObserverKey.GameStateUpdated, UpdateGameState);
        Observer.Instance.AddObserver(ObserverKey.GameSpeedUpdated, UpdateGameSpeed);
        //Observer.Instance.AddObserver(ObserverKey.HeroSkill, ListenSkillHero);
    }

    private void UpdateGameState(object data)
    {
        game_state = (GameState)data;
        GameStateChanged();
    }

    protected virtual void GameStateChanged()
    {

    }

    protected virtual void SetTimeScale()
    {

    }

    private void UpdateGameSpeed(object data)
    {
        game_speed = (float)data;
        GameSpeedChanged();
    }

    protected virtual void GameSpeedChanged()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
    }

    protected virtual void OnEnable() { }

    protected virtual void OnDestroy()
    {
        Observer.Instance.RemoveObserver(ObserverKey.GameSpeedUpdated, UpdateGameSpeed);
        Observer.Instance.RemoveObserver(ObserverKey.GameStateUpdated, UpdateGameState);
    }
}
