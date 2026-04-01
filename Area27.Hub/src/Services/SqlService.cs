using System.Data;
using Microsoft.Data.SqlClient;

namespace Area27.Hub.Services;

/// <summary>
/// Serviço para operações básicas com SQL Server.
/// </summary>
public class SqlService
{
    private readonly string _connectionString;

    public SqlService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DataTable ExecutarConsulta(string query)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(query, conn);
        using var adapter = new SqlDataAdapter(cmd);
        var dt = new DataTable();
        adapter.Fill(dt);
        return dt;
    }

    public int ExecutarComando(string query)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(query, conn);
        conn.Open();
        return cmd.ExecuteNonQuery();
    }
}