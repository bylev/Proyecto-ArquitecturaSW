namespace TransGGP.Application.Interfaces
{
    public interface IPasswordHasher
    {
        string Hashear(string password);
        bool Verificar(string password, string hash);
    }
}
