using System;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;

namespace Minions
{
    class Program
    {
        private static string connectionString =
            "Server=.;Database=Minions;Trusted_Connection=True; TrustServerCertificate=True";

        static void Main(string[] args)
        {
            //NameOfVillains();
            //int id = Convert.ToInt32(Console.ReadLine());
            //NameOfMinions(id);
            string MinionName = "Slave";
            int minionAge = 12;
            string town = "Dungeon";
            string villainName = "Master";
            AddMinions( MinionName,  minionAge, town, villainName);
            //int idDelete = Convert.ToInt32(Console.ReadLine());
            //DeleteVillain(idDelete);
            YearsWent();
        }


        // Задание 2. Напишите программу, которая выводит на консоль имена всех
        // злодеев и количество миньонов тех, у кого есть более 3 миньонов. Список
        // представить упорядоченный по убыванию количества миньонов.
        static void NameOfVillains()
        {
            SqlConnection connectionNameOfVillains = new SqlConnection(connectionString);
            connectionNameOfVillains.Open();
            using (connectionNameOfVillains)
            {
                string selectionVillainsWithMinions =
                    "SELECT v.Name, COUNT(mv.MinionId) as CountMinions " +
                    "FROM Villains AS v " +
                    "JOIN MinionsVillains AS mv " +
                    "ON(v.id = mv.VillainId) " +
                    "GROUP BY v.Name HAVING COUNT(mv.MinionId) >= 3 " +
                    "ORDER BY COUNT(mv.MinionId) DESC";
                SqlCommand commandVillainsWithMinions =
                    new SqlCommand(selectionVillainsWithMinions, connectionNameOfVillains);
                SqlDataReader reader = commandVillainsWithMinions.ExecuteReader();
                using (reader)
                {
                    while (reader.Read())
                    {

                        Console.Write($"{reader["Name"]} - {reader["CountMinions"]} ");
                        Console.WriteLine();
                    }
                }
            }
        }

        // Задание 3. Напишите программу, которая выводит на консоль все имена
        // миньонов и возраст для выбранного Id злодея. У порядочить по имени в
        // алфавитном порядке. Если злодея с заданным идентификатором нет, выведите
        // «В базе данных не существует злодея с идентификатором <VillainId>.»
        // Если у выбранного злодея нет миньонов, выведите «(no minions)» во
        // второй строке.

        static void NameOfMinions(int id)
        {
            string selectionVillainName = $"SELECT Name FROM Villains WHERE Id = @id";
            SqlConnection connectionNameOfMinions = new SqlConnection(connectionString);
            SqlCommand commandVillainName = new SqlCommand(selectionVillainName, connectionNameOfMinions);
            SqlParameter parameterVillainName = new SqlParameter("@id", SqlDbType.Int, 50) {Value = id};
            commandVillainName.Parameters.Add(parameterVillainName);
            connectionNameOfMinions.Open();
            using (connectionNameOfMinions)
            {

                SqlDataReader reader = commandVillainName.ExecuteReader();
                using (reader)
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Console.Write($"Villain: {reader["Name"]}");
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine($"No villain with ID {id} exist in the database");
                    }
                }
            }

