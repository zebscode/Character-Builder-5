﻿using OGL.Base;
using OGL.Common;
using OGL.Descriptions;
using OGL.Keywords;
using OGL.Monsters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OGL
{
    public class Monster : IComparable<Monster>, IXML, IOGLElement<Monster>, IOGLElement
    {
        [XmlIgnore]
        public string FileName { get; set; }
        [XmlIgnore]
        public static XmlSerializer Serializer = new XmlSerializer(typeof(Monster));
        public String Name { get; set; }
        [XmlArrayItem(Type = typeof(Description)),
        XmlArrayItem(Type = typeof(ListDescription)),
        XmlArrayItem(Type = typeof(TableDescription))]
        public List<Description> Descriptions { get; set; } = new List<Description>();
        public String Description { get; set; }
        public String Flavour { get; set; }
        public String Source { get; set; }
        [XmlIgnore]
        public bool ShowSource { get; set; } = false;

        public Size Size { get; set; }

        [XmlArrayItem(Type = typeof(Keyword))]
        public List<Keyword> Keywords { get; set; } = new List<Keyword>();

        public string Alignment { get; set; }
        public int AC { get; set; }
        public string ACText { get; set; }
        public int HP { get; set; }
        public string HPRoll { get; set; }
        public List<String> Speeds { get; set; } = new List<string>();
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Constitution { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Charisma { get; set; }
        public List<String> Resistances { get; set; } = new List<string>();
        public List<String> Vulnerablities { get; set; } = new List<string>();
        public List<String> Immunities { get; set; } = new List<string>();
        public List<String> ConditionImmunities { get; set; } = new List<string>();
        public List<String> Senses { get; set; } = new List<string>();
        public int PassivePerception { get; set; }
        public List<String> Languages { get; set; } = new List<string>();
        public decimal CR { get; set; }
        public int XP { get; set; }
        [XmlArrayItem(Type = typeof(MonsterTrait))]
        public List<MonsterTrait> Traits { get; set; } = new List<MonsterTrait>();
        [XmlArrayItem(Type = typeof(MonsterTrait)),
        XmlArrayItem(Type = typeof(MonsterAction))]
        public List<MonsterTrait> Actions { get; set; } = new List<MonsterTrait>();
        [XmlArrayItem(Type = typeof(MonsterTrait)),
        XmlArrayItem(Type = typeof(MonsterAction))]
        public List<MonsterTrait> Reactions { get; set; } = new List<MonsterTrait>();
        [XmlArrayItem(Type = typeof(MonsterTrait)),
        XmlArrayItem(Type = typeof(MonsterAction))]
        public List<MonsterTrait> LegendaryActions { get; set; } = new List<MonsterTrait>();
        [XmlArrayItem(Type = typeof(MonsterSaveBonus))]
        public List<MonsterSaveBonus> SaveBonus { get; set; } = new List<MonsterSaveBonus>();

        [XmlArrayItem(Type = typeof(MonsterSkillBonus))]
        public List<MonsterSkillBonus> SkillBonus { get; set; } = new List<MonsterSkillBonus>();


        public List<String> Spells { get; set; } = new List<string>();
        public List<int> Slots { get; set; } = new List<int>();

        public byte[] ImageData { get; set; }
        public void Register(OGLContext context, string file)
        {
            FileName = file;
            string full = Name + " " + ConfigManager.SourceSeperator + " " + Source;
            if (context.Monsters.ContainsKey(full)) throw new Exception("Duplicate Monster: " + full);
            context.Monsters.Add(full, this);
            if (context.MonstersSimple.ContainsKey(Name))
            {
                context.MonstersSimple[Name].ShowSource = true;
                ShowSource = true;
            }
            else context.MonstersSimple.Add(Name, this);
        }
        public Monster()
        {
        }
        public Monster(OGLContext context, String name, String description)
        {
            Name = name;
            Description = description;
            Source = "Autogenerated";
            Register(context, null);
        }
        public String ToXML()
        {
            using (StringWriter mem = new StringWriter())
            {
                Serializer.Serialize(mem, this);
                return mem.ToString();
            }
        }
        public void Write(Stream stream)
        {
            Serializer.Serialize(stream, this);
        }
        public MemoryStream ToXMLStream()
        {
            MemoryStream mem = new MemoryStream();
            Serializer.Serialize(mem, this);
            return mem;
        }
        public override string ToString()
        {
            if (ShowSource || ConfigManager.AlwaysShowSource) return Name + " " + ConfigManager.SourceSeperator + " " + Source;
            return Name;
        }
        public int CompareTo(Monster other)
        {
            return Name.CompareTo(other.Name);
        }
        public Monster Clone()
        {
            using (MemoryStream mem = new MemoryStream())
            {
                Serializer.Serialize(mem, this);
                mem.Seek(0, SeekOrigin.Begin);
                Monster r = (Monster)Serializer.Deserialize(mem);
                r.FileName = FileName;
                return r;
            }
        }
        public bool Matches(string text, bool nameOnly)
        {
            CultureInfo Culture = CultureInfo.InvariantCulture;
            if (nameOnly) return Culture.CompareInfo.IndexOf(Name ?? "", text, CompareOptions.IgnoreCase) >= 0;
            return Culture.CompareInfo.IndexOf(Name ?? "", text, CompareOptions.IgnoreCase) >= 0
                || Culture.CompareInfo.IndexOf(Source ?? "", text, CompareOptions.IgnoreCase) >= 0
                || Culture.CompareInfo.IndexOf(Description ?? "", text, CompareOptions.IgnoreCase) >= 0
                || Culture.CompareInfo.IndexOf(Alignment ?? "", text, CompareOptions.IgnoreCase) >= 0
                || Speeds.Exists(s => Culture.CompareInfo.IndexOf(s ?? "", text, CompareOptions.IgnoreCase) >= 0)
                || Senses.Exists(s => Culture.CompareInfo.IndexOf(s ?? "", text, CompareOptions.IgnoreCase) >= 0)
                || Resistances.Exists(s => Culture.CompareInfo.IndexOf(s ?? "", text, CompareOptions.IgnoreCase) >= 0)
                || Vulnerablities.Exists(s => Culture.CompareInfo.IndexOf(s ?? "", text, CompareOptions.IgnoreCase) >= 0)
                || Immunities.Exists(s => Culture.CompareInfo.IndexOf(s ?? "", text, CompareOptions.IgnoreCase) >= 0)
                || Languages.Exists(s => Culture.CompareInfo.IndexOf(s ?? "", text, CompareOptions.IgnoreCase) >= 0)
                || ConditionImmunities.Exists(s => Culture.CompareInfo.IndexOf(s ?? "", text, CompareOptions.IgnoreCase) >= 0)
                || Descriptions.Exists(s => s.Matches(text, nameOnly))
                || Traits.Exists(s => s.Matches(text, nameOnly))
                || LegendaryActions.Exists(s => s.Matches(text, nameOnly))
                || Actions.Exists(s => s.Matches(text, nameOnly))
                || Keywords.Exists(s => Culture.CompareInfo.IndexOf(s.Name ?? "", text, CompareOptions.IgnoreCase) >= 0);
        }
        public int getAbility(Ability? a)
        {
            switch (a)
            {
                case Ability.Charisma: return Charisma;
                case Ability.Constitution: return Constitution;
                case Ability.Dexterity: return Dexterity;
                case Ability.Intelligence: return Intelligence;
                case Ability.Strength: return Strength;
                case Ability.Wisdom: return Wisdom;
                default: return 10;
            }
        }
    }
}
