﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace hist_mmorpg
{
    public interface Game_Observer
    {
        // method to update observer component
        void Update(String info);
    }

    public interface Game_Observable
    {
        // method to register observer
        void RegisterObserver(Form1 obs);

        // method to remove observer
        void RemoveObserver(Form1 obs);

        // method to notify observers of changes to subject
        void NotifyObservers(String info);
    }
}
