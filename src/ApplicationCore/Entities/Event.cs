using System;
using System.Linq;
using System.Collections.Generic;

namespace MMAEvents.ApplicationCore.Entities
{
    public class Event : BaseEntity
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public Uri ImgSrc { get; set; }
        public string Venue { get; set; }
        public string Location { get; set; }
        public bool IsScheduled { get; set; }

        public Dictionary<string, FightCard> FightCards { get; set; }
        public List<string> BonusAwards { get; set; }

        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(this, obj)) return true;

            if (obj != null && this.GetType().Equals(obj.GetType()))
            {
                var other = (Event)obj;

                return IsStringEquals(this.Name, other.Name) &&
                       this.Date == other.Date &&
                       this.IsScheduled == other.IsScheduled &&
                       this.IsDeleted == other.IsDeleted &&
                       IsUriEquals(this.ImgSrc, other.ImgSrc) &&
                       IsStringEquals(this.Venue, other.Venue) &&
                       IsStringEquals(this.Location, other.Location) &&
                       IsFightCardsEqual(this, other) &&
                       IsBonusAwardsEqual(this, other);
            }
            else
                return false;
        }

        private bool IsBonusAwardsEqual(Event a, Event b)
        {
            if (a.BonusAwards == null && b.BonusAwards == null)
                return true;
            if (a.BonusAwards == null ^ b.BonusAwards == null)
                return false;

            return a.BonusAwards.SequenceEqual(b.BonusAwards);
        }

        private bool IsUriEquals(Uri a, Uri b)
        {
            if (a == null && b == null)
                return true;
            if (a == null ^ b == null)
                return false;

            return a.OriginalString.Equals(b.OriginalString, StringComparison.InvariantCultureIgnoreCase);
        }

        private bool IsStringEquals(string a, string b)
        {
            return a.Equals(b, StringComparison.InvariantCulture);
        }

        private bool IsFightCardsEqual(Event a, Event b)
        {
            if (a.FightCards == null && b.FightCards == null)
                return true;
            if (a.FightCards == null ^ b.FightCards == null)
                return false;

            if (a.FightCards.Values == null && b.FightCards.Values == null)
                return true;
            if (a.FightCards.Values == null ^ b.FightCards.Values == null)
                return false;

            return a.FightCards.Values.SequenceEqual(b.FightCards.Values);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Id);
            hash.Add(Name);
            hash.Add(Date);
            return hash.ToHashCode();
        }
    }
}