SET NOCOUNT ON;

-- Step 1: Get the current max TranID (if table is empty, default to 0)
DECLARE @StartID BIGINT = ISNULL((SELECT MAX(TranID) FROM tblInvoice), 0);
DECLARE @EndID BIGINT = @StartID + 1000000;

-- Step 2: Generate 1M new invoice records
WITH Numbers AS (
    SELECT TOP (1000000)
        ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS n
    FROM sys.all_objects a
    CROSS JOIN sys.all_objects b
),
RandomizedInvoices AS (
    SELECT 
        n + @StartID + 1 AS TranID,
        CONCAT('REF', FORMAT(n + @StartID + 1, '000000')) AS TranRefNo,
        ABS(CHECKSUM(NEWID())) % 5 + 1 AS ItemID,
        ABS(CHECKSUM(NEWID())) % 5 + 1 AS PricingListID,
        ABS(CHECKSUM(NEWID())) % 10 + 1 AS Quantity,
        0 AS SalePrice
    FROM Numbers
)
INSERT INTO tblInvoice (TranID, TranRefNo, ItemID, PricingListID, Quantity, SalePrice)
SELECT
    TranID,
    TranRefNo,
    ItemID,
    PricingListID,
    Quantity,
    CASE 
        WHEN PricingListID = 1 THEN 1200
        WHEN PricingListID = 2 THEN 800
        WHEN PricingListID = 3 THEN 450
        WHEN PricingListID = 4 THEN 300
        WHEN PricingListID = 5 THEN 200
        ELSE 0
    END AS SalePrice
FROM RandomizedInvoices;
