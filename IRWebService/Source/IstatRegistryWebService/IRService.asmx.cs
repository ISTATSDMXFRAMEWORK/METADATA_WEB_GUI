using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Configuration;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Data;

namespace IstatRegistryWebService
{
    public class UserAgency
    {
        public UserAgency()
        {
        }

        public UserAgency(string id, string lang, string text)
        {
            this.id = id;
            this.lang = lang;
            this.text = text;
        }

        public string id;
        public string lang;
        public string text;
    }

    public class User
    {
        public User()
        {
        }

        public User(string id, string username, string password, string name, string surname, List<UserAgency> agencies)
        {
            this.id = id;
            this.username = username;
            this.password = password;
            this.name = name;
            this.surname = surname;
            this.agencies = agencies;
        }

        public string id = "";
        public string username = "";
        public string password = "";
        public string name = "";
        public string surname = "";
        public List<UserAgency> agencies = new List<UserAgency>();
    }

    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class IRService : System.Web.Services.WebService
    {
        /// <summary>
        /// -----------------------
        /// GetUserByCredentials
        /// -----------------------
        /// Recupera un utente dal db 
        /// </summary>
        /// <param name="username">Lo username dell'utente</param>
        /// <param name="password">La password dell'utente</param>
        /// <returns>L'oggetto User relativo all'utente appena recuperato dal db. In caso di errore restituisce null</returns>
        [WebMethod]
        public User GetUserByCredentials(string username, string password)
        {
            User foundUser = null;
            string stringConnection = ConfigurationManager.ConnectionStrings["ISTAT_REGISTRY_DB"].ToString();
            connection.ConnectionString = stringConnection;

            // Command per recupero utente
            string userQuery = "SELECT * FROM WR_USERS WHERE username = @username AND password = @password";
            SqlCommand userCmd = new SqlCommand(userQuery, connection);
            userCmd.Parameters.AddWithValue("@username", username);
            userCmd.Parameters.AddWithValue("@password", password);

            // Command per recupero agencies associate a utente
            string agenciesQuery = @"SELECT ITEM.ID, LANGUAGE, LOCALISED_STRING.TEXT FROM agency
                                     INNER JOIN ITEM ON AG_ID = ITEM_ID
                                     INNER JOIN LOCALISED_STRING ON LOCALISED_STRING.ITEM_ID = ITEM.ITEM_ID
                                     INNER JOIN WR_USERS_AGENCY ON agency_id = AG_ID
                                     INNER JOIN WR_USERS ON wr_users_id = wr_users.id
                                     WHERE TYPE = 'name' AND wr_users.username = @username AND WR_USERS.password = @password";
            SqlCommand agenciesCmd = new SqlCommand(agenciesQuery, connection);

            try
            {
                connection.Open();
                SqlDataReader myUserReader = userCmd.ExecuteReader();
                if (myUserReader.HasRows)
                {
                    myUserReader.Read();
                    // Recupero i dati dell'utente
                    string foundUserId = myUserReader.GetValue(0).ToString();
                    string foundUsername = myUserReader.GetValue(1).ToString();
                    string foundPassword = myUserReader.GetValue(2).ToString();
                    string foundName = myUserReader.GetValue(3).ToString();
                    string foundDescription = myUserReader.GetValue(4).ToString();
                    myUserReader.Close();
                    // Recupero le agencies dell'utente
                    agenciesCmd.Parameters.AddWithValue("@username", foundUsername);
                    agenciesCmd.Parameters.AddWithValue("@password", foundPassword);
                    SqlDataReader myAgenciesReader = agenciesCmd.ExecuteReader();

                    List<UserAgency> userAgencies = new List<UserAgency>();

                    if (myAgenciesReader.HasRows)
                    {
                        UserAgency tmpAgency = null;
                        string foundAgencyId = string.Empty;
                        string foundAgencyLanguage = string.Empty;
                        string foundAgencyText = string.Empty;

                        while (myAgenciesReader.Read())
                        {
                            foundAgencyId = myAgenciesReader.GetValue(0).ToString();
                            foundAgencyLanguage = myAgenciesReader.GetValue(1).ToString();
                            foundAgencyText = myAgenciesReader.GetValue(2).ToString();
                            tmpAgency = new UserAgency(foundAgencyId, foundAgencyLanguage, foundAgencyText);
                            userAgencies.Add(tmpAgency);
                        }
                    }

                    myAgenciesReader.Close();
                    //-----------------------------------
                    foundUser = new User(foundUserId, foundUsername, foundPassword, foundName, foundDescription, userAgencies);
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
            }

            return foundUser;
        }

        /// <summary>
        /// --------------------------------
        /// CleanAgenciesRelationForUser
        /// --------------------------------
        /// Elimina le relazioni agencies/utente nel database
        /// </summary>
        /// <param name="userId">Id dell'utente di cui si vuole eliminare le relazioni</param>
        /// <returns>true in caso di successo. false altrimenti.</returns>
        [WebMethod]
        public bool CleanAgenciesRelationForUser(int userId)
        {
            string stringConnection = ConfigurationManager.ConnectionStrings["ISTAT_REGISTRY_DB"].ToString();
            connection.ConnectionString = stringConnection;

            string query = string.Empty;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;

            try
            {
                connection.Open();
                query = "DELETE FROM wr_users_agency WHERE wr_users_id = @userId";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
                return false;
            }
            return true;
        }

        /// <summary>
        /// --------------------------------
        /// CreateUserAgenciesRelation
        /// --------------------------------
        /// Crea una relazione agencies/utente nel database
        /// </summary>
        /// <param name="userId">Id dell'utente</param>
        /// <param name="agencySchemeId">Id dell'agency scheme</param>
        /// <param name="agencySchemeAgencyId">Agency dell'agency scheme</param>
        /// <param name="agencySchemeVersion">Version dell'agency scheme</param>
        /// <param name="agencies">Codice dell'agency</param>
        /// <returns>true in caso di successo. false altrimenti.</returns>
        [WebMethod]
        public bool CreateUserAgenciesRelation(int userId, string agencySchemeId, string agencySchemeAgencyId, string agencySchemeVersion, string agency)
        {
            string stringConnection = ConfigurationManager.ConnectionStrings["ISTAT_REGISTRY_DB"].ToString();
            connection.ConnectionString = stringConnection;
            string version1 = string.Empty, version2 = string.Empty, version3 = string.Empty;
            string[] versionParts = agencySchemeVersion.Split('.');
            switch (versionParts.Length)
            {
                case 1:
                    version1 = versionParts[0];
                    break;
                case 2:
                    version1 = versionParts[0];
                    version2 = versionParts[1];
                    break;
                case 3:
                    version1 = versionParts[0];
                    version2 = versionParts[1];
                    version3 = versionParts[2];
                    break;
            }

            string query = string.Empty;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;

            try
            {
                connection.Open();
                query = "SELECT AG_ID FROM [dbo].[AGENCY] INNER JOIN [AGENCY_SCHEME] ON AGENCY.AG_SCH_ID = AGENCY_SCHEME.AG_SCH_ID INNER JOIN ARTEFACT ON ARTEFACT.ART_ID = AGENCY_SCHEME.AG_SCH_ID INNER JOIN [ITEM] ON ITEM.ITEM_ID = AGENCY.AG_ID WHERE [ARTEFACT].ID = @agencySchemeId AND (version1 = @agencySchemeVersion1 OR version1 IS NULL) AND (version2 = @agencySchemeVersion2 OR version2 IS NULL ) AND (version3 = @agencySchemeVersion3 OR version3 IS NULL ) AND AGENCY = @agencySchemeAgency AND [ITEM].ID = @agencyId";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@agencySchemeId", agencySchemeId);
                cmd.Parameters.AddWithValue("@agencySchemeVersion1", version1);
                cmd.Parameters.AddWithValue("@agencySchemeVersion2", version2);
                cmd.Parameters.AddWithValue("@agencySchemeVersion3", version3);
                cmd.Parameters.AddWithValue("@agencySchemeAgency", agencySchemeAgencyId);
                cmd.Parameters.AddWithValue("@agencyId", agency);

                SqlDataReader reader = cmd.ExecuteReader();
                cmd.Parameters.Clear();
                if (reader.HasRows)
                {
                    List<string> agencyRealCodes = new List<string>();
                    while (reader.Read())
                    {
                        agencyRealCodes.Add(reader.GetValue(0).ToString());
                    }
                    reader.Close();
                    foreach (string agencyRealCode in agencyRealCodes)
                    {
                        cmd.CommandText = string.Format("INSERT INTO WR_USERS_AGENCY ( wr_users_id, agency_id ) VALUES ( {0}, {1} )", userId, agencyRealCode);
                        cmd.ExecuteNonQuery();
                    }
                }
                if (!reader.IsClosed)
                {
                    reader.Close();
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
                return false;
            }
            return true;
        }

        /*
        /// <summary>
        /// --------------------------------
        /// CreateUserAgenciesRelation
        /// --------------------------------
        /// Crea una relazione agencies/utente nel database
        /// </summary>
        /// <param name="userId">Id dell'utente</param>
        /// <param name="agencySchemeId">Id dell'agency scheme</param>
        /// <param name="agencySchemeAgencyId">Agency dell'agency scheme</param>
        /// <param name="agencySchemeVersion">Version dell'agency scheme</param>
        /// <param name="agencies">Array contenente le agencies</param>
        /// <returns>true in caso di successo. false altrimenti.</returns>
        [WebMethod]
        public bool CreateUserAgenciesRelation( int userId, string agencySchemeId, string agencySchemeAgencyId, string agencySchemeVersion, string[] agencies )
        {
            string stringConnection = ConfigurationManager.ConnectionStrings["ISTAT_REGISTRY_DB"].ToString();
            connection.ConnectionString = stringConnection;
            string version1 = string.Empty, version2 = string.Empty, version3 = string.Empty;
            string[] versionParts = agencySchemeVersion.Split( '.' );
            switch ( versionParts.Length )
            {
                case 1:
                    version1 = versionParts[0];
                    break;
                case 2:
                    version1 = versionParts[0];
                    version2 = versionParts[1];
                    break;
                case 3:
                    version1 = versionParts[0];
                    version2 = versionParts[1];
                    version3 = versionParts[2];
                    break;
            }
           
            for ( int i = 0; i < agencies.Length; i++ )
            {
                agencies[i] = string.Format( "'{0}'", agencies[i] ); 
            }            
            
            string query = string.Empty;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;

            try
            {
                connection.Open();    
                query = "DELETE FROM wr_users_agency WHERE wr_users_id = @userId";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue( "@userId", userId );
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                query = "SELECT AG_ID FROM [dbo].[AGENCY] INNER JOIN [AGENCY_SCHEME] ON AGENCY.AG_SCH_ID = AGENCY_SCHEME.AG_SCH_ID INNER JOIN ARTEFACT ON ARTEFACT.ART_ID = AGENCY_SCHEME.AG_SCH_ID INNER JOIN [ITEM] ON ITEM.ITEM_ID = AGENCY.AG_ID WHERE [ARTEFACT].ID = @agencySchemeId AND (version1 = @agencySchemeVersion1 OR version1 IS NULL) AND (version2 = @agencySchemeVersion2 OR version2 IS NULL ) AND (version3 = @agencySchemeVersion3 OR version3 IS NULL ) AND AGENCY = @agencySchemeAgency AND [ITEM].ID in ( " + string.Join( ", ", agencies) +  " )";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue( "@agencySchemeId", agencySchemeId );
                cmd.Parameters.AddWithValue( "@agencySchemeVersion1", version1 );
                cmd.Parameters.AddWithValue( "@agencySchemeVersion2", version2 );
                cmd.Parameters.AddWithValue( "@agencySchemeVersion3", version3 );
                cmd.Parameters.AddWithValue( "@agencySchemeAgency", agencySchemeAgencyId );
                SqlDataReader reader = cmd.ExecuteReader();
                cmd.Parameters.Clear();
                if ( reader.HasRows )
                {
                    List<string> agencyRealCodes = new List<string>();
                    while ( reader.Read() )
                    {
                        agencyRealCodes.Add( reader.GetValue( 0 ).ToString() );                        
                    }
                    reader.Close();
                    foreach ( string agencyRealCode in agencyRealCodes )
                    {
                        cmd.CommandText = string.Format( "INSERT INTO WR_USERS_AGENCY ( wr_users_id, agency_id ) VALUES ( {0}, {1} )", userId, agencyRealCode );
                        cmd.ExecuteNonQuery();
                    }
                }
                if ( !reader.IsClosed )
                {
                    reader.Close();        
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                if ( connection.State == ConnectionState.Open )
                {
                    connection.Close();
                }
#if DEBUG
                Debug.WriteLine( ex.Message );
#endif
                return false;
            }        
            return true;
        }
        */
        /// <summary>
        /// -----------------------
        /// DeleteUser
        /// -----------------------
        /// Elimina un utente dal database
        /// </summary>
        /// <param name="userId">Id dell'utente da eliminare</param>
        /// <returns>true in caso di successo. false altrimenti.</returns>
        [WebMethod]
        public bool DeleteUser(int userId)
        {
            string stringConnection = ConfigurationManager.ConnectionStrings["ISTAT_REGISTRY_DB"].ToString();
            connection.ConnectionString = stringConnection;

            // Command per cancellazione utente
            string userQuery = "DELETE FROM WR_USERS WHERE id = @userId";
            SqlCommand userCmd = new SqlCommand(userQuery, connection);
            userCmd.Parameters.AddWithValue("@userId", userId);

            // Command per la cancellazione delle associazioni utente/agency
            string userAgencyQuery = "DELETE FROM WR_USERS_AGENCY WHERE wr_users_id = @userId";
            SqlCommand userAgencyCmd = new SqlCommand(userAgencyQuery, connection);
            userAgencyCmd.Parameters.AddWithValue("@userId", userId);

            try
            {
                connection.Open();
                userAgencyCmd.ExecuteNonQuery();
                userCmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
                return false;
            }

            return true;
        }

        /// <summary>
        /// -----------------------
        /// InsertUser
        /// -----------------------
        /// Aggiunge un utente nel database
        /// </summary>
        /// <param name="username">Username dell'utente da aggiungere</param>
        /// <param name="password">Password dell'utente da aggiungere</param>
        /// <param name="name">Nome dell'utente da aggiungere</param>
        /// <param name="surname">Cognome dell'utente da aggiungere</param>
        /// <returns>true in caso di successo. false altrimenti.</returns>
        [WebMethod]
        public bool InsertUser(string username, string password, string name, string surname, bool admin)
        {
            string stringConnection = ConfigurationManager.ConnectionStrings["ISTAT_REGISTRY_DB"].ToString();
            connection.ConnectionString = stringConnection;

            // Command per inserimento utente
            string userQuery = "INSERT INTO WR_USERS ( username, password, name, surname, admin ) VALUES ( @username, @password, @name, @surname, @admin )";
            SqlCommand userCmd = new SqlCommand(userQuery, connection);
            userCmd.Parameters.AddWithValue("@username", username);
            userCmd.Parameters.AddWithValue("@password", password);
            userCmd.Parameters.AddWithValue("@name", name);
            userCmd.Parameters.AddWithValue("@surname", surname);
            userCmd.Parameters.AddWithValue("@admin", admin);

            try
            {
                connection.Open();
                userCmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
                return false;
            }

            return true;
        }

        /// <summary>
        /// -----------------------
        /// GetCodesIdByUser
        /// -----------------------
        /// Recupera l'elenco degli id delle agency associate a l'utente
        /// </summary>
        /// <param name="username">Username dell'utente di cui cercare le agencies associate</param>
        /// <returns>Array contenente i codes della agencies associate all'utente</returns>
        [WebMethod]
        public string[] GetCodesIdByUser(int userId)
        {
            string stringConnection = ConfigurationManager.ConnectionStrings["ISTAT_REGISTRY_DB"].ToString();
            connection.ConnectionString = stringConnection;

            // Command per recupero codes
            string query = "SELECT ITEM.ID FROM ITEM INNER JOIN WR_USERS_AGENCY ON AGENCY_ID = ITEM.ITEM_ID INNER JOIN WR_USERS ON WR_USERS.ID = WR_USERS_AGENCY.wr_users_id WHERE WR_USERS.id = @userId;";
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@userId", userId);
            List<string> codes = new List<string>();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            try
            {
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        codes.Add(reader.GetValue(0).ToString());
                    }
                }
                if (!reader.IsClosed)
                {
                    reader.Close();
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
            }
            return codes.ToArray<string>();
        }

        /// <summary>
        /// -----------------------
        /// UpdateUser
        /// -----------------------
        /// Aggiorna un utente nel database
        /// </summary>
        /// <param name="id">Id dell'utente da aggiornare</param>
        /// <param name="username">Username dell'utente da aggiornare</param>
        /// <param name="password">Password dell'utente da aggiornare</param>
        /// <param name="name">Nome dell'utente da aggiornare</param>
        /// <param name="surname">Cognome dell'utente da aggiornare</param>
        /// <returns>true in caso di successo. false altrimenti.</returns>
        [WebMethod]
        public bool UpdateUser(int id, string username, string password, string name, string surname, bool admin)
        {
            string stringConnection = ConfigurationManager.ConnectionStrings["ISTAT_REGISTRY_DB"].ToString();
            connection.ConnectionString = stringConnection;

            // Command per update utente
            string userQuery = string.Empty;
            if (password.Trim().Equals(string.Empty))
            {
                userQuery = "UPDATE WR_USERS SET username = @username, name = @name, surname = @surname, admin= @admin  WHERE id = @id";
            }
            else
            {
                userQuery = "UPDATE WR_USERS SET username = @username, password = @password, name = @name, surname = @surname, admin= @admin WHERE id = @id";
            }
            SqlCommand userCmd = new SqlCommand(userQuery, connection);
            userCmd.Parameters.AddWithValue("@id", id);
            userCmd.Parameters.AddWithValue("@username", username);
            if (!password.Trim().Equals(string.Empty))
            {
                userCmd.Parameters.AddWithValue("@password", password);
            }
            userCmd.Parameters.AddWithValue("@name", name);
            userCmd.Parameters.AddWithValue("@surname", surname);
            userCmd.Parameters.AddWithValue("@admin", admin);

            try
            {
                connection.Open();
                userCmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
                return false;
            }
            return true;
        }

        /// <summary>
        /// -----------------------
        /// GetUserById
        /// -----------------------
        /// Recupera gli utenti dal db 
        /// </summary>
        /// <param name="userId">Id dell'utente da recuperare</param>
        /// <returns>Datatable contenente le info dell'utente recuperato</returns>
        [WebMethod]
        public DataTable GetUserById(int userId)
        {
            string stringConnection = ConfigurationManager.ConnectionStrings["ISTAT_REGISTRY_DB"].ToString();
            connection.ConnectionString = stringConnection;

            // Command per recupero utente
            string userQuery = "SELECT * FROM WR_USERS WHERE id = @userId";
            SqlCommand userCmd = new SqlCommand(userQuery, connection);
            userCmd.Parameters.AddWithValue("@userId", userId);
            DataTable table = new DataTable("users");
            SqlDataAdapter adapter = new SqlDataAdapter(userCmd);
            try
            {
                connection.Open();
                adapter.Fill(table);
                connection.Close();
            }
            catch (Exception ex)
            {
                table = null;
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
            }
            return table;
        }

        /// <summary>
        /// -----------------------
        /// GetUserById
        /// -----------------------
        /// Recupera gli utenti dal db
        /// </summary>
        /// <returns>L'array di User contenuti nel db. In caso di errore restituisce null</returns>
        [WebMethod]
        public DataTable GetAllUsers()
        {
            string stringConnection = ConfigurationManager.ConnectionStrings["ISTAT_REGISTRY_DB"].ToString();
            connection.ConnectionString = stringConnection;

            // Command per recupero utenti
            string userQuery = "SELECT * FROM WR_USERS";
            SqlCommand userCmd = new SqlCommand(userQuery, connection);
            DataTable table = new DataTable("users");
            SqlDataAdapter adapter = new SqlDataAdapter(userCmd);
            try
            {
                connection.Open();
                adapter.Fill(table);
                connection.Close();
            }
            catch (Exception ex)
            {
                table = null;
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
            }

            return table;
        }

        /// <summary>
        /// -----------------------
        /// CheckIfUserExists
        /// -----------------------
        /// Verifica l'esistenza di un utente nel db
        /// </summary>
        /// <param name="username">Username dell'utente da verificare</param>
        /// <returns>true in caso di esistenza dell'utente. false altrimenti</returns>
        [WebMethod]
        public bool CheckIfUserExists(string username)
        {
            int users = 0;
            string stringConnection = ConfigurationManager.ConnectionStrings["ISTAT_REGISTRY_DB"].ToString();
            connection.ConnectionString = stringConnection;

            // Command per recupero utenti
            string userQuery = "SELECT COUNT( * ) FROM WR_USERS WHERE username = @username";
            SqlCommand userCmd = new SqlCommand(userQuery, connection);
            userCmd.Parameters.AddWithValue("@username", username);
            try
            {
                connection.Open();
                users = Convert.ToInt32(userCmd.ExecuteScalar());
                connection.Close();
            }
            catch (Exception ex)
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
            }
            if (users > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// -----------------------
        /// GetCodelistId
        /// -----------------------
        /// Recupera l'id di una codelist dal db
        /// </summary>
        /// <param name="id">id della codelist da recuperare ( livello applicativo )</param>
        /// <param name="agency">agency della codelist da recuperare</param>
        /// <param name="version1IsPresent">indica se la version 1 è presente</param>
        /// <param name="version1">Il valore della version 1</param>
        /// <param name="version2IsPresent">indica se la version 2 è presente</param>
        /// <param name="version2">Il valore della version 2</param>
        /// <param name="version3IsPresent">indica se la version 3 è presente</param>
        /// <param name="version3">Il valore della version 3</param>
        /// <param name="foundId">Il valore della codelist recuperata</param>
        /// <returns>true in caso di successo. false altrimenti</returns>
        [WebMethod]
        public bool GetCodelistId(string id, string agency, bool version1IsPresent, int version1, bool version2IsPresent, int version2, bool version3IsPresent, int version3, ref int foundId)
        {
            string stringConnection = ConfigurationManager.ConnectionStrings["ISTAT_REGISTRY_DB"].ToString();
            connection.ConnectionString = stringConnection;

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "SP_GET_CODELIST_ID";
                cmd.Parameters.Add("@pId", SqlDbType.VarChar).Value = id;
                cmd.Parameters.Add("@pAgency", SqlDbType.VarChar).Value = agency;
                if (version1IsPresent)
                {
                    cmd.Parameters.Add("@pVersion1", SqlDbType.Int).Value = version1;
                }
                else
                {
                    cmd.Parameters.Add("@pVersion1", SqlDbType.Int).Value = DBNull.Value;
                }
                if (version2IsPresent)
                {
                    cmd.Parameters.Add("@pVersion2", SqlDbType.Int).Value = version2;
                }
                else
                {
                    cmd.Parameters.Add("@pVersion2", SqlDbType.Int).Value = DBNull.Value;
                }
                if (version3IsPresent)
                {
                    cmd.Parameters.Add("@pVersion3", SqlDbType.Int).Value = version3;
                }
                else
                {
                    cmd.Parameters.Add("@pVersion3", SqlDbType.Int).Value = DBNull.Value;
                }
                SqlParameter foundIdParam = new SqlParameter("@foundId", SqlDbType.Int);
                foundIdParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(foundIdParam);

                try
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception error)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
#if DEBUG
                    Debug.WriteLine(error.Message);
#endif
                    return false;
                }

                foundId = Convert.ToInt32(cmd.Parameters["@foundId"].Value);
                return true;
            }
        }

