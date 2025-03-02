using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 设置面板
/// </summary>
public class SettingPanel : BasePanel
{
    public Button closeBtn;
    public Slider musicSlider;
    public Slider soundSlider;
    public Toggle musicToggle;
    public Toggle soundToggle;

    public override void Init()
    {
        MusicData data = GameDataManager.Instance.musicData;
        musicSlider.value = data.musicValue;
        soundSlider.value = data.soundValue;
        musicToggle.isOn = data.isMusicOpen;
        soundToggle.isOn = data.isSoundOpen;

        closeBtn.onClick.AddListener(() =>
        {
            //保存数据
            GameDataManager.Instance.SaveMusicData();
            UIManager.Instance.HidePanel<SettingPanel>();
        });

        musicSlider.onValueChanged.AddListener((value) =>
        {
            GameDataManager.Instance.musicData.musicValue = value;
            AudioManager.Instance.SetMusicVolume(value);
        });

        soundSlider.onValueChanged.AddListener((value) =>
        {
            GameDataManager.Instance.musicData.soundValue = value;
            AudioManager.Instance.SetSoundVolume(value);
        });

        musicToggle.onValueChanged.AddListener((value) =>
        {
            GameDataManager.Instance.musicData.isMusicOpen = value;
            AudioManager.Instance.SetIsMusicOpen(value);
        });

        soundToggle.onValueChanged.AddListener((value) =>
        {
            GameDataManager.Instance.musicData.isSoundOpen = value;
            AudioManager.Instance.SetIsSoundOpen(value);
        });
    }
}
