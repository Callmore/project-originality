using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectOriginality.Enums;
using ProjectOriginality.Models;
using Godot;


namespace ProjectOriginality.Resources
{
    public class StatusStack : Resource
    {
        [Export]
        public StatusId Status = StatusId.Block;

        [Export]
        public int Stacks = -1;

        public StatusStack() { }
        public StatusStack(StatusId status = StatusId.Block, int stacks = -1)
        {
            Status = status;
            Stacks = stacks;
        }
    }
}
