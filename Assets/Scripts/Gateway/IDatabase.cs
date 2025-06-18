public interface IDatabase<T>
{
    public T FindByName(string nombre);
    public void Add(T element);
    public void Update(T element);
    public void Delete(T element);
}