        /// <summary>
        /// -----------------------
        /// InsertDsdCode
        /// -----------------------
        /// Inserisce un code nel db
        /// </summary>
        /// <param name="id">l'id del code da inserire ( livello applicativo )</param>
        /// <param name="codelistId">L'id della codelist del code</param>
        /// <param name="parentCode">L'id del parent code del code da inserire, recuperato dal db</param>
        /// <param name="insertedId">L'id nel db del code appena inserito</param>
        /// <returns>true in caso di successo. false altrimenti</returns>
        [WebMethod]
        public bool InsertDsdCode(string id, string codelistId, int parentCode, ref int insertedId)
        {
            string stringConnection = ConfigurationManager.ConnectionStrings["ISTAT_REGISTRY_DB"].ToString();
            connection.ConnectionString = stringConnection;

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "INSERT_DSD_CODE";
                cmd.Parameters.Add("@p_id", SqlDbType.VarChar).Value = id;
                cmd.Parameters.Add("@p_cl_id", SqlDbType.BigInt).Value = codelistId;
                if (parentCode == 0)
                {
                    cmd.Parameters.Add("@p_parent_code_id", SqlDbType.BigInt).Value = DBNull.Value;
                }
                else
                {
                    cmd.Parameters.Add("@p_parent_code_id", SqlDbType.BigInt).Value = parentCode;
                }

                SqlParameter insertedIdParam = new SqlParameter("@p_pk", SqlDbType.BigInt);
                insertedIdParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(insertedIdParam);

                try
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception error)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
#if DEBUG
                    Debug.WriteLine(error.Message);
#endif
                    return false;
                }

                insertedId = Convert.ToInt32(cmd.Parameters["@p_pk"].Value);
                return true;
            }
        }

        /// <summary>
        /// -----------------------
        /// GetDsdCodeId
        /// -----------------------
        /// Recupera l'id di un code all'interno del db
        /// </summary>
        /// <param name="codelistId">l'id della codelist a cui è applicato il code</param>
        /// <param name="codeId">l'id del code ( livello applicativo )</param>
        /// <param name="foundId">l'id del code recuperato ( db )</param>
        /// <returns>true in caso di successo. false altrimenti</returns>
        [WebMethod]
        public bool GetDsdCodeId(int codelistId, string codeId, ref int foundId)
        {
            string stringConnection = ConfigurationManager.ConnectionStrings["ISTAT_REGISTRY_DB"].ToString();
            connection.ConnectionString = stringConnection;

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "GET_DSD_CODE_ID";
                cmd.Parameters.Add("@p_cl_id", SqlDbType.BigInt).Value = codelistId;
                cmd.Parameters.Add("@p_code_id", SqlDbType.VarChar).Value = codeId;

                SqlParameter foundIdParam = new SqlParameter("@p_found_id", SqlDbType.BigInt);
                foundIdParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(foundIdParam);

                try
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception error)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
#if DEBUG
                    Debug.WriteLine(error.Message);
