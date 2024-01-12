using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckManager : MonoBehaviour
{
    public static DuckManager Instance;
    public GameObject DuckPrefab;
    //创建一个简易对象池
    private Stack<DuckContraller> DuckPool = new Stack<DuckContraller>();//对象池
    private List<DuckContraller> _currentDuckList = new List<DuckContraller>();//获取当前场景中已经存在的鸭子
    //因为该游戏我们需要清场功能，在一关结束之后需要清除掉场景中的所有鸭子，所有我们需要一个当前场景鸭的数组
    private void Awake()
    {
        Instance = this;
    }
    public void CreateMenuDuck()
    {
        DuckContraller duckContraller = GetDuck();
        DuckConfig config = ConfigManager.Instance.GetRandomDuckConfig();//获取一个随机的鸭子配置
        duckContraller.InitMenuDuck(config, ConfigManager.Instance.MenuDuckPosition);
        _currentDuckList.Add(duckContraller);//激活一个新鸭子到场景中就把他装入到当前场景鸭列表中
    }
    private DuckContraller GetDuck()
    {
        if (!DuckPool.TryPop(out DuckContraller duckContraller))//先判断池子里有没有鸭子，有的话才能从池子里面拿
        {
            duckContraller = Instantiate(DuckPrefab).GetComponent<DuckContraller>();//池子里没有的话就复制一个新的预制体鸭子返回
        }
        duckContraller.gameObject.SetActive(true);
        return duckContraller;
    }
    public void RecycleDuck(DuckContraller duckContraller)
    {
        //隐藏起来放到池子里面去，从当前鸭子列表中移除
        duckContraller.gameObject.SetActive(false);
        DuckPool.Push(duckContraller);
        _currentDuckList.Remove(duckContraller);
    }
    /// <summary>
    /// 回收所有鸭子到对象池
    /// </summary>
    public void CleanAllDuck()
    {
        for (int i = _currentDuckList.Count - 1; i >= 0; i--)
        {
            RecycleDuck(_currentDuckList[i]);
        }
    }
    public void EnterGame()
    {
        StartCoroutine(SpawnGameDuckEveryInterval());
        StartCoroutine(SpawnGameDuckAvoidZero());
    }
    /// <summary>
    /// 避免鸭子为0个的协程
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnGameDuckAvoidZero()
    {
        while (true)
        {
            yield return null;
            if (GameManager.Instance.SuperMode && _currentDuckList.Count <= 3)//超级模式下，场上一定要有三只鸭子以上
            {
                CreateGameDuck();
            }
            else if (_currentDuckList.Count == 0)
            {
                CreateGameDuck();
            }
        }
    }
    IEnumerator SpawnGameDuckEveryInterval()
    {
        float currTime = 0;
        float spawnDuckInterval = GameManager.Instance.SuperMode ? ConfigManager.Instance.SuperModeDuckInterval : ConfigManager.Instance.DuckInterval;
        WaitForSeconds waitForSecond = new WaitForSeconds(spawnDuckInterval);
        while (true)
        {
            yield return waitForSecond;
            currTime += spawnDuckInterval;
            if (_currentDuckList.Count < 15)
            {
                float randomValue = UnityEngine.Random.Range(0, 1f);
                if (randomValue < ConfigManager.Instance.spawnCurve.Evaluate(currTime / ConfigManager.Instance.maxGameTime))
                {
                    CreateGameDuck();
                }
            }
        }
    }
    private void CreateGameDuck()
    {
        DuckSpawnInfo spawnInfo = ConfigManager.Instance.GetRandomDuckSpawnInfo();
        DuckContraller controller = GetDuck();
        controller.InitGameDuck(spawnInfo);
        _currentDuckList.Add(controller);
    }
    public void StopGame()
    {
        StopAllCoroutines();//关闭所有线程
        CleanAllDuck();
    }
}
