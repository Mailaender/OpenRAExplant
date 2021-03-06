﻿using System;
using System.Collections.Generic;
using Engine;
using Engine.ComponentAnim.Core;
using Engine.ComponentsAI.AgentActions;
using Engine.ComponentsAI.AStarMachine;
using Engine.ComponentsAI.ComponentPlayer;
using Engine.ComponentsAI.Factories;
using Engine.ComponentsAI.GOAP;
using Engine.ComponentsAI.GOAP.Core;
using Engine.Primitives;
using TrueSync;

namespace OAEngine.Engine.ComponentsAI
{
    public interface IActionHandler
    {
        void HandleAction(AgentAction a);
    }

    public enum E_MotionType
    {
        None,
        Walk,
        Run,
        Sprint,
        Roll,
        Attack,
        Block,
        BlockingAttack,
        Injury,
        Knockdown,
        Death,
        AnimationDrive,
    }

    public class BlackBoard
    {
        public bool DontUpdate = true;
        
        public FP IdleTimer = 0;

        private List<AgentAction> m_ActiveActions = new List<AgentAction>();
        private List<IActionHandler> m_ActionHandlers = new List<IActionHandler>();

       
        public E_MotionType MotionType = E_MotionType.None;

        public E_WeaponState WeaponState = E_WeaponState.NotInHands;
        public E_WeaponType WeaponSelected = E_WeaponType.Katana;
        public E_WeaponType WeaponToSelect = E_WeaponType.None;

        public FP WeaponRange = 2;
        public FP sqrWeaponRange { get { return WeaponRange * WeaponRange; } }


        public FP CombatRange = 4;
        public FP sqrCombatRange { get { return CombatRange * CombatRange; } }

        public E_LookType LookType;

        public FP MaxSprintSpeed = 8f;
        public FP MaxRunSpeed = 5f;
        public FP MaxWalkSpeed = 1.8f;
        public FP MaxCombatMoveSpeed = 1f;
        public FP MaxHealth = 50f;
        public FP MaxKnockdownTime = 4f;

        public FP SpeedSmooth = 8f;
        public FP RotationSmooth = 20f;
        public FP RotationSmoothInMove = 8.0f;
        public FP RollDistance = 4.0f;
        public FP MoveSpeedModifier = 1;


        public FP Speed = 0;
        public TSVector2 MoveDir;

        public FP Health = 30;

        public FP Rage = 0;
        public FP Fear = 0;
        public FP Dodge = 0;
        public FP Berserk = 0;
        
        public TSVector2 DesiredPosition;
        public TSVector2 DesiredDirection;
        public FP DesiredFacing;

        public Agent DesiredTarget;
        public E_AttackType DesiredAttackType;
        public AnimAttackData DesiredAttackPhase;


        /////////////// cOMBAT SETTINGS ///////////////////////
        public FP RageMin = 0; //0 = no attack
        public FP RageMax = 100;// 100 % chance is do do attack
        public FP RageModificator = 5;//per second
        public FP DodgeMin = 10;
        public FP DodgeMax = 30;
        public FP DodgeModificator = -3; //per second
        public FP FearMin = 10;
        public FP FearMax = 30;
        public FP FearModificator = -3; //per second
        public FP BerserkMin = 0; // = no attack
        public FP BerserkMax = 0;// 100 % chance is do do attack
        public FP BerserkModificator = 0;//per second

        public FP RageInjuryModificator = 10; // each injury increase rage
        public FP DodgeInjuryModificator = 10;  // each injury increase dodge
        public FP FearInjuryModificator = 5;
        public FP BerserkInjuryModificator = 0;

        public FP RageBlockModificator = 20; // each block increase rage
        public FP FearBlockModificator = 20; // each block increase rage
        public FP BerserkBlockModificator = 0; // each block increase rage

        public FP DodgeAttackModificator = -5; // each attack increase rage
        public FP FearAttackModificator = 20; // each attack increase rage
        public FP BerserkAttackModificator = 0; // each attack increase rage


