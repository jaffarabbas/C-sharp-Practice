use BikeStores
select * from [sales].[order_items]

select min(list_price) from [sales].[order_items] where list_price in 
(select top 3 max(list_price) as list from [sales].[order_items] group by list_price order by list_price desc)