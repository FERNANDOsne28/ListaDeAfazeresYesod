using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

class Program
{
    private static string connectionString;

    static void Main(string[] args)
    {
        // Carrega a configuração do appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        connectionString = configuration.GetConnectionString("dados");

        bool continuar = true;

        while (continuar)
        {
            // Exibe o menu
            Console.WriteLine("Menu:");
            Console.WriteLine("1. Inserir tarefa");
            Console.WriteLine("2. Listar tarefas");
            Console.WriteLine("3. Atualizar tarefa");
            Console.WriteLine("4. Excluir tarefa");
            Console.WriteLine("5. Sair");

            // Solicita a escolha do usuário
            Console.Write("Escolha uma opção (1-5): ");
            string escolha = Console.ReadLine();

            // Processa a escolha do usuário
            switch (escolha)
            {
                case "1":
                    InserirTarefa();
                    break;
                case "2":
                    ListarTarefas();
                    break;
                case "3":
                    AtualizarTarefa();
                    break;
                case "4":
                    ExcluirTarefa();
                    break;
                case "5":
                    Console.WriteLine("Saindo...");
                    continuar = false;
                    break;
                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    break;
            }
        }
    }

    static void InserirTarefa()
    {
        Console.Write("Digite a tarefa: ");
        var tarefa = Console.ReadLine();
        Console.Write("Digite a data da tarefa (ou qualquer outra informação relevante): ");
        var data = Console.ReadLine();

        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var query = "INSERT INTO cad_tarefas(tarefa, data_tarefa) VALUES (@tarefa, @data)";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@tarefa", tarefa);
                command.Parameters.AddWithValue("@data", data);
                command.ExecuteNonQuery();
            }
        }

        Console.WriteLine("Tarefa inserida com sucesso.");
    }

    static void ListarTarefas()
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var query = "SELECT id, tarefa, data_tarefa FROM cad_tarefas";
            using (var command = new MySqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    Console.WriteLine("Tarefas cadastradas:");
                    while (reader.Read())
                    {
                        Console.WriteLine($"ID: {reader["id"]}, Tarefa: {reader["tarefa"]}, Data: {reader["data_tarefa"]}");
                    }
                }
            }
        }
    }

    static void AtualizarTarefa()
    {
        Console.Write("Digite o ID da tarefa a ser atualizada: ");
        int id = int.Parse(Console.ReadLine());

        Console.Write("Digite a nova descrição da tarefa: ");
        var novaTarefa = Console.ReadLine();
        Console.Write("Digite a nova data da tarefa (ou qualquer outra informação relevante): ");
        var novaData = Console.ReadLine();

        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var query = "UPDATE cad_tarefas SET tarefa = @tarefa, data_tarefa = @data WHERE id = @id";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@tarefa", novaTarefa);
                command.Parameters.AddWithValue("@data", novaData);
                command.Parameters.AddWithValue("@id", id);
                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Tarefa atualizada com sucesso.");
                }
                else
                {
                    Console.WriteLine("Nenhuma tarefa encontrada com o ID fornecido.");
                }
            }
        }
    }

    static void ExcluirTarefa()
    {
        Console.Write("Digite o ID da tarefa a ser excluída: ");
        int id = int.Parse(Console.ReadLine());

        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var query = "DELETE FROM cad_tarefas WHERE id = @id";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Tarefa excluída com sucesso.");
                }
                else
                {
                    Console.WriteLine("Nenhuma tarefa encontrada com o ID fornecido.");
                }
            }
        }
    }
}

