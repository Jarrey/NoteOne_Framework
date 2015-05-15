using System;
using System.Linq;
using System.Reflection;

namespace NoteOne_Utility.Extensions
{
    public static class TypeExtension
    {
        public static MemberInfo GetMemberInfo(this TypeInfo typeInfo, string name, MemberType t)
        {
            MemberInfo memberInfo = null;
            switch (t)
            {
                case MemberType.Event:
                    memberInfo = typeInfo.GetDeclaredEvent(name);
                    break;
                case MemberType.Field:
                    memberInfo = typeInfo.GetDeclaredField(name);
                    break;
                case MemberType.Method:
                    memberInfo = typeInfo.GetDeclaredMethod(name);
                    break;
                case MemberType.Property:
                    memberInfo = typeInfo.GetDeclaredProperty(name);
                    break;
                case MemberType.NestedType:
                    memberInfo = typeInfo.GetDeclaredNestedType(name);
                    break;
                case MemberType.All:
                    memberInfo = typeInfo.DeclaredMembers.Single(p => p.Name == name);
                    break;
            }
            if (memberInfo == null)
            {
                Type baseType = typeInfo.BaseType;
                if (baseType != null)
                    return baseType.GetTypeInfo().GetMemberInfo(name, t);
                else
                    return memberInfo;
            }
            return memberInfo;
        }

        public static Type GenerateType(this string assemblyTypeName)
        {
            Type t = null;
            string[] names = assemblyTypeName.Split('|');
            if (names.Length > 1)
            {
                Assembly assembly = Assembly.Load(new AssemblyName((names[0])));
                t = assembly.GetType(names[1]);
            }
            return t;
        }
    }
}