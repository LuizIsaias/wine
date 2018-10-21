using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Routing;
using Microsoft.Reporting.WebForms;
using System.Data.Sql;
using System.Collections.Specialized;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Data.SqlClient;
using System.Data.Common;
using System.Threading;

namespace Admin
{
    public partial class Relatorio : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            bDS bds = new bDS();
            List<ReportDataSource> rep = new List<ReportDataSource>();
            ReportDataSource reps = new ReportDataSource();
            reps.Name = "list";
            reps.Value = bds.DataSetName;
            rep.Add(reps);
        }
        static void Main()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(
                "Network Address=(local);Integrated Security=SSPI;" +
                "Initial Catalog=AdventureWorks");

            // Display the connection string, which should now 
            // contain the "Data Source" key, as opposed to the 
            // supplied "Network Address".
            Console.WriteLine(builder.ConnectionString);

            // Retrieve the DataSource property.
            Console.WriteLine("DataSource = " + builder.DataSource);

            Console.WriteLine("Press any key to continue.");
            Console.ReadLine();
        }
    }
}

class Program
{
    static void Main()
    {
        // Create a SqlConnectionStringBuilder instance, 
        // and ensure that it is set up for asynchronous processing.
        SqlConnectionStringBuilder builder =
            new SqlConnectionStringBuilder(GetConnectionString());
        // Asynchronous method calls won't work unless you
        // have added this option, or have added
        // the clause "Asynchronous Processing=true"
        // to the connection string.
        builder.AsynchronousProcessing = true;

        string commandText =
            "UPDATE Production.Product SET ReorderPoint = ReorderPoint + 1 " +
            "WHERE ReorderPoint IS NOT Null;" +
            "WAITFOR DELAY '0:0:3';" +
            "UPDATE Production.Product SET ReorderPoint = ReorderPoint - 1 " +
            "WHERE ReorderPoint IS NOT Null";
        RunCommandAsynchronously(commandText, builder.ConnectionString);

        Console.WriteLine("Press any key to finish.");
        Console.ReadLine();
    }

    private static string GetConnectionString()
    {
        // To avoid storing the connection string in your code,
        // you can retrieve it from a configuration file. 
        return "Data Source=(local);Integrated Security=SSPI;" +
            "Initial Catalog=AdventureWorks";
    }

    private static void RunCommandAsynchronously(string commandText,
        string connectionString)
    {
        // Given command text and connection string, asynchronously execute
        // the specified command against the connection. For this example,
        // the code displays an indicator as it's working, verifying the 
        // asynchronous behavior. 
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                int count = 0;
                SqlCommand command = new SqlCommand(commandText, connection);
                connection.Open();
                IAsyncResult result = command.BeginExecuteNonQuery();
                while (!result.IsCompleted)
                {
                    Console.WriteLine("Waiting {0}.", count);
                    // Wait for 1/10 second, so the counter
                    // doesn't consume all available resources 
                    // on the main thread.
                    Thread.Sleep(100);
                    count += 1;
                }
                Console.WriteLine("Command complete. Affected {0} rows.",
                    command.EndExecuteNonQuery(result));

            }
            catch (SqlException ex)
            {
                Console.WriteLine(
                    "Error {0}: System.Data.SqlClient.SqlConnectionStringBuilder",
                    ex.Number, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
            }
            catch (Exception ex)
            {
                // You might want to pass these errors
                // back out to the caller.
                Console.WriteLine("Error: {0}", ex.Message);
            }
        }
    }
}