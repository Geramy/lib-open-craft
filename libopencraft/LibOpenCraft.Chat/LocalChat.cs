﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

using LibOpenCraft.ServerPackets;

namespace LibOpenCraft.Chat
{
    [Export(typeof(CoreModule))]
    [ExportMetadata("Name", "LocalChat")]
    public class LocalChat : CoreModule
    {
        string name = "";
        public LocalChat()
            : base()
        {
           
        }

        public override void Start()
        {
            base.Start();
            ModuleHandler.AddEventModule(PacketType.ChatMessage, new ModuleCallback(OnChatMessage));
            base.RunModuleCache();
        }

        public void OnChatMessage(ref PacketReader _pReader, PacketType pt, ref ClientManager _client)
        {
            string message = _pReader.ReadString();
            ChatMessagePacket ChatMessage = new ChatMessagePacket(PacketType.ChatMessage);
            ChatMessage.MessageRecieved = message;
            if (message[0] == '/' && (bool)Config.Configuration["EnableEmbeddedChatCommands"])
            {
                string command = message.Substring(1, message.Length - 1).ToLower();
                switch (command)
                {
                    case "save":
                        ChatMessage.MessageSent = _client._player.name + ": " + "World Saved....";
                        World.SaveWorld();
                        break;
                    case "set -b 1":
                        break;
                    case "set -b 2":
                        break;
                    case "set -b 3":
                        break;
                    case "set -b 4":
                        break;
                }
                try
                {
                    int i = 0;
                    for (; i < base.ModuleAddons.Count; i++)
                    {
                        ChatMessage = (ChatMessagePacket)base.ModuleAddons.ElementAt(i).Value(pt, ModuleAddons.ElementAt(i).Key, ref _pReader, (PacketHandler)ChatMessage, ref _client);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: " + e.Message + " Source:" + e.Source + " Method:" + e.TargetSite + " Data:" + e.Data);
                }
                if (ChatMessage.MessageSent == "")
                {

                }
                else
                {
                    ChatMessage.BuildPacket();
                    _client.SendPacket((PacketHandler)ChatMessage, _client.id, ref _client, false, false);
                }
            }
            else
            {
                ChatMessage.MessageSent = _client._player.name + ": " + message;
                ChatMessage.BuildPacket();
                ClientManager[] player = GridServer.player_list;
                for (int i = 0; i < player.Length; i++)
                {
                    if (player[i] == null)
                    {

                    }
                    else
                    {
                        if (player[i] == null)
                        {

                        }
                        else
                        {
                            player[i].SendPacket((PacketHandler)ChatMessage, player[i].id, ref player[i], false, false);
                        }
                    }
                }
            }
        }

        public override void Stop()
        {
            base.Stop();
            ModuleHandler.RemoveEventModule(PacketType.ChatMessage);
        }

    }
}
