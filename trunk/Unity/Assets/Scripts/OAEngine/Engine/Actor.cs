﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ComponentAnim;
using Engine.ComponentAnim.Core;
using Engine.ComponentsAI;
using Engine.ComponentsAI.ComponentPlayer;
using Engine.Interfaces;
using Engine.Network.Defaults;
using Engine.Network.Interfaces;
using Engine.Primitives;
using Engine.Support;
using TrueSync;

namespace Engine
{
    public class Actor
    {

        public E_ComboLevel[] ComboLevel = { E_ComboLevel.One, E_ComboLevel.One, E_ComboLevel.Two, E_ComboLevel.Two, E_ComboLevel.Two, E_ComboLevel.Three };

        public E_SwordLevel SwordLevel = E_SwordLevel.Four;

        public TSVector2 Pos;

        public int MoveSpeed = 3000;

        public FP Facing = 90;

        public FP TurnSpeed = 4;

        public readonly World World;

        public IRender render;

        public readonly uint ActorID;

        private ComponentPlayer ComponentPlayer;
        private AnimComponent AnimComponent;

        private PlayerAgent Agent;

        public PlayerAgent agent
        {
            get { return this.Agent; }
        }

        public PlayerSensorEyes SensorEyes;

        private AnimSet AnimSet;

        public MersenneTwister Random {
            get { return this.World.SharedRandom; }
        }

        public TSVector2 CurJoystickDir { private set; get; }

        public Actor(World world, int clientIdx ,string name, TypeDictionary initDict)
        {
            this.World = world;
            ActorID = world.NextAID();

            //TODO:
            this.render = Platform.platformInfo.actorRendererFactory.CreateActorRenderer(WPos.Zero,clientIdx,"Player");

            
            
        }

        public void Init()
        {
            this.AnimSet = new AnimSetPlayer();
            this.AnimSet.Init();
            this.Agent = new PlayerAgent(this,this.render,this.AnimSet);
            this.Agent.Init();
            this.SensorEyes = new PlayerSensorEyes(this.agent);


            Animation anim = new Animation(this.render);
            anim.Init();
            this.AnimComponent = new AnimComponent(this.Agent,anim);
            this.AnimComponent.Init();
            this.ComponentPlayer = new ComponentPlayer(this.Agent);
            this.ComponentPlayer.Init((AnimSetPlayer)this.AnimSet);
        }


        public void Tick()
        {
            this.SensorEyes.Tick();
            this.Agent.Tick();
            this.AnimComponent.Update();
            this.ComponentPlayer.Update();
            this.SetTransform();
        }

        private void SetTransform()
        {
            this.Pos = this.agent.Position;
            this.Facing = this.agent.Facing;
        }

        //public void SetMoveDir(byte angle)
        //{
        //    this.Facing = angle;
        //    //this.Pos += this.MoveSpeed*new CVec();
        //}

        public void ProcessOrder(IOrder order)
        {
            Order _order = order as Order;
            E_OpType opType = (E_OpType)_order.OpCode;
            
            switch (opType)
            {
                case E_OpType.X:
                    this.ComponentPlayer.CreateOrderAttack(E_AttackType.X);
                    break;
                case E_OpType.O:
                    this.ComponentPlayer.CreateOrderAttack(E_AttackType.O);
                    break;
                case E_OpType.Dodge:
                    this.ComponentPlayer.CreateOrderDodge();
                    break;
                case E_OpType.Joystick:
                    ushort data = _order.OpData;
                    int angle = ((data & 0xFF00) >> 8);
                    int force = data & 0x00ff;

                    if (force == 0)
                    {
                        this.CurJoystickDir = TSVector2.zero;
                        this.ComponentPlayer.CreateOrderStop();
                        //Log.Write("wyb","Joystick end!");
                    }
                    else
                    {
                        FP MoveSpeedModifier = new FP(force) / 256;
                        FP rad = angle * new FP(360) * TSMath.Deg2Rad / new FP(256);
                        this.CurJoystickDir = new TSVector2(TSMath.Cos(rad), TSMath.Sin(rad));
                        this.CurJoystickDir.Normalize();
                        this.ComponentPlayer.CreateOrderGoTo(MoveSpeedModifier,angle);
                        //this.SetMoveDir(angle);
                        //Log.Write("wyb", "Joystick angle->" + angle+" force->"+force);
                    }
                    
                    break;
                default:
                    break;
            }
        }


        public void RenderSelf(IWorldRenderer worldRenderer)
        {
            if (this.render != null)
            {
                this.render.Render(this,worldRenderer);
            }
        }

    }
}
