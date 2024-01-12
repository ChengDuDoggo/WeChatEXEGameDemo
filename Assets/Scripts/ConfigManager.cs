using System;
using System.Collections.Generic;
using UnityEngine;
//全局配置类，配置一些固有的属性和数据
public class ConfigManager : MonoBehaviour
{
    public static ConfigManager Instance;
    public List<DuckLayerConfig> DuckLayerConfigs = new List<DuckLayerConfig>();
    public List<DuckConfig> DuckConfigs = new List<DuckConfig>();
    public Vector2 HorizontalRange = new Vector2(-25.0f, 25.0f);//鸭子的左右移动范围
    public Vector2 MenuDuckPosition = new Vector2(0, -5.52f);//菜单鸭在场景中的固定坐标位置
    public Vector3 GunOffset = new Vector3(0.0f, 6.28f);//枪的位置的偏移量
    public float DuckMoveSpeed = 8.0f;//鸭子上浮动画的移动速度
    public float ReadyGoAnimationTime = 2.0f;
    public float maxGameTime = 60.0f;
    public float DuckUpDistance = 8.0f;
    public float SuperModeDuckInterval = 0.2f;
    public float DuckInterval = 1f;
    public AnimationCurve spawnCurve;
    public float shootCD = 1;
    public float superModeShootCD = 1;
    [Range(0f, 1f)]
    public float TargetDuckProbability = 0.4f;
    private void Awake()
    {
        Instance = this;
    }
    //随机返回一个鸭子属性来生成一个随机的鸭子
    public DuckConfig GetRandomDuckConfig()
    {
        return DuckConfigs[UnityEngine.Random.Range(0, DuckConfigs.Count)];
    }
    public DuckLayerConfig GetRandomDuckLayerConfig()
    {
        return DuckLayerConfigs[UnityEngine.Random.Range(0, DuckLayerConfigs.Count)];
    }
    public bool GetRandomTargetDuck()
    {
        return UnityEngine.Random.Range(0f, 1f) < TargetDuckProbability;
    }
    public float GetRandomMovePointX()
    {
        return UnityEngine.Random.Range(HorizontalRange.x, HorizontalRange.y);
    }
    public float GetRandomMovePointY(bool isUp)
    {
        if (isUp)
            return UnityEngine.Random.Range(DuckUpDistance / 3, DuckUpDistance);
        else
            return UnityEngine.Random.Range(0, DuckUpDistance / 3);
    }
    public DuckSpawnInfo GetRandomDuckSpawnInfo()
    {
        DuckConfig duckConfig = GetRandomDuckConfig();
        DuckLayerConfig layerConfig = GetRandomDuckLayerConfig();
        Vector2 point = new Vector2(GetRandomMovePointX(), layerConfig.PoxY);
        DuckSpawnInfo duckSpawnInfo = new DuckSpawnInfo(duckConfig, layerConfig, point, GetRandomTargetDuck());
        return duckSpawnInfo;
    }
}
[Serializable]
public class DuckConfig//鸭子常规设置配置类
{
    public Sprite Sprite;
}
[Serializable]
public class DuckLayerConfig//鸭子层级配置类
{
    public string SortingLayer;//层级名称
    public float PoxY;//鸭子的Y轴位置
}
public struct DuckSpawnInfo
{
    public DuckConfig Config;
    public DuckLayerConfig Layer;
    public Vector2 SpawnPoint;
    public bool IsTargetDuck;
    public DuckSpawnInfo(DuckConfig config, DuckLayerConfig layer, Vector2 spawnPoint, bool isTargetDuck)
    {
        this.Config = config;
        this.Layer = layer;
        this.SpawnPoint = spawnPoint;
        this.IsTargetDuck = isTargetDuck;
    }
}
