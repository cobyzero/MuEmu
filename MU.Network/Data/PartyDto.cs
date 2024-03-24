using MU.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using WebZen.Serialization;
using WebZen.Util;

namespace MuEmu.Network.Data
{
    public interface IPartyDto
    {
        public abstract IPartyDto Set(
            int number, 
            string name, 
            Maps map, 
            byte x, 
            byte y, 
            int life, 
            int maxLife, 
            int mana, 
            int maxMana,
            int channel,
            byte assistant);
    }
    [WZContract]
    public class PartyDto : IPartyDto
    {
        [WZMember(0, typeof(BinaryStringSerializer), 10)] public string Id { get; set; }
        [WZMember(1)] public byte Number { get; set; }
        [WZMember(2)] public byte Map { get; set; }
        [WZMember(3)] public byte X { get; set; }
        [WZMember(4)] public byte Y { get; set; }
        [WZMember(5)] public ushort Padding1 { get; set; }
        [WZMember(6)] public int Life { get; set; }
        [WZMember(7)] public int MaxLife { get; set; }

        public IPartyDto Set(int number, string name, Maps map, byte x, byte y, int life, int maxLife, int mana, int maxMana, int channel, byte assistant)
        {
            Number = (byte)number;
            Id = name;
            Map = (byte)Map;
            X = x;
            Y = y;
            Life = life;
            MaxLife = maxLife;
            return this;
        }
    }
    
     
}
