namespace webapi.common.domain;
public abstract class Entity(Guid id)
{
    public Guid Id { get; init; } = id;
    public override bool Equals(object? obj)
    {
        if (obj is not Entity other || GetType() != obj.GetType())
            return false;
            
        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(Entity? left, Entity? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Entity? left, Entity? right)
    {
        return !(left == right);
    }
}