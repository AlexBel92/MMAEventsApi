using System;
using System.Linq;
using System.Collections.Generic;

namespace MMAEvents.ApplicationCore.Entities
{
    public class FightCard
    {
        public string Name { get; set; }
        public List<FightRecord> Fights { get; set; }

        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(this, obj)) return true;

            if (obj != null && this.GetType().Equals(obj.GetType()))
            {
                var other = (FightCard)obj;
                return IsStringEquals(this.Name, other.Name) &&
                       IsFightEqual(this, other);
            }
            else
                return false;
        }

        private bool IsStringEquals(string a, string b)
        {
            return a.Equals(b, StringComparison.InvariantCulture);
        }

        private bool IsFightEqual(FightCard a, FightCard b)
        {
            if (a.Fights == null && b.Fights == null)
                return true;
            if (a.Fights == null ^ b.Fights == null)
                return false;

            return a.Fights.SequenceEqual(b.Fights);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Fights);
        }
    }
}