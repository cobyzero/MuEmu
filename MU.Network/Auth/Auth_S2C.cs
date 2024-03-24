using MU.DataBase;
using MU.Resources;
using MuEmu.Network.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using WebZen.Serialization;
using WebZen.Util;

namespace MU.Network.Auth
{
    [WZContract] // 0xC10CF10001IIIIVVVVVVVVVV
    public class SJoinResult : IAuthMessage
    {
        [WZMember(0)]
        public byte Result { get; set; }

        [WZMember(1)]
        public byte NumberH { get; set; }

        [WZMember(2)]
        public byte NumberL { get; set; }

        [WZMember(3, 5)]
        public byte[] ClientVersion { get; set; }

        public SJoinResult()
        {
            ClientVersion = Array.Empty<byte>();
        }

        public SJoinResult(byte result, int number, string clientVersion)
        {
            Result = result;
            NumberH = (byte)((number >> 8) & 0xff);
            NumberL = (byte)(number & 0xff);
            ClientVersion = clientVersion.GetBytes();
        }
    }
 

    [WZContract]
    public class SLoginResult : IAuthMessage
    {
        [WZMember(1)]
        public LoginResult Result { get; set; }

        public SLoginResult()
        { }

        public SLoginResult(LoginResult result)
        {
            Result = result;
        }
    }

    [WZContract]
    public class SLoginResultS17 : IAuthMessage
    {
        [WZMember(0)]
        public byte UnkBYTE { get; set; }

        [WZMember(1)]
        public LoginResult Result { get; set; }

        [WZMember(2, typeof(BinarySerializer), 0x6)]
        public byte[] Unk { get; set; } = Array.Empty<byte>();

        public SLoginResultS17()
        { }

        public SLoginResultS17(LoginResult result)
        {
            Result = result;
        }
    }
    public abstract class CharList
    {
        public CharList()
        {

        }

        public CharList(byte maxClas, byte moveCnt, byte CharSlotCount, byte WhSlotCount)
        {

        }

        public abstract CharList AddChar(byte id, CharacterDto @char, byte[] charSet, GuildStatus gStatus);
    }

    [WZContract]
    public class SCharacterList : CharList, IAuthMessage
    {
        [WZMember(0)]
        public byte MaxClass { get; set; }

        [WZMember(1)]
        public byte MoveCount { get; set; }

        //public byte Count { get; set; }

        [WZMember(2, SerializerType = typeof(ArrayWithScalarSerializer<byte>))]
        public CharacterPreviewDto[] CharacterList { get; set; }

        public SCharacterList()
        {
            CharacterList = Array.Empty<CharacterPreviewDto>();
        }

        public SCharacterList(byte maxClas, byte moveCnt, byte CharSlotCount, byte WhSlotCount) : base(maxClas, moveCnt, CharSlotCount, WhSlotCount)
        {
            MaxClass = maxClas;
            MoveCount = moveCnt;
            CharacterList = Array.Empty<CharacterPreviewDto>();
        }

        public override CharList AddChar(byte id, CharacterDto @char, byte[] charSet, GuildStatus gStatus)
        {
            var l = CharacterList.ToList();
            l.Add(new CharacterPreviewDto(
                id, @char.Name, @char.Level, (ControlCode)@char.CtlCode, charSet, gStatus
                ));
            CharacterList = l.ToArray();
            return this;
        }
    }
 
    [WZContract]
    public class SCharacterCreate : IAuthMessage
    {
        [WZMember(0)]
        public byte Result { get; set; } // 0: Error

        [WZMember(1, 10)]
        public byte[] btName { get; set; }

        [WZMember(2)]
        public byte Position { get; set; }

        [WZMember(3)]
        public ushort Level { get; set; }

        [WZMember(4)]
        public byte Class { get; set; }

        [WZMember(5, typeof(ArraySerializer))]
        public byte[] Equipament { get; set; }

        public SCharacterCreate()
        {
            btName = Array.Empty<byte>();
            Equipament = new byte[24];
        }

        public SCharacterCreate(byte result)
        {
            btName = Array.Empty<byte>();
            Equipament = new byte[24];

            Result = result;
        }

        public SCharacterCreate(byte result, string name, byte pos, ushort level, byte[] equip, byte @class)
        {
            btName = Array.Empty<byte>();
            Equipament = new byte[24];

            Result = result;
            Name = name;
            Position = pos;
            Level = level;
            Equipament = equip;
            Class = @class;
        }

        public string Name { get => btName.MakeString(); set => btName = value.GetBytes(); }
    }

    [WZContract]
    public class SCharacterDelete : IAuthMessage
    {
        [WZMember(0)]
        public CharacterDeleteResult Result { get; set; }
    }

    [WZContract]
    public class CharacterPreviewDto
    {
        [WZMember(0)] public byte ID { get; set; }//0

        [WZMember(1, 11)] public byte[] btName { get; set; }//1

        [WZMember(2)] public ushort Level { get; set; }//C,D

        [WZMember(3)] public ControlCode ControlCode { get; set; }//E

        [WZMember(4, 18)] public byte[] CharSet { get; set; }//F-20

        [WZMember(5)] public GuildStatus GuildStatus { get; set; }//21

        public CharacterPreviewDto() { }

        public CharacterPreviewDto(byte Id, string name, ushort level, ControlCode cc, byte[] charSet, GuildStatus gStatus)
        {
            ID = Id;
            btName = name.GetBytes();
            Level = level;
            ControlCode = cc;
            CharSet = charSet;
            GuildStatus = gStatus;
        }
    }
     
    [WZContract(Serialized = true)]
    public class SCharacterMapJoin : IAuthMessage
    {
        [WZMember(0, typeof(BinaryStringSerializer), 10)]
        public string Name { get; set; }

