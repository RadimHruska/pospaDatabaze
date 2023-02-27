using System;
using System.Data.SqlClient;

namespace tvorbaTabulek
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //-------------------------------------------nastavení konzoly----------------------------
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            //-------------------------------------------otevření databáze----------------------------
            string připojovacíŘetězec = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=lásky;Integrated Security=True;Connect Timeout=10;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection klíč = new SqlConnection(připojovacíŘetězec);         //výroba klíče k databázi
            try
            {
                klíč.Open();                                                    //odemčení databáze
            }
            catch (SqlException)                                                //akce po neúspěšném odemčení
            {
                Console.WriteLine("Databáze uvedená v connection stringu nenalezena.");
                Console.ReadKey();
                Environment.Exit(0);                                            //ukončení programu
            }
            //--------------------------------------------založení číselníku osob---------------------
            bool pravda = true;                                                 //pravda je pravda
            SqlCommand příkazSql = new SqlCommand("CREATE TABLE osoby (id integer IDENTITY(10, 5) PRIMARY KEY, jméno nvarchar(20) NOT NULL UNIQUE)", klíč);
            try
            {
                příkazSql.ExecuteNonQuery();                                    //založení tabulky
            }
            catch (SqlException)                                                //když se to nepovede
            {
                Console.WriteLine("Tabulka osoby již existuje.");
                pravda =false;                                                  //pravda je lež
            }
            if (pravda) Console.WriteLine("Tabulka osoby založena.");           //provede se v případě pravdy
            //--------------------------------------------založení tabulky vztahy---------------------
            try
            {
                pravda = true;                                                  //pravda je pravda
                příkazSql.CommandText = "CREATE TABLE vztahy(id1 int FOREIGN KEY REFERENCES osoby(id), id2 int FOREIGN KEY REFERENCES osoby(id), PRIMARY KEY(id1, id2))";
                příkazSql.ExecuteNonQuery();                                    //založení tabulky
            }
            catch (SqlException)                                                //když se to nepovede
            {
                Console.WriteLine("Tabulka vztahy již existuje.");
                pravda=false;                                                   //pravda je lež
            }
            if (pravda) Console.WriteLine("Tabulka vztahy založena.");          //provede se v případě pravdy
            //-------------------------------------------zavření databáze----------------------------
            klíč.Close();
            Console.ReadKey();
        }
    }
}