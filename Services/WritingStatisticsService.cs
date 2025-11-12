using JoyLeeWrite.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoyLeeWrite.Services
{
    internal class WritingStatisticsService
    {
        public DbContext dbContext;
        public WritingStatisticsService()
        {
            dbContext = new JoyLeeWriteDbContext();
        }


        public List<(string Day, int WordCount)> GetLast7DaysWordStats(int userId)
        {
            using (var db = new JoyLeeWriteDbContext())
            {
                DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
                DateOnly startDate = today.AddDays(-6);

                var query = db.WritingStatistics
                    .Where(ws => ws.UserId == userId && ws.RecordDate >= startDate)
                    .GroupBy(ws => ws.RecordDate)
                    .Select(g => new
                    {
                        Day = g.Key,
                        WordCount = g.Sum(x => x.WordsWritten)
                    })
                    .OrderBy(x => x.Day)
                    .ToList();

                var result = query.Select(x =>
                {
                    string dayOfWeek = x.Day.DayOfWeek switch
                    {
                        DayOfWeek.Monday => "T2",
                        DayOfWeek.Tuesday => "T3",
                        DayOfWeek.Wednesday => "T4",
                        DayOfWeek.Thursday => "T5",
                        DayOfWeek.Friday => "T6",
                        DayOfWeek.Saturday => "T7",
                        DayOfWeek.Sunday => "CN",
                        _ => "?"
                    };
                    return (dayOfWeek, x.WordCount);
                }).ToList();

                return result;
            }
        }


    }
}
