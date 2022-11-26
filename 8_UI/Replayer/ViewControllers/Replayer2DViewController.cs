﻿using BeatSaberMarkupLanguage.Attributes;
using BeatLeader.Components;
using Zenject;
using UnityEngine;
using BeatLeader.Utils;
using static UnityEngine.UI.CanvasScaler;
using UnityEngine.UI;
using System.Collections;
using BeatLeader.Models;

namespace BeatLeader.ViewControllers {
    [ViewDefinition(Plugin.ResourcesPath + ".BSML.Replayer.Views.Replayer2DView.bsml")]
    internal class Replayer2DViewController : StandaloneViewController<CanvasScreen> {
        #region Injection

        [Inject] private readonly DiContainer _container;
        [Inject] private readonly IReplayExitController _exitController;

        #endregion

        #region UI Components

        [UIValue("main-view")] private MainScreenView _mainScreenView;

        private new CanvasGroup _canvasGroup;

        #endregion

        #region Setup

        public void OpenLayoutEditor() {
            _mainScreenView?.OpenLayoutEditor();
        }

        protected override void InitInternal() {
            base.InitInternal();
            var canvas = Screen.Canvas;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1;
            canvas.additionalShaderChannels =
                AdditionalCanvasShaderChannels.TexCoord1
                | AdditionalCanvasShaderChannels.TexCoord2;

            var scaler = Screen.CanvasScaler;
            scaler.referenceResolution = new(350, 300);
            scaler.uiScaleMode = ScaleMode.ScaleWithScreenSize;
            scaler.dynamicPixelsPerUnit = 3.44f;
            scaler.referencePixelsPerUnit = 10f;

            gameObject.GetOrAddComponent<GraphicRaycaster>();
            _canvasGroup = Screen.gameObject.GetOrAddComponent<CanvasGroup>();
        }

        protected override void OnInstantiate() {
            _canvasGroup.alpha = 0f;
            _mainScreenView = ReeUIComponentV2WithContainer
                .InstantiateInContainer<MainScreenView>(_container, null);
            _exitController.ReplayExitEvent += HandleReplayExit;
            _mainScreenView.LayoutBuiltEvent += HandleUIBuilt;
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            _exitController.ReplayExitEvent -= HandleReplayExit;
        }

        #endregion

        #region Callbacks

        private void HandleUIBuilt() {
            CoroutinesHandler.instance.StartCoroutine(UIAnimationCoroutine());
        }

        private void HandleReplayExit() {
            CoroutinesHandler.instance.StartCoroutine(UIAnimationCoroutine(false));
        }

        #endregion

        #region Animation

        private const float InDuration = 0.5f;
        private const float OutDuration = 0.3f;
        private const float AnimationFrameRate = 60f;

        private IEnumerator UIAnimationCoroutine(bool show = true) {
            yield return new WaitForEndOfFrame();
            _canvasGroup.alpha = show ? 0 : 1;
            var duration = show ? InDuration : OutDuration;
            var totalFramesCount = Mathf.FloorToInt(duration * AnimationFrameRate);
            var frameDuration = duration / totalFramesCount;
            var alphaStep = 1f / (show ? totalFramesCount : -totalFramesCount);

            for (int frame = 0; frame < totalFramesCount; frame++) {
                _canvasGroup.alpha += alphaStep;
                yield return new WaitForSeconds(frameDuration);
            }
        }

        #endregion
    }
}
