﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Network.Interfaces;
using Engine.Network.Server;
using Engine.Support;

namespace Engine.Network.Defaults.ServerTraits
{
    public class PlayerPinger : ServerTrait, ITick<ClientDefault>
    {
        static readonly int PingInterval = 5000; // Ping every 5 seconds
        static readonly int ConnReportInterval = 20000; // Report every 20 seconds
        static readonly int ConnTimeout = 60000; // Drop unresponsive clients after 60 seconds

        public int TickTimeout { get { return PingInterval * 100; } }

        long lastPing = 0;
        long lastConnReport = 0;
        bool isInitialPing = true;

        public PlayerPinger()
        {
        }

        public void Tick(IServer<ClientDefault> s)
        {
            ServerDefault server = s as ServerDefault;
            try
            {
                if ((Game.RunTime - lastPing > PingInterval) || isInitialPing)
                {
                    isInitialPing = false;
                    lastPing = Game.RunTime;

                    // Ignore client timeout in singleplayer games to make debugging easier
                    if (server.LobbyInfo.NonBotClients.Count() < 2 && !server.Dedicated)
                    {
                        foreach (var c in server.Conns.ToList())
                        {
                            server.SendOrderTo(c, "Ping", Game.RunTime.ToString());
                        }
                    }
                    else
                    {
                        foreach (var c in server.Conns.ToList())
                        {
                            if (c == null || c.Socket == null)
                                continue;

                            var client = server.GetClient(c);
                            if (client == null)
                            {
                                server.DropClient(c);
                                server.SendMessage("A player has been dropped after timing out.");
                                continue;
                            }

                            if (c.TimeSinceLastResponse < ConnTimeout)
                            {
                                server.SendOrderTo(c, "Ping", Game.RunTime.ToString());
                                if (!c.TimeoutMessageShown && c.TimeSinceLastResponse > PingInterval * 2)
                                {
                                    server.SendMessage(client.Name + " is experiencing connection problems.");
                                    c.TimeoutMessageShown = true;
                                }
                            }
                            else
                            {
                                server.SendMessage(client.Name + " has been dropped after timing out.");
                                server.DropClient(c);
                            }
                        }

                        if (Game.RunTime - lastConnReport > ConnReportInterval)
                        {
                            lastConnReport = Game.RunTime;

                            var timeouts = server.Conns
                                .Where(c => c.TimeSinceLastResponse > ConnReportInterval && c.TimeSinceLastResponse < ConnTimeout)
                                .OrderBy(c => c.TimeSinceLastResponse);

                            foreach (var c in timeouts)
                            {
                                if (c == null || c.Socket == null)
                                    continue;

                                var client = server.GetClient(c);
                                if (client != null)
                                    server.SendMessage("{0} will be dropped in {1} seconds.".F(client.Name, (ConnTimeout - c.TimeSinceLastResponse) / 1000));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write("wyb", "PlayerPinger error->{0} stackTrace->{1}".F(ex.Message,ex.StackTrace));
            }
            
        }
    }
}