        #region Goal Setting
        public FP GOAP_AlertRelevancy = 0.7f;
        public FP GOAP_CalmRelevancy = 0.2f;
        public FP GOAP_BlockRelevancy = 0.7f;
        public FP GOAP_DodgeRelevancy = 0.9f;
        public FP GOAP_GoToRelevancy = 0.5f;
        public FP GOAP_CombatMoveBackwardRelevancy = 0.7f;
        public FP GOAP_CombatMoveForwardRelevancy = 0.75f;
        public FP GOAP_CombatMoveLeftRelevancy = 0.6f;
        public FP GOAP_CombatMoveRightRelevancy = 0.6f;
        public FP GOAP_LookAtTargetRelevancy = 0.7f;
        public FP GOAP_KillTargetRelevancy = 0.8f;
        public FP GOAP_PlayAnimRelevancy = 0.95f;
        public FP GOAP_UseWorlObjectRelevancy = 0.9f;
        public FP GOAP_ReactToDamageRelevancy = 1.0f;
        public FP GOAP_IdleActionRelevancy = 0.4f;
        public FP GOAP_TeleportRelevancy = 0.9f;

        public FP GOAP_AlertDelay = 0;
        public FP GOAP_CalmDelay = 0;
        public FP GOAP_BlockDelay = 2.7f;
        public FP GOAP_DodgeDelay = 0;
        public FP GOAP_GoToDelay = 0;
        public FP GOAP_CombatMoveDelay = 0.5f;
        public FP GOAP_CombatMoveLeftDelay = 2.6f;
        public FP GOAP_CombatMoveRightDelay = 2.6f;
        public FP GOAP_LookAtTargetDelay = 0.4f;
        public FP GOAP_KillTargetDelay = 0f;
        public FP GOAP_PlayAnimDelay = 0.0f;
        public FP GOAP_UseWorlObjectDelay = 0f;
        public FP GOAP_CombatMoveBackwardDelay = 3.5f;
        public FP GOAP_CombatMoveForwardDelay = 3.5f;
        public FP GOAP_IdleActionDelay = 10 * 1000;
        public FP GOAP_TeleportDelay = 4;

        #endregion



        




        public bool _Stop = false;
        /// <summary>
        /// 某些情况下会禁止获取用户输入
        /// </summary>
        public bool Stop { get { return _Stop; } set { _Stop = value; Game.AllowUserInput(!_Stop); } }

        public Agent Owner;
        
        public AgentAction ActionGet(int index)
        {
            return m_ActiveActions[index];
        }

        public int ActionCount()
        {
            return m_ActiveActions.Count;
        }

