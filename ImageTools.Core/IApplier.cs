namespace ImageTools.Core
{
    public interface IApplier
    {
        public string Id { get; }

        EditableImage Apply(EditableImage image);
    }
}