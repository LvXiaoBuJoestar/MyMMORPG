﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Configuration;

using System.Threading;

using Network;
using GameServer.Services;
using GameServer.Managers;

namespace GameServer
{
    class GameServer
    {
        Thread thread;
        bool running = false;

        NetService netService;

        public bool Init()
        {
            netService = new NetService();
            netService.Init(8000);

            DBService.Instance.Init();
            UserService.Instance.Init();
            DataManager.Instance.Load();
            MapService.Instance.Init();
            BagService.Instance.Init();
            ItemService.Instance.Init();
            QuestService.Instance.Init();
            FriendService.Instance.Init();
            TeamService.Instance.Init();
            GuildService.Instance.Init();
            ChatService.Instance.Init();

            thread = new Thread(new ThreadStart(this.Update));
            return true;
        }

        public void Start()
        {
            netService.Start();

            running = true;
            thread.Start();
        }


        public void Stop()
        {
            running = false;
            thread.Join();

            netService?.Stop();
        }

        public void Update()
        {
            while (running)
            {
                var mapManager = MapManager.Instance;
                Time.Tick();
                Thread.Sleep(100);
                //Console.WriteLine("{0} {1} {2} {3} {4}", Time.deltaTime, Time.frameCount, Time.ticks, Time.time, Time.realtimeSinceStartup);
                mapManager.Update();
            }
        }
    }
}
