﻿using MU.Network.Game;
using MU.Network;
using MU.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebZen.Handlers;
using MuEmu.Util;
using Google.Protobuf.Collections;

namespace MuEmu.Network.GameServices
{
    public partial class GameServices : MessageHandler
    {
        [MessageHandler(typeof(CPShopSetItemPrice))]
        public async Task CPShopSetItemPrice(GSSession session, CPShopSetItemPrice message)
        {
            var @char = session.Player.Character;
            if (@char.Level < 6)
            {
                await session.SendAsync(new SPShopSetItemPrice(PShopResult.LevelTooLow, message.Position));
                return;
            }

            if (message.Price == 0)
            {
                await session.SendAsync(new SPShopSetItemPrice(PShopResult.InvalidPrice, message.Position));
                return;
            }

            var item = @char.Inventory.Get(message.Position);

            if (item == null)
            {
                await session.SendAsync(new SPShopSetItemPrice(PShopResult.InvalidItem, message.Position));
                return;
            }

            item.PShopValueZ = message.Price;
            item.PShopValueB = message.JewelOfBlessPrice;
            item.PShopValueS = message.JewelOfSoulPrice;
            item.PShopValueC = message.JewelOfChaosPrice;

            Logger.ForAccount(session).Information("Update price for {0}, {1}Zen, {2}Bless, {3}Soul, {4}Chaos", item, item.PShopValueZ, item.PShopValueB, item.PShopValueS, item.PShopValueC);
            await session.SendAsync(new SPShopSetItemPrice(PShopResult.Success, message.Position));
        }

        [MessageHandler(typeof(CPShopRequestOpen))]
        public async Task CPShopRequestOpen(GSSession session, CPShopRequestOpen message)
        {
            var @char = session.Player.Character;
            var log = Logger.ForAccount(session);

            if (@char.Map.IsEvent)
            {
                log.Error("Try to open PShop on Event map");
                await session.SendAsync(new SPShopRequestOpen(PShopResult.Disabled));
                return;
            }

            if (@char.Level < 6)
            {
                log.Error("Character Level Too Low ");
                await session.SendAsync(new SPShopRequestOpen(PShopResult.LevelTooLow));
                return;
            }

            if (!@char.Shop.Open)
            {
                log.Information("PShop:{0} Open", message.Name);
                @char.Shop.Name = message.Name;
                @char.Shop.Open = true;
                await session.SendAsync(new SPShopRequestOpen(PShopResult.Success));
                return;
            }

            await session.SendAsync(new SPShopRequestOpen(PShopResult.Disabled));
        }

        [MessageHandler(typeof(CPShopRequestClose))]
        public async Task CPShopRequestClose(GSSession session)
        {
            var @char = session.Player.Character;
            var log = Logger.ForAccount(session);

            if (@char == null)
                return;

            if (!@char.Shop.Open)
            {
                log.Error("PShop isn't open");
            }
            else
            {
                @char.Shop.Open = false;
                log.Error("PShop {0} Closed", @char.Shop.Name);
            }
            var msg = new SPShopRequestClose(PShopResult.Success, (ushort)session.ID);
            await session.SendAsync(msg);
            @char.SendV2Message(msg);
        }

        [MessageHandler(typeof(CPShopRequestList))]
        public async Task CPShopRequestList(GSSession session, CPShopRequestList message)
        {
            var seller = Program.server.Clients.FirstOrDefault(x => x.ID == message.Number);

            if (seller == null)
            {
                await session.SendAsync(new SPShopRequestList(PShopResult.InvalidPosition));
                return;
            }

            if (seller == session)
            {
                await session.SendAsync(new SPShopRequestList(PShopResult.Disabled));
                return;
            }

            await session.SendAsync(new SPShopAlterVault { type = 0 });
            var msg = VersionSelector.CreateMessage<SPShopRequestList>(PShopResult.Success, message.Number, seller.Player.Character.Name, seller.Player.Character.Shop.Name);
            await session.SendAsync(msg);
            session.Player.Window = seller;
            return;
        }

        [MessageHandler(typeof(CPShopCloseDeal))]
        public async Task CPShopCloseDeal(GSSession session, CPShopCloseDeal message)
        {
            var log = Logger.ForAccount(session);
            var seller = session.Player.Window as GSSession;
            session.Player.Window = null;
            if (seller != null)
            {
                log.Information("Close deal with {0}", seller.Player.Character);
            }
            else
            {
                log.Information("Close deal with {0}", message.Name);
            }
        }

