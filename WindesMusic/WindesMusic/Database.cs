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
                userResult.UserID = (int)_reader["UserID"];
                userResult.Email = (string)_reader["Email"];
                userResult.Name = (string)_reader["Name"];
                userResult.IsArtist = Convert.ToBoolean(_reader["IsArtist"]);
                // save user id to application settings
                Properties.Settings.Default.UserID = userResult.UserID;
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

            _command.CommandText = "INSERT INTO Users VALUES (@name, @email, @password, 0, 0, 0, 0)";

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
            _command.CommandText = "SELECT * FROM Users LEFT JOIN Playlist ON Users.UserID = Playlist.UserID WHERE Users.UserID=@id";

            var idParam = _command.CreateParameter();
            idParam.ParameterName = "@id";
            idParam.Value = id;
            _command.Parameters.Add(idParam);
            _reader = _command.ExecuteReader();

            while (_reader.Read())
            {
                Playlist playlistResult = new Playlist();
                userResult.UserID = (int)_reader["UserID"];
                userResult.Email = (string)_reader["Email"];
                userResult.Name = (string)_reader["Name"];
                userResult.IsArtist = Convert.ToBoolean(_reader["IsArtist"]);
                try
                {
                    playlistResult.PlaylistID = (int)_reader["PlaylistID"];
                    playlistResult.PlaylistName = (string)_reader["PlaylistName"];
                    userResult.Playlists.Add(playlistResult);
                }
                catch (Exception e) { Console.WriteLine(e); }

            }
            _connection.Close();

            if (userResult.IsArtist == true)
            {
                OpenConnection();
                _command.CommandText = "SELECT * FROM Song s LEFT JOIN Users ON Users.UserID=Song.UserID WHERE Users.UserID=@id LEFT JOIN Album a on s.AlbumID = a.AlbumID";
                _reader = _command.ExecuteReader();

                while (_reader.Read())
                {
                    try
                    {
                        Song song = new Song();
                        song.SongID = (int)_reader["SongID"];
                        song.SongName = (string)_reader["Name"];
                        song.Artist = (string)_reader["Artist"];
                        song.Album = (string)_reader["AlbumName"];
                        song.Genre = (string)_reader["Genre"];
                        song.Subgenre = (string)_reader["SubGenre"]; 
                        song.UserID = (int)_reader["UserID"];
                        userResult.Songs.Add(song);
                    }
                    catch (Exception e) { Console.WriteLine(e); }
                }
                _connection.Close();
            }
            
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
            _command.CommandText = "SELECT s.SongID, s.Name, s.Artist, s.Genre, a.AlbumName, s.Year FROM Song s left join Album a on s.AlbumID=a.AlbumID WHERE Name LIKE '%' + @criteria + '%' OR Artist LIKE '%' + @criteria + '%' OR a.AlbumName LIKE '%' + @criteria + '%'";

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
                searchResult.Album = (string)_reader["AlbumName"];
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
            _command.CommandText = "SELECT * FROM Song s LEFT JOIN Album a ON s.AlbumID = a.AlbumID WHERE SongID IN(SELECT SongID FROM PlaylistToSong WHERE PlaylistID = @PlaylistID)";

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
                searchResult.Album = (string)_reader["AlbumName"];
                searchResult.Year = (int)_reader["Year"];
                searchResult.Genre = (string)_reader["Genre"];
                searchResult.Subgenre = (string)_reader["Subgenre"];
                listResult.Add(searchResult);
            }

            _connection.Close();
            return listResult;
        }

        //Return a list of songs for the playlist recommender
        public List<Song> GetRecommendedSongsForPlaylist(string mostCommonGenre, string secondMostCommonGenre, int playlistID)
        {

            OpenConnection();
            _command.Parameters.Clear();
            List<Song> listResult = new List<Song>();
            if (secondMostCommonGenre == "")
            {
                _command.CommandText = "SELECT TOP 5 * FROM Song s LEFT JOIN Album a ON s.AlbumID = a.AlbumID WHERE Subgenre = @mostCommonGenre AND SongID NOT IN (SELECT SongID FROM PlayListToSong WHERE PlaylistID = @playlistID) ORDER BY NewID()";
            }
            else
            {
                _command.CommandText = "SELECT TOP 5 * FROM Song s LEFT JOIN Album a ON s.AlbumID = a.AlbumID WHERE Subgenre IN(@mostCommonGenre, @secondMostCommonGenre) AND SongID NOT IN (SELECT SongID FROM PlayListToSong WHERE PlaylistID = @playlistID) ORDER BY NewID()";
            }

            var mostCommonGenreParam = _command.CreateParameter();
            mostCommonGenreParam.ParameterName = "@mostCommonGenre";
            mostCommonGenreParam.Value = mostCommonGenre;
            _command.Parameters.Add(mostCommonGenreParam);

            var secondMostCommonGenreParam = _command.CreateParameter();
            secondMostCommonGenreParam.ParameterName = "@secondMostCommonGenre";
            secondMostCommonGenreParam.Value = secondMostCommonGenre;
            _command.Parameters.Add(secondMostCommonGenreParam);

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
                searchResult.Album = (string)_reader["AlbumName"];
                searchResult.Year = (int)_reader["Year"];
                searchResult.Genre = (string)_reader["Genre"];
                searchResult.Subgenre = (string)_reader["SubGenre"];
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
            _command.CommandText = "UPDATE Users SET IsArtist=1 WHERE UserID=@id";

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
      
        public void AddSongToHistory(int userID, int songID, int timeListened)
        {
            OpenConnection();
            _command.Parameters.Clear();
            _command.CommandText = "INSERT INTO History VALUES (@UserID, @DateTime, @SongID, @TimeListened)";

            var userIDParam = _command.CreateParameter();
            userIDParam.ParameterName = "@UserID";
            userIDParam.Value = userID;
            _command.Parameters.Add(userIDParam);

            var dateTimeParam = _command.CreateParameter();
            dateTimeParam.ParameterName = "@DateTime";
            dateTimeParam.Value = DateTime.Now;
            _command.Parameters.Add(dateTimeParam);

            var songIDParam = _command.CreateParameter();
            songIDParam.ParameterName = "@SongID";
            songIDParam.Value = songID;
            _command.Parameters.Add(songIDParam);

            var timeListenedParam = _command.CreateParameter();
            timeListenedParam.ParameterName = "@TimeListened";
            timeListenedParam.Value = timeListened;
            _command.Parameters.Add(timeListenedParam);

            _command.ExecuteNonQuery();
            _connection.Close();
        }

        public object SaveGeneratedPlaylist(string name, int userID)
        {
            OpenConnection();
            _command.Parameters.Clear();
            _command.CommandText = "SELECT DateGenerated FROM GeneratedPlaylist WHERE UserID=@ID";

            var idParam = _command.CreateParameter();
            idParam.ParameterName = "@ID";
            idParam.Value = userID;
            _command.Parameters.Add(idParam);

            _reader = _command.ExecuteReader();
            DateTime date = new DateTime();
            while (_reader.Read())
            {
                date = (DateTime)_reader["DateGenerated"];
            }
            string[] split = date.ToString().Split(' ');
            string dayMonthYearFromDb = split[0];
            _reader.Close();

            DateTime currentDate = DateTime.Now;
            split = currentDate.ToString().Split(' ');
            string dayMonthYearCurrent = split[0];

            if (dayMonthYearFromDb != dayMonthYearCurrent)
            {
                _command.Parameters.Clear();
                _command.CommandText = "INSERT INTO GeneratedPlaylist OUTPUT INSERTED.PlaylistID VALUES(@Name, @userID, @Date)";

                var nameParam = _command.CreateParameter();
                nameParam.ParameterName = "@Name";
                nameParam.Value = name;
                _command.Parameters.Add(nameParam);

                var userIDParam = _command.CreateParameter();
                userIDParam.ParameterName = "@userID";
                userIDParam.Value = userID;
                _command.Parameters.Add(userIDParam);

                var dateTimeParam = _command.CreateParameter();
                dateTimeParam.ParameterName = "@Date";
                dateTimeParam.Value = currentDate;
                _command.Parameters.Add(dateTimeParam);

                int id = (Int32)_command.ExecuteScalar();

                _connection.Close();
                return id;
            }
            _connection.Close();
            Console.WriteLine($"Playlist has already been created for today {dayMonthYearCurrent}");
            return null;
        }

        public void SaveGeneratedPlaylistToSong(List<Song> songList, int playlistID)
        {
            OpenConnection();
            foreach (Song song in songList)
            {
                _command.Parameters.Clear();
                _command.CommandText = "INSERT INTO GeneratedPlaylistToSong VALUES(@PlaylistID, @SongID)";

                var playlistIDParam = _command.CreateParameter();
                playlistIDParam.ParameterName = "@PlaylistID";
                playlistIDParam.Value = playlistID;
                _command.Parameters.Add(playlistIDParam);

                var songIDParam = _command.CreateParameter();
                songIDParam.ParameterName = "@SongID";
                songIDParam.Value = song.SongID;
                _command.Parameters.Add(songIDParam);
                _command.ExecuteNonQuery();
            }
            _connection.Close();
        }

        public List<Song> getGeneratedPlaylistSongs(int playlistID)
        {
            OpenConnection();
            _command.Parameters.Clear();
            _command.CommandText = "SELECT * FROM Song WHERE SongID IN (SELECT SongID FROM GeneratedPlaylistToSong WHERE PlaylistID=@PlaylistID)";

            var playlistIDParam = _command.CreateParameter();
            playlistIDParam.ParameterName = "@PlaylistID";
            playlistIDParam.Value = playlistID;
            _command.Parameters.Add(playlistIDParam);

            _reader = _command.ExecuteReader();
            List<Song> listResult = new List<Song>();
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

        public Playlist GetHistoryPlaylist(int userID)
        {
            OpenConnection();
            _command.Parameters.Clear();
            _command.CommandText = "SELECT MAX(PlaylistID), PlaylistName FROM GeneratedPlaylist WHERE UserID = @UserID GROUP BY PlaylistID, PlaylistName";

            var IdParam = _command.CreateParameter();
            IdParam.ParameterName = "@UserID";
            IdParam.Value = userID;
            _command.Parameters.Add(IdParam); ;

            _reader = _command.ExecuteReader();
            Playlist playlist = new Playlist();
            while (_reader.Read())
            {
                playlist.PlaylistID = (int)_reader[0];
                playlist.PlaylistName = (string)_reader[1];
            }
            _connection.Close();
            return playlist;
        }

        public List<Song> GetPlayHistory(int userID)
        {
            OpenConnection();
            _command.Parameters.Clear();
            _command.CommandText = "SELECT Song.SongID, Song.Name, Song.Artist, Song.Album, Song.Year, Song.Genre " +
                "FROM Song LEFT JOIN History ON Song.SongID=History.SongID WHERE History.UserID=@ID " +
                "GROUP BY Song.SongID, Song.Name, Song.Artist, Song.Album, Song.Year, Song.Genre";

            var idParam = _command.CreateParameter();
            idParam.ParameterName = "@ID";
            idParam.Value = userID;
            _command.Parameters.Add(idParam); ;

            _reader = _command.ExecuteReader();
            List<Song> listResult = new List<Song>();
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

        public Song getSong(int songID)
        {
            OpenConnection();
            _command.Parameters.Clear();
            _command.CommandText = "SELECT * FROM Song  WHERE SongID=@ID";

            var idParam = _command.CreateParameter();
            idParam.ParameterName = "@ID";
            idParam.Value = songID;
            _command.Parameters.Add(idParam); ;

            _reader = _command.ExecuteReader();
            Song searchResult = new Song();
            while (_reader.Read())
            {
                searchResult.SongID = (int)_reader["SongID"];
                searchResult.SongName = (string)_reader["Name"];
                searchResult.Artist = (string)_reader["Artist"];
                searchResult.Album = (string)_reader["AlbumName"];
                searchResult.Year = (int)_reader["Year"];
                searchResult.Genre = (string)_reader["Genre"];
            }
            _connection.Close();
            return searchResult;
        }

        public string SubmitSongForAdvertising(int songId, int userId)
        {
            OpenConnection();
            _command.Parameters.Clear();

            _command.CommandText = "SELECT * FROM AdvertisedSong WHERE SongID=@SongID";

            var songIdParam = _command.CreateParameter();
            songIdParam.ParameterName = "@SongID";
            songIdParam.Value = songId;
            _command.Parameters.Add(songIdParam);

            _reader = _command.ExecuteReader();
            if (_reader.Read())
            {
                _connection.Close();
                return "Song already submitted for advertisement";
            }
            _reader.Close();

            _command.CommandText = "UPDATE Users SET Credits = Credits - 5 WHERE Id=@UserID";
            var userIdParam = _command.CreateParameter();
            userIdParam.ParameterName = "@UserID";
            userIdParam.Value = userId;
            _command.Parameters.Add(userIdParam);

            try
            {
                _command.ExecuteNonQuery();
                _command.CommandText = "INSERT INTO AdvertisedSong (SongID) VALUES(@SongID)";

                if (_command.ExecuteNonQuery() > 0)
                {
                    _connection.Close();
                    return "Song succesfully submitted for advertisement";
                }
                else
                {
                    _connection.Close();
                    return "Error occured";
                }
            }
            catch(Exception)
            {
                _connection.Close();
                return "Not enough credits to submit advertisement";
            }
        }
    }
}
