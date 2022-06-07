using System;

namespace GamePack.CustomAttributes.Attributes
{
    public class HandleAttribute: Attribute
    {
        public SpaceType Space = SpaceType.World;
    }
    
    public enum SpaceType
    {
        World, Local
    }
}