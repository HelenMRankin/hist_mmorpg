using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using Lidgren.Network;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics.Contracts;

namespace hist_mmorpg
{
    // NOTE: Verification is expensive! We may not want to verify every class. To turn on verification, compile with V_LOGIN
    /// <summary>
    /// This class handles all log in tasks, including certificate transmission and password verification.
    /// It may be possible to turn this class into a stand-alone application in order to create a dedicated log-in server.
    /// </summary>
    [ContractVerification(true)]
    public static class LogInManager
    {
        /// <summary>
        /// Used in generating random salts for use in hashing
        /// </summary>
        static RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
        /// <summary>
        /// Hashing algorithm, used for hashing passwords
        /// </summary>
        static HashAlgorithm hashAlgorithm = new SHA256Managed();
        /// <summary>
        /// Dictionary mapping usernames to session salts, used to ensure each user gets their own salt once connected
        /// </summary>
        private static Dictionary<string, byte[]> sessionSalts = new Dictionary<string, byte[]>();
        /// <summary>
        /// Dictionary mapping player username to password hash and salt- for use during testing, should use database for final. First byte array is hash, second is salt
        /// </summary>
        public static Dictionary<string, Tuple<byte[], byte[]>> users = new Dictionary<string, Tuple<byte[], byte[]>>();
        /// <summary>
        /// The server's own X509 certificate, which clients can verify
        /// </summary>
        public static X509Certificate2 ServerCert
        {
            get; set;
        }
        /// <summary>
        /// Performs RSA en/decryption
        /// </summary>
        private static RSACryptoServiceProvider rsa;

        /// <summary>
        /// Gets a random salt for use in hashing
        /// </summary>
        /// <param name="bytes">size of resulting salt</param>
        /// <returns>salt</returns>
        public static byte[] GetRandomSalt(int bytes)
        {
            Contract.Requires(bytes>0);
            Contract.Ensures(Contract.Result<byte[]>() != null);
            byte[] salt = new byte[bytes];
            crypto.GetBytes(salt);
            return salt;
        }

        /// <summary>
        /// Computes the hash of a salt appended to source byte array
        /// </summary>
        /// <param name="toHash">bytes to be hashed</param>
        /// <param name="salt">salt</param>
        /// <returns>computed hash</returns>
        public static byte[] ComputeHash(byte[] toHash, byte[] salt)
        {
            Contract.Requires(toHash!=null&salt!=null);
            Contract.Ensures(Contract.Result<byte[]>()!=null);
            byte[] fullHash = new byte[toHash.Length + salt.Length];
            toHash.CopyTo(fullHash, 0);
            salt.CopyTo(fullHash, toHash.Length);
            byte[] hashcode = hashAlgorithm.ComputeHash(fullHash);
            return hashcode;
        }

        /// <summary>
        /// Store a new user in the database
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="pass">Password. Note this isn't stored, only the hash and salt are</param>
        public static void StoreNewUser(string username, string pass)
        {
            Contract.Requires(username!=null&&pass!=null);
            Contract.Ensures(users.ContainsKey(username));
            byte[] passBytes = Encoding.UTF8.GetBytes(pass);
            byte[] salt = GetRandomSalt(32);
            byte[] computedHash = ComputeHash(passBytes, salt);
            users.Add(username, new Tuple<byte[], byte[]>(computedHash, salt));
        }

        /// <summary>
        /// Retrieve password hash from database
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>password hash</returns>
        public static byte[] GetPasswordHash(string username)
        {
            Contract.Requires(username !=null);
            Contract.Requires(Contract.Exists(users, a => a.Key == username));
            Tuple<byte[], byte[]> hashNsalt;
            if (users.TryGetValue(username, out hashNsalt))
            {
                return hashNsalt.Item1;
            }
            else
            {
                throw new InvalidLogInException("The username "+username+ " does not exist in the list of recognised users");
            }
        }

        /// <summary>
        /// Retrieve salt used when hashing password from database
        /// </summary>
        /// <param name="username">username</param>
        /// <returns>salt</returns>
        public static byte[] GetUserSalt(string username)
        {
            Contract.Requires(username!=null);
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Contract.Exists(users, a=>a.Key == username);
            Tuple<byte[], byte[]> hashNsalt;
            if (users.TryGetValue(username, out hashNsalt))
            {
                return hashNsalt.Item2;
            }
            else
            {
                throw new InvalidLogInException("The username " + username + " does not exist in the list of recognised users");
            }
        }

        /// <summary>
        /// Verify the identity of a user by computing and comparing password hashes
        /// </summary>
        /// <param name="username">username</param>
        /// <param name="userhash">hash generated by client</param>
        /// <returns></returns>
        public static bool VerifyUser(string username, byte[] userhash)
        {
            Contract.Requires(username!=null);
            if (userhash == null)
            {
                return false;
            }
            byte[] sessionSalt;
            if (!sessionSalts.TryGetValue(username, out sessionSalt))
            {
                return false;
            }
            byte[] passwordHash = ComputeHash(GetPasswordHash(username), sessionSalt);
            return userhash.SequenceEqual(passwordHash);

        }

