﻿using MuEmu.Network.Data;
using MU.Network.Game;
using MuEmu.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MU.Resources;
using MU.Network;

namespace MuEmu
{
    internal class PartyMatching
    {
        public Player Player { get; set; }
        public string Text { get; set; }
        public string Password { get; set; }
        public bool AutAccept { get; set; }
        public ushort MinLevel { get; set; }
        public ushort MaxLevel { get; set; }
        public bool EnergyElf { get; set; }
        public List<Player> Waiting { get; set; } = new List<Player>();

        public bool CanJoin(Character @char)
        {
            if (@char.Level < MinLevel || @char.Level > MaxLevel)
                return false;

            if (Player.Character.Party != null && Player.Character.Party.Count >= 5)
                return false;

            return true;
        }

        internal int TryJoin(Player plr, string password)
        {
            if (Password != password)
                return -1;

            if (AutAccept)
            {
                PartyManager.CreateLink(Player, plr);
            }
            else
            {
                Waiting.Add(plr);
                _ = Player.Session.SendAsync(new SPartyMJoinNotify());
            }

            return 0;
        }
    }
    public class PartyManager
    {
        private static PartyManager _instance;
        private List<Party> _parties = new List<Party>();
        private List<PartyMatching> _partyMatchings = new List<PartyMatching>();
        public static ushort MaxLevelDiff { get; private set; }

        public static void Initialzie(ushort maxLevelDiff)
        {
            if(_instance == null)
                _instance = new PartyManager();

            MaxLevelDiff = maxLevelDiff;
        }

        public static PartyResults CreateLink(Player master, Player member)
        {
            if(Math.Abs(master.Character.Level - member.Character.Level) > MaxLevelDiff)
            {
                return PartyResults.RestrictedLevel;
            }

            if(member.Character.Party != null)
            {
                return PartyResults.InAnotherParty;
            }

            var party = master.Character.Party;

            if (party == null)
            {
                party = new Party(master, member);
                _instance._parties.Add(party);
                SendAll(party);
                return PartyResults.Success;
            }

            if(party.Count == 5)
            {
                return PartyResults.Fail;
            }

            party.Add(member);
            SendAll(party);
            return PartyResults.Success;
        }

        public static void SendAll(Party party)
        {
            if (party == null)
                return;

            var members = party.Members.Select(x => x.Session);
            var basetype = typeof(PartyDto);

            var list = party.List(basetype);
            var msg = VersionSelector.CreateMessage<SPartyList>(list);
            members.SendAsync(msg);
        }

        public static void Remove(Player plr)
        {
            var party = plr.Character.Party;
            if (party == null)
                return;

            party.Remove(plr);
            if(party.Count == 1)
            {
                party.Close();
                _instance._parties.Remove(party);
                return;
            }

            SendAll(party);
        }

        internal static void CreateMatching(Player plr, string text, string password, bool autAccept, ushort minLevel, ushort maxLevel, bool energyElf)
        {
            _instance._partyMatchings.Add(new PartyMatching
            {
                AutAccept = autAccept,
                EnergyElf = energyElf,
                MaxLevel = maxLevel,
                MinLevel = minLevel,
                Password = password,
                Player = plr,
                Text = text
            });
        }

        internal static bool ExistsMatching(Player plr)
        {
            return _instance._partyMatchings.Any(x => x.Player == plr);
        }

        internal static List<PartyMatching> GetMatchings()
        {
            return _instance._partyMatchings;
        }

        internal static int CancelMatching(Player player)
        {
            try
            {
                if (!_instance._partyMatchings.Any(x => x.Player == player))
                {
                    var matching = _instance._partyMatchings.First(x => x.Waiting.Any(y => y == player));
                    matching.Waiting.Remove(player);
                    _=matching.Player.Session.SendAsync(new SPartyMJoinNotify());
                }
                else
                {
                    var matching = _instance._partyMatchings.First(x => x.Player == player);
                    _instance._partyMatchings.Remove(matching);
                }
                return 0;
            }catch(Exception)
            {
                return -1;
            }
        }
    }

    public class Party
    {
        List<Player> _members;

        public ushort MaxLevel => _members.Max(x => x.Character.Level);

        public Player Master => _members.First();
        public int Count => _members.Count();

        public IEnumerable<Player> Members => _members;

        internal Party(Player plr, Player memb)
        {
            _members = new List<Player>
            {
                plr,
                memb,
            };

            plr.Character.Party = this;
            memb.Character.Party = this;

            LifeUpdate();
        }

        internal bool Any(Player plr)
        {
            return _members.Any(x => x == plr);
        }

        internal bool Add(Player plr)
        {
            if (_members.Count == 5)
                return false;

            _members.Add(plr);
            LifeUpdate();
            return true;
        }

        internal bool Remove(Player plr)
        {
            if (!Any(plr))
                return false;

            _members.Remove(plr);
            plr.Character.Party = null;
            plr.Session.SendAsync(new SPartyDelUser()).Wait();
            LifeUpdate();

            return true;
        }

        internal void Close()
        {
            var del = new SPartyDelUser();
            foreach (var memb in Members)
            {
                memb.Session.SendAsync(del).Wait();
                memb.Character.Party = null;
            }

            _members.Clear();
        }

        public IEnumerable<IPartyDto> List(Type baseType)
        {
            byte i = 0;
            foreach(var m in _members)
            {
                var @char = m.Character;
                var obj = Activator.CreateInstance(baseType) as IPartyDto;
                obj.Set(i++,
                    @char.Name,
                    @char.MapID,
                    (byte)@char.Position.X,
                    (byte)@char.Position.Y,
                    (int)@char.Health,
                    (int)@char.MaxHealth,
                    (int)@char.Mana,
                    (int)@char.MaxMana,
                    Program.ServerCode + 1,
                    0);
                yield return obj;
            }
        }

        public async void LifeUpdate()
        {
            var msg = new SPartyLifeAll();

                msg.PartyLives = _members.Select(x => new SPartyLife
                {
                    Name = x.Character.Name,
                    Life = (byte)((float)x.Character.Health / (float)x.Character.MaxHealth * 255.0f),
                    Mana = (byte)((float)x.Character.Mana / (float)x.Character.MaxMana * 255.0f),
                }).ToArray();

            await _members
                .Select(x => x.Session)
                .SendAsync(msg);
        }

        public async void ExpDivision(ushort TargetID, float EXP, Player killer, ushort dmg)
        {
            EXP *= _members.Count * 0.01f + 1.0f;
            var totalLevel = _members.Sum(x => x.Character.Level);

            foreach(var plr in _members)
            {
                var subEXP = (long)(EXP * plr.Character.Level / totalLevel);
                plr.Character.Experience += subEXP;
                await plr.Session.SendAsync(new SKillPlayer(TargetID, (ushort)subEXP, killer == plr ? dmg : (ushort)0));
            }
        }
    }
}
