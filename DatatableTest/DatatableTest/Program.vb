Imports System
Imports System.Data
Imports Microsoft.Data.SqlClient

Public Class DataAccessUtil
    Public Shared Property ConnectionString() As String
        Get
            Return "Server=localhost\SQLEXPRESS;Database=test;Trusted_Connection=True;Encrypt=False;"
        End Get
        Set(ByVal value As String)
            ConnectionString = value
        End Set
    End Property

    Public Shared Function GetDataAndFillDataTable(query As String, Optional pConnection As SqlConnection = Nothing) As DataTable
        Dim dataTable As New DataTable()
        If IsNothing(pConnection) = False Then
            Using command As New SqlCommand(query, pConnection)
                Using adapter As New SqlDataAdapter(command)
                    adapter.Fill(dataTable)
                End Using
            End Using
            Return dataTable
        Else
            Using connection As New SqlConnection(ConnectionString)
                connection.Open()
                Using command As New SqlCommand(query, connection)
                    Using adapter As New SqlDataAdapter(command)
                        adapter.Fill(dataTable)
                    End Using
                End Using
            End Using
            Return dataTable
        End If
    End Function

    Public Shared Sub BulkInsertDataTable(tempTableName As String, dataTable As DataTable, connection As SqlConnection)
        Using bulkCopy As New SqlBulkCopy(connection)
            bulkCopy.DestinationTableName = tempTableName
            bulkCopy.WriteToServer(dataTable)
        End Using
    End Sub

    Public Shared Sub ExecuteNonQuery(query As String, connection As SqlConnection)
        Using command As New SqlCommand(query, connection)
            command.ExecuteNonQuery()
        End Using
    End Sub
End Class

Public Class Test
    Public Sub New()
        Console.WriteLine("Test")
    End Sub

    Sub GetDataAndFillDataTable()
        ' Step 1: Fetch data into a DataTable
        Dim data = DataAccessUtil.GetDataAndFillDataTable("SELECT * FROM tblItem")

        ' Step 2: Create a connection
        Using connection As New SqlConnection(DataAccessUtil.ConnectionString)
            connection.Open()

            ' Step 3: Create a temporary table
            Dim createTempTableQuery As String = "
                CREATE TABLE #TempTable (
                    TranID INT,
                    ItemRefNo VARCHAR(50),
                    ItemTitle VARCHAR(100),
                    SaleRate FLOAT,
                    TransactionDate DATETIME
                );
            "
            DataAccessUtil.ExecuteNonQuery(createTempTableQuery, connection)

            ' Step 4: Bulk-insert data from the DataTable into the temporary table
            DataAccessUtil.BulkInsertDataTable("#TempTable", data, connection)

            ' Step 5: Perform operations on the temporary table
            Dim operationQuery As String = "
                SELECT * FROM #TempTable as dttemp inner join tblPricingList as dtpl on dttemp.TranID = dtpl.ItemID
                WHERE dttemp.SaleRate = 1200.5 and dttemp.TransactionDate Between '2023-10-01' and '2023-12-31';
            "
            Dim resultData = DataAccessUtil.GetDataAndFillDataTable(operationQuery, connection)
            Dim ds As DataSet = New DataSet()
            ds.Tables.Add(data)
            ds.Tables.Add(resultData)
            ds.Relations.Add("ItemID", ds.Tables(0).Columns("TranID"), ds.Tables(1).Columns("ItemID"))
            ' Set the table names
            ds.Tables(0).TableName = "tblItem"
            ds.Tables(1).TableName = "tblPricingList"

            ' Step 2: Create a new column 'TradePrice' in the 'data' table (tblItem)
            If Not ds.Tables(0).Columns.Contains("TradePrice") Then
                ds.Tables(0).Columns.Add("TradePrice", GetType(Decimal))
            End If

            ' Step 3: Use the relation to create a DataColumn Expression that fetches SaleRate from tblPricingList
            ds.Tables(0).Columns("TradePrice").Expression = "ParentItemID.SaleRate"
            'add a new column in data table with name of tradeprice that will extracted from relation

            ' Display the data in the DataTable (for demonstration purposes)
            For Each row As DataRow In resultData.Rows
                Console.WriteLine($"TranID: {row("TranID")}, ItemRefNo: {row("ItemRefNo")}, ItemTitle: {row("ItemTitle")}, SaleRate: {row("SaleRate")}, TransactionDate: {row("TransactionDate")}")
            Next

            ' Step 6: Drop the temporary table (optional, as it will be dropped automatically when the session ends)
            Dim dropTempTableQuery As String = "
                DROP TABLE #TempTable;
            "
            DataAccessUtil.ExecuteNonQuery(dropTempTableQuery, connection)
        End Using
    End Sub
End Class

Module Program
    Sub Main(args As String())
        Dim test As New Test()
        test.GetDataAndFillDataTable()
    End Sub
End Module