        [MessageHandler(typeof(CPShopRequestBuy))]
        public async Task CPShopRequestBuy(GSSession session, CPShopRequestBuy message)
        {
            var seller = Program.server.Clients.FirstOrDefault(x => x.ID == message.Number);

            if (seller == null)
            {
                await session.SendAsync(new SPShopRequestBuy(PShopResult.InvalidPosition));
                return;
            }

            if (seller == session)
            {
                await session.SendAsync(new SPShopRequestBuy(PShopResult.Disabled));
                return;
            }

            var @char = seller.Player.Character;
            if (!@char.Shop.Open)
            {
                await session.SendAsync(new SPShopRequestBuy(PShopResult.Disabled));
                return;
            }

            var item = @char.Inventory.PersonalShop.Get(message.Position);

            if (item == null)
            {
                await session.SendAsync(new SPShopRequestBuy(PShopResult.InvalidItem));
                return;
            }

            if (item.PShopValueZ > session.Player.Character.Money)
            {
                await session.SendAsync(new SPShopRequestBuy(PShopResult.LackOfZen));
                return;
            }

            if (@char.Money + item.PShopValueZ > uint.MaxValue)
            {
                await session.SendAsync(new SPShopRequestBuy(PShopResult.ExceedingZen));
                return;
            }

            var Bless = session.Player.Character.Inventory.FindAllItems(7181);
            var BlessC = session.Player.Character.Inventory.FindAllItems(6174);
            var Bless10Pack = BlessC.Where(x => x.Plus == 0);
            var Bless20Pack = BlessC.Where(x => x.Plus == 1);
            var Bless30Pack = BlessC.Where(x => x.Plus == 2);
            var Soul = session.Player.Character.Inventory.FindAllItems(7182);
            var SoulC = session.Player.Character.Inventory.FindAllItems(6175);
            var Soul10Pack = SoulC.Where(x => x.Plus == 0);
            var Soul20Pack = SoulC.Where(x => x.Plus == 1);
            var Soul30Pack = SoulC.Where(x => x.Plus == 2);
            var Chaos = session.Player.Character.Inventory.FindAllItems(6159);
            var ChaosC = session.Player.Character.Inventory.FindAllItems(6285);
            var Chaos10Pack = ChaosC.Where(x => x.Plus == 0);
            var Chaos20Pack = ChaosC.Where(x => x.Plus == 1);
            var Chaos30Pack = ChaosC.Where(x => x.Plus == 2);

            if (item.PShopValueB > Bless.Count() + Bless30Pack.Count() * 30 + Bless20Pack.Count() * 20 + Bless10Pack.Count() * 10)
            {
                await session.SendAsync(new SPShopRequestBuy(PShopResult.LackOfBless));
                return;
            }

            if (item.PShopValueS > Soul.Count() + Soul30Pack.Count() * 30 + Soul20Pack.Count() * 20 + Soul10Pack.Count() * 10)
            {
                await session.SendAsync(new SPShopRequestBuy(PShopResult.LackOfSoul));
                return;
            }

            if (item.PShopValueC > Chaos.Count() + Chaos30Pack.Count() * 30 + Chaos20Pack.Count() * 20 + Chaos10Pack.Count() * 10)
            {
                await session.SendAsync(new SPShopRequestBuy(PShopResult.LackOfChaos));
                return;
            }

            int jewel = item.PShopValueB;
            var pickB30 = Math.Min(jewel / 30, Bless30Pack.Count());
            jewel -= pickB30 * 30;
            var pickB20 = Math.Min(jewel / 20, Bless20Pack.Count());
            jewel -= pickB20 * 20;
            var pickB10 = Math.Min(jewel / 10, Bless10Pack.Count());
            jewel -= pickB10 * 10;
            var pickB1 = jewel;

            jewel = item.PShopValueS;
            var pickS30 = Math.Min(jewel / 30, Soul30Pack.Count());
            jewel -= pickS30 * 30;
            var pickS20 = Math.Min(jewel / 20, Soul20Pack.Count());
            jewel -= pickS20 * 20;
            var pickS10 = Math.Min(jewel / 10, Soul10Pack.Count());
            jewel -= pickS10 * 10;
            var pickS1 = jewel;

            jewel = item.PShopValueC;
            var pickC30 = Math.Min(jewel / 30, Chaos30Pack.Count());
            jewel -= pickC30 * 30;
            var pickC20 = Math.Min(jewel / 20, Chaos20Pack.Count());
            jewel -= pickC20 * 20;
            var pickC10 = Math.Min(jewel / 10, Chaos10Pack.Count());
            jewel -= pickC10 * 10;
            var pickC1 = jewel;

            var transfer = Bless30Pack.Take(pickB30).ToList();
            transfer.AddRange(Bless20Pack.Take(pickB20));
            transfer.AddRange(Bless10Pack.Take(pickB10));
            transfer.AddRange(Bless.Take(pickB1));

            transfer.AddRange(Soul30Pack.Take(pickS30));
            transfer.AddRange(Soul20Pack.Take(pickS20));
            transfer.AddRange(Soul10Pack.Take(pickS10));
            transfer.AddRange(Soul.Take(pickS1));

            transfer.AddRange(Chaos30Pack.Take(pickC30));
            transfer.AddRange(Chaos20Pack.Take(pickC20));
            transfer.AddRange(Chaos10Pack.Take(pickC10));
            transfer.AddRange(Chaos.Take(pickC1));

            if (!@char.Inventory.TryAdd(transfer))
            {
                await session.SendAsync(new SPShopRequestBuy(PShopResult.SellerInventoryFull));
                return;
            }

            if (!session.Player.Character.Inventory.TryAdd(new[] { item }))
            {
                await session.SendAsync(new SPShopRequestBuy(PShopResult.InvalidPosition));
                return;
            }

            session.Player.Character.Inventory.Remove(transfer);
            @char.Inventory.Add(transfer);

            await @char.Inventory.Remove(message.Position, true);
            var res = session.Player.Character.Inventory.Add(item);

            session.Player.Character.Money -= item.PShopValueZ;
            @char.Money += item.PShopValueZ;

            await session.SendAsync(new SPShopRequestBuy(PShopResult.Success, message.Number, item.GetBytes(), res));
            await @char.Player.Session.SendAsync(new SPShopRequestSold(PShopResult.Success, message.Position, session.Player.Character.Name));

            if (@char.Inventory.PersonalShop.Items.Count == 0)
            {
                await CPShopRequestClose(@char.Player.Session);
            }
        }

