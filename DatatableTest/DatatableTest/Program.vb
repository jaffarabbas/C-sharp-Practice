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

    '    Sub GetDataAndFillDataTable()
    '        ' Step 1: Fetch data into a DataTable
    '        Dim data = DataAccessUtil.GetDataAndFillDataTable("SELECT * FROM tblItem")

    '        ' Step 2: Create a connection
    '        Using connection As New SqlConnection(DataAccessUtil.ConnectionString)
    '            connection.Open()

    '            ' Step 3: Create a temporary table
    '            Dim createTempTableQuery As String = "
    '                CREATE TABLE #TempTable (
    '                    TranID INT,
    '                    ItemRefNo VARCHAR(50),
    '                    ItemTitle VARCHAR(100),
    '                    SaleRate FLOAT,
    '                    TransactionDate DATETIME
    '                );
    '            "
    '            DataAccessUtil.ExecuteNonQuery(createTempTableQuery, connection)

    '            ' Step 4: Bulk-insert data from the DataTable into the temporary table
    '            DataAccessUtil.BulkInsertDataTable("#TempTable", data, connection)

    '            ' Step 5: Perform operations on the temporary table
    '            Dim operationQuery As String = "
    '                SELECT dttemp.TranID,dtpl.PricingTitle FROM #TempTable as dttemp inner join tblPricingList as dtpl on dttemp.TranID = dtpl.ItemID
    '                WHERE dttemp.SaleRate = 1200.5
    '            "
    '            Dim resultData = DataAccessUtil.GetDataAndFillDataTable(operationQuery, connection)
    '            Dim ds As DataSet = New DataSet()
    '            ds.Tables.Add(data)
    '            ds.Tables.Add(resultData)
    '            Dim relation As New DataRelation("CustomerOrders",
    '    ds.Tables(0).Columns("TranID"), ' Parent column
    '    ds.Tables(1).Columns("TranID"), ' Child column
    '    False
    ')
    '            ds.Relations.Add(relation)

    '            Dim newColumn = New DataColumn("PricingTitle", GetType(String), "ISNULL(Parent(CustomerOrders).PricingTitle,'')")
    '            data.Columns.Add(newColumn)
    '            ' Display the data in the DataTable (for demonstration purposes)
    '            For Each row As DataRow In resultData.Rows
    '                Console.WriteLine($"TranID: {row("TranID")}, ItemRefNo: {row("ItemRefNo")}, ItemTitle: {row("ItemTitle")}, SaleRate: {row("SaleRate")}, TransactionDate: {row("TransactionDate")}")
    '            Next

    '            ' Step 6: Drop the temporary table (optional, as it will be dropped automatically when the session ends)
    '            Dim dropTempTableQuery As String = "
    '                DROP TABLE #TempTable;
    '            "
    '            DataAccessUtil.ExecuteNonQuery(dropTempTableQuery, connection)
    '        End Using
    '    End Sub
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
            SELECT dttemp.TranID, dtpl.PricingTitle 
            FROM #TempTable AS dttemp 
            INNER JOIN tblPricingList AS dtpl ON dttemp.TranID = dtpl.ItemID
            WHERE dttemp.SaleRate = 1200.5
        "
            Dim resultData = DataAccessUtil.GetDataAndFillDataTable(operationQuery, connection)

            ' Ensure TranID exists in both tables
            If Not data.Columns.Contains("TranID") OrElse Not resultData.Columns.Contains("TranID") Then
                Throw New Exception("TranID column is missing in one of the tables.")
            End If

            ' Step 6: Create a DataSet and add tables
            Dim ds As New DataSet()
            ds.Tables.Add(data)
            ds.Tables.Add(resultData)

            ' Step 7: Define and add the DataRelation
            Dim relation As New DataRelation("CustomerOrders",
            data.Columns("TranID"),
            resultData.Columns("TranID")
        )
            ds.Relations.Add(relation)

            ' Verify the relation is added
            If ds.Relations.Contains("CustomerOrders") Then
                ' Step 8: Add the new column with the expression
                Dim newColumn = New DataColumn("PricingTitle", GetType(String), "ISNULL(Child(CustomerOrders).PricingTitle,'')")
                data.Columns.Add(newColumn)
            Else
                Throw New Exception("The relation 'CustomerOrders' was not added to the DataSet.")
            End If

            ' Display the data in the DataTable (for demonstration purposes)
            For Each row As DataRow In resultData.Rows
                Console.WriteLine($"TranID: {row("TranID")}, PricingTitle: {row("PricingTitle")}")
            Next

            ' Step 9: Drop the temporary table
            Dim dropTempTableQuery As String = "DROP TABLE #TempTable;"
            DataAccessUtil.ExecuteNonQuery(dropTempTableQuery, connection)
        End Using
    End Sub