            string selectionMinionByIdVillain =
                $"SELECT m.Name AS MinionName,m.Age AS MinionAge, v.id AS VillainId " +
                "FROM Minions AS m " +
                "JOIN MinionsVillains AS mv " +
                "ON(m.id = mv.MinionId) JOIN " +
                "Villains AS v ON (v.id = mv.VillainId) " +
                "GROUP BY v.id,m.Name,m.Age " +
                "HAVING v.id = @id ORDER BY m.Name";
            SqlConnection connectionMinionByIdVillain = new SqlConnection(connectionString);
            SqlCommand commandMinionByIdVillain = new SqlCommand(selectionMinionByIdVillain, connectionMinionByIdVillain);
            SqlParameter parameter2 = new SqlParameter("@id", SqlDbType.Int) {Value = id};
            commandMinionByIdVillain.Parameters.Add(parameter2);
            connectionMinionByIdVillain.Open();
            using (connectionMinionByIdVillain)
            {
                SqlDataReader reader = commandMinionByIdVillain.ExecuteReader();
                using (reader)
                {
                    while (reader.Read())
                    {

                        Console.Write($"{reader["MinionName"]} - {reader["MinionAge"]}");
                        Console.WriteLine();
                    }
                }
            }
        }

        // Задание 4. Напишите программу, которая считывает информацию о миньоне и
        // его злодее и добавляет ее в базу данных. В случае, если город миньона
        // отсутствует в базе данных, вставьте его также. В случае, если злодей
        // отсутствует в базе данных, добавьте его тоже со степенью злобы по умолчанию
        // «зло». Наконец, добавьте нового миньона, чтобы он стал слугой злодея. Выводите
        // соответствующие сообщения после каждой операции. Ввод Входные данные поступают в двух
        // строках:
        // • В первой строке вы получаете информацию о миньоне в формате «Minion: <имя> <возраст> <названиегорода>»
        // •Во второй-информация о злодее в формате «Villain: <имя>» Вывод
        // После завершения операции необходимо распечатать одно из следующих сообщений:
        //•После добавления нового города в базу данных: «Город <TownName> был добавлен в базу данных.»
        //•После добавления нового злодея в базу данных: «Злодей <имя злодея> был добавлен в базу данных.» 
        //•Наконец, после успешного добавления миньона в базу данных и превращения его в слугу злодея:
        //«Успешно добавлен <MinionName>, чтобы быть миньоном <VillainName>.»

        static void AddMinions(string minionName, int minionAge, string town, string villainName)
        {
            string selectionCommandString =
                $"SELECT m.Name, m.Age, t.Name " +
                "FROM Minions AS m " +
                "JOIN Towns AS t " +
                "ON(m.Townid = t.id) " +
                "WHERE m.Name = @MinionName " +
                "AND m.Age = @MinionAge " +
                "AND t.Name = @Town";
            SqlConnection connectionMinion = new SqlConnection(connectionString);
            SqlCommand commandMinion = new SqlCommand(selectionCommandString, connectionMinion);
            commandMinion.Parameters.AddWithValue("@MinionName", minionName);
            commandMinion.Parameters.AddWithValue("@MinionAge", minionAge);
            commandMinion.Parameters.AddWithValue("@Town", town);
            connectionMinion.Open();
            using (connectionMinion)
            {
                SqlDataReader reader = commandMinion.ExecuteReader();
                using (reader)
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {

                            // Console.Write($"Villian: {reader["Name"]} ");
                            Console.WriteLine("Minion exist");
                        }
                    }

                    else
                    {
                        reader.Close();
                        connectionMinion.Close();
                        SqlConnection connectionTown = new SqlConnection(connectionString);
                        string selectionCommandStringTown =
                            $"SELECT t.Name, t.id AS TownId FROM Towns AS t WHERE t.Name = @Town";
                        SqlCommand commandTown = new SqlCommand(selectionCommandStringTown, connectionTown);
                        commandTown.Parameters.AddWithValue("@Town", town);
                        connectionTown.Open();
                        using (connectionTown)
                        {
                            SqlDataReader readerTown = commandTown.ExecuteReader();
                            int townid = 0;
                            using (readerTown)
                            {
                                if (readerTown.HasRows)
                                {
                                    while (readerTown.Read())
                                    {
                                        SqlCommand commandAdding = new SqlCommand(
                                            "INSERT INTO Minions " +
                                            "(Name, Age, TownId) VALUES " +
                                            "(@MinionName, @MinionAge, @Townid)", connectionTown);

                                        commandAdding.Parameters.AddWithValue("@MinionName", minionName);
                                        commandAdding.Parameters.AddWithValue("@MinionAge", minionAge);
                                        commandAdding.Parameters.AddWithValue("@Townid", readerTown["TownId"]);
                                        townid = (int) readerTown["TownId"];
                                        readerTown.Close();
                                        commandAdding.ExecuteNonQuery();

                                        Console.Write($"{minionName} add successfully to be a minion ");
                                    }

                                }
                                else
                                {
                                    SqlCommand commandAddingTown = new SqlCommand(
                                        "INSERT INTO Towns" + "(Name) VALUES" + "(@Town)",
                                        connectionTown);
                                    commandAddingTown.Parameters.AddWithValue("@Town", town);
                                    readerTown.Close();
                                    commandAddingTown.ExecuteNonQuery();
                                    Console.WriteLine($"Town {town} was added to data base.");

                                    SqlCommand commandAdding = new SqlCommand(
                                        "INSERT INTO Minions " +
                                        "(Name, Age, TownId) VALUES " +
                                        "(@MinionName, @MinionAge, @Townid)", connectionTown);

                                    commandAdding.Parameters.AddWithValue("@MinionName", minionName);
                                    commandAdding.Parameters.AddWithValue("@MinionAge", minionAge);
                                    commandAdding.Parameters.AddWithValue("@Townid", townid);

                                    commandAdding.ExecuteNonQuery();

                                    Console.Write($"{minionName} add successfully to be a minion ");

                                }
                            }
                        }

                    }
                }
            }

            int minionId = 0;
            string selectionCommandStringMinionId = $"SELECT m.id AS MinionId FROM Minions AS m WHERE Name = @MinionName";
            SqlConnection connectionMinionId = new SqlConnection(connectionString);
            SqlCommand commandMinionId = new SqlCommand(selectionCommandStringMinionId, connectionMinionId);
            commandMinionId.Parameters.AddWithValue("@MinionName", minionName);
            connectionMinionId.Open();
            using (connectionMinionId)
            {
                SqlDataReader readerMinionId = commandMinionId.ExecuteReader();

                using (readerMinionId)
                {
                    while (readerMinionId.Read())
                    {
                        minionId = (int) readerMinionId["MinionId"];
                    }
                }

                readerMinionId.Close();
            }

            int villianId = 0;
            string selectionCommandStringVillainId =
                $"SELECT v.Name,v.id AS VillainId FROM Villains AS v WHERE Name = @VillainName";
            SqlConnection connectionVillainId = new SqlConnection(connectionString);
            SqlCommand commandVillainId = new SqlCommand(selectionCommandStringVillainId, connectionVillainId);
            commandVillainId.Parameters.AddWithValue("@VillainName", villainName);
            connectionVillainId.Open();
            using (connectionVillainId)
            {
                SqlDataReader readerVillainId = commandVillainId.ExecuteReader();
                using (readerVillainId)
                {
                    if (readerVillainId.HasRows)
                    {
                        while (readerVillainId.Read())
                        {
                            villianId = (int) readerVillainId["VillainId"];
                            SqlCommand commandRelations = new SqlCommand("INSERT INTO MinionsVillains " + "" +
                                                                 "(MinionId,VillainId) VALUES " +
                                                                 "(@Minionid, @Villainid)", connectionVillainId);
                            commandRelations.Parameters.AddWithValue("@Minionid", minionId);
                            commandRelations.Parameters.AddWithValue("@Villainid", villianId);
                            readerVillainId.Close();
                            commandRelations.ExecuteNonQuery();
                            Console.WriteLine($"{villainName} exist");
                        }
                    }
                    else
                    {
                        SqlCommand commandVillainName = new SqlCommand("INSERT INTO Villains" +
                                                             "(Name) VALUES " +
                                                             "(@VillainName)", connectionVillainId);
                        commandVillainName.Parameters.AddWithValue("@VillianName", villainName);
                        readerVillainId.Close();
                        commandVillainName.ExecuteNonQuery();

                        villianId = 0;
                        string selectionCommandStringAddingName =
                            $"SELECT v.id AS VillainId FROM Villains AS v WHERE Name = @VillainName";
                        SqlConnection connectionAddingName = new SqlConnection(connectionString);
                        SqlCommand commandAddingName = new SqlCommand(selectionCommandStringAddingName, connectionAddingName);
                        commandAddingName.Parameters.AddWithValue("@VillainName", villainName);
                        connectionAddingName.Open();
                        using (connectionAddingName)
                        {
                            SqlDataReader readerAddingName = commandAddingName.ExecuteReader();

                            using (readerAddingName)
                            {
                                while (readerAddingName.Read())
                                {
                                    villianId = (int) readerAddingName["VillainId"];
                                }
                            }

                            SqlCommand commandRelation = new SqlCommand("INSERT INTO MinionsVillains " + "" +
                                                                 "(Minionid,Villainid) VALUES " +
                                                                 "(@Minionid, @Villainid)", connectionAddingName);
                            commandRelation .Parameters.AddWithValue("@Minionid", minionId);
                            commandRelation .Parameters.AddWithValue("@Villainid", villianId);
                            commandRelation .ExecuteNonQuery();
                            // Console.Write($@"{VillainName}");
                            Console.WriteLine($"Villain {villainName} was added to base.");
                        }
                    }
                }
            }
        }


        // Задание 5. Ваша программа получает идентификатор (id) злодея, удаляет его из базы данных
        // и освобождает его миньонов от служения ему. Выведите имя удаленного злодея в формате «<имязлодея> был удален.»
        // и количество освобожденных миньонов,в формате «<кол-во>миньонов было освобождено.». Если в базе данных нет злодея
        // с заданным идентификатором, выведите «Такой злодей не найден.»

        static void DeleteVillain(int id)
        {
            string selectionVillainName = $"SELECT v.id,v.Name AS VillainName FROM Villains AS v WHERE Id = @id";
            SqlConnection connectionDeleteVillain = new SqlConnection(connectionString);
            SqlCommand commandDeleteVillain = new SqlCommand(selectionVillainName, connectionDeleteVillain);
            commandDeleteVillain.Parameters.AddWithValue("@id", id);
            connectionDeleteVillain.Open();
            using (connectionDeleteVillain)
            {
                SqlDataReader reader = commandDeleteVillain.ExecuteReader();
                using (reader)
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"Villain {reader["VillainName"]} was deleted ");
                        }

                        reader.Close();
                            string selectionCountMinion = $"SELECT COUNT(mv.MinionId) AS CountMin " +
                                                          "FROM MinionsVillains AS mv " +
                                                          "WHERE mv.VillainId = @VillainId";
                            SqlConnection connectionCountMinion = new SqlConnection(connectionString);
                            SqlCommand commandCountMinion = new SqlCommand(selectionCountMinion, connectionCountMinion);
                            commandCountMinion.Parameters.AddWithValue("@VillainId", id);
                            connectionCountMinion.Open();
                            using (connectionCountMinion)
                            {
                                SqlDataReader readerCountMinion = commandCountMinion.ExecuteReader();

                                using (readerCountMinion)
                                {
                                    if (readerCountMinion.HasRows) 
                                    {
                                        while (readerCountMinion.Read())
                                        {
                                            Console.WriteLine($"{readerCountMinion["CountMin"]} minions freed");
                                        }

                                        SqlCommand commandDeleteMinionRelation = new SqlCommand(
                                                "DELETE FROM MinionsVillains " +
                                                "WHERE MinionsVillains.VillainId = @id", connectionCountMinion);
                                            commandDeleteMinionRelation.Parameters.AddWithValue("@id", id);
                                            readerCountMinion.Close();
                                            commandDeleteMinionRelation.ExecuteNonQuery();
                                            SqlCommand commandDeleteVillainRelation = new SqlCommand(
                                                "DELETE FROM Villains " +
                                                "WHERE Villains.id = @id", connectionCountMinion);
                                            commandDeleteVillainRelation.Parameters.AddWithValue("@id", id);
                                            commandDeleteVillainRelation.ExecuteNonQuery();
                                            Console.WriteLine();
                                        
                                    }
                                    else
                                    {
                                        Console.WriteLine("Villain hasn't minions");
                                    }
                                }
                            }

                        
                    }
                    else
                    {
                        Console.WriteLine("Villain hasn't exist");
                    }
                }
            }
        }

        // Задание 6. Ваша программа получает идентификаторы (id) миньонов через разделитель–пробел.
    // Увеличьте возраст всех указанных миньонов на 1.Далее выведите всех миньонов из базы данных
    // в формате:«<Имя> <Возраст>»

    static void YearsWent()
        {
            var ids = Console.ReadLine().Split(" ").Select(e => Convert.ToInt32(e)).ToList();
            for (int i = 0; i < ids.Count; i++)
            {

                string selectionCommandString = $"UPDATE Minions " +
                                                "SET Age = Age + 1 " +
                                                "WHERE Id = @id";
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(selectionCommandString, connection);
                command.Parameters.AddWithValue("@id", ids[i]);

                connection.Open();
                using (connection)
                {
                    SqlDataReader reader = command.ExecuteReader();
                    using (reader)
                    {
                        while (reader.Read())
                        {

                        }
                    }
                }
            }

            string selectionMinions = $"SELECT * FROM Minions ";
            SqlConnection connectionMinions = new SqlConnection(connectionString);
            SqlCommand commandMinions = new SqlCommand(selectionMinions, connectionMinions);
            connectionMinions.Open();
            using (connectionMinions)
            {
                SqlDataReader readerMinions = commandMinions.ExecuteReader();
                using (readerMinions)
                {
                    while (readerMinions.Read())
                    {
                        Console.WriteLine($"{readerMinions["Name"]} - {readerMinions["Age"]}");
                    }
                }
            }
        }
    }
}

    