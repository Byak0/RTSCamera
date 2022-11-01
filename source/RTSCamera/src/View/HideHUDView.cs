using MissionSharedLibrary.View;
using RTSCamera.Config.HotKey;
using System;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;
using TaleWorlds.ScreenSystem;

namespace RTSCamera.View
{
    class HideHUDView : MissionView
    {
        private bool _hideUI;        
        private bool _isTemporarilyOpenUI;

        private bool _hideBanners;
        private float _previousBannerOpacity = ManagedOptions.GetConfig(ManagedOptions.ManagedOptionsType.FriendlyTroopsBannerOpacity);

        public override void OnRemoveBehavior()
        {
            base.OnRemoveBehavior();

            MBDebug.DisableAllUI = false;            
        }

        public override void OnMissionScreenTick(float dt)
        {
            base.OnMissionScreenTick(dt);

            if ((RTSCameraGameKeyCategory.GetKey(GameKeyEnum.ToggleHUD).IsKeyPressed(Input)) || MBDebug.DisableAllUI && TaleWorlds.InputSystem.Input.IsKeyPressed(InputKey.Home))
                ToggleUI();

            if (!_isTemporarilyOpenUI)
            {
                if (ScreenManager.FocusedLayer != MissionScreen.SceneLayer)
                {
                    _isTemporarilyOpenUI = true;
                    BeginTemporarilyOpenUI();
                }
            }
            else
            {
                if (ScreenManager.FocusedLayer == MissionScreen.SceneLayer)
                {
                    _isTemporarilyOpenUI = false;
                    EndTemporarilyOpenUI();
                }
            }
        }

        public void ToggleUI()
        {
            MBDebug.DisableAllUI = !_hideUI && !MBDebug.DisableAllUI;
            _hideUI = MBDebug.DisableAllUI;
            
            _hideBanners = !_hideBanners;
            _previousBannerOpacity = _hideBanners ? ManagedOptions.GetConfig(ManagedOptions.ManagedOptionsType.FriendlyTroopsBannerOpacity) : _previousBannerOpacity;
            ManagedOptions.SetConfig(ManagedOptions.ManagedOptionsType.FriendlyTroopsBannerOpacity, _hideBanners ? 0 : _previousBannerOpacity);
        }

        public void BeginTemporarilyOpenUI()
        {
            _hideUI = MBDebug.DisableAllUI;
            MBDebug.DisableAllUI = false;             
            ManagedOptions.SetConfig(ManagedOptions.ManagedOptionsType.FriendlyTroopsBannerOpacity, _previousBannerOpacity);
        }

        public void EndTemporarilyOpenUI()
        {
            MBDebug.DisableAllUI = _hideUI;
            _previousBannerOpacity = ManagedOptions.GetConfig(ManagedOptions.ManagedOptionsType.FriendlyTroopsBannerOpacity);
            ManagedOptions.SetConfig(ManagedOptions.ManagedOptionsType.FriendlyTroopsBannerOpacity, _hideBanners ? 0 : _previousBannerOpacity);
        }
    }
}
