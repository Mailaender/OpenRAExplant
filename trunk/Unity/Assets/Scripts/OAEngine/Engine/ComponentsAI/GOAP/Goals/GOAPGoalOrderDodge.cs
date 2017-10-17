﻿using System;
using System.Collections.Generic;
using Engine.ComponentsAI.AStarMachine;
using Engine.ComponentsAI.Factories;
using Engine.ComponentsAI.GOAP.Core;
using TrueSync;

namespace Engine.ComponentsAI.GOAP.Goals
{
    class GOAPGoalOrderDodge : GOAPGoal
    {
        public GOAPGoalOrderDodge(Agent owner) : base(E_GOAPGoals.E_ORDER_DODGE, owner) { }

        public override void InitGoal()
        {

        }

        public override FP GetMaxRelevancy()
        {
            return Owner.BlackBoard.GOAP_DodgeRelevancy;
        }
        public override void CalculateGoalRelevancy()
        {
            if (Owner.CurrentGOAPGoal == this)
            {
                GoalRelevancy = 0;
            }
            else
            {
                WorldStateProp prop = Owner.WorldState.GetWSProperty(E_PropKey.E_ORDER);
                if (prop != null && prop.GetOrder() == AgentOrder.E_OrderType.E_DODGE)
                    GoalRelevancy = 0.9f;
                else
                    GoalRelevancy = 0;
            }
        }

        public override void SetDisableTime() { NextEvaluationTime = 0.5f + Game.WorldTime; }

        public override void SetWSSatisfactionForPlanning(WorldState worldState)
        {
            worldState.SetWSProperty(E_PropKey.E_IN_DODGE, true);
        }

        public override bool IsWSSatisfiedForPlanning(WorldState worldState)
        {
            WorldStateProp prop = worldState.GetWSProperty(E_PropKey.E_IN_DODGE);

            if (prop != null && prop.GetBool() == true)
                return true;

            return false;
        }

        public override bool IsSatisfied()
        {
            return IsPlanFinished();
        }

        public override void Deactivate()
        {
            WorldStateProp prop = Owner.WorldState.GetWSProperty(E_PropKey.E_ORDER);
            if (prop.GetOrder() == AgentOrder.E_OrderType.E_DODGE)
                Owner.WorldState.SetWSProperty(E_PropKey.E_ORDER, AgentOrder.E_OrderType.E_NONE);

            base.Deactivate();
        }
    }
}