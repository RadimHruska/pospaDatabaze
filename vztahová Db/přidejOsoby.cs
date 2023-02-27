using System;
using System.Data.SqlClient;

namespace přidejOsoby
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //--------------------------------------------------nastavení konzoly----------------------------------
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            //--------------------------------------------------otevření databáze----------------------------------
            string připojovacíŘetězec = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=lásky;Integrated Security=True;Connect Timeout=10;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection klíč = new SqlConnection(připojovacíŘetězec);     //výroba klíče k databázi
            try
            {
                klíč.Open();                                                //odemčení databáze
            }
            catch (SqlException)                                            //akce po neúspěšném odemčení
            {
                Console.WriteLine("Databáze uvedená v connection stringu nenalezena.");
                Console.ReadKey();
                Environment.Exit(0);                                        //ukončení programu
            }
            //--------------------------------------------------prohlídka obsahu tabulky osoby---------------------
            SqlCommand auto = new SqlCommand("SELECT * FROM osoby", klíč);  //výroba auta + naložení příkazem a klíčem
            SqlDataReader čtečkaŘádků = auto.ExecuteReader();               //auto přiváží tabulku do prohlížečky
            while (čtečkaŘádků.Read())                                      //krájení přivezené tabulky na řádky
            {
                Console.WriteLine("{0} {1}", čtečkaŘádků[0], čtečkaŘádků[1]); //zobrazování řádků tabulky
            }
            čtečkaŘádků.Close();                                            //zavření prohlížečky řádků tabulky
            //--------------------------------------------------přidávání jmen do tabulky osoby--------------------
            string jméno = "Noe";                                           //deklarace a inicializace pomocné proměnné
            while (jméno != "q")                                            //while-do (dokud nevložíš 'q', budu cyklit)
            {
                do                                                          //do-while (budu cyklit, dokud nevložíš 'a' i jméno)
                {
                    Console.Write("Vlož další jméno, cyklus ukonči vložením q: ");
                    jméno = Console.ReadLine();
                    Console.Write("Zadal jsi {0}, je to vpořádku a/n? ", jméno);
                }
                while (Console.ReadLine() != "a" || jméno=="");             //dvě svislítka zastupují logickou spojku OR
                auto.Parameters.AddWithValue("@jm", jméno);                 //přeložení obsahu z proměnné C# do autopřihrádky @jm
                auto.CommandText = "INSERT INTO osoby VALUES (@jm)";        //naložení SQL příkazu do autovozíku CommandText
                try
                {
                    auto.ExecuteNonQuery();                                 //odjezd k DB; startérem je metoda ExecuteNonQuery()
                }
                catch (SqlException)
                {
                    Console.WriteLine("Zadané jméno již v tabulce existuje.");
                }
                auto.Parameters.Clear();                                    //uvolnění autopřihrádky @jm
            }
            auto.CommandText = "DELETE FROM osoby WHERE id=@@identity";     //naložení autovozíku; 
            auto.ExecuteNonQuery();                                         //odjezd auta k DB 
            //---------------------------------------------------prohlídka aktualizovaného obsahu tabulky osoby----
            auto.CommandText = "SELECT * FROM osoby";                       //naložení autovozíku
            čtečkaŘádků = auto.ExecuteReader();                             //auto přováží tabulku do prohlížeče
            while (čtečkaŘádků.Read())                                      //krájení přivezené tabulky na řádky
            {
                Console.WriteLine("{0} {1}", čtečkaŘádků[0], čtečkaŘádků[1]); //zobrazování řádků tabulky
            }
            čtečkaŘádků.Close();
            //---------------------------------------------------ukončování programu-------------------------------
            klíč.Close();                                                   //zamčení databáze
            Console.ReadKey();                                              //čeká na stisk klávesy
        }
    }
}