#endif
                    return false;
                }

                if (cmd.Parameters["@p_found_id"].Value != DBNull.Value && Convert.ToInt32(cmd.Parameters["@p_found_id"].Value) != 0)
                {
                    foundId = Convert.ToInt32(cmd.Parameters["@p_found_id"].Value);
                }
                else
                {
                    foundId = 0;
                }
                return true;
            }
        }

        /// <summary>
        /// -----------------------
        /// InsertLocalizedString
        /// -----------------------
        /// Inserisce l'una stringa localizzata per un code
        /// </summary>
        /// <param name="itemId">id del code a cui applicare la stringa</param>
        /// <param name="text">la stringa da applicare</param>
        /// <param name="type">il type della stringa ( 'Name' / 'Desc' )</param>
        /// <param name="language">la lingua della stringa</param>
        /// <param name="foundId">l'id della stringa localizzata appena inserita nel db</param>
        /// <returns>true in caso di successo. false altrimenti</returns>
        [WebMethod]
        public bool InsertLocalizedString(int itemId, string text, string type, string language, ref int foundId)
        {
            string stringConnection = ConfigurationManager.ConnectionStrings["ISTAT_REGISTRY_DB"].ToString();
            connection.ConnectionString = stringConnection;

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "INSERT_LOCALISED_STRING";
                cmd.Parameters.Add("@p_item_id", SqlDbType.BigInt).Value = itemId;
                cmd.Parameters.Add("@p_art_id", SqlDbType.BigInt).Value = DBNull.Value;
                cmd.Parameters.Add("@p_text", SqlDbType.NVarChar).Value = text;
                cmd.Parameters.Add("@p_type", SqlDbType.VarChar).Value = type;
                cmd.Parameters.Add("@p_language", SqlDbType.VarChar).Value = language;
                SqlParameter foundIdParam = new SqlParameter("@p_pk", SqlDbType.BigInt);
                foundIdParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(foundIdParam);

                try
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception error)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
#if DEBUG
                    Debug.WriteLine(error.Message);
