﻿using MU.Resources;
using MuEmu.Network.Data;
using System;
using System.Collections.Generic;
using System.Text;
using WebZen.Serialization;
using WebZen.Util;
using System.Linq;

namespace MU.Network.Guild
{
    [WZContract(LongMessage = true)]
    public class SGuildViewPort : IGuildMessage
    {
        [WZMember(0, SerializerType = typeof(ArrayWithScalarSerializer<byte>))]
        public GuildViewPortDto[] Guilds { get; set; }

        public SGuildViewPort()
        {
            Guilds = Array.Empty<GuildViewPortDto>();
        }
    }

    [WZContract]
    public class SGuildMasterQuestion : IGuildMessage
    {

    }

    [WZContract]
    public class SGuildCreateResult : IGuildMessage
    {
        [WZMember(0)] public byte Result { get; set; }

        [WZMember(1)] public byte GuildType { get; set; }
    }

    [WZContract]
    public class SGuildAnsViewport : IGuildMessage
    {
        // 0xC1 // 0
        // Size // 1
        // 0x66 // 2
        [WZMember(0)] public byte padding { get; set; } // 3
        [WZMember(1)] public int GuildNumber { get; set; }    // 4
        [WZMember(2)] public byte btGuildType { get; set; }   // 8
        [WZMember(3,8)] public byte[] btUnionName { get; set; }  // 9
        [WZMember(4,8)] public byte[] btGuildName { get; set; }  // 11
        [WZMember(5,32)] public byte[] Mark { get; set; }	// 19

        public string UnionName { get => btUnionName.MakeString(); set => btUnionName = value.GetBytes(); }
        public string GuildName { get => btGuildName.MakeString(); set => btGuildName = value.GetBytes(); }
    }

    [WZContract(LongMessage = true)]
    public class SGuildList : IGuildMessage
    {
        [WZMember(0)] public byte Result { get; set; }    // 4
        [WZMember(1)] public byte Count { get; set; } // 5
        [WZMember(2)] public ushort Padding { get; set; } // 6, 7
        [WZMember(3)] public int TotalScore { get; set; } // 8, 9, A, B
        [WZMember(4)] public byte Score { get; set; } // C
        [WZMember(0, typeof(BinaryStringSerializer), 8)] public string RivalGuild { get; set; }	// D
        [WZMember(6)] public ushort Padding2 { get; set; }

        [WZMember(7, SerializerType = typeof(ArraySerializer))]
        public GuildListDto[] Members { get; set; }

        public SGuildList()
        {
            Members = Array.Empty<GuildListDto>();
            RivalGuild = "";
        }

        public SGuildList(byte result)
        {
            Result = result;
            Members = Array.Empty<GuildListDto>();
            RivalGuild = "";
        }

        public SGuildList(byte result, byte score, int totalScore, List<GuildListDto> members, List<string> rivals)
        {
            Result = result;
            Score = score;
            TotalScore = totalScore;
            Members = members.ToArray();
            Count = (byte)Members.Length;

            RivalGuild = rivals.FirstOrDefault();
        }
    }

    [WZContract]
    public class GuildRivalDto
    {
        [WZMember(0, typeof(BinaryStringSerializer), 8)] public string RivalGuild { get; set; }
    }
     
    [WZContract]
    public class SGuildResult : IGuildMessage
    {
        [WZMember(0)] public GuildResult Result { get; set; }

        public SGuildResult() { }
        public SGuildResult(GuildResult res) { Result = res; }
    }

    [WZContract]
    public class SGuildSetStatus : IGuildMessage
    {
        [WZMember(0)] public byte Type { get; set; }
        [WZMember(1)] public GuildResult Result { get; set; }
        [WZMember(2, 10)] public byte[] btName { get; set; }

        public string Name => btName.MakeString();

        public SGuildSetStatus() { }

        public SGuildSetStatus(byte type, GuildResult res, string name)
        {
            Type = type;
            Result = res;

            btName = name.GetBytes();
        }
    }

    [WZContract]
    public class SGuildRemoveUser : IGuildMessage
    {
        [WZMember(0)] public GuildResult Result { get; set; }

        public SGuildRemoveUser() { }

        public SGuildRemoveUser(GuildResult res)
        {
            Result = res;
        }
    }

    [WZContract]
    public class SRelationShipJoinBreakOff : IGuildMessage
    {
        [WZMember(0)] public GuildRelationShipType RelationShipType { get; set; }    // 3
        [WZMember(1)] public GuildUnionRequestType RequestType { get; set; } // 4
        [WZMember(2)] public byte Result { get; set; } // 4
        [WZMember(3)] public ushort wzTargetUserIndex { get; set; }    // 5-6
    };

