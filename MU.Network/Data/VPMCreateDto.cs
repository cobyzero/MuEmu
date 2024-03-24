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
    public abstract class VPMCreateAbs
    {
        [WZMember(0)]
        public ushortle Number { get; set; }

        [WZMember(2)]
        public ushortle Type { get; set; }

        [WZMember(4)]
        public byte X { get; set; }

        [WZMember(5)]
        public byte Y { get; set; }

        [WZMember(6)]
        public byte TX { get; set; }

        [WZMember(7)]
        public byte TY { get; set; }

        [WZMember(8)]
        public byte Path { get; set; }

        public Point Position
        {
            get => new Point(X, Y);
            set
            {
                X = (byte)value.X;
                Y = (byte)value.Y;
            }
        }

        public Point TPosition
        {
            get => new Point(TX, TY);
            set
            {
                TX = (byte)value.X;
                TY = (byte)value.Y;
            }
        }
    }

    [WZContract]
    public class VPMCreateDto : VPMCreateAbs
    {
        [WZMember(9, typeof(ArrayWithScalarSerializer<byte>))]
        public byte[] ViewSkillState { get; set; }

        public VPMCreateDto()
        {
            ViewSkillState = Array.Empty<byte>();
        }
    }
     
}
