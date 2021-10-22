using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System;
using Lucene.Net.Util;
using Lucene.Net.Documents;
using System.Diagnostics;
using Lucene.Net.Index;
using System.Linq;

namespace LuceneNetDemo
{
    class Program
    {

        static string indexDir = "_data/Index";
        static string dataDir = "_data/Data";

        static void Main(string[] args)
        {
            string input;
            while (true)
            {
                try
                {
                    DisplayOptions();
                    Console.Write("-> Enter option: ");
                    input = Console.ReadLine();

                    if (input.Equals("1"))
                    {
                        CreateIndex();
                    }
                    else if (input.Equals("2"))
                    {
                        Console.Write("-> Query string: ");
                        string queryStr = Console.ReadLine();
                        Console.WriteLine($"Searching {queryStr}");
                        SearchUsingFuzzyQuery(queryStr, LuceneConstants.CONTENTS);
                    }

                    Console.Write("Press ENTER to clear");

                    Console.ReadLine();
                    Console.Clear();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }
        }
        private static void CreateIndex()
        {
            var indexer = new Indexer(indexDir);
            int numIndexed = indexer.CreateIndex(dataDir);
            indexer.Close();
        }

        private static void SearchUsingFuzzyQuery(string searchQuery, string field)
        {
            var searcher = new Searcher(indexDir);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var startTime = stopwatch.ElapsedMilliseconds;

            //create a term to search file name
            Term term = new Term(field, searchQuery);

            //create the term query object
            Query query = new FuzzyQuery(term);

            //do the search
            TopDocs hits = searcher.Search(query);

            var endTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Stop();

            Console.WriteLine(hits.TotalHits + " documents found. Time :" + (endTime - startTime) + "ms");

            foreach (ScoreDoc scoreDoc in hits.ScoreDocs)
            {
                Document doc = searcher.GetDocument(scoreDoc);
                Console.WriteLine("Score: " + scoreDoc.Score + " ");
                Console.WriteLine("File: " + doc.Get(LuceneConstants.FILE_PATH));
            }

            searcher.Close();
        }

        static void DisplayOptions()
        {
            Console.WriteLine("1. Index files");
            Console.WriteLine("2. Fuzzy search");
        }
    }

}