#endif
                    return false;
                }

                foundId = Convert.ToInt32(cmd.Parameters["@p_pk"].Value);
                return true;
            }
        }

        /// <summary>
        /// -----------------------
        /// InsertAnnotation
        /// -----------------------
        /// Inserisce l'annotazione nel db
        /// </summary>
        /// <param name="itemId">id nel database del code a cui và applicata l'annotazione</param>
        /// <param name="insertedAnnotationId">L'id dell'annotazione appena creata nel db</param>
        /// <returns>true in caso di successo. false altrimenti</returns>
        private bool InsertAnnotation(int itemId, ref int insertedAnnotationId)
        {
            string stringConnection = ConfigurationManager.ConnectionStrings["ISTAT_REGISTRY_DB"].ToString();
            connection.ConnectionString = stringConnection;

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "INSERT_ITEM_ANNOTATION";
                cmd.Parameters.Add("@p_item_id", SqlDbType.BigInt).Value = itemId;
                cmd.Parameters.Add("@p_id", SqlDbType.NVarChar).Value = DBNull.Value;
                cmd.Parameters.Add("@p_title", SqlDbType.NVarChar).Value = DBNull.Value;
                cmd.Parameters.Add("@p_type", SqlDbType.VarChar).Value = "@ORDER@";
                cmd.Parameters.Add("@p_url", SqlDbType.NVarChar).Value = DBNull.Value;
                SqlParameter foundIdParam = new SqlParameter("@p_pk", SqlDbType.BigInt);
                foundIdParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(foundIdParam);

                try
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception error)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
#if DEBUG
                    Debug.WriteLine(error.Message);
