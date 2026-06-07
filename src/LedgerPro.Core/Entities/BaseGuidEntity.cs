namespace LedgerPro.Core.Entities;

/// <summary>
/// Base class for entities that use a GUID as their primary key. Provides a common Id property initialized to a new GUID by default.
/// </summary>
public abstract class BaseGuidEntity
{
    public Guid Id { get; set; } = Guid.Empty; // Let the database generate the GUID value, so we initialize it to Guid.Empty to avoid confusion with uninitialized entities
}
