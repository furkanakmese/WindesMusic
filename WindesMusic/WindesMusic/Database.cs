using System;
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
        private DbProviderFactory _factory;
        private DbConnection _connection;
        private DbCommand _command;
        private DbDataReader _reader;

        public Database()
        {
            _provider = ConfigurationManager.AppSettings["provider"];
            _connectionString = ConfigurationManager.AppSettings["connectionString"];
            _factory = DbProviderFactories.GetFactory(_provider);
            _connection = _factory.CreateConnection();
            _command = _factory.CreateCommand();
            _command.Connection = _connection;
        }

        public void OpenConnection()
        {
            if (_connection == null)
            {
                System.Console.WriteLine("Connection problems");
            }
            _connection.ConnectionString = _connectionString;
            _connection.Open();
        }

        public void SetValues(string Query)
        {
            OpenConnection();
            _command.CommandText = Query;
            _reader = _command.ExecuteReader();

            _connection.Close();
        }

        public List<int> GetRecordsInt(string Query, string ReturnItems)
        {
            OpenConnection();
            List<int> results = new List<int>();
            _command.CommandText = Query;
            _reader = _command.ExecuteReader();

            while (_reader.Read())
            {
                results.Add((int)_reader[$"{ReturnItems}"]);
            }

            _connection.Close();
            return results;
        }

        public List<string> GetRecordsString(string Query, string ReturnItems)
        {
            OpenConnection();
            List<string> results = new List<string>();
            _command.CommandText = Query;
            _reader = _command.ExecuteReader();

            while (_reader.Read())
            {
                results.Add((string)_reader[$"{ReturnItems}"]);
            }

            _connection.Close();
            return results;
        }

        // this function sends the login data to the database and returns a user object, empty if the data is wrong
        public User Login(string email, string password)
        {
            OpenConnection();
            _command.Parameters.Clear();
            User userResult = new User();
            _command.CommandText = "SELECT * FROM Users WHERE Email=@email AND Password=@password";

            var emailParam = _command.CreateParameter();
            emailParam.ParameterName = "@email";
            emailParam.Value = email;

            var sha1 = new SHA1CryptoServiceProvider();
            var data = Encoding.ASCII.GetBytes(password);
            var sha1data = sha1.ComputeHash(data);

            var passwordParam = _command.CreateParameter();
            passwordParam.ParameterName = "@password";
            passwordParam.Value = Encoding.ASCII.GetString(sha1data);

            _command.Parameters.Add(emailParam);
            _command.Parameters.Add(passwordParam);
            _reader = _command.ExecuteReader();

            if (_reader.Read())
            {
                userResult.Id = (int)_reader["Id"];
                userResult.Email = (string)_reader["Email"];
                userResult.Name = (string)_reader["Name"];
                userResult.IsArtist = (int)_reader["IsArtist"];
                // save user id to application settings
                Properties.Settings.Default.UserID = userResult.Id;
                Properties.Settings.Default.Save();
            }
            _connection.Close();
            return userResult;
        }

        public User Register(string name, string email, string password)
        {
            OpenConnection();
            _command.Parameters.Clear();

            _command.CommandText = "SELECT * FROM Users WHERE Email=@email";
            var emailParam = _command.CreateParameter();
            emailParam.ParameterName = "@email";
            emailParam.Value = email;
            _command.Parameters.Add(emailParam);

            _reader = _command.ExecuteReader();
            if(_reader.Read())
            {
                return new User();
            }
            _reader.Close();

            _command.CommandText = "INSERT INTO Users(Name, Email, Password) VALUES (@name, @email, @password)";

            var nameParam = _command.CreateParameter();
            nameParam.ParameterName = "@name";
            nameParam.Value = name;

            var sha1 = new SHA1CryptoServiceProvider();
            var data = Encoding.ASCII.GetBytes(password);
            var sha1data = sha1.ComputeHash(data);

            var passwordParam = _command.CreateParameter();
            passwordParam.ParameterName = "@password";
            passwordParam.Value = Encoding.ASCII.GetString(sha1data);

            _command.Parameters.Add(nameParam);
            _command.Parameters.Add(passwordParam);
            // _reader = _command.ExecuteReader();
            
            if(_command.ExecuteNonQuery() > 0)
            {
                _connection.Close();
                return Login(email, password);
            } else
            {
                _connection.Close();
                return new User();
            }
        }

        // method for retrieving user data after login or on startup using ID
        public User GetUserData(int id)
        {
            OpenConnection();
            _command.Parameters.Clear();
            User userResult = new User();
            _command.CommandText = "SELECT * FROM Users LEFT JOIN Playlist ON Id=UserID WHERE Id=@id";

            var idParam = _command.CreateParameter();
            idParam.ParameterName = "@id";
            idParam.Value = id;
            _command.Parameters.Add(idParam);
            _reader = _command.ExecuteReader();

            while (_reader.Read())
            {
                Playlist playlistResult = new Playlist();
                userResult.Id = (int)_reader["Id"];
                userResult.Email = (string)_reader["Email"];
                userResult.Name = (string)_reader["Name"];
                userResult.IsArtist = (int)_reader["IsArtist"];
                try
                {
                    playlistResult.PlaylistID = (int)_reader["PlaylistID"];
                    playlistResult.PlaylistName = (string)_reader["PlaylistName"];
                    userResult.Playlists.Add(playlistResult);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            _connection.Close();
            foreach(Playlist playlist in userResult.Playlists)
            {
                playlist.SongPlaylist = GetSongsInPlaylist(playlist.PlaylistID);
            }
            return userResult;

        }

        // method to retieve search results
        public List<Song> GetSearchResults(string criteria)
        {
            OpenConnection();
            _command.Parameters.Clear();
            List<Song> listResult = new List<Song>();
            _command.CommandText = "SELECT * FROM Song WHERE Name LIKE '%' + @criteria + '%' OR Artist LIKE '%' + @criteria + '%' OR Album LIKE '%' + @criteria + '%'";

            var criteriaParam = _command.CreateParameter();
            criteriaParam.ParameterName = "@criteria";
            criteriaParam.Value = criteria;
            _command.Parameters.Add(criteriaParam);
            _reader = _command.ExecuteReader();

            while (_reader.Read())
            {
                Song searchResult = new Song();
                searchResult.SongID = (int)_reader["SongID"];
                searchResult.SongName = (string)_reader["Name"];
                searchResult.Artist = (string)_reader["Artist"];
                searchResult.Album = (string)_reader["Album"];
                searchResult.Year = (int)_reader["Year"];
                listResult.Add(searchResult);
            }

            _connection.Close();
            return listResult;
        }

        public List<Song> GetSongsInPlaylist(int PlaylistID)
        {
            OpenConnection();
            _command.Parameters.Clear();
            List<Song> listResult = new List<Song>();
            _command.CommandText = "SELECT * FROM Song WHERE SongID IN(SELECT SongID FROM PlaylistToSong WHERE PlaylistID = @PlaylistID)";

            var criteriaParam = _command.CreateParameter();
            criteriaParam.ParameterName = "@PlaylistID";
            criteriaParam.Value = PlaylistID;
            _command.Parameters.Add(criteriaParam);
            _reader = _command.ExecuteReader();

            while (_reader.Read())
            {
                Song searchResult = new Song();
                searchResult.SongID = (int)_reader["SongID"];
                searchResult.SongName = (string)_reader["Name"];
                searchResult.Artist = (string)_reader["Artist"];
                searchResult.Album = (string)_reader["Album"];
                searchResult.Year = (int)_reader["Year"];
                searchResult.Genre = (string)_reader["Genre"];
                listResult.Add(searchResult);
            }

            _connection.Close();
            return listResult;
        }

        public List<Song> GetRecommendedSongsForPlaylist(string genre, int playlistID)
        {

            OpenConnection();
            _command.Parameters.Clear();
            List<Song> listResult = new List<Song>();
            _command.CommandText = "SELECT TOP 5 * FROM Song WHERE Genre = 'Pop' AND SongID NOT IN (SELECT SongID FROM PlayList WHERE PlaylistID = @PlaylistID) ORDER BY NewID()";

            var genreParm = _command.CreateParameter();
            genreParm.ParameterName = "@genre";
            genreParm.Value = genre;
            _command.Parameters.Add(genreParm);
            _reader = _command.ExecuteReader();

            var criteriaParam = _command.CreateParameter();
            criteriaParam.ParameterName = "@playlistID";
            criteriaParam.Value = playlistID;
            _command.Parameters.Add(criteriaParam);
            _reader = _command.ExecuteReader();

            while (_reader.Read())
            {
                Song searchResult = new Song();
                searchResult.SongID = (int)_reader["SongID"];
                searchResult.SongName = (string)_reader["Name"];
                searchResult.Artist = (string)_reader["Artist"];
                searchResult.Album = (string)_reader["Album"];
                searchResult.Year = (int)_reader["Year"];
                searchResult.Genre = (string)_reader["Genre"];
                listResult.Add(searchResult);
            }

            _connection.Close();
            return listResult;
        }

        public void CreateNewPlaylist(string Name, int UserID)
        {
            OpenConnection();
            _command.Parameters.Clear();
            _command.CommandText = "INSERT INTO Playlist(PlaylistName, UserID) VALUES(@Name, @UserID)";

            var criteriaParamName = _command.CreateParameter();
            criteriaParamName.ParameterName = "@Name";
            criteriaParamName.Value = Name;
            _command.Parameters.Add(criteriaParamName);

            var criteriaParamUserID = _command.CreateParameter();
            criteriaParamUserID.ParameterName = "@UserID";
            criteriaParamUserID.Value = UserID;
            _command.Parameters.Add(criteriaParamUserID);

            _command.ExecuteNonQuery();
        }

        public void ChangePlaylistName(int PlaylistID, string Name)
        {
            OpenConnection();
            _command.Parameters.Clear();
            _command.CommandText = "UPDATE Playlist SET Name=@Name WHERE PlaylistID = @PlaylistID";

            var criteriaParamName = _command.CreateParameter();
            criteriaParamName.ParameterName = "@Name";
            criteriaParamName.Value = Name;
            _command.Parameters.Add(criteriaParamName);

            var criteriaParamPlaylistID = _command.CreateParameter();
            criteriaParamPlaylistID.ParameterName = "@PlaylistID";
            criteriaParamPlaylistID.Value = PlaylistID;
            _command.Parameters.Add(criteriaParamPlaylistID);

            _command.ExecuteNonQuery();
            _connection.Close();
        }

        public void DeletePlaylist(int PlaylistID)
        {
            OpenConnection();
            _command.Parameters.Clear();
            _command.CommandText = "DELETE FROM PlaylistToSong WHERE PlaylistID = @PlaylistID";

            var criteriaParamPlaylistID = _command.CreateParameter();
            criteriaParamPlaylistID.ParameterName = "@PlaylistID";
            criteriaParamPlaylistID.Value = PlaylistID;
            _command.Parameters.Add(criteriaParamPlaylistID);

            _command.ExecuteNonQuery();

            _command.Parameters.Clear();
            _command.CommandText = "DELETE FROM Playlist WHERE PlaylistID = @PlaylistID";
            _command.Parameters.Add(criteriaParamPlaylistID);

            _command.ExecuteNonQuery();
            _connection.Close();
        }

        public void AddSongToPlaylist(int PlaylistID, int SongID)
        {
            OpenConnection();
            _command.Parameters.Clear();
            _command.CommandText = "INSERT INTO PlaylistToSong(PlaylistID, SongID) VALUES (@PlaylistID, @SongID)";

            var criteriaParamPlaylistID = _command.CreateParameter();
            criteriaParamPlaylistID.ParameterName = "@PlaylistID";
            criteriaParamPlaylistID.Value = PlaylistID;
            _command.Parameters.Add(criteriaParamPlaylistID);

            var criteriaParamSongID = _command.CreateParameter();
            criteriaParamSongID.ParameterName = "@SongID";
            criteriaParamSongID.Value = SongID;
            _command.Parameters.Add(criteriaParamSongID);

            _command.ExecuteNonQuery();
            _connection.Close();
        }

        public void RemoveSongFromPlaylist(int PlaylistID, int SongID)
        {
            OpenConnection();
            _command.Parameters.Clear();
            _command.CommandText = "DELETE FROM PlaylistToSong WHERE PlaylistID = @PlaylistID AND SongID = @SongToDelete";

            var criteriaParamPlaylistID = _command.CreateParameter();
            criteriaParamPlaylistID.ParameterName = "@PlaylistID";
            criteriaParamPlaylistID.Value = PlaylistID;
            _command.Parameters.Add(criteriaParamPlaylistID);

            var criteriaParamSongID = _command.CreateParameter();
            criteriaParamSongID.ParameterName = "@SongID";
            criteriaParamSongID.Value = SongID;
            _command.Parameters.Add(criteriaParamPlaylistID);

            _command.ExecuteNonQuery();
            _connection.Close();
        }
        

        public string GetNameFromPlaylist(int PlaylistID)
        {
            string Result = "";
            OpenConnection();
            _command.Parameters.Clear();
            _command.CommandText = "SELECT PlaylistName FROM Playlist WHERE PlaylistID = @PlaylistID";

            var criteriaParamPlaylistID = _command.CreateParameter();
            criteriaParamPlaylistID.ParameterName = "@PlaylistID";
            criteriaParamPlaylistID.Value = PlaylistID;
            _command.Parameters.Add(criteriaParamPlaylistID);

            _reader = _command.ExecuteReader();

            while (_reader.Read())
            {
                Result = (string)_reader["PlaylistName"];
            }

            _connection.Close();
            return Result;
        }

        public bool RequestArtistStatus()
        {
            int id = Properties.Settings.Default.UserID;

            OpenConnection();
            _command.Parameters.Clear();
            _command.CommandText = "UPDATE Users SET IsArtist=1 WHERE Id=@id";

            var idParam = _command.CreateParameter();
            idParam.ParameterName = "@id";
            idParam.Value = id;
            _command.Parameters.Add(idParam);

            if (_command.ExecuteNonQuery() > 0)
            {
                _connection.Close();
                return true;
            }
            else
            {
                _connection.Close();
                return false;
            }
        }
    }
}
