public interface IInteractable
{
    public InteractionData Interact(bool activate = true);
    public void Highlight(bool on, InteractionData data);
}
