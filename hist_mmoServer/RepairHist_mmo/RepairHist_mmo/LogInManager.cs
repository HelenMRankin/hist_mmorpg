﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using Lidgren.Network;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace hist_mmorpg
{
    /// <summary>
    /// Class for handling logging in
    /// </summary>
    public static class LogInManager
    {
        /// <summary>
        /// Random number generator used for producing salts
        /// </summary>
        static RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
        /// <summary>
        /// Hash algorithm used throughout the login process
        /// </summary>
        static HashAlgorithm hash = new SHA256Managed();
        /// <summary>
        /// Maps username to session salt for that user
        /// </summary>
        private static Dictionary<string, byte[]> sessionSalts = new Dictionary<string, byte[]>();
        /// <summary>
        /// Dictionary mapping player username to password hash and salt- for use during testing, should use database for final. First byte array is hash, second is salt
        /// </summary>
        public static Dictionary<string, Tuple<byte[], byte[]>> users = new Dictionary<string, Tuple<byte[], byte[]>>();
        /// <summary>
        /// Server's own certificate
        /// </summary>
        public static X509Certificate2 ServerCert
        {
            get; set;
        }
        /// <summary>
        /// RSA algorithm for public key en/decryption
        /// </summary>
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
        public static byte[] ComputeHash(byte[] toHash,byte[]salt)
        {
            byte[] fullHash = new byte[toHash.Length+salt.Length];
            toHash.CopyTo(fullHash,0);
            salt.CopyTo(fullHash,toHash.Length);
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
            byte[] hash = ComputeHash(passBytes,salt);
            users.Add(username, new Tuple<byte[], byte[]>(hash, salt));
        }

        /// <summary>
        /// Retrieve password hash from database
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>password hash</returns>
        public static byte[] GetPasswordHash(string username)
        {
            Tuple<byte[],byte[]> hashNsalt;
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
        /// <returns>True if verified</returns>
        public static bool VerifyUser(string username, byte[] userhash)
        {
            byte[] sessionSalt;
            if (!sessionSalts.TryGetValue(username, out sessionSalt))
            {
                return false;
            }
            byte[] passwordHash = ComputeHash(GetPasswordHash(username),sessionSalt);
            return userhash.SequenceEqual(passwordHash);
        }

        /// <summary>
        /// Determines whether or not to accept the connection based on whether a user's username is recognised, and constructs a ProtoLogIn containing session salt
        /// </summary>
        /// <param name="client">Client who is trying to connect</param>
        /// <param name="response">Contains the server's certificate and the salts for the client to use for sending their password hash</param>
        /// <returns>True if accepted</returns>
        public static bool AcceptConnection(Client client, out ProtoLogIn response)
        {
            // Client is already logged in
            if (Globals_Game.IsObserver(client))
            {
                response = null;
                return false;
            }
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
            if(ServerCert!=null)
            {
                response.certificate = ServerCert.GetRawCertData();
            }
            return true;
        }

        /// <summary>
        /// Set up the certificate and RSA algorithm
        /// </summary>
        /// <param name="path">Path of certificate</param>
        /// <returns>true if successfully set up</returns>
        public static bool InitialiseCertificateAndRSA(string path)
        {
            try
            {
                path = Path.Combine(path, "ServerCert.pfx");
            }
            catch (Exception e)
            {
                Globals_Server.logError("Failed to initialise certificate: "+e);
                return false;
            }
            // Using dummy certificate for now- final version should make use of certificate stores
#if DEBUG
            ServerCert =
                    new X509Certificate2(path, "zip1020");
            Console.WriteLine("Verify? " + ServerCert.Verify());
            X509Chain chain = new X509Chain();
            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EndCertificateOnly;
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            rsa = (RSACryptoServiceProvider)ServerCert.PrivateKey;
            return true;
#else
            return false;
#endif
        }

        /// <summary>
        /// Attempt to log in a client. Catches OperationCancelledException
        /// </summary>
        /// <param name="login">Log in credentials</param>
        /// <param name="c">Client who is trying to log in</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public static bool ProcessLogIn(ProtoLogIn login, Client c,CancellationToken ct)
        {
            if (!VerifyUser(c.username,login.userSalt))
            {
                // error
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
                if (ct.IsCancellationRequested)
                {
                    ct.ThrowIfCancellationRequested();
                    return false;
                }
                else
                {
                    Server.SendViaProto(clientDetails, c.connection, c.alg);
                    Globals_Game.RegisterObserver(c);
                    return true;
                }
                
            }
            catch (CryptographicException e)
            {
                Globals_Server.logError("Failure during decryption: " + e.GetType() + " " + e.Message + ";" + e.StackTrace);
                return false;
            }
        }
    }
}