        [MessageHandler(typeof(CPShopSearchItem))]
        public async Task CPShopSearchItem(GSSession session, CPShopSearchItem message)
        {
            IEnumerable<PShop> shopList;

            shopList = from cl in Program.server.Clients
                       where
                       cl.Player != null &&
                       cl.Player.Status == LoginStatus.Playing &&
                       cl.Player.Character.Shop.Open == true &&
                       (cl.Player.Character.MapID == Maps.Lorencia || cl.Player.Character.MapID == Maps.Davias || cl.Player.Character.MapID == Maps.Noria || cl.Player.Character.MapID == Maps.Elbeland || cl.Player.Character.MapID == Maps.LorenMarket)
                       select cl.Player.Character.Shop;

            if (message.sSearchItem != -1)
            {
                shopList = from cl in shopList
                           where
                           cl.Chararacter.Inventory.PersonalShop.Items.Values.Count(x => x.Number.Number == (ushort)message.sSearchItem) != 0
                           select cl;
            }

            shopList = shopList.Skip(message.iLastCount).Take(50);
            var msg = new SPShopSearchItem
            {
                iPShopCnt = shopList.Count(),
                btContinueFlag = (byte)(shopList.Count() == 50 ? 1 : 0),
                List = shopList.Select(x => new SPShopSearchItemDto
                {
                    Number = x.Chararacter.Player.ID,
                    szName = x.Chararacter.Name,
                    szPShopText = x.Name,
                }).ToArray()
            };
            await session.SendAsync(msg);
        }
 
        [MessageHandler(typeof(CPShopItemSearch))]
        public async Task CPShopItemSearch(GSSession session, CPShopItemSearch message)
        {
            Logger.Debug(nameof(CPShopItemSearch));
            await CPShopItemSearch(session, new CPShopItemSearch {  Number = message.Number });
        } 
    }
}
