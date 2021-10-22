
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using System;
using System.IO;
using System.Linq;

namespace LuceneNetDemo
{
    public class Indexer
    {

        private IndexWriter writer;
        FSDirectory indexDirectory;

        public Indexer(string indexDirectoryPath)
        {
            //this directory will contain the indexes
            indexDirectory = FSDirectory.Open(indexDirectoryPath);

            //create the indexer
            writer = new IndexWriter(
                d: indexDirectory,
                a: new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30),
                mfl: IndexWriter.MaxFieldLength.UNLIMITED);
        }

        public void Close()
        {
            writer.Optimize();
            writer.Dispose();
            indexDirectory.Dispose();
        }

        private Document GetDocument(System.IO.FileStream file)
        {
            TextReader tr = new StreamReader(file);
            
            Document document = new Document();
            
            string line = tr.ReadLine();

            //index file contents
            Field contentField = new Field(
                name: LuceneConstants.CONTENTS,
                value: line,
                store: Field.Store.YES, index: Field.Index.ANALYZED);
            
            string name = file.Name.Split("\\").Last();

            //index file name
            Field fileNameField = new Field(LuceneConstants.FILE_NAME,
               value: name,
               store: Field.Store.YES, index: Field.Index.NOT_ANALYZED);

            //index file path
            Field filePathField = new Field(LuceneConstants.FILE_PATH,
               value: file.Name,
               store: Field.Store.YES, index: Field.Index.NOT_ANALYZED);

            document.Add(contentField);
            document.Add(fileNameField);
            document.Add(filePathField);

            return document;
        }

        private void IndexFile(System.IO.FileStream file)
        {
            Console.WriteLine("Indexing " + file.Name);
            Document document = GetDocument(file);
            writer.AddDocument(document);
        }

        public int CreateIndex(string dataDirPath)
        {
            //get all files in the data directory
            var files = System.IO.Directory.GetFiles(dataDirPath).Select(x => System.IO.File.OpenRead(x)); //new File(dataDirPath).listFiles();

            foreach (var file in files)
            {
                IndexFile(file);
            }
            return writer.NumDocs();
        }
    }
}
