namespace TechfinChallenge.Infrastructure.Persistence;

using System.Data;
using Dapper;

public static class DatabaseInitializer
{
    public static void Initialize(IDbConnection connection)
    {
        var sql = @"
            CREATE TABLE IF NOT EXISTS Usuarios (
                Id TEXT PRIMARY KEY,
                Email TEXT NOT NULL UNIQUE,
                SenhaHash TEXT NOT NULL,
                DataCriacao TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Clientes (
                Id TEXT PRIMARY KEY,
                Nome TEXT NOT NULL,
                Cpf TEXT NOT NULL UNIQUE,
                ValorLimite REAL NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Transacoes (
                Id TEXT PRIMARY KEY,
                ClienteId TEXT NOT NULL,
                Valor REAL NOT NULL,
                Status INTEGER NOT NULL,
                DataCriacao TEXT NOT NULL,
                FOREIGN KEY (ClienteId) REFERENCES Clientes(Id)
            );
        ";

        connection.Execute(sql);
    }
}
