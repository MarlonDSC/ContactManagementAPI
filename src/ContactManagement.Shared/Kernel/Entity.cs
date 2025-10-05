namespace ContactManagement.Shared.Kernel
{
    public abstract class Entity : IEquatable<Entity>
    {
        public Guid Id { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public bool IsDeleted { get; private set; }
        public DateTime? DeletedAt { get; private set; }

        protected Entity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            IsDeleted = false;
        }

    public void UpdateTimestamps()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        if (!IsDeleted)
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            UpdateTimestamps();
        }
    }

    public void Restore()
    {
        if (IsDeleted)
        {
            IsDeleted = false;
            DeletedAt = null;
            UpdateTimestamps();
        }
    }

        public override bool Equals(object? obj)
        {
            return obj is Entity entity && Id.Equals(entity.Id);
        }

        public bool Equals(Entity? other)
        {
            if (other is null) return false;
            return Id.Equals(other.Id);
        }

        public static bool operator ==(Entity? left, Entity? right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(Entity? left, Entity? right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        // Static comparer property
        public static IEqualityComparer<Entity> IdComparer { get; } = new IdEqualityComparer();

        private sealed class IdEqualityComparer : IEqualityComparer<Entity>
        {
            public bool Equals(Entity? x, Entity? y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x is null || y is null) return false;
                return x.Id.Equals(y.Id);
            }

            public int GetHashCode(Entity obj)
            {
                return obj.Id.GetHashCode();
            }
        }
    }
}