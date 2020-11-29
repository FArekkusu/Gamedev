using System.Collections.Generic;
using System.Linq;

namespace HunterGame
{
    public class DoeGroup
    {
        public const int MinimumCompleteSize = 3;
        public const int MaximumSize = 10;
        
        public List<Doe> Members = new List<Doe>();

        public Doe Leader => Members[0];

        public int Size => Members.Count;
        public bool IsComplete => Size >= MinimumCompleteSize && Size <= MaximumSize;

        public void Add(Doe doe)
        {
            Members.Add(doe);
            
            doe.Group = this;
        }

        public void Join(DoeGroup otherGroup)
        {
            foreach (var member in Members)
                otherGroup.Add(member);

            Members.Clear();
        }

        public void SplitIntoHalves()
        {
            var newGroup = new DoeGroup();
            
            var left = Members.Take(Size / 2);
            var moved = Members.Skip(Size / 2);
            
            foreach (var member in moved)
                newGroup.Add(member);

            Members = left.ToList();
            
            newGroup.Leader.WanderTarget = newGroup.Leader.CenterPosition;
        }
        
        public void CheckAlive()
        {
            var previousLeader = Leader;
            
            Members = Members.Where(doe => doe.IsAlive).ToList();

            if (!previousLeader.IsAlive && Size > 0)
                Leader.WanderTarget = previousLeader.WanderTarget;
        }
    }
}