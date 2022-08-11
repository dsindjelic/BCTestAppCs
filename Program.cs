using System;
using RestSharp;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;


namespace BCTestApp1
{
    public class Rootobject
    {
        public Book[] books { get; set; }
    }

    public class Book
    {
        public int id { get; set; }
        public string display_name { get; set; }
        public string abbr { get; set; }
        public string source_name { get; set; }
        public Meta meta { get; set; }
        public string parent_name { get; set; }
        public int? book_parent_id { get; set; }
        public int? affiliate_id { get; set; }

        public static implicit operator Book(List<Book> v)
        {
            throw new NotImplementedException();
        }
    }

    public class Meta
    {
        public Logos logos { get; set; }
        public string[] states { get; set; }
        public string website { get; set; }
        public Deeplink deeplink { get; set; }
        public bool is_legal { get; set; }
        public int betsync_type { get; set; }
        public bool is_preferred { get; set; }
        public string primary_color { get; set; }
        public int betsync_status { get; set; }
        public string secondary_color { get; set; }
        public bool is_fastbet_enabled_app { get; set; }
        public bool is_fastbet_enabled_web { get; set; }
        public bool is_promoted { get; set; }
        public bool is_supported { get; set; }
    }

    public class Logos
    {
        public string promo { get; set; }
        public string primary { get; set; }
        public string thumbnail { get; set; }
        public string betslip_carousel { get; set; }
        public string brand_dark { get; set; }
        public string brand_light { get; set; }
    }

    public class Deeplink
    {
        public bool has_multi { get; set; }
        public bool is_supported { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            getBooks();//calling function
        }
        //function
        public static void getBooks()
        {
            // get json data from remote site by api
            var client = new RestClient("https://api.actionnetwork.com/web/v1/");
            var request = new RestRequest("books");
            var response = client.Execute(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string rawResp = response.Content;//raw data
                Rootobject listOfBooks=JsonConvert.DeserializeObject<Rootobject>(rawResp);// json deserialized list of Book objects

                //Filter out
                var filteredBooks = new List<Book>();
                foreach (Book book in listOfBooks.books)
                {
                    if((book.parent_name!=null)&&(Array.Exists(book.meta.states,element=>element=="CO"))||((Array.Exists(book.meta.states, element => element == "NJ"))))
                        {
                        filteredBooks.Add(book);
                        }
                   
                }
                
                //Group and order by parent_name
                StreamWriter bookWrite = new StreamWriter("result.txt");//create stream and a file, open a streamewriter
                //generate group and order rules
                var filteredBooksGroupedBy_p_n =
                    from book in filteredBooks
                    group book by book.parent_name into newFilteredBooksGroup
                    orderby newFilteredBooksGroup.Key
                    select newFilteredBooksGroup;
                //grouping and ordering and writing to file
                foreach (var bookGroup in filteredBooksGroupedBy_p_n)
                {
                    bookWrite.WriteLine($"{bookGroup.Key}");
                    foreach (var element in bookGroup)
                    {
                        bookWrite.Write($"\t{element.display_name}");
                        foreach (var e in element.meta.states)
                        {
                            bookWrite.Write($"  {e}");
                        }
                        bookWrite.Write(Environment.NewLine);
                    }
                }


                bookWrite.Close();

            }
        }
    }
    
}
