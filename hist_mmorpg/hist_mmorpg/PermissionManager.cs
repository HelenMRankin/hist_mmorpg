using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// <summary>
        /// Method to determine if player has permission to view fief
        /// </summary>
        /// <param name="pc">PlayerCharacter who wants to view fief</param>
        /// <param name="o">Fief to view</param>
        /// <returns></returns>
        public static bool canSeeFief(PlayerCharacter pc, object o)
        {
            Fief f = o as Fief;
            if (pc.ownedFiefs.Contains(f))
            {
                return true;
            }
            bool isInFief = (pc.location == f);
            foreach (Character character in pc.myNPCs)
            {
                if (character.location == f||isInFief)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Method to determine if a PlayerCharacter owns, or is, a character
        /// </summary>
        /// <param name="pc">PlayerCharacter who is/owns </param>
        /// <param name="o"></param>
        /// <returns></returns>
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

        public static AuthorizeDelegate[] ownsCharOrAdmin = { isAdmin,ownsCharacter };
        public static AuthorizeDelegate[] ownsFiefOrAdmin = { OwnsFief, isAdmin };
        public static AuthorizeDelegate[] ownsArmyOrAdmin = { OwnsArmy, isAdmin };
        public static AuthorizeDelegate[] canSeeFiefOrAdmin = { canSeeFief, isAdmin };
    }
}
