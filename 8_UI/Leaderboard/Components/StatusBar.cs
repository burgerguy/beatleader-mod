using System.Collections;
using BeatLeader.Manager;
using BeatSaberMarkupLanguage.Attributes;
using JetBrains.Annotations;
using UnityEngine;

namespace BeatLeader.Components {
    [ViewDefinition(Plugin.ResourcesPath + ".BSML.Components.StatusBar.bsml")]
    internal class StatusBar : ReeUIComponent {
        #region Initialize/Dispose

        protected override void OnInitialize() {
            LeaderboardEvents.StatusMessageEvent += OnStatusMessage;
            LeaderboardEvents.ProfileRequestFailedEvent += OnProfileRequestFailed;
            LeaderboardEvents.UploadSuccessAction += OnScoreUploadSuccess;
            LeaderboardEvents.UploadFailedAction += OnScoreUploadFailed;
        }

        protected override void OnDispose() {
            LeaderboardEvents.StatusMessageEvent -= OnStatusMessage;
            LeaderboardEvents.ProfileRequestFailedEvent -= OnProfileRequestFailed;
            LeaderboardEvents.UploadSuccessAction -= OnScoreUploadSuccess;
            LeaderboardEvents.UploadFailedAction -= OnScoreUploadFailed;
        }

        protected override void OnDeactivate() {
            StopAllCoroutines();
            MessageText = "";
        }

        #endregion

        #region Events

        private void OnProfileRequestFailed() {
            ShowBadNews("Profile update failed");
        }

        private void OnScoreUploadFailed(bool completely, int retry) {
            var postfix = completely ? "No more retries" : $" Retry attempt {retry}";
            ShowBadNews($"Score upload failed. {postfix}");
        }

        private void OnScoreUploadSuccess() {
            ShowGoodNews("Score uploaded!");
        }

        private void OnStatusMessage(string message, float duration) {
            ShowMessage(message, duration);
        }

        #endregion

        #region ShowMessage

        private const float DefaultDuration = 1.4f;
        private const string GoodNewsColor = "#88FF88";
        private const string BadNewsColor = "#FF8888";

        private void ShowGoodNews(string message) {
            ShowMessage($"<color={GoodNewsColor}>{message}");
        }

        private void ShowBadNews(string message) {
            ShowMessage($"<color={BadNewsColor}>{message}");
        }

        private void ShowMessage(string message, float duration = DefaultDuration) {
            StopAllCoroutines();
            StartCoroutine(ShowMessageCoroutine(message, duration));
        }

        private IEnumerator ShowMessageCoroutine(string message, float duration) {
            MessageText = message;
            yield return new WaitForSeconds(duration);
            MessageText = "";
        }

        #endregion

        #region MessageText

        private string _messageText = "";

        [UIValue("message-text"), UsedImplicitly]
        public string MessageText {
            get => _messageText;
            set {
                if (_messageText.Equals(value)) return;
                _messageText = value;
                NotifyPropertyChanged();
            }
        }

        #endregion
    }
}