End Class

Module Program2
    Public Sub Test()
        ' Step 1: Create a DataSet
        Dim ds As New DataSet()

        ' Step 2: Create Customers table
        Dim customers As New DataTable("Customers")
        customers.Columns.Add("CustomerID", GetType(Integer))
        customers.Columns.Add("CustomerName", GetType(String))
        customers.Columns.Add("Country", GetType(String))

        ' Add data to Customers table
        customers.Rows.Add(101, "John Doe", "USA")
        customers.Rows.Add(102, "Jane Smith", "UK")
        customers.Rows.Add(103, "Emily Davis", "Canada")

        ' Step 3: Create Orders table
        Dim orders As New DataTable("Orders")
        orders.Columns.Add("OrderID", GetType(Integer))
        orders.Columns.Add("CustomerID", GetType(Integer))
        orders.Columns.Add("OrderDate", GetType(DateTime))

        ' Add data to Orders table
        orders.Rows.Add(1, 101, New DateTime(2024, 1, 10))
        orders.Rows.Add(2, 102, New DateTime(2024, 1, 15))
        orders.Rows.Add(3, 101, New DateTime(2024, 2, 5))
        orders.Rows.Add(4, 103, New DateTime(2024, 2, 20))

        ' Step 4: Add tables to the DataSet
        ds.Tables.Add(customers)
        ds.Tables.Add(orders)

        ' Step 5: Define the relationship
        Dim relation As New DataRelation("CustomerOrders",
            customers.Columns("CustomerID"), ' Parent column
            orders.Columns("CustomerID")     ' Child column
        )

        ' Step 6: Add relation to DataSet
        ds.Relations.Add(relation)

        ' Step 7: Fetch Parent Row (Get Customer from Orders)
        Console.WriteLine("Fetching Parent Rows (Get Customer from Orders):")
        For Each orderRow As DataRow In orders.Rows
            Dim customerRow As DataRow = orderRow.GetParentRow("CustomerOrders")
            Console.WriteLine($"Order ID: {orderRow("OrderID")} | Customer: {customerRow("CustomerName")} | Country: {customerRow("Country")}")
        Next
        Console.WriteLine("------------------------------------------------")

        ' Step 8: Fetch Child Rows (Get Orders from Customers)
        Console.WriteLine("Fetching Child Rows (Get Orders from Customers):")
        For Each customerRow As DataRow In customers.Rows
            Dim ordersRows() As DataRow = customerRow.GetChildRows("CustomerOrders")
            Console.WriteLine($"Customer: {customerRow("CustomerName")} has {ordersRows.Length} orders.")
            For Each orderRow As DataRow In ordersRows
                Console.WriteLine($"   Order ID: {orderRow("OrderID")} on {orderRow("OrderDate")}")
            Next
        Next
        Console.WriteLine("------------------------------------------------")

        ' Step 9: Auto-Populate Customer Name in Orders using Expression
        Dim customerNameColumn As New DataColumn("CustomerName", GetType(String), "Parent(CustomerOrders).CustomerName")
        orders.Columns.Add(customerNameColumn)

        Console.WriteLine("Orders Table with Auto-Populated Customer Name:")
        For Each orderRow As DataRow In orders.Rows
            Console.WriteLine($"Order ID: {orderRow("OrderID")} | Customer Name: {orderRow("CustomerName")} | Order Date: {orderRow("OrderDate")}")
        Next
        Console.WriteLine("------------------------------------------------")

        Console.ReadLine()
    End Sub
End Module

Module Program
    Sub Main(args As String())
        Dim test As New Test()
        test.GetDataAndFillDataTable()
        'Program2.Test()
    End Sub
End Module