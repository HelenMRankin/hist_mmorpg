using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Bson;

namespace hist_mmorpg
{
    public partial class TestClient
    {

        /*************************************
         * Battle-related Commands ***
         * **********************************/

        /// <summary>
        /// Examine all armies in a fief. Note: Must have a character in that fief
        /// </summary>
        /// <param name="fiefID">ID of fief to view</param>
        public void ExamineArmies(string fiefID)
        {
            ProtoMessage message = new ProtoMessage();
            message.Message = fiefID;
            message.ActionType = Actions.ExamineArmiesInFief;
            net.Send(message);
        }

        /// <summary>
        /// Recruit some troops for one of your armies
        /// </summary>
        /// <param name="armyID">ID of army to recruit for (will recruit from that army leader's location</param>
        /// <param name="numTroops">Number of troops to recruit</param>
        /// <param name="isConfirm">Boolean indicating that this message is confirming these details- will be changed once message sequences are established</param>
        public void RecruitTroops(string armyID, uint numTroops, bool isConfirm = false)
        {
            ProtoRecruit recruitDetails = new ProtoRecruit();
            recruitDetails.armyID = armyID;
            recruitDetails.amount = numTroops;
            recruitDetails.isConfirm = isConfirm;
            recruitDetails.ActionType = Actions.RecruitTroops;
            net.Send(recruitDetails);
        }

        /// <summary>
        /// Maintain the army indicated by the army ID
        /// </summary>
        /// <param name="armyID">Army to maintain</param>
        public void MaintainArmy(string armyID)
        {
            ProtoMessage maintain = new ProtoMessage();
            maintain.ActionType = Actions.MaintainArmy;
            maintain.Message = armyID;
            net.Send(maintain);
        }


        /// <summary>
        /// Appoint a new army leader
        /// </summary>
        /// <param name="armyID">ID of army to appoint leader to</param>
        /// <param name="charID">Character ID of character to become leader</param>
        public void AppointArmyLeader(string armyID, string charID)
        {
            ProtoMessage appoint = new ProtoMessage();
            appoint.Message = armyID;
            appoint.MessageFields = new string[] { charID };
            appoint.ActionType = Actions.AppointLeader;
            net.Send(appoint);
        }

        /// <summary>
        /// Disband an army
        /// </summary>
        /// <param name="armyID">ID of army to disband</param>
        public void DisbandArmy(string armyID)
        {
            ProtoMessage disband = new ProtoMessage();
            disband.ActionType = Actions.DisbandArmy;
            disband.Message = armyID;
            net.Send(disband);
        }

        public void Attack(string armyID, string targetID)
        {
            ProtoMessage attack = new ProtoMessage();
            attack.ActionType = Actions.Attack;
            attack.Message = armyID;
            attack.MessageFields = new string[] { targetID };
            net.Send(attack);
        }

        /// <summary>
        /// Creates an army detachment to leave for a player
        /// </summary>
        /// <param name="armyID">ID of army to create a detachment from</param>
        /// <param name="playerID">ID of player to leave detachment for</param>
        /// <param name="troops">Number of troops to leave
        /// 
        /// 0 = knights
        /// 1 = menAtArms
        /// 2 = lightCav
        /// 3 = longbowmen
        /// 4 = crossbowmen
        /// 5 = foot
        /// 6 = rabble</param>
        public void DropOffTroops(string armyID, string playerID, uint[] troops)
        {
            ProtoDetachment detachment = new ProtoDetachment();
            detachment.armyID = armyID;
            detachment.leftFor = playerID;
            detachment.troops = troops;
            detachment.ActionType = Actions.DropOffTroops;
            net.Send(detachment);
        }

        /// <summary>
        /// List the detachments available for an army
        /// </summary>
        /// <param name="armyID">ID of army to list detachments available for. Will return detachments based on army owner and army location</param>
        public void ListDetachments(string armyID)
        {
            ProtoMessage requestTransferList = new ProtoMessage();
            requestTransferList.Message = armyID;
            requestTransferList.ActionType = Actions.ListDetachments;
            net.Send(requestTransferList);
        }

