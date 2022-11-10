#define IncludeSetVariable
#define SupportsEditMode
#define ScreenManagerHasPersistentPolygons
#define SpriteHasTolerateMissingAnimations
using Parme.Net.Frb.Example;

﻿using FlatRedBall.Content.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlueControl.Models
{
    public class StateSave
    {
        public string Name
        {
            get;
            set;
        }

        public List<InstructionSave> InstructionSaves { get; set; } = new List<InstructionSave>();

    }
}
