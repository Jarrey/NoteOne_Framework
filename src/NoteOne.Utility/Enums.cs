using System;
using NoteOne_Utility.Attributes;

namespace NoteOne_Utility
{
    /// <summary>
    ///     Suuport for TypeInfo GetMember extension method
    /// </summary>
    public enum MemberType
    {
        All,
        Field,
        Event,
        Property,
        Method,
        NestedType
    }

    [Flags]
    public enum CollectionOperate
    {
        None = 0x00,
        Sort = 0x01,
        Filter = 0x02,
        Group = 0x04,
        Search = 0x08
    }

    public enum LogType
    {
        [Description("Exception")] Exception,
        [Description("Message")] Message,
        [Description("Debug")] Debug
    }
}