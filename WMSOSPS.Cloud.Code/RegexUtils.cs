using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace WMSOSPS.Cloud.Code
{
    public class RegexUtils
    {
        public static bool IsMatch(string regex, string str, RegexOptions options)
        {
            try
            {
                Regex testRegex = new Regex(regex, options);
                if (testRegex.IsMatch(str))
                    return true;
                else
                    return false;
            }
            catch (ArgumentException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("\nThere was an error in your regular expression:\r\n" + ex.Message);
                Console.ResetColor();
                return false;
            }
        }

        //using System.Text.RegularExpressions;
        //using System.Collections;
        public static List<string> RegexMatches(string regex, string str, RegexOptions options)
        {
            try
            {
                Regex matchesRegex = new Regex(regex, options);

                var matchesFound = matchesRegex.Matches(str);

                List<string> rAL = new List<string>();

                foreach (Match matchMade in matchesFound)
                    rAL.Add(matchMade.Value);

                return rAL;
            }
            catch (ArgumentException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("\nThere was an error in your regular expression:\r\n" + ex.Message);
                Console.ResetColor();
                List<string> nAL = new List<string>();
                return nAL;
            }
        }

        public static string RegexMatches(string regex, string str, RegexOptions options, int index)
        {
            ArrayList rAL = new ArrayList();

            try
            {
                Regex matchesRegex = new Regex(regex, options);

                MatchCollection matchesFound;
                matchesFound = matchesRegex.Matches(str);

                //ArrayList rAL = new ArrayList();

                foreach (Match matchMade in matchesFound)
                    rAL.Add(matchMade.Value);

                return rAL[index].ToString();


            }
            catch (ArgumentException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("\nThere was an error in your regular expression:\r\n" + ex.Message);
                Console.ResetColor();
                ArrayList nAL = new ArrayList();
                return "";
            }
        }

        //using System.Text.RegularExpressions;
        public static string RegexReplace(string regex, string newStr, string oldStr, RegexOptions options)
        {
            try
            {
                string returnstr = "";
                Regex replaceRegex = new Regex(regex, options);
                returnstr = replaceRegex.Replace(oldStr, newStr);
                return returnstr;
            }
            catch (ArgumentException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("\nThere was an error in your regular expression:\r\n" + ex.Message);
                Console.ResetColor();
                return oldStr;
            }
        }

        //注意：此代码需要using System.Text.RegularExpressions;            
        public static string[] SplitByRegex(string str, string regex, RegexOptions options)
        {
            try
            {
                String[] splitResults;
                Regex splitRegex = new Regex(regex, options);
                splitResults = splitRegex.Split(str);
                return splitResults;
            }
            catch (ArgumentException ex)
            {
                System.Console.Write("\nThere was an error in your regular expression:\r\n" + ex.Message);
                string[] falseResults = new string[] { };
                return falseResults;
            }
        }

        //注意：此代码需要using System.Text.RegularExpressions;            
        public static ArrayList SplitByRegexToAL(string str, string regex, RegexOptions options)
        {

            try
            {
                ArrayList rAL = new ArrayList();
                String[] splitResults;
                Regex splitRegex = new Regex(regex, options);
                splitResults = splitRegex.Split(str);

                foreach (string t in splitResults)
                    rAL.Add(t);
                return rAL;
            }
            catch (ArgumentException ex)
            {
                ArrayList rAL = new ArrayList();
                System.Console.Write("\nThere was an error in your regular expression:\r\n" + ex.Message);
                return rAL;
            }
        }

    }
}
