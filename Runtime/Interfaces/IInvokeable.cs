namespace StorkStudios.CoreNest
{
    public interface IInvokeable
    {
        public void Invoke();
    }

    public interface IInvokeable<T>
    {
        public void Invoke(T obj);
    }
}