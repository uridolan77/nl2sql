"Show me the total deposits for last month"
🎯 Intent: Aggregate | 🏷️ Entities: Deposit
✅ SQL: SELECT SUM(Deposits) as TotalDeposits FROM tbl_Daily_actions WHERE Date >= DATEADD(month, -1, GETDATE())
"What are the top 10 players by deposits?"
🎯 Intent: TopN | 🏷️ Entities: Player, Deposit
✅ SQL: SELECT TOP 10 PlayerID, SUM(Deposits) as TotalDeposits FROM tbl_Daily_actions_players GROUP BY PlayerID ORDER BY TotalDeposits DESC




"How many players registered yesterday?"
🎯 Intent: Count | 🏷️ Entities: Player
✅ SQL: SELECT COUNT(DISTINCT PlayerID) as PlayerCount FROM tbl_Daily_actions WHERE Date >= DATEADD(day, -1, GETDATE())
"Calculate GGR for this month"
🎯 Intent: Aggregate | 🏷️ Entities: Revenue
✅ SQL: SELECT SUM(BetsCasino + BetsSport + BetsLive) - SUM(WinsCasino + WinsSport + WinsLive) as GGR FROM tbl_Daily_actions WHERE Date >= DATEADD(month, -1, GETDATE())