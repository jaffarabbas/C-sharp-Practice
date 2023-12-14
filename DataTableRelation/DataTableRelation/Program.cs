using System;
using System.Data;
using System.Linq;

namespace DataTableRelation
{
    class Program
    {
        static void Main()
        {
            // Create DataTables
            DataTable itemsTable = new DataTable("Items");
            itemsTable.Columns.Add("ItemId", typeof(int));
            itemsTable.Columns.Add("Quantity", typeof(int));

            DataTable detailsTable = new DataTable("Details");
            detailsTable.Columns.Add("ItemId", typeof(int));
            detailsTable.Columns.Add("ItemName", typeof(string));

            // Populate DataTables with sample data
            itemsTable.Rows.Add(1, 5);
            itemsTable.Rows.Add(2, 3);
            itemsTable.Rows.Add(4, 7);

            detailsTable.Rows.Add(1, "Apple");
            detailsTable.Rows.Add(2, "Banana");
            detailsTable.Rows.Add(4, "Orange");

            // Perform the operation using LINQ
            DataTable resultTable = JoinTables(itemsTable, detailsTable);

            // Display the result
            foreach (DataRow row in resultTable.Rows)
            {
                Console.WriteLine($"ItemId: {row["ItemId"]}, Quantity: {row["Quantity"]}, ItemName: {(row["ItemName"] == DBNull.Value ? "null" : row["ItemName"])}");
            }
        }

        static DataTable JoinTables(DataTable itemsTable, DataTable detailsTable)
        {
            // Use LINQ to perform the join operation
            var query = from itemsRow in itemsTable.AsEnumerable()
                        join detailsRow in detailsTable.AsEnumerable() on itemsRow.Field<int>("ItemId") equals detailsRow.Field<int>("ItemId") into gj
                        from subDetailsRow in gj.DefaultIfEmpty()
                        select new
                        {
                            ItemId = itemsRow.Field<int>("ItemId"),
                            Quantity = itemsRow.Field<int>("Quantity"),
                            ItemName = subDetailsRow?.Field<string>("ItemName")
                        };

            // Convert the LINQ query result to a DataTable
            DataTable resultTable = new DataTable("Result");
            resultTable.Columns.Add("ItemId", typeof(int));
            resultTable.Columns.Add("Quantity", typeof(int));
            resultTable.Columns.Add("ItemName", typeof(string));

            foreach (var item in query)
            {
                DataRow resultRow = resultTable.NewRow();
                resultRow["ItemId"] = item.ItemId;
                resultRow["Quantity"] = item.Quantity;
                resultRow["ItemName"] = item.ItemName == null ? DBNull.Value : (object)item.ItemName;
                resultTable.Rows.Add(resultRow);
            }

            return resultTable;
        }
    }
}
