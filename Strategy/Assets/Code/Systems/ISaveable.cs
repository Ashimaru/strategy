namespace SaveSystem
{
    public interface ISaveable
    {
        object SaveState();
        void LoadState(object savedState);
    }
}