        /// <summary>
        /// Determines whether or not to accept the connection based on whether a user's username is recognised, and constructs a ProtoLogIn containing session salt
        /// </summary>
        /// <param name="client">Client who is connecting</param>
        /// <param name="text">Challenge text from which to create a signature</param>
        /// <param name="response">Response message</param>
        /// <returns>Boolean indicating whether connection was accepted</returns>
        public static bool AcceptConnection(Client client, string text, out ProtoLogIn response)
        {
            Contract.Requires(client!=null);
            byte[] sessionSalt = GetRandomSalt(32);
            byte[] userSalt = GetUserSalt(client.username);
            if (userSalt == null)
            {
                response = null;
                return false;
            }
            response = new ProtoLogIn {sessionSalt = sessionSalt};
            if (!sessionSalts.ContainsKey(client.username))
            {
                sessionSalts.Add(client.username, sessionSalt);
            }
            else
            {
                sessionSalts[client.username] = sessionSalt;
            }
            response.userSalt = userSalt;
            response.ActionType = Actions.LogIn;
            if (ServerCert != null)
            {
                response.certificate = ServerCert.GetRawCertData();
            }
            response.Signature = Sign(text);
            return true;
        }

        /// <summary>
        /// Sign a certificate
        /// Note that now we are allowing certificates to be unsigned for purpose of testing
        /// </summary>
        /// <author> Alejandro Campos Magencio 2008</author>
        /// <param name="text">String to sign</param>
        /// <returns></returns>
        private static byte[] Sign(string text)

        {
            if (string.IsNullOrWhiteSpace(text)||text.Length==0)
            {
                return null;
            }
            UnicodeEncoding encoding = new UnicodeEncoding();

            byte[] data = encoding.GetBytes(text);

            byte[] hash = hashAlgorithm.ComputeHash(data);

            return rsa.SignHash(hash, CryptoConfig.MapNameToOID("SHA256"));
        }

        /// <summary>
        /// Initialise the server certificate, and initialise the RSACryptoServiceProvider to use the server's public and private keys
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool InitialiseCertificateAndRSA(string path)
        {
            Contract.Requires(path!=null);
            try
            {
                path = Path.Combine(path, "ServerCert.pfx");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
            ServerCert =
                    new X509Certificate2(path, "zip1020", X509KeyStorageFlags.Exportable);
            X509Chain chain = new X509Chain
            {
                ChainPolicy =
                {
                    RevocationFlag = X509RevocationFlag.EndCertificateOnly,
                    RevocationMode = X509RevocationMode.NoCheck
                }
            };
            // This song-and-dance is to get SHA256 working for certificate signing
            var rsa2 = ServerCert.PrivateKey as RSACryptoServiceProvider;
            // Create a new RSACryptoServiceProvider
            rsa = new RSACryptoServiceProvider();
            // Export RSA parameters from 'rsa' and import them into 'rsaClear'
            rsa.ImportParameters(rsa2.ExportParameters(true));
            return true;
        }

        /// <summary>
        /// Take a client's log in details, verify them and then choose to either allow the user to log in, or disconnect
        /// </summary>
        /// <param name="login">Log in details</param>
        /// <param name="c">Client who is logging in</param>
        /// <returns>Boolean indicating whether log in was successful</returns>
        public static bool ProcessLogIn(ProtoLogIn login, Client c)
        {
            Contract.Requires(c!=null&&login!=null);
            if (!VerifyUser(c.username, login.userSalt))
            {
                return false;
            }
            try
            {
                if (login.Key != null)
                {
                    byte[] key = rsa.Decrypt(login.Key, false);
                    // Key must be non-null and long enough

                    if (key == null || key.Length < 5)
                    {
                        return false;
                    }
                    c.alg = new NetAESEncryption(Globals_Server.server, key, 0, key.Length);
                }
                else
                {
#if ALLOW_UNENCRYPT
                    c.alg = null;
#else
                    return false;
#endif
                }
                    ProtoClient clientDetails = new ProtoClient(c);
                clientDetails.ActionType = Actions.LogIn;
                clientDetails.ResponseType = DisplayMessages.LogInSuccess;
                Server.SendViaProto(clientDetails, c.conn, c.alg);
                Globals_Game.RegisterObserver(c);
                return true;
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine("Failure during decryption: " + e.GetType() + " " + e.Message + ";" + e.StackTrace);
#endif
                return false;
            }
        }

        class InvalidLogInException : Exception
        {

            InvalidLogInException() : base()
            {
            }

            public InvalidLogInException(string p) : base (p)
            {
            }
        }
    }
}
