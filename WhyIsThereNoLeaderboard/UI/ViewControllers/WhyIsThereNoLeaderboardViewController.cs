using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.FloatingScreen;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using System;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WhyIsThereNoLeaderboard.Interfaces;
using WhyIsThereNoLeaderboard.Managers;
using Zenject;

#nullable enable
namespace WhyIsThereNoLeaderboard.UI.ViewControllers
{
    internal class WhyIsThereNoLeaderboardViewController : BSMLAutomaticViewController, IInitializable, IDisposable, INotifyLeaderboardSet
    {

        [Inject]
        private readonly PlatformLeaderboardViewController platformLeaderboardViewController = null!;

        [Inject]
        private BeatmodsManager _beatmodsManager = null!;

        [UIParams]
        internal BeatSaberMarkupLanguage.Parser.BSMLParserParams? parserParams = null;

        [UIComponent("info-text")]
        private TextMeshProUGUI infoText = null!;

        [UIComponent("info-title")]
        private TextMeshProUGUI infoTitle = null!;

        [UIComponent("ss-download")]
        private Button scoreSaberDownloadbutton = null!;

        [UIComponent("bl-download")]
        private Button beatLeaderDownloadbutton = null!;

        private BeatmapKey? selectedLevelKey;
        private FloatingScreen? customPanelFloatingScreen;

        public void Initialize()
        {
            customPanelFloatingScreen = FloatingScreen.CreateFloatingScreen(new Vector2(82.5f, 100f), false, Vector3.zero, Quaternion.identity);

            var customFloatingScreenTransform = customPanelFloatingScreen.transform;
            customFloatingScreenTransform.SetParent(platformLeaderboardViewController.transform);
            customFloatingScreenTransform.localPosition = new Vector3(7.5f, 0f, 0f);
            customFloatingScreenTransform.localScale = Vector3.one;

            var customFloatingScreenGO = customPanelFloatingScreen.gameObject;
            customFloatingScreenGO.SetActive(false);
            customFloatingScreenGO.SetActive(true);
            customFloatingScreenGO.name = "WhyIsThereNoLeaderboardPanel";

            BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetAssembly(GetType()), "WhyIsThereNoLeaderboard.UI.Views.LeaderboardInfo.bsml"), customPanelFloatingScreen.gameObject, this);
            platformLeaderboardViewController.didActivateEvent += OnLeaderboardActivated;
        }

        public void Dispose() => platformLeaderboardViewController.didActivateEvent -= OnLeaderboardActivated;

        private void OnLeaderboardStatusUpdated()
        {
            if(selectedLevelKey != null && selectedLevelKey.Value.levelId.StartsWith("custom_level_"))
            {
                customPanelFloatingScreen?.gameObject.SetActive(true);
            }
            else
            {
                customPanelFloatingScreen?.gameObject.SetActive(false);
            }
        }

        [UIAction("ss-info-click")]
        private void ScoreSaberInfoClick()
        {
            Application.OpenURL("https://scoresaber.com/");

            infoText.text = "More info on ScoreSaber has been opened in your browser.";
            infoTitle.text = "More Info";

            parserParams?.EmitEvent("open-moreInfoModal");
        }

        [UIAction("bl-info-click")]
        private void BeatLeaderInfoClick()
        {
            Application.OpenURL("https://www.beatleader.xyz/");

            infoText.text = "More info on BeatLeader has been opened in your browser.";
            infoTitle.text = "More Info";

            parserParams?.EmitEvent("open-moreInfoModal");
        }

        [UIAction("closePressed")]
        private void ModalClosePressed()
        {
            scoreSaberDownloadbutton.interactable = true;
            beatLeaderDownloadbutton.interactable = true;

            parserParams?.EmitEvent("closeAllModals");
        }

        [UIAction("ss-download-click")]
        private async void ScoreSaberDownloadClick()
        {
            scoreSaberDownloadbutton.interactable = false;
            beatLeaderDownloadbutton.interactable = false;

            var mod = await _beatmodsManager.DownloadModFromID("ScoreSaber");

            if (mod == true)
            {
                infoText.text = "ScoreSaber has been successfully downloaded. Restart your game to finish installation.";
                infoTitle.text = "Download";
            }
            else
            {
                infoText.text = "ScoreSaber failed to install. Please install manually using your preferred mod installer.";
                infoTitle.text = "Download";
            }

            parserParams?.EmitEvent("open-moreInfoModal");
        }

        [UIAction("bl-download-click")]
        private async void BeatLeaderDownloadClick()
        {
            scoreSaberDownloadbutton.interactable = false;
            beatLeaderDownloadbutton.interactable = false;

            var mod = await _beatmodsManager.DownloadModFromID("BeatLeader");

            if (mod == true)
            {
                infoText.text = "BeatLeader has been successfully downloaded. Restart your game to finish installation.";
                infoTitle.text = "Download";
            }
            else
            {
                infoText.text = "BeatLeader failed to install. Please install manually using your preferred mod installer.";
                infoTitle.text = "Download";
            }

            parserParams?.EmitEvent("open-moreInfoModal");
        }

        private void OnLeaderboardActivated(bool firstactivation, bool addedtohierarchy, bool screensystemenabling)
        {
            customPanelFloatingScreen?.GetComponent<CurvedCanvasSettings>().SetRadius(platformLeaderboardViewController.transform.parent.parent.GetComponent<CurvedCanvasSettings>().radius);
            OnLeaderboardStatusUpdated();
        }

        public void OnLeaderboardSet(BeatmapKey beatmapKey)
        {
            selectedLevelKey = beatmapKey;
            OnLeaderboardStatusUpdated();
        }
    }
}
