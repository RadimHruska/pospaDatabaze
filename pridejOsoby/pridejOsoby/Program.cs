using System;
using System.Data.SqlClient;

namespace PřidejOsoby
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //-------------------------------------------------------------nastavení konzoly---------------------------------------------------------------------

            Console.BackgroundColor = ConsoleColor.DarkBlue;    //barvení pozadí konzoly
            Console.Clear();                                    //smaže původní černou barvu konzoly
            Console.ForegroundColor = ConsoleColor.White;       //barvení popředí konzoly
            //-------------------------------------------------------------otevření databáze------------------------------------------------------------------------

            string připojovacíŘetězec = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=vztahy;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"; 
            SqlConnection klíč = new SqlConnection(připojovacíŘetězec); //založení instance klíč třídy SQL connection
            try                                                         //zkus a chyť blok na ošetřování běhových chyb a chyb vstupů
            {
                klíč.Open();                                            //instance klíč odemkne databázi
            }
            catch (SqlException) //tento blok program přeskočí, když při otevření nenastane chyba
            {
                Console.WriteLine("Databáze uvedená v connectionStringu nenalezena.");
                Console.ReadKey();
                Environment.Exit(0); //metoda pro násilné ukončení programu!
            }
            //-------------------------------------------------------------prohlídka obsahu tabulky osoby------------------------------------------------------------------
            Console.WriteLine("Databáze připojena");
            SqlCommand auto = new SqlCommand("SELECT * FROM Osoby", klíč);  //výroba auta a jeho naložení SQL příkazem a klíčem DB
            SqlDataReader čtečkaŘádků = auto.ExecuteReader();               //jízda auta k DB a zpět a přivezení tabulky Osoby
            while (čtečkaŘádků.Read())                                      //krájení přivezené tabulky na řádky
            {
                Console.WriteLine($"{čtečkaŘádků[0]} {čtečkaŘádků[1]}");    //zobrazování řádků tabulky
            }
            čtečkaŘádků.Close();
            //-------------------------------------------------------------přidávání jmen do tabulky osoby-----------
            string name = "Noe";                        //deklarace a inicializace proměnné
            while (name != "q")                         //while-do:dokud nevložíš místo jména znak q,budeš vkládat další jména
            {
                do                                      //cyklus do-while:bude cyklit, dokud nevložíš'a'
                {
                    Console.Write("Vlož další jméno, cyklus ukonči klávesou q: ");
                    name = Console.ReadLine();
                    Console.Write("Zadal jsi {0}, je to vpořádku a/n ?: ", name); 
                } while (Console.ReadLine().ToLower()!= "a");
                auto.Parameters.AddWithValue("@jm", name);              //přeložení obsahu z proměnné C# do autoppřihrádky @name
                auto.CommandText = "INSERT INTO osoby VALUES (@jm)";    //naložení SQL příkazu do autovozíku CommandText
                try
                {
                    auto.ExecuteNonQuery();                             //odjezd k DB;startérem je metoda ExecuteNonQuery()
                }
                catch (SqlException)
                {
                    Console.WriteLine("Zadané jméno již v databázi existuje.");
                }
             auto.Parameters.Clear();                                   //uvolnění autopřihrádky @jm
            }
            auto.CommandText = "DELETE FROM osoby WHERE id=@@identity";
            auto.ExecuteNonQuery();
            //--------------------prohlídka aktualizovaného obsahu tabulky odoby----------------------
            auto.CommandText = "SELECT * FROM Osoby";   //naložení SQL příkazu do autovozíku CommandText
            čtečkaŘádků = auto.ExecuteReader();         //jízda auta a přivezení tabulky osoby do čtečkyŘádků
            while (čtečkaŘádků.Read())                  //krájení přivezené tabulky na řádky
            {
                Console.WriteLine($"{čtečkaŘádků[0]} {čtečkaŘádků[1]}");    //zobrazování řádků tabulky
            }
            čtečkaŘádků.Close();
            //--------------------ukončování programu----------------------------
            Console.ReadKey();
            klíč.Close();
        }
    }
}
