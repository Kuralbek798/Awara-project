namespace AwaraIT.Kuralbek.Plugins.PluginExtensions.Interfaces
{
    public interface IPluginSubscriptionBuilder
    {
        IPluginSubscribeToMessage ToMessage(string message);
    }
}
