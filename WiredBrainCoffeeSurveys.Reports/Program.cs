using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace WiredBrainCoffeeSurveys.Reports
{
    class Program
    {
         //if you want a loop, make a 'do while'
        static void Main(string[] args)
        {
            //if you want a loop, make a 'do while'
            bool quitApp = false;
            do
            {
                Console.WriteLine("Please specify a report to run (rewards, comments, tasks or quit): ");
                var selectedReport = Console.ReadLine();

                Console.WriteLine("Please specify awich quarter of data: (q1, q2) ");
                var selectedData = Console.ReadLine();

                var surveyResults = JsonConvert.DeserializeObject<SurveyResults>
                    (File.ReadAllText($"data/{selectedData}.json"));

                switch (selectedReport)
                {
                    case "rewards":
                        GenerateWinnerEmails(surveyResults);
                        break;

                    case "comments":
                        GenerateCommentsReport(surveyResults);
                        break;

                    case "tasks":
                        GenerateTasksReport(surveyResults);
                        break;

                    case "quit":
                        Console.WriteLine("ended....");
                        quitApp = true;
                        break; 

                    default:
                        Console.WriteLine("Sorry, that's not a valid option!!");
                        break;
                }
                Console.WriteLine();
            } while (!quitApp);
        }
            

        private static void GenerateWinnerEmails(SurveyResults results)
        {
            var selectedEmails = new List<string>();
            int counter = 0;

            Console.WriteLine(Environment.NewLine + "Selected Winners Output:");
            while (selectedEmails.Count < 2 && counter < results.Responses.Count)
            {
                var currenItem = results.Responses[counter];

                if (currenItem.FavoriteProduct == "Cappucino")
                {
                    selectedEmails.Add(currenItem.EmailAddress);
                    Console.WriteLine(currenItem.EmailAddress);
                }
                counter++;
            }
            File.WriteAllLines("WinnersReport.csv", selectedEmails);
        }

        public static void GenerateCommentsReport(SurveyResults results)
        {
            var comments = new List<string>();

            Console.WriteLine(Environment.NewLine + "Comments Output:");
            for (var i = 0; i < results.Responses.Count; i++)
            {
                var currentResponse = results.Responses[i];

                if (currentResponse.WouldRecommend < 7.0)
                {
                    Console.WriteLine(currentResponse.Comments);
                    comments.Add(currentResponse.Comments);
                }
            }

            foreach (var response in results.Responses)
            {
                if (response.AreaToImprove == results.AreaToImprove)
                {
                    Console.WriteLine(response.Comments);
                    comments.Add(response.Comments);
                }
            }
            File.WriteAllLines("CommentReport.csv", comments);
        }

        public static void GenerateTasksReport(SurveyResults results)
        {
            var tasks = new List<string>();

            //------- calculeted values
            double responseRate = results.NumberResponded / results.NumberSurveyed;
            double overallScore = (results.ServiceScore + results.CoffeeScore + results.FoodScore + results.PriceScore) / 4;


            if (results.CoffeeScore < results.FoodScore)
            {
                tasks.Add("Investigate coffee recipes and ingredients.");
            }

            //exemple improve loops 1
            tasks.Add(overallScore >8.0 ? "Work with leadership" : "Work with employees for ideas.");
            
            //if (overallScore > 8.0)
            //{
            //    tasks.Add("Work with leadership to reward staff. ");
            //}
            //else
            //{
            //    tasks.Add("Work with employees for improvement ideas. ");
            //}

            //exemple improve loops 2

            tasks.Add(responseRate switch
            {
                var rate when rate < .33 => "Reserch options to improve response rate.",
                var rate when rate > .33 && rate < .66 => "Reward participants with free coffee cupon.",
                var rate when rate > .66 => "Rewards participants with discount coffee coupon"
            });

            //if (responseRate < .33)
            //{
            //    tasks.Add("Reserch options to improve response rate.");
            //}
            //else if (responseRate > .33 && responseRate < .66)
            //{
            //    tasks.Add("Reward participants with free coffee cupon.");
            //}
            //else
            //{
            //    tasks.Add("Rewards participants with discount coffee coupon");
            //}

            //exemple improve loops 3
            tasks.Add(results.AreaToImprove switch
            {
                "RewardsProgram" => "Revisit the rewards deals. ",
                "Cleanliness" => "Contact the cleaning vendor. ",
                "MobileApp" => "Contact consulting foirm about app. ",
                _ => "Investigate individual comments for ideas."
            });
            //switch (results.AreaToImprove)
            //{
            //    case "RewardsProgram":
            //        tasks.Add("Revisit the rewards deals. ");
            //        break;

            //    case "Cleanliness":
            //        tasks.Add("Contact the cleaning vendor. ");
            //        break;

            //    case "MobileApp":
            //        tasks.Add("Contact consulting foirm about app. ");
            //        break;

            //    default:
            //        tasks.Add("Investigate individual comments for ideas.");
            //        break;
            //}
            Console.WriteLine(Environment.NewLine + "Tasks Output:");
            foreach(var task in tasks)
            {
                Console.WriteLine(task);
            }
            File.WriteAllLines("TaskReport.csv", tasks);

        }
    }
}
