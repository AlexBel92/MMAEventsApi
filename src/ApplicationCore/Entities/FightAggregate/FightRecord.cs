using System;

namespace MMAEvents.ApplicationCore.Entities
{
    public class FightRecord
    {
        public string WeightClass { get; set; }
        public string FirtsFighter { get; set; }
        public string SecondFighter { get; set; }
        public string Method { get; set; }
        public string Round { get; set; }
        public string Time { get; set; }

        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(this, obj)) return true;

            if (obj != null && this.GetType().Equals(obj.GetType()))
            {
                var other = (FightRecord)obj;
                return IsStringEquals(this.FirtsFighter, other.FirtsFighter) &&
                       IsStringEquals(this.SecondFighter, other.SecondFighter) &&
                       IsStringEquals(this.WeightClass, other.WeightClass) &&
                       IsStringEquals(this.Method, other.Method) &&
                       IsStringEquals(this.Round, other.Round) &&
                       IsStringEquals(this.Time, other.Time);
            }
            else
                return false;
        }

        private bool IsStringEquals(string a, string b)
        {
            return a.Equals(b, StringComparison.InvariantCulture);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(WeightClass, FirtsFighter, SecondFighter, Method, Round, Time);
        }
    }
}