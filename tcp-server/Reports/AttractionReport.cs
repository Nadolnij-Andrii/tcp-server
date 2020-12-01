using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    class AttractionReport
    {
        public List<ReportedAttraction> reportedAttractions { get; set; }
        public AttractionTotal attractionTotal { get; set; }
        public void GetAttractionReport(DateTime fromDate, DateTime toDate)
        {
            SqlConn conn = new SqlConn();
            List<Attraction> attractions = conn.selectAttractions("attractions", "");
            List<TransactionAttractions> transactionAttractions = conn.selectTransactionAttractions("transactions_attractions",
                       " date BETWEEN '" + fromDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 0:00:00' AND '" +
                       toDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 23:59:59'");
            List<AttractionType> attractionTypes = conn.selectAttractionTypes("attraction_types");
            int allGamesForMoney = 0, allGamesForBonuses = 0, allTotalGames = 0;

            decimal allSpendMoney = 0, allSpendBonuses = 0, allSpendTotal = 0, allIssuedTickets = 0;
            this.reportedAttractions = new List<ReportedAttraction>();
            foreach (Attraction attraction in attractions)
            {
                int gamesForMoney = 0, gamesForBonuses = 0, gamesTotal;
                decimal spendMoney = 0, spendBonuses = 0, spendTotal, issuedTickets = 0;
                string RentalStatus = "";
                foreach (TransactionAttractions transactionAttraction in transactionAttractions)
                {
                    if ((int)transactionAttraction.attractionId == (int)attraction.id)
                    {
                        if ((decimal)transactionAttraction.summ != 0)
                        {
                            gamesForMoney++;
                            spendMoney += (decimal)transactionAttraction.summ;

                        }
                        else if ((decimal)transactionAttraction.bonus != 0)
                        {
                            gamesForBonuses++;
                            spendBonuses += (decimal)transactionAttraction.bonus;
                        }
                        issuedTickets += (int)transactionAttraction.tickets;
                    }
                }
                gamesTotal = gamesForMoney + gamesForBonuses;
                spendTotal = spendMoney + spendBonuses;
                AttractionType attractionType = attractionTypes.Find(x => (int)x.attractionTypeId == (int)attraction.attractionType);
                if ((bool)attraction.attractionIsRental == true)
                {
                    RentalStatus = "Арендованный";
                }
                else
                {
                    RentalStatus = "Не арендованный";
                }
                allGamesForMoney += gamesForMoney;
                allGamesForBonuses += gamesForBonuses;
                allTotalGames += gamesTotal;
                allIssuedTickets += issuedTickets;
                allSpendMoney += spendMoney;
                allSpendBonuses += spendBonuses;
                allSpendTotal += spendTotal;
                reportedAttractions.Add(new ReportedAttraction(
                    (int)attraction.id,
                    (string)attraction.attractionName,
                    attraction.attractionPrice,
                    (string)attractionType.attractionTypeName,
                    RentalStatus,
                    gamesForMoney,
                    gamesForBonuses,
                    gamesTotal,
                    spendMoney,
                    spendBonuses,
                    spendTotal,
                    issuedTickets
                    ));
            }
            this.attractionTotal = new AttractionTotal(
                allGamesForMoney,
                allGamesForBonuses,
                allTotalGames,
                allSpendMoney,
                allSpendBonuses,
                allSpendTotal,
                allIssuedTickets
                );
        }
        public class ReportedAttraction
        {
            public int id { get; set; }
            public string name { get; set; }
            public decimal price { get; set; }
            public string type { get; set; }
            public string isarended { get; set; }
            public int gameformoney { get; set; }
            public int gameforbonuses { get; set; }
            public int allgames { get; set; }
            public decimal spendmoney { get; set; }
            public decimal spendbonuses { get; set; }
            public decimal allspend { get; set; }
            public decimal issedtickets { get; set; }
            
            public ReportedAttraction(
                int id,
                string name,
                decimal price,
                string type,
                string isarended,
                int gameformoney,
                int gameforbonuses,
                int allgames,
                decimal spendmoney,
                decimal spendbonuses,
                decimal allspend,
                decimal issedtickets
                )
            {
                this.id = id;
                this.name = name;
                this.price = price;
                this.type = type;
                this.isarended = isarended;
                this.gameformoney = gameformoney;
                this.gameforbonuses = gameforbonuses;
                this.allgames = allgames;
                this.spendmoney = spendmoney;
                this.spendbonuses = spendbonuses;
                this.issedtickets = issedtickets;
            }

        }
        public class AttractionTotal
        {
            public int gameformoney { get; set; }
            public int gameforbonuses { get; set; }
            public int allgames { get; set; }
            public decimal spendmoney { get; set; }
            public decimal spendbonuses { get; set; }
            public decimal allspend { get; set; }
            public decimal issedtickets { get; set; }
            public AttractionTotal(
                int gameformoney,
                int gameforbonuses,
                int allgames,
                decimal spendmoney,
                decimal spendbonuses,
                decimal allspend,
                decimal issedtickets
                )
            {
                this.gameformoney = gameformoney;
                this.gameforbonuses = gameforbonuses;
                this.allgames = allgames;
                this.spendmoney = spendmoney;
                this.spendbonuses = spendbonuses;
                this.issedtickets = issedtickets;
            }
        }
    }
}
