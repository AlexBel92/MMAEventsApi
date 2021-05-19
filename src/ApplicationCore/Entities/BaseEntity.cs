namespace MMAEvents.ApplicationCore.Entities
{
    public abstract class BaseEntity
    {
        public virtual long Id { get; set; }
        public bool IsDeleted { get; set; }
    }
}