using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Web.Mvc;
namespace hist_mmorpg
{
    /// <summary>
    /// Class for permission handling
    /// Permissions are activity based,
    /// </summary>
    public static class PermissionManager
    {
        public delegate bool AuthorizeDelegate(PlayerCharacter pc, object o = null);

        public static bool OwnsArmy(PlayerCharacter pc, object o)
        {
            if (o == null) return false;
            Army army = (Army)o;
            return (army.GetOwner() == pc);
        }

        public static bool OwnsFief(PlayerCharacter pc, object o)
        {
            if (o == null) return false;
            Fief fief = (Fief)o;
            return (fief.owner == pc);
        }

        public static bool OverlordOfFief(PlayerCharacter pc, object o)
        {
            if (o == null) return false;
            Fief fief = (Fief)o;
            return (fief.GetOverlord() == pc);
        }

        public static bool HeadOfFamily(PlayerCharacter pc, object o)
        {
            if (o == null) return false;
            Character c = (Character)o;
            return (c.GetHeadOfFamily() == pc);
        }

        public static bool isAdmin(PlayerCharacter pc, object o = null)
        {
            return pc.CheckIsSysAdmin();
        }
        public static bool isKing(PlayerCharacter pc, object o = null)
        {
            return pc.CheckIsKing();
        }
        public static bool isHerald(PlayerCharacter pc, object o = null) {
            return pc.CheckIsHerald();
        }
        public static bool isPrince(PlayerCharacter pc, object o = null)
        {
            return pc.CheckIsPrince();
        }
        public static bool isAlive(PlayerCharacter pc, object o = null)
        {
            return pc.isAlive;
        }
        public static bool isAuthorized(AuthorizeDelegate[] delegates, PlayerCharacter pc, Object o)
        {
            bool b = false;
            foreach (AuthorizeDelegate d in delegates)
            {
                b = b || d(pc,o);
            }
            return b;
        }
        public static bool ownsCharacter(PlayerCharacter pc, object o)
        {
            Character character = (Character)o;
            if (character is PlayerCharacter)
            {
                return (character as PlayerCharacter) == pc;
            }
            else{
                return (character as NonPlayerCharacter).GetHeadOfFamily() == pc || (character as NonPlayerCharacter).GetEmployer() == pc;
            }
        }

        static AuthorizeDelegate[] delegates = { OverlordOfFief, OwnsFief,isAlive };
        static PlayerCharacter pc = new PlayerCharacter();
        static Fief f = new Fief();
        static bool test = isAuthorized(delegates,pc,f);
    }
}
