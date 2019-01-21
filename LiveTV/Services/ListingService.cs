using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LiveTV.Entities;
using LiveTV.Models;

namespace LiveTV.Services
{
    public class ListingService : IListingService
    {
        public List<Channels> getAllChannels()
        {
            IList<Channels> returnChannels;
            var sefact = WebApiApplication.cfg.BuildSessionFactory();

            using (var session = sefact.OpenSession())
            {

                using (var tx = session.BeginTransaction())
                {
                    returnChannels = session
                        .CreateQuery("select c from Channels c").List<Channels>();
                }
            }

            return returnChannels.ToList<Channels>();
        }

        public ListingInfo getCurrentListings(int hours)
        {
            ListingInfo listingInfo = new ListingInfo();

            List<Programs> programs = findCurrentPrograms(hours);
            List<Channels> channels = getAllChannels();
            List<Listing> listings = new List<Listing>();
            Dictionary<long, string> channelMap = new Dictionary<long, string>();

            Dictionary<string, List<Programs>> programMap = new Dictionary<string, List<Programs>>();

            DateTime beginningOfRange = DateTime.UtcNow;
            beginningOfRange = beginningOfRange.AddSeconds(-beginningOfRange.Second);

            if (beginningOfRange.Minute < 15)
            {
                beginningOfRange = beginningOfRange.AddMinutes(-beginningOfRange.Minute);
            }
            else if (beginningOfRange.Minute < 30)
            {
                beginningOfRange = beginningOfRange.AddMinutes(-(beginningOfRange.Minute - 15));
            }
            else if (beginningOfRange.Minute < 45)
            {
                beginningOfRange = beginningOfRange.AddMinutes(-(beginningOfRange.Minute - 30));
            }
            else
            {
                beginningOfRange = beginningOfRange.AddMinutes(-(beginningOfRange.Minute - 45));
            }

            listingInfo.addListingOffset(beginningOfRange.ToString("yyyy-MM-dd'T'HH:mm'Z'"));

            int minutesInSegment = (hours * 60) / 6;

            DateTime next = beginningOfRange;
            for (int i = 0; i < 5; i++)
            {
                next = next.AddMinutes(minutesInSegment);
                listingInfo.addListingOffset(next.ToString("yyyy-MM-dd'T'HH:mm'Z'"));
            }


            foreach (Channels channel in channels)
            {
                String channelName = channel.DisplayName;
                channelMap.Add(channel.ChannelID, channelName);
            }

            foreach (Programs program in programs)
            {
                List<Programs> channelPrograms;
                if (programMap.ContainsKey(channelMap[program.ChannelID]))
                {
                    channelPrograms = programMap[channelMap[program.ChannelID]];
                }
                else
                {
                    channelPrograms = new List<Programs>();
                    programMap.Add(channelMap[program.ChannelID], channelPrograms);
                }

                channelPrograms.Add(program);
            }

            foreach (List<Programs> channelPrograms in programMap.Values)
            {

                Listing listing = new Listing();
                String channelDisplayName = channelMap[channelPrograms[0].ChannelID];
                String[] channelParts = channelDisplayName.Split(' ');

                listing.ChannelNumber = channelParts[0];
                if (channelParts.Length > 1)
                {
                    listing.ChannelName = channelParts[1];
                }

                int minutesInRange = hours * 60;
                DateTime endOfRange = beginningOfRange.AddMinutes(minutesInRange);
                
                double diffInMinutes = 0;
                long totalPercents = 0;

                for (int i = 0; i < channelPrograms.Count; i++)
                {
                    Programs program = channelPrograms[i];

                    ProgramInfo info = new ProgramInfo();
                    info.Title = program.Title;
                    info.SubTitle = program.SubTitle;

                    info.CategoryColor = getCategoryColor(program.Category);

                    info.ProgramID = program.ProgramID;

                    int percent = 0;

                    if (i == channelPrograms.Count - 1)
                    {
                        percent = (int)(92 - totalPercents);

                    }
                    else
                    {
                        if (beginningOfRange >= program.StartTime &&
                                endOfRange <= program.StopTime)
                        {
                            diffInMinutes = minutesInRange;
                        }
                        else if (beginningOfRange >= program.StartTime &&
                              endOfRange > program.StopTime)
                        {

                            // minutes from beginning of range to program stop time
                            TimeSpan difference = program.StopTime - beginningOfRange;
                            diffInMinutes = difference.TotalMinutes;

                        }
                        else if (beginningOfRange < program.StartTime &&
                              endOfRange <= program.StopTime)
                        {

                            // minutes from program start time to end of range
                            TimeSpan difference = endOfRange - program.StartTime;
                            diffInMinutes = difference.TotalMinutes;

                        }
                        else
                        {

                            // program falls completely in range
                            TimeSpan difference = program.StopTime - program.StartTime;
                            diffInMinutes = difference.TotalMinutes;
                        }

                        percent = (int)(((double)diffInMinutes / 180) * 92);
                    }

                    totalPercents += percent;

                    info.TimeRangePercent = percent;

                    listing.addProgramInfo(info);
                }

                listings.Add(listing);
            }

            listingInfo.Listings = sortListings(listings);

            return listingInfo;
        }

