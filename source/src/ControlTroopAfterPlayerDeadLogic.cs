﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Screens;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.Missions;
using TaleWorlds.MountAndBlade.View.Screen;

namespace EnhancedMission
{
    class ControlTroopAfterPlayerDeadLogic : MissionLogic
    {

        private readonly GameKeyConfig _gameKeyConfig = GameKeyConfig.Get();
        private readonly EnhancedMissionConfig _config = EnhancedMissionConfig.Get();
        public bool ControlTroopAfterDead()
        {
            // Mission.MainAgent may be null because of free camera mode.
            if (Utility.IsPlayerDead() && this.Mission.PlayerTeam != null && Utility.IsAgentDead(this.Mission.PlayerTeam.PlayerOrderController.Owner))
            {
                var missionScreen = ScreenManager.TopScreen as MissionScreen;
                Agent closestAllyAgent =
                    (missionScreen?.LastFollowedAgent?.IsActive() ?? false) &&
                    missionScreen?.LastFollowedAgent.Team == Mission.PlayerTeam
                        ? missionScreen?.LastFollowedAgent
                        : GetAgentToControl() ?? this.Mission.PlayerTeam.Leader;
                if (closestAllyAgent != null)
                {
                    GameTexts.SetVariable("ControlledTroopName", closestAllyAgent.Name);
                    Utility.DisplayLocalizedText("str_em_control_troop");
                    var switchCameraLogic = Mission.GetMissionBehaviour<SwitchFreeCameraLogic>();
                    if (!switchCameraLogic.isSpectatorCamera)
                        closestAllyAgent.Controller = Agent.ControllerType.Player;
                    else
                        Mission.MainAgent = closestAllyAgent;
                    return true;
                }
                else
                {
                    Utility.DisplayLocalizedText("str_em_no_troop_to_control");
                    return false;
                }
            }

            return false;
        }

        public Agent GetAgentToControl()
        {
            var agents = Mission.GetNearbyAllyAgents(
                new WorldPosition(this.Mission.Scene, this.Mission.Scene.LastFinalRenderCameraPosition).AsVec2, 1E+7f,
                Mission.PlayerTeam);
            var inPlayerPartyOnly = _config.ControlTroopsInPlayerPartyOnly;
            if (_config.PreferToControlCompanions)
            {
                Agent firstAgent = null;
                var heroAgent = agents.FirstOrDefault(agent =>
                {
                    if (inPlayerPartyOnly && !Utility.IsInPlayerParty(agent)) return false;
                    if (firstAgent != null)
                        firstAgent = agent;
                    return agent.IsHero;
                });
                return heroAgent ?? firstAgent;
            }
            else
            {
                return agents.FirstOrDefault(agent => !inPlayerPartyOnly || Utility.IsInPlayerParty(agent));
            }
        }

        public override void OnBehaviourInitialize()
        {
            base.OnBehaviourInitialize();

            Mission.OnMainAgentChanged += OnMainAgentChanged;
        }

        public override void OnRemoveBehaviour()
        {
            base.OnRemoveBehaviour();

            this.Mission.OnMainAgentChanged -= OnMainAgentChanged;
        }

        public override void OnMissionTick(float dt)
        {
            base.OnMissionTick(dt);

            if (this.Mission.InputManager.IsKeyPressed(_gameKeyConfig.GetKey(GameKeyEnum.ControlTroop)))
            {
                ControlTroopAfterDead();
            }
        }

        private void OnMainAgentChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Mission.MainAgent == null && _config.ControlAlliesAfterDeath)
            {
                ControlTroopAfterDead();
            }
        }
    }
}