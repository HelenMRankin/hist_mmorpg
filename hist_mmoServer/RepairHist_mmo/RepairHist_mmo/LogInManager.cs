﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.InteropServices;
using Lidgren.Network;
using System.Security.Cryptography.X509Certificates;

namespace hist_mmorpg
{
    public static class LogInManager
    {
        static RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
        static HashAlgorithm hash = new SHA256Managed();
        private static Dictionary<string, byte[]> sessionSalts = new Dictionary<string, byte[]>();
        /// <summary>
        /// Dictionary mapping player username to password hash and salt- for use during testing, should use database for final. First byte array is hash, second is salt
        /// </summary>
        public static Dictionary<string, Tuple<byte[], byte[]>> users = new Dictionary<string, Tuple<byte[], byte[]>>();
        public static X509Certificate2 ServerCert
        {
            get; set;
        }
        private static RSACryptoServiceProvider rsa;

        /// <summary>
        /// Gets a random salt for use in hashing
        /// </summary>
        /// <param name="bytes">size of resulting salt</param>
        /// <returns>salt</returns>
        public static byte[] GetRandomSalt(int bytes)
        {
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
            byte[] fullHash = new byte[toHash.Length + salt.Length];
            toHash.CopyTo(fullHash, 0);
            salt.CopyTo(fullHash, toHash.Length);
            byte[] hashcode = hash.ComputeHash(fullHash);
            return hashcode;
        }

        /// <summary>
        /// Store a new user in the database
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="pass">Password. Note this isn't stored, only the hash and salt are</param>
        public static void StoreNewUser(string username, string pass)
        {
            byte[] passBytes = Encoding.UTF8.GetBytes(pass);
            byte[] salt = GetRandomSalt(32);
            byte[] hash = ComputeHash(passBytes, salt);
            users.Add(username, new Tuple<byte[], byte[]>(hash, salt));
        }

        /// <summary>
        /// Retrieve password hash from database
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>password hash</returns>
        public static byte[] GetPasswordHash(string username)
        {
            Tuple<byte[], byte[]> hashNsalt;
            if (users.TryGetValue(username, out hashNsalt))
            {
                return hashNsalt.Item1;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Retrieve salt used when hashing password from database
        /// </summary>
        /// <param name="username">username</param>
        /// <returns>salt</returns>
        public static byte[] GetUserSalt(string username)
        {
            Tuple<byte[], byte[]> hashNsalt;
            if (users.TryGetValue(username, out hashNsalt))
            {
                return hashNsalt.Item2;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Verify the identity of a user by computing and comparing password hashes
        /// </summary>
        /// <param name="username">username</param>
        /// <param name="userhash">hash generated by client</param>
        /// <param name="sessionSalt">this session's salt</param>
        /// <returns></returns>
        public static bool VerifyUser(string username, byte[] userhash)
        {
            byte[] sessionSalt;
            if (!sessionSalts.TryGetValue(username, out sessionSalt))
            {
                return false;
            }
            byte[] passwordHash = ComputeHash(GetPasswordHash(username), sessionSalt);
            if(userhash!=null && passwordHash!= null)
            {
                return userhash.SequenceEqual(passwordHash);
            }
            return false;
        }

        /// <summary>
        /// Determines whether or not to accept the connection based on whether a user's username is recognised, and constructs a ProtoLogIn containing session salt
        /// </summary>
        /// <param name="client"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public static bool AcceptConnection(Client client, out ProtoLogIn response)
        {
            byte[] sessionSalt = GetRandomSalt(32);
            byte[] userSalt = GetUserSalt(client.username);
            if (userSalt == null)
            {
                response = null;
                return false;
            }
            response = new ProtoLogIn();
            response.sessionSalt = sessionSalt;
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
            return true;
        }

        /// <summary>
        /// test function
        /// </summary>
        /// <param name="user">username</param>
        /// <param name="pass">password</param>
        public static void TestVerify(string user, string pass)
        {
            StoreNewUser(user, pass);
            byte[] sessionSalt = GetRandomSalt(32);
            byte[] passSalt = GetUserSalt(user);
            if (passSalt == null)
            {
                Console.WriteLine("error getting random hash");
                return;
            }
            byte[] passbytes = Encoding.UTF8.GetBytes(pass);
            byte[] hashPassword = ComputeHash(passbytes, passSalt);

            byte[] clientHash = ComputeHash(hashPassword, sessionSalt);
            if (!VerifyUser(user, clientHash))
            {
                Console.WriteLine("Not verified");
            }
            else
            {
                Console.WriteLine("Verified");
            }
        }

        public static bool InitialiseCertificateAndRSA(string path)
        {
            try
            {
                Console.WriteLine(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                path = Path.Combine(path, "ServerCert.pfx");
                Console.WriteLine("Certificate path: " + path);


            }
            catch (Exception e)
            {
                Console.WriteLine("Type: " + e.GetType().FullName + " message: " + e.Message);
                return false;
            }
            ServerCert =
                    new X509Certificate2(path, "zip1020");
            Console.WriteLine("Verify? " + ServerCert.Verify());
            X509Chain chain = new X509Chain();
            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EndCertificateOnly;
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            Console.WriteLine("Chain build? " + chain.Build(ServerCert));
            foreach (X509ChainStatus stat in chain.ChainStatus)
            {
                Console.WriteLine("Status: " + stat.Status + " info: " + stat.StatusInformation);
            }
            // Set up asymmetric decryption algorithm
            Console.WriteLine("Private key?: " + ServerCert.HasPrivateKey);

            rsa = (RSACryptoServiceProvider)ServerCert.PrivateKey;
            Console.WriteLine("public key: ");
            foreach (var bite in rsa.ExportParameters(false).Exponent)
            {
                Console.Write(bite.ToString());
            }
            return true;
        }


        public static bool ProcessLogIn(ProtoLogIn login, Client c)
        {
            if (!VerifyUser(c.username, login.userSalt))
            {
                // error
                return false;
            }
            try
            {
                byte[] key = rsa.Decrypt(login.Key, false);
                // Key must be non-null and long enough
                if (key == null || key.Length < 5)
                {
                    return false;
                }
                Console.WriteLine("SERVER: symmetric key after decryption:");
                foreach (var bite in key)
                {
                    Console.Write(bite.ToString());
                }
                c.alg = new NetAESEncryption(Globals_Server.server, key, 0, key.Length);
                ProtoClient clientDetails = new ProtoClient(c);
                clientDetails.ActionType = Actions.LogIn;
                clientDetails.ResponseType = DisplayMessages.LogInSuccess;
                Server.SendViaProto(clientDetails, c.conn, c.alg);
                Globals_Game.RegisterObserver(c);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failure during decryption: " + e.GetType() + " " + e.Message + ";" + e.StackTrace);
                return false;
            }
        }
    }
}
