using System.Linq;

namespace Inheritance.MapObjects
{
    public interface IInteraction<in TInteracting>
    {
        bool Make(Player player, TInteracting interacting);
    }

    public interface IInteracting<TSelf>
        where TSelf : IInteracting<TSelf>
    {
        IInteraction<TSelf>[] Interactions { get; }
    }
    
    public interface IBeatable<TSelf> : IInteracting<TSelf>
        where TSelf : IBeatable<TSelf>
    {
        Army Army { get;  }
    }

    public interface IConsumable<TSelf> : IInteracting<TSelf>
        where TSelf : IConsumable<TSelf>
    {
        Treasure Treasure { get; }
    }

    public interface ICapturable<TSelf> : IInteracting<TSelf>
        where TSelf : ICapturable<TSelf>
    {
        int Owner { get; set; }
    }

    public class BeatableInteraction<TInteracting> : IInteraction<TInteracting>
        where TInteracting : IBeatable<TInteracting>
    {
        public bool Make(Player player, TInteracting interacting)
        {
            if (player.CanBeat(interacting.Army))
                return true;

            player.Die();
            return false;
        }
    }

    public class ConsumableInteraction<TInteracting> : IInteraction<TInteracting>
        where TInteracting : IConsumable<TInteracting>
    {
        public bool Make(Player player, TInteracting interacting)
        {
            player.Consume(interacting.Treasure);
            return true;
        }
    }

    public class CapturableInteraction<TInteracting> : IInteraction<TInteracting>
        where TInteracting : ICapturable<TInteracting>
    {
        public bool Make(Player player, TInteracting interacting)
        {
            interacting.Owner = player.Id;
            return true;
        }
    }
    
    public class Dwelling : ICapturable<Dwelling>
    {
        public int Owner { get; set; }
        public IInteraction<Dwelling>[] Interactions { get; } = 
            { new CapturableInteraction<Dwelling>() };
    }

    public class Mine : IBeatable<Mine>, IConsumable<Mine>, ICapturable<Mine>
    {
        public int Owner { get; set; }
        public Army Army { get; set; }
        public Treasure Treasure { get; set; }

        public IInteraction<Mine>[] Interactions { get; } =
            { new BeatableInteraction<Mine>(), new ConsumableInteraction<Mine>(), new CapturableInteraction<Mine>() };
    }

    public class Creeps : IBeatable<Creeps>, IConsumable<Creeps>
    {
        public Army Army { get; set; }
        public Treasure Treasure { get; set; }

        public IInteraction<Creeps>[] Interactions { get; } =
            { new BeatableInteraction<Creeps>(), new ConsumableInteraction<Creeps>() };
    }

    public class Wolves : IBeatable<Wolves>
    {
        public Army Army { get; set; }
        public IInteraction<Wolves>[] Interactions { get; } = 
            { new BeatableInteraction<Wolves>() };
    }

    public class ResourcePile : IConsumable<ResourcePile>
    {
        public Treasure Treasure { get; set; }
        public IInteraction<ResourcePile>[] Interactions { get; } = 
            { new ConsumableInteraction<ResourcePile>() };
    }

    public static class IInteractingExtensions
    {
        public static void Interact<TInteracting>(this TInteracting interacting, Player player)
            where TInteracting : IInteracting<TInteracting>
        {
            interacting.Interactions.All(interaction => interaction.Make(player, interacting));
        }
    }
    
    public static class Interaction
    {
        public static void Make<TInteracting>(Player player, TInteracting mapObject)
            where TInteracting : IInteracting<TInteracting>
        {
            mapObject.Interact(player);
        }
    }
}
