using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DuckContraller : MonoBehaviour
{
    public Animation Animation;
    public PolygonCollider2D PolygonCollider;
    public SortingGroup SortingGroup;
    public Transform DuckTargetIcon;
    public SpriteRenderer DuckSpriteRenderer;//鸭子精灵渲染器
    public bool IsDead = false;
    private List<Vector2> _physicsShape = new List<Vector2>();
    private bool _isGameDuck;
    /// <summary>
    /// 通用鸭的初始化
    /// </summary>
    /// <param name="duckConfig">鸭子的配置数据</param>
    private void Init(DuckConfig duckConfig)
    {
        IsDead = false;
        DuckSpriteRenderer.sprite = duckConfig.Sprite;
        DuckSpriteRenderer.transform.localRotation = Quaternion.identity;
        SetColliderShape(DuckSpriteRenderer.sprite);
    }
    /// <summary>
    /// 设置碰撞体形状
    /// </summary>
    /// <param name="sprite">精灵图片</param>
    private void SetColliderShape(Sprite sprite)
    {
        sprite.GetPhysicsShape(0, _physicsShape);//得到物理形状就是得到它的所有坐标点，因此需要一个List来放所有坐标点
        PolygonCollider.SetPath(0, _physicsShape);//通过获得的坐标点来设置碰撞体形状
    }
    public void Die()
    {
        IsDead = true;
        Animation.Play("Hit");
        //因为场景开始时有一个鸭子向上的动画协程，如果玩家手速很快在鸭子向上动画协程未播放完毕时点击开始
        //那么又会立即开始鸭子向下的动画协程，所有安全起见在播放鸭子向下动画协程时先关闭所有协程
        StopAllCoroutines();
        StartCoroutine(DoExit());
    }
    //鸭子退场动画
    private IEnumerator DoExit()
    {
        AudioManager.Instance.PlayeDuckGoBackClip();
        float targetPosY = transform.position.y - 7;
        while (transform.position.y > targetPosY)
        {
            transform.position += new Vector3(0, -Time.deltaTime * ConfigManager.Instance.DuckMoveSpeed, 0);
            yield return null;
        }
        DuckManager.Instance.RecycleDuck(this);
    }
    #region Menu
    /// <summary>
    /// 菜单鸭的初始化
    /// </summary>
    /// <param name="config">鸭子的配置数据</param>
    /// <param name="targetPos">鸭子的Y轴坐标位置</param>
    public void InitMenuDuck(DuckConfig config, Vector2 targetPos)
    {
        AudioManager.Instance.PlayeMenuDuckClip();
        _isGameDuck = false;
        IsTargetDuck = false;
        Init(config);
        transform.position = targetPos + new Vector2(0, -5.0f);//先将鸭子降低到地下，之后用协程的方式回归正常位置
        DuckTargetIcon.gameObject.SetActive(false);
        SortingGroup.sortingLayerName = "Duck2";
        StartCoroutine(MoveToMenuPosition(targetPos.y));
    }
    /// <summary>
    /// 协程：鸭子移动到地面动画
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveToMenuPosition(float targetPosY)
    {
        while (transform.position.y < targetPosY)
        {
            transform.position += new Vector3(0, Time.deltaTime * ConfigManager.Instance.DuckMoveSpeed, 0);
            yield return null;
        }
        AudioManager.Instance.PlayeMenuDuckReadyClip();
        transform.position = new Vector3(transform.position.x, targetPosY, 0);
    }
    #endregion
    #region Game
    public bool IsTargetDuck;
    private bool _isLeft;
    private bool _isUp;
    private Vector2 _spawnPoint;
    private float _targetPosX;
    private float _targetPosY;

    public float TargetPosX
    {
        get => _targetPosX;
        set
        {
            _targetPosX = value;
            _isLeft = transform.position.x > TargetPosX;
            transform.localScale = new Vector3(_isLeft ? -1 : 1, 1, 1);
        }
    }
    public float TargetPosY
    {
        get => _targetPosY;
        set
        {
            _targetPosY = value;
            _isUp = transform.position.y < TargetPosY;
        }
    }

    public void InitGameDuck(DuckSpawnInfo spawnInfo)
    {
        if (spawnInfo.IsTargetDuck)
            AudioManager.Instance.PlayeMenuDuckReadyClip();
        Init(spawnInfo.Config);
        _spawnPoint = spawnInfo.SpawnPoint;
        _isGameDuck = true;
        IsTargetDuck = spawnInfo.IsTargetDuck;
        DuckTargetIcon.gameObject.SetActive(IsTargetDuck);
        SortingGroup.sortingLayerName = spawnInfo.Layer.SortingLayer;
        transform.position = spawnInfo.SpawnPoint;
        TargetPosX = ConfigManager.Instance.GetRandomMovePointX();
        TargetPosY = ConfigManager.Instance.GetRandomMovePointY(true) + _spawnPoint.y;
    }
    private void Update()
    {
        if (!_isGameDuck || IsDead)
            return;
        Move();
    }
    private void Move()
    {
        if (_isLeft && transform.position.x <= _targetPosX)
        {
            TargetPosX = ConfigManager.Instance.GetRandomMovePointX();
        }
        else if (!_isLeft && transform.position.x >= _targetPosX)
        {
            TargetPosX = ConfigManager.Instance.GetRandomMovePointX();
        }
        if (_isUp && transform.position.y >= _targetPosY)
        {
            TargetPosY = ConfigManager.Instance.GetRandomMovePointY(!_isUp) + _spawnPoint.y;
        }
        else if (!_isUp && transform.position.y <= _targetPosY)
        {
            TargetPosY = ConfigManager.Instance.GetRandomMovePointY(!_isUp) + _spawnPoint.y;
        }
        Vector2 dir = Vector2.zero;
        dir.x = _isLeft ? -1 : 1;
        dir.y = _isUp ? 1 : -1;
        transform.Translate(dir.normalized * ConfigManager.Instance.DuckMoveSpeed * Time.deltaTime);
    }
    #endregion
}
