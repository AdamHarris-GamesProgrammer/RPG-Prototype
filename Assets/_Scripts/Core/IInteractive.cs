public interface IInteractive 
{
    /// <summary>
    /// The interaction method for the object
    /// </summary>
    void Interact();

    /// <summary>
    /// Enables or disables the ui for this interactive object based on the passed in bool
    /// </summary>
    /// <param name="isActive"></param>
    void ShowUI(bool isActive);
}