    [WZContract]
    public class UnionListDto
    {
        [WZMember(0)] public byte MemberNum { get; set; }   // 0
        [WZMember(1, 32)] public byte[] Mark { get; set; }  // 1
        [WZMember(2, typeof(BinaryStringSerializer), 8)] public string GuildName { get; set; }  // 21
    }

    [WZContract(LongMessage = true)]
    public class SUnionList : IGuildMessage
    {
        [WZMember(0)] public byte Count { get; set; }   // 4
        [WZMember(1)] public byte Result { get; set; }  // 5
        [WZMember(2)] public byte RivalMemberNum { get; set; }  // 6
        [WZMember(3)] public byte UnionMemberNum { get; set; }	// 7
        [WZMember(4, typeof(ArraySerializer))] public UnionListDto[] List { get; set; }
    };

    [WZContract]
    public class SGuildMatchingRegister : IGuildMessage
    {
        [WZMember(0)] public uint Result { get; set; }
    }

    [WZContract]
    public class SGuildMatchingRegisterCancel : IGuildMessage
    {
        [WZMember(0)] public uint Result { get; set; }
    }


    [WZContract]
    public class SGuildMatchingJoin : IGuildMessage
    {
        [WZMember(0)] public int Result { get; set; }
    }

    [WZContract]
    public class SGuildMatchingAccept : IGuildMessage
    {
        [WZMember(0, typeof(BinaryStringSerializer), 12)] public string Name { get; set; }
        [WZMember(1)] public int Type { get; set; }
        [WZMember(2)] public int Result { get; set; }
    }

    [WZContract]
    public class SGuildMatchingJoinInfo : IGuildMessage
    {
        [WZMember(0, typeof(BinaryStringSerializer), 11)] public string MasterName { get; set; }
        [WZMember(1, typeof(BinaryStringSerializer), 9)] public string GuildName { get; set; }
        [WZMember(2)] public int Result { get; set; }
    }

    [WZContract(LongMessage = true)]
    public class SGuildMatchingJoinList : IGuildMessage
    {
        [WZMember(0, typeof(ArraySerializer))] public byte[] padding { get; set; } = new byte[3];
        [WZMember(1)] public int Count { get; set; }
        [WZMember(2)] public int Result { get; set; }
        [WZMember(3, typeof(ArraySerializer))] public GuildMatchingJoinListDto[] List { get; set; }
    }

    [WZContract]
    public class GuildMatchingJoinListDto : IGuildMessage
    {
        [WZMember(0, typeof(BinaryStringSerializer), 11)] public string Name { get; set; }
        [WZMember(1)] public byte Class { get; set; }
        [WZMember(2)] public uint Level { get; set; }
        [WZMember(3)] public uint Padding { get; set; }

    }

    [WZContract]
    public class SGuildMatchingNotify : IGuildMessage
    {
        [WZMember(0)] public uint Result { get; set; }
    }

    [WZContract]
    public class SGuildMatchingNotifyMaster : IGuildMessage
    {
        [WZMember(0)] public uint Result { get; set; }
    }
    [WZContract(LongMessage = true)]
    public class SGuildMatchingList : IGuildMessage
    {
        [WZMember(0, typeof(ArraySerializer))] public byte[] Padding { get; set; } = new byte[3];
        [WZMember(1)] public int CurrentPage { get; set; }
        [WZMember(2)] public int MaxPage { get; set; }
        [WZMember(3)] public int Count { get; set; }
        [WZMember(4)] public uint Result { get; set; }
        [WZMember(5, typeof(ArraySerializer))] public GuildMatchingListDto[] List { get; set; }
    }
    [WZContract]
    public class GuildMatchingListDto
    {
        [WZMember(0, typeof(BinaryStringSerializer), 41)] public string Text { get; set; }// [GUILD_MATCHING_TEXT_LENGTH + 1]; //0
        [WZMember(1, typeof(BinaryStringSerializer), 11)] public string Name { get; set; }//[MAX_CHARACTER_LENGTH + 1]; //41
        [WZMember(2, typeof(BinaryStringSerializer), 9)] public string GuildName { get; set; }//[MAX_GUILD_NAME_LENGTH + 1]; //52
        [WZMember(3)] public byte MembersCount { get; set; } //61
        [WZMember(4)] public byte MasterClass { get; set; } //62
        [WZMember(5)] public GMInterestType InterestType { get; set; } //63
        [WZMember(6)] public GMLevelRange LevelRange { get; set; } //64
        [WZMember(7)] public byte Padding { get; set; } //65
        [WZMember(8)] public GMClass ClassType { get; set; } //66
        [WZMember(9)] public uint MasterLevel { get; set; } //68
        [WZMember(10)] public uint BoardNumber { get; set; } //72
        [WZMember(11)] public int GuildId { get; set; } //76
        [WZMember(12)] public int Gens { get; set; } //80
    }
}
