using System.Collections;
using UnityEngine;
using WeChatWASM;
//定义一个枚举来建立一个简易状态机
public enum GameState
{
    Menu,//游戏菜单时状态
    ReadyGo,//游戏准备开始的倒计时状态
    Game,//正式游戏游玩儿状态
    GameOver//游戏结束时状态
}
public class GameManager : MonoBehaviour
{
    //简易单例
    public static GameManager Instance;
    public Animation GunAnimation;
    public bool SuperMode;//看广告后进入的超级模式
    private int _currentScore;
    private float _gameTime;
    private float _shootTimer;
    private VibrateShortOption vibrateShort = new VibrateShortOption() { type = "medium" };//微信让手机震动的模式：短震动，比较轻的
    private VibrateShortOption superVibrateShort = new VibrateShortOption() { type = "heavy" };//短震动，比较重的
    private void Awake()
    {
        Instance = this;
    }
    //创建一个简易状态机领域来处理逻辑
    private GameState _gameState;
    public GameState GameState//当游戏状态发生变化(进入)时，处理相应的逻辑
    {
        get => _gameState;
        set
        {
            _gameState = value;
            switch (_gameState)
            {
                case GameState.Menu:
                    SuperMode = false;
                    UIManager.Instance.EnterMenu();
                    //召唤菜单鸭
                    DuckManager.Instance.CreateMenuDuck();
                    break;
                case GameState.ReadyGo:
                    UIManager.Instance.EnterReadyGo();
                    AudioManager.Instance.PlayReadyGoClip();
                    //延迟等待动画播放完毕后进入游戏状态
                    Invoke(nameof(StartGame), ConfigManager.Instance.ReadyGoAnimationTime);//延迟两秒再激活StartGame函数
                    break;
                case GameState.Game:
                    _currentScore = 0;
                    _gameTime = ConfigManager.Instance.maxGameTime;
                    UIManager.Instance.EnterGame();
                    UIManager.Instance.UpdateScore(_currentScore);
                    DuckManager.Instance.EnterGame();
                    break;
                case GameState.GameOver:
                    DuckManager.Instance.StopGame();
                    UIManager.Instance.GameOver();
                    break;
            }
        }
    }
    /// <summary>
    /// Unity生命周期中的函数如果将它们设定为协程，那么协程会自动调用
    /// </summary>
    /// <returns></returns>
    IEnumerator Start()
    {
        AudioManager.Instance.PlayShowSceneClip();
        yield return new WaitForSeconds(1.5f);
        GameState = GameState.Menu;
    }
    void Update()
    {
        switch (GameState)//当游戏状态处于某个状态时每帧执行逻辑处
        {
            case GameState.Menu:
                DuckContraller menuDuck = RayCastDuck();
                if (menuDuck != null && !menuDuck.IsDead)
                {
                    GunShoot(menuDuck);
                    menuDuck.Die();
                    GameState = GameState.ReadyGo;
                }
                break;
            case GameState.ReadyGo:

                break;
            case GameState.Game:
                _shootTimer -= Time.deltaTime;
                _gameTime -= Time.deltaTime;
                if (_gameTime <= 0)
                {
                    _gameTime = 0;
                    UIManager.Instance.UpdateTime((int)_gameTime);
                    GameState = GameState.GameOver;
                    return;
                }
                UIManager.Instance.UpdateTime((int)_gameTime);
                float shootCD = SuperMode ? ConfigManager.Instance.superModeShootCD : ConfigManager.Instance.shootCD;
                UIManager.Instance.UpdateGunCD(_shootTimer / shootCD);
                if (_shootTimer <= 0 && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    _shootTimer = shootCD;
                    DuckContraller duck = RayCastDuck();
                    if (duck != null && !duck.IsDead)
                    {
                        _currentScore += duck.IsTargetDuck ? 5 : 1;
                        GunShoot(duck);
                        duck.Die();
                        UIManager.Instance.UpdateScore(_currentScore);
#if !UNITY_EDITOR//设定一个宏命令，微信SDK只有在非编辑器状态下才会调用
                        //屏幕震动（调用微信小程序文档的SDK接口）
                        if (SuperMode)
                        {
                            WX.VibrateShort(superVibrateShort);//短震动
                        }
                        else
                        {
                            WX.VibrateShort(vibrateShort);
                        }
#endif
                    }
                    else
                    {
                        if (!SuperMode)
                        {
                            AudioManager.Instance.PlayeUnHitDuckClip();//如果没有命中鸭子就发出嘲笑声
                            GunShoot(Input.GetTouch(0).position);
                        }
                    }
                }
                break;
            case GameState.GameOver:

                break;
        }
    }
    private void StartGame()
    {
        GameState = GameState.Game;
    }
    /// <summary>
    /// 射线检测鸭子，判断触碰是否触碰到鸭子并返回触碰到的鸭子
    /// </summary>
    /// <returns></returns>
    private DuckContraller RayCastDuck()
    {
        if (Input.touchCount > 0)//因为手机可以同时有几根手指触摸屏幕，所有有触摸的数量，如果触摸的数量大于0则表示手指点了
        {
            //获取第一个触摸点的Touch信息即可
            UnityEngine.Touch touch = Input.GetTouch(0);
            //Touch.phase：手指触摸到屏幕的状态
            /*
             TouchPhase.Began(一个手指触碰了屏幕)
             TouchPhase.Moved(一个手指在屏幕上移动)
             TouchPhase.Stationary(手指在触屏,但没有动)
             TouchPhase.Ended(一根手指从屏幕上移开。这是触摸的最后阶段)
             TouchPhase.Canceled(系统取消了触摸跟踪)
             */
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);//从手指触碰的位置的屏幕发射一根射线
                RaycastHit2D hitInfo = Physics2D.Raycast(ray.origin, Vector2.zero, Mathf.Infinity);//获取这根射线的碰撞信息(Mathf.Infinity表示正无穷大)
                if (hitInfo.collider != null && hitInfo.collider.TryGetComponent(out DuckContraller duckContraller))//说明碰到东西了
                {
                    return duckContraller;
                }
            }
        }
        return null;
    }
    private void GunShoot(Vector3 pos)
    {
        GunAnimation.transform.position = pos;
        GunAnimation.transform.localScale = new Vector3(pos.x >= 0 ? 1 : -1, 1, 1);
        GunAnimation.Play("Gun");
        AudioManager.Instance.PlayShotGunClip();
    }
    private void GunShoot(DuckContraller duckContraller)
    {
        GunShoot(duckContraller.transform.position + ConfigManager.Instance.GunOffset);
        AudioManager.Instance.PlayHitDuckClip();
    }
}
