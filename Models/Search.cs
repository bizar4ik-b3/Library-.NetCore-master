namespace MvcMovie.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Lucene.Net.Analysis;
    using Lucene.Net.Analysis.Standard;
    using Lucene.Net.Documents;
    using Lucene.Net.Index;
    using Lucene.Net.QueryParsers.Classic;
    using Lucene.Net.Search;
    using Lucene.Net.Store;
    using Version = Lucene.Net.Util.LuceneVersion;

    public class Search
    {
        //private DataContext context;
        static private FSDirectory Directory { get; set; }
        static private DirectoryReader Reader { get; set; }
        static public IndexSearcher Searcher { get; set; }

        public Search()
        {
            InitializeComponent();
        }

        void InitializeComponent()
        {
            if (Directory == null)
                Directory = FSDirectory.Open(
                                 AppDomain.CurrentDomain.BaseDirectory + @"\App_Data\LuceneIndexes"
                              //AppDomain.CurrentDomain.BaseDirectory + @"/App_Data/LuceneIndexes" //For Linux
                              );
            if (Reader == null)
                Reader = DirectoryReader.Open(Directory);
            if (Searcher == null)
                Searcher = new IndexSearcher(Reader);

        }

        public void createIndex(LibDocument rec)
        {
            using (Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_48))
            {
                IndexWriterConfig config = new IndexWriterConfig(Version.LUCENE_48, analyzer);
                using (var writer = new IndexWriter(Directory, config))
                {

                    var document = new Document();

                    document.Add(new Field("Id", rec.id.ToString(), StringField.Store.YES, Field.Index.ANALYZED));
                    document.Add(new Field("Name", rec.Name.ToString(), StringField.Store.YES, Field.Index.ANALYZED));
                    document.Add(new Field("Path", rec.Path.ToString(), StringField.Store.YES, Field.Index.ANALYZED));
                    document.Add(new Field("Desc2", rec.Desc2.ToString(), StringField.Store.YES, Field.Index.ANALYZED));
                    document.Add(new Field("Desc1", rec.Desc1.ToString(), StringField.Store.YES, Field.Index.ANALYZED));
                    document.Add(new Field("FullText",
                        string.Format("{0} {1} {2} {3}", rec.Name, rec.Path, rec.Desc2, rec.Desc1)
                        , Field.Store.YES, Field.Index.ANALYZED
                        ));

                    writer.AddDocument(document);
                    // }

                    //writer.Optimize();
                    writer.Commit();
                }
            }
        }

        public List<SearchDoc> searchfield(string textSearch)
        {

            List<SearchDoc> list = new List<SearchDoc>();

            using (Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_48))
            {
                var queryParser = new QueryParser(Version.LUCENE_48, "FullText", analyzer);

                queryParser.AllowLeadingWildcard = true;

                var query = queryParser.Parse(textSearch);
                Sort sort = new Sort(SortField.FIELD_SCORE);

                var matches = Searcher.Search(query, null, 100, sort, true, true).ScoreDocs;
                matches.OrderBy(de => de.Score);

                foreach (var item in matches)
                {

                    var id = item.Doc;
                    var doc = Searcher.Doc(id);

                    SearchDoc searchDoc = new SearchDoc();

                    searchDoc.id = doc.Get("Id");
                    searchDoc.Name = doc.GetField("Name").GetStringValue();
                    searchDoc.Path = doc.GetField("Path").GetStringValue();
                    searchDoc.Desc1 = doc.GetField("Desc1").GetStringValue();
                    list.Add(searchDoc);
                }
            }
            return list;
        }




        public void DeleteFromIndex(LibDocument doc)
        {
            var directory = FSDirectory.Open(
                             AppDomain.CurrentDomain.BaseDirectory + @"\App_Data\LuceneIndexes"
                          );
            IndexReader indexReader = DirectoryReader.Open(directory);

            using (Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_48))
            {
                IndexWriterConfig config = new IndexWriterConfig(Version.LUCENE_48, analyzer);
                using (var writer = new IndexWriter(directory, config))
                {
                    var term = new Term("Id", doc.id.ToString());
                    writer.DeleteDocuments(term);
                    writer.DeleteUnusedFiles();
                    writer.ForceMergeDeletes();
                    writer.Commit();
                }
            }
        }
    }
}