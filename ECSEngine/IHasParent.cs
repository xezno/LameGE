namespace ECSEngine
{
    /// <summary>
    /// Any class that is able to have a parent.
    /// </summary>
    public interface IHasParent
    {
        IHasParent parent { get; set; }
    }
}
