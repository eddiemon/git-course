using System;

namespace StateFramework
{
    [System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public sealed class StatePathAttribute : Attribute
    {

        public StatePathAttribute(string path, StatePath.PermissionModifier permission = StatePath.PermissionModifier.Public)
        {
            Path = path;
            Permission = permission;
        }

        public string Path { get; }
        public StatePath.PermissionModifier Permission { get; set; }
    }
}