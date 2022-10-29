using RTSCamera.Config.HotKey;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.ScreenSystem;

namespace RTSCamera.View
{
    class HideHUDView : MissionView
    {
        private bool _hideUI;        
        private bool _isTemporarilyOpenUI;

        private bool _hideBanners;
        private float _previousBannerOpacity;

        public override void OnRemoveBehavior()
        {
            base.OnRemoveBehavior();

            MBDebug.DisableAllUI = false;
            ManagedOptions.SetConfig(ManagedOptions.ManagedOptionsType.FriendlyTroopsBannerOpacity, _previousBannerOpacity);
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

            float bannerOpacityOption = ManagedOptions.GetConfig(ManagedOptions.ManagedOptionsType.FriendlyTroopsBannerOpacity);
            _previousBannerOpacity = _hideBanners ? _previousBannerOpacity : bannerOpacityOption;
            ManagedOptions.SetConfig(ManagedOptions.ManagedOptionsType.FriendlyTroopsBannerOpacity, _hideBanners ? _previousBannerOpacity : 0);
            _hideBanners = !_hideBanners;

        }

        public void BeginTemporarilyOpenUI()
        {
            _hideUI = MBDebug.DisableAllUI;
            MBDebug.DisableAllUI = false;
        }

        public void EndTemporarilyOpenUI()
        {
            MBDebug.DisableAllUI = _hideUI;
        }
    }
}
