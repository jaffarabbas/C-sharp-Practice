using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using static DataTableRelation.Program;

namespace DataTableRelation
{
    class Program
    {
        public static void checkDatatable()
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
        public class item
        {
            public int itemID { get; set; }
            public string name { get; set; }
        }
        public class bom
        {
            public int id { get; set; }
            public int itemID { get; set; }
            public int quantity { get; set; }
        }

        public List<bom> boms = new List<bom>()
            {
                new bom()
                {
                    id = 1,
                    itemID = 1,
                    quantity = 2
                },
                new bom()
                {
                    id = 2,
                    itemID = 2,
                    quantity = 2
                },
                new bom()
                {
                    id = 3,
                    itemID = 4,
                    quantity = 2
                },
            };

        public List<item> items = new List<item>()
            {
                new item()
                {
                    itemID = 1,
                    name = "asdasd"
                },new item()
                {
                    itemID = 2,
                    name = "asdasd"
                }
                ,new item()
                {
                    itemID = 3,
                    name = "asdasd"
                },
            };

        public void test()
        {

            //bool allItemsPresent = boms.All(b => item.Any(i => i.itemID == b.itemID));

            //if (allItemsPresent)
            //{
            //    Console.WriteLine("All bom itemIDs are present in the items list.");
            //}
            //else
            //{
            //    Console.WriteLine("Not all bom itemIDs are present in the items list.");
            //}

            List<bom> unmatchedBoms = boms
            .Where(b => !items.Any(i => i.itemID == b.itemID))
            .ToList();

            if (unmatchedBoms.Count > 0)
            {
                Console.WriteLine("The following bom items do not have a corresponding match in the items list:");
                unmatchedBoms.ForEach(unmatchedBom =>
                    Console.WriteLine($"Bom ID: {unmatchedBom.id}, ItemID: {unmatchedBom.itemID}")
                );
            }
            else
            {
                Console.WriteLine("All bom items have a corresponding match in the items list.");
            }
        }
        static void Main()
        {
            Program obj = new Program();
            obj.test();
        }
    }
}
