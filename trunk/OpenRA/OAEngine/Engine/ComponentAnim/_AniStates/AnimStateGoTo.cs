﻿using System;
using System.Collections.Generic;
using Engine.ComponentAnim.Core;
using Engine.ComponentsAI.AgentActions;
using Engine.ComponentsAI.AStarMachine;
using Engine.Primitives;
using OAEngine.Engine.ComponentsAI;

namespace Engine.ComponentAnim._AniStates
{
    public class AnimStateGoTo : AnimState
    {
        AgentActionGoTo Action;
        float MaxSpeed;
        string AnimName;

        WRot FinalRotation = new WRot();
        WRot StartRotation = new WRot();
        float RotationProgress;


        public AnimStateGoTo(Animation anims, Agent owner)
            : base(anims, owner)
        {
        }


        override public void OnActivate(AgentAction action)
        {
            // Time.timeScale = 0.1f;
            base.OnActivate(action);

            AnimName = null;
            PlayAnim(Action.Motion);
        }

        override public void OnDeactivate()
        {
            AnimEngine[AnimName].speed = 1;

            Action.SetSuccess();
            Action = null;

            Owner.BlackBoard.Speed = 0;

            base.OnDeactivate();

            // Time.timeScale = 1;
        }

        override public void Update()
        {
            long dist = (Action.FinalPosition - Owner.Position).LengthSquared;
            Vector3 dir;
            
            if (Action.Motion == E_MotionType.Sprint)
            {
                if (dist < 0.5f * 0.5f)
                    MaxSpeed = Owner.BlackBoard.MaxWalkSpeed;
            }
            else
            {
                if (dist < 1.5f * 1.5f)
                    MaxSpeed = Owner.BlackBoard.MaxWalkSpeed;
            }


            if (Owner.BlackBoard.LookType == E_LookType.TrackTarget && Owner.BlackBoard.DesiredTarget != null)
            {
                //if (Owner.debugAnims) Debug.Log(this.ToString() + " track " + Owner.BlackBoard.LookType.ToString() + " target: " + Owner.BlackBoard.DesiredTarget.ToString());


                dir = Owner.BlackBoard.DesiredTarget.Position - Owner.Transform.position;
                dir.y = 0;
                dir.Normalize();


                //Debug.DrawLine(OwnerTransform.position + Vector3.up, OwnerTransform.position + Vector3.up + dir * 3);

                FinalRotation.SetLookRotation(dir);
            }


            RotationProgress += Time.deltaTime * Owner.BlackBoard.RotationSmooth;
            RotationProgress = Mathf.Min(RotationProgress, 1);
            Quaternion q = Quaternion.Slerp(StartRotation, FinalRotation, RotationProgress);
            Owner.Transform.rotation = q;

            /*if (Quaternion.Angle(q, FinalRotation) > 40.0f)
                return;*/

            // Smooth the speed based on the current target direction
            float curSmooth = Owner.BlackBoard.SpeedSmooth * Time.deltaTime;

            Owner.BlackBoard.Speed = Mathf.Lerp(Owner.BlackBoard.Speed, MaxSpeed, curSmooth);

            dir = Action.FinalPosition - Transform.position;
            dir.y = 0;
            dir.Normalize();
            Owner.BlackBoard.MoveDir = dir;

            // MOVE
            if (Move(Owner.BlackBoard.MoveDir * Owner.BlackBoard.Speed * Time.deltaTime) == false)
            {
                Release();
            }
            else if ((Action.FinalPosition - Transform.position).sqrMagnitude < 0.3f * 0.3f)
            {
                Release();
            }
            else
            {
                E_MotionType motion = GetMotionType();

                if (motion != Owner.BlackBoard.MotionType)
                    PlayAnim(motion);
            }

        }

        override public bool HandleNewAction(AgentAction action)
        {
            if (action is AgentActionGoTo)
            {
                if (Action != null)
                    Action.SetSuccess();

                SetFinished(false); // just for sure, if we already finish in same tick

                Initialize(action);

                return true;
            }

            if (action is AgentActionWeaponShow)
            {
                action.SetSuccess();

                //Owner.ShowWeapon((action as AgentActionWeaponShow).Show, 0);

                PlayAnim(GetMotionType());
                return true;
            }
            return false;
        }

        private void PlayAnim(E_MotionType motion)
        {
            Owner.BlackBoard.MotionType = motion;
            
            AnimName = Owner.AnimSet.GetMoveAnim(Owner.BlackBoard.MotionType, E_MoveType.Forward, Owner.BlackBoard.WeaponSelected, Owner.BlackBoard.WeaponState);

            CrossFade(AnimName, 200);

        }

        private E_MotionType GetMotionType()
        {
            if (Owner.BlackBoard.Speed > (int)Owner.BlackBoard.MaxRunSpeed * 1.5f)
                return E_MotionType.Sprint;
            else if (Owner.BlackBoard.Speed > (int)Owner.BlackBoard.MaxWalkSpeed * 1.5f)
                return E_MotionType.Run;

            return E_MotionType.Walk;
        }

        protected override void Initialize(AgentAction action)
        {
            base.Initialize(action);

            Action = action as AgentActionGoTo;

            WVec dir;

            if (Owner.BlackBoard.LookType == E_LookType.TrackTarget && Owner.BlackBoard.DesiredTarget != null)
            {
                dir = Owner.BlackBoard.DesiredTarget.Position - Owner.Position;
            }
            else
            {
                dir = Action.FinalPosition - Owner.Position;
      
            }
            
            StartRotation = WRot.FromFacing(Owner.Forward.Yaw.Facing);

            if (dir != WVec.Zero)
                FinalRotation = WRot.FromFacing(dir.Yaw.Facing);

            Owner.BlackBoard.MotionType = GetMotionType();

            if (Action.Motion == E_MotionType.Sprint)
            {
                MaxSpeed = Owner.BlackBoard.MaxSprintSpeed;

            }
            else if (Action.Motion == E_MotionType.Run)
                MaxSpeed = Owner.BlackBoard.MaxRunSpeed;
            else
                MaxSpeed = Owner.BlackBoard.MaxWalkSpeed;

            RotationProgress = 0;
        }
    }
}
