﻿using MU.Network.CashShop;
using System;
using System.Collections.Generic;
using System.Text;
using WebZen.Handlers;

namespace MuEmu.Network
{
    public class CashShopServices : MessageHandler
    {
        [MessageHandler(typeof(CCashOpen))]
        public void CashOpen(GSSession session)
        {
            var result = !session.Player.Character.CashShop.IsOpen && session.Player.Window == null;
            session.SendAsync(new SCashOpen { Result = (byte)(result?1:0) }).Wait();

            session.Player.Character.CashShop.IsOpen = result;
        }
      
        [MessageHandler(typeof(CCashInventoryItem))]
        public void CashInventoryItem(GSSession session, CCashInventoryItem message)
        {
            session.Player.Character.CashShop.SendInventory(message);
        }
        [MessageHandler(typeof(CCashItemBuy))]
        public void CashItemBuy(GSSession session, CCashItemBuy message)
        {
            session.Player.Character.CashShop.BuyItem(message);
        }
    }
}
