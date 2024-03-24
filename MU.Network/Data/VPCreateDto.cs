using MU.Resources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using WebZen.Serialization;
using WebZen.Util;

namespace MuEmu.Network.Data
{
    [WZContract]
    public abstract class VPCreateAbs
    {
        [WZMember(0)]
        public ushortle Number { get; set; }

        [WZMember(2)]
        public byte X { get; set; }

        [WZMember(3)]
        public byte Y { get; set; }

        public Point Position
        {
            get => new Point(X, Y);
            set
            {
                X = (byte)value.X;
                Y = (byte)value.Y;
            }
        }
        public object Player { get; set; }

        //[WZMember(4, 18)] public virtual byte[] CharSet { get; set; } //18
        //public ulong ViewSkillState;
    }

    [WZContract]
    public class VPCreateDto : VPCreateAbs
    {
        [WZMember(4, 18)] public byte[] CharSet { get; set; }

        [WZMember(5, typeof(BinaryStringSerializer), 10)]
        public string Name { get; set; } //10

        [WZMember(6)]
        public byte TX { get; set; }

        [WZMember(7)]
        public byte TY { get; set; }

        [WZMember(8)]
        public byte DirAndPkLevel { get; set; }

        public Point TPosition
        {
            get => new Point(TX, TY);
            set
            {
                TX = (byte)value.X;
                TY = (byte)value.Y;
            }
        }

        [WZMember(9, SerializerType = typeof(ArrayWithScalarSerializer<byte>))]
        public byte[] ViewSkillState { get; set; }

        public VPCreateDto()
        {
            CharSet = Array.Empty<byte>();
            //Id = Array.Empty<byte>();
            ViewSkillState = Array.Empty<byte>();
        }

    }
     
}
