using AwaraIT.Kuralbek.Plugins.PluginExtensions.Enums;
using System;

namespace AwaraIT.Kuralbek.Plugins.PluginExtensions.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class AssertionConditionAttribute : Attribute
    {
        public AssertionConditionAttribute(AssertionConditionType conditionType)
        {
            this.ConditionType = conditionType;
        }

        public AssertionConditionType ConditionType { get; private set; }
    }
}
