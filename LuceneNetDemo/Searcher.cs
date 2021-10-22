using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.Text;
using static LuceneNetDemo.Program;

namespace LuceneNetDemo
{
    public class Searcher
    {

        IndexSearcher indexSearcher;
        QueryParser queryParser;
        Query query;

        public Searcher(string indexDirectoryPath)
        {
            Directory indexDirectory = FSDirectory.Open(indexDirectoryPath);

            indexSearcher = new IndexSearcher(indexDirectory);

            queryParser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30,
                f: LuceneConstants.CONTENTS,
                a: new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));
        }

        public TopDocs Search(string searchQuery)
        {
            var query = queryParser.Parse(searchQuery);

            return indexSearcher.Search(query, LuceneConstants.MAX_SEARCH);
        }

        public TopDocs Search(Query query)
        {
            return indexSearcher.Search(query, LuceneConstants.MAX_SEARCH);
        }

        public Document GetDocument(ScoreDoc scoreDoc)
        {
            return indexSearcher.Doc(scoreDoc.Doc);
        }

        public void Close()
        {
            indexSearcher.Dispose();
        }
    }
}
