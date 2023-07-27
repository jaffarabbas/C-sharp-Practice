--create database dbMvc

use dbMvc

create table customer
(
id int identity(1,1) primary key,
name varchar(40) not null,
email varchar(100) not null unique,
password varchar(30) not null
)

create table item
(
itemId int identity(1,1) primary key,
iname varchar(30) not  null,
idescription varchar(100) not null,
price decimal(10,2) null,
quantity int null,
)

create table sales
(
 sid int identity(1,1) primary key,
 sdate datetime not null,
 cid int not null,
 iid int not null
)

alter table sales
add constraint fk_sales_customer foreign key (cid) references customer(id)


alter table sales
add constraint fk_sales_item foreign key (iid) references item(itemId)