        public void OrderAdd(AgentOrder order)
        {
            if (IsOrderAddPossible(order.Type))
            {
                Owner.WorldState.SetWSProperty(E_PropKey.E_ORDER, order.Type);
                switch (order.Type)
                {
                    case AgentOrder.E_OrderType.E_STOPMOVE:
                        Owner.WorldState.SetWSProperty(E_PropKey.E_AT_TARGET_POS, true);
                        DesiredPosition = Owner.Position;
						DesiredFacing = 0;
                        break;
                    case AgentOrder.E_OrderType.E_GOTO:
                        Owner.WorldState.SetWSProperty(E_PropKey.E_AT_TARGET_POS, false);
                        DesiredPosition = order.Position;
                        DesiredDirection = order.Direction;
						DesiredFacing = order.Facing * new FP(360) / new FP(256);
                        MoveSpeedModifier = order.MoveSpeedModifier;
                        break;
                    case AgentOrder.E_OrderType.E_DODGE:
                        DesiredDirection = order.Direction;
                        //Debug.Log(Time.timeSinceLevelLoad + " order arrived " + order.Type);
                        break;
                    //case AgentOrder.E_OrderType.E_USE:
                    //    UnityEngine.Debug.LogError("AgentOrder.E_OrderType.E_USE:");
                    //    Owner.WorldState.SetWSProperty(E_PropKey.E_USE_WORLD_OBJECT, true);

                    //    if ((order.Position - Owner.Position).sqrMagnitude <= 1)
                    //        Owner.WorldState.SetWSProperty(E_PropKey.E_AT_TARGET_POS, true);
                    //    else
                    //        Owner.WorldState.SetWSProperty(E_PropKey.E_AT_TARGET_POS, false);
                    //    DesiredPosition = order.Position;
                    //    InteractionObject = order.InteractionObject;
                    //    Interaction = order.Interaction;
                    //    break;
                    case AgentOrder.E_OrderType.E_ATTACK:
                        if (order.Target == null || (order.Target.Position - Owner.Position).magnitude <= (WeaponRange + 0.2))
                            Owner.WorldState.SetWSProperty(E_PropKey.E_IN_WEAPONS_RANGE, true);
                        else
                            Owner.WorldState.SetWSProperty(E_PropKey.E_IN_WEAPONS_RANGE, false);

                        DesiredAttackType = order.AttackType;
                        DesiredTarget = order.Target;
                        DesiredDirection = order.Direction;
                        DesiredAttackPhase = order.AnimAttackData;
                        break;
                }

                //  Debug.Log(Time.timeSinceLevelLoad + " new order arrived " + order.Type);
            }
            else if (order.Type == AgentOrder.E_OrderType.E_ATTACK)
            {
                // Debug.Log(Time.timeSinceLevelLoad +  " " +order.Type + " is nto allowed because " + currentOrder);
            }
            AgentOrderFactory.Return(order);
        }

        public bool IsOrderAddPossible(AgentOrder.E_OrderType orderType)
        {
            AgentOrder.E_OrderType currentOrder = Owner.WorldState.GetWSProperty(E_PropKey.E_ORDER).GetOrder();

            if (orderType == AgentOrder.E_OrderType.E_DODGE && currentOrder != AgentOrder.E_OrderType.E_DODGE && currentOrder != AgentOrder.E_OrderType.E_USE)
                return true;
            else if (currentOrder != AgentOrder.E_OrderType.E_ATTACK && currentOrder != AgentOrder.E_OrderType.E_DODGE && currentOrder != AgentOrder.E_OrderType.E_USE)
                return true;
            else
                return false;
        }


        public void Update()
        {
            IdleTimer += Time.deltaTime;

            for (int i = 0; i < m_ActiveActions.Count; i++)
            {
                if (m_ActiveActions[i].IsActive())
                    continue;

                ActionDone(m_ActiveActions[i]);
                m_ActiveActions.RemoveAt(i);

                return;
            }
            
        }

        private void ActionDone(AgentAction action)
        {
            AgentActionFactory.Return(action);
        }

        public void ActionHandlerAdd(IActionHandler handler)
        {
            for (int i = 0; i < m_ActionHandlers.Count; i++)
                if (m_ActionHandlers[i] == handler)
                    return;

            m_ActionHandlers.Add(handler);
        }

        public void ActionAdd(AgentAction action)
        {
            IdleTimer = 0;

            m_ActiveActions.Add(action);

            for (int i = 0; i < m_ActionHandlers.Count; i++)
                m_ActionHandlers[i].HandleAction(action);
        }

        public void Reset()
        {
            m_ActiveActions.Clear();

            Stop = false;
            MotionType = E_MotionType.None;
            WeaponState = E_WeaponState.NotInHands;
            WeaponToSelect = E_WeaponType.None;

            Speed = 0;

            //TODO://
            //Health = RealMaxHealth;

            Rage = RageMin;
            Dodge = DodgeMin;
            Fear = FearMin;
            IdleTimer = 0;

            MoveDir = TSVector2.zero;

            DesiredPosition = TSVector2.zero;
            DesiredDirection = TSVector2.zero;

            //InteractionObject = null;
            //Interaction = E_InteractionType.None;

            //DesiredAnimation = "";

            DesiredTarget = null;
            DesiredAttackType = E_AttackType.None;

            DontUpdate = false;
        }


    }
}
