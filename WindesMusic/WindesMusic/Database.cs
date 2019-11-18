﻿using System;
using System.Collections.Generic; 
using System.Configuration; 
using System.Data.Common; 
using System.Linq;
using System.Security.Cryptography;
using System.Text; 
using System.Threading.Tasks; 
 
namespace WindesMusic
{
    public class Database
    {
        private string _provider;
        private string _connectionString;
        private DbProviderFactory factory;
        private DbConnection _connection;

        public Database()
        {
            _provider = ConfigurationManager.AppSettings["provider"];
            _connectionString = ConfigurationManager.AppSettings["connectionString"];

            factory = DbProviderFactories.GetFactory(_provider);
        }
        public void SetValues(string Query)
        {
            using (_connection = factory.CreateConnection())
            {
                if (_connection == null)
                {
                System.Console.WriteLine("Connection problems");
                }
                _connection.ConnectionString = _connectionString;

                _connection.Open();
                DbCommand command = factory.CreateCommand();
                List<int> results = new List<int>();

                command.Connection = _connection;
                command.CommandText = Query;
                DbDataReader reader = command.ExecuteReader();

                _connection.Close();
            }
        }
        public List<int> GetRecords(string Query)
        {
            using (_connection = factory.CreateConnection())
            {
                if (_connection == null)
                {
                    return null;
                }
                _connection.ConnectionString = _connectionString;

                _connection.Open();
                DbCommand command = factory.CreateCommand();
                List<int> results = new List<int>();

                command.Connection = _connection;
                command.CommandText = Query;

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add((int)reader["SongID"]);
                    }
                }
                _connection.Close();
                return results;
            }
        }
        // this function sends the login data to the database and returns a user object, empty if the data is wrong
        public User Login(string email, string password)
        {
            using (_connection = factory.CreateConnection())
            {
                if (_connection == null)
                {
                    System.Console.WriteLine("Connection problems");
                }
                _connection.ConnectionString = _connectionString;

                _connection.Open();
                DbCommand command = factory.CreateCommand();
                User userResult = new User();    

                command.Connection = _connection;
                command.CommandText = "SELECT * FROM Users WHERE Email=@email AND Password=@password";

                var emailParam = command.CreateParameter();
                emailParam.ParameterName = "@email";
                emailParam.Value = email;

                var sha1 = new SHA1CryptoServiceProvider();
                var data = Encoding.ASCII.GetBytes(password);
                var sha1data = sha1.ComputeHash(data);

                var passwordParam = command.CreateParameter();
                passwordParam.ParameterName = "@password";
                passwordParam.Value = Encoding.ASCII.GetString(sha1data);
                Console.WriteLine(Encoding.ASCII.GetString(sha1data));
                command.Parameters.Add(emailParam);
                command.Parameters.Add(passwordParam);

                DbDataReader reader = command.ExecuteReader();

                if(reader.Read())
                {
                    userResult.Id = (int) reader["Id"];
                    userResult.Email = (string) reader["Email"];
                }
                _connection.Close();
                return userResult;
            }
        }
    }
}