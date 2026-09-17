namespace TechfinChallenge.Infrastructure.Persistence;

using System.Reflection;
using TechfinChallenge.Domain.Entities;

public static class ReflectionHelper
{
    public static Usuario CreateUsuario(Guid id, string email, string senhaHash, DateTime dataCriacao)
    {
        var usuario = (Usuario)Activator.CreateInstance(typeof(Usuario), true)!;
        
        SetProperty(usuario, "Id", id);
        SetProperty(usuario, "Email", email);
        SetProperty(usuario, "SenhaHash", senhaHash);
        SetProperty(usuario, "DataCriacao", dataCriacao);
        
        return usuario;
    }

    private static void SetProperty<T>(object obj, string propertyName, T value)
    {
        var property = obj.GetType().GetProperty(propertyName, 
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        property?.SetValue(obj, value);
    }
}
