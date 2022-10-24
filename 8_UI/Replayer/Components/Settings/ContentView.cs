﻿using BeatSaberMarkupLanguage.Attributes;
using System;
using UnityEngine;

namespace BeatLeader.Components {
    internal abstract class ContentView : ReeUIComponentV2 {
        #region Events

        public event Action<ContentView, ContentView> ContentWasPresentedEvent;
        public event Action<ContentView, ContentView> ContentWasDismissedEvent;

        #endregion

        #region Present & Dismiss

        private Transform _viewOriginalParent;
        private ContentView _presentedView;

        public void PresentView(ContentView view) {
            _presentedView = view;
            _viewOriginalParent = view.Content.parent;

            _presentedView.Content.SetParent(Content.parent);
            _presentedView.Content.gameObject.SetActive(true);
            Content.gameObject.SetActive(false);

            RefreshContentSize(view);
            SubscribeToView(view);
            NotifyContentWasPresented(this, view);
        }

        public bool DismissView() {
            if (_presentedView == null) return false;

            _presentedView.Content.gameObject.SetActive(false);
            _presentedView.Content.SetParent(_viewOriginalParent);
            Content.gameObject.SetActive(true);

            UnsubscribeFromView(_presentedView);
            NotifyContentWasDismissed(this, _presentedView);

            _presentedView = null;
            _viewOriginalParent = null;
            return true;
        }

        #endregion

        #region Events Notify

        private void NotifyContentWasPresented(ContentView view, ContentView presentedView) {
            ContentWasPresentedEvent?.Invoke(view, presentedView);
        }
        private void NotifyContentWasDismissed(ContentView view, ContentView dismissedView) {
            ContentWasDismissedEvent?.Invoke(view, dismissedView);
        }

        #endregion

        #region Events Hierarchy

        private void SubscribeToView(ContentView view) {
            view.ContentWasPresentedEvent += NotifyContentWasPresented;
            view.ContentWasDismissedEvent += NotifyContentWasDismissed;
        }
        private void UnsubscribeFromView(ContentView view) {
            view.ContentWasPresentedEvent += NotifyContentWasPresented;
            view.ContentWasDismissedEvent += NotifyContentWasDismissed;
        }

        #endregion

        #region RefreshContentSize

        private void RefreshContentSize(ContentView view) {
            view.Content.transform.transform.localScale = Vector3.one;
        }

        #endregion
    }
}