        [WZMember(1)]
        public byte Valid { get; set; }
    }

    [WZContract(Serialized = true)] // 0xC3
    public class SCharacterMapJoin2 : IAuthMessage
    {
        [WZMember(0)] public byte MapX { get; set; }//0
        [WZMember(1)] public byte MapY { get; set; }//1
        [WZMember(2)] public byte Map { get; set; }//2
        [WZMember(3)] public byte Direccion { get; set; }//3
        [WZMember(4)] public long Experience { get; set; }//4
        [WZMember(5)] public long NextExperience { get; set; }//c
        [WZMember(6)] public ushort LevelUpPoints { get; set; }//14
        [WZMember(7)] public ushort Str { get; set; }//16
        [WZMember(8)] public ushort Agi { get; set; }//18
        [WZMember(9)] public ushort Vit { get; set; }//1a
        [WZMember(10)] public ushort Ene { get; set; }//1c
        [WZMember(11)] public ushort Life { get; set; }//1e
        [WZMember(12)] public ushort MaxLife { get; set; }//20
        [WZMember(13)] public ushort Mana { get; set; }//22
        [WZMember(14)] public ushort MaxMana { get; set; }//24
        [WZMember(15)] public ushort Shield { get; set; }//26
        [WZMember(16)] public ushort MaxShield { get; set; }//28
        [WZMember(17)] public ushort Stamina { get; set; }//2a
        [WZMember(18)] public ushort MaxStamina { get; set; }//2c
        [WZMember(19)] public ushort unk { get; set; }
        [WZMember(20)] public uint Zen { get; set; }//2e
        [WZMember(21)] public byte PKLevel { get; set; }//36
        [WZMember(22)] public byte ControlCode { get; set; }//37
        [WZMember(23)] public short AddPoints { get; set; }//38
        [WZMember(24)] public short MaxAddPoints { get; set; }//3a 
        [WZMember(25)] public ushort Cmd { get; set; }//3c
        [WZMember(26)] public short MinusPoints { get; set; }//3e
        [WZMember(27)] public short MaxMinusPoints { get; set; }//40
        [WZMember(28)] public byte ExpandedInv { get; set; }//41
        [WZMember(29)] public uint Ruud { get; set; }//42
        //[WZMember(30)]
        //public byte ExpandedVault { get; set; }//44

        public SCharacterMapJoin2()
        {

        }

        public SCharacterMapJoin2(Maps mapID, byte x, byte y, byte direction, ushort strength, ushort agility, ushort vitality, ushort energy, ushort comm, long experience, long nextExperience, ushort hp, ushort hpMax, ushort mp, ushort mpMax, ushort shield, ushort maxShield, ushort bp, ushort bpMax, byte pk, short addPoints, short maxAddPoints, short minusPoints, short maxMinusPoints, ushort levelUpPoints, byte expandedInventory, uint Money, uint ruud, byte ctlCode)
        {
            Map = (byte)mapID;
            LevelUpPoints = levelUpPoints;
            Str = strength;
            Agi = agility;
            Vit = vitality;
            Ene = energy;
            Cmd = comm;
            Direccion = direction;
            Experience = experience.ShufleEnding();
            NextExperience = nextExperience.ShufleEnding();
            Life = hp;
            MaxLife = hpMax;
            Mana = mp;
            MaxMana = mpMax;
            Shield = shield;
            MaxShield = maxShield;
            Stamina = bp;
            MaxStamina = bpMax;
            Zen = Money;
            PKLevel = pk;
            AddPoints = addPoints;
            MaxAddPoints = maxAddPoints;
            MinusPoints = minusPoints;
            MaxMinusPoints = maxMinusPoints;
            ExpandedInv = expandedInventory;
            Ruud = ruud;
            ControlCode = ctlCode;
            MapX = x;
            MapY = y;
        }

        //45
        public Point Position
        {
            get => new Point(MapX, MapY);
            set
            {
                MapX = (byte)value.X;
                MapY = (byte)value.Y;
            }
        }
    }

 
    [WZContract]
    public class ServerDto
    {
        [WZMember(0)] public ushort server { get; set; }
        [WZMember(1)] public ushort data1 { get; set; }
        [WZMember(2)] public ushort data2 { get; set; }
        [WZMember(3)] public byte type { get; set; }
        [WZMember(4)] public byte gold { get; set; }
}

    [WZContract(LongMessage = true)]
    public class SServerList : IAuthMessage
    {
        [WZMember(0, typeof(ArrayWithScalarSerializer<ushort>))] public ServerDto[] List { get; set; }
    }

    [WZContract]
    public class SServerMove : IAuthMessage
    {
        [WZMember(0, 16)] public byte[] IpAddress { get; set; }
        [WZMember(1)] public ushort ServerPort { get; set; }
        [WZMember(2)] public ushort ServerCode { get; set; }
        [WZMember(3)] public uint AuthCode1 { get; set; }
        [WZMember(4)] public uint AuthCode2 { get; set; }
        [WZMember(5)] public uint AuthCode3 { get; set; }
        [WZMember(6)] public uint AuthCode4 { get; set; }
    }

    [WZContract]
    public class SResetCharList : IAuthMessage
    {
        [WZMember(0, serializerType: typeof(ArraySerializer))] public ushort[] Resets { get; set; } = new ushort[5];
    }

    [WZContract]
    public class SResets : IAuthMessage
    {
        [WZMember(0)] public ushort Resets { get; set; }
    }

    [WZContract]
    public class SEnableCreation : IAuthMessage
    {
        [WZMember(0)]
        public EnableClassCreation EnableCreation { get; set; }
    }
}
