﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Interfaces;

namespace Engine.Support
{
    public interface IPlatformImpl
    {
        PlatformType currentPlatform { get; }

        /// <summary>
        /// 获取游戏资源目录
        /// </summary>
        string GameContentsDir { get; }

        /// <summary>
        /// 游戏逻辑循环
        /// </summary>
        Action<float> LogicTick { set; get; }

        /// <summary>
        /// 游戏循环
        /// </summary>
        Action<float> Tick { set; get; }

        /// <summary>
        /// 游戏退出回调
        /// </summary>
        Action OnApplicationQuit { set; get; }

        /// <summary>
        /// 设备日志输出器
        /// </summary>
        ILogger Logger { get; }

        /// <summary>
        /// 输入输出器
        /// </summary>
        IInputsGetter inputer { set; get; }


        IActorRendererFactory actorRendererFactory { set; get; }
    }
}