        private List<Listing> sortListings(List<Listing> unsorted)
        {
            List<Listing> sorted = new List<Listing>();
            
            int lastLowestChannel = 0;

            for (int i = 0; i < unsorted.Count; i++)
            {
                int currentLowestChannel = int.MaxValue;
                Listing currentListing = null;
                for (int j = 0; j < unsorted.Count; j++)
                {
                    int channelNumber = int.Parse(unsorted[j].ChannelNumber);
                    if (channelNumber < currentLowestChannel && channelNumber > lastLowestChannel)
                    {
                        currentLowestChannel = channelNumber;
                        currentListing = unsorted[j];
                    }
                }

                if(currentLowestChannel < int.MaxValue)
                {
                    sorted.Add(currentListing);
                    lastLowestChannel = currentLowestChannel;
                }
            }

            return sorted;
        }

        private List<Programs> findCurrentPrograms(int hours)
        {
            List<Programs> currentPrograms = new List<Programs>();
            var sefact = WebApiApplication.cfg.BuildSessionFactory();

            using (var session = sefact.OpenSession())
            {

                using (var tx = session.BeginTransaction())
                {
                    string nativeQuery =
@"SELECT [ProgramID]
      ,[ChannelID]
      ,[StartTime]
      ,[StopTime]
      ,[Title]
      ,[Desc]
      ,[Category]
      ,[SubTitle]  
FROM Programs 
WHERE (StartTime >= GETUTCDATE() AND StartTime < DATEADD(HOUR, 3, GETUTCDATE()))  
OR (StartTime < GETUTCDATE() AND StopTime > GETUTCDATE())  
ORDER BY StartTime, ChannelID";
                    var programs = session
                        .CreateSQLQuery(nativeQuery).List();

                    foreach(Object[] result in programs)
                    {
                        
                            Programs program = new Programs();
                            program.ProgramID = (Int32)result[0];
                            program.ChannelID = (Int32)result[1];
                            program.StartTime = (DateTime)result[2];
                            program.StopTime = (DateTime)result[3];
                            program.Title = (string)result[4];
                            program.Desc = (string)result[5];
                            program.Category = (string)result[6];
                            program.SubTitle = (string)result[7];

                            currentPrograms.Add(program);
                        
                    }

                }
            }

            return currentPrograms;
        }

        public Programs getProgramById(int id)
        {
            IList<Programs> programs;
            var sefact = WebApiApplication.cfg.BuildSessionFactory();

            using (var session = sefact.OpenSession())
            {

                using (var tx = session.BeginTransaction())
                {
                    programs = session
                        .CreateQuery("select p from Programs p where p.ProgramID = ?")
                        .SetInt32(0, id)
                        .List<Programs>();
                }
            }
            List<Programs> returnProgram = programs.ToList<Programs>();
            return returnProgram[0];
        }

        private String getCategoryColor(String category)
        {
            String categoryColor;

            switch (category)
            {
                case "Action":
                case "Adventure":
                case "Suspense":
                case "War":
                case "Military":
                case "Western":
                case "Fantasy":
                case "Paranormal":
                case "Science":
                case "Science fiction":
                case "Horror":
                    categoryColor = "Teal";
                    break;

                case "Comedy":
                case "Comedy-drama":
                case "Crime":
                case "Crime drama":
                case "Documentary":
                case "Biography":
                case "Drama":
                case "Docudrama":
                case "Mystery":
                case "Historical drama":
                case "History":
                    categoryColor = "DarkRed";
                    break;

                case "Auction":
                case "Auto":
                case "News":
                case "Educational":
                case "Environment":
                case "Newsmagazine":
                case "Talk":
                case "Travel":
                case "Public affairs":
                case "Politics":
                case "Weather":
                case "Community":
                case "Consumer":
                case "Cooking":
                case "Interview":
                case "Home improvement":
                case "Health":
                case "House/garden":
                case "How-to":
                    categoryColor = "MidnightBlue";
                    break;

                case "Outdoors":
                case "Basketball":
                case "Pro wrestling":
                case "Rugby":
                case "Soccer":
                case "Sports event":
                case "Sports non-event":
                case "Sports talk":
                case "Fishing":
                case "Football":
                case "Triathlon":
                case "Mixed martial arts":
                case "Hunting":
                case "Game show":
                case "Gaming":
                    categoryColor = "DarkGreen";
                    break;

                case "Shopping":
                case "Reality":
                case "Religious":
                case "Romance":
                case "Romance-comedy":
                case "Music":
                case "Musical":
                case "Musical comedy":
                case "Fashion":
                case "Exercise":
                    categoryColor = "DarkSlateBlue";
                    break;

                case "Series":
                case "Sitcom":
                case "Soap":
                case "Variety":
                case "Spanish":
                case "Special":
                case "Anthology":
                case "Entertainment":
                case "Children":
                case "Animals":
                case "Nature":
                    categoryColor = "Indigo";
                    break;

                default:
                    categoryColor = "DarkSlateGrey";
                    break;
            }

            return categoryColor;
        }
    }
}