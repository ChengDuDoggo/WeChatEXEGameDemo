using UnityEngine;
using UnityEngine.UI;
using WeChatWASM;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Animation ReadyGoAnimation;
    public Text ScoreText;
    public Text TimeText;
    public Text gameOverScoreText;
    public Image GunCDMask;
    public Button superModeButton;
    public Transform gameOverPanel;
    public Button gameOverShareButton;
    public Button gameOverCloseButton;
    //调用微信小游戏SDK，定义一个微信激励广告类型
    private WXRewardedVideoAd videoAd;
    private void Awake()
    {
        Instance = this;
        superModeButton.onClick.AddListener(SuperModeButtonClick);
        gameOverShareButton.onClick.AddListener(GameOverShareButtonClick);
        gameOverCloseButton.onClick.AddListener(GameOverCloseButtonClick);
#if !UNITY_EDITOR
        //实例化广告，但是需要到微信公众平台获得广告位ID
        videoAd = WX.CreateRewardedVideoAd(new WXCreateRewardedVideoAdParam() { adUnitId = "adunit-ad79c7fbc07c782d" });//创建激励类型广告
        //可以为广告被关闭时做一些监听
        videoAd.OnClose(OnVideoADClose);
#endif
        EnterMenu();
    }
    public void EnterMenu()
    {
        superModeButton.gameObject.SetActive(true);
        gameOverPanel.gameObject.SetActive(false);
        ReadyGoAnimation.gameObject.SetActive(false);
        ScoreText.transform.parent.gameObject.SetActive(false);
        TimeText.gameObject.SetActive(false);
        GunCDMask.transform.parent.gameObject.SetActive(false);
    }
    public void EnterReadyGo()
    {
        ReadyGoAnimation.gameObject.SetActive(true);
        ReadyGoAnimation.Play("ReadyGo");
    }
    public void EnterGame()
    {
        superModeButton.gameObject.SetActive(false);
        ScoreText.transform.parent.gameObject.SetActive(true);
        TimeText.gameObject.SetActive(true);
        GunCDMask.transform.parent.gameObject.SetActive(true);
    }
    public void UpdateScore(int score)
    {
        ScoreText.text = score.ToString();
    }
    public void UpdateGunCD(float value)
    {
        GunCDMask.fillAmount = value;
    }
    public void UpdateTime(int time)
    {
        TimeText.text = time.ToString();
    }
    private void SuperModeButtonClick()
    {
#if !UNITY_EDITOR
        videoAd.Show();//展示广告
#endif
    }
    private void OnVideoADClose(WXRewardedVideoAdOnCloseResponse response)
    {
        if (response != null && response.isEnded)
        {
            //视频正常播放完毕，向用户发放奖励
            superModeButton.gameObject.SetActive(false);
            GameManager.Instance.SuperMode = true;
        }
    }
    public void GameOver()
    {
        gameOverScoreText.text = ScoreText.text;
        gameOverPanel.gameObject.SetActive(true);
    }
    private void GameOverShareButtonClick()
    {
        //微信SDK包功能：拉起通讯录进行分享APP并显示参数title显示分享信息
#if !UNITY_EDITOR
        WX.ShareAppMessage(new ShareAppMessageOption() { title = $"我在DuckShoot中获得了{gameOverScoreText.text}的分数，欢迎挑战!" });   
#endif
    }
    private void GameOverCloseButtonClick()
    {
        GameManager.Instance.GameState = GameState.Menu;
    }
}
