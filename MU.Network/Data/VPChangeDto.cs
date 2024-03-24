﻿using MU.Resources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using WebZen.Serialization;
using WebZen.Util;

namespace MuEmu.Network.Data
{
    [WZContract]
    public abstract class VPChangeAbs
    {
        [WZMember(0)]
        public byte NumberH { get; set; }

        [WZMember(1)]
        public byte NumberL { get; set; }

        [WZMember(2)]
        public byte X { get; set; }

        [WZMember(3)]
        public byte Y { get; set; }

        [WZMember(4)]
        public ushortle Skin { get; set; }

        [WZMember(6, 10)]
        public byte[] Id { get; set; } //10

        [WZMember(7)]
        public byte TX { get; set; }

        [WZMember(8)]
        public byte TY { get; set; }

        [WZMember(9)]
        public byte DirAndPkLevel { get; set; }
        //public ulong ViewSkillState;

        [WZMember(10, 20)]
        public byte[] CharSet { get; set; } //18

        public int Number
        {
            get => (NumberH << 8) | NumberL;
            set
            {
                NumberH = (byte)(value >> 8);
                NumberL = (byte)(value & 0xFF);
            }
        }

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

        public string Name
        {
            get => Id.MakeString();
            set => Id = value.GetBytes();
        }

    }

    [WZContract]
    public class VPChangeDto : VPChangeAbs
    {
        [WZMember(11, SerializerType = typeof(ArrayWithScalarSerializer<byte>))]
        //public byte SkillStateCount { get; set; }
        public byte[] ViewSkillState { get; set; } //Num_ViewSkillState

        public VPChangeDto()
        {
            CharSet = Array.Empty<byte>();
            Id = Array.Empty<byte>();
            ViewSkillState = Array.Empty<byte>();
        }
    }
     
}
