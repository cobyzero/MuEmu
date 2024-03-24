﻿using MU.DataBase;
using MuEmu.Entity;
using MU.Network.Game;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using MU.Resources;
using System.Linq;
using MuEmu.Resources.XML;
using System.IO;

namespace MuEmu
{
    public class MasterLevel
    {
        private bool _needSave;
        private ushort _level;
        private long _experience;
        private ushort _points;
        private ushort _mPoints;
        private bool _new;

        public static MasterSkillTreeDto MasterSkillTree { get; set; }

        public ushort Level
        {
            get => _level;
            set
            {
                _level = value;
                _needSave = true;
            }
        }
        public long Experience
        {
            get => _experience; set
            {
                _experience = value;
                _needSave = true;
            }
        }
        public long NextExperience => GetExperienceFromLevel((ushort)(Level + Character.Level-1));
        public ushort Points
        {
            get => _points; set
            {
                _needSave = true;
                _points = value;
            }
        }
        public ushort MPoints
        {
            get => _mPoints; set
            {
                _needSave = true;
                _mPoints = value;
            }
        }
        public Character Character { get; private set; }
        public static void Initialize()
        {
            if (MasterSkillTree == null)
                MasterSkillTree = Resources.ResourceLoader.XmlLoader<MasterSkillTreeDto>($"./Data/MasterLevel/MasterSkillTree.xml");

            var skillClear = MasterSkillTree.Trees.SelectMany(x => x.Skill).Where(x => string.IsNullOrEmpty(x.Ecuation));
            var skillSets = MasterSkillTree.Trees.SelectMany(x => x.Skill).Where(x => !string.IsNullOrEmpty(x.Ecuation));
            foreach(var skill in skillClear)
            {
                var result = skillSets.FirstOrDefault(x => x.MagicNumber == skill.MagicNumber);
                skill.Ecuation = result?.Ecuation ?? "";
                skill.Property = result?.Property ?? "";
            }
        }
        public MasterLevel(Character @char, CharacterDto @charDto)
        {
            Character = @char;
            _new = @charDto.MasterInfo == null;
            Level = @charDto.MasterInfo?.Level ?? 1;
            Experience = @charDto.MasterInfo?.Experience ?? 0;
            Points = @charDto.MasterInfo?.Points ?? 0;
        }

        public void GetExperience(long exp)
        {
            if(!Character.MasterClass || Character.Level != 400)
            {
                return;
            }

            Experience += exp;
            var level = Level;
            while(Experience >= NextExperience)
            {
                Level++;
            }

            if(level != Level)
            {
                var LevelAdd = Level - level;
                var levelPoint = Character.BaseClass == HeroClass.MagicGladiator || Character.BaseClass == HeroClass.DarkLord ? 7 : 5;

                Points += (ushort)(levelPoint*LevelAdd);
                Character.LevelUpPoints += (ushort)((levelPoint+1) * LevelAdd);
                if (Points > 200)
                    Points = 200;

                Character.CalcStats();
                Character.Player.Session
                    .SendAsync(new SMasterLevelUp(Level, (ushort)levelPoint, Points, maxPoints:(ushort)200, (ushort)Character.MaxHealth, (ushort)Character.MaxShield, (ushort)Character.MaxMana, (ushort)Character.MaxStamina)).Wait();

                Character.Player.Session.SendAsync(new SEffect(Character.Index, ClientEffect.LevelUp)).Wait();
            }
        }

        private long GetExperienceFromLevel(ushort level)
        {
            var exp = (((level + 9L) * level) * level) * 10L + ((level>255)?(((((long)(level - 255) + 9L) * (level - 255L)) * (level - 255L)) * 1000L):0L);
            if (level >= 400)
            {
                exp -= 3892250000;
                exp /= 2;
            }
            if (level > 600)
            {
                var Level3 = (double)((level - 600) * (level - 600));
                exp = (long)(exp * (1 + (Level3 * 1.2) / 100000.0));
            }
            return exp;
        }

        public async void SendInfo()
        {
            if (Character.MasterClass)
            {
                var XPSend = Character.Level >= 400 ? Experience : Character.Experience;
                await Character
                    .Player
                    .Session
                    .SendAsync(new SMasterInfo(
                        Level, 
                        XPSend, 
                        NextExperience, 
                        Points, 
                        (ushort)Character.MaxHealth, 
                        (ushort)Character.MaxShield, 
                        (ushort)Character.MaxMana, 
                        (ushort)Character.MaxStamina
                        ));
            }
            if(Character.MajesticClass)
            {
                await Character
                    .Player
                    .Session
                    .SendAsync(new SMajesticInfo
                    {
                        Points = MPoints,
                        SkillList = Array.Empty<MajesticInfoDto>(),
                    });

                await Character
                    .Player
                    .Session
                    .SendAsync(new SMajesticStatsInfo
                    {
                        SkillList = Array.Empty<MajesticInfoDto>(),
                    });
            }
        }

        public async Task Save(GameContext db)
        {
            if (!_needSave || !Character.MasterClass)
                return;

            if (_new)
            {
                db.MasterLevel.Add(new MasterInfoDto
                {
                    MasterInfoId = Character.Id,
                    Experience = Experience,
                    Level = Level,
                    Points = Points,
                });
                _new = false;
                return;
            }

            var info = db.MasterLevel.First(x => x.MasterInfoId == Character.Id);
            info.Experience = Experience;
            info.Level = Level;
            info.Points = Points;
            db.MasterLevel.Update(info);
        }
    }
}
