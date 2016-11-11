using System;

namespace DrinkOBand.Core.Entities
{
    public class IntervalItem : IEquatable<IntervalItem>
    {
        public bool Equals(IntervalItem other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IntervalItem) obj);
        }

        public override int GetHashCode()
        {
            return Value;
        }

        public static bool operator ==(IntervalItem left, IntervalItem right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(IntervalItem left, IntervalItem right)
        {
            return !Equals(left, right);
        }

        public string Name { get; set; }
        public int Value { get; set; } 
    }
}