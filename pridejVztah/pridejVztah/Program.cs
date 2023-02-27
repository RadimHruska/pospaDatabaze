using System;
using System.Data.SqlClient;

namespace přidejVztah
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //----------------------------nastavení konzoly--------------------------------------------------       
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            //----------------------------otevření databáze--------------------------------------------------
            string připojovacíŘetězec = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=vztahy;Integrated Security=True;Connect Timeout=10;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection klíč = new SqlConnection(připojovacíŘetězec);
            try
            {
                klíč.Open();
            }
            catch (SqlException)
            {
                Console.WriteLine("Databáze uvedená v connection stringu nenalezena.");
                Console.ReadKey();
                System.Environment.Exit(1);
            }
            //-----------------------------prohlídka obsahu tabulky osoby------------------------------------
            SqlCommand příkazSql = new SqlCommand("SELECT jméno FROM osoby", klíč);
            SqlDataReader čtečkaŘádků = příkazSql.ExecuteReader();
            while (čtečkaŘádků.Read()) Console.WriteLine("\t{0}", čtečkaŘádků[0]);
            čtečkaŘádků.Close();
            Console.WriteLine();
            //-----------------------------prohlídka obsahu tabulky vztahy-----------------------------------
            Console.WriteLine("\tAktuální dvojice:");
            příkazSql.CommandText = "SELECT osoby.jméno, dvojníci.jméno FROM osoby, osoby dvojníci, vztahy WHERE id1=osoby.id AND id2=dvojníci.id";
            čtečkaŘádků = příkazSql.ExecuteReader();
            while (čtečkaŘádků.Read()) Console.WriteLine("\t{0} {1}", čtečkaŘádků[0], čtečkaŘádků[1]);
            čtečkaŘádků.Close();
            //------------------------------výběr dvojic z tabulky osoby-------------------------------------
            int id_osoba, id_dvojče, inverze = 0; //0 znamená neexistenci obrácené dvojice k dvojici existující
            string jm_osoba, jm_dvojče, ano = "a";
            Console.WriteLine("");              //vložení prázdného řádku
            while (ano == "a")
            {
                první:
                Console.Write("\tVytvoř vztah - zadej jméno prvního partnera: ");
                jm_osoba = Console.ReadLine();
                příkazSql.Parameters.AddWithValue("@osoba", jm_osoba);
                příkazSql.CommandText = "SELECT id FROM osoby WHERE @osoba=jméno";
                try
                {
                    id_osoba = (int)příkazSql.ExecuteScalar(); //metoda vracející ze SELECTu jen jeden údaj!!!
                }
                catch (NullReferenceException)
                {
                    Console.WriteLine("\tOsoba tohoto jména v databázi neexistuje");
                    příkazSql.Parameters.Clear();
                    goto první;
                }
                druhý:
                Console.Write("\tVytvoř vztah - zadej jméno druhého partnera: ");
                jm_dvojče = Console.ReadLine();
                příkazSql.Parameters.AddWithValue("@dvojče", jm_dvojče);
                příkazSql.CommandText = "SELECT id FROM osoby WHERE @dvojče=jméno";
                try
                {
                    id_dvojče = (int)příkazSql.ExecuteScalar();
                }
                catch (NullReferenceException)
                {
                    Console.WriteLine("\tOsoba tohoto jména v databázi neexistuje");
                    příkazSql.Parameters.Clear();
                    goto druhý;
                }
                //------------------------------zápis id osob do tabulky vztahy----------------------------------
                příkazSql.Parameters.AddWithValue("@id1", id_osoba);
                příkazSql.Parameters.AddWithValue("@id2", id_dvojče);
                příkazSql.CommandText = "INSERT INTO vztahy (id1, id2) VALUES (@id2, @id1)"; //přehození pořadí = test na neexistenci inverzní dvojice
                try
                {
                    příkazSql.ExecuteNonQuery();
                }
                catch (SqlException)
                {
                    Console.WriteLine("\tTato dvojice již existuje v obráceném pořadí");
                    inverze = 1;    //nastavení příznaku existence inverzní dvojice
                }
                if (inverze != 1)   //pasáž běžící v případě neexistence inverzní dvojice
                {
                    příkazSql.CommandText = "DELETE FROM vztahy WHERE id1=@id2 AND id2=@id1";
                    příkazSql.ExecuteNonQuery(); //vymazání obrácené dvojice, která jen testovala databázi
                    příkazSql.CommandText = "INSERT INTO vztahy (id1, id2) VALUES (@id1, @id2)";
                    try
                    {
                        příkazSql.ExecuteNonQuery();    //vložení dvojice v původním pořadí
                    }
                    catch (SqlException)
                    {
                        Console.WriteLine("\tTato dvojice již existuje");
                    }
                }
                příkazSql.Parameters.Clear();
                inverze = 0;                            //vynulování příznaku existence inverzní dvojice
                                                        //------------------------------interaktivní podmínka while cyklu--------------------------------
                Console.Write("\tVložit novou dvojici? a/n: ");
                ano = Console.ReadLine();
            }
            //------------------------------prohlídka aktualizovaného obsahu tabulky vztahy------------------
            Console.WriteLine("\n\tAktuální dvojice:");
            příkazSql.CommandText = "SELECT osoby.jméno, dvojníci.jméno FROM osoby, osoby dvojníci, vztahy WHERE id1=osoby.id AND id2=dvojníci.id";
            čtečkaŘádků = příkazSql.ExecuteReader();
            while (čtečkaŘádků.Read()) Console.WriteLine("\t{0} {1}", čtečkaŘádků[0], čtečkaŘádků[1]);
            //------------------------------ukončování programu----------------------------------------------
            čtečkaŘádků.Close();
            klíč.Close();
            Console.ReadKey();
        }
    }
}