        /// <summary>
        /// Pick up selected attachments and add them to an army
        /// </summary>
        /// <param name="selectedDetachments">Array of detachment IDs to add</param>
        /// <param name="armyID">ID of army to add detachments to</param>
        public void PickUpDetachments(string[] selectedDetachments, string armyID)
        {
            ProtoMessage requestPickups = new ProtoMessage();
            requestPickups.ActionType = Actions.PickUpTroops;
            requestPickups.MessageFields = selectedDetachments;
            requestPickups.Message = armyID;
            net.Send(requestPickups);
        }

        /// <summary>
        /// Adjust the combat odds and agression values for a particular army
        /// </summary>
        /// <param name="armyID">ID of army to adjust for</param>
        /// <param name="newOdds">Combat odds value</param>
        /// <param name="newAgg">Aggression value</param>
        public void AdjustOddsAndAgression(string armyID, byte newOdds, byte newAgg)
        {
            ProtoCombatValues newValues = new ProtoCombatValues();
            newValues.armyID = armyID;
            newValues.aggression = newAgg;
            newValues.odds = newOdds;
            newValues.ActionType = Actions.AdjustCombatValues;
            net.Send(newValues);
        }

        /// <summary>
        /// Get list of sieges you are currently involved in, whether as a besieger or defender
        /// </summary>
        public void GetSiegeList()
        {
            ProtoMessage siegelist = new ProtoMessage();
            siegelist.ActionType = Actions.SiegeList;
            net.Send(siegelist);
        }

        /// <summary>
        /// Get more in-depth information on a siege
        /// </summary>
        /// <param name="siegeID">ID of siege</param>
        public void GetSiege(string siegeID)
        {
            ProtoMessage viewSiege = new ProtoMessage();
            viewSiege.ActionType = Actions.ViewSiege;
            viewSiege.Message = siegeID;
            net.Send(viewSiege);
        }

        /// <summary>
        /// Pillage a fief
        /// </summary>
        /// <param name="fiefID">ID of fief to pillage</param>
        /// <param name="armyID">ID of army to be pillaging</param>
        public void PillageFief(string fiefID, string armyID)
        {
            ProtoMessage pillage = new ProtoMessage();
            pillage.ActionType = Actions.PillageFief;
            pillage.Message = armyID;
            net.Send(pillage);
        }

        /// <summary>
        /// Besiege the fief your army is currently in
        /// </summary>
        /// <param name="armyID">ID of army who will be besieging</param>
        public void Besiege(string armyID)
        {
            ProtoMessage besiege = new ProtoMessage();
            besiege.ActionType = Actions.BesiegeFief;
            besiege.Message = armyID;
            net.Send(besiege);
        }

        /// <summary>
        /// Conduct a storm round during this siege
        /// </summary>
        /// <param name="siegeID">ID of siege</param>
        public void StormRound(string siegeID)
        {
            ProtoMessage storm = new ProtoMessage();
            storm.ActionType = Actions.SiegeRoundStorm;
            storm.Message = siegeID;
            net.Send(storm);
        }

        /// <summary>
        /// Conduct a negotiation round during this siege
        /// </summary>
        /// <param name="siegeID">ID of siege</param>
        public void NegotiationRound(string siegeID)
        {
            ProtoMessage negotiate = new ProtoMessage();
            negotiate.ActionType = Actions.SiegeRoundNegotiate;
            negotiate.Message = siegeID;
            net.Send(negotiate);
        }


        /// <summary>
        /// Conduct a reduction round during this siege
        /// </summary>
        /// <param name="siegeID">ID of siege</param>
        public void ReductionRound(string siegeID)
        {
            ProtoMessage reduce = new ProtoMessage();
            reduce.ActionType = Actions.SiegeRoundReduction;
            reduce.Message = siegeID;
            net.Send(reduce);
        }

        /// <summary>
        /// End the siege
        /// </summary>
        /// <param name="siegeID">ID of siege</param>
        public void EndSiege(string siegeID)
        {
            ProtoMessage end = new ProtoMessage();
            end.ActionType = Actions.EndSiege;
            end.Message = siegeID;
            net.Send(end);
        }
    }
}
