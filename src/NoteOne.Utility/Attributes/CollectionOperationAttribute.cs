using System;

namespace NoteOne_Utility.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class CollectionOperationAttribute : Attribute
    {
        public CollectionOperationAttribute()
            : this(CollectionOperate.None)
        {
        }

        public CollectionOperationAttribute(CollectionOperate operate)
        {
            OperationValue = operate;
        }

        protected CollectionOperate OperationValue { get; set; }

        public virtual CollectionOperate Operation
        {
            get { return OperationValue; }
        }

        public override bool Equals(object obj)
        {
            if (obj == this) return true;
            var attribute = obj as CollectionOperationAttribute;
            return ((attribute != null) && (attribute.Operation == Operation));
        }

        public override int GetHashCode()
        {
            return Operation.GetHashCode();
        }
    }
}