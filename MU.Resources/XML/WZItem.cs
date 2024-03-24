using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml.Serialization;

namespace MU.Resources.XML
{
    [XmlType(AnonymousType = true)]
    [XmlRoot("Item")]
    public class WZItem
    {
        [XmlElement] public WZItemGroupWeapons[] Sword { get; set; }
        [XmlElement] public WZItemGroupWeapons[] Axe { get; set; }
        [XmlElement] public WZItemGroupWeapons[] Scepter { get; set; }
        [XmlElement] public WZItemGroupWeapons[] Spear { get; set; }
        [XmlElement] public WZItemGroupWeapons[] BowOrCrossbow { get; set; }
        [XmlElement] public WZItemGroupWeapons[] Staff { get; set; }
        [XmlElement] public WZItemGroupSet[] Shield { get; set; }
        [XmlElement] public WZItemGroupSet[] Helm { get; set; }
        [XmlElement] public WZItemGroupSet[] Armor { get; set; }
        [XmlElement] public WZItemGroupSet[] Pant { get; set; }
        [XmlElement] public WZItemGroupSet[] Gloves { get; set; }
        [XmlElement] public WZItemGroupSet[] Boots { get; set; }
        [XmlElement] public WZItemGroupWingsOrbsSeeds[] WingsOrbsSeeds { get; set; }
        [XmlElement] public WZItemGroupMisc[] Miscs  { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class WZItemGroupBasic
    {
        [XmlAttribute] public ushort Index { get; set; }
        [XmlAttribute] public byte Slot { get; set; }
        [XmlAttribute] public ushort Skill { get; set; }
        [XmlAttribute] public byte X { get; set; }
        [XmlAttribute] public byte Y { get; set; }
        [XmlAttribute] public byte Serial { get; set; }
        [XmlAttribute] public byte Option { get; set; }
        [XmlAttribute] public byte Drop { get; set; }
        [XmlAttribute] public string Name { get; set; }
        [XmlAttribute] public ushort Level { get; set; }

        [XmlIgnore] public Size Size => new Size(X, Y);
    }

    [XmlType(AnonymousType = true)]
    public class WZItemGroupWeapons : WZItemGroupBasic
    {
        [XmlAttribute] public ushort DmgMin { get; set; }
        [XmlAttribute] public ushort DmgMax { get; set; }
        [XmlAttribute] public ushort Speed { get; set; }
        [XmlAttribute] public ushort Durability { get; set; }
        [XmlAttribute] public ushort MagicDur { get; set; }
        [XmlAttribute] public ushort MagicPower { get; set; }
        [XmlAttribute] public ushort RequiredLvl { get; set; }
        [XmlAttribute] public ushort Str { get; set; }
        [XmlAttribute] public ushort Agi { get; set; }
        [XmlAttribute] public ushort Ene { get; set; }
        [XmlAttribute] public ushort Vit { get; set; }
        [XmlAttribute] public ushort Cmd { get; set; }
        [XmlAttribute] public ushort Type { get; set; }
        [XmlAttribute] public byte DW { get; set; }
        [XmlAttribute] public byte DK { get; set; }
        [XmlAttribute] public byte Elf { get; set; }
        [XmlAttribute] public byte MG { get; set; }
        [XmlAttribute] public byte DL { get; set; }
        [XmlAttribute] public byte SM { get; set; }

        [XmlIgnore] public Point Damage => new Point(DmgMin, DmgMax);
        [XmlIgnore] public List<HeroClass> Classes => GetClasses();
        private List<HeroClass> GetClasses()
        {
            var result = new List<HeroClass>();
            var list = new byte[] { DW, DK, Elf, MG, DL, SM };
            for(var i = 0; i < list.Length; i++)
            {
                if (list[i] <= 0)
                    continue;

                var @class = (HeroClass)((i << 8) + (list[i] - 1));
                result.Add(@class);
            }

            return result;
        }
    }

    [XmlType(AnonymousType = true)]
    public class WZItemGroupSet : WZItemGroupBasic
    {
        [XmlAttribute] public ushort Def { get; set; }
        [XmlAttribute] public ushort DefRate { get; set; }
        [XmlAttribute] public ushort Durability { get; set; }
        [XmlAttribute] public ushort ReqLevel { get; set; }
        [XmlAttribute] public ushort Str { get; set; }
        [XmlAttribute] public ushort Agi { get; set; }
        [XmlAttribute] public ushort Ene { get; set; }
        [XmlAttribute] public ushort Vit { get; set; }
        [XmlAttribute] public ushort Command { get; set; }
        [XmlAttribute] public ushort Type { get; set; }
        [XmlAttribute] public byte DW { get; set; }
        [XmlAttribute] public byte DK { get; set; }
        [XmlAttribute] public byte Elf { get; set; }
        [XmlAttribute] public byte MG { get; set; }
        [XmlAttribute] public byte DL { get; set; }
        [XmlAttribute] public byte SM { get; set; }

        [XmlIgnore] public List<HeroClass> Classes => GetClasses();
        private List<HeroClass> GetClasses()
        {
            var result = new List<HeroClass>();
            var list = new byte[] { DW, DK, Elf, MG, DL, SM };
            for (var i = 0; i < list.Length; i++)
            {
                if (list[i] <= 0)
                    continue;

                var @class = (HeroClass)((i << 8) + (list[i] - 1));
                result.Add(@class);
            }

            return result;
        }
    }

    [XmlType(AnonymousType = true)]
    public class WZItemGroupWingsOrbsSeeds : WZItemGroupBasic
    {
        [XmlAttribute] public ushort Defense { get; set; }
        [XmlAttribute] public ushort Durability { get; set; }
        [XmlAttribute] public ushort RequiredLvl { get; set; }
        [XmlAttribute] public ushort Ene { get; set; }
        [XmlAttribute] public ushort Strength { get; set; }
        [XmlAttribute] public ushort Agi { get; set; }
        [XmlAttribute] public ushort Command { get; set; }
        [XmlAttribute] public ushort Type { get; set; }
        [XmlAttribute] public byte DW { get; set; }
        [XmlAttribute] public byte DK { get; set; }
        [XmlAttribute] public byte Elf { get; set; }
        [XmlAttribute] public byte MG { get; set; }
        [XmlAttribute] public byte DL { get; set; }
        [XmlAttribute] public byte SM { get; set; }
        [XmlIgnore] public List<HeroClass> Classes => GetClasses();
        private List<HeroClass> GetClasses()
        {
            var result = new List<HeroClass>();
            var list = new byte[] { DW, DK, Elf, MG, DL, SM };
            for (var i = 0; i < list.Length; i++)
            {
                if (list[i] <= 0)
                    continue;

                var @class = (HeroClass)((i << 8) + (list[i] - 1));
                result.Add(@class);
            }

            return result;
        }
    }

    [XmlType(AnonymousType = true)]
    public class WZItemGroupMisc : WZItemGroupBasic
    {
        [XmlAttribute] public ushort Durability { get; set; }
        [XmlAttribute] public ushort Ice { get; set; }
        [XmlAttribute] public ushort Poison { get; set; }
        [XmlAttribute] public ushort Light { get; set; }
        [XmlAttribute] public ushort Fire { get; set; }
        [XmlAttribute] public ushort Earth { get; set; }
        [XmlAttribute] public ushort Wind { get; set; }
        [XmlAttribute] public ushort Water { get; set; }
        [XmlAttribute] public ushort Type { get; set; }
        [XmlAttribute] public byte DW { get; set; }
        [XmlAttribute] public byte DK { get; set; }
        [XmlAttribute] public byte Elf { get; set; }
        [XmlAttribute] public byte MG { get; set; }
        [XmlAttribute] public byte DL { get; set; }
        [XmlAttribute] public byte SM { get; set; }
        [XmlIgnore] public List<HeroClass> Classes => GetClasses();
        private List<HeroClass> GetClasses()
        {
            var result = new List<HeroClass>();
            var list = new byte[] { DW, DK, Elf, MG, DL, SM };
            for (var i = 0; i < list.Length; i++)
            {
                if (list[i] <= 0)
                    continue;

                var @class = (HeroClass)((i << 8) + (list[i] - 1));
                result.Add(@class);
            }

            return result;
        }
    }

  
}
