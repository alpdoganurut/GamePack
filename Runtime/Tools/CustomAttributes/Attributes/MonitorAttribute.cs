using System;

namespace GamePack.CustomAttributes.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class MonitorAttribute : Attribute
    {
        public DrawType DrawType;
    }

    public enum DrawType
    {
        GUI, WorldSelected, WorldAnyTime
    }
}