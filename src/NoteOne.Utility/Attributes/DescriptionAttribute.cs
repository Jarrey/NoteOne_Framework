using System;

namespace NoteOne_Utility.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class DescriptionAttribute : Attribute
    {
        public DescriptionAttribute()
            : this(string.Empty)
        {
        }

        public DescriptionAttribute(string description)
        {
            DescriptionValue = description;
        }

        protected string DescriptionValue { get; set; }

        public virtual string Description
        {
            get { return DescriptionValue; }
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
                return true;
            var attribute = obj as DescriptionAttribute;
            return ((attribute != null) && (attribute.Description == Description));
        }

        public override int GetHashCode()
        {
            return Description.GetHashCode();
        }
    }
}