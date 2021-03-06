﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Engine;
using Engine.Maps;
using Engine.Network.Defaults.ServerTraits;
using OAUnityLayer;
using UnityEngine;
using UnityEditor;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class CrateCfg
{
    [MenuItem("Configs/Create Map Config")]
    public static void CrateMapConfig()
    {
        Map map1 = new Map();

        List<PlayerReference> players = new List<PlayerReference>();
        map1.Players = players;
        players.Add(new PlayerReference
        {
            Name = "France",
            Faction = "allies",
            Allies =new string[] { "Germany" } ,
            Enemies = new string[] { "USSR" },
        });
        players.Add(new PlayerReference
        {
            Name = "Germany",
            Faction = "allies",
            Allies = new string[] { "France" },
            NonCombatant = true,
            Enemies = new string[] { "USSR" },
        });
        players.Add(new PlayerReference
        {
            Name = "USSR",
            Playable = true,
            AllowBots = false,
            Required = true,
            LockFaction = true,
            LockSpawn = true,
            LockTeam = true,
            Faction = "soviet",
            Enemies = new string[] { "France","Germany" },
        });
        players.Add(new PlayerReference
        {
            Name = "Neutral",
            OwnsWorld = true,
            NonCombatant = true,
            Faction = "allies"
        });


        List<ActorReference> actors = new List<ActorReference>();
        map1.Actors = actors;
        actors.Add(new ActorReference
        {
            ActorTypeName = "wood",
            InitInfo = new ActorInitInfo()
            {
                Location = new LocationInit()
                {
                    X = -30,
                    Y = 0,
                },
                Owner = new OwnerInit()
                {
                    PlayerName = "Germany",
                }
            },

        });
        actors.Add(new ActorReference
        {
            ActorTypeName = "wood",
            InitInfo = new ActorInitInfo()
            {
                Location = new LocationInit()
                {
                    X = -15,
                    Y = 0,
                },
                Owner = new OwnerInit()
                {
                    PlayerName = "USSR",
                }
            },

        });
        actors.Add(new ActorReference
        {
            ActorTypeName = "wood",
            InitInfo = new ActorInitInfo()
            {
                Location = new LocationInit()
                {
                    X = 15,
                    Y = 0,
                },
                Owner = new OwnerInit()
                {
                    PlayerName = "Neutral",
                }
            },

        });
        actors.Add(new ActorReference
        {
            ActorTypeName = "wood",
            InitInfo = new ActorInitInfo()
            {
                Location = new LocationInit()
                {
                    X = 30,
                    Y = 0,
                },
                Owner = new OwnerInit()
                {
                    PlayerName = "France",
                }
            },

        });

        var serializer = new SerializerBuilder().Build();
        var yaml = serializer.Serialize(map1);

        PlatformInfo platformInfo = new PlatformInfo();
        platformInfo.GatherInfomation();

        Platform.SetCurrentPlatform(platformInfo);

        string dir = Platform.SupportDir + @"/Maps/fight0/";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        using (StreamWriter sw = new StreamWriter(File.OpenWrite(dir+"map.yaml"), Encoding.UTF8))
        {
            sw.Write(yaml);
        }
        
        Debug.Log("Test write map config successful!");
        
    }

    [MenuItem("Configs/Create Setting Config")]
    public static void CrateSettingConfig()
    {
        Settings setting = new Settings();

        ServerSettings server = new ServerSettings();
        setting.Server = server;
        server.AllowPortForward = false;
        

        Engine.PlayerSettings player = new Engine.PlayerSettings();
        setting.Player = player;
        
        DebugSettings debug = new DebugSettings();
        setting.Debug = debug;
        debug.BotDebug = true;
        debug.LuaDebug = true;
        debug.PerfText = true;
        debug.PerfGraph = true;
        debug.SanityCheckUnsyncedCode = false;
        debug.EnableDebugCommandsInReplays = true;

        var serializer = new SerializerBuilder().Build();
        var yaml = serializer.Serialize(setting);

        PlatformInfo platformInfo = new PlatformInfo();
        platformInfo.GatherInfomation();

        Platform.SetCurrentPlatform(platformInfo);

        string dir = Platform.SupportDir + @"/";
        //if (!Directory.Exists(dir))
        //{
        //    Directory.CreateDirectory(dir);
        //}
        //using (StreamWriter sw = new StreamWriter(File.OpenWrite(dir + "settings.yaml"), Encoding.UTF8))
        //{
        //    sw.Write(yaml);
        //}

        string path = dir + "settings.yaml";

        Settings setting1 = null;
        using (Stream stream = File.OpenRead(path))
        {
            var input = File.ReadAllText(path, Encoding.UTF8);

            //var deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties()
            //    .WithNamingConvention(new CamelCaseNamingConvention())
            //    .Build();
            var deserializer = new DeserializerBuilder().WithNamingConvention(new NullNamingConvention())
                .Build();

            setting1 = deserializer.Deserialize<Settings>(input);
        }
        Debug.Log("Player name ->"+setting1.Player.Name);
        Debug.Log("Test write setting config successful!");
    }

    [MenuItem("Configs/Create manifest Config")]
    public static void CrateManifestConfig()
    {
        Manifest manifest = new Manifest();

        ModMetadata metaData = new ModMetadata();
        metaData.Title = "Red Alert";
        metaData.Version = "{DEV_VERSION}";
        manifest.Metadata = metaData;


        manifest.ServerTraits = new string[]
        {
            typeof(LobbyCommands).FullName,
            typeof(LobbySettingsNotification).FullName,
            typeof(MasterServerPinger).FullName,
            typeof(PlayerPinger).FullName
        };

        PlatformInfo platformInfo = new PlatformInfo();
        platformInfo.GatherInfomation();

        Platform.SetCurrentPlatform(platformInfo);
        string dir = Platform.platformInfo.GameContentsDir + @"/mods/ra/";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        string path = dir + "mod.yaml";
        var serializer = new SerializerBuilder().Build();
        var yaml = serializer.Serialize(manifest);

        using (StreamWriter sw = new StreamWriter(File.OpenWrite(path), Encoding.UTF8))
        {
            sw.Write(yaml);
        }
        Debug.Log("Write mod.yaml successful!");
    }

}