#endif
                    return false;
                }

                insertedAnnotationId = Convert.ToInt32(cmd.Parameters["@p_pk"].Value);
                return true;
            }
        }

        /// <summary>
        /// -----------------------
        /// InsertAnnotationText
        /// -----------------------
        /// Inserisce il text relativo all'annotazione
        /// </summary>
        /// <param name="itemId">id nel database dell'annotazione a cui va applicato il text</param>
        /// <param name="order">l'ordine del code da creare</param>
        /// <param name="insertedAnnotationTextId">l'id del text dell'annotazione appena creato nel db</param>
        /// <returns>true in caso di successo. false altrimenti</returns>        
        private bool InsertAnnotationText(int annotationId, int order, ref int insertedAnnotationTextId)
        {
            string stringConnection = ConfigurationManager.ConnectionStrings["ISTAT_REGISTRY_DB"].ToString();
            connection.ConnectionString = stringConnection;

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "INSERT_ANNOTATION_TEXT";
                cmd.Parameters.Add("@p_ann_id", SqlDbType.BigInt).Value = annotationId;
                cmd.Parameters.Add("@p_language", SqlDbType.NVarChar).Value = "en";
                cmd.Parameters.Add("@p_text", SqlDbType.VarChar).Value = order.ToString();
                SqlParameter foundIdParam = new SqlParameter("@p_pk", SqlDbType.BigInt);
                foundIdParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(foundIdParam);

                try
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception error)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
#if DEBUG
                    Debug.WriteLine(error.Message);
#endif
                    return false;
                }

                insertedAnnotationTextId = Convert.ToInt32(cmd.Parameters["@p_pk"].Value);
                return true;
            }
        }

        /// <summary>
        /// -----------------------
        /// InsertOrderAnnotation
        /// -----------------------
        /// Inserisce l'annotazione per l'ordine relativo al codice
        /// richiamando i metodi di creazione dell'annotazione e del
        /// relativo text
        /// </summary>
        /// <param name="itemId">id nel database del code a cui và applicato l'ordine</param>
        /// <param name="order">l'ordine del code da creare</param>
        /// <returns>true in caso di successo. false altrimenti</returns>
        [WebMethod]
        public bool InsertOrderAnnotation(int itemId, int order)
        {
            int insertedAnnotationId = 0, insertedAnnotationTextId = 0;
            if (!InsertAnnotation(itemId, ref insertedAnnotationId))
            {
                return false;
            }
            if (!InsertAnnotationText(insertedAnnotationId, order, ref insertedAnnotationTextId))
            {
                return false;
            }
            return true;
        }

        private SqlConnection connection = new SqlConnection();
    }
}
