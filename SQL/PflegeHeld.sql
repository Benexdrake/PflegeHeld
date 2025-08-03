create database PflegeHeld;

go

use [PflegeHeld];

go

create table contact
(
	id varchar(64) not null,
	[name] varchar(128) not null,
	tel varchar(64) null,
	[address] varchar(max) null,
	avatar varchar(max) null,
	is_emergency_contact bit not null,
	Primary Key(id)
);

go

create table caregiver_patient
(
	caregiver_id varchar(64) not null,
	patient_id varchar(64) not null,
	foreign key(caregiver_id) references contact(id),
	foreign key(patient_id) references contact(id)
);

go

create table [product]
(
	id varchar(64) not null,
	[name] varchar(max) not null,
	get_from varchar(64) not null,
	Primary Key(id),
	foreign key(get_from) references contact(id)
);

go

create table medicament
(
	product_id varchar(64) not null,
	dosis varchar(32) not null,
	intake_form varchar(64) not null,
	foreign key(product_id) references product(id)
);

go

create table product_inventory
(
	product_id varchar(64) not null,
	stock int not null,
	foreign key(product_id) references product(id)
);

go

create table documentation
(
	id varchar(64) not null,
	[date] varchar(128) not null,
	title varchar(128) not null,
	[type] varchar(128) not null,
	message text not null,
	Primary Key(id)
);

go

create table document
(
	id varchar(64) not null,
	[name] varchar(128) not null,
	[filename] varchar(128) not null,
	filetype varchar(64) not null,
	Primary Key(id)
);

go

create table documentation_document
(
	documentation_id varchar(64) not null,
	document_id varchar(64) not null,
	foreign key(documentation_id) references documentation(id),
	foreign key(document_id) references document(id),
);

go

create table appointment
(
	id varchar(64) not null,
	[type] varchar(128) not null,
	[date] varchar(64) not null,
	[time] varchar(64) not null,
	contact_id varchar(64) not null,
	Primary Key(id),
	foreign key(contact_id) references contact(id)
);

go

create table timer
(
	id varchar(64) not null,
	[name] varchar(128) not null,
	[start] varchar(64) not null,
	[end] varchar(64) not null,
	interval int not null,
	[description] varchar(max) not null,
	ticker_id varchar(64) not null,
	Primary Key(id)
);