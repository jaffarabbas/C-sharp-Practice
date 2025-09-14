select * from tblItem

select * from tblPricingList

select tblItem.*,tblPricingList.SaleRate from tblItem 
inner join tblPricingList on tblItem.TranID = tblPricingList.ItemID

select * from tblInvoice

create table tblInvoice
(
	TranID bigint not null default(-1) primary key,
	TranRefNo varchar(100) not null default(''),
	ItemID bigint not null default(0),
	PricingListID bigint not null default(0),
	Quantity bigint not null default(0),
	SalePrice bigint not null default(0),
	CreationDate datetime not null default(getdate()),
)
