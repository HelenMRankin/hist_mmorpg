using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using QuickGraph;
using RiakClient;
namespace hist_mmorpg
{
    /// <summary>
    /// Initialises all server details
    /// </summary>
    class Server
    {
        //TODO initialise server IP address, port etc
        void initialise()
        {
            Globals_Server.rCluster = (RiakCluster)RiakCluster.FromConfig("riakConfig");
            Globals_Server.rClient = (RiakClient.RiakClient)Globals_Server.rCluster.CreateClient();
        }
        //TODO listen to incoming connections
        void listen()
        {
        }

        public Server()
        {
            initialise();
        }

        //TODO write all client details to database, remove client from connected list and close connection
        void disconnect()
        {

        }
        

    }
}
